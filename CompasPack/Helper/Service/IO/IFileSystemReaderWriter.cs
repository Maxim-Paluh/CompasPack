using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public interface IFileSystemReaderWriter
    {
        string CompasPackLog { get; set; }
        string PathRoot { get; set; }
        Task<string> ReadAllTextAsync(string path);
        Task WriteAllTextAsync(string path, string text);
        string[] GetListFile(string path);
        string[] GetListFolder(string path);
    }
}
