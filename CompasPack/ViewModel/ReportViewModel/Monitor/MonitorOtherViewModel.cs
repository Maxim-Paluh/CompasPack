using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class MonitorOtherViewModel : ReportHardWareViewModelBase, IReportViewModel
    {
        public MonitorOtherViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
        }
        public void Load()
        {
            //throw new NotImplementedException();
        }
    }
}
