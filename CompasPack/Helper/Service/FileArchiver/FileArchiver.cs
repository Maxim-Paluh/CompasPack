using System;
using System.Diagnostics;
using CompasPack.Model.Enum;

namespace CompasPack.Helper.Service
{
    public class FileArchiver : IFileArchiver
    {
        public  ResultArchiverEnum Decompress(string pathRar, string pathFolder, string password, int timeOut = 10000 )
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
                    return ResultArchiverEnum.TimeOut; 
                }
                else
                    return ResultArchiverEnum.OK;
            }
            catch (Exception)
            {
                return ResultArchiverEnum.Error;
            }
        }
    }

}
