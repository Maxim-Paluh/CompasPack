using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    class LaptopBatteryViewModel : ReportHardWareViewModelBase, IReportViewModel
    {
        private string _wearLevel;

        public string WearLevel
        {
            get { return _wearLevel; }
            set
            {
                _wearLevel = value;
                OnPropertyChanged();
            }
        }

        public LaptopBatteryViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
        }

        public void Load()
        {
            var tempWear = Document.XPathSelectElement(SettingsReport.LaptopBattery.XPath);
            if (tempWear != null)
                WearLevel = tempWear.Value;
            else
                WearLevel = string.Empty;

            if (!string.IsNullOrWhiteSpace(WearLevel))
            {
                var tempLaptopWearLevel = WearLevel;
                foreach (var item in SettingsReport.Monitor.MonitorType.Regex)
                    tempLaptopWearLevel = Regex.Replace(tempLaptopWearLevel, item, "");

                if(int.TryParse(tempLaptopWearLevel, out int temp))
                    Result = $"{100-temp}%";
                else
                    Result = "0%";


            }
            else
            {
                WearLevel = "Battery not found!";
                Result = "0%";
            }
        }
    }
}
