using CompasPack.Data.Providers;
using CompasPack.Model.Settings;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MonitorAspectRatioViewModel : ReportHardwareViewModelBase<Monitor>
    {     
        private string _monitorAspectRatio;
        public string MonitorAspectRatio
        {
            get { return _monitorAspectRatio; }
            set { _monitorAspectRatio = value; }
        }
        public MonitorAspectRatioViewModel(ReportSettingsProvider reportSettingsProvider)
        {
            Settings = reportSettingsProvider.Settings.Monitor;

        }
        public void Load(XDocument xDocument)
        {
            MonitorAspectRatio = xDocument.XPathSelectElement(Settings.MonitorAspectRatio.XPath)?.Value ?? string.Empty;
            Result = MonitorAspectRatio;
        }
    }
}
