using CompasPack.Service;
using System;
using System.Collections.Generic;

namespace CompasPack.Settings
{
    public class GroupPrograms : ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool SingleChoice { get; set; }
        public List<Program> Programs { get; set; }
        public object Clone()
        {
            var GroupPrograms = (GroupPrograms)MemberwiseClone();
            if(Programs!=null)
                GroupPrograms.Programs = (List<Program>)Programs.Clone();
            return GroupPrograms;
        }
    }
}
