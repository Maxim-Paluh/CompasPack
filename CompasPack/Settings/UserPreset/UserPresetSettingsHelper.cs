using CompasPack.View.Service;
using CompasPakc.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings
{
    public class UserPresetSettingsHelper : SettringsHelperBase<UserPresetsCommon>
    {

        #region Properties
        #endregion

        #region Constructor
        public UserPresetSettingsHelper(IIOManager iIOManager, IMessageDialogService messageDialogService) : base(iIOManager, messageDialogService, "UserPresetSettings")
        {
        }
        #endregion

        #region Metods
        #endregion

    }
}
