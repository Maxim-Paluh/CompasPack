using CompasPack.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.ViewAndViewModel
{
    public class AntivirusStatus
    {
        public AntivirusStatusEnum RealtimeMonitoringStatus { get; set; }
        public AntivirusStatusEnum TamperProtectionStatus { get; set; }
    }
}
