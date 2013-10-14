using System.Linq;
using ODataPad.Core.ViewModels;

namespace ODataPad.Specifications.Infrastructure
{
    public class ViewModelDriver
    {
        public void SelectService(HomeViewModel viewModel, string serviceName)
        {
            var service = viewModel.Services.Single(x => x.Name == serviceName);
            viewModel.SelectedService = service;
            viewModel.SelectServiceCommand.Execute(service);
        }
    }
}