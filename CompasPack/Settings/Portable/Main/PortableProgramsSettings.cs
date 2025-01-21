using System;
using System.Collections.Generic;
using CompasPack.Service;

namespace CompasPack.Settings
{
    [Serializable]
    public class PortableProgramsSettings : ICloneable
    {
        public List<PortableProgram> PortableProgramsList { get; set; }
        public PortableProgramsSettings()
        {
            PortableProgramsList = new List<PortableProgram>();
        }
        public object Clone()
        {
            return new PortableProgramsSettings
            {
                PortableProgramsList = (List<PortableProgram>)PortableProgramsList.Clone()
            };
        }
    }
   
}
