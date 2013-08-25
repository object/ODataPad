﻿using System.Collections.Generic;
using System.IO;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Test.Core;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;
using ODataPad.Platform.Net45;

namespace ODataPad.Specifications
{
    public class AppDriver : MvxIoCSupportingTest
    {
        public void Initialize()
        {
            base.ClearAll();

            InitializeServices();
        }

        public IMvxApplication CreateApp()
        {
            return new ODataPadApp("ODataPad.Samples", "SampleServices.xml");
        }

        private void InitializeServices()
        {
            Mvx.RegisterSingleton<IResourceManager>(new ResourceManager());
            Mvx.RegisterSingleton<IServiceLocalStorage>(new ServiceLocalStorage());
            Mvx.RegisterSingleton<IApplicationLocalData>(new ApplicationLocalData());
            Mvx.RegisterSingleton<IResultProvider>(new ResultProvider());
        }

        public void CreateServices(IEnumerable<string> serviceNames)
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
    }
}
