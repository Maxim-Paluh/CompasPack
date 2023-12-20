using CompasPack.View.Service;
using CompasPakc.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings.Programs
{
    public class UserProgramsSettingsHelper : SettringsHelperBase<GroupsProgramsCommon>
    {
        public UserProgramsSettingsHelper(IIOManager iIOManager, IMessageDialogService messageDialogService) : base(iIOManager, messageDialogService, "UserProgramsSettings")
        {
        }
    }
}
