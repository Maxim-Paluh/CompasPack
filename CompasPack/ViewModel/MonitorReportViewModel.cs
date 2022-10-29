using CompasPakc.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class MonitorReportViewModel : ViewModelBase, IDetailViewModel
    {
        private SettingsReportViewModel _settingsReportViewModel;
        private XDocument _xDocument;
        private IIOManager _ioManager;
        public MonitorReportViewModel(IIOManager iOManager, SettingsReportViewModel settingsReportViewModel, XDocument xDocument)
        {
            _ioManager = iOManager;
            _settingsReportViewModel = settingsReportViewModel;
            _xDocument = xDocument;
        }

        public bool HasChanges()
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync(int? Id)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
