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
        Task<string> ReadAllTextAsync(string path);
        Task WriteAllTextAsync(string path, string text);
        //--------------------------------------------------------
        string[] GetListFile(string path);
        string[] GetListFolder(string path);
        //--------------------------------------------------------
        void OpenFolder(string path);
        void OpenCreateFolder(string path);
        void OpenFolderAndSelectFile(string path);
        //--------------------------------------------------------
        string CompasPackLog { get; set; }
        string PathRoot { get; set; }
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
            return await Task.Factory.StartNew(() => File.ReadAllText(path));
        }
        public async Task WriteAllTextAsync(string pathFile, string text)
        {
            if (!Directory.Exists(Path.GetDirectoryName(pathFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
            await Task.Factory.StartNew(() => File.WriteAllText(pathFile, text));
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
    }
}
