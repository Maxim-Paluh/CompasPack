using System;
using System.Xml.Linq;

using CompasPack.Model.Settings;
using CompasPack.Data.Providers;
using CompasPack.Data.Constants;

namespace CompasPack.ViewModel
{
    public class MonitorResolutionViewModel : ReportHardwareViewModelBase<Monitor>
    {
        private readonly IHardwareInfoProvider _hardwareInfoProvider;
        private string _monitorResolution;
        public string MonitorResolution
        {
            get { return _monitorResolution; }
            set
            {
                _monitorResolution = value;
                OnPropertyChanged();
            }
        }
        public MonitorResolutionViewModel(ReportSettingsProvider reportSettingsProvider, IHardwareInfoProvider hardwareInfoProvider)
        {
            Settings = reportSettingsProvider.Settings.Monitor;
            _hardwareInfoProvider = hardwareInfoProvider;
        }
        public void Load(XDocument xDocument)
        {
            var resolution = _hardwareInfoProvider.GetScreenResolution();

            MonitorResolution = $"{resolution.Width}x{resolution.Height}";

            var nameResolution = string.Join(", ", ResolutionNameList.GetNameResolution(resolution));
            if (!string.IsNullOrWhiteSpace(nameResolution))
                MonitorResolution += $" {nameResolution}";

            if (!string.IsNullOrWhiteSpace(MonitorResolution))
                Result = MonitorResolution;
        }
    }
}
