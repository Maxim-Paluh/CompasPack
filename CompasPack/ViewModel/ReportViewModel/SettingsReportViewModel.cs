﻿using System;
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
        private List<string> _powersupply;
        private Dictionary<string, List<string>> _laptops;

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
        public Dictionary<string, List<string>> Laptops
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
