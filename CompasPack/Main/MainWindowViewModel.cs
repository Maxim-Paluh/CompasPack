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

namespace CompasPack.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IMessageDialogService _messageDialogService;
        private readonly IIOHelper _iOHelper;
        private IDetailViewModel? _formViewModel;

        private readonly IIndex<string, IDetailViewModel> _formViewModelCreator;
        private readonly UserPathSettingsHelper _userPathSettingsHelper;
        private readonly UserProgramsSettingsHelper _userProgramsSettingsHelper;
        private readonly UserPresetSettingsHelper _userPresetSettingsHelper;
        private readonly ReportSettingsSettingsHelper _reportSettingsSettingsHelper;

        public MainWindowViewModel(IMessageDialogService messageDialogService, IIOHelper iOHelper, IEventAggregator eventAggregator, IIndex<string, IDetailViewModel> formViewModelCreator,
            UserPathSettingsHelper userPathSettingsHelper,
            UserProgramsSettingsHelper userProgramsSettingsHelper,
            UserPresetSettingsHelper userPresetSettingsHelper,
            ReportSettingsSettingsHelper reportSettingsSettingsHelper)
        {
            _messageDialogService = messageDialogService;
            _iOHelper = iOHelper;
            _formViewModelCreator = formViewModelCreator;
            _userPathSettingsHelper = userPathSettingsHelper;
            _userProgramsSettingsHelper = userProgramsSettingsHelper;
            _userPresetSettingsHelper = userPresetSettingsHelper;
            _reportSettingsSettingsHelper = reportSettingsSettingsHelper;
            ClosedAppCommand = new DelegateCommand(OnClosedApp);
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
            await _reportSettingsSettingsHelper.LoadFromFile();
            await tempPrograms.LoadAsync(null);
            await Task.Delay(1000);
            FormViewModel = tempPrograms;
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
