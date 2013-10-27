using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public partial class ServiceDetailsViewModel
    {
        public void Init(NavObject navObject)
        {
            this.Name = navObject.Name;
            this.Description = navObject.Description;
            this.Url = navObject.Url;
            this.ImageBase64 = navObject.ImageBase64;
            this.MetadataCache = navObject.MetadataCache;

            _resourceSets = new ResourceSetListViewModel(this.Url);

            RebuildMetadataFromCache();

            AppState.ActiveService = this;
        }

        public class NavObject
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string ImageBase64 { get; set; }
            public string MetadataCache { get; set; }

            public NavObject() { }

            public NavObject(ServiceInfo serviceInfo)
            {
                this.Name = serviceInfo.Name;
                this.Description = serviceInfo.Description;
                this.Url = serviceInfo.Url;
                this.ImageBase64 = serviceInfo.ImageBase64;
                this.MetadataCache = serviceInfo.MetadataCache;
            }
        }

        public class SavedState
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string ImageBase64 { get; set; }
            public string MetadataCache { get; set; }
        }

        public SavedState SaveState()
        {
            return new SavedState()
            {
                Name = this.Name,
                Description = this.Description,
                Url = this.Url,
                ImageBase64 = this.ImageBase64,
                MetadataCache = this.MetadataCache,
            };
        }

        public void ReloadState(SavedState savedState)
        {
            Init(new NavObject
            {
                Name = this.Name,
                Description = this.Description,
                Url = this.Url,
                ImageBase64 = this.ImageBase64,
                MetadataCache = this.MetadataCache,
            });
        }
    }
}
