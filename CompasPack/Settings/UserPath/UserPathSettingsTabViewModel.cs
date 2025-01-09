using Prism.Commands;
using System;
using System.Windows.Input;
using CompasPack.Wrapper;
using CompasPack.View.Service;


namespace CompasPack.Settings
{
    public class UserPathSettingsTabViewModel: BaseSettingsViewModel<UserPath, UserPathWrapper, UserPathSettingsHelper>
    {
        #region Properties
        #endregion

        #region Constructor
        public UserPathSettingsTabViewModel(IMessageDialogService messageDialogService, UserPathSettingsHelper settingsHelper) : base(messageDialogService, settingsHelper)
        {
            Title = "Шляхи";
        }
        #endregion

        #region Metods
       
        #endregion

        #region Commands
        #endregion
    }
}
