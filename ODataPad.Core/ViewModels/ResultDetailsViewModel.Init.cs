using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public partial class ResultDetailsViewModel
    {
        public void Init(NavObject navObject)
        {
            var converter = Mvx.Resolve<IMvxJsonConverter>();

            _resultRow = new ResultRow(
                converter.DeserializeObject<Dictionary<string, object>>(navObject.SerializedProperties),
                converter.DeserializeObject<List<string>>(navObject.SerializedKeys));

            this.Text = string.Join(Environment.NewLine + Environment.NewLine,
                _resultRow.Properties
                    .Select(y => y.Key + Environment.NewLine + (y.Value == null ? "(null)" : y.Value.ToString())));

            AppState.ActiveResult = this;
        }

        public class NavObject
        {
            public string SerializedKeys { get; set; }
            public string SerializedProperties { get; set; }

            public NavObject() {}

            public NavObject(ResultDetailsViewModel result)
            {
                var converter = Mvx.Resolve<IMvxJsonConverter>();

                this.SerializedKeys = converter.SerializeObject(result.Keys);
                this.SerializedProperties = converter.SerializeObject(result.Properties);
            }
        }

        public class SavedState
        {
            public string SerializedKeys { get; set; }
            public string SerializedProperties { get; set; }
        }

        public SavedState SaveState()
        {
            var converter = Mvx.Resolve<IMvxJsonConverter>();
            return new SavedState()
            {
                SerializedKeys = converter.SerializeObject(_resultRow.Keys),
                SerializedProperties = converter.SerializeObject(_resultRow.Properties)
            };
        }

        public void ReloadState(SavedState savedState)
        {
            Init(new NavObject
            {
                SerializedKeys = savedState.SerializedKeys,
                SerializedProperties = savedState.SerializedProperties,
            });
        }
    }
}
