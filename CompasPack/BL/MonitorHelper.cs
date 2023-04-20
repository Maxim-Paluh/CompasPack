using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CompasPack.BL
{
   public static  class MonitorHelper
    {
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
                    Name = "Full HD",
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

            return resolution.Where(x => x.Size.Any(t => t.Width == size.Width && t.Height == size.Height)).Select(x => x.Name).ToList();
        }
    }
    public class Resolution
    {
        public string Name { get; set; }
        public List<Size> Size { get; set; }
    }
}
