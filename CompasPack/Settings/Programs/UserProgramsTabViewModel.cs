using CompasPack.Settings.Common;
using CompasPack.Settings.Programs;
using CompasPack.View.Service;
using CompasPack.Wrapper;
using CompasPack.Wrapper.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings
{
    internal class UserProgramsTabViewModel : BaseSettingsViewModel<GroupsProgramsCommon, GroupsProgramsWrapper, UserProgramsSettingsHelper>
    {
        public UserProgramsTabViewModel(IMessageDialogService messageDialogService, UserProgramsSettingsHelper settingsHelper) : base(messageDialogService, settingsHelper)
        {
        }
    }
}
