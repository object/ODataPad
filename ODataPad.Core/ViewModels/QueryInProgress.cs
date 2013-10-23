using System;
using ODataPad.Core.Interfaces;

namespace ODataPad.Core.ViewModels
{
    class QueryInProgress : INotifyInProgress
    {
        private readonly Action<bool> _action;

        public QueryInProgress(Action<bool> action)
        {
            _action = action;
        }

        public bool IsInProgress
        {
            set { _action(value); }
        }
    }
}