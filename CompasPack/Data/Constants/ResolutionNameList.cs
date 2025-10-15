using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

using CompasPack.Model.ViewAndViewModel;

namespace CompasPack.Data.Constants
{
    internal class ResolutionNameList
    {
        public static readonly List<ResolutionName> Resolutions = new List<ResolutionName>()
            {
                new ResolutionName()
                {
                    Name = "VGA",
                    Size = new List<Size>()
                    {
                        new Size(640, 480),
                    }
                },
                new ResolutionName()
                {
                    Name = "SVGA",
                    Size = new List<Size>()
                    {
                        new Size(800, 600),
                        new Size(640, 480)

                    }
                },
                new ResolutionName()
                {
                    Name = "XGA",
                    Size = new List<Size>()
                    {
                        new Size(1024,768),
                        new Size(640, 480)
                    }
                },
                new ResolutionName()
                {
                    Name = "XGA+",
                    Size = new List<Size>()
                    {
                        new Size(1152,864),
                        new Size(1152,870),
                        new Size(1152,900),

                    }
                },
                new ResolutionName()
                {
                    Name = "HD",
                    Size = new List<Size>()
                    {
                        new Size(1280,720),
                        new Size(1360,768),
                        new Size(1366,768),

                    }
                },
                new ResolutionName()
                {
                    Name = "WXGA",
                    Size = new List<Size>()
                    {
                        new Size(1280,800),

                    }
                },
                new ResolutionName()
                {
                    Name = "SXGA",
                    Size = new List<Size>()
                    {
                        new Size(1280,1024),

                    }
                },
                new ResolutionName()
                {
                    Name = "SXGA+",
                    Size = new List<Size>()
                    {
                        new Size(1400,1050),

                    }
                },
                new ResolutionName()
                {
                    Name = "WXGA+",
                    Size = new List<Size>()
                    {
                        new Size(1440,900),

                    }
                },
                new ResolutionName()
                {
                    Name = "HD+",
                    Size = new List<Size>()
                    {
                        new Size(1600,900),

                    }
                },
                new ResolutionName()
                {
                    Name = "UXGA",
                    Size = new List<Size>()
                    {
                        new Size(1600,1200),

                    }
                },
                new ResolutionName()
                {
                    Name = "WSXGA+",
                    Size = new List<Size>()
                    {
                        new Size(1680,1050),

                    }
                },
                new ResolutionName()
                {
                    Name = "Full HD",
                    Size = new List<Size>()
                    {
                        new Size(1920,1080),

                    }
                },
                new ResolutionName()
                {
                    Name = "WUXGA",
                    Size = new List<Size>()
                    {
                        new Size(1920,1200),

                    }
                },
                new ResolutionName()
                {
                    Name = "DCI 2K",
                    Size = new List<Size>()
                    {
                        new Size(2048,1080),

                    }
                },
                new ResolutionName()
                {
                    Name = "QWXGA",
                    Size = new List<Size>()
                    {
                        new Size(2048,1152),

                    }
                },
                new ResolutionName()
                {
                    Name = "QXGA",
                    Size = new List<Size>()
                    {
                        new Size(2048,1536),

                    }
                },
                new ResolutionName()
                {
                    Name = "QHD",
                    Size = new List<Size>()
                    {
                        new Size(2560,1440),

                    }
                },
                new ResolutionName()
                {
                    Name = "WQXGA",
                    Size = new List<Size>()
                    {
                        new Size(2560,1600),

                    }
                },
                new ResolutionName()
                {
                    Name = "QSXGA",
                    Size = new List<Size>()
                    {
                        new Size(2560,2048),

                    }
                },
                new ResolutionName()
                {
                    Name = "QWXGA+",
                    Size = new List<Size>()
                    {
                        new Size(2880,1800),

                    }
                },
                new ResolutionName()
                {
                    Name = "WQSXGA",
                    Size = new List<Size>()
                    {
                        new Size(3200,2048),

                    }
                },
                new ResolutionName()
                {
                    Name = "QUXGA",
                    Size = new List<Size>()
                    {
                        new Size(3200,2400),

                    }
                },
                new ResolutionName()
                {
                    Name = "UWQHD",
                    Size = new List<Size>()
                    {
                        new Size(3440,1440),

                    }
                },
                new ResolutionName()
                {
                    Name = "UW4K",
                    Size = new List<Size>()
                    {
                        new Size(3840,1600),

                    }
                },
                new ResolutionName()
                {
                    Name = "UHD 4K",
                    Size = new List<Size>()
                    {
                        new Size(3840,2160),

                    }
                },
                new ResolutionName()
                {
                    Name = "WQUXGA",
                    Size = new List<Size>()
                    {
                        new Size(3840,2400),

                    }
                },
                new ResolutionName()
                {
                    Name = "DCI 4K",
                    Size = new List<Size>()
                    {
                        new Size(4096,2160),

                    }
                },
                new ResolutionName()
                {
                    Name = "HXGA",
                    Size = new List<Size>()
                    {
                        new Size(4096,3072),

                    }
                },

            };

        public static List<string> GetNameResolution(Size size)
        {
            return Resolutions.Where(x => x.Size.Any(t => t.Width == size.Width && t.Height == size.Height)).Select(x => x.Name).ToList();
        }
    }
}
