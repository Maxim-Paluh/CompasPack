using System;
using System.Collections.Generic;

using CompasPack.Helper.Extension;

namespace CompasPack.Model.Settings
{
    public class ProgramsSettings : ICloneable
    {
        public string ArchivePassword { get; set; }
        public ProgramsPaths ProgramsPaths { get; set; }
        public List<ProgramsSet> ProgramsSets  { get; set; }
        public List<ProtectedProgram> ProtectedPrograms { get; set; }
        public List<GroupPrograms> GroupsPrograms { get; set; }

        public ProgramsSettings()
        {
            ArchivePassword = string.Empty;
            ProgramsPaths = new ProgramsPaths();
            ProgramsSets = new List<ProgramsSet>();
            ProtectedPrograms = new List<ProtectedProgram>();
            GroupsPrograms = new List<GroupPrograms>();
        }
        public object Clone()
        {
            var programsSettings = (ProgramsSettings)MemberwiseClone();
            programsSettings.ProgramsPaths = (ProgramsPaths)ProgramsPaths.Clone();
            programsSettings.ProgramsSets = (List<ProgramsSet>)ProgramsSets.Clone();
            programsSettings.ProtectedPrograms = (List<ProtectedProgram>)ProtectedPrograms.Clone();
            programsSettings.GroupsPrograms = (List<GroupPrograms>)GroupsPrograms.Clone();
            return programsSettings;
        }
    }
}
