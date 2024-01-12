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
        private readonly UserProgramsSettingsHelper _userProgramsSettingsHelper;
        private readonly UserPresetSettingsHelper _userPresetSettingsHelper;
        private readonly UserPathSettingsHelper _userPathSettingsHelper;
        private UserPath _userPath;
        private readonly IIOHelper _iOHelper;
        private string _selectedUserPreset;
        private string _textConsole;
        private bool _isEnabled;
        public ObservableCollection<UserPreset> UserPresetPrograms { get; }
        public ObservableCollection<GroupProgramViewModel> GroupProgramViewModel { get; }
        public string TextConsole
        {
            get { return _textConsole; }
            set
            {
                _textConsole = value;
                OnPropertyChanged();
            }
        }
        public string SelectedUserPreset
        {
            get { return _selectedUserPreset; }
            set
            {
                _selectedUserPreset = value;
                OnPropertyChanged();
            }
        }
        public bool OnlyFree
        {
            get;
            set;
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
            UserProgramsSettingsHelper userProgramsSettingsHelper,
            UserPresetSettingsHelper userPresetSettingsHelper,
            UserPathSettingsHelper userPathSettingsHelper)
        {
            UserPresetPrograms = new ObservableCollection<UserPreset>();
            GroupProgramViewModel = new ObservableCollection<GroupProgramViewModel>();

            _messageDialogService = messageDialogService;
            _eventAggregator = eventAggregator;
            _userProgramsSettingsHelper = userProgramsSettingsHelper;
            _userPresetSettingsHelper = userPresetSettingsHelper;
            _userPathSettingsHelper = userPathSettingsHelper;
            _iOHelper = iOHelper;
            IsEnabled = true;


            SelectUserPresetCommand = new DelegateCommand(OnSelectUserPreset);
            InstallCommand = new DelegateCommand(OnInstall);
            OnlyFreeCommand = new DelegateCommand(OnOnlyFree);
            ClearConsoleCommand = new DelegateCommand(OnClearConsole);

            AUCCommand = new DelegateCommand(OnAUC);
            IconCommand = new DelegateCommand(OnIcon);
            OpenDesktopIconSettingsCommand = new DelegateCommand(OnOpenDesktopIconSettings);

            OpenAppLogCommand = new DelegateCommand(OnOpenAppLog);
            OpenExampleFileCommand = new DelegateCommand(OnOpenExampleFile);
            OpenKMSAutoCommand = new DelegateCommand(OpenKMSAuto);
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

            GroupProgramViewModel.Clear();
            UserPresetPrograms.Clear();

            _userPath = (UserPath)_userPathSettingsHelper.Settings.Clone();
            PathHelper.SetRootPath(_iOHelper.PathRoot, _userPath);

            var GroupsPrograms = (List<GroupPrograms>)_userProgramsSettingsHelper.Settings.GroupsPrograms?.Clone();
            foreach (var groupProgram in GroupsPrograms)
                GroupProgramViewModel.Add(new GroupProgramViewModel(groupProgram, new ObservableCollection<UserProgramViewModel>(groupProgram.UserPrograms.Select(x => new UserProgramViewModel(x, groupProgram, _eventAggregator)))));

            ProgramsHelper.CombinePathFolderAndImage(GroupProgramViewModel, _userPath);
            ProgramsHelper.CheckInstallPrograms(GroupProgramViewModel); // CheckInstall

            _userPresetSettingsHelper.Settings.UserPresets.ForEach(x => UserPresetPrograms.Add(x)); // Add UserPresetPrograms     
            var tempUserPrest = UserPresetPrograms.FirstOrDefault(x => x.Name.Contains(Regex.Match(WinInfoHelper.GetProductName(), @"\d+").Value, StringComparison.InvariantCultureIgnoreCase)); // heck UserPresetPrograms
            if (tempUserPrest != null)
                SelectedUserPreset = tempUserPrest.Name;
            OnSelectUserPreset();
            return Task.CompletedTask;
        }

        private void SelectSingleProgram(SelectSingleProgramEventArgs obj)
        {
            foreach (var userProgramViewModel in GroupProgramViewModel.Single(x => x.GroupProgram.Name == obj.NameGroup).UserProgramViewModels)
            {
                if (userProgramViewModel.UserProgram.ProgramName != obj.NameProgram)
                {
                    userProgramViewModel.NotSelectProgram();
                }
            }
        }
        public void Unsubscribe()
        {
            UserPresetPrograms.Clear();
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
        private void OnSelectUserPreset()
        {
            if (UserPresetPrograms.Count != 0)
            {
                var Preset = UserPresetPrograms.Single(x => x.Name == SelectedUserPreset);
                foreach (var program in GroupProgramViewModel.SelectMany(group => group.UserProgramViewModels))
                {
                    if (Preset.InstallProgramName.Contains(program.UserProgram.ProgramName))
                    {
                        if (OnlyFree == false)
                            program.SelectProgram();
                        else
                        {
                            if (program.UserProgram.IsFree == true)
                                program.SelectProgram();
                            else
                                program.NotSelectProgram();
                        }
                    }
                    else
                        program.NotSelectProgram();
                }
            }

        }
        private void OnOnlyFree()
        {
            OnSelectUserPreset();
        }
        //---------------------------------------------------------------------------------------------------
        private async void OnInstall()
        {
            var programsToInstall = GroupProgramViewModel.SelectMany(group => group.UserProgramViewModels).Where(x => x.Install == true);

            if (programsToInstall.Any(x => x.UserProgram.DisableDefender == true && !x.IsInstall)) // якщо програма потребує вимклення антивірусника для свого встановлення і вона ще не встановлена тоді
            {
                if (!WinDefenderHelper.CheckTamperProtectionDisable()) // Перевіряємо чи можемо ми вимкнути антивірусник
                {
                    _messageDialogService.ShowInfoDialog($"Нічого не буде, треба вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!!!\n" +
                        $"Оскільки встановлення одної з програм потребує автоматичного відключення ативірусного ПЗ!!!", "Помилка!");
                    return;
                }
            }
            IsEnabled = false;
            AddSplitter();
            foreach (var programToInstall in programsToInstall)
            {
                TextConsole += $"Start Install Programs: {programToInstall.UserProgram.ProgramName}\n";
                if (programToInstall.IsInstall) // перевіряємо чи програма встановлена
                {
                    TextConsole += $"Programs: {programToInstall.UserProgram.ProgramName}, Already installed!!!\n";
                    AddSplitter();
                    continue;
                }
                if (programToInstall.UserProgram.OnlineInstaller != null && await SpeedTest(true) >= 0.5) // якщо є онлайн інсталятор і інтернет
                    await InstallProgram(programToInstall, true); // встановлюємо онлайн 
                else
                    await InstallProgram(programToInstall, false); // встановлюємо офлайн
                AddSplitter();
            }
            IsEnabled = true;
        }
        private async Task InstallProgram(UserProgramViewModel userProgramViewMode, bool tryOnlineInstall)
        {
            var userProgram = userProgramViewMode.UserProgram;
            
            string arguments = null;
            int countInstall = 0; // лічильник спроб встановити програму (всього дві спроби)
            do // цикл встановлення програми (Дві спроби)
            {
                int countUnzipping = 0; // лічильник спроб розпакувати архів якщо інсталятор схавав антивірусник(одна спроба розпакувати на одну спробу встановити програму)
                string ExecutableFile = null;
                do // цикл пошуку файла (цікаво зроблено, він виконується або 0.5 або на 1.5 рази завдяки if (ExecutableFile != null)  break; та if (countUnzipping >= 1)  break;)
                {
                    if (userProgramViewMode.UserProgram.DisableDefender && !WinInfoHelper.GetProductName().Contains("7", StringComparison.InvariantCultureIgnoreCase)) // якщо треба вимкнути антивірусник windows 10 і це не windows 7
                    {
                        if (!await WinDefenderHelper.CheckDefenderDisable()) // якщо антивірусник увімкнутий
                        {
                            await OffDefender(true); // вимикаємо
                            await Task.Delay(100); // зачекаємо вимкнення
                        }
                        if (!await WinDefenderHelper.CheckDefenderDisable()) // якщо антивірусник досі увімкнутий
                        {
                            TextConsole += "Error: defender is not disabled\n"; // сповіщаємо про помилку
                            break; // виходимо з циклу (далі буде помилка в циклі встановлення)
                        }
                    }
                    if (tryOnlineInstall) // якщо є онлайн інсталятор намагаємось його знайти і задаємо аргумент онлайн інсталятора
                    {
                        ExecutableFile = ProgramsHelper.GetExeMsiFile(_iOHelper, userProgram.OnlineInstaller.FileName, userProgram.PathFolder).FirstOrDefault();
                        arguments = string.Join(" ", userProgram.OnlineInstaller.Arguments);
                    }
                    if (ExecutableFile == null) // якщо онлайн інсталятор не знайдено тоді шукаємо офлайн і задаємо аргументи офлайн інсталятора
                    {
                        var tempExecutableFile = ProgramsHelper.GetExeMsiFile(_iOHelper, userProgram.FileName, userProgram.PathFolder); // ортимуємо список файлів
                        if (WinInfoHelper.GetIs64BitOperatingSystem()) // якщо наша система х64
                            ExecutableFile = tempExecutableFile.FirstOrDefault(x => x.Contains("x64", StringComparison.InvariantCultureIgnoreCase)); // то намагаємось знайти файл, що містить х64 в назві
                        if (ExecutableFile == null) //якщо система не х64 або файла х64 нема
                            ExecutableFile = tempExecutableFile.LastOrDefault();  // обираємо те що є
                        arguments = string.Join(" ", userProgram.Arguments); 
                    }
                    if (ExecutableFile != null) // якщо інсталятор знайдено то 
                        break; // покидаємо цикл пошуку файлу (внутрішній) (далі буде спроба встановити програму) 
                    else // якщо онлайн і офлайн інсталятор не знайдено
                    {
                        TextConsole += $"Not found install file in folder: {userProgram.PathFolder}\n"; // сповіщаємо користувача, що файлу нема
                        if (countUnzipping >= 1) // якщо ми вже спробували його розпакувати еле його досі нема
                            break; // то покидаємо цикл пошуку файлу (далі буде помилка в циклі встановлення)
                        if (userProgram.DisableDefender) // якщо треба вимикати антивірусник і файла нема то намагаємось його добути з архіва
                        {
                            if (!File.Exists(_userPath.PortablePathSettings.RarPath)) // перевіряємо чи на місці архіватор
                            {
                                TextConsole += $"Error, Not Find Rar.exe\n"; // якщо його нема то сповіщаємо користувача
                                break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі встановлення)
                            }
                            var pathRar = Directory.GetFiles(userProgram.PathFolder).Where(x => x.Contains(userProgram.FileName, StringComparison.InvariantCultureIgnoreCase) && x.EndsWith(".rar")).FirstOrDefault(); // шукаємо файл архіва
                            TextConsole += $"Find arkhive {userProgram.FileName}, resault: "; // сповіщаємо користувача
                            if (string.IsNullOrWhiteSpace(pathRar)) // перевіряємо чи знайдено архів
                            {
                                TextConsole += $"Error, Not Find arkhive {userProgram.FileName}\n"; // якщо його нема то сповіщаємо користувача
                                break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі встановлення)
                            }
                            TextConsole += $"OK!!!, File: {Path.GetFileName(pathRar)}\n"; // яповіщаємо користувача, що все гаразд
                            try // весь try catch це спробу розпакувати архів
                            {
                                ProcessStartInfo ps = new ProcessStartInfo();
                                ps.FileName = _userPath.PortablePathSettings.RarPath;
                                ps.Arguments = $@"x -p1234 -o- {pathRar} {userProgram.PathFolder}";
                                TextConsole += $"Start unzipping, resault: ";
                                var proc = Process.Start(ps);
                                if (!proc.WaitForExit(20000))
                                {
                                    try { proc.Kill(); } catch (Exception) { }
                                    try { proc.Close(); } catch (Exception) { }

                                    TextConsole += "Time out unzipping\n";
                                }
                                else
                                    TextConsole += $"OK!!!\n";
                            }
                            catch (Exception)
                            {
                                TextConsole += "Error unzipping\n";
                            }
                            countUnzipping++; // лічильник спроб розпакувати архів
                        }
                        else // якщо антивірусник не треба вимикати і файла немає то значить помилився десь користувач
                            break; //тому покидаємо цикл пошуку файлу(далі буде помилка в циклі встановлення)
                    }
                } while (true);
                
                if (ExecutableFile == null) // якщо файл так і не знайдено 
                {
                    TextConsole += $"Programs: {userProgram.ProgramName}, not installed!!!\n"; //то сповіщаємо користувача про це
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
                            UseShellExecute = false
                        };
                        TextConsole += $"File exe: {ExecutableFile}\n";
                    }
                    TextConsole += $"Arguments: {StartInfo.Arguments}\n";

                    try // намагаємось встановити програму і очікуємо завершення її встановлення
                    {
                        Process proc = Process.Start(StartInfo);
                        await Task.Factory.StartNew(() => proc.WaitForExit());
                        TextConsole += $"Programs: {userProgram.ProgramName}, Installed!!!\n";
                        await Task.Delay(1000); // пауза для CheckInstall (щоб встиг оновитись реєстр)
                        userProgramViewMode.CheckInstall(WinInfoHelper.ListInstallPrograms()); // CheckInstall
                        break; // покидаємо цикл встановлення програми
                    }
                    catch (Exception exp)
                    {
                        TextConsole += $"Program: {userProgram.ProgramName}, error install: {exp.Message}\n";
                        if (!userProgram.DisableDefender) // якщо це програма яка не потребує вимкнення антивірусника і її встановелння викликало помилку тоді не пробуємо ставити її ще раз
                            break; // покидаємо цикл встановлення програми
                    }

                    countInstall++; // лічильник спроб встановити програму їх як сказано на початку буде лише дві
                    if (countInstall >= 2)
                        break;
                    TextConsole += $"<***************************************************************************>\n";
                }
            } while (true);
            if (userProgram.DisableDefender && await WinDefenderHelper.CheckDefenderDisable()) //якщо треба було вимкнути антивірусник  і він вимкнутий
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
            WinSettingsHelper.OpenDefaultPrograms();
        }
        private void OnOpenExampleFile()
        {
            _iOHelper.OpenFolder(_userPath.PathExampleFile);
        }
        private async void OpenKMSAuto()
        {
            if (!WinDefenderHelper.CheckTamperProtectionDisable()) // Перевіряємо чи можемо ми вимкнути антивірусник
            {
                _messageDialogService.ShowInfoDialog($"Потрібно вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!!!", "Помилка!"); // якщо ні то сповіщаємо користувача
                return; // виходимо
            }
            IsEnabled = false;
            AddSplitter();
            TextConsole += $"Start open KMSAuto\n";
            int countOpenKMSAuto = 0;
            do // цикл запуску KMSAuto (Дві спроби)
            {
                int countUnzipping = 0;
                string ExecutableFile = null;
                do // цикл пошуку файла (цікаво зроблено, він виконується або 0.5 або на 1.5 рази)
                {
                    if (!WinInfoHelper.GetProductName().Contains("7", StringComparison.InvariantCultureIgnoreCase)) // якщо треба вимкнути антивірусник windows 10 і це не windows 7
                    {
                        if (!await WinDefenderHelper.CheckDefenderDisable()) // якщо антивірусник увімкнутий
                        {
                            await OffDefender(true); // вимикаємо
                            await Task.Delay(100);
                        }
                        if (!await WinDefenderHelper.CheckDefenderDisable()) // якщо антивірусник досі увімкнутий
                        {
                            TextConsole += "Error: defender is not disabled\n";
                            break; // виходимо з циклу
                        }
                    }
                    if (File.Exists(_userPath.PortablePathSettings.KMSAutoPath)) // якщо KMSAuto є на свому місці
                    {
                        ExecutableFile = _userPath.PortablePathSettings.KMSAutoPath; // назначаємо його
                        break; // покидаємо цикл пошуку файлу (внутрішній) (далі буде спроба запустити KMSAuto) 
                    }
                    else // якщо файла нема
                    {
                        TextConsole += $"Not found KMSAuto\n"; // сповіщаємо користувача, що файлу нема
                        if (countUnzipping >= 1) // якщо ми вже спробували його розпакувати еле його досі нема
                            break; // то покидаємо цикл пошуку файлу(далі буде помилка в циклі встановлення)
                        if (!File.Exists(_userPath.PortablePathSettings.RarPath)) // перевіряємо чи на місці архіватор
                        {
                            TextConsole += $"Error, Not Find Rar.exe\n"; // якщо його нема то сповіщаємо користувача
                            break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі запуска KMSAuto)
                        }
                        TextConsole += $"Find arkhive {Path.GetFileName(_userPath.PortablePathSettings.KMSAutoRarPath)}, resault: ";
                        if (!File.Exists(_userPath.PortablePathSettings.KMSAutoRarPath)) // перевіряємо чи знайдено архів
                        {
                            TextConsole += $"Error, not Find arkhive!\n"; // якщо його нема то сповіщаємо користувача
                            break; // зупиняємо спробу розпакувати архіві виходимо з циклу пошуку файлу (далі буде помилка в циклі запуска KMSAuto)
                        }
                        TextConsole += $"OK!\n";
                        try // весь try catch це спробу розпакувати архів
                        {
                            ProcessStartInfo ps = new ProcessStartInfo();
                            ps.FileName = _userPath.PortablePathSettings.RarPath;
                            ps.Arguments = $@"x -p1234 -o- {_userPath.PortablePathSettings.KMSAutoRarPath} {Path.GetDirectoryName(_userPath.PortablePathSettings.KMSAutoRarPath)}";
                            TextConsole += $"Start unzipping, resault: ";
                            var proc = Process.Start(ps);
                            if (!proc.WaitForExit(20000))
                            {
                                try { proc.Kill(); } catch (Exception) { }
                                try { proc.Close(); } catch (Exception) { }

                                TextConsole += "Time out unzipping\n";
                            }
                            else
                                TextConsole += $"OK!!!\n";
                        }
                        catch (Exception)
                        {
                            TextConsole += "Error unzipping\n";
                        }
                        countUnzipping++; // лічильник спроб розпакувати архів
                    }
                } while (true);
                if (ExecutableFile == null) // якщо KMSAuto так і не знайдено то
                {
                    TextConsole += $"KMSAuto is not open!!!\n"; //сповіщаємо користувача про це (далі ще раз спробуємо знайти файл якщо не вичерпано ліміт спроб)
                }
                else // якщо KMSAuto знайдено
                {
                    var StartInfo = new ProcessStartInfo // ProcessStartInfo
                    {
                        FileName = ExecutableFile,
                        UseShellExecute = false,
                    };
                    try // спроба запустити
                    {
                        Process proc = Process.Start(StartInfo);
                        TextConsole += $"KMSAuto open\n";
                        AddSplitter();
                        await Task.Run(() => proc.WaitForExit()); // очыкуэмо завершення роботи програми
                        break; // покидаэмо цикл запуску програми
                    }
                    catch (Exception exp)
                    {
                        TextConsole += $"KMSAuto error open: {exp.Message}\n"; // якщо сталась помилка сповыщаэмо про це
                        if (countOpenKMSAuto + 1 < 2)
                        {
                            TextConsole += $"Pause 20 seconds...\n";
                            await Task.Delay(20000); // пауза для того щоб антивірусник "відпустив" файл
                        }
                    }
                }
                countOpenKMSAuto++; // збільшуємо лічильник спроб запуску
                if (countOpenKMSAuto >= 2) // перевіряємо кількість спроб запуску
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
            await OnDefender(false);
        }
        private async Task OnDefender(bool OnInstall) // ✓
        {
            if (!OnInstall)
            {
                if (!WinDefenderHelper.CheckTamperProtectionDisable())
                {
                    _messageDialogService.ShowInfoDialog($"Нічого не буде, треба вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!!!", "Помилка!");
                    return;
                }
                IsEnabled = false;
                AddSplitter();
            }

            TextConsole += $"Start on defender: \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            var ResponseDefender = (await WinDefenderHelper.EnableRealtimeMonitoring()).Trim();
            if (!string.IsNullOrWhiteSpace(ResponseDefender))
                TextConsole += $"Response defender: {ResponseDefender}\n";
            TextConsole += $"End on defender:  \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            TextConsole += $"Defender is disable: {await WinDefenderHelper.CheckDefenderDisable()}\n";
            if (!OnInstall)
            {
                AddSplitter();
                IsEnabled = true;
            }
        }
        private async void OnOffDefender() // ✓
        {
            await OffDefender(false);
        }
        private async Task OffDefender(bool OnInstall) // ✓
        {
            if (!OnInstall)
            {
                if (!WinDefenderHelper.CheckTamperProtectionDisable())
                {
                    _messageDialogService.ShowInfoDialog($"Нічого не буде, треба вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!!!", "Помилка!");
                    return;
                }
                IsEnabled = false;
                AddSplitter();
            }
            TextConsole += $"Start off defender: \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            var ResponseDefender = (await WinDefenderHelper.DisableRealtimeMonitoring()).Trim();
            if (!string.IsNullOrWhiteSpace(ResponseDefender))
                TextConsole += $"Response defender: {ResponseDefender}\n";
            TextConsole += $"End off defender:  \t{DateTime.Now:dd/MM hh:mm:ss}\n";
            TextConsole += $"Defender is disable: {await WinDefenderHelper.CheckDefenderDisable()}\n";
            if (!OnInstall)
            {
                AddSplitter();
                IsEnabled = true;
            }
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
        public ICommand SelectUserPresetCommand { get; }
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
        public ICommand OpenKMSAutoCommand { get; }
        public ICommand OpenAppLogCommand { get; }
        //****************************************************
        public ICommand SpeedTestCommand { get; }
        public ICommand OffDefenderCommand { get; }
        public ICommand OnDefenderCommand { get; }

        //--------------------------------------
    }
}
