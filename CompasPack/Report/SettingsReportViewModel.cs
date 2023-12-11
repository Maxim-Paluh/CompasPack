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
using System.Collections.ObjectModel;

namespace CompasPack.ViewModel
{
    public class SettingsReportViewModel : ViewModelBase
    {
        private CPU _cpu;
        private Motherboard _motherboard;
        private Memory _memory;
        private VideoController _videoController;
        private LaptopBattery _laptopBattery;
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
        public Monitor Monitor
        {
            get { return _monitor; }
            set { _monitor = value; }
        }

        public ObservableCollection<LaptopHardWare> LaptopHardWares { get; set; }

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

    public class Monitor
    {
        private MonitorName _monitorName;
        private MonitorModel _monitorModel;

        private MonitorType _monitorType;
        private MonitorSize _monitorSize;
        private AspectRatio _aspectRatio;


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
        public AspectRatio AspectRatio
        {
            get { return _aspectRatio; }
            set { _aspectRatio = value; }
        }

        public ObservableCollection<MonitorInterface> MonitorInterfaces { get; set; }
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
    public class AspectRatio : ReportBase
    {

    }

    public class MonitorInterface : ViewModelBase
    {
        private bool _isSelect;
        public string Name { get; set; }
        public bool IsSelect
        {
            get { return _isSelect; }
            set
            {
                _isSelect = value;
                OnPropertyChanged();
            }
        }
    }

    public class LaptopHardWare : MonitorInterface
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