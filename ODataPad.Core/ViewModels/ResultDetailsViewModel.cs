using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public partial class ResultDetailsViewModel : MvxViewModel
    {
        private ResultInfo _resultInfo;

        public ResultDetailsViewModel()
        {
        }

        public ResultDetailsViewModel(ResultInfo resultInfo)
        {
            _resultInfo = resultInfo;
            this.Text = GetTextSummary();
        }

        public AppState AppState { get { return AppState.Current; } }

        public IEnumerable<string> Keys { get { return _resultInfo.Keys; } }
        public IDictionary<string, object> Properties { get { return _resultInfo.Properties; } }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(() => Text);
            }
        }

        private string GetTextSummary()
        {
            return string.Join(Environment.NewLine + Environment.NewLine,
                _resultInfo.Properties
                    .Select(y => y.Key + Environment.NewLine + (y.Value == null ? "(null)" : y.Value.ToString())));
        }

        private bool IsError()
        {
            return this.Properties.Count == 1 &&
                this.Properties.Keys.First() == "Error" &&
                !this.Keys.Contains(this.Properties.Keys.First());
        }

        public static ResultDetailsViewModel DesignModeCreate(ResultInfo resultInfo)
        {
            var result = new ResultDetailsViewModel(resultInfo);
            AppState.Current.ActiveResult = result;
            return result;
        }
    }
}