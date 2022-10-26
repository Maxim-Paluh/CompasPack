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
using System.Globalization;
using System.Text.RegularExpressions;
using Prism.Commands;
using CompasPack.ViewModel;

namespace CompasPack.ViewModel
{
    public class ComputerReportViewModel : ViewModelBase, IDetailViewModel
    {
        private CPUViewModel _CPUViewModel;
        private MotherboardViewModel _motherboardViewModel;
        private MemoryViewModel _memoryViewModel;

        private string _videoControllersSource;
        private string _videoControllers;
        private string _videoControllersSize;


        private readonly IIOManager _iOManager;


        public ComputerReportViewModel(IIOManager iOManager)
        {
            _iOManager = iOManager;
        }

        public CPUViewModel CPUViewModel
        {
            get { return _CPUViewModel; }
            set
            {
                _CPUViewModel = value;
                OnPropertyChanged();
            }
        }
        public MotherboardViewModel MotherboardViewModel
        {
            get { return _motherboardViewModel; }
            set
            {
                _motherboardViewModel = value;
                OnPropertyChanged();
            }
        }
        public MemoryViewModel MemoryViewModel
        {
            get { return _memoryViewModel; }
            set
            {
                _memoryViewModel = value;
                OnPropertyChanged();
            }
        }


        public string VideoControllers
        {
            get { return _videoControllers; }
            set
            {
                _videoControllers = value;
                OnPropertyChanged();
            }
        }
        public string VideoControllersSource
        {
            get { return _videoControllersSource; }
            set
            {
                _videoControllersSource = value;
                OnPropertyChanged();
            }
        }
        public string VideoControllersSize
        {
            get { return _videoControllersSize; }
            set
            {
                _videoControllersSize = value;
                OnPropertyChanged();
            }
        }
        public bool HasChanges()
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync(int? Id)
        {
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

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Report.xml", Encoding.GetEncoding("windows-1251")))
            {
                document = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, new System.Threading.CancellationToken());
            }
            //-------------------------------------------------------------------------------------------------------------------

            var UserReport = await _iOManager.GetUserReport();

            CPUViewModel = new CPUViewModel(UserReport, document);
            MotherboardViewModel = new MotherboardViewModel(UserReport, document);
            MemoryViewModel = new MemoryViewModel(UserReport, document);
            CPUViewModel.Load();
            MotherboardViewModel.Load();
            MemoryViewModel.Load();
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                foreach (ManagementObject mo in searcher.Get())
                {
                    var tempDescription = mo["Description"];
                    var tempAdapterRAM = mo["AdapterRAM"];
                    if (tempDescription != null)
                        VideoControllersSource += tempDescription.ToString() + "\n";
                    else
                        VideoControllersSource = "Not found";
                    if (tempAdapterRAM != null)
                        VideoControllersSize += $"({double.Parse(tempAdapterRAM.ToString()) / 1073741824}Gb)\n";
                    else
                        VideoControllersSize = "Not found";
                }

                VideoControllersSource = VideoControllersSource.TrimEnd();
                VideoControllersSize = VideoControllersSize.TrimEnd();

                var tempVideo = VideoControllersSource.Split('\n');
                var tempVideoSize = VideoControllersSize.Split('\n');
                for (int i = 0; i < tempVideo.Count(); i++)
                {
                    var tempVideoRegex = tempVideo[i];
                    foreach (var item in UserReport.VideoController.Regex)
                        tempVideoRegex = Regex.Replace(tempVideoRegex, item, "");
                    VideoControllers += $"{tempVideoRegex} {tempVideoSize[i]}\n";
                }
                VideoControllers = VideoControllers.TrimEnd();
            }
            {
                
            }
        }


       


        public void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
