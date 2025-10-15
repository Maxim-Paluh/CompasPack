using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;

using Prism.Commands;
using Prism.Events;

using CompasPack.Helper.Event;
using CompasPack.Model.Settings;
using CompasPack.Helper.Extension;

namespace CompasPack.ViewModel
{
    public class ProgramViewModel : ViewModelBase
    {
        #region Properties
        private bool _visibilityIsInstall;
        private bool _install;
        private bool _isInstall;
        private IEventAggregator _eventAggregator;

        public GroupPrograms GroupProgram { get; set; }
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
        public ProgramViewModel(Program program, GroupPrograms groupProgram, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            Program = program;
            GroupProgram = groupProgram;
            SelectProgramCommand = new DelegateCommand(OnSelectProgram);
            IsInstall = false;
            VisibilityIsInstall = false;
        }
        #endregion

        #region Motods
        private void OnSelectProgram()
        {
            Install = !Install;
            if (GroupProgram.SingleChoice)
            {
                _eventAggregator.GetEvent<SelectSingleProgramEvent>().Publish(new SelectSingleProgramEventArgs()
                {
                    NameProgram = Program.ProgramName,
                    NameGroup = GroupProgram.Name,
                });
            }
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
            if (Program.InstallProgramName != null)
            {
                if(listPrograms.Where(x => x.Contains(Program.InstallProgramName, StringComparison.InvariantCultureIgnoreCase)).Count() >= 1)
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
