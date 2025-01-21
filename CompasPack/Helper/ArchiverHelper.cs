using CompasPack.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper
{
    public static class ArchiverHelper
    {
        public static ResultArchiver Decompress(string pathRar, string pathFolder, string password, int timeOut = 10000 )
        {
            try
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.FileName = "7za.exe";
                ps.Arguments = $"x \"{pathRar}\" -o\"{pathFolder}\" -p{password} -aoa";  //-aoa Overwrite All existing files without prompt. 
                
                var proc = Process.Start(ps);
                if (!proc.WaitForExit(timeOut))
                {
                    try { proc.Kill(); } catch (Exception) { }
                    try { proc.Close(); } catch (Exception) { }
                    return ResultArchiver.TimeOut; 
                }
                else
                    return ResultArchiver.OK;
            }
            catch (Exception)
            {
                return ResultArchiver.Error;
            }
        }
    }

    public enum ResultArchiver
    {
        OK,
        TimeOut,
        Error
    }
}
