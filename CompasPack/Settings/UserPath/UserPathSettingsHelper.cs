using CompasPack.View.Service;
using CompasPakc.BL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;


namespace CompasPack.Settings
{
    public class UserPathSettingsHelper : SettringsHelperBase<UserPath>
    {
        #region Properties
        #endregion

        #region Constructor
        public UserPathSettingsHelper(IIOManager iIOManager, IMessageDialogService messageDialogService) : base(iIOManager, messageDialogService, "UserPathSettings")
        {   
        }
        #endregion

        #region Metods
        #endregion
    }
   
}
