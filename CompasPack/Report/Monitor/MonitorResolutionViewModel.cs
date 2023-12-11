using CompasPack.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class MonitorResolutionViewModel : ReportHardWareViewModelBase, IReportViewModel
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
        public MonitorResolutionViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
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
