using CompasPack.Data.Providers;
using CompasPack.Data.Providers.API;
using CompasPack.Helper.Extension;
using CompasPack.Helper.Service;
using CompasPack.Model.Enum;
using CompasPack.Model.Entities.Programs;
using CompasPack.Model.Wrapper;
using CompasPack.Model.Support;
using Prism.Commands;
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
    public class ProgramsViewModel : ViewModelBase, IViewModel
    {
        #region Properties
        private ProgramsPaths _programsPaths;

        private readonly IProgramsService _programsService;
        private readonly WinInfo _winInfo;
        private readonly IEnumerable<IAntivirus> _antiviruses;
        private readonly IMessageDialogService _messageDialogService;
        private readonly IFileSystemReaderWriter _fileSystemReaderWriter;
        private readonly IFileSystemNavigator _fileSystemNavigator;
        private readonly IFileArchiver _fileArchiver;
        private readonly ProgramsSettingsProvider _programsSettingsProvider;
        private readonly IWinSettingsLauncher _winSettingsLauncher;

        public ObservableCollection<ProgramsSet> ProgramsSets { get; }
        public ObservableCollection<GroupProgramsWrapper> GroupProgramsWrappers { get; }
        public ObservableCollection<ProtectedProgram> ProtectedPrograms { get; }
        public IConsoleBuffer ConsoleBuffer { get; private set; }

        private string _selectedProgramsSets;
        private bool _isEnabled;
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
        #endregion

        #region Constructors
        public ProgramsViewModel(IProgramsService programsService,
                                 WinInfo winInfo, IEnumerable<IAntivirus> antiviruses, 
                                 IConsoleBuffer consoleBuffer, IMessageDialogService messageDialogService, 
                                 IFileSystemReaderWriter fileSystemReaderWriter, IFileSystemNavigator fileSystemNavigator, IFileArchiver fileArchiver,
                                 ProgramsSettingsProvider programsSettingsProvider, IWinSettingsLauncher winSettingsLauncher)
        {
            ProgramsSets = new ObservableCollection<ProgramsSet>();
            GroupProgramsWrappers = new ObservableCollection<GroupProgramsWrapper>();
            ProtectedPrograms = new ObservableCollection<ProtectedProgram>();

            _programsService = programsService;

            _winInfo = winInfo;
            _antiviruses = antiviruses;

            ConsoleBuffer = consoleBuffer;
            _messageDialogService = messageDialogService;
            
            _fileSystemReaderWriter = fileSystemReaderWriter;
            _fileSystemNavigator = fileSystemNavigator;
            _fileArchiver = fileArchiver;

            _programsSettingsProvider = programsSettingsProvider;
            _winSettingsLauncher = winSettingsLauncher;

            IsEnabled = true;

            InitializeCommands();
        }
        private void InitializeCommands()
        {
            SelectProgramsSetCommand = new DelegateCommand(OnSelectProgramsSet);
            SelectOnlyFreeProgramsCommand = new DelegateCommand(OnSelectOnlyFreePrograms);

            OpenAUCSettingsCommand = new DelegateCommand(OnOpenAUCSettings);
            OpenTrayIconSettingsCommand = new DelegateCommand(OnOpenTrayIconSettings);
            OpenDesktopIconSettingsCommand = new DelegateCommand(OnOpenDesktopIconSettings);
            SpeedTestCommand = new DelegateCommand(OnSpeedTest);

            OpenDefaultProgramsSettingsCommand = new DelegateCommand(OnOpenDefaultProgramsSettings);
            OpenFolderExampleFilesCommand = new DelegateCommand(OnOpenFolderExampleFiles);
            OpenFolderLogFilesCommand = new DelegateCommand(OnOpenFolderLogFiles);
            
            OpenProtectedProgramCommand = new DelegateCommand<ProtectedProgram>(OnOpenProtectedProgram);
            
            OffAllAntivirusesCommand = new DelegateCommand(OnOffAllAntiviruses);
            OnAllAntivirusesCommand = new DelegateCommand(OnOnAllAntiviruses);

            ClearConsoleCommand = new DelegateCommand(OnClearConsole);
            InstallCommand = new DelegateCommand(OnInstall);
        }
        #endregion

        #region Motods
        public Task LoadAsync()
        {
            ConsoleBuffer.Text = _winInfo.ToString();

            ProgramsSets.Clear();
            GroupProgramsWrappers.Clear();
            ProtectedPrograms.Clear();

            _programsPaths = (ProgramsPaths)_programsSettingsProvider.Settings.ProgramsPaths.Clone();
            PathService.SetRootPath(_fileSystemReaderWriter.PathRoot, _programsPaths);
            //----------------------------------------------------------------------------------------------------
            var GroupsPrograms = (List<GroupPrograms>)_programsSettingsProvider.Settings.GroupsPrograms?.Clone();
            foreach (var groupProgram in GroupsPrograms)
                GroupProgramsWrappers.Add(new GroupProgramsWrapper(groupProgram));

            _programsService.CombinePath(GroupProgramsWrappers, _programsPaths);
            //ProgramsHelper.CheckInstallPrograms(GroupProgramsWrappers, _winInfo.WinArchitecture); // CheckInstall
            //----------------------------------------------------------------------------------------------------
            _programsSettingsProvider.Settings.ProgramsSets.ForEach(x => ProgramsSets.Add(x)); // Add ProgramsSets     
            var tempProgramsSet = ProgramsSets.FirstOrDefault(x => x.Name.Contains(Regex.Match(_winInfo.ProductName, @"\d+").Value, StringComparison.InvariantCultureIgnoreCase)); // check ProgramsSet
            if (tempProgramsSet != null)
                SelectedProgramsSet = tempProgramsSet.Name;
            OnSelectProgramsSet();
            //----------------------------------------------------------------------------------------------------
            ((List<ProtectedProgram>)_programsSettingsProvider.Settings.ProtectedPrograms.Clone()).ForEach(x=> 
            {
                PathService.SetRootPath(_fileSystemReaderWriter.PathRoot, x.ProtectedProgramPaths);
                ProtectedPrograms.Add(x);
            });

            return Task.CompletedTask;
        }
        public void Unsubscribe()
        {
           
        }
        public bool HasChanges()
        {
            return !IsEnabled;
        }
        //***************************************************************************************************
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
        private void OnSelectOnlyFreePrograms()
        {
            foreach (var program in GroupProgramsWrappers.SelectMany(group => group.ProgramWrappers))
            {
                if(program.Install == true && program.Program.IsFree==false)
                    program.NotSelectProgram();
            }
        }
        //---------------------------------------------------------------------------------------------------
        private void OnOpenAUCSettings()
        {
            _winSettingsLauncher.OpenAUCSettings();
        }
        private void OnOpenTrayIconSettings()
        {
            _winSettingsLauncher.OpenIconSettings();
        }
        private void OnOpenDesktopIconSettings()
        {
            _winSettingsLauncher.OpenDesktopIconSettings();
        }
        private async void OnSpeedTest()
        {
            IsEnabled = false;
            ConsoleBuffer.AddSplitter();
            await _programsService.SpeedTest();
            ConsoleBuffer.AddSplitter();
            IsEnabled = true;
        }
        //---------------------------------------------------------------------------------------------------
        private void OnOpenDefaultProgramsSettings()
        {
            _winSettingsLauncher.OpenDefaultProgramsSettings();
        }
        private void OnOpenFolderExampleFiles()
        {
            _fileSystemNavigator.OpenFolder(_programsPaths.PathExampleFile);
        }
        private void OnOpenFolderLogFiles()
        {
            _fileSystemNavigator.OpenFolder(_fileSystemReaderWriter.CompasPackLog);
        }
        //---------------------------------------------------------------------------------------------------
        private async void OnOpenProtectedProgram(ProtectedProgram protectedProgram)
        {
            IsEnabled = false;
            await _programsService.OpenProtectedProgram(protectedProgram);
            IsEnabled = true;
        }
        //---------------------------------------------------------------------------------------------------
        private async void OnOnAllAntiviruses()
        {
            IsEnabled = false;
            ConsoleBuffer.AddSplitter();
            foreach (var unManualAntivirus in _antiviruses.Where(av => !av.IsControlled))
                ConsoleBuffer.WriteLine($"{unManualAntivirus.AntivirusInfo.DisplayName}: UnManual (Fail!)\n");
            foreach (var tamperProtectionAntivirus in _antiviruses.Where(av => av.IsControlled && av.GetTamperProtectionStatus() != AntivirusStatusEnum.Disabled))
                ConsoleBuffer.WriteLine($"{tamperProtectionAntivirus.AntivirusInfo.DisplayName}: TamperProtection (Fail!)\n");
            await _programsService.OnAntiviruses(_antiviruses.Where(av => av.IsControlled && av.GetTamperProtectionStatus() == AntivirusStatusEnum.Disabled).ToList());
            ConsoleBuffer.AddSplitter();
            IsEnabled = true;
        }
        private async void OnOffAllAntiviruses()
        {
            IsEnabled = false;
            ConsoleBuffer.AddSplitter();
            foreach (var unManualAntivirus in _antiviruses.Where(av => !av.IsControlled))
                ConsoleBuffer.WriteLine($"{unManualAntivirus.AntivirusInfo.DisplayName}: UnManual (Fail!)\n");
            foreach (var tamperProtectionAntivirus in _antiviruses.Where(av => av.IsControlled && av.GetTamperProtectionStatus() != AntivirusStatusEnum.Disabled))
                ConsoleBuffer.WriteLine($"{tamperProtectionAntivirus.AntivirusInfo.DisplayName}: TamperProtection (Fail!)\n");
            await _programsService.OffAntiviruses(_antiviruses.Where(av => av.IsControlled && av.GetTamperProtectionStatus() == AntivirusStatusEnum.Disabled).ToList());
            ConsoleBuffer.AddSplitter();
            IsEnabled = true;
        }
        //---------------------------------------------------------------------------------------------------
        private void OnClearConsole()
        {
            ConsoleBuffer.Text = _winInfo.ToString();
        }
        private async void OnInstall()
        {
            IsEnabled = false;
            var selectedPrograms = GroupProgramsWrappers.SelectMany(group => group.ProgramWrappers).Where(x => x.Install == true).ToList();
            await _programsService.InstallPrograms(selectedPrograms);
            IsEnabled = true;
        }
        //---------------------------------------------------------------------------------------------------
        #endregion

        #region Commands
        public ICommand SelectProgramsSetCommand { get; private set; }
        public ICommand SelectOnlyFreeProgramsCommand { get; private set; }
        //---------------------------------------------------------------------------------------------------
        public ICommand OpenAUCSettingsCommand { get; private set; }
        public ICommand OpenTrayIconSettingsCommand { get; private set; }
        public ICommand OpenDesktopIconSettingsCommand { get; private set; }
        public ICommand SpeedTestCommand { get; private set; }
        //---------------------------------------------------------------------------------------------------
        public ICommand OpenDefaultProgramsSettingsCommand { get; private set; }
        public ICommand OpenFolderExampleFilesCommand { get; private set; }
        public ICommand OpenFolderLogFilesCommand { get; private set; }
        //---------------------------------------------------------------------------------------------------
        public ICommand OpenProtectedProgramCommand { get; private set; }
        //---------------------------------------------------------------------------------------------------
        public ICommand OffAllAntivirusesCommand { get; private set; }
        public ICommand OnAllAntivirusesCommand { get; private set; }
        //---------------------------------------------------------------------------------------------------
        public ICommand InstallCommand { get; private set; }
        public ICommand ClearConsoleCommand { get; private set; }
        #endregion
    }
}
