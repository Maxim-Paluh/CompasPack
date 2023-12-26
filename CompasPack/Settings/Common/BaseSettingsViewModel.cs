using CompasPack.View.Service;
using CompasPack.Wrapper;
using Prism.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CompasPack.Settings
{
    public interface ISettingsViewModel
    {
        public bool HasChanges { get; }
        public string Title { get; }
        public Task LoadAsync();
    }

    public class BaseSettingsViewModel<TSettings, TWrapper, TSettingsHelper> : ViewModelBase, ISettingsViewModel
        where TSettings : class, ICloneable, new()
        where TWrapper : ModelWrapper<TSettings>, new()
        where TSettingsHelper : SettringsHelperBase<TSettings>      
    {
        #region Properties
        protected IMessageDialogService _messageDialogService;
        private string _title;
        private bool _hasChanges;
        private TSettings _currentSettings { get; set; }
        private TSettings _newSettings { get; set; }       
        protected readonly TSettingsHelper SettingsHelper;      
        public TWrapper SettingsWrapper { get; set; }
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public BaseSettingsViewModel(IMessageDialogService messageDialogService, TSettingsHelper settingsHelper)
        {
            _messageDialogService = messageDialogService;
            SettingsHelper = settingsHelper;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            SetDefaultCommand = new DelegateCommand(OnSetDefaultExecute);
            //ImportCommand = new DelegateCommand(OnImportExecute);
            //ExportCommand = new DelegateCommand(OnExportExecute);
        }
        #endregion

        #region Motods
        public virtual async Task LoadAsync()
        {
            if (SettingsHelper.IsLoad == false)
                await SettingsHelper.LoadFromFile();
            if (SettingsHelper.Settings == null)
            {
                _currentSettings = await SettingsHelper.LoadDefault();
                _messageDialogService.ShowInfoDialog($"Оскільки в файлі {Path.GetFileName(SettingsHelper.SettingsPathFile)} є помилки якщо збережете нові налаштування всі зміни які ви вносили в ньому будуть перезаписані!", "Попередження!");
            }
            else
                _currentSettings= SettingsHelper.Settings;

            _newSettings = (TSettings)_currentSettings.Clone();
            SettingsWrapper = new TWrapper();
            SettingsWrapper.Model = _newSettings;

            SettingsWrapper.PropertyChanged += UserPath_PropertyChanged;
            SettingsWrapper.ValidateAllCustomErrors();
        }

        private void UserPath_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            HasChanges = !_newSettings.Equals(_currentSettings);
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }


        protected virtual async void OnSaveExecute()
        {
            if (SettingsHelper.Settings == null)
                SettingsHelper.Settings = _newSettings;
            else
            {
                SettingsHelper.Settings = (TSettings)_newSettings.Clone();
                _currentSettings = SettingsHelper.Settings;

            }

            await SettingsHelper.Save();
            UserPath_PropertyChanged(null, null);
        }
        protected virtual bool OnSaveCanExecute()
        {
            return HasChanges&& !SettingsWrapper.HasErrors;
        }
        protected virtual void OnSetDefaultExecute()
        { }
        //protected abstract void OnExportExecute();
        //protected abstract void OnImportExecute();
        #endregion

        #region Commands
        public ICommand SaveCommand { get; private set; }
        public ICommand SetDefaultCommand { get; private set; }
        //public ICommand ImportCommand { get; private set; }
        //public ICommand ExportCommand { get; private set; }
        #endregion
    }
}
