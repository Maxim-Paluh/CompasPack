using CompasPack.Model.Enum;
using CompasPack.Model.ViewAndViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public class IWinAntivirusBase : IWinAntivirus
    {
        public bool IsControlled { get { return false; } }

        public Task<AntivirusStatus> CheckStatus()
        {
            return Task.FromResult(new AntivirusStatus() { RealtimeMonitoringStatus = AntivirusStatusEnum.Error, TamperProtectionStatus = AntivirusStatusEnum.Error });
        }

        public Task<AntivirusStatusEnum> DisableRealTimeMonitoring()
        {
            return Task.FromResult(AntivirusStatusEnum.Error);
        }

        public Task<AntivirusStatusEnum> EnableRealTimeMonitoring()
        {
            return Task.FromResult(AntivirusStatusEnum.Error);
        }

        public Task<AntivirusStatusEnum> GetRealTimeMonitoringStatus()
        {
            return Task.FromResult(AntivirusStatusEnum.Error);
        }

        public AntivirusStatusEnum GetTamperProtectionStatus()
        {
            return AntivirusStatusEnum.Error;
        }
    }
}
