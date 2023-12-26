using System;
using System.IO;
using System.Linq;

namespace CompasPack.Helper
{
    public static class KMSAutoHelper
    {
        public static string? FindKMSAutoExe(string path)
        {
            var directori = Directory.GetDirectories(path)
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
        public static string? FindKMSAutoRar(string path)
        {
            var File = Directory.GetFiles(path)
               .Where(x => x.Contains("KMSAuto", StringComparison.InvariantCultureIgnoreCase) && x.EndsWith(".rar")).LastOrDefault();
            return File;
        }
    }
}
