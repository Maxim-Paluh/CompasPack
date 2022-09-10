using CompasPac.BL;
using CompasPac.Data;
using CompasPac.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CompasPac.ViewModel
{
    public class UserProgramViewModel : ViewModelBase
    {
        private Visibility _visibility;
        private Brush _background;
        private Brush _isinstallBackground;
        private IEventAggregator _eventAggregator;

        public UserProgramViewModel(UserProgram userProgram, GroupProgram groupProgram, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            UserProgram = userProgram;
            GroupProgram = groupProgram;
            SelectProgramCommand = new DelegateCommand(OnSelectProgram);
            Background = new SolidColorBrush(Colors.LightGray);
            IsInstall = new SolidColorBrush(Colors.Red);
            VisibilityIsInstall = Visibility.Hidden;
        }

        public GroupProgram GroupProgram { get; set; }
        public UserProgram UserProgram { get; set; }
        public Brush Background
        {
            get { return _background; }
            set
            {
                _background = value;
                OnPropertyChanged();
            }

        }
        public Brush IsInstall
        {
            get { return _isinstallBackground; }
            set
            {
                _isinstallBackground = value;
                OnPropertyChanged();
            }
        }
        public Visibility VisibilityIsInstall
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }
        public bool Install { get; set; }
        private void OnSelectProgram()
        {
            Install = !Install;
            if (Install)
                Background = new SolidColorBrush(Colors.NavajoWhite);
            else
                Background = new SolidColorBrush(Colors.LightGray);

            if (GroupProgram.SingleChoice)
            {
                _eventAggregator.GetEvent<SelectSingleProgramEvent>().Publish(new SelectSingleProgramEventArgs()
                {
                    IdProgram = UserProgram.Id,
                    IdGroup = GroupProgram.Id,
                });
            }
        }
        public void SelectProgram()
        {
            Install = true;
            Background = new SolidColorBrush(Colors.NavajoWhite);
            if (GroupProgram.SingleChoice)
            {
                _eventAggregator.GetEvent<SelectSingleProgramEvent>().Publish(new SelectSingleProgramEventArgs()
                {
                    IdProgram = UserProgram.Id,
                    IdGroup = GroupProgram.Id,
                });
            }
        }
        public void NotSelectProgram()
        {
            Install = false;
            Background = new SolidColorBrush(Colors.LightGray);
        }

        public void CheckInstall(List<string> listPrograms)
        {
            if (UserProgram.InstallProgramName != null)
            {
                if (WinInfo.IsInstallPrograms(listPrograms, UserProgram.InstallProgramName))
                    IsInstall = new SolidColorBrush(Colors.Green);
                else
                    IsInstall = new SolidColorBrush(Colors.Red);
                VisibilityIsInstall = Visibility.Visible;
            }
            else
            {
                IsInstall = new SolidColorBrush(Colors.Yellow);
                VisibilityIsInstall = Visibility.Hidden;
            }

        }

        public ICommand SelectProgramCommand { get; }

    }
}
