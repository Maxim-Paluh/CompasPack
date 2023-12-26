using CompasPack.Helper;
using CompasPack.View.Service;

namespace CompasPack.Settings
{
    public class UserProgramsSettingsHelper : SettringsHelperBase<GroupsProgramsCommon>
    {
        public UserProgramsSettingsHelper(IIOHelper iOHelper, IMessageDialogService messageDialogService) : base(iOHelper, messageDialogService, "UserProgramsSettings")
        {
        }
    }
}
