using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public interface IFileSystemNavigator
    {
        void OpenFolder(string path);
        void OpenCreateFolder(string path);
        void OpenFolderAndSelectFile(string path);
    }
}
