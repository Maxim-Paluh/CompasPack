using CompasPack.Data;
using CompasPakc.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.BL
{
    public static class KMSAuto
    {
        public static string? FindKMSAutoExe(IIOManager iOManager)
        {
            var directori = Directory.GetDirectories(iOManager.Crack)
                   .Where(x => x.Contains("KMSAuto", StringComparison.InvariantCultureIgnoreCase)).LastOrDefault();

            if (!string.IsNullOrWhiteSpace(directori))
            {
                var File = Directory.GetFiles(directori)
                   .Where(x => x.Contains("KMSAuto Net", StringComparison.InvariantCultureIgnoreCase)).LastOrDefault();
                return File;
            }
            else
            { return null; }
        }
        public static string? FindKMSAutoRar(IIOManager iOManager)
        {
            var File = Directory.GetFiles(iOManager.Crack)
               .Where(x => x.Contains("KMSAuto", StringComparison.InvariantCultureIgnoreCase) && x.EndsWith(".rar")).LastOrDefault();
            return File;
        }
    }
}
