using CompasPack.Event;
using CompasPack.View.Service;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CompasPack.Helper;
using System.IO;
using System.Diagnostics;
using CompasPack.Service;
using CompasPack.Settings;
using System.Text.RegularExpressions;
using CompasPack.Settings.Programs;

namespace CompasPack.ViewModel
{
    public interface ITextConsole
    {
        string TextConsole { get; set; }
    }
    public class ProgramsViewModel : ViewModelBase, IDetailViewModel, ITextConsole
    {
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private readonly ProgramsSettingsHelper _programsSettingsHelper;
        private ProgramsPaths _programsPaths;
        private readonly IIOHelper _iOHelper;
        private string _selectedProgramsSets;
        private string _textConsole;
        private bool _isEnabled;
        public ObservableCollection<ProgramsSet> ProgramsSets { get; }
        public ObservableCollection<GroupProgramViewModel> GroupProgramViewModel { get; }
        public ObservableCollection<ProtectedProgram> ProtectedPrograms { get; }
        public string TextConsole
        {
            get { return _textConsole; }
            set
            {
                _textConsole = value;
                OnPropertyChanged();
            }
        }
        public string SelectedProgramsSet
        {
            get { return _selectedProgramsSets; }
            set
            {
                _selectedProgramsSets = value;
                OnPropertyChanged();
            }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }
        public ProgramsViewModel(IMessageDialogService messageDialogService, IIOHelper iOHelper, IEventAggregator eventAggregator,
            ProgramsSettingsHelper ProgramsSettingsHelper)
        {
            ProgramsSets = new ObservableCollection<ProgramsSet>();
            GroupProgramViewModel = new ObservableCollection<GroupProgramViewModel>();
            ProtectedPrograms = new ObservableCollection<ProtectedProgram>();

            _messageDialogService = messageDialogService;
            _eventAggregator = eventAggregator;
            _programsSettingsHelper = ProgramsSettingsHelper;
            _iOHelper = iOHelper;
            IsEnabled = true;


            SelectProgramsSetCommand = new DelegateCommand(OnSelectProgramsSet);
            InstallCommand = new DelegateCommand(OnInstall);
            OnlyFreeCommand = new DelegateCommand(OnOnlyFree);
            ClearConsoleCommand = new DelegateCommand(OnClearConsole);

            AUCCommand = new DelegateCommand(OnAUC);
            IconCommand = new DelegateCommand(OnIcon);
            OpenDesktopIconSettingsCommand = new DelegateCommand(OnOpenDesktopIconSettings);

            OpenAppLogCommand = new DelegateCommand(OnOpenAppLog);
            OpenExampleFileCommand = new DelegateCommand(OnOpenExampleFile);
            OpenProtectedProgramCommand = new DelegateCommand<ProtectedProgram>(OpenProtectedProgram);
            DefaultCommand = new DelegateCommand(OnDefault);

            SpeedTestCommand = new DelegateCommand(OnSpeedTest);
            OffDefenderCommand = new DelegateCommand(OnOffDefender);
            OnDefenderCommand = new DelegateCommand(OnOnDefender);

            _eventAggregator.GetEvent<SelectSingleProgramEvent>().Subscribe(SelectSingleProgram);
        }
        //*****************************************************************************************
        public Task LoadAsync(int? Id)
        {
            TextConsole = WinInfoHelper.GetSystemInfo();

            ProgramsSets.Clear();
            GroupProgramViewModel.Clear();
            ProtectedPrograms.Clear();
            
            _programsPaths = (ProgramsPaths)_programsSettingsHelper.Settings.ProgramsPaths.Clone();
            PathHelper.SetRootPath(_iOHelper.PathRoot, _programsPaths);
            //----------------------------------------------------------------------------------------------------
            var GroupsPrograms = (List<GroupPrograms>)_programsSettingsHelper.Settings.GroupsPrograms?.Clone();
            foreach (var groupProgram in GroupsPrograms)
                GroupProgramViewModel.Add(new GroupProgramViewModel(groupProgram, new ObservableCollection<ProgramViewModel>(groupProgram.Programs.Select(x => new ProgramViewModel(x, groupProgram, _eventAggregator)))));

            ProgramsHelper.CombinePathFolderAndImage(GroupProgramViewModel, _programsPaths);
            ProgramsHelper.CheckInstallPrograms(GroupProgramViewModel); // CheckInstall
            //----------------------------------------------------------------------------------------------------
            _programsSettingsHelper.Settings.ProgramsSets.ForEach(x => ProgramsSets.Add(x)); // Add ProgramsSets     
            var tempProgramsSet = ProgramsSets.FirstOrDefault(x => x.Name.Contains(Regex.Match(WinInfoHelper.ProductName, @"\d+").Value, StringComparison.InvariantCultureIgnoreCase)); // check ProgramsSet
            if (tempProgramsSet != null)
                SelectedProgramsSet = tempProgramsSet.Name;
            OnSelectProgramsSet();
            //----------------------------------------------------------------------------------------------------
            ((List<ProtectedProgram>)_programsSettingsHelper.Settings.ProtectedPrograms.Clone()).ForEach(x=> 
            {
                PathHelper.SetRootPath(_iOHelper.PathRoot, x.ProtectedProgramPaths);
                ProtectedPrograms.Add(x);
            });
            return Task.CompletedTask;
        }

        private void SelectSingleProgram(SelectSingleProgramEventArgs obj)
        {
            foreach (var programViewModel in GroupProgramViewModel.Single(x => x.GroupProgram.Name == obj.NameGroup).ProgramViewModels)
            {
                if (programViewModel.Program.ProgramName != obj.NameProgram)
                {
                    programViewModel.NotSelectProgram();
                }
            }
        }
        public void Unsubscribe()
        {
           
        }
        public bool HasChanges()
        {
            return !IsEnabled;
        }
        //***************************************************************************************************
        private void OnClearConsole()
        {
            TextConsole = WinInfoHelper.GetSystemInfo();
        }
        //---------------------------------------------------------------------------------------------------
        private void OnSelectProgramsSet()
        {
            if (ProgramsSets.Count != 0)
            {
                var Preset = ProgramsSets.Single(x => x.Name == SelectedProgramsSet);
                foreach (var program in GroupProgramViewModel.SelectMany(group => group.ProgramViewModels))
                {
                    if (Preset.InstallProgramName.Contains(program.Program.ProgramName))
                    {
                        program.SelectProgram();
                    }
                    else
                        program.NotSelectProgram();
                }
            }

        }
        private void OnOnlyFree()
        {
            foreach (var program in GroupProgramViewModel.SelectMany(group => group.ProgramViewModels))
            {
                if(program.Install == true && program.Program.IsFree==false)
                    program.NotSelectProgram();
            }
        }
        //---------------------------------------------------------------------------------------------------
        private async void OnInstall()
        {
            var programsToInstall = GroupProgramViewModel.SelectMany(group => group.ProgramViewModels).Where(x => x.Install == true);

            if (programsToInstall.Any(x => x.Program.DisableDefender == true && !x.IsInstall)) // якщо програма потребує вимклення антивірусника для свого встановлення і вона ще не встановлена тоді
            {
                if (WinDefenderHelper.CheckTamperProtection() == WinDefenderEnum.Enabled) // Перевіряємо чи увімкнуто захист від підробок і відповідно чи можемо ми керувати  Windows Defender
                {
                    _messageDialogService.ShowInfoDialog($"Нічого не буде, треба вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!\n" +
                        $"Оскільки встановлення одної з програм потребує автоматичного відключення ативірусного ПЗ!", "Помилка!");
                    WinDefenderHelper.OpenWinDefenderSettings();
                    return;
                }
            }
            IsEnabled = false;
            if(programsToInstall.Count()!=0)
                AddSplitter();
            foreach (var programToInstall in programsToInstall)
            {
                TextConsole += $"Start Install Programs: {programToInstall.Program.ProgramName}\n";
                if (programToInstall.IsInstall) // перевіряємо чи програма встановлена
                {
                    TextConsole += $"Programs: {programToInstall.Program.ProgramName}, Already installed!\n";
                    AddSplitter();
                    continue;
                }
                if (programToInstall.Program.OnlineInstaller != null && await SpeedTest(true) >= 0.5) // якщо є онлайн інсталятор і інтернет
                    await InstallProgram(programToInstall, true); // встановлюємо онлайн 
                else
                    await InstallProgram(programToInstall, false); // встановлюємо офлайн
                AddSplitter();
            }
            IsEnabled = true;
        }
        private async Task InstallProgram(ProgramViewModel programViewMode, bool tryOnlineInstall)
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
                    if (programViewMode.Program.DisableDefender && (WinInfoHelper.WinVer == WinVerEnum.Win10 || WinInfoHelper.WinVer == WinVerEnum.Win11)) // якщо треба вимкнути антивірусник і це windows 10 або 11
                    {
                        if (await WinDefenderHelper.CheckRealtimeMonitoring() == WinDefenderEnum.Enabled) // якщо антивірусник увімкнутий
                        {
                            await OffDefender(); // вимикаємо
                            await Task.Delay(100); // зачекаємо вимкнення
                        }
                        if (await WinDefenderHelper.CheckRealtimeMonitoring() == WinDefenderEnum.Enabled) // якщо антивірусник досі увімкнутий
                        {
                            TextConsole += "Error: defender is not disabled\n"; // сповіщаємо про помилку
                            break; // виходимо з циклу (далі буде помилка в циклі встановлення)
                        }
                    }
                    if (tryOnlineInstall) // якщо є онлайн інсталятор намагаємось його знайти і задаємо аргумент онлайн інсталятора
                    {
                        ExecutableFile = ProgramsHelper.GetExeMsiFile(_iOHelper, program.OnlineInstaller.FileName, program.PathFolder).FirstOrDefault();
                        arguments = string.Join(" ", program.OnlineInstaller.Arguments);
                    }
                    if (ExecutableFile == null) // якщо онлайн інсталятор не знайдено тоді шукаємо офлайн і задаємо аргументи офлайн інсталятора
                    {
                        var tempExecutableFile = ProgramsHelper.GetExeMsiFile(_iOHelper, program.FileName, program.PathFolder); // ортимуємо список файлів
                        if (WinInfoHelper.Isx64) // якщо наша система х64
                            ExecutableFile = tempExecutableFile.FirstOrDefault(x => x.Contains("x64", StringComparison.InvariantCultureIgnoreCase)); // то намагаємось знайти файл, що містить х64 в назві
                        if (ExecutableFile == null) //якщо система не х64 або файла х64 нема
                            ExecutableFile = tempExecutableFile.LastOrDefault();  // обираємо те що є
                        arguments = string.Join(" ", program.Arguments); 
                    }
                    if (ExecutableFile != null) // якщо інсталятор знайдено то 
                        break; // покидаємо цикл пошуку файлу (внутрішній) (далі буде спроба встановити програму) 
                    else // якщо онлайн і офлайн інсталятор не знайдено
                    {
                        TextConsole += $"Not found install file in folder: {program.PathFolder}\n"; // сповіщаємо користувача, що файлу нема
                        if (countDecompress >= 1) // якщо ми вже спробували його розпакувати еле його досі нема
                            break; // то покидаємо цикл пошуку файлу (далі буде помилка в циклі встановлення)
                        if (program.DisableDefender) // якщо треба вимикати антивірусник і файла нема то намагаємось його добути з архіва
                        {
                            if (!File.Exists("7za.exe")) // перевіряємо чи на місці архіватор 
                            {
                                TextConsole += $"Error, Not 7za.exe\n"; // якщо його нема то сповіщаємо користувача
                                break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі встановлення)
                            }
                            TextConsole += $"Trying find archive, resault: "; // сповіщаємо користувача про спробу знайти архів
                            var pathRar = Directory.GetFiles(program.PathFolder).Where(x => x.Contains(program.FileName, StringComparison.InvariantCultureIgnoreCase) && x.EndsWith(".7z")).FirstOrDefault(); // шукаємо архів
                            if (string.IsNullOrWhiteSpace(pathRar)) // перевіряємо чи знайдено архів
                            {
                                TextConsole += $"Error, Not Find arkhive!\n"; // якщо його нема то сповіщаємо користувача
                                break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі встановлення)
                            }
                            TextConsole += $"OK!\n"; // сповіщаємо користувача, що архів знайдено
                            TextConsole += $"Start decompress, resault: ";
                            await Task.Delay(500); // дамо часу основному потоку обновити інформацію в консолі
                            switch (ArchiverHelper.Decompress(pathRar, program.PathFolder, _programsSettingsHelper.Settings.ArchivePassword, 20000)) //спробу розпакувати архів
                            {
                                case ResultArchiver.OK:
                                    TextConsole += $"OK!\n";
                                    break;
                                case ResultArchiver.TimeOut:
                                    TextConsole += "Time out decompress\n";
                                    break;
                                case ResultArchiver.Error:
                                    TextConsole += "Error decompress\n";
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
                    TextConsole += $"Programs: {program.ProgramName}, not installed!\n"; //то сповіщаємо користувача про це
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
                        TextConsole += $"File msi: {ExecutableFile}\n";
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
                        TextConsole += $"File exe: {ExecutableFile}\n";
                    }
                    TextConsole += $"Arguments: {StartInfo.Arguments}\n";

                    try // намагаємось встановити програму і очікуємо завершення її встановлення
                    {
                        Process proc = Process.Start(StartInfo);
                        await Task.Factory.StartNew(() => proc.WaitForExit());
                        TextConsole += $"Programs: {program.ProgramName}, Installed!\n";
                        await Task.Delay(1000); // пауза для CheckInstall (щоб встиг оновитись реєстр)
                        programViewMode.CheckInstall(WinInfoHelper.ListInstallPrograms()); // CheckInstall
                        break; // покидаємо цикл встановлення програми
                    }
                    catch (Exception exp)
                    {
                        TextConsole += $"Program: {program.ProgramName}, error install: {exp.Message}\n";
                        if (!program.DisableDefender) // якщо це програма яка не потребує вимкнення антивірусника і її встановелння викликало помилку тоді не пробуємо ставити її ще раз
                            break; // покидаємо цикл встановлення програми
                    }

                    countInstall++; // лічильник спроб встановити програму їх як сказано на початку буде лише дві
                    if (countInstall >= 2)
                        break;
                    TextConsole += $"<***************************************************************************>\n";
                }
            } while (true);
            if (program.DisableDefender && await WinDefenderHelper.CheckRealtimeMonitoring() == WinDefenderEnum.Disabled) //якщо треба було вимкнути антивірусник  і він вимкнутий
                await WinDefenderHelper.EnableRealtimeMonitoring(); // вмикаємо назад
        }


        //---------------------------------------------------------------------------------------------------
        private void OnAUC()
        {
            WinSettingsHelper.OpenAUC();
        }
        private void OnIcon()
        {
            WinSettingsHelper.OpenIcon();
        }
        private void OnOpenDesktopIconSettings()
        {
            WinSettingsHelper.OpenDesktopIconSettings();
        }
        //**************************************************************************************************
        private void OnDefault()
        {
            WinSettingsHelper.OpenDefaultPrograms(_messageDialogService);
        }
        private void OnOpenExampleFile()
        {
            _iOHelper.OpenFolder(_programsPaths.PathExampleFile);
        }
        private async void OpenProtectedProgram(ProtectedProgram protectedProgram)
        {
            if ((int)WinInfoHelper.WinVer < 6) // якщо версія ОС менша Windows 10
            {
                _messageDialogService.ShowInfoDialog("Можна керувати Windows Defender лише з Windows 10 або Windows 11", "Помилка!");
                return; // виходимо
            }
            if (WinDefenderHelper.CheckTamperProtection() == WinDefenderEnum.Enabled) // Перевіряємо чи увімкнуто захист від підробок і відповідно чи можемо ми керувати  Windows Defender
            {
                _messageDialogService.ShowInfoDialog($"Потрібно вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!", "Помилка!"); // якщо ні то сповіщаємо користувача
                WinDefenderHelper.OpenWinDefenderSettings();
                return; // виходимо
            }
            
            IsEnabled = false;
            AddSplitter();
            TextConsole += $"Start open {protectedProgram.Name}\n";
            int countOpen = 0;
            do // цикл запуску protectedProgram (Дві спроби)
            {
                int countDecompress = 0;
                string ExecutableFile = null;
                do // цикл пошуку файла (цікаво зроблено, він виконується або 0.5 або на 1.5 рази)
                {
                    if ((int)WinInfoHelper.WinVer>5) //
                    {
                        if (await WinDefenderHelper.CheckRealtimeMonitoring() == WinDefenderEnum.Enabled) // якщо антивірусник увімкнутий
                        {
                            await OffDefender(); // вимикаємо
                            await Task.Delay(100);
                        }
                        if (await WinDefenderHelper.CheckRealtimeMonitoring() == WinDefenderEnum.Enabled) // якщо антивірусник досі увімкнутий
                        {
                            TextConsole += "Error: defender is not disabled\n";
                            break; // виходимо з циклу
                        }
                    }
                    if (File.Exists(protectedProgram.ProtectedProgramPaths.PathExe)) // якщо protectedProgram є на свому місці
                    {
                        ExecutableFile = protectedProgram.ProtectedProgramPaths.PathExe; // назначаємо його
                        break; // покидаємо цикл пошуку файлу (внутрішній) (далі буде спроба запустити protectedProgram) 
                    }
                    else // якщо файла нема
                    {
                        TextConsole += $"Not found {protectedProgram.Name}\n"; // сповіщаємо користувача, що файлу нема
                        if (countDecompress >= 1) // якщо ми вже спробували його розпакувати еле його досі нема
                            break; // то покидаємо цикл пошуку файлу(далі буде помилка в циклі встановлення)
                        if (!File.Exists("7za.exe")) // перевіряємо чи на місці архіватор 
                        {
                            TextConsole += $"Error, Not 7za.exe\n"; // якщо його нема то сповіщаємо користувача
                            break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі запуску protectedProgram)
                        }
                        TextConsole += $"Trying find archive {Path.GetFileName(protectedProgram.ProtectedProgramPaths.PathRar)}, resault: "; // сповіщаємо користувача про спробу знайти архів
                        if (!File.Exists(protectedProgram.ProtectedProgramPaths.PathRar)) // перевіряємо чи знайдено архів
                        {
                            TextConsole += $"Error, not Find arkhive!\n"; // якщо його нема то сповіщаємо користувача
                            break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі запуска protectedProgram)
                        }
                        TextConsole += $"OK!\n";
                         TextConsole += $"Start decompress, resault: ";
                            await Task.Delay(500); // дамо часу основному потоку обновити інформацію в консолі
                            switch (ArchiverHelper.Decompress(protectedProgram.ProtectedProgramPaths.PathRar, Path.GetDirectoryName(protectedProgram.ProtectedProgramPaths.PathRar), _programsSettingsHelper.Settings.ArchivePassword, 20000)) //спробу розпакувати архів
                            {
                                case ResultArchiver.OK:
                                    TextConsole += $"OK!\n";
                                    break;
                                case ResultArchiver.TimeOut:
                                    TextConsole += "Time out decompress\n";
                                    break;
                                case ResultArchiver.Error:
                                    TextConsole += "Error decompress\n";
                                    break;
                            }
                            countDecompress++; // лічильник спроб розпакувати архів
                    }
                } while (true);
                if (ExecutableFile == null) // якщо protectedProgram так і не знайдено то
                {
                    TextConsole += $"{protectedProgram.Name} is not open!\n"; //сповіщаємо користувача про це (далі ще раз спробуємо знайти файл якщо не вичерпано ліміт спроб)
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
                        TextConsole += $"{protectedProgram.Name} open\n";
                        AddSplitter();
                        await Task.Run(() => proc.WaitForExit()); // очікуємо завершення роботи програми
                        break; // покидаємо цикл запуску програми
                    }
                    catch (Exception exp)
                    {
                        TextConsole += $"{protectedProgram.Name} error open: {exp.Message}\n"; // якщо сталась помилка сповыщаэмо про це
                        if (countOpen + 1 < 2)
                        {
                            TextConsole += $"Pause 20 seconds...\n";
                            await Task.Delay(20000); // пауза для того щоб антивірусник "відпустив" файл
                        }
                    }
                }
                countOpen++; // збільшуємо лічильник спроб запуску
                if (countOpen >= 2) // перевіряємо кількість спроб запуску
                    break; // виходимо якщо їх було дві
                TextConsole += $"<***************************************************************************>\n";
            } while (true);
            AddSplitter();
            IsEnabled = true;
            await WinDefenderHelper.EnableRealtimeMonitoring();
   
        }
        private void OnOpenAppLog()
        {
            _iOHelper.OpenFolder(_iOHelper.CompasPackLog);
        }
        //**************************************************************************************************
        private async void OnSpeedTest() // ✓
        {
            await SpeedTest(false);
        }
        private async Task<double> SpeedTest(bool OnInstall) // ✓
        {
            if (!OnInstall)
            {
                IsEnabled = false;
                AddSplitter();
            }
            TextConsole += $"Start speed test: \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            var speed = await NetworkHelper.SpeedTest();
            TextConsole += $"End speed test: \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            TextConsole += $"Speed: {Math.Round(speed, 2)} Mbyte/s\n";
            if (!OnInstall)
            {
                AddSplitter();
                IsEnabled = true;
            }
            return speed;
        }
        private async void OnOnDefender() // ✓
        {
            if ((int)WinInfoHelper.WinVer < 6) // якщо версія ОС менша Windows 10
            {
                _messageDialogService.ShowInfoDialog("Можна керувати Windows Defender лише з Windows 10 або Windows 11", "Помилка!");
                return; // виходимо
            }
            if (WinDefenderHelper.CheckTamperProtection() == WinDefenderEnum.Enabled)
            {
                _messageDialogService.ShowInfoDialog($"Потрібно вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!", "Помилка!"); // якщо ні то сповіщаємо користувача
                WinDefenderHelper.OpenWinDefenderSettings();
                return; // виходимо
            }
            IsEnabled = false;
            AddSplitter();
            await OnDefender();
            AddSplitter();
            IsEnabled = true;
        }
        private async void OnOffDefender() // ✓
        {
            if ((int)WinInfoHelper.WinVer < 6) // якщо версія ОС менша Windows 10
            {
                _messageDialogService.ShowInfoDialog("Можна керувати Windows Defender лише з Windows 10 або Windows 11", "Помилка!");
                return;
            }
            if (WinDefenderHelper.CheckTamperProtection() == WinDefenderEnum.Enabled)
            {
                _messageDialogService.ShowInfoDialog($"Потрібно вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!", "Помилка!"); // якщо ні то сповіщаємо користувача
                WinDefenderHelper.OpenWinDefenderSettings();
                return; // виходимо
            }
            IsEnabled = false;
            AddSplitter();
            await OffDefender();
            AddSplitter();
            IsEnabled = true;
        }

        private async Task OnDefender() // ✓
        {
            TextConsole += $"Start on defender: \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            var ResponseDefender = (await WinDefenderHelper.EnableRealtimeMonitoring()).Trim();
            if (!string.IsNullOrWhiteSpace(ResponseDefender))
                TextConsole += $"Response defender: {ResponseDefender}\n";
            TextConsole += $"End on defender:  \t{DateTime.Now:dd/MM hh:mm:ss}\n";

            var realtimeMonitoring = await WinDefenderHelper.CheckRealtimeMonitoring();
            TextConsole += $"Defender is: \t\t{realtimeMonitoring} ";
            if (realtimeMonitoring == WinDefenderEnum.Enabled)
                TextConsole += $"(OK!)\n";
            else
                TextConsole += $"(Fail!)\n";
        }

        private async Task OffDefender() // ✓
        {
            TextConsole += $"Start off defender: \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            var ResponseDefender = (await WinDefenderHelper.DisableRealtimeMonitoring()).Trim();
            if (!string.IsNullOrWhiteSpace(ResponseDefender))
                TextConsole += $"Response defender: {ResponseDefender}\n";
            TextConsole += $"End off defender:  \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            var realtimeMonitoring = await WinDefenderHelper.CheckRealtimeMonitoring();
            TextConsole += $"Defender is: \t\t{realtimeMonitoring} ";
            if(realtimeMonitoring == WinDefenderEnum.Disabled)
                TextConsole += $"(OK!)\n";
            else
                TextConsole += $"(Fail!)\n";
        }
        //--------------------------------------
        public void AddSplitter()
        {
            string[] strings = TextConsole.Split('\n');
            if (string.IsNullOrWhiteSpace(strings.Last()))
            {
                if (!strings[strings.Length - 2].Contains("<------------------------------------------------------------------------------>"))
                    TextConsole += "<------------------------------------------------------------------------------>\n";
            }
        }

        //--------------------------------------
        public ICommand SelectProgramsSetCommand { get; }
        public ICommand OnlyFreeCommand { get; }
        public ICommand InstallCommand { get; }
        public ICommand ClearConsoleCommand { get; }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++
        public ICommand AUCCommand { get; }
        public ICommand IconCommand { get; }
        public ICommand OpenDesktopIconSettingsCommand { get; }
        //****************************************************
        public ICommand DefaultCommand { get; }
        public ICommand OpenExampleFileCommand { get; }
        public ICommand OpenProtectedProgramCommand { get; }
        public ICommand OpenAppLogCommand { get; }
        //****************************************************
        public ICommand SpeedTestCommand { get; }
        public ICommand OffDefenderCommand { get; }
        public ICommand OnDefenderCommand { get; }

        //--------------------------------------
    }
}
