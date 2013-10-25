using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ServiceEditViewModel : MvxViewModel
    {
        public ServiceEditViewModel Self { get { return this; } }
        public HomeViewModelBase Home { get; set; }

        private ServiceInfo _sourceService;
        public ServiceInfo SourceService
        {
            get { return _sourceService; }
            set { _sourceService = value; RaisePropertyChanged(() => SourceService); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(() => Name); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged(() => Description); }
        }

        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; RaisePropertyChanged(() => Url); }
        }

        private bool _isSaveEnabled;
        public bool IsSaveEnabled
        {
            get { return _isSaveEnabled; }
            set { _isSaveEnabled = value; RaisePropertyChanged(() => IsSaveEnabled); }
        }

        public ICommand ServiceNameChangedCommand
        {
            get { return new MvxCommand(RefreshSaveButtonState); }
        }

        public ICommand ServiceDescriptionChangedCommand
        {
            get { return new MvxCommand(RefreshSaveButtonState); }
        }

        public ICommand ServiceUrlChangedCommand
        {
            get { return new MvxCommand(RefreshSaveButtonState); }
        }

        public ICommand SaveServiceEditCommand
        {
            get { return new MvxCommand(SaveServiceEdit); }
        }

        public ICommand CancelServiceEditCommand
        {
            get { return new MvxCommand(CancelServiceEdit); }
        }

        private void SaveServiceEdit()
        {
            // TODO: add or update service

            this.Home.Services.IsServiceEditInProgress = false;
        }

        private void CancelServiceEdit()
        {
            this.Home.Services.IsServiceEditInProgress = false;
        }

        private void RefreshSaveButtonState()
        {
            this.IsSaveEnabled =
                !string.IsNullOrEmpty(this.Name) &&
                !string.IsNullOrEmpty(this.Url) &&
                !string.IsNullOrEmpty(this.Description);
        }

        public bool ServiceHasUniqueName()
        {
            return this.Home.Services.Items.All(x => x.Name != this.Name);
        }


        public bool ServiceHasValidUrl()
        {
            return Regex.IsMatch(this.Url, @"((http|https)://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
        }
    }
}