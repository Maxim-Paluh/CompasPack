using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    class LaptopMonitorViewModel : ReportViewModelBase, IReportViewModel, IDataErrorInfo
    {
        private string _laptopMonitorType;
        private string _laptopMonitorSize;
        private string _laptopMonitorResolution;

        public string LaptopMonitorResolution
        {
            get { return _laptopMonitorResolution; }
            set
            {
                _laptopMonitorResolution = value;
                OnPropertyChanged();
            }
        }

        public string LaptopMonitorSize
        {
            get { return _laptopMonitorSize; }
            set
            {
                _laptopMonitorSize = value;
                OnPropertyChanged();
            }
        }

        public string LaptopMonitorType
        {
            get { return _laptopMonitorType; }
            set
            {

                _laptopMonitorType = value;
                OnPropertyChanged();
            }
        }

        public LaptopMonitorViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
        }

        public string this[string columnName] => throw new NotImplementedException();

        public string Error => throw new NotImplementedException();

        public void Load()
        {
            var tempType = Document.XPathSelectElement(SettingsReport.LaptopMonitor.LaptopMonitorType.XPath);
            if (tempType != null)
                LaptopMonitorType = tempType.Value;
            else
                LaptopMonitorType = string.Empty;

            var tempSize = Document.XPathSelectElement(SettingsReport.LaptopMonitor.LaptopMonitorSize.XPath);
            if (tempSize != null)
                LaptopMonitorSize = tempSize.Value;
            else
                LaptopMonitorSize = string.Empty;

            if (!string.IsNullOrWhiteSpace(LaptopMonitorType))
            {
                var tempLaptopMonitorType = LaptopMonitorType;
                foreach (var item in SettingsReport.LaptopMonitor.LaptopMonitorType.Regex)
                    tempLaptopMonitorType = Regex.Replace(tempLaptopMonitorType, item, "");
                Result = $"{tempLaptopMonitorType}";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(LaptopMonitorSize))
                {
                    var tempLaptopMonitorSize = LaptopMonitorSize;
                    foreach (var item in SettingsReport.LaptopMonitor.LaptopMonitorSize.Regex)
                        tempLaptopMonitorSize = Regex.Replace(tempLaptopMonitorSize, item, "");
                    Result = $"{tempLaptopMonitorSize}";
                }
            }
            var resolution = GetOptimalScreenResolution();

            LaptopMonitorResolution = $"{resolution.Width}x{resolution.Height}";

            var nameResolution = string.Join(", ", GetNameResolution(resolution));
            if (!string.IsNullOrWhiteSpace(nameResolution))
                LaptopMonitorResolution += $" {nameResolution}";
        }

        public static Size GetOptimalScreenResolution()
        {

            UInt32 maxHResolution = 0;
            UInt32 maxVResolution = 0;
            UInt32 maxHForMaxVResolution = 0;
            var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM CIM_VideoControllerResolution");

            foreach (ManagementObject item in searcher.Get())
            {
                if ((UInt32)item["HorizontalResolution"] >= maxHResolution)
                {
                    maxHResolution = (UInt32)item["HorizontalResolution"];
                    if ((UInt32)item["VerticalResolution"] > maxVResolution || maxHForMaxVResolution != maxHResolution)
                    {
                        maxVResolution = (UInt32)item["VerticalResolution"];
                        maxHForMaxVResolution = (UInt32)item["HorizontalResolution"];
                    }
                }
            }

            return new Size(maxHResolution, maxVResolution);
        }
        public static List<string> GetNameResolution(Size size)
        {
            var resolution = new List<Resolution>()
            {
                new Resolution()
                {
                    Name = "VGA",
                    Size = new List<Size>()
                    {
                        new Size(640, 480),
                    }
                },
                new Resolution()
                {
                    Name = "SVGA",
                    Size = new List<Size>()
                    {
                        new Size(800, 600),
                        new Size(640, 480)

                    }
                },
                new Resolution()
                {
                    Name = "XGA",
                    Size = new List<Size>()
                    {
                        new Size(1024,768),
                        new Size(640, 480)
                    }
                },
                new Resolution()
                {
                    Name = "XGA+",
                    Size = new List<Size>()
                    {
                        new Size(1152,864),
                        new Size(1152,870),
                        new Size(1152,900),

                    }
                },
                new Resolution()
                {
                    Name = "HD",
                    Size = new List<Size>()
                    {
                        new Size(1280,720),
                        new Size(1360,768),
                        new Size(1366,768),

                    }
                },
                new Resolution()
                {
                    Name = "WXGA",
                    Size = new List<Size>()
                    {
                        new Size(1280,800),

                    }
                },
                new Resolution()
                {
                    Name = "SXGA",
                    Size = new List<Size>()
                    {
                        new Size(1280,1024),

                    }
                },
                new Resolution()
                {
                    Name = "SXGA+",
                    Size = new List<Size>()
                    {
                        new Size(1400,1050),

                    }
                },
                new Resolution()
                {
                    Name = "WXGA+",
                    Size = new List<Size>()
                    {
                        new Size(1440,900),

                    }
                },
                new Resolution()
                {
                    Name = "HD+",
                    Size = new List<Size>()
                    {
                        new Size(1600,900),

                    }
                },
                new Resolution()
                {
                    Name = "UXGA",
                    Size = new List<Size>()
                    {
                        new Size(1600,1200),

                    }
                },
                new Resolution()
                {
                    Name = "WSXGA+",
                    Size = new List<Size>()
                    {
                        new Size(1680,1050),

                    }
                },
                new Resolution()
                {
                    Name = "FHD",
                    Size = new List<Size>()
                    {
                        new Size(1920,1080),

                    }
                },
                new Resolution()
                {
                    Name = "WUXGA",
                    Size = new List<Size>()
                    {
                        new Size(1920,1200),

                    }
                },
                new Resolution()
                {
                    Name = "DCI 2K",
                    Size = new List<Size>()
                    {
                        new Size(2048,1080),

                    }
                },
                new Resolution()
                {
                    Name = "QWXGA",
                    Size = new List<Size>()
                    {
                        new Size(2048,1152),

                    }
                },
                new Resolution()
                {
                    Name = "QXGA",
                    Size = new List<Size>()
                    {
                        new Size(2048,1536),

                    }
                },
                new Resolution()
                {
                    Name = "QHD",
                    Size = new List<Size>()
                    {
                        new Size(2560,1440),

                    }
                },
                new Resolution()
                {
                    Name = "WQXGA",
                    Size = new List<Size>()
                    {
                        new Size(2560,1600),

                    }
                },
                new Resolution()
                {
                    Name = "QSXGA",
                    Size = new List<Size>()
                    {
                        new Size(2560,2048),

                    }
                },
                new Resolution()
                {
                    Name = "QWXGA+",
                    Size = new List<Size>()
                    {
                        new Size(2880,1800),

                    }
                },
                new Resolution()
                {
                    Name = "WQSXGA",
                    Size = new List<Size>()
                    {
                        new Size(3200,2048),

                    }
                },
                new Resolution()
                {
                    Name = "QUXGA",
                    Size = new List<Size>()
                    {
                        new Size(3200,2400),

                    }
                },
                new Resolution()
                {
                    Name = "UWQHD",
                    Size = new List<Size>()
                    {
                        new Size(3440,1440),

                    }
                },
                new Resolution()
                {
                    Name = "UW4K",
                    Size = new List<Size>()
                    {
                        new Size(3840,1600),

                    }
                },
                new Resolution()
                {
                    Name = "UHD 4K",
                    Size = new List<Size>()
                    {
                        new Size(3840,2160),

                    }
                },
                new Resolution()
                {
                    Name = "WQUXGA",
                    Size = new List<Size>()
                    {
                        new Size(3840,2400),

                    }
                },
                new Resolution()
                {
                    Name = "DCI 4K",
                    Size = new List<Size>()
                    {
                        new Size(4096,2160),

                    }
                },
                new Resolution()
                {
                    Name = "HXGA",
                    Size = new List<Size>()
                    {
                        new Size(4096,3072),

                    }
                },

            };

            return  resolution.Where(x => x.Size.Any(t => t.Width == size.Width && t.Height == size.Height)).Select(x=>x.Name).ToList();
        }



        public class Resolution
        {
            public string Name { get; set; }
            public List<Size> Size { get; set; }
        }
    }
}
