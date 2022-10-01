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

            string? ExecutableFile = null;
            string? arguments = null;
            if (onlineInstall)
            {
                var File = Directory.GetFiles(userProgram.PathFolder)
                    .Where(x => x.Contains(userProgram.OnlineInstaller.FileName, StringComparison.InvariantCultureIgnoreCase))
                   . Where(x => x.Contains("exe") || x.Contains("msi")).FirstOrDefault();

                arguments = string.Join(" ", userProgram.OnlineInstaller.Arguments);
            }
            if (ExecutableFile == null)
            {
                var Files = Directory.GetFiles(userProgram.PathFolder)
                   .Where(x => x.Contains(userProgram.FileName, StringComparison.InvariantCultureIgnoreCase)).Where(x => x.Contains("exe") || x.Contains("msi"));
                if (userProgram.Architecture == "x64")
                {
                    if (WinInfo.GetIs64BitOperatingSystem())
                        ExecutableFile = Files.Where(x => x.Contains("x64", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    else
                        ExecutableFile = Files.Where(x => x.Contains("x86", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                }
                else
                    ExecutableFile = Files.LastOrDefault();
                arguments = String.Join(" ", userProgram.Arguments);
            }

            if (ExecutableFile != null)
            {
                var StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(userProgram.PathFolder, ExecutableFile),
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
