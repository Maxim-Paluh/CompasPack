using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompasPack.Data
{
    public static class ReportHelper
    {
        public static UserReport GetUserReport()
        {
            return new UserReport()
            {
                CPU = new CPU()
                {
                    Regex = new List<string>() { "\\((?:[^)(]|\\([^)(]*\\))*\\)", "\\{(?:[^}{]|\\{[^}{]*\\))*\\}", "Test" }
                },
                Motherboard = new Motherboard()
                {
                    XPath = "/Report/Page[5]/Group[1]/Item[2]/Value",
                    Regex = new List<string>() { "Test" }
                },
                Memory = new Memory()
                {
                    MemoryType = new MemoryType()
                    {
                        XPath= "/Report/Page[5]/Group[3]/Item[1]/Value",
                        Regex = new List<string>() { " ??SDRAM" }
                    },
                    MemoryFrequency = new MemoryFrequency()
                    {
                        XPath = "/Report/Page[5]/Group[3]/Item[5]/Value",
                        Regex = new List<string>() { "\\D" }
                    }
                }
                
            };
        }
    }




    public class UserReport : ViewModelBase
    {
        private CPU _cpu;
        private Motherboard _motherboard;
        private Memory _memory;
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
