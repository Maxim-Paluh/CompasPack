using CompasPack.BL;
using CompasPack.Enum;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    class LaptopOtherViewModel : ReportViewModelBase, IReportViewModel, IDataErrorInfo
    {
        private Hardware? _webCam;
        private Hardware? _microphone;

        private string _laptopMonitorResolution;
        private bool _wifi;
        private bool _cardReader;
        private bool _vga;
        private bool _hdmi;
        private bool _displayPort;
        private bool _eSATA;
        private bool _DVDRom;
        private bool _ethernet;
        private bool _touchScreen;


        public bool TouchScreen
        {
            get { return _touchScreen; }
            set
            {
                _touchScreen = value;
                OnPropertyChanged();
            }
        }
        public bool DVDRom
        {
            get { return _DVDRom; }
            set
            {
                _DVDRom = value;
                OnPropertyChanged();
            }
        }
        public bool eSATA
        {
            get { return _eSATA; }
            set
            {
                _eSATA = value;
                OnPropertyChanged();
            }
        }
        public bool DisplayPort
        {
            get { return _displayPort; }
            set
            {
                _displayPort = value;
                OnPropertyChanged();
            }
        }
        public bool HDMI
        {
            get { return _hdmi; }
            set
            {
                _hdmi = value;
                OnPropertyChanged();
            }
        }
        public bool VGA
        {
            get { return _vga; }
            set
            {
                _vga = value;
                OnPropertyChanged();
            }
        }
        public bool CardReader
        {
            get { return _cardReader; }
            set
            {
                _cardReader = value;
                OnPropertyChanged();
            }
        }
        public bool WiFi
        {
            get { return _wifi; }
            set
            {
                _wifi = value;
                OnPropertyChanged();
            }
        }
        public bool Ethernet
        {
            get { return _ethernet; }
            set
            {
                _ethernet = value;
                OnPropertyChanged();
            }
        }
        public string LaptopMonitorResolution
        {
            get { return _laptopMonitorResolution; }
            set
            {
                _laptopMonitorResolution = value;
                OnPropertyChanged();
            }
        }

        public Hardware? WebCam
        {
            get { return _webCam; }
            set
            {
                _webCam = value;
                OnPropertyChanged();
            }
        }
        public Hardware? Microphone
        {
            get { return _microphone; }
            set
            {
                _microphone = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<Hardware> HardwareValues
        {
            get
            {
                return System.Enum.GetValues(typeof(Hardware)).Cast<Hardware>();
            }
        }
        public LaptopOtherViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;

            TestWebCamCommand = new DelegateCommand(OnTestWebCam);
            TestMicrophoneCommand = new DelegateCommand(OnTestMicrophone);
            ChangeHardwareCommand = new DelegateCommand(OnChangeHardware);

            WiFi = true;
            Ethernet = true;
            CardReader = true;
        }

        private void OnChangeHardware()
        {
            Result = string.Join(", ", GetHardware());
        }

        private void OnTestMicrophone()
        {
            OpenUrl("https://webcammictest.com/check-mic.html");
        }
        private void OnTestWebCam()
        {
            OpenUrl("https://webcammictest.com");
        }
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
        public void Load()
        {
            var resolution = GetOptimalScreenResolution();

            LaptopMonitorResolution = $"{resolution.Width}x{resolution.Height}";

            var nameResolution = string.Join(", ", GetNameResolution(resolution));
            if (!string.IsNullOrWhiteSpace(nameResolution))
                LaptopMonitorResolution += $" {nameResolution}";

            Result = string.Join(", ", GetHardware());
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
        public class Resolution
        {
            public string Name { get; set; }
            public List<Size> Size { get; set; }
        }

        private List<string> GetHardware()
        { 
            var temp = new List<string>();
            if(!string.IsNullOrWhiteSpace(LaptopMonitorResolution))
                temp.Add(LaptopMonitorResolution);
            if (TouchScreen)
                temp.Add($"TouchScreen");
            if (WiFi)
                temp.Add($"Wi-Fi");
            if (HDMI)
                temp.Add($"HDMI");
            if (VGA)
                temp.Add($"VGA");
            if (DisplayPort)
                temp.Add($"DisplayPort");
            if (DVDRom)
                temp.Add($"DVDRom");
            if (CardReader)
                temp.Add($"CardReader");
            if (eSATA)
                temp.Add($"eSATA");
            if (Ethernet)
                temp.Add($"Ethernet");
            return temp;
        }

        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "WebCam":
                        if (WebCam == null)
                            error = "Введи значення";
                        break;
                    case "Microphone":
                        if (Microphone == null)
                            error = "Введи значення";
                        break;
                }
                return error;
            }
        }

        public ICommand TestWebCamCommand { get; set; }
        public ICommand TestMicrophoneCommand { get; set; }    
        public ICommand ChangeHardwareCommand { get; set; }
    }
}
