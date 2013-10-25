﻿using System;
using System.Windows;
using System.Windows.Threading;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Wpf.Platform;
using Cirrious.MvvmCross.Wpf.Views;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using ODataPad.Core.ViewModels;
using ODataPad.Platform.Net45;

namespace ODataPad.UI.Net45
{
    public class Setup : MvxWpfSetup
    {
        public Setup(Dispatcher dispatcher, IMvxWpfViewPresenter presenter)
            : base(dispatcher, presenter)
        {
        }

        protected override void InitializePlatformServices()
        {
            base.InitializePlatformServices();

            Mvx.RegisterSingleton<IResourceManager>(new ResourceManager());
            Mvx.RegisterSingleton<IServiceLocalStorage>(new ServiceLocalStorage());
            Mvx.RegisterSingleton<IApplicationLocalData>(new ApplicationLocalData());
            Mvx.RegisterSingleton<IResultProvider>(new ResultProvider());
        }

        protected override IMvxApplication CreateApp()
        {
#if FORMFACTOR_PHONE
            AppState.ViewModelsWithOwnViews.AddRange(new[]
            {
                typeof (ServiceDetailsViewModel), 
                typeof (ResourceSetDetailsViewModel), 
                typeof (ResultDetailsViewModel)
            });
#endif
            return new ODataPadApp("ODataPad.Samples", "SampleServices.xml");
        }
    }
}
