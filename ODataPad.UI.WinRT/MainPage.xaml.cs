using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ODataPad.Core.ViewModels;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ODataPad.Core.Models;

namespace ODataPad.UI.WinRT
{
    public sealed partial class MainPage : ODataPad.UI.WinRT.Common.LayoutAwarePage
    {
        private ServiceItem _editedItem;
        private bool _movingToFirst = false;

        public MainPage()
        {
            this.InitializeComponent();

            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
            App.theApp.HomeViewModel.PopulateAsync().Wait();
        }

        #region Page state management

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var item = App.theApp.HomeViewModel.Services.Where(x => x.Name == navigationParameter.ToString());
            if (item == null)
                return;

            this.DefaultViewModel["Item"] = item;
            this.DefaultViewModel["ItemElements"] = App.theApp.HomeViewModel.Services;

            if (pageState == null)
            {
                this.itemListView.SelectedItem = null;
                _movingToFirst = true;
                this.itemsViewSource.View.MoveCurrentToFirst();
                _movingToFirst = false;
            }
            else
            {
                if (pageState.ContainsKey("SelectedItem") && this.itemsViewSource.View != null)
                {
                    var selectedItem = App.theApp.HomeViewModel.Services.Single(x => x.Name == (String)pageState["SelectedItem"]);
                    this.itemsViewSource.View.MoveCurrentTo(selectedItem);
                }
            }
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (this.itemsViewSource.View != null)
            {
                var selectedItem = (ServiceItem)this.itemsViewSource.View.CurrentItem;
                if (selectedItem != null) pageState["SelectedItem"] = selectedItem.Name;
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

        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var cmdWebSite = new SettingsCommand("WebSite", "ODataPad on GitHub", 
                (x) => Windows.System.Launcher.LaunchUriAsync(new Uri("http://object.github.com/ODataPad/index.html")));
            var cmdPrivacyPolicy = new SettingsCommand("PrivacePolicy", "Privacy Policy", 
                (x) => Windows.System.Launcher.LaunchUriAsync(new Uri("http://object.github.com/ODataPad/privacy_policy.html")));

            args.Request.ApplicationCommands.Add(cmdWebSite);
            args.Request.ApplicationCommands.Add(cmdPrivacyPolicy);
        }
        void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();

            this.bottomAppBar.IsOpen = e.AddedItems.Count > 0 && !_movingToFirst;
        }

        private void ItemCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 1)
            {
                var collection = e.RemovedItems.First() as ServiceCollection;
                collection.QueryResults = null;
            }
            if (e.AddedItems.Count == 1)
            {
                var collection = e.AddedItems.First() as ServiceCollection;
                if (this.collectionMode.SelectedIndex == 0)
                    collection.QueryResults = null;
                else
                    RequestCollectionData(collection);
            }
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
            _editedItem = this.itemListView.SelectedItem as ServiceItem;
            this.serviceName.Text = _editedItem.Name;
            this.serviceUrl.Text = _editedItem.Url;
            this.serviceDescription.Text = _editedItem.Description;
            this.editPopup.IsOpen = true;
            RefreshSaveButtonState();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.bottomAppBar.IsOpen = false;
        }

        private void editBackButton_Click(object sender, RoutedEventArgs e)
        {
            this.editPopup.IsOpen = false;
        }

        private async void editSaveButton_Click(object sender, RoutedEventArgs e)
        {
            var ok = await ServiceHasValidUrl();
            if (ok && _editedItem == null) ok = await ServiceHasUniqueName();
            if (!ok) return;

            DispatchedHandler action;
            if (_editedItem == null)
                action = AddServiceAsync;
            else
                action = UpdateServiceAsync;
            var dispatcher = Window.Current.Dispatcher;
            /*await*/
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);

            this.editPopup.IsOpen = false;
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

        private void collectionMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.collectionMode == null)
                return;

            if (this.collectionMode.SelectedIndex == 0)
            {
                this.itemProperties.Visibility = Visibility.Visible;
                this.itemData.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.itemProperties.Visibility = Visibility.Collapsed;
                this.itemData.Visibility = Visibility.Visible;
                if (this.itemCollection.SelectedItem != null)
                {
                    RequestCollectionData(this.itemCollection.SelectedItem as ServiceCollection);
                }
            }
        }

        private void itemData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.itemData.SelectedItem != null)
            {
                this.itemData.Visibility = Visibility.Collapsed;

                var row = this.itemData.SelectedItem as ResultRow;
                var properties = string.Join(Environment.NewLine + Environment.NewLine,
                    row.Properties
                        .Select(y => y.Key + Environment.NewLine + (y.Value == null ? "(null)" : y.Value.ToString())));

                var block = new Paragraph();
                block.Inlines.Add(new Run() { Text = properties });
                this.itemText.Blocks.Add(block);

                this.itemText.Visibility = Visibility.Visible;
            }
        }

        private void itemText_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.itemText.Blocks.Clear();
            this.itemText.Visibility = Visibility.Collapsed;

            this.itemData.SelectedItem = null;
            this.itemData.Visibility = Visibility.Visible;
            this.itemData.Focus(FocusState.Pointer);
        }

        private void RefreshSaveButtonState()
        {
            this.editSaveButton.IsEnabled =
                !string.IsNullOrEmpty(this.serviceName.Text) &&
                !string.IsNullOrEmpty(this.serviceUrl.Text) &&
                !string.IsNullOrEmpty(this.serviceDescription.Text);
        }

        private async Task<bool> ServiceHasUniqueName()
        {
            var item = App.theApp.HomeViewModel.Services.Where(x => x.Name == this.serviceName.Text);
            if (item != null)
            {
                var dialog = new MessageDialog("A service with this name already exists.");
                await dialog.ShowAsync();
                this.editPopup.IsOpen = true;
                return false;
            }
            return true;
        }

        private async Task<bool> ServiceHasValidUrl()
        {
            if (!IsValidUrl(this.serviceUrl.Text))
            {
                var dialog = new MessageDialog("Invalid service URL.");
                await dialog.ShowAsync();
                this.editPopup.IsOpen = true;
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
            return Regex.IsMatch(url, @"((http|https)://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
        }

        private async void AddServiceAsync()
        {
            var serviceInfo = new ServiceInfo()
            {
                Name = this.serviceName.Text,
                Url = this.serviceUrl.Text,
                Description = this.serviceDescription.Text,
                Logo = "Custom",
            };
            await App.theApp.HomeViewModel.AddServiceItemAsync(serviceInfo);
        }

        private async void UpdateServiceAsync()
        {
            var serviceInfo = new ServiceInfo()
            {
                Name = this.serviceName.Text,
                Url = this.serviceUrl.Text,
                Description = this.serviceDescription.Text,
                Logo = Path.GetFileNameWithoutExtension(_editedItem.ImagePath),
            };
            serviceInfo.MetadataCache = null;
            await App.theApp.HomeViewModel.UpdateServiceItemAsync(_editedItem, serviceInfo);
        }

        private async void RemoveServiceAsync()
        {
            var item = this.itemListView.SelectedItem as ServiceItem;
            await App.theApp.HomeViewModel.RemoveServiceItemAsync(item);
        }

        private void RequestCollectionData(ServiceCollection serviceCollection)
        {
            if (serviceCollection.QueryResults == null)
            {
                serviceCollection.QueryResults = new ObservableResultCollection(
                    (this.itemListView.SelectedItem as ServiceItem).Url,
                    serviceCollection.Name, serviceCollection.Properties, this);
            }
        }

        public void EnableResultProgressBar(bool enabled)
        {
            this.resultProgress.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
