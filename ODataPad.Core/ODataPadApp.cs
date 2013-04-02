using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.CrossCore.IoC;
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
        public static string SamplesFolder;
        public static string SamplesFilename;

        public ODataPadApp(
            string samplesFolder,
            string samplesFilename)
        {
            SamplesFolder = samplesFolder;
            SamplesFilename = samplesFilename;

            InitalizeServices();
            InitializeStartNavigation();
            InitializePlugIns();
        }

        public IServiceRepository ServiceRepository { get; private set; }
        public IResourceManager ResourceManager { get; private set; }
        public IServiceLocalStorage ServiceLocalStorage { get; private set; }
        public ISamplesService SamplesService { get; private set; }
        public IDataVersioningService DataVersioningService { get; private set; }

        public HomeViewModel HomeViewModel { get; private set; }

        private void InitalizeServices()
        {
            Mvx.RegisterSingleton<IServiceRepository>(new ServiceRepository());
            Mvx.RegisterSingleton<IDataVersioningService>(new DataVersioningService());
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