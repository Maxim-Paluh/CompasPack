using System;
using System.Linq;
using System.Text.RegularExpressions;

using CompasPack.Data.Providers;
using CompasPack.Model.Settings;
using CompasPack.Model.ViewAndViewModel;

namespace CompasPack.ViewModel
{
    public class CPUViewModel : ReportHardwareViewModelBase<CPU>
    {
        private CPUInfo _cPUInfo;
        private readonly IHardwareInfoProvider _hardwareInfoProvider;

        public CPUInfo CPUInfo
        {
            get { return _cPUInfo; }
            set { _cPUInfo = value; OnPropertyChanged(); }
        }

        public CPUViewModel(IHardwareInfoProvider hardwareInfoProvider)
        {
            _hardwareInfoProvider = hardwareInfoProvider;
        }

        public void Load(CPU CPUReportSettings)
        {
            Settings = CPUReportSettings;
            CPUInfo = _hardwareInfoProvider.GetCPUInfo();
            var tempName = Settings.Regex.Aggregate(CPUInfo.Name, (current, pattern) => Regex.Replace(current, pattern, ""));

            Result = tempName + " " + CPUInfo.Clock;
        }
    }
}
