using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public class FileSystemReaderWriter : IFileSystemReaderWriter
    {   
        public string PathRoot { get; set; }
        public string CompasPackLog { get; set; }

        public FileSystemReaderWriter()
        {
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
    }
}
