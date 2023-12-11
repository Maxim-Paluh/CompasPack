using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class CPUViewModel : ReportHardWareViewModelBase, IReportViewModel
    {
        private string _name;
        private string _clock;
        
        public CPUViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Clock
        {
            get { return _clock; }
            set
            {
                _clock = value;
                OnPropertyChanged();
            }
        }
        public void Load()
        {
            ManagementObjectSearcher processors = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            var processor = processors.Get().Cast<ManagementObject>().First();
            Name += processor["Name"];
            var temp = processor["MaxClockSpeed"];
            if (temp != null)
                Clock += $"{(double.Parse(temp.ToString()) / 1000).ToString().Replace(',', '.')}GHz";

            var tempName = Name;
            foreach (var item in SettingsReport.CPU.Regex)
                tempName = Regex.Replace(tempName, item, "");

            Result = tempName + " " + Clock;
        }


    }
}
