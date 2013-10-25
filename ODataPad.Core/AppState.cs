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

        public AppState()
        {
            this.View = new AppStateViewModel();
        }

        public static AppState Current { get { return Instance; } }
        public static List<Type> ViewModelsWithOwnViews { get; private set; }
        public AppStateViewModel View { get; private set; }
    }
}