using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Settings
{
    public class Monitor
    {
        public MonitorName MonitorName { get; set; }
        public MonitorModel MonitorModel { get; set; }
        public MonitorType MonitorType { get; set; }
        public MonitorSize MonitorSize { get; set; }
        public MonitorAspectRatio MonitorAspectRatio { get; set; }
        public List<MonitorPort> MonitorInterfaces { get; set; }
        public List<string> MonitorsBrand { get; set; }
    }
}
