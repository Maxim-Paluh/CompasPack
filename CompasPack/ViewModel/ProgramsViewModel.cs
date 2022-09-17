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
            DefaultCommand = new DelegateCommand(OnDefault);

            SpeedTestCommand = new DelegateCommand(OnSpeedTest);
            OffDefenderCommand = new DelegateCommand(OnOffDefender);
            OnDefenderCommand = new DelegateCommand(OnOnDefender);

            OpenAppLogCommand = new DelegateCommand(OnOpenAppLog);
            OpenExampleFileCommand = new DelegateCommand(OnOpenExampleFile);



            _eventAggregator.GetEvent<SelectSingleProgramEvent>().Subscribe(SelectSingleProgram);
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
            _eventAggregator.GetEvent<SelectSingleProgramEvent>().Unsubscribe(SelectSingleProgram);
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
            TextConsole += "<--------------------Start Install----------------------->\n";
            IsEnabled = false;
            var userPrograms = GroupProgramViewModel.SelectMany(group => group.UserProgramViewModels).Where(x => x.Install == true);

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
                    if (userProgramViewModel.UserProgram.DisableDefender)
                    {
                        TextConsole += $"Start off defender: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                        var ResponseDefender = (await WinDefender.DisableRealtimeMonitoring()).Trim();
                        if (!string.IsNullOrWhiteSpace(ResponseDefender))
                            TextConsole += $"Response defender: {ResponseDefender}\n";
                        TextConsole += $"End off defender:  \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                        TextConsole += $"Resault: {await WinDefender.CheckDefenderDisable()}\n";
                    }

                    if (userProgramViewModel.UserProgram.OnlineInstaller != null)
                    {
                        TextConsole += $"Start speed test: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                        var speed = await Network.SpeedTest();
                        TextConsole += $"End speed test: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
                        TextConsole += $"Speed: {Math.Round(speed, 2)} Mbyte/s\n";

                        if (speed >= 0.5)
                            TextConsole += await Installer.InstallProgram(userProgramViewModel, true);
                        else
                            TextConsole += await Installer.InstallProgram(userProgramViewModel, false);
                    }
                    else
                    {
                        TextConsole += await Installer.InstallProgram(userProgramViewModel, false);
                    }
                    TextConsole += "<-------------------------------------------------------->\n";
                }

            }
            IsEnabled = true;
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
        private void OnDefault()
        {
            WinSettings.OpenDefaultPrograms();
        }
        private void OnOpenAppLog()
        {
            _iOManager.OpenAppLog();
        }
        private void OnOpenExampleFile()
        {
            _iOManager.OpenExampleFile();
        }
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
            TextConsole += "<----------------Start off defender-------------------->\n";
            TextConsole += $"Start off defender: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
            IsEnabled = false;
            var ResponseDefender = (await WinDefender.DisableRealtimeMonitoring()).Trim();
            if (!string.IsNullOrWhiteSpace(ResponseDefender))
                TextConsole += $"Response defender: {ResponseDefender}\n";
            TextConsole += $"End off defender:  \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
            TextConsole += $"Resault: {await WinDefender.CheckDefenderDisable()}\n";
            TextConsole += "<-----------------End off defender--------------------->\n";
            IsEnabled = true;
        }
        private async void OnOnDefender()
        {
            TextConsole += "<-----------------Start on defender-------------------->\n";
            TextConsole += $"Start on defender: \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
            IsEnabled = false;
            var ResponseDefender = (await WinDefender.EnableRealtimeMonitoring()).Trim();
            if (!string.IsNullOrWhiteSpace(ResponseDefender))
                TextConsole += $"Response defender: {ResponseDefender}\n";
            TextConsole += $"End on defender:  \t{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")}\n";
            TextConsole += $"Resault: {!(await WinDefender.CheckDefenderDisable())}\n";
            TextConsole += "<------------------End on defender--------------------->\n";
            IsEnabled = true;
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
        public ICommand DefaultCommand { get; }
        public ICommand OpenAppLogCommand { get; }
        public ICommand OpenExampleFileCommand { get; }
        public ICommand SpeedTestCommand { get; }
        public ICommand OffDefenderCommand { get; }
        public ICommand OnDefenderCommand { get; }
        //--------------------------------------
    }
}
