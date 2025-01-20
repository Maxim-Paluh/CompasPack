using CompasPack.Helper;
using CompasPack.View.Service;

namespace CompasPack.Settings
{
    public class ReportSettingsSettingsHelper : SettingsHelperBase<ReportSettings>
    {
        public ReportSettingsSettingsHelper(IIOHelper iOHelper, IMessageDialogService messageDialogService) : base(iOHelper, messageDialogService, "ReportSettings")
        {
        }
    }
}
