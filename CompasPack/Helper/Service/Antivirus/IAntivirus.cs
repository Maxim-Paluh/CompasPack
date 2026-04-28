using CompasPack.Model.Enum;
using CompasPack.Model.ViewAndViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public interface IAntivirus
    {
        bool IsControlled { get; }
        AntivirusInfo AntivirusInfo { get; }
        Task<AntivirusStatusEnum> DisableRealTimeMonitoring();
        Task<AntivirusStatusEnum> EnableRealTimeMonitoring();
        Task<AntivirusStatus> CheckStatus();
        Task<AntivirusStatusEnum> GetRealTimeMonitoringStatus();
        AntivirusStatusEnum GetTamperProtectionStatus();
        void OpenSettings();
    }
}
