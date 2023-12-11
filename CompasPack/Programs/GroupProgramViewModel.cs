using CompasPack.Data;
using Prism.Commands;
using Prism.Events;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CompasPack.ViewModel
{
    public class GroupProgramViewModel : ViewModelBase
    {
        #region Properties
        private bool _isVisibility;
        public GroupProgram GroupProgram { get; set; }
        public ObservableCollection<UserProgramViewModel> UserProgramViewModels { get; set; }
        public bool IsVisibility
        {
            get { return _isVisibility; }
            set
            {
                _isVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion
        
        #region Constructors
        public GroupProgramViewModel(GroupProgram groupProgram, ObservableCollection<UserProgramViewModel> userProgramViewModels)
        {
            SetVisibilityCommand = new DelegateCommand(OnSetVisibility);
            GroupProgram = groupProgram;
            UserProgramViewModels = userProgramViewModels;
            _isVisibility = true;
        }
        #endregion

        #region Motods
        private void OnSetVisibility()
        {
            IsVisibility = !IsVisibility;
        }
        #endregion

        #region Commands
        public ICommand SetVisibilityCommand { get; }
        #endregion
    }
}
