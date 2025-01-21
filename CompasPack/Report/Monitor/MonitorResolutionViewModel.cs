using CompasPack.Helper;
using CompasPack.Settings;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class MonitorResolutionViewModel : ReportHardWareViewModelBase<Monitor>, IReportViewModel
    {
        private string _monitorResolution;
        public string MonitorResolution
        {
            get { return _monitorResolution; }
            set
            {
                _monitorResolution = value;
                OnPropertyChanged();
            }
        }
        public MonitorResolutionViewModel(Monitor monitorReportSettings, XDocument xDocument)
        {
            Settings = monitorReportSettings;
            Document = xDocument;
        }
        public void Load()
        {
            var resolution = MonitorHelper.GetOptimalScreenResolution();

            MonitorResolution = $"{resolution.Width}x{resolution.Height}";

            var nameResolution = string.Join(", ", MonitorHelper.GetNameResolution(resolution));
            if (!string.IsNullOrWhiteSpace(nameResolution))
                MonitorResolution += $" {nameResolution}";

            if (!string.IsNullOrWhiteSpace(MonitorResolution))
                Result = MonitorResolution;
        }
    }
}
