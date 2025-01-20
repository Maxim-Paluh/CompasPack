using System.Collections.Generic;

namespace CompasPack.Settings
{
    public class ReportSettings
    {
        public CPUReportSettings CPUReportSettings { get; set; }                            // Common Folder
        public MemoryReportSettings MemoryReportSettings { get; set; }                      // Common Folder
        public VideoControllerReportSettings VideoControllerReportSettings { get; set; }    // Common Folder
        public LaptopBatteryReportSettings LaptopBatteryReportSettings { get; set; }        // Laptops Folder
        public Dictionary<string, List<string>> LaptopsBrandAndModel { get; set; }          // Laptops
        public List<LaptopHardWare> LaptopHardWares { get; set; }                           // Laptops Folder
        public MotherboardReportSettings MotherboardReportSettings { get; set; }            //PC Folder
        public List<string> PCPowerSupply { get; set; }                                     //PC 
        public MonitorReportSettings MonitorReportSettings { get; set; }                    //Monitor Folder
        public Dictionary<DocxReplaceTextEnum, string> ReportViewModelDictionary { get; set; }
    }
}