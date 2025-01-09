using Autofac.Features.Indexed;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings
{
    public class MainSettingsViewModel : ViewModelBase, IDetailViewModel
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

        public async Task LoadAsync(int? Id)
        {
            var tabUserPathSettings = _settingsViewModeCreator[nameof(UserPathSettingsTabViewModel)];
            await tabUserPathSettings.LoadAsync();
          
            SettingsViewModels.Add(tabUserPathSettings);

            SelectedSettingsViewModel = tabUserPathSettings;
        }

        public void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
