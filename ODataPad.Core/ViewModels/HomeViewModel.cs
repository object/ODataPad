using System.Collections.ObjectModel;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModel
    {
        public ObservableCollection<ServiceInfo> Services { get; private set; }
        public ObservableCollection<ServiceCollection> Collections { get; private set; }
        public ObservableCollection<CollectionProperty> CollectionProperties { get; private set; }
        public ObservableCollection<ResultRow> QueryResults { get; private set; }
    }
}
