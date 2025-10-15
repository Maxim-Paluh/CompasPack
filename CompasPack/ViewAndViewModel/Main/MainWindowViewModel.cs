using Autofac.Features.Indexed;
using CompasPack.Data.Providers;
using CompasPack.Data.Providers.API;
using CompasPack.Helper.Extension;
using CompasPack.Helper.Service;
using CompasPack.Model.Settings;
using CompasPack.Settings;
using CompasPack.View;
using DocumentFormat.OpenXml.Packaging;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CompasPack.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IMessageDialogService _messageDialogService;
        private readonly Func<MainSettingsView> _mainSettingsViewCreator;
        private readonly IFileSystemReaderWriter _fileSystemReaderWriter;
        private readonly IWinInfoProvider _winInfoProvider;
        private IViewModel _formViewModel;
        private readonly IIndex<string, IViewModel> _formViewModelCreator;
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
        public IViewModel FormViewModel
        {
            get { return _formViewModel; }
            private set
            {
                _formViewModel = value;
                OnPropertyChanged();
            }
        }

        //-------------------------------------------------------------------------
        private readonly ProgramsSettingsProvider _ProgramsSettingsProvider;
        private readonly ReportSettingsProvider _reportSettingsProvider;
        private readonly PortableProgramsSettingsProvider _portableProgramsSettingsProvider;

        public MainWindowViewModel(IMessageDialogService messageDialogService, IFileSystemReaderWriter fileSystemReaderWriter, IEventAggregator eventAggregator, IIndex<string, IViewModel> formViewModelCreator,
            ProgramsSettingsProvider programsSettingsProvider,
            ReportSettingsProvider reportSettingsProvider,
            IWinInfoProvider winInfoProvider,
            PortableProgramsSettingsProvider portableProgramsSettingsProvider,
            Func<MainSettingsView> MainSettingsViewCreator)
        {
            _messageDialogService = messageDialogService;
            _fileSystemReaderWriter = fileSystemReaderWriter;
            _winInfoProvider = winInfoProvider;
            _formViewModelCreator = formViewModelCreator;
            _ProgramsSettingsProvider = programsSettingsProvider;
            _reportSettingsProvider = reportSettingsProvider;
            _portableProgramsSettingsProvider = portableProgramsSettingsProvider;
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
            OpenPortableProgramCommand = new DelegateCommand<PortableProgram>(OpenPortableProgram);
        }

        //******************************************************
        public async Task LoadAsync()
        {
            if ((int)_winInfoProvider.WinVer < 3) // якщо версія ОС нижча за Windows 7
            {
                _messageDialogService.ShowInfoDialog($"Нічого не буде, потрібно використовувати Windows 7 або новішу версію ОС", "Помилка!");
                System.Windows.Application.Current.Shutdown();
            }

            var tempLoad = (LoadViewModel)_formViewModelCreator[typeof(LoadViewModel).Name];
            tempLoad.Message = "Завантаження налаштувань...";
            FormViewModel = tempLoad;
            
            await _portableProgramsSettingsProvider.LoadFromFile();
            PortableIsEnabled = _portableProgramsSettingsProvider.IsLoad;
            if(PortableIsEnabled)
            {
                var portablePrograms = _portableProgramsSettingsProvider.Settings.PortableProgramsList.Clone();
                foreach (var portableProgram in portablePrograms)
                {
                    if (portableProgram != null && !string.IsNullOrWhiteSpace(portableProgram.Path))
                        portableProgram.Path = Path.Combine(_fileSystemReaderWriter.PathRoot, portableProgram.Path); // якщо path2 повний, то буде використано його (повний шлях), інакше буде використано відносний шлях

                    PortablePrograms.Add(portableProgram);
                }
            }

            await _ProgramsSettingsProvider.LoadFromFile();
            ProgramsIsEnabled = _ProgramsSettingsProvider.IsLoad;

            await _reportSettingsProvider.LoadFromFile();
            ReportIsEnabled = _reportSettingsProvider.IsLoad;
            
            if (ProgramsIsEnabled)
            {
#if DEBUG

#else
                await Task.Delay(1000);// show logo comp@s   
#endif
                var tempPrograms = _formViewModelCreator[typeof(ProgramsViewModel).Name];
                await tempPrograms.LoadAsync();
                FormViewModel = tempPrograms;
            }
            else
            {
                FormViewModel = null;
            }
        }
        //******************************************************
        //--------------------------------------
        private void OpenPortableProgram(PortableProgram  portableProgram)
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
                FormViewModel.LoadAsync();
            }
        }
        //--------------------------------------
        public ICommand ClosedAppCommand { get; }
        public ICommand OpenSettingsCommand { get; set; }
        public ICommand CheckUpdateProgramCommand { get; }
        public ICommand AboutProgramCommand { get; }
        public ICommand CreateFormCommand { get; set; }
        public ICommand OpenPortableProgramCommand { get; set; }
    }
}
