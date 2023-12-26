using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CompasPack.Settings;

namespace CompasPack.ViewModel
{
    public class GroupProgramViewModel : ViewModelBase
    {
        #region Properties
        private bool _isVisibility;
        public GroupPrograms GroupProgram { get; set; }
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
        public GroupProgramViewModel(GroupPrograms groupProgram, ObservableCollection<UserProgramViewModel> userProgramViewModels)
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
