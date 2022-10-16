using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Data
{
    public static class ReportHelper
    {
        public static UserReport GetUserReport()
        {
            return new UserReport()
            {
                Motherboard = new Motherboard()
                {
                    XPath = "/Report/Page[5]/Group[1]/Item[2]/Value",
                    Regex = new List<string>() { "\\((?:[^)(]|\\([^)(]*\\))*\\)" }
                }
            };
        }
    }




    public class UserReport : ViewModelBase
    {
        private Motherboard _motherboard;
        public Motherboard Motherboard
        {
            get { return _motherboard; }
            set
            {
                _motherboard = value;
                OnPropertyChanged();
            }
        }
        public Memory Memory { get; set; }
    }

    public class CPU : ViewModelBase
    {
        public List<string> Regex { get; set; }
    }

    public class Motherboard : ViewModelBase
    {
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
        public List<string> Regex { get; set; }
    }

    public class Memory
    {
        public MemorySize MemorySize { get; set; }
        public MemoryFrequency MemoryFrequency { get; set; }

        public string NameSPD { get; set; }
        public string XPathSPD { get; set; }
        public List<MemorySPD> MemorySPDs { get; set; }
    }

    public class MemorySize
    {
        public string XPath { get; set; }
        public List<string> Regex { get; set; }
    }

    public class MemoryFrequency
    {
        public string XPath { get; set; }
        public List<string> Regex { get; set; }
    }

    public class MemorySPD
    {
        public MemorySPDName MemorySPDName { get; set; }
        public MemorySPDSize MemorySPDSize { get; set; }
        public MemorySPDType MemorySPDType { get; set; }
    }

    public class MemorySPDName
    {
        public string XPath { get; set; }
        public List<string> Regex { get; set; }
    }
    public class MemorySPDSize
    {
        public string XPath { get; set; }
        public List<string> Regex { get; set; }
    }

    public class MemorySPDType
    {
        public string XPath { get; set; }
        public List<string> Regex { get; set; }
    }

    public class MemorySPDSpeed
    {
        public string XPath { get; set; }
        public List<string> Regex { get; set; }
    }

}
