using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.Interfaces.ViewModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ODataPad.UI.WinRT
{
    sealed partial class App : Application, IMvxServiceConsumer<IMvxStartNavigation>
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;
            
            if (rootFrame == null)
            {
                rootFrame = new Frame();
//                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                    //try
                    //{
                    //    await SuspensionManager.RestoreAsync();
                    //}
                    //catch (SuspensionManagerException)
                    //{
                    //}
                }

                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                //if (!rootFrame.Navigate(typeof(MainPage), ""))
                //{
                //    throw new Exception("Failed to create initial page");
                //}
                var setup = new Setup(rootFrame);
                setup.Initialize();

                var start = this.GetService<IMvxStartNavigation>();
                start.Start();
            }
            Window.Current.Activate();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            //await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
