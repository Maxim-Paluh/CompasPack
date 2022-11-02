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
using System.Xml.Linq;
using System.Xml.XPath;
using System.Globalization;
using System.Text.RegularExpressions;
using Prism.Commands;
using CompasPack.ViewModel;
using System.Windows.Input;
using System.Security.Principal;
using System.Windows.Controls;
using System.Windows.Media;

namespace CompasPack.ViewModel
{
    public class ComputerReportViewModel : ViewModelBase, IDetailViewModel
    {
        private SettingsReportViewModel _settingsReportViewModel;
        private XDocument _xDocument;
        private IIOManager _ioManager;

        private bool _isEnable;

        private string _reportFile;
        private string _reportPath;

        private PCCaseViewModel _pCCaseViewModel;
        private CPUViewModel _CPUViewModel;
        private MotherboardViewModel _motherboardViewModel;
        private MemoryViewModel _memoryViewModel;
        private VideoViewModel _videoViewModel;
        private PhysicalDiskViewModel _physicalDiskViewModel;
        private PowerSupplyViewModel _powerSupplyView;

        public ComputerReportViewModel(IIOManager iOManager, SettingsReportViewModel settingsReportViewModel, XDocument xDocument)
        {
            IsEnable = false;
            _ioManager = iOManager;
            _settingsReportViewModel = settingsReportViewModel;
            _xDocument = xDocument;

            SaveReportCommand = new DelegateCommand(OnSaveReport);
            SavePriceCommand = new DelegateCommand(OnSavePrice);
        }

        private void OnSavePrice()
        {
            throw new NotImplementedException();
        }

        private async void OnSaveReport()
        {
            IsEnable = false;
            if (!Directory.Exists(_ioManager.CompasPackLog))
                Directory.CreateDirectory(_ioManager.CompasPackLog);
            ProcessStartInfo? StartInfo = new ProcessStartInfo
            {
                FileName = _ioManager.Aida,
                Arguments = "/R " + _ioManager.ReportPC + $"\\{ReportFile}. " + "/HML " + "/CUSTOM " + Path.GetDirectoryName(_ioManager.Aida) + "\\ForReportPC.rpf",
                UseShellExecute = false
            };
            try
            {
                Process proc = Process.Start(StartInfo);
                await proc.WaitForExitAsync();
            }
            catch (Exception) { }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{PCCaseViewModel.Result}\t");
            stringBuilder.AppendLine($"{CPUViewModel.Result}\t");
            stringBuilder.AppendLine($"{MotherboardViewModel.Result}\t");
            stringBuilder.AppendLine($"{MemoryViewModel.Result}\t");
            stringBuilder.AppendLine($"{VideoViewModel.Result}\t");
            stringBuilder.AppendLine($"{PhysicalDiskViewModel.Result}\t");
            stringBuilder.AppendLine($"{PowerSupplyViewModel.Result}\t");
            stringBuilder.AppendLine($"ID: {_ioManager.GetLastReport(_ioManager.ReportPC) + 1:000} (Прийшов {DateTime.Now:dd.MM.yyyy})\t");

            await _ioManager.WriteAllTextAsync($"{_ioManager.ReportPC}\\{ReportFile}.txt", stringBuilder.ToString());
            IsEnable = true;
        }

        public PCCaseViewModel PCCaseViewModel
        {
            get { return _pCCaseViewModel; }
            set
            {
                _pCCaseViewModel = value;
                OnPropertyChanged();
            }
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
        public PhysicalDiskViewModel PhysicalDiskViewModel
        {
            get { return _physicalDiskViewModel; }
            set
            {
                _physicalDiskViewModel = value;
                OnPropertyChanged();
            }
        }
        public PowerSupplyViewModel PowerSupplyViewModel
        {
            get { return _powerSupplyView; }
            set
            {
                _powerSupplyView = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged();
            }
        }

        public string ReportFile
        {
            get { return _reportFile; }
            set
            {
                _reportFile = value;
                OnPropertyChanged();
            }
        }
        public string ReportPath
        {
            get { return _reportPath; }
            set
            {
                _reportPath = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync(int? Id)
        {
            PCCaseViewModel = new PCCaseViewModel();
            CPUViewModel = new CPUViewModel(_settingsReportViewModel, _xDocument);
            MotherboardViewModel = new MotherboardViewModel(_settingsReportViewModel, _xDocument);
            MemoryViewModel = new MemoryViewModel(_settingsReportViewModel, _xDocument);
            VideoViewModel = new VideoViewModel(_settingsReportViewModel);
            PhysicalDiskViewModel = new PhysicalDiskViewModel(_settingsReportViewModel, _xDocument);
            PowerSupplyViewModel = new PowerSupplyViewModel(_settingsReportViewModel);

            await Task.Factory.StartNew(() =>
            {
                CPUViewModel.Load();
                MotherboardViewModel.Load();
                MemoryViewModel.Load();
                VideoViewModel.Load();
                PhysicalDiskViewModel.Load();
                PowerSupplyViewModel.Load();
            });

            ReportPath = _ioManager.ReportPC;
            ReportFile = $"Report_{_ioManager.GetLastReport(_ioManager.ReportPC) + 1:000}";
            IsEnable = true;
        }
        public bool HasChanges()
        {
            return !IsEnable;
        }
        public void Unsubscribe()
        {

        }

        public ICommand SaveReportCommand { get; set; }
        public ICommand SavePriceCommand { get; set; }
    }
}
