using CompasPack.Service;
using CompasPack.Settings.Programs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings
{
    public class ProgramsSettings : ICloneable
    {
        public ProgramsPaths ProgramsPaths { get; set; }
        public List<ProgramsSet> ProgramsSets  { get; set; }
        public List<ProtectedProgram> ProtectedPrograms { get; set; }
        public List<GroupPrograms> GroupsPrograms { get; set; }

        public ProgramsSettings()
        {
            GroupsPrograms = new List<GroupPrograms>();
        }
        public object Clone()
        {
            return new ProgramsSettings()
            {
                GroupsPrograms = (List<GroupPrograms>)GroupsPrograms.Clone()
            };
        }
    }
}
