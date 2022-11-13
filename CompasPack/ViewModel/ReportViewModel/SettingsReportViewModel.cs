using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Windows.Controls;
using System.Windows.Interop;

namespace CompasPack.ViewModel
{
    public static class SettingsReportHelper
    {
        public static SettingsReportViewModel GetSettingsReport()
        {
            return new SettingsReportViewModel()
            {
                CPU = new CPU()
                {
                    Regex = new List<string>() { "\\((?:[^)(]|\\([^)(]*\\))*\\)", "\\{(?:[^}{]|\\{[^}{]*\\))*\\}", "Test" }
                },
                Motherboard = new Motherboard()
                {
                    XPath = "/Report/Page[5]/Group[1]/Item[2]/Value",
                    Regex = new List<string>() { "\\((?:[^)(]|\\([^)(]*\\))*\\)" }
                },
                Memory = new Memory()
                {
                    MemoryType = new MemoryType()
                    {
                        XPath = "/Report/Page[5]/Group[3]/Item[1]/Value",
                        Regex = new List<string>() { " ??SDRAM" }
                    },
                    MemoryFrequency = new MemoryFrequency()
                    {
                        XPath = "/Report/Page[5]/Group[3]/Item[5]/Value",
                        Regex = new List<string>() { "\\D" }
                    }
                },
                VideoController = new VideoController()
                {
                    Regex = new List<string>() { "\\((?:[^)(]|\\([^)(]*\\))*\\)" }
                },
                PowerSupply = new List<string>() { "1stPlayer", "2N", "AeroCool", "Antec", "Argus", "ASPower", "ASUS", "Azza", "Be quiet!", "Casecom", "Chenbro", "Chieftec", "Chieftronic", "CoolerMaster", 
                    "Corsair", "Cougar", "Deepcool", "Dell", "Delux", "Enermax", "Extradigital", "Fractal Design", "Frime", "FrimeCom", "FSP", "Gamemax", "GIGABYTE", "Golden Field", "GreatWall", "Greenvision",
                    "High Power", "HP", "IBM", "INTEL", "Inter-Tech", "Lenovo", "Logic concept", "LogicPower", "Meraki", "Modecom", "MSI", "Nikon", "NZXT", "Qdion", "QNap", "Qube", "Rezone Case", "Seasonic",
                    "Segotep", "Silver Stone", "Supermicro", "TECNOWARE", "ThermalTake", "Vinga", "Xilence", "Zalman" },
                
                Laptops = new List<string>() {  "Acer", "Apple", "Asus", "Dell", "Fujitsu", "Gigabyte", "Google", "HP", "Huawei", "Lenovo", "LG", "MSI", "Panasonic", "Philips", "Samsung", "Sony", "Toshiba", "VAIO",
                                                "AGB Supreme Technology", "Alienware", "Avell", "Axioo", "BenQ", "Bmax", "BOXX Technologies", "Casper", "CHUWI", "Clevo", "Corsair", "CyberPowerPC", "Digital Storm", "Durabook", "Dynabook", "Eluktronics", "Epson", "Eurocom", "Evoo", "Falcon Northwest", "Framework Computer", "Gateway", "Geo", "Getac", "Grundig", "HCL", "Honor", "HTC", "Hyundai Technology", "IBM", "Illegear", "Lava International", "Machenike", "Maguay", "Medion", "Metabox", "Microsoft", "Microtech", "Monster Notebook", "Multilaser", "NEC", "Njoy", "Nokia", "Obsidian-PC", "Olivetti", "Onkyo", "Optima", "Origin PC", "OverPowered", "Packard Bell", "Positivo", "Purism", "Razer", "Realme", "Sager Notebook computers", "Shenzhen Jumper Technology", "Slimbook", "Star Labs", "System76", "TUXEDO Computers", "UMAX", "Vastking", "Velocity Micro", "Vestel", "VIT", "Walmart", "Walton", "Wipro", "Xiaomi", "XMG", "Xolo", "Zeuslap", "Zyrex"
                                             }
            };
        }
    }




    public class SettingsReportViewModel : ViewModelBase
    {
        private CPU _cpu;
        private Motherboard _motherboard;
        private Memory _memory;
        private VideoController _videoController;
        private List<string> _powersupply;
        private List<string> _laptops;

        //---------------------------------
        public CPU CPU
        {
            get { return _cpu; }
            set
            {
                _cpu = value;
                OnPropertyChanged();
            }
        }
        public Motherboard Motherboard
        {
            get { return _motherboard; }
            set
            {
                _motherboard = value;
                OnPropertyChanged();
            }
        }
        public Memory Memory
        {
            get { return _memory; }
            set
            {
                _memory = value;
                OnPropertyChanged();
            }
        }
        public VideoController VideoController
        {
            get { return _videoController; }
            set
            {
                _videoController = value;
                OnPropertyChanged();
            }
        }
        public List<string> PowerSupply
        {
            get { return _powersupply; }
            set
            {
                _powersupply = value;
                OnPropertyChanged();
            }
        }

        public List<string> Laptops
        {
            get { return _laptops; }
            set
            {
                _laptops = value;
                OnPropertyChanged();
            }
        }
    }

    public class CPU : ReportBase
    {
    }

    public class Motherboard : ReportBase
    {

    }

    public class Memory : ViewModelBase
    {
        private MemoryType _memoryType;
        private MemoryFrequency _memoryFrequency;
        public MemoryType MemoryType
        {
            get { return _memoryType; }
            set
            {
                _memoryType = value;
                OnPropertyChanged();
            }
        }
        public MemoryFrequency MemoryFrequency
        {
            get { return _memoryFrequency; }
            set
            {
                _memoryFrequency = value;
                OnPropertyChanged();
            }
        }
    }
    public class MemoryType : ReportBase
    {
    }
    public class MemoryFrequency : ReportBase
    {
    }
    public class VideoController : ReportBase
    {

    }


    public class ReportBase : ViewModelBase
    {
        private List<string> _regex;
        private string xPath;

        public string XPath
        {
            get { return xPath; }
            set
            {
                xPath = value;
                OnPropertyChanged();
            }
        }
        public List<string> Regex
        {
            get { return _regex; }
            set
            {
                _regex = value;
                OnPropertyChanged();
            }
        }
    }

}
