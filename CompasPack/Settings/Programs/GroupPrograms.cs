using CompasPack.Service;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CompasPack.Settings
{
    public class GroupsProgramsCommon : ICloneable
    {
        public List<GroupPrograms> GroupsPrograms { get; set; }

        public object Clone()
        {
            return new GroupsProgramsCommon()
            {
                GroupsPrograms = (List<GroupPrograms>)GroupsPrograms.Clone()
            };
        }
    }
    public class GroupPrograms : ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool SingleChoice { get; set; }
        public List<UserProgram> UserPrograms { get; set; }

        public object Clone()
        {
            var GroupPrograms = (GroupPrograms)this.MemberwiseClone();
            GroupPrograms.UserPrograms = (List<UserProgram>)UserPrograms.Clone();
            return GroupPrograms;
        }
    }
}
