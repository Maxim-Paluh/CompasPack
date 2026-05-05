using CompasPack.Helper.Extension;
using CompasPack.Model.Entities.Programs;
using CompasPack.Model.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace CompasPack.ViewModel
{
    public class ProgramWrapper : ViewModelBase
    {
        #region Properties
        private bool _visibilityIsInstall;
        private bool _install;
        private bool _isInstall;

        public GroupProgramsWrapper GroupProgramsWrapper { get; set; }
        public Program Program { get; set; }
        public bool Install
        {
            get{ return _install; }
            set 
            { 
                _install = value;
                OnPropertyChanged();
            }

        }
        public bool IsInstall
        {
            get { return _isInstall; }
            set
            {
                _isInstall = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Використовується для приховування чи відображення IsInstall, залежно від поля "InstallProgramName" в класі Program.
        /// Якщо string.IsNullOrWhiteSpace(Program.InstallProgramName) то іконка встановлення буде прихована, інакше буде показано статус (дивись поле Install)
        /// Цей параметр може бути зміненим лише в методі CheckInstall()
        /// </summary>
        public bool VisibilityIsInstall
        {
            get { return _visibilityIsInstall; }
            set
            {
                _visibilityIsInstall = value;
                OnPropertyChanged();
            }
        } 
        #endregion

        #region Constructors
        public ProgramWrapper(Program program, GroupProgramsWrapper groupProgramsWrapper)
        {
            Program = program;
            GroupProgramsWrapper = groupProgramsWrapper;
            SelectProgramCommand = new DelegateCommand(OnSelectProgram);
            IsInstall = false;
            VisibilityIsInstall = false;
        }
        #endregion

        #region Motods
        private void OnSelectProgram()
        {
            Install = !Install;
            if (GroupProgramsWrapper.GroupProgram.SingleChoice)
                GroupProgramsWrapper.SelectSingleProgram(Program.ProgramName);
        }
        public void SelectProgram()
        {
            if (!Install)
                OnSelectProgram();
        }
        public void NotSelectProgram()
        {
            Install = false;
        }
        public void CheckInstall(List<string> listPrograms)
        {
            if (!string.IsNullOrWhiteSpace(Program.InstallProgramName))
            {
                if(listPrograms.Where(x => x.Contains(Program.InstallProgramName, StringComparison.InvariantCultureIgnoreCase)).Any())
                    IsInstall = true;
                VisibilityIsInstall = true;
            }
        }
        #endregion

        #region Commands
        public ICommand SelectProgramCommand { get; }
        #endregion
    }
}
