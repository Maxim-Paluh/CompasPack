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
        private VideoViewModel _videoViewModel;



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
        public VideoViewModel VideoViewModel
        {
            get { return _videoViewModel; }
            set
            {
                _videoViewModel = value;
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
            VideoViewModel = new VideoViewModel(UserReport);
            CPUViewModel.Load();
            MotherboardViewModel.Load();
            MemoryViewModel.Load();
            VideoViewModel.Load();
        }


       


        public void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
