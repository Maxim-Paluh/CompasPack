using CompasPack.Data.Providers;
using CompasPack.Model.Settings;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    class LaptopBatteryViewModel : ReportHardwareViewModelBase<LaptopBattery>
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
        public LaptopBatteryViewModel(ReportSettingsProvider reportSettingsProvider)
        {
            Settings = reportSettingsProvider.Settings.LaptopBattery;
        }
        public void Load(XDocument xDocument)
        {
            WearLevel = xDocument.XPathSelectElement(Settings.XPath)?.Value ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(WearLevel))
            {
                var tempLaptopWearLevel = Settings.Regex.Aggregate(WearLevel, (current, pattern) => Regex.Replace(current, pattern, ""));

                if (int.TryParse(tempLaptopWearLevel, out int tempLaptopWearLevelInt))
                    Result = $"{100 - tempLaptopWearLevelInt}%";
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
