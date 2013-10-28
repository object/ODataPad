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

        public AppState AppState { get { return AppState.Current; } }

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
            var resourceSet = AppState.ActiveService.ResourceSets.Items.Single(x => x.Name == resourceSetName);
            AppState.ActiveService.ResourceSets.SelectedItem = resourceSet;
            AppState.ActiveService.ResourceSets.SelectResourceSetCommand.Execute(resourceSet);

            if (AppState.IsResultViewSelected)
                WaitForResults(5);
        }

        public void SelectResourceSetMode(string mode)
        {
            AppState.ActiveResourceSetMode = mode.ToLower().Contains("data")
                ? AppState.ResourceSetModes.Last()
                : AppState.ResourceSetModes.First();
        }

        public void SelectResult(string key)
        {
            var result = AppState.ActiveResourceSet.Results.QueryResults.Single(x => x.Properties[x.Keys.Single()].ToString() == key);
            AppState.ActiveResourceSet.Results.SelectedResult = result;
            AppState.ActiveResourceSet.Results.SelectResultCommand.Execute(result);
        }

        private void WaitForResults(int maxSeconds)
        {
            for (var seconds = 0; seconds < maxSeconds * 10; seconds++)
            {
                if (AppState.ActiveResourceSet.Results.QueryResults.Any())
                    break;
                Thread.Sleep(100);
            }
        }
    }
}