using CompasPack.Data.Providers;
using CompasPack.Model.Settings;
using CompasPack.Model.ViewAndViewModel;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MemoryViewModel : ReportHardwareViewModelBase<Memory>
    {
        private MemoryInfo _memoryInfo;
        private readonly IHardwareInfoProvider _hardwareInfoProvider;

        public MemoryInfo MemoryInfo
        {
            get { return _memoryInfo; }
            set { _memoryInfo = value; OnPropertyChanged(); }
        }
        public MemoryViewModel(ReportSettingsProvider reportSettingsProvider, IHardwareInfoProvider hardwareInfoProvider)
        {
            Settings = reportSettingsProvider.Settings.Memory;
            _hardwareInfoProvider = hardwareInfoProvider;
        }

        public void Load(XDocument xDocument)
        {
            MemoryInfo = _hardwareInfoProvider.GetMemoryInfo();

            MemoryInfo.Type = xDocument.XPathSelectElement(Settings.MemoryType.XPath)?.Value ?? "Not found";
            MemoryInfo.Frequency = xDocument.XPathSelectElement(Settings.MemoryFrequency.XPath)?.Value ?? "Not found";

            var tempMemoryType = Settings.MemoryType.Regex.Aggregate(MemoryInfo.Type, (current, pattern) => Regex.Replace(current, pattern, ""));
            var tempMemoryFrequency = Settings.MemoryFrequency.Regex.Aggregate(MemoryInfo.Frequency, (current, pattern) => Regex.Replace(current, pattern, ""));

            Result = $"{tempMemoryType}-{MemoryInfo.Size} ({tempMemoryFrequency}MHz)";
        }
    }
}
