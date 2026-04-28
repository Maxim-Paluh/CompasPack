using CompasPack.Helper.Service;
using CompasPack.Model.Enum;
using CompasPack.Model.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service.Antivirus
{
    public class UnmanagedAntivirus : AntivirusBase
    {
        public override bool IsControlled => false;

        public UnmanagedAntivirus(AntivirusInfo antivirusInfo) : base(antivirusInfo) { }

        public override Task<AntivirusStatus> CheckStatus() =>
            Task.FromResult(new AntivirusStatus { RealtimeMonitoringStatus = AntivirusStatusEnum.Error, TamperProtectionStatus = AntivirusStatusEnum.Error });

        public override Task<AntivirusStatusEnum> DisableRealTimeMonitoring() => Task.FromResult(AntivirusStatusEnum.Error);
        public override Task<AntivirusStatusEnum> EnableRealTimeMonitoring() => Task.FromResult(AntivirusStatusEnum.Error);
        public override Task<AntivirusStatusEnum> GetRealTimeMonitoringStatus() => Task.FromResult(AntivirusStatusEnum.Error);
        public override AntivirusStatusEnum GetTamperProtectionStatus() => AntivirusStatusEnum.Error;
        public override void OpenSettings() { return; }
    }
}