using CompasPack.Settings;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MonitorDiagonalViewModel : ReportHardWareViewModelBase<MonitorReportSettings>, IReportViewModel
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
        public MonitorDiagonalViewModel(MonitorReportSettings monitorReportSettings, XDocument xDocument)
        {
            Settings = monitorReportSettings;
            Document = xDocument;
        }
        public void Load()
        {
            var tempType = Document.XPathSelectElement(Settings.MonitorType.XPath);
            if (tempType != null)
                LaptopMonitorType = tempType.Value;
            else
                LaptopMonitorType = string.Empty;

            var tempSize = Document.XPathSelectElement(Settings.MonitorSize.XPath);
            if (tempSize != null)
                LaptopMonitorSize = tempSize.Value;
            else
                LaptopMonitorSize = string.Empty;

            if (!string.IsNullOrWhiteSpace(LaptopMonitorType))
            {
                var tempLaptopMonitorType = LaptopMonitorType;
                foreach (var item in Settings.MonitorType.Regex)
                    tempLaptopMonitorType = Regex.Replace(tempLaptopMonitorType, item, "");

                if(!string.IsNullOrWhiteSpace(tempLaptopMonitorType))
                    Result = $"{tempLaptopMonitorType}";
            }

            if (string.IsNullOrWhiteSpace(Result))
            {
                if (!string.IsNullOrWhiteSpace(LaptopMonitorSize))
                {
                    var tempLaptopMonitorSize = LaptopMonitorSize;
                    foreach (var item in Settings.MonitorSize.Regex)
                        tempLaptopMonitorSize = Regex.Replace(tempLaptopMonitorSize, item, "");
                    Result = $"{tempLaptopMonitorSize}";
                }
            }

        }
    }
}
