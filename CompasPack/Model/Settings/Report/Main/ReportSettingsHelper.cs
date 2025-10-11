using CompasPack.Helper;
using CompasPack.View.Service;

namespace CompasPack.Settings
{
    public class ReportSettingsHelper : SettingsHelperBase<ReportSettings>
    {
        public ReportSettingsHelper(IIOHelper iOHelper, IMessageDialogService messageDialogService) : base(iOHelper, messageDialogService, "ReportSettings")
        {
        }
    }
}
