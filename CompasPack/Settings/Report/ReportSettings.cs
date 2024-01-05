using System.Collections.Generic;

namespace CompasPack.Settings
{
    public class ReportSettings
    {
        public CPUReportSettings CPUReportSettings { get; set; }                            // Common
        public MemoryReportSettings MemoryReportSettings { get; set; }                      // Common
        public VideoControllerReportSettings VideoControllerReportSettings { get; set; }    // Common
        public LaptopBatteryReportSettings LaptopBatteryReportSettings { get; set; }        // Laptops
        public Dictionary<string, List<string>> LaptopsBrandAndModel { get; set; }          // Laptops
        public List<LaptopHardWare> LaptopHardWares { get; set; }                           // Laptops
        public MotherboardReportSettings MotherboardReportSettings { get; set; }            //PC
        public List<string> PCPowerSupply { get; set; }                                     //PC
        public MonitorReportSettings MonitorReportSettings { get; set; }                    //Monitor            
        public Dictionary<DocxReplaceTextEnum, string> ReportViewModelDictionary { get; set; }
    }
    public class CPUReportSettings : ReportBase { }
    public class MotherboardReportSettings : ReportBase { }
    public class MemoryReportSettings
    {
        public MemoryType MemoryType { get; set; }
        public MemoryFrequency MemoryFrequency { get; set; }
    }
    public class MemoryType : ReportBase { }
    public class MemoryFrequency : ReportBase { }
    public class VideoControllerReportSettings : ReportBase { }
    public class LaptopBatteryReportSettings : ReportBase { }
    public class MonitorReportSettings
    {
        public MonitorName MonitorName { get; set; }
        public MonitorModel MonitorModel { get; set; }
        public MonitorType MonitorType { get; set; }
        public MonitorSize MonitorSize { get; set; }
        public AspectRatio AspectRatio { get; set; }
        public List<MonitorInterface> MonitorInterfaces { get; set; }
        public List<string> MonitorsBrand { get; set; }
    }  
    public class MonitorName : ReportBase { }
    public class MonitorModel : ReportBase { }
    public class MonitorType : ReportBase { }
    public class MonitorSize : ReportBase { }
    public class AspectRatio : ReportBase { }
    public class MonitorInterface : HardWare{ }
    public class LaptopHardWare : HardWare { }
    public class HardWare
    {
        public string Name { get; set; }
        public bool IsSelect { get; set; }
    }
    public class ReportBase
    {
        public string XPath { get; set; }
        public List<string> Regex { get; set; }

    }

    public enum DocxReplaceTextEnum 
    {
        CPU,
        Memory,
        MonitorDiagonal,
        PhysicalDisk,
        VideoController,
        LaptopBattery,
        Laptop_Brand,
        Laptop_Line_Model,
        LaptopOther,
        MonitorAspectRatio,
        Monitor_Brand,
        MonitorOther,
        MonitorResolution,
        Motherboard,
        PCCase,
        PowerSupply,
        ReportId
    }
}