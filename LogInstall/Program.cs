using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using LogInstall;

string CompasPackLogName = "CompasPackLog";
var CompasPackLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\"+ CompasPackLogName;
var CurrentDirectory = Directory.GetCurrentDirectory();
var Root = Path.GetPathRoot(CurrentDirectory);
var AidaPath = Path.Combine(Root, "!Portable\\AIDA64");
var typeReport = TypeReport.Unknown;

if (!Directory.Exists(CompasPackLog))
    Directory.CreateDirectory(CompasPackLog);

var _fileName = DateTime.Now.ToString().Replace(':', '.').Replace(' ', '_');

if (args.Any(x => x.Contains("\\LogInstall", StringComparison.InvariantCultureIgnoreCase)))
    typeReport = TypeReport.LogInstall;
if (args.Any(x => x.Contains("\\Report", StringComparison.InvariantCultureIgnoreCase)))
    typeReport = TypeReport.Report;

if (typeReport == TypeReport.LogInstall)
{
    var name = args.FirstOrDefault(x => x.Contains("Name:", StringComparison.InvariantCultureIgnoreCase));
    if (!string.IsNullOrWhiteSpace(name))
        _fileName = new Regex("Name:").Replace(name, "") + "_" + _fileName;
}



switch (typeReport)
{
    case TypeReport.Unknown:
        Console.WriteLine($"Не вказано тип звiту!!!");
        break;
    case TypeReport.LogInstall:
        {
            var proc = new ProcessStartInfo()
            {
                UseShellExecute = false,
                FileName = AidaPath + "\\aida64.exe",
                Arguments = "/R " + CompasPackLog + $"\\{"LogInstall_" +_fileName}. " + "/HTML " + "/CUSTOM " + AidaPath + "\\ForLog.rpf",
            };

            var process = Process.Start(proc);
            Console.WriteLine("Запущено генерацiю звiту з такими параметрами:");
            Console.WriteLine($"FileName: {proc.FileName}");
            Console.WriteLine($"Arguments: {proc.Arguments}");
            process.WaitForExit();
            Console.WriteLine($"Готово!!!");
        }
        break;
    case TypeReport.Report:
        {
            var proc = new ProcessStartInfo()
            {
                UseShellExecute = false,
                FileName = AidaPath + "\\aida64.exe",
                Arguments = "/R " + CompasPackLog + "\\Report. " + "/XML " + "/CUSTOM " + AidaPath + "\\ForReportPC.rpf",
            };

            var process = Process.Start(proc);
            Console.WriteLine("Запущено генерацiю звiту з такими параметрами:");
            Console.WriteLine($"FileName: {proc.FileName}");
            Console.WriteLine($"Arguments: {proc.Arguments}");
            process.WaitForExit();
            Console.WriteLine($"Готово!!!");
        }
        break;
    default:
        Console.WriteLine($"Не вказано тип звiту!!!");
        break;
}

