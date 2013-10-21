using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Test.Core;
using Cirrious.MvvmCross.ViewModels;
using Moq;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;
using ODataPad.Platform.Net45;

namespace ODataPad.Specifications.Infrastructure
{
    public class AppDriver : MvxIoCSupportingTest
    {
        // ReSharper disable once InconsistentNaming
        private static readonly AppDriver _instance = new AppDriver();
        public static AppDriver Instance { get { return _instance; } }
        
        public bool MocksEnabled { get; private set; }

        private Mock<IODataService> _mockODataService;

        public void Initialize()
        {
            base.ClearAll();

            this.MocksEnabled = true;
            InitializeServices();
        }

        public IMvxApplication CreateApp()
        {
            var app = new ODataPadApp("ODataPad.Samples", "SampleServices.xml");

            if (this.MocksEnabled)
                MockServices();

            return app;
        }

        private void InitializeServices()
        {
            Mvx.RegisterSingleton<IMvxTrace>(new TestTrace());

            Mvx.RegisterSingleton<IResourceManager>(new ResourceManager());
            Mvx.RegisterSingleton<IServiceLocalStorage>(new ServiceLocalStorage());
            Mvx.RegisterSingleton<IApplicationLocalData>(new ApplicationLocalData());
            Mvx.RegisterSingleton<IResultProvider>(new ResultProvider());
        }

        private void MockServices()
        {
            _mockODataService = new Mock<IODataService>();
            _mockODataService.Setup(x => x
                .LoadResultsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<INotifyInProgress>()))
                .Returns(new FakeODataService().GetQueryResultAsync);
            Mvx.RegisterSingleton(_mockODataService.Object);
        }

        public void CreateODataServices(IEnumerable<string> serviceNames)
        {
            var storage = Mvx.GetSingleton<IServiceLocalStorage>();
            var services = new List<ServiceInfo>();
            foreach (var serviceName in serviceNames)
            {
                services.Add(new ServiceInfo() { Name = serviceName });
            }
            storage.SaveServiceInfosAsync(services).Wait();
        }

        public void ClearData()
        {
            Mvx.GetSingleton<IServiceLocalStorage>().ClearServiceInfosAsync().Wait();
        }

        public void SetDataVersion(int versionNumber)
        {
            var localData = Mvx.GetSingleton<IApplicationLocalData>();
            localData.SetDataVersionAsync(versionNumber).Wait();
        }

        //public void EnsureHomeViewModel()
        //{
        //    ScenarioContext.Current.GetOrAdd("HomeViewModel", () =>
        //    {
        //        var viewModel = new HomeViewModel();
        //        viewModel.Init(null).Wait();
        //        return viewModel;
        //    });
        //}

        private class TestTrace : IMvxTrace
        {
            public void Trace(MvxTraceLevel level, string tag, Func<string> message)
            {
                Debug.WriteLine(tag + ":" + level + ":" + message());
            }

            public void Trace(MvxTraceLevel level, string tag, string message)
            {
                Debug.WriteLine(tag + ": " + level + ": " + message);
            }

            public void Trace(MvxTraceLevel level, string tag, string message, params object[] args)
            {
                Debug.WriteLine(tag + ": " + level + ": " + message, args);
            }
        }
    }
}
