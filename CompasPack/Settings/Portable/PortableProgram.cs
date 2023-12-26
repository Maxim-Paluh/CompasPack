using System;
using System.Collections.Generic;
using CompasPack.Service;

namespace CompasPack.Settings
{
    public class PortablePrograms : ICloneable
    {
        public List<PortableProgram> portablePrograms { get; set; }
        public PortablePrograms()
        {
            portablePrograms = new List<PortableProgram>();
        }
        public object Clone()
        {
            return new PortablePrograms
            {
                portablePrograms = (List<PortableProgram>)portablePrograms.Clone()
            };
        }
    }
    public class PortableProgram : ICloneable
    {
        public string ProgramName { get; set; }
        public string PathFolder { get; set; }
        public string FileName { get; set; }
        public object Clone()
        {
           return MemberwiseClone();
        }
    }
}
