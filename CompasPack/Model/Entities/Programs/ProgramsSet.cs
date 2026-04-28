using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Entities.Programs
{
    public class ProgramsSet : ICloneable
    {
        public string Name { get; set; }
        public List<string> InstallProgramName { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
