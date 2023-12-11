using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MonitorDiagonalViewModel : ReportHardWareViewModelBase, IReportViewModel, IDataErrorInfo
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

        public MonitorDiagonalViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
        }

        public string this[string columnName] => throw new NotImplementedException();

        public string Error => throw new NotImplementedException();

        public void Load()
        {
            var tempType = Document.XPathSelectElement(SettingsReport.Monitor.MonitorType.XPath);
            if (tempType != null)
                LaptopMonitorType = tempType.Value;
            else
                LaptopMonitorType = string.Empty;

            var tempSize = Document.XPathSelectElement(SettingsReport.Monitor.MonitorSize.XPath);
            if (tempSize != null)
                LaptopMonitorSize = tempSize.Value;
            else
                LaptopMonitorSize = string.Empty;

            if (!string.IsNullOrWhiteSpace(LaptopMonitorType))
            {
                var tempLaptopMonitorType = LaptopMonitorType;
                foreach (var item in SettingsReport.Monitor.MonitorType.Regex)
                    tempLaptopMonitorType = Regex.Replace(tempLaptopMonitorType, item, "");

                if(!string.IsNullOrWhiteSpace(tempLaptopMonitorType))
                    Result = $"{tempLaptopMonitorType}";
            }

            if (string.IsNullOrWhiteSpace(Result))
            {
                if (!string.IsNullOrWhiteSpace(LaptopMonitorSize))
                {
                    var tempLaptopMonitorSize = LaptopMonitorSize;
                    foreach (var item in SettingsReport.Monitor.MonitorSize.Regex)
                        tempLaptopMonitorSize = Regex.Replace(tempLaptopMonitorSize, item, "");
                    Result = $"{tempLaptopMonitorSize}";
                }
            }

        }


    }
}
