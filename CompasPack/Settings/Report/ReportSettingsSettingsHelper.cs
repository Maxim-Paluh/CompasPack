using CompasPack.View.Service;
using CompasPakc.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings
{
    public class ReportSettingsSettingsHelper : SettringsHelperBase<ReportSettings>
    {
        public ReportSettingsSettingsHelper(IIOManager iIOManager, IMessageDialogService messageDialogService) : base(iIOManager, messageDialogService, "ReportSettings")
        {
        }
    }
}
