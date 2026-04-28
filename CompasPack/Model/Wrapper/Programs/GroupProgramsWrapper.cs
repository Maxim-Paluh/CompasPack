using System;
using System.Windows.Input;
using System.Collections.ObjectModel;

using Prism.Commands;

using CompasPack.ViewModel;
using CompasPack.Model.Entities.Programs;


namespace CompasPack.Model.Wrapper
{
    public class GroupProgramsWrapper : ViewModelBase
    {
        #region Properties
        private bool _isVisibility;
        public GroupPrograms GroupProgram { get; set; }
        public ObservableCollection<ProgramWrapper> ProgramWrappers { get; set; }
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
        public GroupProgramsWrapper(GroupPrograms groupProgram, ObservableCollection<ProgramWrapper> programWrappers)
        {
            SetVisibilityCommand = new DelegateCommand(OnSetVisibility);
            GroupProgram = groupProgram;
            ProgramWrappers = programWrappers;
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
