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
        public string TextConsole { get; set; }
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
            string? ExecutableFile = null;
            string? arguments = null;
            int countOpen = 0;
            do
            {
                if (userProgramViewMode.UserProgram.DisableDefender && !WinInfoHelper.GetProductName().Contains("7", StringComparison.InvariantCultureIgnoreCase)) // якщо треба вимкнути антивірусник windows 10 і це windows 10 
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

                if (tryOnlineInstall) // якщо є онлайн інсталятор намагаємось його знайти
                {
                    ExecutableFile = ProgramsHelper.GetExeMsiFile(_iOHelper, userProgram.OnlineInstaller.FileName, userProgram.PathFolder).FirstOrDefault();
                    arguments = string.Join(" ", userProgram.OnlineInstaller.Arguments);
                }

                if (ExecutableFile == null) // якщо онлайн інсталятор не знайдено тоді шукаємо офлайн
                {
                    var tempExecutableFile = ProgramsHelper.GetExeMsiFile(_iOHelper, userProgram.FileName, userProgram.PathFolder);
                    if (userProgram.Architecture == "x64")
                    {
                        if (WinInfoHelper.GetIs64BitOperatingSystem())
                            ExecutableFile = tempExecutableFile.Where(x => x.Contains("x64", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        else
                            ExecutableFile = tempExecutableFile.Where(x => x.Contains("x86", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    }
                    else
                        ExecutableFile = tempExecutableFile.LastOrDefault();
                    arguments = string.Join(" ", userProgram.Arguments);
                }

                if (ExecutableFile == null) // якщо онлайн і офлайн інсталятор не знайдено
                {
                    TextConsole += $"Not found file: {userProgram.FileName} In folder: {userProgram.PathFolder}\n"; // сповіщаємо користувача, що файлу нема
                    if (userProgram.DisableDefender) // якщо треба вимикати антивірусник і файла нема то намагаємось його добути з архіва і попереджаємо користувача      
                    {
                        TextConsole += $" {userProgram.FileName} (Try {countOpen + 1} with 3), Resault:\n";
                        TextConsole += $"Error, Not Find {userProgram.FileName}\n"; // а тут 
                        TextConsole += $"Find Rar.exe, Resault:\n";
                        var RarPath = _userPathSettingsHelper.Settings.PortablePathSettings.RarPath;
                        if (!File.Exists(RarPath))
                        {
                            TextConsole += $"Error, Not Find Rar.exe\n";
                            return;
                        }
                        TextConsole += $"OK!!!, Path: {RarPath}\n";
                        var pathRar = Directory.GetFiles(userProgram.PathFolder).Where(x => x.Contains(userProgram.FileName, StringComparison.InvariantCultureIgnoreCase) && x.EndsWith(".rar")).FirstOrDefault();
                        int countUnrar = 0;
                        TextConsole += $"Find arkhive {userProgram.FileName}, Resault:\n";
                        if (string.IsNullOrWhiteSpace(pathRar))
                        {
                            TextConsole += $"Error, Not Find arkhive {userProgram.FileName}\n";
                        }
                        TextConsole += $"OK!!!, Path: {pathRar}\n";
                    b:
                        try
                        {
                            ProcessStartInfo ps = new ProcessStartInfo();
                            ps.FileName = pathRar;
                            ps.Arguments = $@"x -p1234 -o- {pathRar} {userProgram.PathFolder}";
                            TextConsole += $"Start UnRar with Args (Try {countUnrar + 1} with 3):\n{ps.Arguments}, Resault:\n";
                            var proc = Process.Start(ps);
                            if (!proc.WaitForExit(20000))
                            {
                                try { proc.Kill(); } catch (Exception) { }
                                try { proc.Close(); } catch (Exception) { }

                                TextConsole += "Error UnRar\n";
                                if (countUnrar < 2)
                                {
                                    countUnrar++;
                                    await Task.Delay(5000);
                                    goto b;
                                }
                            }
                            else
                            {
                                TextConsole += $"OK!!!\n";
                            }
                        }
                        catch (Exception)
                        {
                            TextConsole += "Error UnRar\n";
                        }
                        countOpen++;
                        TextConsole += "********************************************************\n";
                        continue;
                    }
                }
            } while (countOpen < 2);
            
            if (ExecutableFile == null) // якщо файл так і не знайдено то сповіщаємо користувача про це
            {
                TextConsole += $"Programs: {userProgram.ProgramName}, Not installed!!!\n";
            }
            else
            {
                if (userProgram.DisableDefender)
                    TextConsole += $"OK!!!, Find File and start Install: {ExecutableFile}\n";
                else
                    TextConsole += $"File: {ExecutableFile}\n";

                ProcessStartInfo? StartInfo = null;
                if (ExecutableFile.EndsWith(".msi"))
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "msiexec",
                        Arguments = $"/i {ExecutableFile} {arguments}",
                        UseShellExecute = false
                    };
                    TextConsole += $"Install MSI File!!!\n";
                }
                else
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ExecutableFile,
                        Arguments = arguments,
                        UseShellExecute = false
                    };
                }
                TextConsole += $"Arguments: {StartInfo.Arguments}\n";
                try
                {
                    Process proc = Process.Start(StartInfo);
                    //await proc.WaitForExitAsync();
                    TextConsole += $"Programs: {userProgram.ProgramName}, Installed!!!\n";
                    await Task.Delay(1000);
                    userProgramViewMode.CheckInstall(WinInfoHelper.ListInstallPrograms());
                }
                catch (Exception exp)
                {
                    TextConsole += $"Program: {userProgram.ProgramName}.\nError install: \n{exp.Message}\n";
                }
            }

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
            if (!WinDefenderHelper.CheckTamperProtectionDisable())
            {
                _messageDialogService.ShowInfoDialog($"Нічого не буде, треба вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!!!", "Помилка!");
                return;
            }
            //------------------------------------------------------------------------------------------------------
            TextConsole += "<-----------------Start open KMSAuto-------------------->\n";

            int countOpenKMSAuto = 0;
        a:
            if (!await WinDefenderHelper.CheckDefenderDisable())
            {
                TextConsole += $"Start off defender: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                var ResponseDefender = (await WinDefenderHelper.DisableRealtimeMonitoring()).Trim();
                if (!string.IsNullOrWhiteSpace(ResponseDefender))
                    TextConsole += $"Response defender: {ResponseDefender}\n";
                TextConsole += $"End off defender:  \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
            }
            var Defender = await WinDefenderHelper.CheckDefenderDisable();
            TextConsole += $"Defender is disable: {Defender}\n";
            if (!Defender)
            {
                TextConsole += "<-----------------End open KMSAuto--------------------->\n";
                return;
            }
            //------------------------------------------------------------------------------------------------------
            var pathKMS = _userPathSettingsHelper.Settings.PortablePathSettings.KMSAutoPath;
            TextConsole += $"Find KMSAuto (Try {countOpenKMSAuto + 1} with 3), Resault:\n";
            if (!string.IsNullOrWhiteSpace(pathKMS))
            {
                TextConsole += $"OK!!!, Path: {pathKMS}\n";
                Process proc = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = pathKMS,
                        UseShellExecute = false,
                    }
                };
                proc.Start();
            }
            else
            {
                TextConsole += $"Error, Not Find KMSAuto\n";
                TextConsole += $"Find Rar.exe, Resault:\n";
                var RarPath = _userPathSettingsHelper.Settings.PortablePathSettings.RarPath;
                if (File.Exists(RarPath))
                {
                    TextConsole += $"OK!!!, Path: {_userPathSettingsHelper.Settings.PortablePathSettings.RarPath}\n";
                    var KMSAutoRarPath = _userPathSettingsHelper.Settings.PortablePathSettings.KMSAutoRarPath;
                    int countUnrar = 0;
                    TextConsole += $"Find arkhive KMSAuto, Resault:\n";
                    if (!string.IsNullOrWhiteSpace(KMSAutoRarPath))
                    {
                        TextConsole += $"OK!!!, Path: {KMSAutoRarPath}\n";
                    b:
                        try
                        {
                            ProcessStartInfo ps = new ProcessStartInfo();
                            ps.FileName = RarPath;
                            //ps.Arguments = $@"x -p1234 -o- {KMSAutoRarPath} {_iOHelper.Crack}";
                            //TODO FIX!!!!!!!!!!!!
                            TextConsole += $"Start UnRar with Args (Try {countUnrar + 1} with 3):\n{ps.Arguments}, Resault:\n";
                            var proc = Process.Start(ps);
                            if (!proc.WaitForExit(20000))
                            {
                                try { proc.Kill(); } catch (Exception) { }
                                try { proc.Close(); } catch (Exception) { }

                                TextConsole += "Error UnRar\n";
                                if (countUnrar < 2)
                                {
                                    countUnrar++;
                                    await Task.Delay(5000);
                                    goto b;
                                }
                            }
                            else
                            {
                                TextConsole += $"OK!!!\n";
                            }
                        }
                        catch (Exception)
                        {
                            TextConsole += "Error UnRar\n";
                        }
                        if (countOpenKMSAuto < 2)
                        {
                            countOpenKMSAuto++;
                            TextConsole += "********************************************************\n";
                            goto a;
                        }
                    }
                    else
                    {
                        TextConsole += $"Error, Not Find arkhive KMSAuto\n";
                    }
                }
                else
                {
                    TextConsole += $"Error, Not Find Rar.exe\n";
                }
            }
            TextConsole += "<-----------------End open KMSAuto--------------------->\n";
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
                if (!strings[strings.Length - 2].Contains("<-------------------------------------------------------------------->"))
                    TextConsole += "<-------------------------------------------------------------------->\n";
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
