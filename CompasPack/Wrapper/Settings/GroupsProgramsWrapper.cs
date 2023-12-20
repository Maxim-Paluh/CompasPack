using CompasPack.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Wrapper.Settings
{
    internal class GroupsProgramsWrapper : ModelWrapper<GroupsProgramsCommon>
    {
        public GroupsProgramsWrapper() : base(null)
        {
        }
        public GroupsProgramsWrapper(GroupsProgramsCommon model) : base(model)
        {
        }

    }
}
