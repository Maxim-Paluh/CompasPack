using CompasPack.Settings;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class CPUViewModel : ReportHardWareViewModelBase<CPU>, IReportViewModel
    {
        private string _name;
        private string _clock;
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
        public CPUViewModel(CPU CPUReportSettings)
        {
            Settings = CPUReportSettings;
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
            foreach (var item in Settings.Regex)
                tempName = Regex.Replace(tempName, item, "");

            Result = tempName + " " + Clock;
        }
    }
}
