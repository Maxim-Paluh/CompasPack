using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogInstall
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var CompasPackLogFile = string.Empty;
            try
            {
                var name = args.FirstOrDefault();
                var CompasPackLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CompasPackLog");
                CompasPackLogFile = Path.Combine(CompasPackLog, name != null ? $"LogInstall_{name}." : $"LogInstall_{DateTime.Now.ToString().Replace(':', '.').Replace(' ', '_')}.");
                var CurrentDirectory = Directory.GetCurrentDirectory();
                var Root = Path.GetPathRoot(CurrentDirectory);
                var aidaExe = Path.Combine(Root, ReadAllTextAsync("AidaPath.txt"));
                var aidaRPF = Path.Combine(Path.GetDirectoryName(aidaExe), "ForLog.rpf");
                GetAidaReport(aidaExe, CompasPackLogFile, "/HML", aidaRPF, 240);
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"В процесi формування звiту AIDA64 вiдбулася помилка:\n{ex.Message}\nФайл: {CompasPackLogFile}hml не створено!");
                Thread.Sleep(10000);
            }
        }

        public static void GetAidaReport(string aidaExeFilePath, string reportPath, string type, string rpfPath, int secondswait)
        {
            if (string.IsNullOrWhiteSpace(aidaExeFilePath))
                throw new ArgumentException($"{nameof(aidaExeFilePath)} Is Null Or White Space");

            if (string.IsNullOrWhiteSpace(reportPath))
                throw new ArgumentException($"{nameof(reportPath)} Is Null Or White Space");

            if (string.IsNullOrWhiteSpace(rpfPath))
                throw new ArgumentException($"{nameof(rpfPath)} Is Null Or White Space");


            Process proc = null;
            ProcessStartInfo StartInfo = new ProcessStartInfo
            {
                FileName = aidaExeFilePath,
                Arguments = $"/R {reportPath} {type} /CUSTOM {rpfPath}",
                UseShellExecute = false
            };
            try
            {
                proc = Process.Start(StartInfo);// TODO
                Console.WriteLine("Запущено генерацiю звiту з такими параметрами:");
                Console.WriteLine($"FileName: {StartInfo.FileName}");
                Console.WriteLine($"Arguments: {StartInfo.Arguments}");
                if (!proc.WaitForExit(secondswait*1000))
                {
                    throw new Exception("Aida report time out");
                }
                Console.WriteLine($"Готово!!!");
            }
            catch (Exception)
            {
                try { proc?.Kill(); } catch (Exception) { }
                try { proc?.Close(); } catch (Exception) { }
                throw;
            }
        }

        public static string ReadAllTextAsync(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
