using CompasPack.Helper;
using CompasPack.View.Service;

namespace CompasPack.Settings
{
    public class ProgramsSettingsHelper : SettingsHelperBase<ProgramsSettings>
    {
        public ProgramsSettingsHelper(IIOHelper iOHelper, IMessageDialogService messageDialogService) : base(iOHelper, messageDialogService, "ProgramsSettings")
        {
        }
    }
}
