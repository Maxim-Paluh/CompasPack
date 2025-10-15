using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Settings
{
    public class ProtectedProgramPaths : ICloneable
    {
        public string PathExe { get; set; }
        public string PathRar { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
