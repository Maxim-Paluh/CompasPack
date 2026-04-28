using CompasPack.Model.Enum;
using CompasPack.Model.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Data.Providers.API
{
    public interface IWinInfoProvider
    {
        WinInfo GetWinInfo();
    }
}
