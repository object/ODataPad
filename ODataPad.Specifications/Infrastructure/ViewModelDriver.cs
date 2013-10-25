using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ODataPad.Core;
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

        public AppStateViewModel StateView { get { return AppState.Current.View; } }

        public void EnsureHomeViewModel()
        {
            lock (this)
            {
                if (_home == null)
                {
                    _home = new HomeViewModel();
                    _home.InitAsync().Wait();
                }
            }
        }

        public void SelectService(string serviceName)
        {
            var service = Home.Services.Items.Single(x => x.Name == serviceName);
            Home.Services.SelectedService = service;
            Home.Services.SelectServiceCommand.Execute(service);
        }

        public void SelectResourceSet(string resourceSetName)
        {
            var resourceSet = StateView.ActiveService.ResourceSets.Items.Single(x => x.Name == resourceSetName);
            StateView.ActiveService.ResourceSets.SelectedItem = resourceSet;
            StateView.ActiveService.ResourceSets.SelectResourceSetCommand.Execute(resourceSet);

            if (StateView.ActiveResourceSetMode == StateView.ResourceSetModes.Last())
                WaitForResults(5);
        }

        public void SelectResourceSetMode(string mode)
        {
            StateView.ActiveResourceSetMode = mode.ToLower().Contains("data")
                ? StateView.ResourceSetModes.Last()
                : StateView.ResourceSetModes.First();
        }

        public void SelectResult(string key)
        {
            var result = StateView.ActiveResourceSet.Results.QueryResults.Single(x => x.Properties[x.Keys.Single()].ToString() == key);
            StateView.ActiveResourceSet.Results.SelectedResult = result;
            StateView.ActiveResourceSet.Results.SelectResultCommand.Execute(result);
        }

        private void WaitForResults(int maxSeconds)
        {
            for (var seconds = 0; seconds < maxSeconds * 10; seconds++)
            {
                if (StateView.ActiveResourceSet.Results.QueryResults.Any())
                    break;
                Thread.Sleep(100);
            }
        }
    }
}