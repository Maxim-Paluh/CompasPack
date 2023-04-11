using CompasPack.Data;
using CompasPack.View.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompasPack.ViewModel;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace CompasPakc.BL
{
    public interface IIOManager
    {
        public Task WriteAllTextAsync(string path, string text);

        public Task<List<UserPresetProgram>> GetUserPresetProgram();
        public Task<List<GroupProgram>> GetGroupPrograms();

        public Task<SettingsReportViewModel> GetSettingsReport();
        public Task<XDocument> GetXDocument();
        public void CheckReportFolders();
        public int GetLastReport(string type);

        public void OpenFolder(string path);
        public void OpenFolderAndSelectFile(string path);

        public Task SetDefaultGroupProgram();
        public Task SetDefaultUserPresetProgram();
        public Task SetSettingsReport();

        public string CompasPackLog { get; set; }
        public string CompasExampleFile { get; set; }

        public string CpuZ { get; set; }
        public string GpuZ { get; set; }
        public string Aida { get; set; }
        public string CrystalDisk { get; set; }
        public string FurMark { get; set; }
        public string TotalCommander951 { get; set; }
        public string TotalCommander700 { get; set; }
        public string WinRar { get; set; }
        public string Rar { get; set; }
        public string Crack { get; set; }

        public string ReportPC { get; set; }
        public string ReportLaptop { get; set; }
        public string ReportMonitor { get; set; }
    }

    public class IOManager : IIOManager
    {
        private IMessageDialogService _messageDialogService;

        private static readonly string _portable = "!Portable\\";
        private static readonly string _install = "!Install\\";

        private static readonly string _compasPackLogName = "CompasPackLog";
        private static readonly string crack = _portable + "!Crack";

        private static readonly string cpuZ =               _portable + "CPU-Z\\cpuz_x32.exe";
        private static readonly string gpuZ =               _portable + "FurMark\\gpuz.exe";
        private static readonly string aida =               _portable + "AIDA64\\aida64.exe";
        private static readonly string crystalDisk =        _portable + "CrystalDisk\\DiskInfo32.exe";
        private static readonly string furMark =            _portable + "FurMark\\FurMark.exe";
        private static readonly string totalCommander951 =  _portable + "TotalCommander951\\TOTALCMD.EXE";
        private static readonly string totalCommander700 =  _portable + "TotalCommander700\\Totalcmd.exe";
        private static readonly string winRar =             _portable + "WinRAR\\WinRAR.exe";
        private static readonly string rar =                _portable + "WinRAR\\Rar.exe";

        public string CurrentDirectoryPath { get; set; }
        public string SettingsGroupProgramFileNamePath { get; set; }
        public string SettingUserPresetProgramFileNamePath { get; set; }
        public string SettingsUserReportPath { get; set; }
        public string PathRoot { get; set; }
        public string CompasPackLog { get; set; }
        public string CompasExampleFile { get; set; }
        public string CpuZ { get; set; }
        public string GpuZ { get; set; }
        public string Aida { get; set; }
        public string CrystalDisk { get; set; }
        public string FurMark { get; set; }    
        public string TotalCommander951 { get; set; }
        public string TotalCommander700 { get; set; }
        public string WinRar { get; set; }
        public string Rar { get; set; }
        public string Crack { get; set; }
        public string Report { get; set; }

        public string ReportPC { get; set; }
        public string ReportLaptop{ get; set; }
        public string ReportMonitor { get; set; }

        public IOManager(IMessageDialogService messageDialogService)
        {
            _messageDialogService = messageDialogService;
            CompasPackLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + _compasPackLogName;
            CurrentDirectoryPath = Directory.GetCurrentDirectory();
            SettingsGroupProgramFileNamePath = Directory.GetCurrentDirectory() + "\\" + "SettingsPrograms.json";
            SettingUserPresetProgramFileNamePath = Directory.GetCurrentDirectory() + "\\" + "SettingsPreset.json";
            SettingsUserReportPath = Directory.GetCurrentDirectory() + "\\" + "SettingsUserReport.json";
            PathRoot = Path.GetPathRoot(Directory.GetCurrentDirectory());

            CompasExampleFile = Path.Combine(PathRoot, _install + "!ExampleFile");

            CpuZ = Path.Combine(PathRoot, cpuZ);
            GpuZ = Path.Combine(PathRoot, gpuZ);
            Aida = Path.Combine(PathRoot, aida);
            CrystalDisk = Path.Combine(PathRoot, crystalDisk);
            FurMark = Path.Combine(PathRoot, furMark);
            TotalCommander951 = Path.Combine(PathRoot, totalCommander951);
            TotalCommander700 = Path.Combine(PathRoot, totalCommander700);
            WinRar = Path.Combine(PathRoot, winRar);
            Rar = Path.Combine(PathRoot, rar);
            Crack = Path.Combine(PathRoot, crack);

            Report = Path.Combine(PathRoot, "Report");
            ReportPC = Path.Combine(Report, "pc");
            ReportLaptop = Path.Combine(Report, "laptop");
            ReportMonitor = Path.Combine(Report, "monitor");
        }
        public async Task<List<GroupProgram>> GetGroupPrograms()
        {
            FileInfo fileSettingsJson = new FileInfo(SettingsGroupProgramFileNamePath);

            if (!fileSettingsJson.Exists)
                await SetDefaultGroupProgram();

            try
            {
                var temp = JsonConvert.DeserializeObject<List<GroupProgram>>(await
                File.ReadAllTextAsync(SettingsGroupProgramFileNamePath),
                new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error, });

                foreach (var program in temp.SelectMany(group => group.UserPrograms))
                {
                    program.PathFolder = Path.Combine(PathRoot, program.PathFolder);
                    program.FileImage = PathRoot+ _install +"!ExampleFile\\Icon\\" + program.FileImage;
                }
                return temp;
            }
            catch (Exception exp)
            {
                _messageDialogService.ShowInfoDialog($"Шаблон для списку програм має помилку, для вирішення проблеми:\n\n" +
                    $"1. Виправіть помилку:\n{exp.Message}\n\n" +
                    $"2. Згенеруйте файл по замовчуванню в меню налаштувань програми!", "Error");
                return new List<GroupProgram>();
            }
        }
        public async Task<List<UserPresetProgram>> GetUserPresetProgram()
        {
            FileInfo fileSettingsJson = new FileInfo(SettingUserPresetProgramFileNamePath);

            if (!fileSettingsJson.Exists)
                await SetDefaultUserPresetProgram();
            
            try
            {
                return JsonConvert.DeserializeObject<List<UserPresetProgram>>(await
            File.ReadAllTextAsync(SettingUserPresetProgramFileNamePath),
            new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error, });
            }
            catch (Exception exp)
            {
                _messageDialogService.ShowInfoDialog($"Шаблон набору програм має помилку, для вирішення проблеми:\n\n" +
                    $"1. Виправіть помилку:\n{exp.Message}\n\n" +
                    $"2. Згенеруйте файл по замовчуванню в меню налаштувань програми!", "Error");
                return new List<UserPresetProgram>();
            }



        }
        public async Task SetDefaultGroupProgram()
        {
            var SettingsJsonExample = JsonConvert.SerializeObject(GetDefaultGroupProgram(), Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });
            await File.WriteAllTextAsync(SettingsGroupProgramFileNamePath, SettingsJsonExample).ConfigureAwait(false);
        }
        public async Task SetDefaultUserPresetProgram()
        {
            var SettingsJsonExample = JsonConvert.SerializeObject(GetDefaultUserPresetProgram(), Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });
            await File.WriteAllTextAsync(SettingUserPresetProgramFileNamePath, SettingsJsonExample).ConfigureAwait(false);
        }
        private List<GroupProgram> GetDefaultGroupProgram()
        {

            List<GroupProgram> groupPrograms = new List<GroupProgram>()
            {
                new GroupProgram()
                {
                    Id = 0,
                    Name = "Браузери",
                    Description = $"Браузер - це програмне забезпечення для комп'ютера або іншого електронного пристрою, як правило, під'єднаного до Інтернету,\n" +
                    $"що дає можливість користувачеві взаємодіяти з текстом, малюнками або іншою інформацією на гіпертекстовій вебсторінці.\n" +
                    $"Тексти та малюнки можуть містити посилання на інші вебсторінки, розташовані на тому ж вебсайті або на інших вебсайтах.\n" +
                    $"Вебпереглядач з допомогою гіперпосилань дозволяє користувачеві швидко та просто отримувати інформацію, розміщену на багатьох вебсторінках.",
                    SingleChoice = false,
                    UserPrograms = new List<UserProgram>
                    {
                        new UserProgram()
                        {
                            Id = 0,
                            ProgramName = "Google Chrome",
                            InstallProgramName = "Chrome",
                            Description =   $"Google Chrome - це безкоштовний веббраузер, розроблений компанією Google на основі браузера\n" +
                                            "з відкритим кодом Chromium та іншого відкритого програмного забезпечення.",
                            Arguments = new List<string>() {"/silent", "/install"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Browser\\",
                            FileName = "Chrome",
                            Architecture = "x64",
                            FileImage = "Chrome.png",

                            OnlineInstaller = new OnlineInstaller()
                            {
                                Arguments = new List<string>() { "/silent", "/install"},
                                FileName= "ChromeSetup"
                            }
                        },
                        new UserProgram()
                        {
                            Id = 1,
                            ProgramName = "Mozilla Firefox",
                            InstallProgramName = "Mozilla",
                            Description = "Mozilla Firefox - це вільний безкоштовний браузер з відкритим кодом, використовує ядро Quantum (вдосконалений Gecko).",
                            Arguments = new List<string>() { "/S"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Browser\\",
                            FileName = "Firefox",
                            Architecture = "x64",
                            FileImage = "Firefox.png",

                            OnlineInstaller = new OnlineInstaller()
                            {
                                Arguments = new List<string>() { "-ms"},
                                FileName= "FirefoxInstaller"
                            }
                        },
                        new UserProgram()
                        {
                            Id = 2,
                            ProgramName = "Opera",
                            InstallProgramName = "Opera",
                            Description = "Opera — веббраузер розробки норвезької компанії Opera Software. Вперше випущений у 1994 році групою дослідників з норвезької компанії Telenor.",
                            Arguments = new List<string>() { "/silent", "/allusers=1", "/launchbrowser=0", "/setdefaultbrowser=0"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Browser\\",
                            FileName = "opera",
                            Architecture = "x64",
                            FileImage = "Opera.png",

                            OnlineInstaller = new OnlineInstaller()
                            {
                                Arguments = new List<string>() { "/silent", "/allusers=1", "/launchbrowser=0", "/setdefaultbrowser=0"},
                                FileName= "OperaSetup"
                            }
                        }
                    }

                },
                new GroupProgram()
                {
                    Id = 1,
                    Name = "Аудіовізуальні медіа",
                    Description = $"Різноманітні програми для перегляду медіа файлів",
                    SingleChoice = false,
                    UserPrograms = new List<UserProgram>
                    {
                        new UserProgram()
                        {
                            Id = 3,
                            ProgramName = "Adobe Acrobat Reader",
                            InstallProgramName = "Adobe",
                            Description =   $"Adobe Acrobat Reader — це програмний продукт виробництва Adobe для роботи з PDF-файлами.",
                            Arguments = new List<string>() {"/sAll"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Media\\",
                            FileName = "Adobe",
                            Architecture = "x86",
                            FileImage = "Adobe.png",
                        },
                        new UserProgram()
                        {
                            Id = 4,
                            ProgramName = "FastStone",
                            InstallProgramName = "FastStone",
                            Description =   $"FastStone Image Viewer - це програма для перегляду, сканування, редагування та пакетної обробки зображень під виконанням операційної системи Microsoft Window.\n" +
                                            $"Вона включає вбудований ескізний файловий менеджер і базу даних, тому також може використовуватись як менеджер зображень.\n" +
                                            $"Вважається однією з найбільш функціональної безкоштовною програмою в своєму класі.",
                            Arguments = new List<string>() {"/S", "/I"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Media\\",
                            FileName = "FastStone",
                            Architecture = "x86",
                            FileImage = "FastStone.png",
                        }, 
                        new UserProgram()
                        {
                            Id = 5,
                            ProgramName = "K-Lite Codec Pack",
                            InstallProgramName = "K-Lite",
                            Description =   $"K-Lite Codec Pack - це колекція аудіо та відео кодеків для Microsoft Windows DirectShow,\n" +
                                            $"яка дозволяє операційній системі та її програмному забезпеченню відтворювати різні аудіо та відео формати,\n" +
                                            $"як правило, не підтримуються самою операційною системою.",
                            Arguments = new List<string>() {"/SP-", "/verysilent"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Media\\",
                            FileName = "K-Lite_Codec_Pack_1550_Mega",
                            Architecture = "x86",
                            FileImage = "K-Lite_Codec_Pack_1550_Mega.png",
                        },
                        new UserProgram()
                        {
                            Id = 6,
                            ProgramName = "Notepad++",
                            InstallProgramName = "Notepad++",
                            Description =   $"Notepad++ - це текстовий редактор, призначений для програмістів і тих, кого не влаштовує скромна функціональність програми «блокнот», що входить до складу Windows.\n" +
                                            $"Notepad++ базується на компоненті Scintilla (потужному компоненті для редагування), написаному на C++ з використанням тільки Windows API і STL,\n" +
                                            $"що забезпечує максимальну швидкість роботи при мінімальному розмірі програми.",
                            Arguments = new List<string>() {"/S"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Media\\",
                            FileName = "npp",
                            Architecture = "x64",
                            FileImage = "npp.png",
                        },                   
                        new UserProgram()
                        {
                            Id = 7,
                            ProgramName = "7-Zip",
                            InstallProgramName = "7-Zip",
                            Description =   $"7-Zip — файловий архіватор з високим ступенем стиснення.\n" +
                                            $"Велика частина вихідного коду є відкритою і поширюється за ліцензією GNU LGPL,\n" +
                                            $"код unRAR поширюється під змішаною ліцензією (GNU LGPL + обмеження unRAR).\n" +
                                            $"За умовами ліцензії 7-Zip можна використовувати безкоштовно на будь-якому комп'ютері,\n" +
                                            $"включаючи комп'ютери комерційних організацій, без необхідності реєстрації.",
                            Arguments = new List<string>() {"/S"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Media\\",
                            FileName = "7z",
                            Architecture = "x64",
                            FileImage = "7-Zip.png",
                        },
                        new UserProgram()
                        {
                            Id = 8,
                            ProgramName = "WinRAR 4.00",
                            InstallProgramName = "WinRAR",
                            Description =   $"WinRAR — це файловий архіватор для Windows з високим ступенем стиснення,\n" +
                                            $"є одним із найкращих архіваторів за співвідношенням ступеня стиснення до швидкості роботи.\n" +
                                            $"Розповсюджується як умовно-безкоштовне програмне забезпечення.",
                            Arguments = new List<string>() {"-y"},
                            IsFree = false,
                            DisableDefender = false,
                            PathFolder = _install + "Media\\",
                            FileName = "WinRAR40",
                            Architecture = "x86",
                            FileImage = "WinRAR.png",
                        },
                        new UserProgram()
                        {
                            Id = 9,
                            ProgramName = "WinRAR 5.40",
                            InstallProgramName = "WinRAR",
                            Description =   $"WinRAR — це файловий архіватор для Windows з високим ступенем стиснення,\n" +
                                            $"є одним із найкращих архіваторів за співвідношенням ступеня стиснення до швидкості роботи.\n" +
                                            $"Розповсюджується як умовно-безкоштовне програмне забезпечення.",
                            Arguments = new List<string>() {"/s"},
                            IsFree = false,
                            DisableDefender = false,
                            PathFolder = _install + "Media\\WinRar540\\",
                            FileName = "WinRAR540",
                            Architecture = "x64",
                            FileImage = "WinRAR.png",
                        },
                        new UserProgram()
                        {
                            Id =10,
                            ProgramName = "STDU Viewer",
                            InstallProgramName = "STDU",
                            Description =   $"STDU Viewer - це невеликий за розміром переглядач PDF, DjVu, Comic Book Archive (CBR або CBZ), XPS, TCR, TIFF, TXT, EMF, WMF, BMP, GIF, JPG, JPEG, PNG, PSD\n" +
                                            $"для Microsoft Windows безкоштовний для некомерційного використання.",
                            Arguments = new List<string>() {"/VERYSILENT"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Media\\",
                            FileName = "stduviewer",
                            Architecture = "x86",
                            FileImage = "stduviewer.png",
                        },
                        new UserProgram()
                        {
                            Id =11,
                            ProgramName = "AIMP",
                            InstallProgramName = "AIMP",
                            Description =   $"AIMP — безкоштовний аудіопрогравач з закритим початковим кодом, написаний на Delphi російським програмістом Артемом Ізмайловим.",
                            Arguments = new List<string>() {"/SILENT", "/AUTO"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Media\\",
                            FileName = "AIMP",
                            Architecture = "x86",
                            FileImage = "AIMP.ico",
                        },
                          new UserProgram()
                        {
                            Id = 12,
                            ProgramName = "ACDSeePro3",
                            InstallProgramName = "ACDSee",
                            Description =   $"ACDSeePro3 - умовно-безкоштовна програма для перегляду і організації зображень для Microsoft Windows, а так само для Mac OS X, що випускається ACD Systems.\n" +
                                            $"Містить численні інструменти для обробки зображень, у тому числі і пакетної. В останніх версіях має два режими перегляду:\n" +
                                            $"швидкий, в якому доступні тільки інструменти повороту зображення і зміна масштабу, і повний, з завантаженням всіх інструментів обробки.",
                            Arguments = new List<string>() {"/quiet"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Media\\",
                            FileName = "ACDSee",
                            Architecture = "x86",
                            FileImage = "ACDSeePro3.ico",
                        }
                    }
                },
                new GroupProgram()
                {
                    Id = 2,
                    Name = "Утиліти",
                    Description = $"Сервісні програми, що допомагають керувати файлами, отримувати інформацію про комп'ютер, діагностувати й усувати проблеми, забезпечувати ефективну роботу системи. Утиліти розширюють можливості ОС.",
                    SingleChoice = false,
                    UserPrograms = new List<UserProgram>
                    {
                        new UserProgram()
                        {
                            Id = 13,
                            ProgramName = "Unlocker",
                            InstallProgramName = "Unlocker",
                            Description =   $"Unlocker — це безкоштовна утиліта, яка дозволяє розблокувати файли, що використовуються системним процесом або перебувають у закритому доступі.",
                            Arguments = new List<string>() {"-y", "-fm0"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Tools\\",
                            FileName = "Unlocker",
                            Architecture = "x86",
                            FileImage = "Unlocker.png",
                        },
                        new UserProgram()
                        {
                            Id = 14,
                            ProgramName = "AIDA64",
                            InstallProgramName = "AIDA64",
                            Description =   $"AIDA64 — це утиліта FinalWire Ltd. для тестування та ідентифікації компонентів персонального комп'ютера під керуванням операційних систем Windows,\n" +
                                            "що надає детальні відомості про апаратне та програмне забезпечення.",
                            Arguments = new List<string>() {"/silent", "/IE"},
                            IsFree = false,
                            DisableDefender = false,
                            PathFolder = _install + "Tools\\AIDA\\",
                            FileName = "AIDA64",
                            Architecture = "x86",
                            FileImage = "AIDA64.png",
                        },
                        new UserProgram()
                        {
                            Id = 15,
                            ProgramName = "AnyDesk",
                            InstallProgramName = "AnyDesk",
                            Description =   $"AnyDesk — це програма віддаленого робочого столу із закритим кодом, що поширюється компанією AnyDesk Software GmbH\n" +
                                            $"Забезпечує незалежний від платформи віддалений доступ до персональних комп’ютерів та інших пристроїв, на яких запущена основна програма.",
                            Arguments = new List<string>() { "--install \"C:\\Program Files (x86)\\AnyDesk\"", "--start-with-win", "--create-shortcuts", "--create-desktop-icon", "--silent"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Tools\\",
                            FileName = "AnyDesk",
                            Architecture = "x86",
                            FileImage = "AnyDesk.png",
                        },
                        new UserProgram()
                        {
                            Id = 16,
                            ProgramName = "Total Commander 7.0",
                            InstallProgramName = "Total Commander",
                            Description =   $"Total Commander — популярний двопанельний файловий менеджер із закритим початковим кодом для операційних систем Windows, Windows CE, Windows Mobile і Android.",
                            Arguments = new List<string>() {"/S", "/IE"},
                            IsFree = false,
                            DisableDefender = true,
                            PathFolder = _install + "Tools\\",
                            FileName = "TotalCommander",
                            Architecture = "x86",
                            FileImage = "TotalCommander.png",
                        },
                        new UserProgram()
                        {
                            Id = 17,
                            ProgramName = "Total Commander 9.51",
                            InstallProgramName = "Total Commander",
                            Description =   $"Total Commander — популярний двопанельний файловий менеджер із закритим початковим кодом для операційних систем Windows, Windows CE, Windows Mobile і Android.",
                            Arguments = new List<string>() {"/S", "/IE"},
                            IsFree = false,
                            DisableDefender = true,
                            PathFolder = _install + "Tools\\",
                            FileName = "TotalCommander",
                            Architecture = "x86",
                            FileImage = "TotalCommander.png",
                        }
                    }
                },
                new GroupProgram()
                {
                    Id = 3,
                    Name = "Office",
                    Description =   $"Microsoft Office — офісний пакет, створений корпорацією Microsoft для операційних систем Windows, macOS, iOS та Android.\n" +
                                    $"До складу цього пакету входить програмне забезпечення для роботи з різними типами документів: текстами, електронними таблицями, презентаціями, базами даних тощо.\n" +
                                    $"Microsoft Office також є сервером OLE об'єктів і його функції можуть використовуватися іншими застосунками, а також самими застосунками Microsoft Office.\n" +
                                    $"Підтримує скрипти та макроси, написані на VBA.",
                    SingleChoice = true,
                    UserPrograms = new List<UserProgram>()
                    {
                        new UserProgram(){
                            Id = 18,
                            ProgramName = "Microsoft Office 2003",
                            Description =   $"Microsoft Office 2003 - пакет офісних додатків, який розповсюджується компанією Microsoft для операційних системи Windows.\n" +
                                            $"Випуск розпочався 19 серпня 2003, на ринок був випущений 21 жовтня 2003. Його попередником є ​​Microsoft Office XP, а наступником - Microsoft Office 2007.\n" +
                                            $"Це остання версія Microsoft Office, який використовував формат 97-2003, а також остання версія, в якій інтерфейс додатків був виконаний у вигляді панелей інструментів і меню.",
                            Arguments = new List<string>() {""},
                            IsFree = false,
                            DisableDefender = false,
                            PathFolder = _install + "Office\\Office 2003\\",
                            FileName = "SETUP",
                            Architecture = "x86",
                            FileImage = "Office2003.png",
                        },
                         new UserProgram(){
                            Id = 19,
                            ProgramName = "Microsoft Office 2007",
                            Description =   $"Microsoft Office 2007 - версія пакету додатків Microsoft Office, що послідувала за Microsoft Office 2003 і попередник Microsoft Office 2010.\n" +
                                            $"Надійшла у продаж для організацій 30 листопада 2006, для індивідуальних клієнтів — 30 січня 2007.\n" +
                                            $"Одночасно була випущена операційна система Windows Vista.",
                            Arguments = new List<string>() {""},
                            IsFree = false,
                            DisableDefender = false,
                            PathFolder = _install + "Office\\Office 2007\\",
                            FileName = "setup",
                            Architecture = "x86",
                            FileImage = "Office2007.png",
                        },
                          new UserProgram(){
                            Id = 20,
                            ProgramName = "Microsoft Office 2010",
                            Description =   $"Microsoft Office 2010 - версія додатків офісного пакету для операційної системи Microsoft Windows.\n" +
                                            $"Office 2010 є наступником Microsoft Office 2007 та попередником Microsoft Office 2013.\n" +
                                            $"Office 2010 включає в себе підтримку розширених форматів файлів, поліпшення інтерфейсу користувача, а також змінений користувальницький досвід.\n" +
                                            $"У 64-розрядній версії Office 2010 виключена підтримка для Windows XP та для Windows Server 2003.",
                            Arguments = new List<string>() {""},
                            IsFree = false,
                            DisableDefender = false,
                            PathFolder = _install + "Office\\Office 2010\\",
                            FileName = "Office_ProPlus_2010_RePack",
                            Architecture = "x86",
                            FileImage = "Office2010.png",
                        },
                           new UserProgram(){
                            Id = 21,
                            ProgramName = "Microsoft Office 2016",
                            InstallProgramName = "плюс 2016",
                            Description =   $"Microsoft Office 2016 — остання версія популярного офісного пакету компанії Microsoft, що розповсюджувався за допомогою MSI-інсталятора.\n" +
                                            $"Пакет був представлений широкій публіці наприкінці 2014 року.\n" +
                                            $"Офіційний реліз відбувся разом із новою операційною системою Windows 10 в один день, 29 липня 2015.",
                            Arguments = new List<string>() {""},
                            IsFree = false,
                            DisableDefender = false,
                            PathFolder = _install + "Office\\Office 2016\\",
                            FileName = "Office_ProPlus_2016_RePack",
                            Architecture = "x86",
                            FileImage = "Office2016.png",
                        }
                    }
                },
                new GroupProgram()
                {
                    Id = 4,
                    Name = "Антивірусне програмне забезпечення",
                    Description = $"Антивірус - це спеціалізована програма для знаходження комп'ютерних вірусів,\n" +
                    $"а також небажаних (шкідливих) програм загалом, та відновлення заражених (модифікованих) такими програмами файлів,\n" +
                    $"а також для профілактики — запобігання зараженню (модифікації) файлів чи операційної системи шкідливим кодом.",
                    SingleChoice = true,
                    UserPrograms = new List<UserProgram>
                    {
                        new UserProgram()
                        {
                            Id = 22,
                            ProgramName = "360 Total Security",
                            InstallProgramName = "360 Total Security",
                            Description = $"360 Total Security — це програма, розроблена китайською компанією Qihoo 360,\n" +
                            $" яка займається інтернет-безпекою.",
                            Arguments = new List<string>() {"/S"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Antivirus\\",
                            FileName = "360TS",
                            Architecture = "x86",
                            FileImage = "360TS.png",
                        },
                        new UserProgram()
                        {
                            Id = 23,
                            ProgramName = "Microsoft Security Essentials",
                            InstallProgramName = "Security Essentials",
                            Description = "Microsoft Security Essentials (MSE) — безкоштовний пакет антивірусних програм від компанії Microsoft.",
                            Arguments = new List<string>() {"/s", "/runwgacheck"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Antivirus\\",
                            FileName = "MSEInstall",
                            Architecture = "x64",
                            FileImage = "MSEInstall.png"
                        }
                    }
                },
                new GroupProgram()
                {
                    Id = 5,
                    Name = "Report CompasPack",
                    Description = "Звіти про встановлення",
                    SingleChoice = true,
                     UserPrograms = new List<UserProgram>()
                    {
                     new UserProgram(){
                            Id = 24,
                            ProgramName = "Report Greg_House_M_D",
                            Description =   $"Ставив Greg_House_M_D",
                            Arguments = new List<string>() { "\\LogInstall", "Name:Greg_House_M_D"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Tools\\",
                            FileName = "LogInstall",
                            Architecture = "x86",
                            FileImage = "LogInstall.png",
                        },
                     new UserProgram(){
                            Id = 25,
                            ProgramName = "Report Vadimakus",
                            Description =   $"Ставив Vadimakus",
                            Arguments = new List<string>() { "\\LogInstall", "Name:Vadimakus"},
                            IsFree = true,
                            DisableDefender = false,
                            PathFolder = _install + "Tools\\",
                            FileName = "LogInstall",
                            Architecture = "x86",
                            FileImage = "LogInstall.png",
                        }
                     },

                },
               

            };

            return groupPrograms;
        }  
        private List<UserPresetProgram> GetDefaultUserPresetProgram()
        {
            return new List<UserPresetProgram>()
            {
                new UserPresetProgram()
                {
                    Id = -1,
                    Name = "Nothing",
                    Description = "",
                    IdPrograms = new List<int>() {}
                },
                new UserPresetProgram()
                {
                    Id=0,
                    Name = "User Windows 10",
                    Description="Для Windows 10",
                    IdPrograms = new List<int>() {0, 1, 2, 3, 4, 5, 6, 7, 9, 13, 24 }
                },
                new UserPresetProgram()
                {
                    Id=1,
                    Name = "User Windows 7",
                    Description="Для Windows 7",
                    IdPrograms = new List<int>() {0, 1, 2, 3, 4, 5, 6, 7, 8, 13, 23, 24 }
                }
            };
        }
        public void OpenFolder(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(CompasExampleFile);
            Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", path);
        }
        public void OpenFolderAndSelectFile(string path)
        {
            if (File.Exists(path))
            {
                string argument = "/select, \"" + path + "\"";
                Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", argument);
            }
        }
        public async Task WriteAllTextAsync(string path, string text)
        {
            await File.WriteAllTextAsync(path, text).ConfigureAwait(false);
        }


        public async Task SetSettingsReport()
        {
            var SettingsJsonExample = JsonConvert.SerializeObject(SettingsReportHelper.GetSettingsReport(), Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });
            await File.WriteAllTextAsync(SettingsUserReportPath, SettingsJsonExample).ConfigureAwait(false);
        }
        public async Task<SettingsReportViewModel> GetSettingsReport()
        {
            FileInfo fileSettingsJson = new FileInfo(SettingsUserReportPath);
            //if (!fileSettingsJson.Exists)
                await SetSettingsReport();

            try
            {
                var temp = JsonConvert.DeserializeObject<SettingsReportViewModel>(await
                File.ReadAllTextAsync(SettingsUserReportPath),
                new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error, });

                return temp;
            }
            catch (Exception exp)
            {
                _messageDialogService.ShowInfoDialog($"Шаблон для списку програм має помилку, для вирішення проблеми:\n\n" +
                    $"1. Виправіть помилку:\n{exp.Message}\n\n" +
                    $"2. Згенеруйте файл по замовчуванню в меню налаштувань програми!", "Error");
                return SettingsReportHelper.GetSettingsReport();
            }
        }
        public async Task<XDocument> GetXDocument()
        {

            if (!Directory.Exists(CompasPackLog))
                Directory.CreateDirectory(CompasPackLog);
            ProcessStartInfo? StartInfo = new ProcessStartInfo
            {
                FileName = Aida,
                Arguments = "/R " + CompasPackLog + "\\Report. " + "/XML " + "/CUSTOM " + Path.GetDirectoryName(Aida) + "\\ForReport.rpf",
                UseShellExecute = false
            };
            try
            {
                if (!File.Exists(CompasPackLog + "\\Report.xml"))
                {
                    Process proc = Process.Start(StartInfo);
                    await proc.WaitForExitAsync();
                }
            }
            catch (Exception) { }

            if (!File.Exists(CompasPackLog + "\\Report.xml"))
                return null;
            //-------------------------------------------------------------------------------------------------------------------
            XDocument? document;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = new StreamReader(CompasPackLog + "\\Report.xml", Encoding.GetEncoding("windows-1251")))
            {
                document = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, new System.Threading.CancellationToken());
            }
            return document;
            //-------------------------------------------------------------------------------------------------------------------
        }

        public void CheckReportFolders()
        {
            if (!Directory.Exists(Report))
                Directory.CreateDirectory(Report);
            if (!Directory.Exists(ReportPC))
                Directory.CreateDirectory(ReportPC);
            if (!Directory.Exists(ReportLaptop))
                Directory.CreateDirectory(ReportLaptop);
            if (!Directory.Exists(ReportMonitor))
                Directory.CreateDirectory(ReportMonitor);
        }
        
        public int GetLastReport(string paht)
        {
            var lastString = Directory.GetFiles(paht).Select(x => x = Regex.Match(x, "\\d+").Value).OrderBy(x => x).LastOrDefault();
            if (lastString != null)
            {
                if (int.TryParse(lastString, out int last))
                    return last;     
                else
                    return -1;
            }
            else
                return -1;
        }
    }
}
