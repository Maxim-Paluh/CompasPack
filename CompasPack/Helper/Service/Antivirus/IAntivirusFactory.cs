using CompasPack.Model.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service.Antivirus
{
    public interface IAntivirusFactory
    {
        IAntivirus Create(AntivirusInfo info);
    }
}
