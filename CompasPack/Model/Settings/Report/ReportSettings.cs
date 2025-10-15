using System;
using System.Collections.Generic;

using CompasPack.Model.Enum;

namespace CompasPack.Model.Settings
{
    public class ReportSettings
    {
        public ReportPaths ReportPaths { get; set; }
        
        public CPU CPU { get; set; }                                                            // Common
        public Memory Memory { get; set; }                                                      // Common
        public VideoController VideoController { get; set; }                                    // Common

        public LaptopBattery LaptopBattery { get; set; }                                        // Laptops
        public Dictionary<string, List<string>> LaptopsBrandAndModel { get; set; }              // Laptops
        public LaptopInterface LaptopInterface { get; set; }                                     // Laptops

        public Motherboard Motherboard { get; set; }                                            //PC
        public List<string> PCPowerSupply { get; set; }                                         //PC 

        public Monitor Monitor { get; set; }                                                    //Monitor

        public Dictionary<DocxReplaceTextEnum, string> DocxReplaceTextDictionary { get; set; }
    }
}