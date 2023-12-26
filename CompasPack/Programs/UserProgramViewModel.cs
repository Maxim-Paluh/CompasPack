using CompasPack.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CompasPack.Settings;

namespace CompasPack.ViewModel
{
    public class UserProgramViewModel : ViewModelBase
    {
        #region Properties
        private bool _visibilityIsInstall;
        private bool _install;
        private bool _isInstall;
        private IEventAggregator _eventAggregator;

        public GroupPrograms GroupProgram { get; set; }
        public UserProgram UserProgram { get; set; }
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
        public UserProgramViewModel(UserProgram userProgram, GroupPrograms groupProgram, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            UserProgram = userProgram;
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
                    NameProgram = UserProgram.ProgramName,
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
            if (UserProgram.InstallProgramName != null)
            {
                if(listPrograms.Where(x => x.Contains(UserProgram.InstallProgramName, StringComparison.InvariantCultureIgnoreCase)).Count() >= 1)
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
