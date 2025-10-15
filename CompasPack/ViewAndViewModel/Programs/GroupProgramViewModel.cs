using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CompasPack.Model.Settings;

namespace CompasPack.ViewModel
{
    public class GroupProgramViewModel : ViewModelBase
    {
        #region Properties
        private bool _isVisibility;
        public GroupPrograms GroupProgram { get; set; }
        public ObservableCollection<ProgramViewModel> ProgramViewModels { get; set; }
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
        public GroupProgramViewModel(GroupPrograms groupProgram, ObservableCollection<ProgramViewModel> programViewModels)
        {
            SetVisibilityCommand = new DelegateCommand(OnSetVisibility);
            GroupProgram = groupProgram;
            ProgramViewModels = programViewModels;
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
