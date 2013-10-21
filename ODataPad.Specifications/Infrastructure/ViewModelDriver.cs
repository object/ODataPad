using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;

namespace ODataPad.Specifications.Infrastructure
{
    public class ViewModelDriver
    {
        private HomeViewModel _home;

        public HomeViewModelBase Home
        {
            get
            {
                EnsureHomeViewModel();
                return _home;
            }
        }

        public ServiceListViewModel Services { get { return Home.Services; } }
        public IList<ServiceDetailsViewModel> ServiceDetails { get { return Services.Items; } }
        public ServiceDetailsViewModel SelectedServiceDetails { get { return Services.SelectedService; } }
        public ResourceSetListViewModel ResourceSets { get { return SelectedServiceDetails.ResourceSets; } }
        public IList<ResourceSetDetailsViewModel> ResourceSetDetails { get { return ResourceSets.Items; } }
        public ResourceSetDetailsViewModel SelectedResourceSetDetails { get { return ResourceSets.SelectedItem; } }
        public IEnumerable<string> ResourceSetModes { get { return Home.ResourceSetModes; } }
        public string SelectedResourceSetMode { get { return Home.SelectedResourceSetMode; } }
        public string SelectedSchemaSummary { get { return SelectedResourceSetDetails.Summary; } }
        public ResultListViewModel Results { get { return SelectedResourceSetDetails.Results; } }
        public ObservableResultCollection ResultDetails { get { return Results.QueryResults; } }
        public ResultDetailsViewModel SelectedResultDetails { get { return Results.SelectedResult; } set { Results.SelectedResult = value; } }
        public string SelectedResultSummary { get { return Results.SelectedResultDetails; } }
        public bool IsSingleResultSelected { get { return Results.IsSingleResultSelected; } }

        public void EnsureHomeViewModel()
        {
            lock (this)
            {
                if (_home == null)
                {
                    _home = new HomeViewModel();
                    _home.InitAsync(null).Wait();
                }
            }
        }

        public void SelectService(string serviceName)
        {
            var service = ServiceDetails.Single(x => x.Name == serviceName);
            Services.SelectedService = service;
            Services.SelectServiceCommand.Execute(service);
        }

        public void SelectResourceSet(string resourceSetName)
        {
            var resourceSet = ResourceSetDetails.Single(x => x.Name == resourceSetName);
            ResourceSets.SelectedItem = resourceSet;
            ResourceSets.SelectResourceSetCommand.Execute(resourceSet);

            if (SelectedResourceSetMode == ResourceSetModes.Last())
                WaitForResults(5);
        }

        public void SelectResourceSetMode(string mode)
        {
            Home.SelectedResourceSetMode = mode.ToLower().Contains("data")
                ? Home.ResourceSetModes.Last()
                : Home.ResourceSetModes.First();
        }

        public void SelectResult(string key)
        {
            var result = ResultDetails.Single(x => x.Properties[x.Keys.Single()].ToString() == key);
            Results.SelectedResult = result;
            Results.SelectResultCommand.Execute(result);
        }

        private void WaitForResults(int maxSeconds)
        {
            for (var seconds = 0; seconds < maxSeconds * 10; seconds++)
            {
                if (ResultDetails.Any())
                    break;
                Thread.Sleep(100);
            }
        }
    }
}