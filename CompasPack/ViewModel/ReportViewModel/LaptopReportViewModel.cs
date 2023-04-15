using CompasPack.View;
using CompasPack.View.Service;
using CompasPakc.BL;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    internal class LaptopReportViewModel : ViewModelBase, IDetailViewModel
    {
        private SettingsReportViewModel _settingsReportViewModel;
        private XDocument _xDocument;
        private IIOManager _ioManager;

        private bool _isEnable;
        private string _reportPath;
        private int _indexReport;
        
        private LaptopMainViewModel _laptopMainViewModel;
        private LaptopMonitorViewModel _laptopMonitorViewModel;
        private CPUViewModel _CPUViewModel;
        private MemoryViewModel _memoryViewModel;
        private VideoViewModel _videoViewModel;
        private PhysicalDiskViewModel _physicalDiskViewModel;
        private LaptopBatteryViewModel _laptopBatteryViewModel;
        private LaptopOtherViewModel _laptopOtherViewModel;

        private IMessageDialogService _messageDialogService;
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


        public LaptopReportViewModel(IIOManager iOManager, SettingsReportViewModel settingsReportViewModel, XDocument xDocument, IMessageDialogService messageDialogService)
        {
            _ioManager = iOManager;
            _settingsReportViewModel = settingsReportViewModel;
            _xDocument = xDocument;
            _messageDialogService = messageDialogService;

            ReportPath = _ioManager.ReportLaptop;

            SaveReportCommand = new DelegateCommand(OnSaveReport);

            OpenReportCommand = new DelegateCommand(OnOpenReport);
            OpenPriceCommand = new DelegateCommand(OnOpenPrice);
            OpenFolderCommand = new DelegateCommand(OnOpenFolder);
        }

        private async void OnSaveReport()
        {
            //if (string.IsNullOrWhiteSpace(PCCaseViewModel.Name) || string.IsNullOrWhiteSpace(PowerSupplyViewModel.Text) || string.IsNullOrWhiteSpace(PowerSupplyViewModel.Power) || !PowerSupplyViewModel.Power.All(char.IsDigit))
            //{
            //    _messageDialogService.ShowInfoDialog("Заповни всі поля виділені червоним", "Помилка!");
            //    return;
            //}

            bool checkHml = File.Exists($"{ReportPath}\\Report_{IndexReport:000}.htm");
            bool checkHtml = File.Exists($"{ReportPath}\\Report_{IndexReport:000}.html");
            bool checkDocx = File.Exists($"{ReportPath}\\Report_{IndexReport:000}.docx");

            if (checkHml || checkHtml || checkDocx)
            {
                string listFile = string.Empty;
                if (checkHml)
                    listFile += $"{ReportPath}\\Report_{IndexReport:000}.htm\n";
                if (checkHtml)
                    listFile += $"{ReportPath}\\Report_{IndexReport:000}.html\n";
                if (checkDocx)
                    listFile += $"{ReportPath}\\Report_{IndexReport:000}.docx\n";

                var res = _messageDialogService.ShowYesNoDialog($"В папці призначення вже є файл(и):\n\n{listFile}\nВи хочете замінити його(їх)\n\n(Це невідворотня дія, зробіть їх копію!)", "Попередження!");
                if (res == MessageDialogResult.No || res == MessageDialogResult.Cancel)
                { return; }
            }

            await Task.Delay(100);
            IsEnable = false;

            await GetHML();

            await GetHTML();

            await GetDOCX();

            IsEnable = true;
        }
        private async Task GetHML()
        {
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
            catch (Exception e)
            {
                _messageDialogService.ShowInfoDialog($"В процесі формування звіту AIDA64 відбулася помилка.\n\nФайл: {ReportPath}\\Report_{IndexReport:000}.htm не створено!\n\nЗвернись до розробника і скинь фото:\n\n" +
                    $"{e.Message}\n\n{e.StackTrace}", "Помилка");
            }
        }
        private async Task GetHTML()
        {
            try
            {
                string html = $"<html><head><style>table{{font-family: Arial;font-size: 13px;}}</style>" +
               $"</head><body><table><tbody>" +
               //$"<tr><th>{PCCaseViewModel.Result}</th><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td>{CPUViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               //$"<tr><td>{MotherboardViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td>{MemoryViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td>{VideoViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td>{PhysicalDiskViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
              // $"<tr><td>{PowerSupplyViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td><b>ID: {IndexReport:000} (Прийшов {DateTime.Now:dd.MM.yyyy})</b></td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"> </td></tr></tbody></table></body></html>";
                await _ioManager.WriteAllTextAsync($"{ReportPath}\\Report_{IndexReport:000}.html", html);
            }
            catch (Exception e)
            {
                _messageDialogService.ShowInfoDialog($"В процесі формування звіту HTML відбулася помилка.\n\nФайл: {ReportPath}\\Report_{IndexReport:000}.html не створено!\n\nЗвернись до розробника і скинь фото:\n\n" +
                   $"{e.Message}\n\n{e.StackTrace}", "Помилка");
            }

        }
        private async Task GetDOCX()
        {
            try
            {
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
                //text = text.Replace("compasmotherboard", MotherboardViewModel.Result);
                text = text.Replace("compasmemory", MemoryViewModel.Result);
                text = text.Replace("compasgpu", VideoViewModel.Result);
                text = text.Replace("compashdd", PhysicalDiskViewModel.Result);
               // text = text.Replace("compaspower", PowerSupplyViewModel.Result);
                text = text.Replace("compasid", $"{IndexReport:000}");

                using (StreamWriter writer = new StreamWriter($"{unzipPath}\\document.xml", false))
                {
                    await writer.WriteLineAsync(text);
                }

                if (File.Exists($"{ReportPath}\\Report_{IndexReport:000}.docx"))
                    File.Delete($"{ReportPath}\\Report_{IndexReport:000}.docx");

                ZipFile.CreateFromDirectory($"{ReportPath}\\SourcePrice\\unzip", $"{ReportPath}\\Report_{IndexReport:000}.docx", CompressionLevel.SmallestSize, false);
            }
            catch (Exception e)
            {
                _messageDialogService.ShowInfoDialog($"В процесі формування звіту docx відбулася помилка.\n\nФайл: {ReportPath}\\Report_{IndexReport:000}.docx не створено!\n\nЗвернись до розробника і скинь фото:\n\n" +
                  $"{e.Message}\n\n{e.StackTrace}", "Помилка");
            }
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

        public LaptopMonitorViewModel LaptopMonitorViewModel
        {
            get { return _laptopMonitorViewModel; }
            set
            {
                _laptopMonitorViewModel = value;
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
        public LaptopMainViewModel LaptopMainViewModel 
        {
            get { return _laptopMainViewModel; }
            set
            {
                _laptopMainViewModel = value;
                OnPropertyChanged();
            }
        }
        public LaptopBatteryViewModel LaptopBatteryViewModel
        {
            get { return _laptopBatteryViewModel; }
            set { _laptopBatteryViewModel = value; }
        }
        public LaptopOtherViewModel LaptopOtherViewModel
        {
            get { return _laptopOtherViewModel; }
            set
            {
                _laptopOtherViewModel = value;
                OnPropertyChanged();
            }
        }
       
        

        public async Task LoadAsync(int? Id)
        {
            LaptopMainViewModel = new LaptopMainViewModel(_settingsReportViewModel);
            LaptopMonitorViewModel = new LaptopMonitorViewModel(_settingsReportViewModel, _xDocument);

            CPUViewModel = new CPUViewModel(_settingsReportViewModel, _xDocument);
            MemoryViewModel = new MemoryViewModel(_settingsReportViewModel, _xDocument);
            VideoViewModel = new VideoViewModel(_settingsReportViewModel);
            PhysicalDiskViewModel = new PhysicalDiskViewModel(_settingsReportViewModel, _xDocument);
            LaptopOtherViewModel = new LaptopOtherViewModel(_settingsReportViewModel, _xDocument);
            LaptopBatteryViewModel = new LaptopBatteryViewModel(_settingsReportViewModel, _xDocument);

            await Task.Factory.StartNew(() =>
            {
                CPUViewModel.Load();
                LaptopMonitorViewModel.Load();
                MemoryViewModel.Load();
                VideoViewModel.Load();
                PhysicalDiskViewModel.Load();
                LaptopBatteryViewModel.Load();
                LaptopOtherViewModel.Load();
            });

            IndexReport = _ioManager.GetLastReport(ReportPath) + 1;
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
        public ICommand OpenReportCommand { get; set; }
        public ICommand SavePriceCommand { get; set; }
        public ICommand OpenPriceCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
    }
}
