﻿using CompasPack.BL;
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
using CompasPack.View.Service;
using System.Windows.Shapes;
using System.IO.Compression;


namespace CompasPack.ViewModel
{
    public class ComputerReportViewModel : ViewModelBase, IDetailViewModel
    {
        private SettingsReportViewModel _settingsReportViewModel;
        private XDocument _xDocument;
        private IIOManager _ioManager;

        private bool _isEnable;

        private string _reportPath;
        private int _indexReport;

        private PCCaseViewModel _pCCaseViewModel;
        private CPUViewModel _CPUViewModel;
        private MotherboardViewModel _motherboardViewModel;
        private MemoryViewModel _memoryViewModel;
        private VideoViewModel _videoViewModel;
        private PhysicalDiskViewModel _physicalDiskViewModel;
        private PowerSupplyViewModel _powerSupplyView;
        private IMessageDialogService _messageDialogService;

        public ComputerReportViewModel(IIOManager iOManager, SettingsReportViewModel settingsReportViewModel, XDocument xDocument, IMessageDialogService messageDialogService)
        {
            IsEnable = false;
            _ioManager = iOManager;
            _settingsReportViewModel = settingsReportViewModel;
            _xDocument = xDocument;
            _messageDialogService = messageDialogService;

            ReportPath = _ioManager.ReportPC;

            SaveReportCommand = new DelegateCommand(OnSaveReport);

            OpenReportCommand = new DelegateCommand(OnOpenReport);
            OpenPriceCommand = new DelegateCommand(OnOpenPrice);
            OpenFolderCommand = new DelegateCommand(OnOpenFolder);

        }


        private async void OnSaveReport()
        {
            if (string.IsNullOrWhiteSpace(PCCaseViewModel.Name) || string.IsNullOrWhiteSpace(PowerSupplyViewModel.Text) || string.IsNullOrWhiteSpace(PowerSupplyViewModel.Power) || !PowerSupplyViewModel.Power.All(char.IsDigit))
            {
                _messageDialogService.ShowInfoDialog("Заповни всі поля виділені червоним", "Помилка!");
                return;
            }

            if(File.Exists($"{ReportPath}\\Report_{IndexReport:000}.html"))
            {
                var res = _messageDialogService.ShowYesNoDialog("Файл з таким ім'ям вже існує, ви хочете його замінити (це невідворотня дія)", "Попередження!");
               if (res == MessageDialogResult.No || res==MessageDialogResult.Cancel)
                { return; }    
            }

            await Task.Delay(100);
            IsEnable = false;

            #region Aida .hml

            if (!Directory.Exists(_ioManager.CompasPackLog))
                Directory.CreateDirectory(_ioManager.CompasPackLog);
            ProcessStartInfo? StartInfo = new ProcessStartInfo
            {
                FileName = _ioManager.Aida,
                Arguments = "/R " + ReportPath + $"\\Report_{IndexReport:000}. " + "/HML " + "/CUSTOM " + System.IO.Path.GetDirectoryName(_ioManager.Aida) + "\\ForReport.rpf",
                UseShellExecute = false
            };
            try
            {
                Process proc = Process.Start(StartInfo);
                await proc.WaitForExitAsync();
            }
            catch (Exception) { }
            #endregion

            #region FastReport .html
            string html = $"<html><head><style>table{{font-family: Arial;font-size: 13px;}}</style>" +
                $"</head><body><table><tbody>" +
                $"<tr><th>{PCCaseViewModel.Result}</th><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
                $"<tr><td>{CPUViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
                $"<tr><td>{MotherboardViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
                $"<tr><td>{MemoryViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
                $"<tr><td>{VideoViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
                $"<tr><td>{PhysicalDiskViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
                $"<tr><td>{PowerSupplyViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
                $"<tr><td><b>ID: {IndexReport:000} (Прийшов {DateTime.Now:dd.MM.yyyy})</b></td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
                $"<tr><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"> </td></tr></tbody></table></body></html>";

            await _ioManager.WriteAllTextAsync($"{ReportPath}\\Report_{IndexReport:000}.html", html);
            #endregion


            #region Price .docx
            var documentPath = $"{ReportPath}\\SourcePrice\\document.xml";
            if (!File.Exists(documentPath))
            {
                _messageDialogService.ShowInfoDialog($"Не знайдено файл: {documentPath}", "Помилка!");
                return;
            }
            var unzipPath = $"{ReportPath}\\SourcePrice\\unzip\\word";
            if (!Directory.Exists(unzipPath))
            {
                _messageDialogService.ShowInfoDialog($"Не знайдено папку: {documentPath}", "Помилка!");
                return;
            }
            File.Copy(documentPath, $"{unzipPath}\\document.xml", true);
            string text = string.Empty;
            using (StreamReader reader = new StreamReader($"{unzipPath}\\document.xml"))
            {
                text = await reader.ReadToEndAsync();
            }

            text = text.Replace("compascpu", CPUViewModel.Result);
            text = text.Replace("compasmotherboard", MotherboardViewModel.Result);
            text = text.Replace("compasmemory", MemoryViewModel.Result);
            text = text.Replace("compasgpu", VideoViewModel.Result);
            text = text.Replace("compashdd", PhysicalDiskViewModel.Result);
            text = text.Replace("compaspower", PowerSupplyViewModel.Result);
            text = text.Replace("compasid", $"{IndexReport:000}");

            using (StreamWriter writer = new StreamWriter($"{unzipPath}\\document.xml", false))
            {
                await writer.WriteLineAsync(text);
            }
            if (File.Exists($"{ReportPath}\\Report_{IndexReport:000}.docx"))
            {
                File.Delete($"{ReportPath}\\Report_{IndexReport:000}.docx");
            }
            ZipFile.CreateFromDirectory($"{ReportPath}\\SourcePrice\\unzip", $"{ReportPath}\\Report_{IndexReport:000}.docx", CompressionLevel.SmallestSize, false);
            #endregion

            IsEnable = true;
        }
        private void OnOpenReport()
        {
            if (!File.Exists($"{ReportPath}\\Report_{IndexReport:000}.html"))
                _messageDialogService.ShowInfoDialog("Такого файлу нема!", "Помилка!");
            _ioManager.OpenFolderAndSelectFile($"{ReportPath}\\Report_{IndexReport:000}.html");
        }
      
        private void OnOpenPrice()
        {
            if (!File.Exists($"{ReportPath}\\Report_{IndexReport:000}.docx"))
                _messageDialogService.ShowInfoDialog("Такого файлу нема!", "Помилка!");
            _ioManager.OpenFolderAndSelectFile($"{ReportPath}\\Report_{IndexReport:000}.docx");
        }
        private void OnOpenFolder()
        {
            _ioManager.OpenFolder(ReportPath);
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


        public string ReportPath
        {
            get { return _reportPath; }
            set
            {
                _reportPath = value;
                OnPropertyChanged();
            }
        }
        public int IndexReport
        {
            get { return _indexReport; }
            set
            {
                _indexReport = value;
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
            IndexReport = _ioManager.GetLastReport(ReportPath) + 1;

            IsEnable = true;
        }
        public bool HasChanges()
        {
            return false;
        }
        public void Unsubscribe()
        {

        }

        public ICommand SaveReportCommand { get; set; }
        public ICommand OpenReportCommand { get; set; }
        public ICommand SavePriceCommand { get; set; }
        public ICommand OpenPriceCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
    }
}
