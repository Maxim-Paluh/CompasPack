﻿using System;
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

namespace CompasPack.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IMessageDialogService _messageDialogService;
        private readonly IIOHelper _iOHelper;
        private IDetailViewModel? _formViewModel;
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
            PortableProgramsSettingsHelper portableProgramsSettingsHelper)
        {
            _messageDialogService = messageDialogService;
            _iOHelper = iOHelper;
            _formViewModelCreator = formViewModelCreator;
            _userPathSettingsHelper = userPathSettingsHelper;
            _userProgramsSettingsHelper = userProgramsSettingsHelper;
            _userPresetSettingsHelper = userPresetSettingsHelper;
            _reportSettingsSettingsHelper = reportSettingsSettingsHelper;
            _portableProgramsSettingsHelper = portableProgramsSettingsHelper;
            //--------------------------------------------------------------------
            ProgramsIsEnabled = true;
            ReportIsEnabled = true;
            PortableIsEnabled = true;
            PortablePrograms = new ObservableCollection<PortableProgram>();
            //--------------------------------------------------------------------
            ClosedAppCommand = new DelegateCommand(OnClosedApp);
            CheckUpdateProgramCommand = new DelegateCommand(OnCheckUpdateProgram);
            AboutProgramCommand = new DelegateCommand(OnAboutProgram);
            CreateFormCommand = new DelegateCommand<Type>(OnCreateNewFormExecute);
        }

        //******************************************************
        public async Task LoadAsync()
        {
            var tempLoad = (LoadViewModel)_formViewModelCreator[typeof(LoadViewModel).Name];
            tempLoad.Message = "Завантаження налаштувань...";
            FormViewModel = tempLoad;
            await _portableProgramsSettingsHelper.LoadFromFile();
            PortableIsEnabled = _portableProgramsSettingsHelper.IsLoad;
            if(PortableIsEnabled)
            {
                foreach (var portableProgram in _portableProgramsSettingsHelper.Settings.portablePrograms)
                    PortablePrograms.Add(portableProgram);
                //if (PortablePrograms.Count == 0)
                //    PortableIsEnabled = false;
            }
            await _userPathSettingsHelper.LoadFromFile();
            await _userProgramsSettingsHelper.LoadFromFile();
            ProgramsIsEnabled = _userProgramsSettingsHelper.IsLoad;
            await _userPresetSettingsHelper.LoadFromFile();
            await _reportSettingsSettingsHelper.LoadFromFile();
            ReportIsEnabled = _reportSettingsSettingsHelper.IsLoad;
            if (ProgramsIsEnabled)
            {
                await Task.Delay(1000);
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
