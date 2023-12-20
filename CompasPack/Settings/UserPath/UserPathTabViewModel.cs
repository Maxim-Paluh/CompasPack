using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CompasPack.Settings.Common;
using CompasPack.Wrapper;
using CompasPack.View.Service;


namespace CompasPack.Settings
{
    public class UserPathTabViewModel: BaseSettingsViewModel<UserPath, UserPathWrapper, UserPathSettingsHelper>
    {
        #region Properties

        #endregion

        #region Constructor
        public UserPathTabViewModel(IMessageDialogService messageDialogService, UserPathSettingsHelper settingsHelper) : base(messageDialogService, settingsHelper)
        {
            //SetrtfFolderPath = new DelegateCommand(OnSetrtfFolderPathExecute);
            //SetArchiveFolderPath = new DelegateCommand(OnSetArchiveFolderPath);
            Title = "Шляхи";
        }
        #endregion

        #region Metods

        #endregion

        #region Commands
        public ICommand SetrtfFolderPath { get; private set; }
        public ICommand SetArchiveFolderPath { get; private set; }

        protected override void OnSetDefaultExecute()
        {
            SettingsWrapper.rtfFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            SettingsWrapper.ArchiveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
        #endregion
    }
}
