using CompasPack;
using CompasPack.BL;
using CompasPack.Data;
using CompasPack.Event;
using CompasPack.View.Service;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CompasPakc.BL;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CompasPack.ViewModel
{
    public class ProgramsViewModel : ViewModelBase, IDetailViewModel
    {
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private readonly IIOManager _iOManager;
        private int _selectedUserPreset;
        private string _textConsole;
        private bool _isEnabled;
        SubscriptionToken _token;
        public ProgramsViewModel(IMessageDialogService messageDialogService, IIOManager iOManager, IEventAggregator eventAggregator)
        {
            UserPresetPrograms = new ObservableCollection<UserPresetProgram>();
            GroupProgramViewModel = new ObservableCollection<GroupProgramViewModel>();

            _messageDialogService = messageDialogService;
            _eventAggregator = eventAggregator;
            _selectedUserPreset = -1;
            _iOManager = iOManager;
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

            _token = _eventAggregator.GetEvent<SelectSingleProgramEvent>().Subscribe(SelectSingleProgram);
        }

        public ObservableCollection<UserPresetProgram> UserPresetPrograms { get; }
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
        public int SelectedUserPreset
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

        //*****************************************************************************************
        public async Task LoadAsync(int? Id)
        {
            TextConsole = WinInfo.GetSystemInfo();

            GroupProgramViewModel.Clear();
            UserPresetPrograms.Clear();

            var lookupProgramGroups = await _iOManager.GetGroupPrograms();
            var lookupPresetProgram = await _iOManager.GetUserPresetProgram();

            foreach (var groupProgram in lookupProgramGroups)
            {
                var temp = from x in groupProgram.UserPrograms select new UserProgramViewModel(x, groupProgram, _eventAggregator);
                GroupProgramViewModel
                    .Add(new GroupProgramViewModel(groupProgram, new ObservableCollection<UserProgramViewModel>(temp), _eventAggregator));
            }

            var temoListPrograms = WinInfo.ListInstallPrograms();
            foreach (var item in GroupProgramViewModel.SelectMany(group => group.UserProgramViewModels))
                item.CheckInstall(temoListPrograms);



            foreach (var preset in lookupPresetProgram)
                UserPresetPrograms.Add(preset);


            var TempProduscName = WinInfo.GetProductName();
            if (TempProduscName.Contains("Windows 10", StringComparison.InvariantCultureIgnoreCase))
            {
                var tempUserPrest = UserPresetPrograms.FirstOrDefault(x => x.Name.Contains("Windows 10", StringComparison.InvariantCultureIgnoreCase));
                if (tempUserPrest != null)
                    SelectedUserPreset = tempUserPrest.Id;
            }
            else if (TempProduscName.Contains("Windows 7", StringComparison.InvariantCultureIgnoreCase))
            {
                var tempUserPrest = UserPresetPrograms.FirstOrDefault(x => x.Name.Contains("Windows 7", StringComparison.InvariantCultureIgnoreCase));
                if (tempUserPrest != null)
                    SelectedUserPreset = tempUserPrest.Id;
            }


        }
        private void SelectSingleProgram(SelectSingleProgramEventArgs obj)
        {
            foreach (var userProgramViewModel in GroupProgramViewModel.Single(x => x.GroupProgram.Id == obj.IdGroup).UserProgramViewModels)
            {
                if (userProgramViewModel.UserProgram.Id != obj.IdProgram)
                {
                    userProgramViewModel.NotSelectProgram();
                }
            }
        }

        public void Unsubscribe()
        {
            //_eventAggregator.GetEvent<SelectSingleProgramEvent>().Unsubscribe(_token);
            UserPresetPrograms.Clear();
        }

        public bool HasChanges()
        {
            throw new NotImplementedException();
        }
        //*****************************************************************************************

        //--------------------------------------
        private void OnSelectUserPreset()
        {
            if (UserPresetPrograms.Count != 0)
            {
                var Preset = UserPresetPrograms.Single(x => x.Id == SelectedUserPreset);
                foreach (var program in GroupProgramViewModel.SelectMany(group => group.UserProgramViewModels))
                {
                    if (Preset.IdPrograms.Contains(program.UserProgram.Id))
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
        private async void OnInstall()
        { 
            var userPrograms = GroupProgramViewModel.SelectMany(group => group.UserProgramViewModels).Where(x => x.Install == true);
            var t = userPrograms.FirstOrDefault().IsInstall.ToString();
            if (userPrograms.Any(x => x.UserProgram.DisableDefender == true && x.IsInstall.ToString() == "#FFFF0000"))
            {
                if (!WinDefender.CheckTamperProtectionDisable())
                {
                    _messageDialogService.ShowInfoDialog($"Нічого не буде, треба вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!!!\n" +
                        $"Оскільки встановлення одної з програм потребує автоматичного відключення ативірусного ПЗ!!!", "Помилка!");
                    return;
                }
            }
            IsEnabled = false;
            TextConsole += "<--------------------Start Install----------------------->\n";
            foreach (var userProgramViewModel in userPrograms)
            {
                if (WinInfo.IsInstallPrograms(WinInfo.ListInstallPrograms(), userProgramViewModel.UserProgram.InstallProgramName))
                {
                    TextConsole += $"Programs: {userProgramViewModel.UserProgram.ProgramName}, Already Installed!!!\n";
                    TextConsole += "<-------------------------------------------------------->\n";
                }
                else
                {
                    TextConsole += $"Start Install Programs: {userProgramViewModel.UserProgram.ProgramName}\n";
                    if (userProgramViewModel.UserProgram.OnlineInstaller != null)
                    {
                        TextConsole += $"Start speed test: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                        var speed = await Network.SpeedTest();
                        TextConsole += $"End speed test: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                        TextConsole += $"Speed: {Math.Round(speed, 2)} Mbyte/s\n";

                        if (speed >= 0.5)
                            await InstallProgram(userProgramViewModel, true);
                        else
                            await InstallProgram(userProgramViewModel, false);
                    }
                    else
                    {
                         await InstallProgram(userProgramViewModel, false);
                    }
                    TextConsole += "<-------------------------------------------------------->\n";
                }

            }
            IsEnabled = true;
        }
        private async Task InstallProgram(UserProgramViewModel userProgramViewMode, bool onlineInstall)
        {
            var userProgram = userProgramViewMode.UserProgram;
            string? ExecutableFile = null;
            string? arguments = null;
            int countOpen = 0;
            a:
            if (userProgramViewMode.UserProgram.DisableDefender && !WinInfo.GetProductName().Contains("Windows 7", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!await WinDefender.CheckDefenderDisable())
                {
                    TextConsole += $"Start off defender: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                    var ResponseDefender = (await WinDefender.DisableRealtimeMonitoring()).Trim();
                    if (!string.IsNullOrWhiteSpace(ResponseDefender))
                        TextConsole += $"Response defender: {ResponseDefender}\n";
                    TextConsole += $"End off defender:  \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                }
                var Defender = await WinDefender.CheckDefenderDisable();
                TextConsole += $"Defender is disable: {Defender}\n";
                if (!Defender)
                {
                    TextConsole += "Error defender is disable\n";
                    return;
                }
            }
            if (onlineInstall)
            {
                ExecutableFile = Directory.GetFiles(userProgram.PathFolder)
               .Where(x => x.Contains(userProgram.OnlineInstaller.FileName, StringComparison.InvariantCultureIgnoreCase))
               .Where(x => x.Contains("exe", StringComparison.InvariantCultureIgnoreCase) || x.Contains("msi", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                arguments = string.Join(" ", userProgram.OnlineInstaller.Arguments);
            }
            if (ExecutableFile == null)
            {
                var Files = Directory.GetFiles(userProgram.PathFolder)
                   .Where(x => x.Contains(userProgram.FileName, StringComparison.InvariantCultureIgnoreCase))
                   .Where(x => x.Contains("exe", StringComparison.InvariantCultureIgnoreCase) || x.Contains("msi", StringComparison.InvariantCultureIgnoreCase));
                if (userProgram.Architecture == "x64")
                {
                    if (WinInfo.GetIs64BitOperatingSystem())
                        ExecutableFile = Files.Where(x => x.Contains("x64", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    else
                        ExecutableFile = Files.Where(x => x.Contains("x86", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                }
                else
                    ExecutableFile = Files.LastOrDefault();
                arguments = String.Join(" ", userProgram.Arguments);
            }

            if (userProgram.DisableDefender)
                TextConsole += $"Find {userProgram.FileName} (Try {countOpen + 1} with 3), Resault:\n";
            
            if (ExecutableFile != null)
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
                    await proc.WaitForExitAsync();
                    TextConsole += $"Programs: {userProgram.ProgramName}, Installed!!!\n";
                    await Task.Delay(1000);
                    userProgramViewMode.CheckInstall(WinInfo.ListInstallPrograms());
                }
                catch (Exception exp)
                {
                    TextConsole += $"Program: {userProgram.ProgramName}.\nError install: \n{exp.Message}\n";
                }
            }
            else
            {
                if (userProgram.DisableDefender)
                {
                    TextConsole += $"Error, Not Find {userProgram.FileName}\n";
                    TextConsole += $"Find Rar.exe, Resault:\n";
                    
                    if (File.Exists(_iOManager.WinRar))
                    {
                        TextConsole += $"OK!!!, Path: {_iOManager.WinRar}\n";
                        var pathRar = Directory.GetFiles(userProgram.PathFolder).Where(x => x.Contains(userProgram.FileName, StringComparison.InvariantCultureIgnoreCase) && x.EndsWith(".rar")).FirstOrDefault();
                        int countUnrar = 0;
                        TextConsole += $"Find arkhive {userProgram.FileName}, Resault:\n";
                        if (!string.IsNullOrWhiteSpace(pathRar))
                        {
                            TextConsole += $"OK!!!, Path: {pathRar}\n";
                        b:
                            try
                            {
                                ProcessStartInfo ps = new ProcessStartInfo();
                                ps.FileName = _iOManager.WinRar;
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
                            if (countOpen < 2)
                            {
                                countOpen++;
                                TextConsole += "********************************************************\n";
                                goto a;
                            }
                        }
                        else
                        {
                            TextConsole += $"Error, Not Find arkhive {userProgram.FileName}\n";
                        }
                    }
                    else
                    {
                        TextConsole += $"Error, Not Find Rar.exe\n";
                    }
                }
                 else
                    TextConsole += $"Not fount file: {userProgram.FileName} In folder: {userProgram.PathFolder}\n";
            }
        }
        private void OnClearConsole()
        {
            TextConsole = WinInfo.GetSystemInfo();
        }
        //-------------------------------------

        //--------------------------------------
        private void OnAUC()
        {
            WinSettings.OpenAUC();
        }
        private void OnIcon()
        {
            WinSettings.OpenIcon();
        }
        private void OnOpenDesktopIconSettings()
        {
            WinSettings.OpenDesktopIconSettings();
        }
        //**************************************************
        private void OnDefault()
        {
            WinSettings.OpenDefaultPrograms();
        }
        private void OnOpenExampleFile()
        {
            _iOManager.OpenFolder(_iOManager.CompasExampleFile);
        }
        private async void OpenKMSAuto()
        {
            if (!WinDefender.CheckTamperProtectionDisable())
            {
                _messageDialogService.ShowInfoDialog($"Нічого не буде, треба вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!!!", "Помилка!");
                return;
            }
            //------------------------------------------------------------------------------------------------------
            TextConsole += "<-----------------Start open KMSAuto-------------------->\n";

            int countOpenKMSAuto = 0;
            a:
            if (!await WinDefender.CheckDefenderDisable())
            {
                TextConsole += $"Start off defender: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                var ResponseDefender = (await WinDefender.DisableRealtimeMonitoring()).Trim();
                if (!string.IsNullOrWhiteSpace(ResponseDefender))
                    TextConsole += $"Response defender: {ResponseDefender}\n";
                TextConsole += $"End off defender:  \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
            }
            var Defender = await WinDefender.CheckDefenderDisable();
            TextConsole += $"Defender is disable: {Defender}\n";
            if (!Defender)
            {
                TextConsole += "<-----------------End open KMSAuto--------------------->\n";
                return;
            }
            //------------------------------------------------------------------------------------------------------
            var pathKMS = KMSAuto.FindKMSAutoExe(_iOManager);
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
                if (File.Exists(_iOManager.WinRar))
                {
                    TextConsole += $"OK!!!, Path: {_iOManager.WinRar}\n";
                    var pathRar = KMSAuto.FindKMSAutoRar(_iOManager);
                    int countUnrar = 0;
                    TextConsole += $"Find arkhive KMSAuto, Resault:\n";
                    if (!string.IsNullOrWhiteSpace(pathRar))
                    {
                        TextConsole += $"OK!!!, Path: {pathRar}\n";
                    b:
                        try
                        {
                            ProcessStartInfo ps = new ProcessStartInfo();
                            ps.FileName = _iOManager.WinRar;
                            ps.Arguments = $@"x -p1234 -o- {pathRar} {_iOManager.Crack}";
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
            _iOManager.OpenFolder(_iOManager.CompasPackLog);
        }
        //***************************************************
        private async void OnSpeedTest()
        {
            TextConsole += "<-----------------Start test speed--------------------->\n";
            TextConsole += $"Start test: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
            IsEnabled = false;
            var speed = await Network.SpeedTest();
            TextConsole += $"End test: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
            TextConsole += $"Speed: {Math.Round(speed, 2)} Mbyte/s\n";
            TextConsole += "<------------------End test speed---------------------->\n";
            IsEnabled = true;
        }
        private async void OnOffDefender()
        {
            if (WinDefender.CheckTamperProtectionDisable())
            {
                IsEnabled = false;
                TextConsole += "<----------------Start off defender-------------------->\n";
                TextConsole += $"Start off defender: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                var ResponseDefender = (await WinDefender.DisableRealtimeMonitoring()).Trim();
                if (!string.IsNullOrWhiteSpace(ResponseDefender))
                    TextConsole += $"Response defender: {ResponseDefender}\n";
                TextConsole += $"End off defender:  \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                TextConsole += $"Defender is disable: {await WinDefender.CheckDefenderDisable()}\n";
                TextConsole += "<-----------------End off defender--------------------->\n";
                IsEnabled = true;
            }
            else
            {
                _messageDialogService.ShowInfoDialog($"Нічого не буде, треба вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!!!", "Помилка!");
            }
        }
        private async void OnOnDefender()
        {
            if (WinDefender.CheckTamperProtectionDisable())
            {
                IsEnabled = false;
                TextConsole += "<-----------------Start on defender-------------------->\n";
                TextConsole += $"Start on defender: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                var ResponseDefender = (await WinDefender.EnableRealtimeMonitoring()).Trim();
                if (!string.IsNullOrWhiteSpace(ResponseDefender))
                    TextConsole += $"Response defender: {ResponseDefender}\n";
                TextConsole += $"End on defender:  \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                TextConsole += $"Defender is disable: {await WinDefender.CheckDefenderDisable()}\n";
                TextConsole += "<------------------End on defender--------------------->\n";
                IsEnabled = true;
            }
            else
            {
                _messageDialogService.ShowInfoDialog($"Нічого не буде, треба вимкнути: \"Захист від підробок\" в налаштуваннях Windows Defender!!!", "Помилка!");
            }
        }

        //--------------------------------------


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
