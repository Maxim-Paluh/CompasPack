using CompasPack.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Data.Providers.API
{
    public interface IWinInfoProvider
    {
        string ProductName { get; }
        string DisplayVersion { get; }
        string EditionID { get; }
        string CurrentBuild { get; }
        WinArchitectureEnum WinArchitecture { get; }
        WinVersionEnum WinVer { get; }
    }
}
