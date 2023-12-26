﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CompasPack.View.Service;
using System.IO.Compression;
using CompasPack.Settings;
using CompasPack.Helper;


namespace CompasPack.ViewModel
{
    public class ComputerReportViewModel : ReportViewModelBase, IDetailViewModel
    {
        private PCCaseViewModel _pCCaseViewModel;
        private CPUViewModel _CPUViewModel;
        private MotherboardViewModel _motherboardViewModel;
        private MemoryViewModel _memoryViewModel;
        private VideoControllerViewModel _videoControllerViewModel;
        private PhysicalDiskViewModel _physicalDiskViewModel;
        private PowerSupplyViewModel _powerSupplyView;
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
        public VideoControllerViewModel VideoControllerViewModel
        {
            get { return _videoControllerViewModel; }
            set
            {
                _videoControllerViewModel = value;
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
        public ComputerReportViewModel(IIOHelper iOHelper, ReportSettings reportSettings, XDocument xDocument, IMessageDialogService messageDialogService) : 
            base (iOHelper, reportSettings, xDocument, messageDialogService)
        {
            ReportPath = _iOHelper.ReportPC;
        }
        public async Task LoadAsync(int? Id)
        {
            PCCaseViewModel = new PCCaseViewModel();
            CPUViewModel = new CPUViewModel(_reportSettings.CPUReportSettings, _xDocument);
            MotherboardViewModel = new MotherboardViewModel(_reportSettings.MotherboardReportSettings, _xDocument);
            MemoryViewModel = new MemoryViewModel(_reportSettings.MemoryReportSettings, _xDocument);
            VideoControllerViewModel = new VideoControllerViewModel(_reportSettings.VideoControllerReportSettings);
            PhysicalDiskViewModel = new PhysicalDiskViewModel(_xDocument);
            PowerSupplyViewModel = new PowerSupplyViewModel(_reportSettings.PCPowerSupply);

            await Task.Factory.StartNew(() =>
            {
                CPUViewModel.Load();
                MotherboardViewModel.Load();
                MemoryViewModel.Load();
                VideoControllerViewModel.Load();
                PhysicalDiskViewModel.Load();
                PowerSupplyViewModel.Load();
            });
            IndexReport = _iOHelper.GetLastReport(ReportPath) + 1;

            IsEnable = true;
        }
        protected override  async void OnSaveReport()
        {
            if (string.IsNullOrWhiteSpace(PCCaseViewModel.Name) || string.IsNullOrWhiteSpace(PowerSupplyViewModel.Name) || string.IsNullOrWhiteSpace(PowerSupplyViewModel.Power) || !PowerSupplyViewModel.Power.All(char.IsDigit))
            {
                _messageDialogService.ShowInfoDialog("Заповни всі поля виділені червоним", "Помилка!");
                return;
            }

            bool checkHml = File.Exists($"{ReportPath}\\Report_{IndexReport:000}.htm");
            bool checkHtml = File.Exists($"{ReportPath}\\Report_{IndexReport:000}.html");
            bool checkDocx = File.Exists($"{ReportPath}\\Report_{IndexReport:000}.docx");

            if (checkHml || checkHtml || checkDocx)
            {
                string listFile = string.Empty;
                if (checkHml)
                    listFile += $"{ReportPath}\\Report_{IndexReport:000}.htm\n";
                if(checkHtml)
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
                FileName = _iOHelper.Aida,
                Arguments = "/R " + ReportPath + $"\\Report_{IndexReport:000}. " + "/HML " + "/CUSTOM " + System.IO.Path.GetDirectoryName(_iOHelper.Aida) + "\\ForReport.rpf",
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
               $"<tr><th>{PCCaseViewModel.Result}</th><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td>{CPUViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td>{MotherboardViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td>{MemoryViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td>{VideoControllerViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td>{PhysicalDiskViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td>{PowerSupplyViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td><b>ID: {IndexReport:000} (Прийшов {DateTime.Now:dd.MM.yyyy})</b></td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
               $"<tr><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"> </td></tr></tbody></table></body></html>";
               await _iOHelper.WriteAllTextAsync($"{ReportPath}\\Report_{IndexReport:000}.html", html);
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
                text = text.Replace("compasmotherboard", MotherboardViewModel.Result);
                text = text.Replace("compasmemory", MemoryViewModel.Result);
                text = text.Replace("compasgpu", VideoControllerViewModel.Result);
                text = text.Replace("compashdd", PhysicalDiskViewModel.Result);
                text = text.Replace("compaspower", PowerSupplyViewModel.Result);
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
        public bool HasChanges()
        {
            return false;
        }
        public void Unsubscribe()
        {

        }
    }
}
