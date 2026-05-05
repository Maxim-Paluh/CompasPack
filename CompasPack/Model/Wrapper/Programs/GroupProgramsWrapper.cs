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
        public GroupProgramsWrapper(GroupPrograms groupProgram)
        {
            SetVisibilityCommand = new DelegateCommand(OnSetVisibility);
            GroupProgram = groupProgram;
            _isVisibility = true;

            ProgramWrappers = new ObservableCollection<ProgramWrapper>();
            foreach (var program in groupProgram.Programs)
                ProgramWrappers.Add(new ProgramWrapper(program, this));
        }
        #endregion

        #region Motods
        public void SelectSingleProgram(string programName)
        {
            foreach (var programWrapper in ProgramWrappers)
                if (programWrapper.Program.ProgramName != programName)
                    programWrapper.NotSelectProgram();
        }
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
