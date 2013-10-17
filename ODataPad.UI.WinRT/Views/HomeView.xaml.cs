using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;

namespace ODataPad.UI.WinRT.Views
{
    public sealed partial class MainPage : ODataPad.UI.WinRT.Common.LayoutAwarePage
    {
        private bool _movingToFirst = false;

        public MainPage()
        {
            this.InitializeComponent();

            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
        }

        public new HomeViewModel ViewModel
        {
            get { return (HomeViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        #region Page state management

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (this.ViewModel == null)
                return;
            var item = this.ViewModel.Services.Items.Where(x => x.Name == navigationParameter.ToString());
            if (item == null)
                return;

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
                    var selectedItem = this.ViewModel.Services.Items.Single(x => x.Name == (String)pageState["SelectedItem"]);
                    this.itemsViewSource.View.MoveCurrentTo(selectedItem);
                }
            }
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (this.itemsViewSource.View != null)
            {
                var selectedItem = (ServiceDetailsViewModel)this.itemsViewSource.View.CurrentItem;
                if (selectedItem != null) pageState["SelectedItem"] = selectedItem.Name;
            }
        }

        #endregion

        #region Logical page navigation
        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null) viewState = ApplicationView.Value;
            return viewState == ApplicationViewState.FullScreenPortrait ||
                viewState == ApplicationViewState.Snapped;
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                this.itemListView.SelectedItem = null;
            }
            else
            {
                base.GoBack(sender, e);
            }
        }

        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            var logicalPageBack = this.UsingLogicalPageNavigation(viewState) && this.itemListView.SelectedItem != null;
            var physicalPageBack = this.Frame != null && this.Frame.CanGoBack;
            this.DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

            if (viewState == ApplicationViewState.Filled ||
                viewState == ApplicationViewState.FullScreenLandscape)
            {
                var windowWidth = Window.Current.Bounds.Width;
                if (windowWidth >= 1366) return "FullScreenLandscapeOrWide";
                return "FilledOrNarrow";
            }

            var defaultStateName = base.DetermineVisualState(viewState);
            return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
        }

        #endregion

        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var cmdWebSite = new SettingsCommand("WebSite", "ODataPad on GitHub",
                (x) => Windows.System.Launcher.LaunchUriAsync(new Uri("http://object.github.com/ODataPad/index.html")));
            var cmdPrivacyPolicy = new SettingsCommand("PrivacePolicy", "Privacy Policy",
                (x) => Windows.System.Launcher.LaunchUriAsync(new Uri("http://object.github.com/ODataPad/privacy_policy.html")));

            args.Request.ApplicationCommands.Add(cmdWebSite);
            args.Request.ApplicationCommands.Add(cmdPrivacyPolicy);
        }

        //void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();

        //    this.bottomAppBar.IsOpen = e.AddedItems.Count > 0 && !_movingToFirst;
        //}

        //private void ItemCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //if (e.RemovedItems.Count == 1)
        //{
        //    var collection = e.RemovedItems.First() as ServiceCollection;
        //    collection.QueryResults = null;
        //}
        //if (e.AddedItems.Count == 1)
        //{
        //    var collection = e.AddedItems.First() as ServiceCollection;
        //    if (this.collectionMode.SelectedIndex == 0)
        //        collection.QueryResults = null;
        //    else
        //        RequestCollectionData(collection);
        //}
        //}

        //private void addButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //this.bottomAppBar.IsOpen = false;
        //    _editedItem = null;

        //    this.serviceName.Text = string.Empty;
        //    this.serviceUrl.Text = string.Empty;
        //    this.serviceDescription.Text = string.Empty;
        //    this.editPopup.IsOpen = true;
        //    RefreshSaveButtonState();
        //}

        //private async void removeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    this.bottomAppBar.IsOpen = false;
        //    bool ok = await ServiceCanBeRemoved();
        //    if (ok)
        //    {
        //        var dispatcher = Window.Current.Dispatcher;
        //        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, RemoveServiceAsync);
        //    }
        //}

        //private void editButton_Click(object sender, RoutedEventArgs e)
        //{
        //    this.bottomAppBar.IsOpen = false;
        //    _editedItem = this.itemListView.SelectedItem as ServiceDetailsViewModel;
        //    this.serviceName.Text = _editedItem.Name;
        //    this.serviceUrl.Text = _editedItem.Url;
        //    this.serviceDescription.Text = _editedItem.Description;
        //    this.editPopup.IsOpen = true;
        //    RefreshSaveButtonState();
        //}

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.bottomAppBar.IsOpen = false;
        }

        //private async void editSaveButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var ok = await ServiceHasValidUrl();
        //    if (ok && _editedItem == null) ok = await ServiceHasUniqueName();
        //    if (!ok) return;

        //    DispatchedHandler action;
        //    if (_editedItem == null)
        //        action = AddServiceAsync;
        //    else
        //        action = UpdateServiceAsync;
        //    var dispatcher = Window.Current.Dispatcher;
        //    /*await*/
        //    dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);

        //    this.editPopup.IsOpen = false;
        //}

        private async void AddServiceAsync(ServiceEditViewModel service)
        {
            var serviceInfo = new ServiceInfo()
            {
                Name = service.Name,
                Url = service.Url,
                Description = service.Description,
                Logo = "Custom",
            };
            await this.ViewModel.AddServiceItemAsync(serviceInfo);
        }

        private async void UpdateServiceAsync(ServiceEditViewModel service)
        {
            var serviceInfo = new ServiceInfo()
            {
                Name = service.Name,
                Url = service.Url,
                Description = service.Description,
            };
            serviceInfo.MetadataCache = null;
            await this.ViewModel.UpdateServiceItemAsync(service.SourceService, serviceInfo);
        }

        private async void RemoveServiceAsync()
        {
            var item = this.itemListView.SelectedItem as ServiceDetailsViewModel;
            await this.ViewModel.RemoveServiceItemAsync(item);
        }
    }
}
