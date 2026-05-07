using CompasPack.Data.Providers;
using CompasPack.Data.Providers.API;
using CompasPack.Helper.Extension;
using CompasPack.Model.Entities.Programs;
using CompasPack.Model.Enum;
using CompasPack.Model.Support;
using CompasPack.Model.Wrapper;
using CompasPack.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public class ProgramsService : IProgramsService
    {
        private readonly IMessageDialogService _messageDialogService;
        private readonly WinInfo _winInfo;
        private readonly IEnumerable<IAntivirus> _antiviruses;
        private readonly IFileSystemReaderWriter _fileSystemReaderWriter;
        private readonly IFileArchiver _fileArchiver;
        private readonly ProgramsSettingsProvider _programsSettingsProvider;
        public IConsoleBuffer ConsoleBuffer { get; private set; }
        public ProgramsService(WinInfo winInfo, IEnumerable<IAntivirus> antiviruses,
                               IConsoleBuffer consoleBuffer, IMessageDialogService messageDialogService,
                               IFileSystemReaderWriter fileSystemReaderWriter, IFileArchiver fileArchiver,
                               ProgramsSettingsProvider programsSettingsProvider)
        {
            _winInfo = winInfo;
            _antiviruses = antiviruses;

            ConsoleBuffer = consoleBuffer;
            _messageDialogService = messageDialogService;

            _fileSystemReaderWriter = fileSystemReaderWriter;
            _fileArchiver = fileArchiver;

            _programsSettingsProvider = programsSettingsProvider;
        }

        public void CombinePath(IList<GroupProgramsWrapper> groupPrograms, ProgramsPaths programsPaths)
        {
            if (programsPaths.PathFolderPrograms != null && programsPaths.PathFolderImageProgram != null)
            {
                foreach (var item in groupPrograms.SelectMany(group => group.ProgramWrappers))
                {
                    item.Program.PathFolder = Path.Combine(programsPaths.PathFolderPrograms, item.Program.PathFolder);
                    item.Program.FileImage = Path.Combine(programsPaths.PathFolderImageProgram, item.Program.FileImage);
                }
            }
        }
        public void CheckInstallPrograms(IList<GroupProgramsWrapper> groupPrograms, WinArchitectureEnum winArchitecture)
        {
            var tempListPrograms = SoftwareInfoProvider.GetInstallPrograms(winArchitecture);
            foreach (var program in groupPrograms.SelectMany(group => group.ProgramWrappers))
                program.CheckInstall(tempListPrograms);
        }

        public async Task<double> SpeedTest()
        {
            ConsoleBuffer.WriteLine($"Speed test: ");
            var speed = await NetworkService.SpeedTest(_programsSettingsProvider.Settings.SpeedTestURL);
            ConsoleBuffer.WriteLine($"{Math.Round(speed, 2)} Mbit/s\n");
            return speed;
        }
        public async Task OnAntiviruses(IList<IAntivirus> antiviruses)
        {
            foreach (var antivirus in antiviruses)
            {
                ConsoleBuffer.WriteLine($"{antivirus.AntivirusInfo.DisplayName}: ");
                await antivirus.EnableRealTimeMonitoring();
                var realtimeMonitoring = await antivirus.GetRealTimeMonitoringStatus();
                ConsoleBuffer.WriteLine($"{realtimeMonitoring} ");

                if (realtimeMonitoring == AntivirusStatusEnum.Enabled)
                    ConsoleBuffer.WriteLine($"(OK!)\n");
                else
                    ConsoleBuffer.WriteLine($"(Fail!)\n");
            }
        }
        public async Task OffAntiviruses(IList<IAntivirus> antiviruses)
        {
            foreach (var antivirus in antiviruses)
            {
                ConsoleBuffer.WriteLine($"{antivirus.AntivirusInfo.DisplayName}: ");
                await antivirus.DisableRealTimeMonitoring();
                var realtimeMonitoring = await antivirus.GetRealTimeMonitoringStatus();
                ConsoleBuffer.WriteLine($"{realtimeMonitoring} ");

                if (realtimeMonitoring == AntivirusStatusEnum.Disabled)
                    ConsoleBuffer.WriteLine($"(OK!)\n");
                else
                    ConsoleBuffer.WriteLine($"(Fail!)\n");
            }
        }

        public async Task OpenProtectedProgram(ProtectedProgram protectedProgram)
        {
            if (!await IsAllAntivirusManual(new[] { protectedProgram }.Select(p => p.Name).ToList(), UserActionEnum.Open))
                return;

            ConsoleBuffer.AddSplitter();
            ConsoleBuffer.WriteLine($"Start open {protectedProgram.Name}\n");
            int countOpen = 0;
            do // цикл запуску protectedProgram (Дві спроби)
            {
                int countDecompress = 0;
                string ExecutableFile = null;
                do // цикл пошуку файла (цікаво зроблено, він виконується або 0.5 або на 1.5 рази)
                {

                    var activeAntiviruses = (await _antiviruses.WhereAsync(async av => await av.GetRealTimeMonitoringStatus() != AntivirusStatusEnum.Disabled)).ToList();

                    if (activeAntiviruses.Any()) // якщо антивірус(и) увімк-нутий (нені)
                    {
                        await OffAntiviruses(activeAntiviruses); // вимикаємо
                        await Task.Delay(500); // зачекаємо вимкнення
                    }
                    //bool isStillActive = (await Task.WhenAll(activeAntiviruses.Select(async av => await av.GetRealTimeMonitoringStatus() != AntivirusStatusEnum.Disabled)))
                    //                                                          .Any(isActive => isActive);
                    bool isStillActive = (await activeAntiviruses.WhereAsync(async av => await av.GetRealTimeMonitoringStatus() != AntivirusStatusEnum.Disabled)).Any();

                    if (isStillActive) // якщо антивірусник досі увімкнутий
                    {
                        ConsoleBuffer.WriteLine("Error: defender is not disabled\n"); // сповіщаємо про помилку
                        break; // виходимо з циклу (далі буде помилка в циклі встановлення)
                    }

                    var executable = _fileSystemReaderWriter.GetListFile(protectedProgram.ProtectedProgramPaths.PathFolder)
                                .Where(x => x.Contains(protectedProgram.FileName, StringComparison.InvariantCultureIgnoreCase) && x.Contains("exe", StringComparison.InvariantCultureIgnoreCase))
                                .FirstOrDefault();// шукаємо архів
                    if (executable != null) // якщо protectedProgram є на свому місці
                    {
                        ExecutableFile = executable; // назначаємо його
                        break; // покидаємо цикл пошуку файлу (внутрішній) (далі буде спроба запустити protectedProgram) 
                    }
                    else // якщо файла нема
                    {
                        ConsoleBuffer.WriteLine($"Not found {protectedProgram.Name}\n"); // сповіщаємо користувача, що файлу нема
                        if (countDecompress >= 1) // якщо ми вже спробували його розпакувати еле його досі нема
                            break; // то покидаємо цикл пошуку файлу(далі буде помилка в циклі встановлення)
                        if (!File.Exists("7za.exe")) // перевіряємо чи на місці архіватор 
                        {
                            ConsoleBuffer.WriteLine($"Error, Not 7za.exe\n"); // якщо його нема то сповіщаємо користувача
                            break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі запуску protectedProgram)
                        }
                        ConsoleBuffer.WriteLine($"Trying find archive, resault: "); // сповіщаємо користувача про спробу знайти архів
                        var pathArchiv = _fileSystemReaderWriter.GetListFile(protectedProgram.ProtectedProgramPaths.PathFolder)
                               .Where(x => x.Contains(protectedProgram.FileName, StringComparison.InvariantCultureIgnoreCase) && x.Contains("7z", StringComparison.InvariantCultureIgnoreCase))
                               .FirstOrDefault();
                        if (pathArchiv == null) // перевіряємо чи знайдено архів
                        {
                            ConsoleBuffer.WriteLine($"Error, not Find arkhive!\n"); // якщо його нема то сповіщаємо користувача
                            break; // зупиняємо спробу розпакувати архівів виходимо з циклу пошуку файлу (далі буде помилка в циклі запуска protectedProgram)
                        }
                        ConsoleBuffer.WriteLine($"OK!\n");
                        ConsoleBuffer.WriteLine($"Start decompress, resault: ");
                        await Task.Delay(500); // дамо часу основному потоку обновити інформацію в консолі
                        switch (_fileArchiver.Decompress(pathArchiv, Path.GetDirectoryName(pathArchiv), _programsSettingsProvider.Settings.ArchivePassword, 20000)) //спробу розпакувати архів
                        {
                            case ResultArchiverEnum.OK:
                                ConsoleBuffer.WriteLine($"OK!\n");
                                break;
                            case ResultArchiverEnum.TimeOut:
                                ConsoleBuffer.WriteLine("Time out decompress\n");
                                break;
                            case ResultArchiverEnum.Error:
                                ConsoleBuffer.WriteLine("Error decompress\n");
                                break;
                        }
                        countDecompress++; // лічильник спроб розпакувати архів
                    }
                } while (true);
                if (ExecutableFile == null) // якщо protectedProgram так і не знайдено то
                {
                    ConsoleBuffer.WriteLine($"{protectedProgram.Name} is not open!\n"); //сповіщаємо користувача про це (далі ще раз спробуємо знайти файл якщо не вичерпано ліміт спроб)
                }
                else // якщо protectedProgram знайдено
                {
                    var StartInfo = new ProcessStartInfo // ProcessStartInfo
                    {
                        FileName = ExecutableFile,
                        UseShellExecute = false,
                    };
                    try // спроба запустити
                    {
                        Process proc = Process.Start(StartInfo);
                        ConsoleBuffer.WriteLine($"{protectedProgram.Name} Opened!\n");
                        await Task.Run(() => proc.WaitForExit()); // очікуємо завершення роботи програми
                        ConsoleBuffer.WriteLine($"{protectedProgram.Name} Closed!\n");
                        break; // покидаємо цикл запуску програми
                    }
                    catch (Exception exp)
                    {
                        ConsoleBuffer.WriteLine($"{protectedProgram.Name} error open: {exp.Message}\n"); // якщо сталась помилка сповіщаємо про це
                        if (countOpen + 1 < 2)
                        {
                            ConsoleBuffer.WriteLine($"Pause 20 seconds...\n");
                            await Task.Delay(20000); // пауза для того щоб антивірусник "відпустив" файл
                        }
                    }
                }
                countOpen++; // збільшуємо лічильник спроб запуску
                if (countOpen >= 2) // перевіряємо кількість спроб запуску
                    break; // виходимо якщо їх було дві
                ConsoleBuffer.WriteLine($"<***************************************************************************>\n");
            } while (true);
            await OnAntiviruses((await _antiviruses.WhereAsync(async av => av.IsControlled && await av.GetTamperProtectionStatus() == AntivirusStatusEnum.Disabled)).ToList());
            ConsoleBuffer.AddSplitter();
        }
        public async Task InstallPrograms(IList<ProgramWrapper> selectedPrograms)
        {
            var antiVirusSensitivePrograms = selectedPrograms.Where(p => p.Program.DisableDefender && !p.IsInstall).ToList();

            if (antiVirusSensitivePrograms.Any()) // якщо є програми встановлення яких чутливе до антивірусів
                if (!await IsAllAntivirusManual(antiVirusSensitivePrograms.Select(x => x.Program.ProgramName).ToList(), UserActionEnum.Install)) // якщо є антивіруси якими не можна керувати
                    CancelProblematicPrograms(ref selectedPrograms, antiVirusSensitivePrograms); // вимикаємо програми встановлення яких чутливе до антивірусів 


            if (selectedPrograms.Count() != 0)
                ConsoleBuffer.AddSplitter();
            foreach (var programToInstall in selectedPrograms)
            {
                ConsoleBuffer.WriteLine($"{programToInstall.Program.ProgramName}: Start Of Installation!\n");
                if (programToInstall.IsInstall) // перевіряємо чи програма встановлена
                {
                    ConsoleBuffer.Text = ConsoleBuffer.Text.ReplaceLastOccurrence($"{programToInstall.Program.ProgramName}: Start Of Installation!",
                                                      $"{programToInstall.Program.ProgramName}: Already installed!");
                    ConsoleBuffer.AddSplitter();
                    await Task.Delay(500);
                    continue;
                }
                await InstallProgram(programToInstall); // встановлюємо офлайн
                ConsoleBuffer.AddSplitter();
            }
            if (selectedPrograms.Where(p => p.Program.DisableDefender && !p.IsInstall).Any()) ////якщо треба було вимкнути антивірусники то вмикаємо
                await OnAntiviruses((await _antiviruses.WhereAsync(async av => av.IsControlled && await av.GetTamperProtectionStatus() == AntivirusStatusEnum.Disabled)).ToList());
            ConsoleBuffer.AddSplitter();
        }
        private async Task InstallProgram(ProgramWrapper programViewMode)
        {
            var program = programViewMode.Program;

            string arguments = null;
            int countInstall = 0; // лічильник спроб встановити програму (всього дві спроби)
            do // цикл встановлення програми (Дві спроби)
            {
                int countDecompress = 0; // лічильник спроб розпакувати архів якщо інсталятор схавав антивірусник(одна спроба розпакувати на одну спробу встановити програму)
                string ExecutableFile = null;
                do // цикл пошуку файла (цікаво зроблено, він виконується або 0.5 або на 1.5 рази завдяки if (ExecutableFile != null)  break; та if (countDecompress >= 1)  break;)
                {
                    if (programViewMode.Program.DisableDefender) // якщо треба вимкнути антивірусник
                    {
                        var activeAntiviruses = (await _antiviruses.WhereAsync(async av => av.IsControlled && await av.GetRealTimeMonitoringStatus() != AntivirusStatusEnum.Disabled)).ToList();
                        if (activeAntiviruses.Any()) // якщо антивірус(и) увімк-нутий (нені)
                        {
                            await OffAntiviruses(activeAntiviruses); // вимикаємо
                            await Task.Delay(500); // зачекаємо вимкнення
                        }
                        //bool isStillActive = (await Task.WhenAll(activeAntiviruses.Select(async av => await av.GetRealTimeMonitoringStatus() != AntivirusStatusEnum.Disabled)))
                        //                                                          .Any(isActive => isActive);
                        bool isStillActive = (await activeAntiviruses.WhereAsync(async av => await av.GetRealTimeMonitoringStatus() != AntivirusStatusEnum.Disabled)).Any();

                        if (isStillActive) // якщо антивірусник досі увімкнутий
                        {
                            ConsoleBuffer.WriteLine("Error: defender is not disabled\n"); // сповіщаємо про помилку
                            break; // виходимо з циклу (далі буде помилка в циклі встановлення)
                        }
                    }
                    if (program.OnlineInstaller != null) // якщо є онлайн інсталятор намагаємось його знайти і задаємо аргумент онлайн інсталятора
                    {
                        if (await SpeedTest() >= 5)
                        {
                            ExecutableFile = GetExeMsiFile(program.OnlineInstaller.FileName, program.PathFolder).FirstOrDefault();
                            arguments = string.Join(" ", program.OnlineInstaller.Arguments);
                        }
                    }
                    if (ExecutableFile == null) // якщо онлайн інсталятор не знайдено тоді шукаємо офлайн і задаємо аргументи офлайн інсталятора
                    {
                        var tempExecutableFile = GetExeMsiFile(program.FileName, program.PathFolder); // ортимуємо список файлів
                        if (_winInfo.WinArchitecture == WinArchitectureEnum.x64) // якщо наша система х64
                            ExecutableFile = tempExecutableFile.FirstOrDefault(x => x.Contains("x64", StringComparison.InvariantCultureIgnoreCase)); // то намагаємось знайти файл, що містить х64 в назві
                        if (ExecutableFile == null) //якщо система не х64 або файла х64 нема
                            ExecutableFile = tempExecutableFile.LastOrDefault();  // обираємо те що є
                        arguments = string.Join(" ", program.Arguments);
                    }
                    if (ExecutableFile != null) // якщо інсталятор знайдено то 
                        break; // покидаємо цикл пошуку файлу (внутрішній) (далі буде спроба встановити програму) 
                    else // якщо онлайн і офлайн інсталятор не знайдено
                    {
                        ConsoleBuffer.WriteLine($"Not found install file in folder: {program.PathFolder}\n"); // сповіщаємо користувача, що файлу нема
                        if (countDecompress >= 1) // якщо ми вже спробували його розпакувати еле його досі нема
                            break; // то покидаємо цикл пошуку файлу (далі буде помилка в циклі встановлення)
                        if (program.DisableDefender) // якщо треба вимикати антивірусник і файла нема то намагаємось його добути з архіва
                        {
                            if (!File.Exists("7za.exe")) // перевіряємо чи на місці архіватор 
                            {
                                ConsoleBuffer.WriteLine($"Error, Not 7za.exe\n"); // якщо його нема то сповіщаємо користувача
                                break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі встановлення)
                            }
                            ConsoleBuffer.WriteLine($"Trying find archive, resault: "); // сповіщаємо користувача про спробу знайти архів
                            var pathArchiv = _fileSystemReaderWriter.GetListFile(program.PathFolder)
                                .Where(x => x.Contains(program.FileName, StringComparison.InvariantCultureIgnoreCase) && x.Contains("7z", StringComparison.InvariantCultureIgnoreCase))
                                .FirstOrDefault();// шукаємо архів
                            if (string.IsNullOrWhiteSpace(pathArchiv)) // перевіряємо чи знайдено архів
                            {
                                ConsoleBuffer.WriteLine($"Error, Not Find arkhive!\n"); // якщо його нема то сповіщаємо користувача
                                break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі встановлення)
                            }
                            ConsoleBuffer.WriteLine($"OK!\n"); // сповіщаємо користувача, що архів знайдено
                            ConsoleBuffer.WriteLine($"Start decompress, resault: ");
                            await Task.Delay(500); // дамо часу основному потоку обновити інформацію в консолі
                            switch (_fileArchiver.Decompress(pathArchiv, program.PathFolder, _programsSettingsProvider.Settings.ArchivePassword, 20000)) //спробу розпакувати архів
                            {
                                case ResultArchiverEnum.OK:
                                    ConsoleBuffer.WriteLine($"OK!\n");
                                    break;
                                case ResultArchiverEnum.TimeOut:
                                    ConsoleBuffer.WriteLine("Time out decompress\n");
                                    break;
                                case ResultArchiverEnum.Error:
                                    ConsoleBuffer.WriteLine("Error decompress\n");
                                    break;
                            }
                            countDecompress++; // лічильник спроб розпакувати архів
                        }
                        else // якщо антивірусник не треба вимикати і файла немає то значить помилився десь користувач
                            break; //тому покидаємо цикл пошуку файлу(далі буде помилка в циклі встановлення)
                    }
                } while (true);

                if (ExecutableFile == null) // якщо файл так і не знайдено 
                {
                    ConsoleBuffer.Text = ConsoleBuffer.Text.ReplaceLastOccurrence($"{programViewMode.Program.ProgramName}: Start Of Installation!",
                                                      $"{programViewMode.Program.ProgramName}: Not Installed!");
                    break; // і виходимо з циклу встановлення програми
                }
                else // якщо файл знайдено
                {
                    ProcessStartInfo StartInfo = null;
                    if (ExecutableFile.EndsWith(".msi")) // перевіряємо чи це Msi чи Exe і створюємо відповідні ProcessStartInfo
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "msiexec",
                            Arguments = $"/i {ExecutableFile} {arguments}",
                            UseShellExecute = false
                        };
                    }
                    else
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = ExecutableFile,
                            Arguments = arguments,
                            UseShellExecute = false,
                            WorkingDirectory = program.PathFolder
                        };
                    }
                    ConsoleBuffer.WriteLine($"Command: {StartInfo.FileName} {StartInfo.Arguments}\n");
                    try // намагаємось встановити програму і очікуємо завершення її встановлення
                    {
                        //Process proc = Process.Start(StartInfo);
                        //await Task.Factory.StartNew(() => proc.WaitForExit());
                        await Task.Delay(1000); // пауза для CheckInstall (щоб встиг оновитись реєстр)
                        programViewMode.CheckInstall(SoftwareInfoProvider.GetInstallPrograms(_winInfo.WinArchitecture)); // CheckInstall
                        ConsoleBuffer.Text = ConsoleBuffer.Text.ReplaceLastOccurrence($"{programViewMode.Program.ProgramName}: Start Of Installation!",
                                  $"{programViewMode.Program.ProgramName}: Installed!");
                        break; // покидаємо цикл встановлення програми
                    }
                    catch (Exception exp)
                    {
                        ConsoleBuffer.Text = ConsoleBuffer.Text.ReplaceLastOccurrence($"{programViewMode.Program.ProgramName}: Start Of Installation!",
                                                                        $"{programViewMode.Program.ProgramName}: Error Install!");
                        ConsoleBuffer.WriteLine($"Error Message: {exp.Message}\n");
                        if (!program.DisableDefender) // якщо це програма яка не потребує вимкнення антивірусника і її встановелння викликало помилку тоді не пробуємо ставити її ще раз
                            break; // покидаємо цикл встановлення програми
                    }

                    countInstall++; // лічильник спроб встановити програму їх як сказано на початку буде лише дві
                    if (countInstall >= 2)
                        break;
                    ConsoleBuffer.WriteLine($"<***************************************************************************>\n");
                }
            } while (true);
        }


        private enum UserActionEnum
        {
            Open,
            Install,
        }
        private enum ReasonAntivirusConflictEnum
        {
            unManual,
            tamperProtection
        }
        private void CancelProblematicPrograms(ref IList<ProgramWrapper> selected, List<ProgramWrapper> problematic)
        {
            problematic.ForEach(p => p.Install = false);
            selected = selected.Except(problematic).ToList();
        }
        private void ShowAntivirusConflictDialog(List<string> prograsmNames, List<string> antivirusesNames, UserActionEnum userAction, ReasonAntivirusConflictEnum reasonAntivirusConflict)
        {
            if (prograsmNames == null || !prograsmNames.Any() || antivirusesNames == null || !antivirusesNames.Any())
                return;

            // 1. Блок програми
            bool isSinglePrograsm = prograsmNames.Count == 1;
            string programPart = string.Empty;
            switch (userAction)
            {
                case UserActionEnum.Open:
                    programPart = $"Запуск {prograsmNames.First()} неможливий!";
                    break;
                case UserActionEnum.Install:
                    programPart = isSinglePrograsm
                        ? $"Встановлення програми {prograsmNames.First()} неможливе!"
                        : $"Встановлення наступних програм неможливе:\n{string.Join(";\n", prograsmNames)}.";
                    break;
            }
            // 2. Блок антивірусу
            bool isSingleAntivirus = antivirusesNames.Count == 1;
            string antivirusNamesPart = isSingleAntivirus
                ? $"антивірус {antivirusesNames.First()}"
                : "наступні антивіруси";

            // 3. Блок причини
            string reasonPart;
            if (reasonAntivirusConflict == ReasonAntivirusConflictEnum.unManual)
            {
                reasonPart = isSingleAntivirus
                    ? "не керується програмно!"
                    : $"не керуються програмно:\n{string.Join(";\n", antivirusesNames)}.";
            }
            else
            {
                reasonPart = isSingleAntivirus
                    ? "захищений від змін!"
                    : $"захищені від змін:\n{string.Join(";\n", antivirusesNames)}.";
            }

            // 4. Фінальна збірка
            var message = $"{programPart}\n\nОскільки {antivirusNamesPart} {reasonPart}";

            if (userAction == UserActionEnum.Install)
                message += isSinglePrograsm ? "\n\nВстановлення цієї програми буде пропущено!" : "\n\nВстановлення цих програм буде пропущено!";

            _messageDialogService.ShowInfoDialog(message, "Увага");
        }
        private async Task<bool> IsAllAntivirusManual(List<string> programs, UserActionEnum userAction)
        {
            var unManualAntiviruses = _antiviruses.Where(av => !av.IsControlled).ToList();

            if (unManualAntiviruses.Any())
            {
                ShowAntivirusConflictDialog(programs, unManualAntiviruses.Select(x => x.AntivirusInfo.DisplayName).ToList(), userAction, ReasonAntivirusConflictEnum.unManual);
                return false;
            }
            else
            {
                while (true)
                {
                    var tamperProtectionAntiviruses = (await _antiviruses.WhereAsync(async av => await av.GetTamperProtectionStatus() != AntivirusStatusEnum.Disabled)).ToList();
                    if (!tamperProtectionAntiviruses.Any())
                        return true;

                    var result = _messageDialogService.ShowYesNoDialog(
                        $"Для встановлення деяких програм потрібно вимкнути 'Захист від підробки' (Tamper Protection) у налаштуваннях {(tamperProtectionAntiviruses.Count > 1 ? "антивірусів" : "антивірусу")}.\n" +
                        "Відкрити налаштування зараз?", "Необхідна дія");

                    if (result == MessageDialogResultEnum.Yes)
                    {
                        tamperProtectionAntiviruses.ForEach(x => x.OpenSettings());
                        _messageDialogService.ShowInfoDialog("Натисніть 'ОК' ПІСЛЯ того, як вимкнете захист у налаштуваннях.", "Очікування");
                    }
                    else
                    {
                        ShowAntivirusConflictDialog(programs, tamperProtectionAntiviruses.Select(x => x.AntivirusInfo.DisplayName).ToList(), userAction, ReasonAntivirusConflictEnum.tamperProtection);
                        return false;
                    }
                }
            }
        }
        private string[] GetExeMsiFile(string fileName, string folderPath)
        {
            return _fileSystemReaderWriter.GetListFile(folderPath).Where(x => x.Contains(fileName, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => x.Contains("exe", StringComparison.InvariantCultureIgnoreCase) || x.Contains("msi", StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }
    }
}
