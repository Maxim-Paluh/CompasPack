using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using System.Diagnostics;
using System.IO;
using CompasPack.View.Service;
using CompasPack.View;
using Autofac.Features.Indexed;
using CompasPack.Settings;
using CompasPack.Helper;
using CompasPack.Settings.Portable;
using System.Collections.ObjectModel;
using CompasPack.Service;
using DocumentFormat.OpenXml.Packaging;

namespace CompasPack.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IMessageDialogService _messageDialogService;
        private readonly Func<MainSettingsView> _mainSettingsViewCreator;
        private readonly IIOHelper _iOHelper;
        private IDetailViewModel _formViewModel;
        private readonly IIndex<string, IDetailViewModel> _formViewModelCreator;
        private bool _programsIsEnabled;
        private bool _reportIsEnabled;
        private bool _portableIsEnabled;
        public bool ProgramsIsEnabled
        {
            get { return _programsIsEnabled; }
            set 
            { 
                _programsIsEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool ReportIsEnabled
        {
            get { return _reportIsEnabled; }
            set 
            { 
                _reportIsEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool PortableIsEnabled
        {
            get { return _portableIsEnabled; }
            set { 
                _portableIsEnabled = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<PortableProgram> PortablePrograms { get; private set; }
        public IDetailViewModel FormViewModel
        {
            get { return _formViewModel; }
            private set
            {
                _formViewModel = value;
                OnPropertyChanged();
            }
        }

        //-------------------------------------------------------------------------
        private readonly UserPathSettingsHelper _userPathSettingsHelper;
        private readonly UserProgramsSettingsHelper _userProgramsSettingsHelper;
        private readonly UserPresetSettingsHelper _userPresetSettingsHelper;
        private readonly ReportSettingsSettingsHelper _reportSettingsSettingsHelper;
        private readonly PortableProgramsSettingsHelper _portableProgramsSettingsHelper;

        public MainWindowViewModel(IMessageDialogService messageDialogService, IIOHelper iOHelper, IEventAggregator eventAggregator, IIndex<string, IDetailViewModel> formViewModelCreator,
            UserPathSettingsHelper userPathSettingsHelper,
            UserProgramsSettingsHelper userProgramsSettingsHelper,
            UserPresetSettingsHelper userPresetSettingsHelper,
            ReportSettingsSettingsHelper reportSettingsSettingsHelper,
            PortableProgramsSettingsHelper portableProgramsSettingsHelper,
            Func<MainSettingsView> MainSettingsViewCreator)
        {
            _messageDialogService = messageDialogService;
            _iOHelper = iOHelper;
            _formViewModelCreator = formViewModelCreator;
            _userPathSettingsHelper = userPathSettingsHelper;
            _userProgramsSettingsHelper = userProgramsSettingsHelper;
            _userPresetSettingsHelper = userPresetSettingsHelper;
            _reportSettingsSettingsHelper = reportSettingsSettingsHelper;
            _portableProgramsSettingsHelper = portableProgramsSettingsHelper;
            _mainSettingsViewCreator = MainSettingsViewCreator;
            //--------------------------------------------------------------------
            ProgramsIsEnabled = true;
            ReportIsEnabled = true;
            PortableIsEnabled = true;
            PortablePrograms = new ObservableCollection<PortableProgram>();
            //--------------------------------------------------------------------
            ClosedAppCommand = new DelegateCommand(OnClosedApp);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettings);
            CheckUpdateProgramCommand = new DelegateCommand(OnCheckUpdateProgram);
            AboutProgramCommand = new DelegateCommand(OnAboutProgram);
            CreateFormCommand = new DelegateCommand<Type>(OnCreateNewFormExecute);
            OpenProgramCommand = new DelegateCommand<PortableProgram>(OpenProgram);
        }

        //******************************************************
        public async Task LoadAsync()
        {
            var tempLoad = (LoadViewModel)_formViewModelCreator[typeof(LoadViewModel).Name];
            tempLoad.Message = "Завантаження налаштувань...";
            FormViewModel = tempLoad;
            
            await _userPresetSettingsHelper.LoadFromFile();
            
            await _portableProgramsSettingsHelper.LoadFromFile();
            PortableIsEnabled = _portableProgramsSettingsHelper.IsLoad;
            if(PortableIsEnabled)
            {
                var portablePrograms = _portableProgramsSettingsHelper.Settings.PortableProgramsList.Clone();
                foreach (var portableProgram in portablePrograms)
                {
                    if (portableProgram != null && !string.IsNullOrWhiteSpace(portableProgram.Path))
                        portableProgram.Path = Path.Combine(_iOHelper.PathRoot, portableProgram.Path); // якщо path2 повний, то буде використано його (повний шлях), інакше буде використано відносний шлях

                    PortablePrograms.Add(portableProgram);
                }
            }

            await _userProgramsSettingsHelper.LoadFromFile();
            ProgramsIsEnabled = _userProgramsSettingsHelper.IsLoad;

            await _reportSettingsSettingsHelper.LoadFromFile();
            ReportIsEnabled = _reportSettingsSettingsHelper.IsLoad;

            await _userPathSettingsHelper.LoadFromFile();
            if (!_userPathSettingsHelper.IsLoad)
            {
                PortableIsEnabled = false;
                ProgramsIsEnabled = false;
                ReportIsEnabled = false;
            }
            
            if (ProgramsIsEnabled)
            {
#if DEBUG

#else
                await Task.Delay(1000);// show logo comp@s   
#endif
                var tempPrograms = _formViewModelCreator[typeof(ProgramsViewModel).Name];
                await tempPrograms.LoadAsync(null);
                FormViewModel = tempPrograms;
            }
            else
            {
                FormViewModel = null;
            }
        }
        //******************************************************
        //--------------------------------------
        private void OpenProgram(PortableProgram  portableProgram)
        {
            if (FormViewModel != null && FormViewModel.HasChanges())
            {
                _messageDialogService.ShowInfoDialog("Краще дочекатись виконання поточного завдання!", "Попередження");
                return;
            }

            if (File.Exists(portableProgram.Path))
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = portableProgram.Path,
                        WorkingDirectory = Path.GetDirectoryName(portableProgram.Path)
                    }
                }.Start();
            }
            else
                _messageDialogService.ShowInfoDialog($"Виконуваний файл не знайдено: {portableProgram.Path}", "Помилка!");   
        }
        //--------------------------------------
        private void OnClosedApp()
        {
            System.Windows.Application.Current.Shutdown();
        }
        private async void OnOpenSettings()
        {
            var mainSettingsView = _mainSettingsViewCreator.Invoke();
            mainSettingsView.ShowDialog();
            await LoadAsync(); //TODO Змінити це в залежності від принципу роботи збереження налаштувань для оптимізації
        }
        private void OnCheckUpdateProgram()
        {
            _messageDialogService.ShowInfoDialog("В даний момент ця функція відсутня, зверніться до розробника!", "Помилка!");
        }
        private void OnAboutProgram()
        {
            var About = new AboutView();
            About.ShowDialog();
        }
        private void OnCreateNewFormExecute(Type viewModelType)
        {
            if (FormViewModel != null && FormViewModel.HasChanges())
            {
                _messageDialogService.ShowInfoDialog("Зачекай завершення роботи в поточному вікні!", "Попередження");
                return;
            }


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
        public ICommand ClosedAppCommand { get; }
        public ICommand OpenSettingsCommand { get; set; }
        public ICommand CheckUpdateProgramCommand { get; }
        public ICommand AboutProgramCommand { get; }
        public ICommand CreateFormCommand { get; set; }
        public ICommand OpenProgramCommand { get; set; }
    }
}
