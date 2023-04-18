using CompasPack.View.Service;
using CompasPakc.BL;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class MonitorReportViewModel : ReportViewModelBase, IDetailViewModel
    {

        public MonitorReportViewModel(IIOManager iOManager, SettingsReportViewModel settingsReportViewModel, XDocument xDocument, IMessageDialogService messageDialogService) :
            base(iOManager, settingsReportViewModel, xDocument, messageDialogService)
        {
            ReportPath = _ioManager.ReportMonitor;
        }

        protected override async void OnSaveReport()
        {
            //if (string.IsNullOrWhiteSpace(LaptopMainViewModel.Brand) || string.IsNullOrWhiteSpace(LaptopMainViewModel.Model) || LaptopOtherViewModel.Microphone == null || LaptopOtherViewModel.WebCam == null)
            //{
            //    _messageDialogService.ShowInfoDialog("Заповни всі поля виділені червоним", "Помилка!");
            //    return;
            //}

            //bool checkHml = File.Exists($"{ReportPath}\\Report_{IndexReport:000}.htm");
            //bool checkHtml = File.Exists($"{ReportPath}\\Report_{IndexReport:000}.html");
            //bool checkDocx = File.Exists($"{ReportPath}\\Report_{IndexReport:000}.docx");

            //if (checkHml || checkHtml || checkDocx)
            //{
            //    string listFile = string.Empty;
            //    if (checkHml)
            //        listFile += $"{ReportPath}\\Report_{IndexReport:000}.htm\n";
            //    if (checkHtml)
            //        listFile += $"{ReportPath}\\Report_{IndexReport:000}.html\n";
            //    if (checkDocx)
            //        listFile += $"{ReportPath}\\Report_{IndexReport:000}.docx\n";

            //    var res = _messageDialogService.ShowYesNoDialog($"В папці призначення вже є файл(и):\n\n{listFile}\nВи хочете замінити його(їх)\n\n(Це невідворотня дія, зробіть їх копію!)", "Попередження!");
            //    if (res == MessageDialogResult.No || res == MessageDialogResult.Cancel)
            //    { return; }
            //}

            //await Task.Delay(100);
            //IsEnable = false;

            //await GetHML();

            //await GetHTML();

            //await GetDOCX();

            //IsEnable = true;
        }
        private async Task GetHML()
        {
            //ProcessStartInfo? StartInfo = new ProcessStartInfo
            //{
            //    FileName = _ioManager.Aida,
            //    Arguments = "/R " + ReportPath + $"\\Report_{IndexReport:000}. " + "/HML " + "/CUSTOM " + System.IO.Path.GetDirectoryName(_ioManager.Aida) + "\\ForReport.rpf",
            //    UseShellExecute = false
            //};
            //try
            //{
            //    Process proc = Process.Start(StartInfo);
            //    await proc.WaitForExitAsync();
            //}
            //catch (Exception e)
            //{
            //    _messageDialogService.ShowInfoDialog($"В процесі формування звіту AIDA64 відбулася помилка.\n\nФайл: {ReportPath}\\Report_{IndexReport:000}.htm не створено!\n\nЗвернись до розробника і скинь фото:\n\n" +
            //        $"{e.Message}\n\n{e.StackTrace}", "Помилка");
            //}
        }
        private async Task GetHTML()
        {
            //try
            //{
            //    string html = $"<html> <head> <style> table {{ font-family: Arial; font-size: 13px; }} </style> </head> <body> <table> <tbody> " +
            //        $"<tr> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> </tr>" +
            //        $" <tr> <td style=\"text-align:right;\">{IndexReport:000}</td> <td style=\"text-align:left;\"><b>{LaptopMainViewModel.Result}</b></td> <td style=\"background-color: red; text-align:center;\"><b>0</b></td> <td style=\"background-color: red; text-align:center;\"><b>0</b></td> <td style=\"text-align:center;\">{DateTime.Now:dd.MM.yyyy}</td> <td></td> <td></td> <td></td> </tr>" +
            //        $"<tr> <td style=\"text-align:right;\">Cam {LaptopOtherViewModel.WebCam}</td> <td style=\"text-align:left;\">{CPUViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
            //        $"<tr> <td style=\"text-align:right;\">Mic {LaptopOtherViewModel.Microphone}</td> <td style=\"text-align:left;\">{MemoryViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
            //        $"<tr> <td></td> <td style=\"text-align:left;\">{VideoViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
            //        $"<tr> <td></td> <td style=\"text-align:left;\">{PhysicalDiskViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
            //        $"<tr> <td></td> <td style=\"text-align:left;\">{LaptopOtherViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
            //        $"<tr> <td></td> <td style=\"text-align:left;\">{LaptopBatteryViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
            //        $"<tr> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> </tr>" +
            //        $"</tbody> </table> </body> </html>";
            //    await _ioManager.WriteAllTextAsync($"{ReportPath}\\Report_{IndexReport:000}.html", html);
            //}
            //catch (Exception e)
            //{
            //    _messageDialogService.ShowInfoDialog($"В процесі формування звіту HTML відбулася помилка.\n\nФайл: {ReportPath}\\Report_{IndexReport:000}.html не створено!\n\nЗвернись до розробника і скинь фото:\n\n" +
            //       $"{e.Message}\n\n{e.StackTrace}", "Помилка");
            //}

        }
        private async Task GetDOCX()
        {
            //try
            //{
            //    var documentPath = $"{ReportPath}\\SourcePrice\\document.xml";
            //    if (!File.Exists(documentPath))
            //    {
            //        _messageDialogService.ShowInfoDialog($"Не знайдено файл: {documentPath}", "Помилка!");
            //        return;
            //    }

            //    var unzipPath = $"{ReportPath}\\SourcePrice\\unzip\\word";
            //    if (!Directory.Exists(unzipPath))
            //    {
            //        _messageDialogService.ShowInfoDialog($"Не знайдено папку: {documentPath}", "Помилка!");
            //        return;
            //    }

            //    File.Copy(documentPath, $"{unzipPath}\\document.xml", true);
            //    string text = string.Empty;
            //    using (StreamReader reader = new StreamReader($"{unzipPath}\\document.xml"))
            //    {
            //        text = await reader.ReadToEndAsync();
            //    }

            //    text = text.Replace("brand", LaptopMainViewModel.Brand);

            //    if (!string.IsNullOrWhiteSpace(LaptopMainViewModel.Line))
            //        text = text.Replace("model", $"{LaptopMainViewModel.Line} {LaptopMainViewModel.Model}");
            //    else
            //        text = text.Replace("model", $"{LaptopMainViewModel.Model}");

            //    text = text.Replace("diag", LaptopMonitorViewModel.Result);
            //    text = text.Replace("compascpu", CPUViewModel.Result);
            //    text = text.Replace("compasmemory", MemoryViewModel.Result);
            //    text = text.Replace("compasgpu", VideoViewModel.Result);
            //    text = text.Replace("compashdd", PhysicalDiskViewModel.Result);
            //    text = text.Replace("compasother", LaptopOtherViewModel.Result);
            //    text = text.Replace("compasbettery", LaptopBatteryViewModel.Result);
            //    text = text.Replace("compasid", $"{IndexReport:000}");

            //    using (StreamWriter writer = new StreamWriter($"{unzipPath}\\document.xml", false))
            //    {
            //        await writer.WriteLineAsync(text);
            //    }

            //    if (File.Exists($"{ReportPath}\\Report_{IndexReport:000}.docx"))
            //        File.Delete($"{ReportPath}\\Report_{IndexReport:000}.docx");

            //    ZipFile.CreateFromDirectory($"{ReportPath}\\SourcePrice\\unzip", $"{ReportPath}\\Report_{IndexReport:000}.docx", CompressionLevel.SmallestSize, false);
            //}
            //catch (Exception e)
            //{
            //    _messageDialogService.ShowInfoDialog($"В процесі формування звіту docx відбулася помилка.\n\nФайл: {ReportPath}\\Report_{IndexReport:000}.docx не створено!\n\nЗвернись до розробника і скинь фото:\n\n" +
            //      $"{e.Message}\n\n{e.StackTrace}", "Помилка");
            //}
        }

        public bool HasChanges()
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync(int? Id)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe()
        {
            throw new NotImplementedException();
        }

    }
}
