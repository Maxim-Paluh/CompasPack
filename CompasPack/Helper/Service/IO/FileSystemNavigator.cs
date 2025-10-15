using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public class FileSystemNavigator : IFileSystemNavigator
    {
        private IMessageDialogService _messageDialogService;
        public FileSystemNavigator(IMessageDialogService messageDialogService)
        {
            _messageDialogService = messageDialogService;
        }
        public void OpenFolder(string path)
        {
            if (!Directory.Exists(path))
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
    }
}
