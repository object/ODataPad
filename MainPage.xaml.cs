﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ODataPad.Common;
using ODataPad.DataModel;
using Simple.OData.Client;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace ODataPad
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for the
    /// currently selected item.
    /// </summary>
    public sealed partial class MainPage : ODataPad.Common.LayoutAwarePage
    {
        private ServiceDataItem _editedItem;
        private bool _movingToFirst = false;

        public MainPage()
        {
            this.InitializeComponent();
        }

        #region Page state management

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var item = DataSource.Instance.GetItem(navigationParameter.ToString());
            if (item == null)
                return;

            this.DefaultViewModel["Item"] = item;
            this.DefaultViewModel["ItemElements"] = item.Elements;

            if (pageState == null)
            {
                this.itemListView.SelectedItem = null;
                _movingToFirst = true;
                this.itemsViewSource.View.MoveCurrentToFirst();
                _movingToFirst = false;
            }
            else
            {
                // Restore the previously saved state associated with this page
                if (pageState.ContainsKey("SelectedItem") && this.itemsViewSource.View != null)
                {
                    var selectedItem = DataSource.Instance.GetItem((String)pageState["SelectedItem"]);
                    this.itemsViewSource.View.MoveCurrentTo(selectedItem);
                }
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (this.itemsViewSource.View != null)
            {
                var selectedItem = (ServiceDataItem)this.itemsViewSource.View.CurrentItem;
                if (selectedItem != null) pageState["SelectedItem"] = selectedItem.UniqueId;
            }
        }

        #endregion

        #region Logical page navigation

        // Visual state management typically reflects the four application view states directly
        // (full screen landscape and portrait plus snapped and filled views.)  The split page is
        // designed so that the snapped and portrait view states each have two distinct sub-states:
        // either the item list or the details are displayed, but not both at the same time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed, or null
        /// for the current view state.  This parameter is optional with null as the default
        /// value.</param>
        /// <returns>True when the view state in question is portrait or snapped, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null) viewState = ApplicationView.Value;
            return viewState == ApplicationViewState.FullScreenPortrait ||
                viewState == ApplicationViewState.Snapped;
        }

        /// <summary>
        /// Invoked when the page's back button is pressed.
        /// </summary>
        /// <param name="sender">The back button instance.</param>
        /// <param name="e">Event data that describes how the back button was clicked.</param>
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return
                // to the item list.  From the user's point of view this is a logical backward
                // navigation.
                this.itemListView.SelectedItem = null;
            }
            else
            {
                // When logical page navigation is not in effect, or when there is no selected
                // item, use the default back button behavior.
                base.GoBack(sender, e);
            }
        }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed.</param>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            // Update the back button's enabled state when the view state changes
            var logicalPageBack = this.UsingLogicalPageNavigation(viewState) && this.itemListView.SelectedItem != null;
            var physicalPageBack = this.Frame != null && this.Frame.CanGoBack;
            this.DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

            // Determine visual states for landscape layouts based not on the view state, but
            // on the width of the window.  This page has one layout that is appropriate for
            // 1366 virtual pixels or wider, and another for narrower displays or when a snapped
            // application reduces the horizontal space available to less than 1366.
            if (viewState == ApplicationViewState.Filled ||
                viewState == ApplicationViewState.FullScreenLandscape)
            {
                var windowWidth = Window.Current.Bounds.Width;
                if (windowWidth >= 1366) return "FullScreenLandscapeOrWide";
                return "FilledOrNarrow";
            }

            // When in portrait or snapped start with the default visual state name, then add a
            // suffix when viewing details instead of the list
            var defaultStateName = base.DetermineVisualState(viewState);
            return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
        }

        #endregion

        private void bottomAppBar_Opened(object sender, object e)
        {
            var buttons = this.leftStackPanel.Children;
            foreach (var button in buttons)
            {
                button.Visibility = this.itemListView.SelectedItem == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void bottomAppBar_Closed(object sender, object e)
        {
        }

        void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();

            this.bottomAppBar.IsOpen = e.AddedItems.Count > 0 && !_movingToFirst;
        }

        private void ItemCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            this.bottomAppBar.IsOpen = false;
            _editedItem = null;

            this.serviceName.Text = string.Empty;
            this.serviceUrl.Text = string.Empty;
            this.serviceDescription.Text = string.Empty;
            this.editPopup.IsOpen = true;
            RefreshSaveButtonState();
        }

        private async void removeButton_Click(object sender, RoutedEventArgs e)
        {
            this.bottomAppBar.IsOpen = false;
            bool ok = await ServiceCanBeRemoved();
            if (ok)
            {
                var dispatcher = Window.Current.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, RemoveServiceAsync);
            }
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            this.bottomAppBar.IsOpen = false;
            _editedItem = this.itemListView.SelectedItem as ServiceDataItem;
            this.serviceName.Text = _editedItem.Title;
            this.serviceUrl.Text = _editedItem.Subtitle;
            this.serviceDescription.Text = _editedItem.Description;
            this.editPopup.IsOpen = true;
            RefreshSaveButtonState();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.bottomAppBar.IsOpen = false;
        }

        private void dataButton_Click(object sender, RoutedEventArgs e)
        {
            this.bottomAppBar.IsOpen = false;
            var selectedCollection = itemCollection.SelectedItem as ServiceDataItem;
            this.Frame.Navigate(typeof(ItemDetailPage), selectedCollection.UniqueId);
        }

        private void editBackButton_Click(object sender, RoutedEventArgs e)
        {
            this.editPopup.IsOpen = false;
        }

        private async void editSaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool ok = _editedItem == null ? await ServiceCanBeAdded() : await ServiceCanBeUpdated();
            if (ok)
            {
                DispatchedHandler action;
                if (_editedItem == null)
                    action = AddServiceAsync;
                else
                    action = UpdateServiceAsync;
                var dispatcher = Window.Current.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);

                this.editPopup.IsOpen = false;
            }
        }

        private void serviceName_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshSaveButtonState();
        }

        private void serviceUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshSaveButtonState();
        }

        private void serviceDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshSaveButtonState();
        }

        private void RefreshSaveButtonState()
        {
            this.editSaveButton.IsEnabled =
                !string.IsNullOrEmpty(this.serviceName.Text) &&
                !string.IsNullOrEmpty(this.serviceUrl.Text) &&
                !string.IsNullOrEmpty(this.serviceDescription.Text);
        }

        private async Task<bool> ServiceCanBeAdded()
        {
            var item = DataSource.Instance.GetItem(this.serviceName.Text);
            if (item != null)
            {
                var dialog = new MessageDialog("A service with this name already exists.");
                await dialog.ShowAsync();
                return false;
            }
            else if (!IsValidUrl(this.serviceUrl.Text))
            {
                var dialog = new MessageDialog("Invalid service URL.");
                await dialog.ShowAsync();
                return false;
            }
            return true;
        }

        private async Task<bool> ServiceCanBeUpdated()
        {
            if (!IsValidUrl(this.serviceUrl.Text))
            {
                var dialog = new MessageDialog("Invalid service URL.");
                await dialog.ShowAsync();
                return false;
            }
            return true;
        }

        private async Task<bool> ServiceCanBeRemoved()
        {
            var dialog = new MessageDialog("Are you sure you want to delete this service?");
            dialog.Commands.Add(new UICommand("Yes", command => { }));
            dialog.Commands.Add(new UICommand("No", command => { }));
            var cmd = await dialog.ShowAsync();
            return cmd.Label == "Yes";
        }

        private bool IsValidUrl(string url)
        {
            return Regex.IsMatch(url, @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
        }

        private async void AddServiceAsync()
        {
            var serviceInfo = new ServiceInfo()
            {
                Name = this.serviceName.Text,
                Url = this.serviceUrl.Text,
                Description = this.serviceDescription.Text,
                Logo = "Custom"
            };
            App.AppData.AddService(serviceInfo);
            await DataSource.Instance.AddServiceDataItemAsync(serviceInfo);
        }

        private async void UpdateServiceAsync()
        {
            var serviceInfo = new ServiceInfo()
            {
                Name = this.serviceName.Text,
                Url = this.serviceUrl.Text,
                Description = this.serviceDescription.Text
            };
            if (_editedItem.Subtitle != serviceInfo.Url)
            {
                serviceInfo.MetadataCache = null;
            }
            App.AppData.UpdateService(_editedItem.Title, serviceInfo);
            await DataSource.Instance.UpdateServiceDataItemAsync(_editedItem, serviceInfo);
        }

        private async void RemoveServiceAsync()
        {
            var dataItem = this.itemListView.SelectedItem as ServiceDataItem;
            var serviceInfo = new ServiceInfo()
            {
                Name = dataItem.Title,
            };
            App.AppData.DeleteService(serviceInfo);
            await DataSource.Instance.RemoveServiceDataItemAsync(dataItem);
        }
    }
}
