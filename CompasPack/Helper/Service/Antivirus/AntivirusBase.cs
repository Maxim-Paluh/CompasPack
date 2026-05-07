using CompasPack.Model.Enum;
using CompasPack.Model.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public abstract class AntivirusBase : IAntivirus
    {
        public AntivirusInfo AntivirusInfo { get; }

        protected AntivirusBase(AntivirusInfo info)
        {
            AntivirusInfo = info;
        }

        public abstract bool IsControlled { get; }
        public abstract Task<AntivirusStatus> CheckStatus();
        public abstract Task<AntivirusStatusEnum> DisableRealTimeMonitoring();
        public abstract Task<AntivirusStatusEnum> EnableRealTimeMonitoring();
        public abstract Task<AntivirusStatusEnum> GetRealTimeMonitoringStatus();
        public abstract Task<AntivirusStatusEnum> GetTamperProtectionStatus();
        public abstract void OpenSettings();
    }
}


