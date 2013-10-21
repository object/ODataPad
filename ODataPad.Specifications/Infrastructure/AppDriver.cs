using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Test.Core;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Views;
using Moq;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;
using ODataPad.Platform.Net45;
using ODataPad.Specifications.Mocks;

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
            InitializeApplicationServices();
            InitializeMvxServices();
        }

        public IMvxApplication CreateApp()
        {
            var app = new ODataPadApp("ODataPad.Samples", "SampleServices.xml");

            if (this.MocksEnabled)
                MockApplicationServices();

            return app;
        }

        public void CreateODataServices(IEnumerable<string> serviceNames)
        {
            var storage = Ioc.GetSingleton<IServiceLocalStorage>();
            var services = new List<ServiceInfo>();
            foreach (var serviceName in serviceNames)
            {
                services.Add(new ServiceInfo() { Name = serviceName });
            }
            storage.SaveServiceInfosAsync(services).Wait();
        }

        public void ClearData()
        {
            Ioc.GetSingleton<IServiceLocalStorage>().ClearServiceInfosAsync().Wait();
        }

        public void SetDataVersion(int versionNumber)
        {
            var localData = Ioc.GetSingleton<IApplicationLocalData>();
            localData.SetDataVersionAsync(versionNumber).Wait();
        }

        private void InitializeMvxServices()
        {
            Ioc.RegisterSingleton<IMvxTrace>(new TestTrace());

            var dispatcher = new MockMvxViewDispatcher();
            Ioc.RegisterSingleton<IMvxMainThreadDispatcher>(dispatcher);
            Ioc.RegisterSingleton<IMvxViewDispatcher>(dispatcher);
        }

        private void InitializeApplicationServices()
        {
            Ioc.RegisterSingleton<IMvxTrace>(new TestTrace());

            Ioc.RegisterSingleton<IResourceManager>(new ResourceManager());
            Ioc.RegisterSingleton<IServiceLocalStorage>(new ServiceLocalStorage());
            Ioc.RegisterSingleton<IApplicationLocalData>(new ApplicationLocalData());
            Ioc.RegisterSingleton<IResultProvider>(new ResultProvider());
        }

        private void MockApplicationServices()
        {
            _mockODataService = new Mock<IODataService>();
            _mockODataService.Setup(x => x
                .LoadResultsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<INotifyInProgress>()))
                .Returns(new MockODataService().GetQueryResultAsync);
            Ioc.RegisterSingleton(_mockODataService.Object);
        }

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
