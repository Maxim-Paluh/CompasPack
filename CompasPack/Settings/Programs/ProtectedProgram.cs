using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings.Programs
{
    public class ProtectedProgram : ICloneable
    {
        public string Name { get; set; }
        public string PathExe { get; set; }
        public string PathRar { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
