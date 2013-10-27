using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public partial class ResultDetailsViewModel : MvxViewModel
    {
        private ResultRow _resultRow;

        public ResultDetailsViewModel()
        {
        }

        public ResultDetailsViewModel(ResultRow resultRow)
        {
            _resultRow = resultRow;
        }

        public static ResultDetailsViewModel DesignModeCreate(ResultRow resultRow)
        {
            var result = new ResultDetailsViewModel(resultRow);
            AppState.Current.ActiveResult = result;
            return result;
        }

        public AppState AppState { get { return AppState.Current; } }

        public IEnumerable<string> Keys { get { return _resultRow.Keys; } }
        public IDictionary<string, object> Properties { get { return _resultRow.Properties; } }
        public string KeySummary { get { return GetKeySummary(); } }
        public string ValueSummary { get { return GetPropertySummary(); } }

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

        private string GetKeySummary()
        {
            if (IsError())
            {
                return this.Properties.Keys.First();
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var result in this.Properties)
                {
                    if (!this.Keys.Contains(result.Key))
                        continue;

                    if (sb.Length > 0)
                        sb.Append(" | ");
                    var text = result.Value.ToString();
                    sb.Append(text);
                }
                return sb.ToString();
            }
        }

        private string GetPropertySummary()
        {
            if (IsError())
            {
                return this.Properties.Values.First().ToString();
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var result in this.Properties)
                {
                    if (this.Keys.Contains(result.Key))
                        continue;

                    if (sb.Length > 0)
                        sb.Append(" | ");
                    var text = result.Value == null ? "(null)" : result.Value.ToString();
                    if (text.Length > 30)
                        text = text.Substring(0, 30) + "...";
                    sb.Append(text);
                }
                return sb.ToString();
            }
        }

        private bool IsError()
        {
            return this.Properties.Count == 1 &&
                this.Properties.Keys.First() == "Error" &&
                !this.Keys.Contains(this.Properties.Keys.First());
        }
    }
}