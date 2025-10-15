using CompasPack.Model.ViewAndViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Data.Providers.API
{
    internal class HardwareInfoProviderWin7 : HardwareInfoProviderBase
    {
        public override List<DiskInfo> GetDiskInfos()
        {
            return new List<DiskInfo>();
        }
    }
}
