using Autofac.Features.Indexed;
using CompasPack.Data.Providers;
using CompasPack.Data.Providers.API;
using CompasPack.Helper.Event;
using CompasPack.Helper.Extension;
using CompasPack.Helper.Service;
using CompasPack.Helper.Service.Antivirus;
using CompasPack.Model.Enum;
using CompasPack.Model.Entities.Programs;
using CompasPack.Model.Wrapper;
using CompasPack.Model.Support;
using CompasPack.Startup;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;



namespace CompasPack.ViewModel
{
    public interface ITextConsole
    {
        string TextConsole { get; set; }
    }
    public class ProgramsViewModel : ViewModelBase, IViewModel, ITextConsole
    {
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private readonly IEnumerable<IAntivirus> _antiviruses;
        private readonly ProgramsSettingsProvider _programsSettingsProvider;
        private readonly WinInfo _winInfo;
        private readonly IWinSettingsLauncher _winSettingsLauncher;
        private ProgramsPaths _programsPaths;
        private readonly IFileSystemReaderWriter _fileSystemReaderWriter;
        private readonly IFileSystemNavigator _fileSystemNavigator;
        private readonly IFileArchiver _fileArchiver;
        private string _selectedProgramsSets;
        private string _textConsole;
        private bool _isEnabled;
        public ObservableCollection<ProgramsSet> ProgramsSets { get; }
        public ObservableCollection<GroupProgramsWrapper> GroupProgramsWrappers { get; }
        public ObservableCollection<ProtectedProgram> ProtectedPrograms { get; }       
        public List<IAntivirus> Antiviruses { get; }
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
        public ProgramsViewModel(IEventAggregator eventAggregator, IEnumerable<IAntivirus> antiviruses,
            IMessageDialogService messageDialogService, 
            IFileSystemReaderWriter fileSystemReaderWriter, IFileSystemNavigator fileSystemNavigator, IFileArchiver fileArchiver,
            ProgramsSettingsProvider programsSettingsProvider, WinInfo winInfo, IWinSettingsLauncher winSettingsLauncher)
        {
            ProgramsSets = new ObservableCollection<ProgramsSet>();
            GroupProgramsWrappers = new ObservableCollection<GroupProgramsWrapper>();
            ProtectedPrograms = new ObservableCollection<ProtectedProgram>();
            Antiviruses = new List<IAntivirus>();

            _eventAggregator = eventAggregator;
            _antiviruses = antiviruses;

            _messageDialogService = messageDialogService;
            
            _programsSettingsProvider = programsSettingsProvider;
            _winInfo = winInfo;
            _winSettingsLauncher = winSettingsLauncher;
            _fileSystemReaderWriter = fileSystemReaderWriter;
            _fileSystemNavigator = fileSystemNavigator;
            _fileArchiver = fileArchiver;
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
        public Task LoadAsync()
        {
            TextConsole = _winInfo.ToString();

            ProgramsSets.Clear();
            GroupProgramsWrappers.Clear();
            ProtectedPrograms.Clear();
            Antiviruses.Clear();

            _programsPaths = (ProgramsPaths)_programsSettingsProvider.Settings.ProgramsPaths.Clone();
            ProgramsHelper.SetRootPath(_fileSystemReaderWriter.PathRoot, _programsPaths);
            //----------------------------------------------------------------------------------------------------
            var GroupsPrograms = (List<GroupPrograms>)_programsSettingsProvider.Settings.GroupsPrograms?.Clone();
            foreach (var groupProgram in GroupsPrograms)
                GroupProgramsWrappers.Add(new GroupProgramsWrapper(groupProgram, new ObservableCollection<ProgramWrapper>(groupProgram.Programs.Select(x => new ProgramWrapper(x, groupProgram, _eventAggregator)))));

            ProgramsHelper.CombinePathFolderAndImage(GroupProgramsWrappers, _programsPaths);
            ProgramsHelper.CheckInstallPrograms(GroupProgramsWrappers, _winInfo.WinArchitecture); // CheckInstall
            //----------------------------------------------------------------------------------------------------
            _programsSettingsProvider.Settings.ProgramsSets.ForEach(x => ProgramsSets.Add(x)); // Add ProgramsSets     
            var tempProgramsSet = ProgramsSets.FirstOrDefault(x => x.Name.Contains(Regex.Match(_winInfo.ProductName, @"\d+").Value, StringComparison.InvariantCultureIgnoreCase)); // check ProgramsSet
            if (tempProgramsSet != null)
                SelectedProgramsSet = tempProgramsSet.Name;
            OnSelectProgramsSet();
            //----------------------------------------------------------------------------------------------------
            ((List<ProtectedProgram>)_programsSettingsProvider.Settings.ProtectedPrograms.Clone()).ForEach(x=> 
            {
                ProgramsHelper.SetRootPath(_fileSystemReaderWriter.PathRoot, x.ProtectedProgramPaths);
                ProtectedPrograms.Add(x);
            });

            return Task.CompletedTask;
        }

        private void SelectSingleProgram(SelectSingleProgramEventArgs obj)
        {
            foreach (var programViewModel in GroupProgramsWrappers.Single(x => x.GroupProgram.Name == obj.NameGroup).ProgramWrappers)
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
            TextConsole = _winInfo.ToString();
        }
        //---------------------------------------------------------------------------------------------------
        private void OnSelectProgramsSet()
        {
            if (ProgramsSets.Count != 0)
            {
                var Preset = ProgramsSets.Single(x => x.Name == SelectedProgramsSet);
                foreach (var program in GroupProgramsWrappers.SelectMany(group => group.ProgramWrappers))
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
            foreach (var program in GroupProgramsWrappers.SelectMany(group => group.ProgramWrappers))
            {
                if(program.Install == true && program.Program.IsFree==false)
                    program.NotSelectProgram();
            }
        }
        //---------------------------------------------------------------------------------------------------
        void CancelProblematicPrograms(ref List<ProgramWrapper> selected, List<ProgramWrapper> problematic)
        {
            problematic.ForEach(p => p.Install = false);
            selected = selected.Except(problematic).ToList();
        }
        void ShowConflictDialog(List<ProgramWrapper> progs, List<IAntivirus> avs, string reason)
        {
            var pNames = progs.Select(p => p.Program.ProgramName).ToList();
            var aNames = avs.Select(av => av.AntivirusInfo.DisplayName).ToList();

            var message = $"Ви обрали {(pNames.Count > 1 ? "програми" : "програму")}, для яких треба вимкнути захист:\n" +
                          $"{string.Join(";\n", pNames)}.\n\n" +
                          $"Антивіруси ({reason}):\n" +
                          $"{string.Join(";\n", aNames)}.\n\n" +
                          "Ці програми буде пропущено!";

            _messageDialogService.ShowInfoDialog(message, "Увага");
        }
        private async void OnInstall()
        {
            var selectedPrograms = GroupProgramsWrappers.SelectMany(group => group.ProgramWrappers).Where(x => x.Install == true).ToList();
            var problematicPrograms = selectedPrograms.Where(p => p.Program.DisableDefender /*&& !p.IsInstall*/).ToList();

            if (problematicPrograms.Any())
            {
                var unManualAntiviruses = _antiviruses.Where(av => !av.IsControlled).ToList();
                
                if (unManualAntiviruses.Any())
                {
                    ShowConflictDialog(problematicPrograms, unManualAntiviruses, "не керуються програмно");
                    CancelProblematicPrograms(ref selectedPrograms, problematicPrograms);
                }
                else
                {
                    bool checkAgain = true;
                    while (checkAgain)
                    {
                        var tamperProtectionAntiviruses = _antiviruses.Where(av => av.IsControlled && av.GetTamperProtectionStatus() != AntivirusStatusEnum.Disabled).ToList();

                        if (!tamperProtectionAntiviruses.Any())
                        {
                            checkAgain = false;
                            continue;
                        }

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
                            ShowConflictDialog(problematicPrograms, tamperProtectionAntiviruses, "захищені від змін");
                            CancelProblematicPrograms(ref selectedPrograms, problematicPrograms);
                            checkAgain = false;
                        }
                    }
                }
            }
            IsEnabled = false;
            if(selectedPrograms.Count()!=0)
                AddSplitter();
            foreach (var programToInstall in selectedPrograms)
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
        private async Task InstallProgram(ProgramWrapper programViewMode, bool tryOnlineInstall)
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
                        if (await Antiviruses.First().GetRealTimeMonitoringStatus() == AntivirusStatusEnum.Enabled) // якщо антивірусник увімкнутий
                        {
                            await OffDefender(); // вимикаємо
                            await Task.Delay(100); // зачекаємо вимкнення
                        }
                        if (await Antiviruses.First().GetRealTimeMonitoringStatus() == AntivirusStatusEnum.Enabled) // якщо антивірусник досі увімкнутий
                        {
                            TextConsole += "Error: defender is not disabled\n"; // сповіщаємо про помилку
                            break; // виходимо з циклу (далі буде помилка в циклі встановлення)
                        }
                    }
                    if (tryOnlineInstall) // якщо є онлайн інсталятор намагаємось його знайти і задаємо аргумент онлайн інсталятора
                    {
                        ExecutableFile = ProgramsHelper.GetExeMsiFile(_fileSystemReaderWriter, program.OnlineInstaller.FileName, program.PathFolder).FirstOrDefault();
                        arguments = string.Join(" ", program.OnlineInstaller.Arguments);
                    }
                    if (ExecutableFile == null) // якщо онлайн інсталятор не знайдено тоді шукаємо офлайн і задаємо аргументи офлайн інсталятора
                    {
                        var tempExecutableFile = ProgramsHelper.GetExeMsiFile(_fileSystemReaderWriter, program.FileName, program.PathFolder); // ортимуємо список файлів
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
                            switch (_fileArchiver.Decompress(pathRar, program.PathFolder, _programsSettingsProvider.Settings.ArchivePassword, 20000)) //спробу розпакувати архів
                            {
                                case ResultArchiverEnum.OK:
                                    TextConsole += $"OK!\n";
                                    break;
                                case ResultArchiverEnum.TimeOut:
                                    TextConsole += "Time out decompress\n";
                                    break;
                                case ResultArchiverEnum.Error:
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
                        //Process proc = Process.Start(StartInfo);
                        //await Task.Factory.StartNew(() => proc.WaitForExit());
                        TextConsole += $"Programs: {program.ProgramName}, Installed!\n";
                        await Task.Delay(1000); // пауза для CheckInstall (щоб встиг оновитись реєстр)
                        programViewMode.CheckInstall(SoftwareInfoProvider.GetInstallPrograms(_winInfo.WinArchitecture)); // CheckInstall
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
            if (program.DisableDefender && await Antiviruses.First().GetRealTimeMonitoringStatus() == AntivirusStatusEnum.Disabled) //якщо треба було вимкнути антивірусник  і він вимкнутий
                await Antiviruses.First().EnableRealTimeMonitoring(); // вмикаємо назад
        }

        //---------------------------------------------------------------------------------------------------
        private void OnAUC()
        {
            _winSettingsLauncher.OpenAUCSettings();
        }
        private void OnIcon()
        {
            _winSettingsLauncher.OpenIconSettings();
        }
        private void OnOpenDesktopIconSettings()
        {
            _winSettingsLauncher.OpenDesktopIconSettings();
        }
        //**************************************************************************************************
        private void OnDefault()
        {
            _winSettingsLauncher.OpenDefaultProgramsSettings();
        }
        private void OnOpenExampleFile()
        {
            _fileSystemNavigator.OpenFolder(_programsPaths.PathExampleFile);
        }
        private async void OpenProtectedProgram(ProtectedProgram protectedProgram)
        {
            bool isActiveRealtimeMonitoring = false;
            var antivirusProduct = SoftwareInfoProvider.GetAntivirusProducts();
            if (antivirusProduct.Count == 0)
            {
                var res = _messageDialogService.ShowYesNoDialog($"Ви використовуєте {_winInfo.ProductName}.\n" +
                    $"В системі не знайдено жодного антивірусного ПЗ.\n" +
                    $"Якщо антивірусне ПЗ дійсно відсутнє натисніть \"Так\" для запуску \"{protectedProgram.Name}\" на свій страх та ризик!\n" +
                    $"Якщо ви не впевнені тоді натисни \"Ні\" для зупинки запуску \"{protectedProgram.Name}\"!", "Попередження!");
                if (res == MessageDialogResultEnum.No)
                    return;
            }
            else
            {
                if (_winInfo.WinVer == WinVersionEnum.Win_10 || _winInfo.WinVer == WinVersionEnum.Win_11) // Win10, Win11
                {
                    if (antivirusProduct.Count == 1)
                    {
                        if (!antivirusProduct.First().DisplayName.Contains("Windows Defender", StringComparison.InvariantCultureIgnoreCase))
                        {
                            _messageDialogService.ShowInfoDialog($"В системі працює посторонній: \"{antivirusProduct.First()}\", управління ними та \"{protectedProgram.Name}\" можливо лише вручну!", "Помилка!");
                            return;
                        }
                    }
                    else
                    {
                        _messageDialogService.ShowInfoDialog($"В системі працює посторонні антивіруси, управління ними та \"{protectedProgram.Name}\" можливо лише вручну!", "Помилка!");
                        return;
                    }
                    if (IsTamperProtectionEnabled())
                        return;
                    isActiveRealtimeMonitoring = true;
                }
                else // Win8, Win8, Win8.1
                {
                    _messageDialogService.ShowInfoDialog($"Ви використовуєте {_winInfo.ProductName}.\n" +
                        $"В системі працює посторонні(й) антивірус(и), управління ним(и) та \"{protectedProgram.Name}\" можливо лише вручну!", "Попередження!");
                    return;
                }
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
                    if (isActiveRealtimeMonitoring) // якщо це Win10 або Win11, Windows Defender знайдено і захист від підробок вимкнуто
                    {
                        if (await Antiviruses.First().GetRealTimeMonitoringStatus() == AntivirusStatusEnum.Enabled) // якщо антивірусник увімкнутий
                        {
                            await OffDefender(); // вимикаємо
                            await Task.Delay(100);
                        }
                        if (await Antiviruses.First().GetRealTimeMonitoringStatus() == AntivirusStatusEnum.Enabled) // якщо антивірусник досі увімкнутий
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
                            switch (_fileArchiver.Decompress(protectedProgram.ProtectedProgramPaths.PathRar, Path.GetDirectoryName(protectedProgram.ProtectedProgramPaths.PathRar), _programsSettingsProvider.Settings.ArchivePassword, 20000)) //спробу розпакувати архів
                            {
                                case ResultArchiverEnum.OK:
                                    TextConsole += $"OK!\n";
                                    break;
                                case ResultArchiverEnum.TimeOut:
                                    TextConsole += "Time out decompress\n";
                                    break;
                                case ResultArchiverEnum.Error:
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
            if (isActiveRealtimeMonitoring) // якщо це Win10 або Win11, Windows Defender знайдено і захист від підробок вимкнуто
            {
                await Antiviruses.First().EnableRealTimeMonitoring();
            }
        }
        private void OnOpenAppLog()
        {
            _fileSystemNavigator.OpenFolder(_fileSystemReaderWriter.CompasPackLog);
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
            var speed = await NetworkService.SpeedTest("https://github.com/Maxim-Paluh/SpeedTest/raw/main/10MB");
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
            if (!IsSupportedWindowsVersionDefender(_winInfo.WinVer))
                return;
            if (!IsWindowsDefender())
                return;
            if (IsTamperProtectionEnabled())
                return;
            IsEnabled = false;
            AddSplitter();
            await OnDefender();
            AddSplitter();
            IsEnabled = true;
        }
        private async void OnOffDefender() // ✓
        {
            if (!IsSupportedWindowsVersionDefender(_winInfo.WinVer))
                return;
            if (!IsWindowsDefender())
                return;
            if(IsTamperProtectionEnabled())
                return;
            IsEnabled = false;
            AddSplitter();
            await OffDefender();
            AddSplitter();
            IsEnabled = true;
        }

        private async Task OnDefender() // ✓
        {
            TextConsole += $"Start on defender: \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            var ResponseDefender = await Antiviruses.First().EnableRealTimeMonitoring();
                TextConsole += $"Response defender: {ResponseDefender}\n";
            TextConsole += $"End on defender:  \t{DateTime.Now:dd/MM hh:mm:ss}\n";

            var realtimeMonitoring = await Antiviruses.First().GetRealTimeMonitoringStatus();
            TextConsole += $"Defender is: \t\t{realtimeMonitoring} ";
            if (realtimeMonitoring == AntivirusStatusEnum.Enabled)
                TextConsole += $"(OK!)\n";
            else
                TextConsole += $"(Fail!)\n";
        }

        private async Task OffDefender() // ✓
        {
            TextConsole += $"Start off defender: \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            var ResponseDefender = await Antiviruses.First().DisableRealTimeMonitoring();
                TextConsole += $"Response defender: {ResponseDefender}\n";
            TextConsole += $"End off defender:  \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            var realtimeMonitoring = await Antiviruses.First().GetRealTimeMonitoringStatus();
            TextConsole += $"Defender is: \t\t{realtimeMonitoring} ";
            if(realtimeMonitoring == AntivirusStatusEnum.Disabled)
                TextConsole += $"(OK!)\n";
            else
                TextConsole += $"(Fail!)\n";
        }

        private bool IsWindowsDefender()
        {
            var antivirusProduct = SoftwareInfoProvider.GetAntivirusProducts();
            if (antivirusProduct.Count == 0)
            {
                _messageDialogService.ShowInfoDialog("Windows Defender не знайдено в системі", "Помилка!");
                return false;
            }
            else if (antivirusProduct.Count == 1)
            {
                if (antivirusProduct.First().DisplayName.Contains("Windows Defender", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                else
                {
                    _messageDialogService.ShowInfoDialog($"У системі працює посторонній: \"{antivirusProduct.First()}\", керуйте ним в ручну!", "Помилка!");
                    return false;
                }
            }
            else
            {
                _messageDialogService.ShowInfoDialog($"У системі працює кілька посторонніх антивірусів, керуйте ними в ручну!", "Помилка!");
                return false;
            }
        }

        private bool IsSupportedWindowsVersionDefender(WinVersionEnum winVer)
        {
            if (!(winVer == WinVersionEnum.Win_10 || winVer == WinVersionEnum.Win_11))
            {
                _messageDialogService.ShowInfoDialog("Можна керувати Windows Defender лише з Windows 10 або Windows 11", "Помилка!");
                return false;
            }
            return true;
        }

        private bool IsTamperProtectionEnabled()
        {
            if (Antiviruses.First().GetTamperProtectionStatus() == AntivirusStatusEnum.Enabled)
            {
                _messageDialogService.ShowInfoDialog($"Потрібно вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!", "Помилка!"); // якщо ні то сповіщаємо користувача
                Antiviruses.First().OpenSettings();
                return true;
            }
            else
                return false;
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
