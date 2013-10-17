using System.Linq;
using ODataPad.Core.ViewModels;

namespace ODataPad.Specifications.Infrastructure
{
    public class ViewModelDriver
    {
        public void SelectService(HomeViewModel viewModel, string serviceName)
        {
            var service = viewModel.Services.Items.Single(x => x.Name == serviceName);
            viewModel.Services.SelectedService = service;
            viewModel.Services.SelectServiceCommand.Execute(service);
        }
    }
}