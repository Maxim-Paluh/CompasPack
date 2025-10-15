using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Autofac.Features.Indexed;

using CompasPack.ViewModel;

namespace CompasPack.Settings
{
    public class MainSettingsViewModel : ViewModelBase, IViewModel
    {

        private ISettingsViewModel _selectedSettingsViewModel;
        private readonly IIndex<string, ISettingsViewModel> _settingsViewModeCreator;

        public ObservableCollection<ISettingsViewModel> SettingsViewModels { get; set; }
        public ISettingsViewModel SelectedSettingsViewModel
        {
            get { return _selectedSettingsViewModel; }
            set
            {
                _selectedSettingsViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainSettingsViewModel(IIndex<string, ISettingsViewModel> settingsViewModeCreator)
        {
            SettingsViewModels = new ObservableCollection<ISettingsViewModel>();
            _settingsViewModeCreator = settingsViewModeCreator;
        }

        public bool HasChanges()
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync()
        {
            //var tabUserPathSettings = _settingsViewModeCreator[nameof(UserPathSettingsTabViewModel)];
            //await tabUserPathSettings.LoadAsync();

            //SettingsViewModels.Add(tabUserPathSettings);

            //SelectedSettingsViewModel = tabUserPathSettings;
            return Task.CompletedTask;
        }

        public void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
