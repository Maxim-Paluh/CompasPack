using CompasPac.Data;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CompasPac.ViewModel
{
    public class GroupProgramViewModel : ViewModelBase
    {
        private Visibility _visibility;
        private IEventAggregator _eventAggregator;
        
        public GroupProgramViewModel(GroupProgram groupProgram, ObservableCollection<UserProgramViewModel> userProgramViewModels, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SetVisibilityCommand = new DelegateCommand(OnSetVisibility);
            GroupProgram = groupProgram;
            UserProgramViewModels = userProgramViewModels;
        }

        public Visibility VisibilityGroup
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }
        public GroupProgram GroupProgram { get; set; }
        public ObservableCollection<UserProgramViewModel> UserProgramViewModels { get; set; }
        
        private void OnSetVisibility()
        {
            if (VisibilityGroup == Visibility.Collapsed)
                VisibilityGroup = Visibility.Visible;
            else if (VisibilityGroup == Visibility.Visible)
                VisibilityGroup = Visibility.Collapsed;
        }

        public ICommand SetVisibilityCommand { get; }
    }
}
