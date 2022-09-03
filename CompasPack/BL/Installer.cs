using CompasPac.Data;
using CompasPac.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace CompasPac.BL
{
    public static class Installer
    {
        public async static Task<string> InstallProgram(UserProgramViewModel userProgramViewMode)
        {
            var userProgram = userProgramViewMode.UserProgram;
            StringBuilder stringBuilder = new StringBuilder();

            string? exeFile = null;
            string? arguments = null;
            if (userProgram.OnlineInstaller != null)
            {
                if(string.IsNullOrWhiteSpace(userProgram.InstallProgramName) || WinInfo.IsInstallPrograms(WinInfo.ListInstallPrograms(), userProgram.InstallProgramName))
                {
                    exeFile = Directory.GetFiles(userProgram.PathFolder)
                         .Where(x => x.Contains(userProgram.OnlineInstaller.FileName, StringComparison.InvariantCultureIgnoreCase) && x.EndsWith(".exe")).FirstOrDefault();
                }
                else
                {
                    if(await Network.IsFastSpeed())
                    {
                        exeFile = Directory.GetFiles(userProgram.PathFolder)
                         .Where(x => x.Contains(userProgram.OnlineInstaller.FileName, StringComparison.InvariantCultureIgnoreCase) && x.EndsWith(".exe")).FirstOrDefault();
                    }
                }
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
            }

            if (exeFile != null)
            {
                var StartInfo = new ProcessStartInfo
                {

                    FileName = Path.Combine(userProgram.PathFolder, exeFile),
                    Arguments = String.Join(" ", userProgram.Arguments),
                    UseShellExecute = false
                };

                stringBuilder.Append($"Start Install Programs: {userProgram.ProgramName}\nFrom: {exeFile}\n");
                try
                {
                    if (!string.IsNullOrWhiteSpace(userProgram.InstallProgramName) && WinInfo.IsInstallPrograms(WinInfo.ListInstallPrograms(), userProgram.InstallProgramName))
                        stringBuilder.Append($"Programs: {userProgram.ProgramName}, Already Installed!!!\n");
                    else
                    {
                        if (userProgram.DisableDefender)
                            stringBuilder.Append(await WinDefender.DisableRealtimeMonitoring());

                        Process proc = Process.Start(StartInfo);
                        await proc.WaitForExitAsync();
                        stringBuilder.Append($"Programs: {userProgram.ProgramName}, Installed!!!\n");
                        userProgramViewMode.CheckInstall(WinInfo.ListInstallPrograms());
                    }
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
            stringBuilder.Append("<-------------------------------------------------------->\n");
            return stringBuilder.ToString();
        }
    }
}
