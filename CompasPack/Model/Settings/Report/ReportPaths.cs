using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Settings
{
    public class ReportPaths : ICloneable
    {
        public string AidaExeFilePath { get; set; }
        public string ReportRPF { get; set; }           // Використовується для формування звіту по залізу (CompasPackLog/Report.xml)
        public string LogInstallRPF { get; set; }       // Використовується для формування звіту по залізу та програмам (CompasPackLog/LogInstall_*.htm) Або (LaptopReportPath|PCReportPath) 
        public string MonitorReportRPF { get; set; }    // Використовується для формування звіту по монітору (MonitorReportPath)

        public string LaptopReportPath { get; set; }
        public string PCReportPath { get; set; }
        public string MonitorReportPath { get; set; }

        public string LaptopPricePath { get; set; }
        public string PCPricePath { get; set; }
        public string MonitorPricePath { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
