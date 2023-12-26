using CompasPack.View.Service;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace CompasPack.Helper
{
    public interface IIOHelper
    {
        public Task<string> ReadAllTextAsync(string path);
        public Task WriteAllTextAsync(string path, string text);
        public Task<XDocument> GetXDocument();
        public void CheckReportFolders();
        public int GetLastReport(string type);

        public void OpenFolder(string path);
        public void OpenCreateFolder(string path);
        public void OpenFolderAndSelectFile(string path);

        public void ReInstallPatternDocx();

        public string CompasPackLog { get; set; }
        public string PathRoot { get; set; }

        public string Aida { get; set; }
        public string WinRar { get; set; }
        public string Crack { get; set; }

        public string ReportPC { get; set; }
        public string ReportLaptop { get; set; }
        public string ReportMonitor { get; set; }
    }

    public class IOHelper : IIOHelper
    {
        private IMessageDialogService _messageDialogService;

        private static readonly string _portable = "!Portable\\";

        private static readonly string crack = _portable + "!Crack";

        private static readonly string aida = _portable + "AIDA64\\aida64.exe";
        private static readonly string winRar = _portable + "WinRAR\\WinRAR.exe";

        public string CurrentDirectoryPath { get; set; }
        public string SettingsGroupProgramFileNamePath { get; set; }
        public string SettingUserPresetProgramFileNamePath { get; set; }
        public string SettingsUserReportPath { get; set; }
        public string PathRoot { get; set; }
        public string CompasPackLog { get; set; }
        
        public string Aida { get; set; }
        public string WinRar { get; set; }
        public string Crack { get; set; }
        public string Report { get; set; }

        public string ReportPC { get; set; }
        public string ReportLaptop { get; set; }
        public string ReportMonitor { get; set; }

        public IOHelper(IMessageDialogService messageDialogService)
        {
            _messageDialogService = messageDialogService;
            CompasPackLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CompasPackLog");
            CurrentDirectoryPath = Directory.GetCurrentDirectory();
            SettingsUserReportPath = CurrentDirectoryPath + "\\" + "SettingsUserReport.json";
            PathRoot = Path.GetPathRoot(Directory.GetCurrentDirectory());

            Aida = Path.Combine(PathRoot, aida);
            WinRar = Path.Combine(PathRoot, winRar);
            Crack = Path.Combine(PathRoot, crack);

            Report = Path.Combine(PathRoot, "Report");
            ReportPC = Path.Combine(Report, "pc");
            ReportLaptop = Path.Combine(Report, "laptop");
            ReportMonitor = Path.Combine(Report, "monitor");
        }

        public async Task<string> ReadAllTextAsync(string path)
        {
            return await File.ReadAllTextAsync(path).ConfigureAwait(false);
        }

        public async Task WriteAllTextAsync(string pathFile, string text)
        {
            if (!Directory.Exists(Path.GetDirectoryName(pathFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
            await File.WriteAllTextAsync(pathFile, text).ConfigureAwait(false);
        }


        public void OpenFolder(string path)
        {
            if (!Directory.Exists(path) )
                _messageDialogService.ShowInfoDialog($"Не знайдено папки за шляхом:\n{path}\nПеревірте налаштування", "Помилка!");
            else
                Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", path);
        }

        public void OpenCreateFolder(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
                Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", path);
        }


        public void OpenFolderAndSelectFile(string path)
        {
            if (File.Exists(path))
            {
                string argument = "/select, \"" + path + "\"";
                Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", argument);
            }
        }
        public void CheckReportFolders()
        {
            if (!Directory.Exists(Report))
                Directory.CreateDirectory(Report);
            if (!Directory.Exists(ReportPC))
                Directory.CreateDirectory(ReportPC);
            if (!Directory.Exists(ReportLaptop))
                Directory.CreateDirectory(ReportLaptop);
            if (!Directory.Exists(ReportMonitor))
                Directory.CreateDirectory(ReportMonitor);
        }

        public void ReInstallPatternDocx()
        {
            CheckReportFolders();
            ReInstallPatternDocxInFolder(Path.Combine(CurrentDirectoryPath, "SourcePricePC.rar"), ReportPC);
            ReInstallPatternDocxInFolder(Path.Combine(CurrentDirectoryPath, "SourcePriceLaptop.rar"), ReportLaptop);
            ReInstallPatternDocxInFolder(Path.Combine(CurrentDirectoryPath, "SourcePriceMonitor.rar"), ReportMonitor);
        }

        private void ReInstallPatternDocxInFolder(string pathRar, string pathFolder)
        {
            if (Directory.Exists($"{pathFolder}\\SourcePrice"))
                Directory.Delete($"{pathFolder}\\SourcePrice", true);

            if (File.Exists(WinRar))
            {
                if (File.Exists(pathRar))
                {
                    try
                    {
                        ProcessStartInfo ps = new ProcessStartInfo();
                        ps.FileName = WinRar;
                        ps.Arguments = $@"x -p1234 -o- {pathRar} {pathFolder}";
                        var proc = Process.Start(ps);
                        if (!proc.WaitForExit(20000))
                        {
                            try { proc.Kill(); } catch (Exception) { }
                            try { proc.Close(); } catch (Exception) { }

                            _messageDialogService.ShowInfoDialog($"Не вдалося розпакувати архів:\n{pathRar}\nСпробуйте ще ра!", "Помилка!");
                        }
                    }
                    catch (Exception)
                    {
                        _messageDialogService.ShowInfoDialog($"Не вдалося розпакувати архів:\n{pathRar}\nСпробуйте ще ра!", "Помилка!");
                    }
                }
                else
                    _messageDialogService.ShowInfoDialog($"Не знайдено архів:\n{pathRar}\nПоверни шаблон в корінь програми!", "Помилка!");
            }
            else
                _messageDialogService.ShowInfoDialog("Не знайдено Rar.exe!\nНеможливо розпакувати архіви!", "Помилка!");
        }
        public int GetLastReport(string paht)
        {
            var lastString = Directory.GetFiles(paht).Select(x => x = Regex.Match(x, "\\d+").Value).OrderBy(x => x).LastOrDefault();
            if (lastString != null)
            {
                if (int.TryParse(lastString, out int last))
                    return last;
                else
                    return -1;
            }
            else
                return -1;
        }
        public async Task<XDocument> GetXDocument()
        {
            try
            {
                if (!Directory.Exists(CompasPackLog))
                    Directory.CreateDirectory(CompasPackLog);
                
                #if DEBUG
                if (!File.Exists(CompasPackLog + "\\Report.xml"))
                {
                    Process proc = Process.Start(new ProcessStartInfo()
                    {
                        FileName = Aida,
                        Arguments = "/R " + CompasPackLog + "\\Report. " + "/XML " + "/CUSTOM " + Path.GetDirectoryName(Aida) + "\\ForReport.rpf",
                        UseShellExecute = false
                    });
                    await proc.WaitForExitAsync();
                }
                #else
                    Process proc = Process.Start(new ProcessStartInfo()
                    {
                        FileName = Aida,
                        Arguments = "/R " + CompasPackLog + "\\Report. " + "/XML " + "/CUSTOM " + Path.GetDirectoryName(Aida) + "\\ForReport.rpf",
                        UseShellExecute = false
                    });
                    await proc.WaitForExitAsync();
                #endif

                if (!File.Exists(CompasPackLog + "\\Report.xml"))
                    return null;
                
                XDocument? document;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using (var stream = new StreamReader(CompasPackLog + "\\Report.xml", Encoding.GetEncoding("windows-1251")))
                {
                    document = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, new System.Threading.CancellationToken());
                }
                return document;
            }
            catch (Exception) 
            {
                _messageDialogService.ShowInfoDialog($"Звіт AIDA64 не сформовано!", "Помилка!");
                return null;
            }
        }
    }
}
