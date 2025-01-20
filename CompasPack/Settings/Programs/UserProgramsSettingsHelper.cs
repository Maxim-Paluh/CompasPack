using CompasPack.Helper;
using CompasPack.View.Service;

namespace CompasPack.Settings
{
    public class UserProgramsSettingsHelper : SettingsHelperBase<GroupsProgramsCommon>
    {
        public UserProgramsSettingsHelper(IIOHelper iOHelper, IMessageDialogService messageDialogService) : base(iOHelper, messageDialogService, "UserProgramsSettings")
        {
        }
    }
}
