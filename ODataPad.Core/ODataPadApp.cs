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
        public const int ApplicationDataVersion = 2;
        private readonly string _samplesModuleName;
        private readonly string _samplesFilename;

        public ODataPadApp(string samplesModuleName, string samplesFilename)
        {
            _samplesModuleName = samplesModuleName;
            _samplesFilename = samplesFilename;

            InitalizeServices();
            InitializeStartNavigation();
            InitializePlugIns();
        }

        public HomeViewModel HomeViewModel { get; private set; }

        private void InitalizeServices()
        {
            Mvx.RegisterSingleton<IServiceRepository>(
                new ServiceRepository());
            Mvx.RegisterSingleton<ISamplesService>(
                new SamplesService(_samplesModuleName, _samplesFilename));
            Mvx.RegisterSingleton<IDataVersioningService>(
                new DataVersioningService());
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