using CompasPack.BL;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using CompasPack.Event;
using System.Diagnostics;
using System.IO;
using CompasPack.View.Service;
using CompasPack.View;
using System.Data;
using CompasPack;
using Autofac.Features.Indexed;
using CompasPakc.BL;
using CompasPack.Main;
using CompasPack.Settings.Common;
using CompasPack.Settings;
using CompasPack.Settings.Programs;

namespace CompasPack.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IMessageDialogService _messageDialogService;
        private readonly IIOManager _iOManager;
        private IDetailViewModel? _formViewModel;

        private readonly IIndex<string, IDetailViewModel> _formViewModelCreator;
        private readonly UserPathSettingsHelper _userPathSettingsHelper;
        private readonly UserProgramsSettingsHelper _userProgramsSettingsHelper;
        private readonly UserPresetSettingsHelper _userPresetSettingsHelper;

        public MainWindowViewModel(IMessageDialogService messageDialogService, IIOManager iOManager, IEventAggregator eventAggregator, IIndex<string, IDetailViewModel> formViewModelCreator,
            UserPathSettingsHelper userPathSettingsHelper, UserProgramsSettingsHelper userProgramsSettingsHelper, UserPresetSettingsHelper userPresetSettingsHelper)
        {
            _messageDialogService = messageDialogService;
            _iOManager = iOManager;
            _formViewModelCreator = formViewModelCreator;
            _userPathSettingsHelper = userPathSettingsHelper;
            _userProgramsSettingsHelper = userProgramsSettingsHelper;
            _userPresetSettingsHelper = userPresetSettingsHelper;
            OpenAidaCommand = new DelegateCommand(OnOpenAida);
            OpenCpuZCommand = new DelegateCommand(OnOpenCpuZ);
            OpenGpuZCommand = new DelegateCommand(OnOpenGpuZ);
            OpenCrystalCommand = new DelegateCommand(OnOpenCrystal);
            OpenFurMarkCommand = new DelegateCommand(OnOpenFurMark);
            OpenTotalCommander951Command = new DelegateCommand(OnOpenTotalCommander951);
            OpenTotalCommander700Command = new DelegateCommand(OnOpenTotalCommander700);
            OpenWinRarCommand = new DelegateCommand(OnOpenWinRar);

            ClosedAppCommand = new DelegateCommand(OnClosedApp);
            SetDefaultGroupProgramCommand = new DelegateCommand(OnSetDefaultGroupProgram);
            SetDefaultUserPresetProgramCommand = new DelegateCommand(OnSetDefaultUserPresetProgram);
            SetDefaultUserDocxCommand = new DelegateCommand(OnSetDefaultUserDocx);
            CheckUpdateProgramCommand = new DelegateCommand(OnCheckUpdateProgram);
            AboutProgramCommand = new DelegateCommand(OnAboutProgram);

            CreateFormCommand = new DelegateCommand<Type>(OnCreateNewFormExecute);
        }

        //******************************************************
        public async Task LoadAsync()
        {
            FormViewModel = _formViewModelCreator[typeof(LoadViewModel).Name];
            var tempPrograms = _formViewModelCreator[typeof(ProgramsViewModel).Name];
            await _userPathSettingsHelper.LoadFromFile();
            await _userProgramsSettingsHelper.LoadFromFile();
            await _userPresetSettingsHelper.LoadFromFile();
            await tempPrograms.LoadAsync(null);
            await Task.Delay(500);
            FormViewModel = tempPrograms;
        }
        //******************************************************
        //--------------------------------------
        private void OnOpenFurMark()
        {
            OpenProgram(_iOManager.FurMark);
        }
        private void OnOpenCrystal()
        {
            OpenProgram(_iOManager.CrystalDisk);
        }
        private void OnOpenCpuZ()
        {
            OpenProgram(_iOManager.CpuZ);
        }
        private void OnOpenGpuZ()
        {
            OpenProgram(_iOManager.GpuZ);
        }
        private void OnOpenAida()
        {
            OpenProgram(_iOManager.Aida);
        }
        private void OnOpenTotalCommander951()
        {
            OpenProgram(_iOManager.TotalCommander951);
        }
        private void OnOpenTotalCommander700()
        {
            OpenProgram(_iOManager.TotalCommander700);
        }
        private void OnOpenWinRar()
        {
            OpenProgram(_iOManager.WinRar);
        }
        private void OpenProgram(string path)
        {
            if (File.Exists(path))
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                    }
                }.Start();
            }
            else
            {
                _messageDialogService.ShowInfoDialog($"Виконуваний файл не знайдено: {path}", "Помилка!");
            }
        }
        //--------------------------------------
        private void OnClosedApp()
        {
            System.Windows.Application.Current.Shutdown();
        }
        private async void OnSetDefaultGroupProgram()
        {
            var resultDialog = _messageDialogService.ShowYesNoDialog($"Очистити шаблон програм до стандартного?\nЦю дію не можна відмінити!!!", "Очистка шаблону!");
            if (resultDialog == MessageDialogResult.Yes)
            {
                await _iOManager.SetDefaultGroupProgram();
                if (_formViewModel != null)
                    if (_formViewModel.GetType().Name == nameof(ProgramsViewModel))
                        await _formViewModel.LoadAsync(null);
            }
        }
        private async void OnSetDefaultUserPresetProgram()
        {
            var resultDialog = _messageDialogService.ShowYesNoDialog($"Очистити шаблон набору програм до стандартного?\nЦю дію не можна відмінити!!!", "Очистка шаблону!");
            if (resultDialog == MessageDialogResult.Yes)
            {
                await _iOManager.SetDefaultUserPresetProgram();
                if(_formViewModel!=null)
                    if (_formViewModel.GetType().Name == nameof(ProgramsViewModel))
                        await _formViewModel.LoadAsync(null);
            }
        }
        private void OnSetDefaultUserDocx()
        {
            var resultDialog = _messageDialogService.ShowYesNoDialog($"Очистити шаблони Docx?\nЦю дію не можна відмінити!!!", "Очистка шаблону!");
            if (resultDialog == MessageDialogResult.Yes)
            {
                 _iOManager.ReInstallPatternDocx();
            }
        }
        private void OnCheckUpdateProgram()
        {
            _messageDialogService.ShowInfoDialog("Охх горе, нажаль ця функція нереалізована, зверніться до розробника!", "Помилка!");
        }
        private void OnAboutProgram()
        {
            var About = new AboutView();
            About.ShowDialog();
        }
        private void OnCreateNewFormExecute(Type viewModelType)
        {
            if (FormViewModel != null)
                FormViewModel.Unsubscribe();

            if (viewModelType == null)
                FormViewModel = null;
            else
            {
                FormViewModel = _formViewModelCreator[viewModelType.Name];
                FormViewModel.LoadAsync(null);
            }
        }
        //--------------------------------------




        public ICommand OpenAidaCommand { get; }
        public ICommand OpenCpuZCommand { get; }
        public ICommand OpenGpuZCommand { get; }
        public ICommand OpenCrystalCommand { get; }
        public ICommand OpenFurMarkCommand { get; }
        public ICommand OpenTotalCommander951Command { get; }
        public ICommand OpenTotalCommander700Command { get; }
        public ICommand OpenWinRarCommand { get; }


        public ICommand ClosedAppCommand { get; }
        public ICommand SetDefaultGroupProgramCommand { get; }
        public ICommand SetDefaultUserPresetProgramCommand { get; }
        public ICommand SetDefaultUserDocxCommand { get; }
        public ICommand CheckUpdateProgramCommand { get; }
        public ICommand AboutProgramCommand { get; }

        public IDetailViewModel? FormViewModel
        {
            get { return _formViewModel; }
            private set
            {
                _formViewModel = value;
                OnPropertyChanged();
            }
        }
        public ICommand CreateFormCommand { get; set; }
    }
}
