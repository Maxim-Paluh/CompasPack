using CompasPack.View.Service;
using System;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using CompasPack.Settings;
using CompasPack.Helper;

namespace CompasPack.ViewModel
{
    public class MonitorReportViewModel : ReportViewModelBase, IDetailViewModel
    {
        private MonitorMainViewModel _monitorMainViewModel;
        private MonitorOtherViewModel _monitorOtherViewModel;
        private MonitorAspectRatioViewModel _MonitorAspectRatioViewModel;
        private MonitorDiagonalViewModel _monitorDiagonalViewModel;
        private MonitorResolutionViewModel _monitorResolutionViewModel;
        public MonitorResolutionViewModel MonitorResolutionViewModel
        {
            get { return _monitorResolutionViewModel; }
            set { _monitorResolutionViewModel = value; }
        }
        public MonitorDiagonalViewModel MonitorDiagonalViewModel
        {
            get { return _monitorDiagonalViewModel; }
            set
            {
                _monitorDiagonalViewModel = value;
                OnPropertyChanged();
            }
        }
        public MonitorAspectRatioViewModel MonitorAspectRatioViewModel
        {
            get { return _MonitorAspectRatioViewModel; }
            set { _MonitorAspectRatioViewModel = value; }
        }
        public MonitorOtherViewModel MonitorOtherViewModel
        {
            get { return _monitorOtherViewModel; }
            set { _monitorOtherViewModel = value; }
        }
        public MonitorMainViewModel MonitorMainViewModel
        {
            get { return _monitorMainViewModel; }
            set { _monitorMainViewModel = value; }
        }
        public MonitorReportViewModel(IIOHelper iOHelper, ReportSettings reportSettings, XDocument xDocument, IMessageDialogService messageDialogService) :
            base(iOHelper, reportSettings, xDocument, messageDialogService)
        {
            ReportPath = iOHelper.ReportMonitor;
        }
        public async Task LoadAsync(int? Id)
        {
            MonitorMainViewModel = new MonitorMainViewModel(_reportSettings.MonitorReportSettings, _xDocument);
            MonitorAspectRatioViewModel = new MonitorAspectRatioViewModel(_reportSettings.MonitorReportSettings, _xDocument);
            MonitorOtherViewModel = new MonitorOtherViewModel(_reportSettings.MonitorReportSettings, _xDocument);
            MonitorDiagonalViewModel = new MonitorDiagonalViewModel(_reportSettings.MonitorReportSettings, _xDocument);
            MonitorResolutionViewModel = new MonitorResolutionViewModel(_reportSettings.MonitorReportSettings, _xDocument);

            await Task.Factory.StartNew(() =>
            {
                MonitorMainViewModel.Load();
                MonitorAspectRatioViewModel.Load();
                MonitorOtherViewModel.Load();
                MonitorDiagonalViewModel.Load();
                MonitorResolutionViewModel.Load();
            });

            IndexReport = _iOHelper.GetLastReport(ReportPath) + 1;
            IsEnable = true;
        }
        protected override async void OnSaveReport()
        {

            if (string.IsNullOrWhiteSpace(MonitorMainViewModel.Brand) || string.IsNullOrWhiteSpace(MonitorOtherViewModel.Result))
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
                FileName = _iOHelper.Aida,
                Arguments = "/R " + ReportPath + $"\\Report_{IndexReport:000}. " + "/HML " + "/CUSTOM " + System.IO.Path.GetDirectoryName(_iOHelper.Aida) + "\\ForMonitor.rpf",
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
                string html = $"<html> <head> <style> table {{ font-family: Arial; font-size: 13px; }} </style> " +
                    $"</head> <body> <table> <tbody>" +
                    $" <tr> <th>{IndexReport:000}</th> <td style=\"text-align:left;\"><b>{MonitorMainViewModel.Result}</b></td> <td style=\"background-color: red; text-align:center;\"><b>0</b></td> <td style=\"background-color: red; text-align:center;\"><b>0</b></td> <td style=\"text-align:center;\">{MonitorDiagonalViewModel.Result}</td> <td style=\"text-align:center;\">{MonitorAspectRatioViewModel.Result}</td> <td style=\"text-align:center;\">{MonitorResolutionViewModel.Result}</td> <td style=\"text-align:center;\">{DateTime.Now:dd.MM.yyyy}</td> </tr> " +
                    $"</tbody> </table> </body> </html>";
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

                text = text.Replace("brand", MonitorMainViewModel.Result);
                text = text.Replace("diag", _monitorDiagonalViewModel.Result);
                text = text.Replace("compasaspectratio", MonitorAspectRatioViewModel.Result);
                text = text.Replace("compasresolution", MonitorResolutionViewModel.Result);
                text = text.Replace("compasinterface", MonitorOtherViewModel.Result);
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
