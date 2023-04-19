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
    public class SettingsReportViewModel : ViewModelBase
    {
        private CPU _cpu;
        private Motherboard _motherboard;
        private Memory _memory;
        private VideoController _videoController;
        private LaptopBattery _laptopBattery;
        private LaptopOther _laptopOther;
        private Monitor _monitor;

        
        private List<string> _powersupply;
        private Dictionary<string, List<string>> _laptops;
        private List<string> _monitors;


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
        public LaptopBattery LaptopBattery
        {
            get { return _laptopBattery; }
            set { _laptopBattery = value; }
        }
        public LaptopOther LaptopOther
        {
            get { return _laptopOther; }
            set { _laptopOther = value; }
        }
        public Monitor Monitor
        {
            get { return _monitor; }
            set { _monitor = value; }
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
        public Dictionary<string, List<string>> Laptops
        {
            get { return _laptops; }
            set
            {
                _laptops = value;
                OnPropertyChanged();
            }
        }
        public List<string> Monitors
        {
            get { return _monitors; }
            set
            {
                _monitors = value;
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

    public class LaptopBattery : ReportBase
    {
    }
    public class LaptopOther : ReportBase
    { }

    public class Monitor
    {
        private MonitorName _monitorName;
        private MonitorModel _monitorModel;

        private MonitorType _monitorType;
        private MonitorSize _monitorSize;


        private MonitorAspectRatio _monitorAspectRatio;


        public MonitorName MonitorName
        {
            get { return _monitorName; }
            set
            {
                _monitorName = value;
            }
        }
        public MonitorModel MonitorModel
        {
            get { return _monitorModel; }
            set { _monitorModel = value; }
        }

        public MonitorType MonitorType
        {
            get { return _monitorType; }
            set { _monitorType = value; }
        }
        public MonitorSize MonitorSize
        {
            get { return _monitorSize; }
            set { _monitorSize = value; }
        }
        public MonitorAspectRatio MonitorAspectRatio
        {
            get { return _monitorAspectRatio; }
            set { _monitorAspectRatio = value; }
        }
    }
    
    public class MonitorName : ReportBase
    {

    }
    public class MonitorModel : ReportBase
    {

    }


    public class MonitorType : ReportBase
    {

    }
    public class MonitorSize : ReportBase
    {

    }
    public class MonitorAspectRatio : ReportBase
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


//LaptopMonitorType = new LaptopMonitorType()
//{
//    XPath = "//Item[contains(translate(., 'АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯI', 'Абвгґдеєжзиіїйклмнопрстуфхцчшщьюяi') , \"тип монiтора\")]/Value",
//    
//},
//                    LaptopMonitorSize = new LaptopMonitorSize()
//                    {
//                       
//                        
//                    }