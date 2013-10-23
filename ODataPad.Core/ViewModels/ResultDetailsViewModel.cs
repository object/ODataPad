using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ResultDetailsViewModel : MvxViewModel
    {
        private ResultRow _resultRow;

        public ResultDetailsViewModel(ResultRow resultRow)
        {
            _resultRow = resultRow;
        }

        public void Init(NavObject navObject)
        {
            _resultRow = navObject.ResultDetails;
        }

        public class NavObject
        {
            public ResultRow ResultDetails { get; set; }
        }

        public class SavedState
        {
            public List<string> Keys { get; set; }
            public Dictionary<string, object> Properties { get; set; }
        }

        public SavedState SaveState()
        {
            return new SavedState()
            {
                Keys = new List<string>(_resultRow.Keys),
                Properties = new Dictionary<string, object>(_resultRow.Properties)
            };
        }

        public void ReloadState(SavedState savedState)
        {
            _resultRow = new ResultRow(savedState.Properties, savedState.Keys);
        }

        public IEnumerable<string> Keys { get { return _resultRow.Keys; } }
        public IDictionary<string, object> Properties { get { return _resultRow.Properties; } }
        public string KeySummary { get { return GetKeySummary(); } }
        public string ValueSummary { get { return GetPropertySummary(); } }

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