using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ODataPad.Core;
using ODataPad.WinRT;
using ODataPad.UI.WinRT.Common;

namespace ODataPad.UI.WinRT
{
    sealed partial class App : Application
    {
        public static ODataPadApp theApp { get; private set; }
        public const uint RequestedVersion = 3;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            theApp = new ODataPadApp(
                new ServiceLocalStorage(),
                new ResourceManager(),
                "Samples", 
                "SampleServices.xml");

            LoadAppDataAsync().Wait();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                    }
                }

                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                if (!rootFrame.Navigate(typeof(MainPage), ""))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            Window.Current.Activate();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        private async Task<bool> LoadAppDataAsync()
        {
            await ApplicationData.Current.SetVersionAsync(RequestedVersion, SetVersionHandlerAsync);
            await theApp.InitializeODataServicesAsync();
            return true;
        }

        private async void SetVersionHandlerAsync(SetVersionRequest request)
        {
            SetVersionDeferral deferral = request.GetDeferral();
            await theApp.SetVersionAsync((int)request.CurrentVersion, (int)RequestedVersion);
            deferral.Complete();
        }
    }
}
