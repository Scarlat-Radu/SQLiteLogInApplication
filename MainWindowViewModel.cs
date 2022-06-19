using Dragablz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_SQLite_Demo
{
    class MainWindowViewModel
    {

        private readonly IInterTabClient _interTabClient;
        private readonly ObservableCollection<HeaderedItemViewModel> _tabContents = new ObservableCollection<HeaderedItemViewModel>();

        public MainWindowViewModel()
        {
            _interTabClient = new DefaultInterTabClient();


        }


        public ObservableCollection<HeaderedItemViewModel> TabContents
        {
            get { return _tabContents; }
        }

        public IInterTabClient InterTabClient
        {
            get { return _interTabClient; }
        }

    }
}
