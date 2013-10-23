using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Services;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core
{
    public class ODataPadApp 
        : MvxApplication
    {
        public const int ApplicationDataVersion = 3;
        private readonly string _samplesModuleName;
        private readonly string _samplesFilename;
        private static readonly List<Type> _viewModelsWithOwnViews = new List<Type>(new[] { typeof(HomeViewModel) });

        public ODataPadApp(string samplesModuleName, string samplesFilename)
        {
            _samplesModuleName = samplesModuleName;
            _samplesFilename = samplesFilename;

            InitalizeServices();
            InitializeStartNavigation();
            InitializePlugIns();
        }

        public HomeViewModel HomeViewModel { get; private set; }
        public static List<Type> ViewModelsWithOwnViews { get { return _viewModelsWithOwnViews; }} 

        private void InitalizeServices()
        {
            Mvx.RegisterSingleton<IServiceRepository>(
                new ServiceRepository());
            Mvx.RegisterSingleton<ISamplesService>(
                new SamplesService(_samplesModuleName, _samplesFilename));
            Mvx.RegisterSingleton<IDataVersioningService>(
                new DataVersioningService());
            Mvx.RegisterSingleton<IODataService>(
                new ODataService());
        }

        private void InitializeStartNavigation()
        {
            var startApplicationObject = new AppStart();
            Mvx.RegisterSingleton<IMvxAppStart>(startApplicationObject);
        }

        private void InitializePlugIns()
        {
        }
    }
}