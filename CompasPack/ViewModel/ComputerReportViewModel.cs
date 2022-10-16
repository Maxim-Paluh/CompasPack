using CompasPack.BL;
using CompasPack.Data;
using CompasPack.View;
using CompasPakc.BL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using CompasPack.Data;

using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class ComputerReportViewModel : ViewModelBase, IDetailViewModel
    {
        private string _processorNameSource;
        private string _processorXPath;
        private string _processorName;

        private string _processorParameters;

        private string _baseBoardName;
        private readonly IIOManager _iOManager;
        private UserReport _userReport;

        public ComputerReportViewModel(IIOManager iOManager)
        {
            _iOManager = iOManager;
        }
        public UserReport UserReport
        {
            get { return _userReport; }
            set
            {
                _userReport = value;
                OnPropertyChanged();
            }
        }

        public string ProcessorNameSource
        {
            get { return _processorNameSource; }
            set
            {
                _processorNameSource = value;
                OnPropertyChanged();
            }
        }
        public string ProcessorName
        {
            get { return _processorName; }
            set
            {
                _processorName = value;
                OnPropertyChanged();
            }
        }

        public string ProcessorParameters
        {
            get { return _processorParameters; }
            set
            {
                _processorParameters = value;
                OnPropertyChanged();
            }
        }

        public string BaseBoardName
        {
            get { return _baseBoardName; }
            set
            {
                _baseBoardName = value;
                OnPropertyChanged();
            }
        }
        public bool HasChanges()
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync(int? Id)
        {
            UserReport = await _iOManager.GetUserReport();

            _iOManager.SetUserReport();



            //_userReport = ReportHelper.GetUserReport();


            //-------------------------------------------------------------------------------------------------------------------
            ProcessStartInfo? StartInfo = new ProcessStartInfo
            {
                FileName = _iOManager.Aida,
                Arguments = "/R " + Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Report. " + "/XML " + "/CUSTOM " + Path.GetDirectoryName(_iOManager.Aida) + "\\ForReportPC.rpf",
                UseShellExecute = false
            };
            try
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Report.xml"))
                {
                    Process proc = Process.Start(StartInfo);
                    await proc.WaitForExitAsync();
                }
            }
            catch (Exception) { }

            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Report.xml"))
                return;
            //-------------------------------------------------------------------------------------------------------------------
            XDocument? document;
            using (var stream = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Report.xml"))
            {
                document = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, new System.Threading.CancellationToken());
            }
            //-------------------------------------------------------------------------------------------------------------------

            //var t = document.XPathSelectElement("");

            ManagementObjectSearcher processors = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

            foreach (ManagementObject processor in processors.Get())
            {
                ProcessorNameSource += processor.Properties["Name"].Value;

                //if (property != null)
                //    {
                //        if (property.Name == "Name")
                //        {
                //            ProcessorNameSource = property.Value.ToString();
                //            break;
                //        }
                //    }
                //}
                //foreach (PropertyData property in processor.Properties)
                //{
                //    if (property.Name == "MaxClockSpeed")
                //    ProcessorParameters += $"{double.Parse(property.Value.ToString()) / 1000}(GHz)";
                //if (property.Name == "NumberOfCores")
                //    ProcessorParameters += $"(core:{property.Value}";

            }






            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");

            string graphicsCard = string.Empty;
            foreach (ManagementObject mo in searcher.Get())
            {
                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name == "Description")
                    {
                        graphicsCard = property.Value.ToString();
                    }
                }
            }
        }

        public void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
