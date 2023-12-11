using CompasPack.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MonitorAspectRatioViewModel : ReportHardWareViewModelBase, IReportViewModel
    {
       
        private string _monitorAspectRatio;

        public string MonitorAspectRatio
        {
            get { return _monitorAspectRatio; }
            set { _monitorAspectRatio = value; }
        }

        public MonitorAspectRatioViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
        }
        public void Load()
        {
            var tempMonitorAspectRatio = Document.XPathSelectElement(SettingsReport.Monitor.AspectRatio.XPath);
            if (tempMonitorAspectRatio != null)
                MonitorAspectRatio = tempMonitorAspectRatio.Value;
            else
                MonitorAspectRatio = string.Empty;
            
            if(!string.IsNullOrWhiteSpace(MonitorAspectRatio))
                Result = MonitorAspectRatio;
        }
    }
}
