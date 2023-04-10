using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MemoryViewModel : ReportViewModelBase, IReportViewModel
    {
        private string _type;
        private string _size;
        private string _frequency;

        public MemoryViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
        }
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }
        public string Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }
        public string Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                OnPropertyChanged();
            }
        }

        
        public void Load()
        {
            var tempType = Document.XPathSelectElement(SettingsReport.Memory.MemoryType.XPath);
            if (tempType != null)
                Type = tempType.Value;
            else
                Type = "Not found";

            ManagementObjectSearcher memorys = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            UInt64 total = 0;
            foreach (ManagementObject ram in memorys.Get())
                total += (UInt64)ram.GetPropertyValue("Capacity");

            Size = total / 1073741824 + "Gb";

            var tempFrequency = Document.XPathSelectElement(SettingsReport.Memory.MemoryFrequency.XPath);
            if (tempFrequency != null)
                Frequency= tempFrequency.Value;
            else
                Frequency = "Not found";

            var tempMemoryType = Type;
            foreach (var item in SettingsReport.Memory.MemoryType.Regex)
                tempMemoryType = Regex.Replace(tempMemoryType, item, "");

            var tempMemoryFrequency = Frequency;
            foreach (var item in SettingsReport.Memory.MemoryFrequency.Regex)
                tempMemoryFrequency = Regex.Replace(tempMemoryFrequency, item, "");

            Result = $"{tempMemoryType}-{Size} ({tempMemoryFrequency}MHz)";
        }
    }
}
