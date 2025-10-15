using CompasPack.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public interface IFileArchiver
    {
        ResultArchiverEnum Decompress(string pathRar, string pathFolder, string password, int timeOut);
    }
}
