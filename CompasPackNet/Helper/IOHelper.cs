using CompasPack.View.Service;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack.Helper
{
    public interface IIOHelper
    {
        public Task<string> ReadAllTextAsync(string path);
        public Task WriteAllTextAsync(string path, string text);
        //--------------------------------------------------------
        public string[] GetListFile(string path);
        public string[] GetListFolder(string path);
        //--------------------------------------------------------
        public void OpenFolder(string path);
        public void OpenCreateFolder(string path);
        public void OpenFolderAndSelectFile(string path);
        //--------------------------------------------------------
        public string CompasPackLog { get; set; }
        public string PathRoot { get; set; }
    }

    public class IOHelper : IIOHelper
    {
        private IMessageDialogService _messageDialogService;
        public string PathRoot { get; set; }
        public string CompasPackLog { get; set; }

        public IOHelper(IMessageDialogService messageDialogService)
        {
            _messageDialogService = messageDialogService;
            PathRoot = Path.GetPathRoot(Directory.GetCurrentDirectory());
            CompasPackLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CompasPackLog");
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
        //-------------------------------------------------------------------------------
        public string[] GetListFile(string path)
        {
            if(Directory.Exists(path))
                return Directory.GetFiles(path);
            else return new string[0];
        }
        public string[] GetListFolder(string path)
        {
            if (Directory.Exists(path))
                return Directory.GetDirectories(path);
            else 
                return new string[0];
        }
        //-------------------------------------------------------------------------------
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
        //-------------------------------------------------------------------------------



        //public async Task<XDocument> GetXDocument()
        //{
        //    try
        //    {
        //        if (!Directory.Exists(CompasPackLog))
        //            Directory.CreateDirectory(CompasPackLog);
                
        //        #if DEBUG
        //        if (!File.Exists(CompasPackLog + "\\Report.xml"))
        //        {
        //            Process proc = Process.Start(new ProcessStartInfo()
        //            {
        //                FileName = Aida,
        //                Arguments = "/R " + CompasPackLog + "\\Report. " + "/XML " + "/CUSTOM " + Path.GetDirectoryName(Aida) + "\\ForReport.rpf",
        //                UseShellExecute = false
        //            });
        //            await proc.WaitForExitAsync();
        //        }
        //        #else
        //            Process proc = Process.Start(new ProcessStartInfo()
        //            {
        //                FileName = Aida,
        //                Arguments = "/R " + CompasPackLog + "\\Report. " + "/XML " + "/CUSTOM " + Path.GetDirectoryName(Aida) + "\\ForReport.rpf",
        //                UseShellExecute = false
        //            });
        //            await proc.WaitForExitAsync();
        //        #endif

        //        if (!File.Exists(CompasPackLog + "\\Report.xml"))
        //            return null;
                
        //        XDocument? document;
        //        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //        using (var stream = new StreamReader(CompasPackLog + "\\Report.xml", Encoding.GetEncoding("windows-1251")))
        //        {
        //            document = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, new System.Threading.CancellationToken());
        //        }
        //        return document;
        //    }
        //    catch (Exception) 
        //    {
        //        _messageDialogService.ShowInfoDialog($"Звіт AIDA64 не сформовано!", "Помилка!");
        //        return null;
        //    }
        //}
    }
}
