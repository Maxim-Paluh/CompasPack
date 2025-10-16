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
            var nameResolution = string.Join(", ", ResolutionNameList.GetNameResolution(resolution));

            MonitorResolution = $"{resolution.Width}x{resolution.Height}" + (string.IsNullOrWhiteSpace(nameResolution) ? string.Empty : $" {nameResolution}");
            Result = string.IsNullOrWhiteSpace(MonitorResolution) ? string.Empty : MonitorResolution;
        }
    }
}
