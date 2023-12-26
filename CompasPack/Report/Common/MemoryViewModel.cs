using CompasPack.Settings;
using System;
using System.Management;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MemoryViewModel : ReportHardWareViewModelBase<MemoryReportSettings>, IReportViewModel
    {
        private string _type;
        private string _size;
        private string _frequency;
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
        public MemoryViewModel(MemoryReportSettings memoryReportSettings, XDocument xDocument)
        {
            Settings= memoryReportSettings;
            Document = xDocument;
        }
        public void Load()
        {
            var tempType = Document.XPathSelectElement(Settings.MemoryType.XPath);
            if (tempType != null)
                Type = tempType.Value;
            else
                Type = "Not found";

            ManagementObjectSearcher memorys = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            UInt64 total = 0;
            foreach (ManagementObject ram in memorys.Get())
                total += (UInt64)ram.GetPropertyValue("Capacity");

            Size = total / 1073741824 + "Gb";

            var tempFrequency = Document.XPathSelectElement(Settings.MemoryFrequency.XPath);
            if (tempFrequency != null)
                Frequency= tempFrequency.Value;
            else
                Frequency = "Not found";

            var tempMemoryType = Type;
            foreach (var item in Settings.MemoryType.Regex)
                tempMemoryType = Regex.Replace(tempMemoryType, item, "");

            var tempMemoryFrequency = Frequency;
            foreach (var item in Settings.MemoryFrequency.Regex)
                tempMemoryFrequency = Regex.Replace(tempMemoryFrequency, item, "");

            Result = $"{tempMemoryType}-{Size} ({tempMemoryFrequency}MHz)";
        }
    }
}
