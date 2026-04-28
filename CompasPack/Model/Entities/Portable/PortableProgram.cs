using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Entities.Portable
{
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
