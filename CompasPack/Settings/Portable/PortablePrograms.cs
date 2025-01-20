using System;
using System.Collections.Generic;
using CompasPack.Service;

namespace CompasPack.Settings
{
    [Serializable]
    public class PortablePrograms : ICloneable
    {
        public List<PortableProgram> PortableProgramsList { get; set; }
        public PortablePrograms()
        {
            PortableProgramsList = new List<PortableProgram>();
        }
        public object Clone()
        {
            return new PortablePrograms
            {
                PortableProgramsList = (List<PortableProgram>)PortableProgramsList.Clone()
            };
        }
    }
    [Serializable]
    public class PortableProgram : ICloneable
    {
        public string ProgramName { get; set; }
        public string Path { get; set; }
        public object Clone()
        {
           return MemberwiseClone();
        }
    }
}
