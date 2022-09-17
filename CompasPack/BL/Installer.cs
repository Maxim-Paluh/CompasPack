using CompasPack.Data;
using CompasPack.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace CompasPack.BL
{
    public static class Installer
    {
        public async static Task<string> InstallProgram(UserProgramViewModel userProgramViewMode, bool onlineInstall)
        {
            var userProgram = userProgramViewMode.UserProgram;
            StringBuilder stringBuilder = new StringBuilder();

            string? exeFile = null;
            string? arguments = null;
            if (onlineInstall)
            {
                exeFile = Directory.GetFiles(userProgram.PathFolder)
                 .Where(x => x.Contains(userProgram.OnlineInstaller.FileName, StringComparison.InvariantCultureIgnoreCase) && x.EndsWith(".exe")).FirstOrDefault();
                arguments = String.Join(" ", userProgram.OnlineInstaller.Arguments);
            }
            if (exeFile == null)
            {
                var exeFiles = Directory.GetFiles(userProgram.PathFolder)
                   .Where(x => x.Contains(userProgram.FileName, StringComparison.InvariantCultureIgnoreCase) && x.EndsWith(".exe"));
                if (userProgram.Architecture == "x64")
                {
                    if (WinInfo.GetIs64BitOperatingSystem())
                        exeFile = exeFiles.Where(x => x.Contains("x64", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    else
                        exeFile = exeFiles.Where(x => x.Contains("x86", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                }
                else
                    exeFile = exeFiles.LastOrDefault();
                arguments = String.Join(" ", userProgram.Arguments);
            }

            if (exeFile != null)
            {
                var StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(userProgram.PathFolder, exeFile),
                    Arguments = arguments,
                    UseShellExecute = false
                };
                try
                {
                    stringBuilder.Append($"File: {StartInfo.FileName}\n");
                    stringBuilder.Append($"Arguments: {StartInfo.Arguments}\n");

                    Process proc = Process.Start(StartInfo);
                    await proc.WaitForExitAsync();
                    stringBuilder.Append($"Programs: {userProgram.ProgramName}, Installed!!!\n");
                    userProgramViewMode.CheckInstall(WinInfo.ListInstallPrograms());
                }
                catch (Exception exp)
                {
                    stringBuilder.Append($"Program: {userProgram.ProgramName}.\nError install: \n{exp.Message}\n");
                }
            }
            else
            {
                stringBuilder.Append($"Not fount file: {userProgram.ProgramName} In folder: {userProgram.PathFolder}\n");
            }

            return stringBuilder.ToString();
        }
    }
}
