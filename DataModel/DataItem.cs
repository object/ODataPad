using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ODataPad.DataModel
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class DataItem : DataCommon
    {
        public DataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, DataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private ObservableCollection<DataItem> _elements = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> Elements
        {
            get { return this._elements; }
            set { this.SetProperty(ref this._elements, value); }
        }

        private IEnumerable<IDictionary<string, object>> _results;
        public IEnumerable<IDictionary<string, object>> Results
        {
            get { return this._results; }
            set { this.SetProperty(ref this._results, value); }
        }

        private DataGroup _group;
        public DataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }
}