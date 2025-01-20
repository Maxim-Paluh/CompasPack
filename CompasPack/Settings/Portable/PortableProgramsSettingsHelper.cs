using CompasPack.Helper;
using CompasPack.View.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings.Portable
{
    public class PortableProgramsSettingsHelper : SettingsHelperBase<PortableProgramsSettings>
    {
        public PortableProgramsSettingsHelper(IIOHelper iOHelper, IMessageDialogService messageDialogService) : base(iOHelper, messageDialogService, "PortableProgramsSettings")
        {
        }
    }
}
