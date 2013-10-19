using System;
using System.Collections.Generic;
using System.Linq;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;

namespace ODataPad.Specifications.Infrastructure
{
    public class ViewModelDriver
    {
        public void SelectService(ServiceListViewModel viewModel, string serviceName)
        {
            var service = viewModel.Items.Single(x => x.Name == serviceName);
            viewModel.SelectedService = service;
            viewModel.SelectServiceCommand.Execute(service);
        }

        public void SelectResourceSet(ResourceSetListViewModel viewModel, string resourceSetName)
        {
            var resourceSet = viewModel.Items.Single(x => x.Name == resourceSetName);
            viewModel.SelectedItem = resourceSet;
            viewModel.SelectResourceSetCommand.Execute(resourceSet);
        }

        public void SelectResourceSetMode(HomeViewModel viewModel, string mode)
        {
            viewModel.SelectedResourceSetMode = mode.ToLower().Contains("data")
                ? viewModel.ResourceSetModes.Last()
                : viewModel.ResourceSetModes.First();
        }

        public void SelectResult(ResultListViewModel viewModel, string key)
        {
            var result = viewModel.QueryResults.Single(x => x.Properties[x.Keys.Single()].ToString() == key);
            viewModel.SelectedResult = result;
            viewModel.SelectResultCommand.Execute(result);
        }

        public string GetSchemaSummary(ResourceSetDetailsViewModel viewModel)
        {
            return viewModel.Summary;
        }

        public ObservableResultCollection GetQueryResults(ResultListViewModel viewModel)
        {
            return viewModel.QueryResults;
        }
    }
}