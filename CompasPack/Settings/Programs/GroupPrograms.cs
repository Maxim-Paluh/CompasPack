using CompasPack.Service;
using System;
using System.Collections.Generic;

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
            var GroupPrograms = (GroupPrograms)MemberwiseClone();
            GroupPrograms.UserPrograms = (List<UserProgram>)UserPrograms.Clone();
            return GroupPrograms;
        }
    }
}
