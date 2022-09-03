using System.Diagnostics;
using System.IO;

string CompasPacLogName = "CompasPackLog";
var CompasPacLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\"+ CompasPacLogName;
var CurrentDirectory = Directory.GetCurrentDirectory();
var Root = Path.GetPathRoot(CurrentDirectory);
var AidaPath = Path.Combine(Root, "Programs\\Portable\\AIDA64");

if (!Directory.Exists(CompasPacLog))
    Directory.CreateDirectory(CompasPacLog);

var DataTime = DateTime.Now.ToString().Replace(':', '.').Replace(' ', '_');

if (args.Length == 1)
{
    DataTime = args.First().Replace(' ', '_') + "_" + DataTime;
}
else
{
    DataTime = "NoName_" + DataTime;
}

var proc = new ProcessStartInfo()
{
    UseShellExecute = true,
    FileName = AidaPath + "\\aida64.exe",
    Arguments = "/R " + CompasPacLog + $"\\{DataTime}. " + "/HTML " + "/CUSTOM " + AidaPath+ "\\ForLog.rpf",
};

var process = Process.Start(proc);
Console.WriteLine("Запущено генерацiю звiту з такими параметрами:");
Console.WriteLine($"FileName: {proc.FileName}");
Console.WriteLine($"Arguments: {proc.Arguments}");
process.WaitForExit();
Console.WriteLine($"Готово!!!");
