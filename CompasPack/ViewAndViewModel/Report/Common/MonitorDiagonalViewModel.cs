using CompasPack.Data.Providers;
using CompasPack.Model.Settings;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MonitorDiagonalViewModel : ReportHardwareViewModelBase<Monitor>
    {
        private string _laptopMonitorType;
        private string _laptopMonitorSize;
        public string LaptopMonitorSize
        {
            get { return _laptopMonitorSize; }
            set
            {
                _laptopMonitorSize = value;
                OnPropertyChanged();
            }
        }
        public string LaptopMonitorType
        {
            get { return _laptopMonitorType; }
            set
            {

                _laptopMonitorType = value;
                OnPropertyChanged();
            }
        }
        public MonitorDiagonalViewModel(ReportSettingsProvider reportSettingsProvider)
        {
            Settings = reportSettingsProvider.Settings.Monitor;
        }
        public void Load(XDocument xDocument)
        {
            LaptopMonitorType = xDocument.XPathSelectElement(Settings.MonitorType.XPath)?.Value;
            LaptopMonitorSize = xDocument.XPathSelectElement(Settings.MonitorSize.XPath)?.Value;

            if (!string.IsNullOrWhiteSpace(LaptopMonitorType))
            {
                var tempLaptopMonitorType = Settings.MonitorType.Regex.Aggregate(LaptopMonitorType, (current, pattern) => Regex.Replace(current, pattern, ""));
                Result = $"{tempLaptopMonitorType}";
            }
            if (string.IsNullOrWhiteSpace(Result) && !string.IsNullOrWhiteSpace(LaptopMonitorSize))
            {
                var tempLaptopMonitorSize = Settings.MonitorSize.Regex.Aggregate(LaptopMonitorSize, (current, pattern) => Regex.Replace(current, pattern, ""));
                Result = $"{tempLaptopMonitorSize}";
            }
        }
    }
}
