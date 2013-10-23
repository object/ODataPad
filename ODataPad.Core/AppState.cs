using System;
using System.Collections.Generic;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core
{
    public class AppState
    {
        private static readonly AppState Instance = new AppState();

        static AppState()
        {
            ViewModelsWithOwnViews = new List<Type>(new[] { typeof(HomeViewModel) });
        }

        public static AppState Current { get { return Instance; } }
        public static List<Type> ViewModelsWithOwnViews { get; private set; } 

        public ServiceDetailsViewModel ActiveService { get; set; }
        public ResourceSetDetailsViewModel ActiveResourceSet { get; set; }
        public ResultDetailsViewModel ActiveResult { get; set; }

        public bool IsQueryInProgress { get; set; }
    }
}