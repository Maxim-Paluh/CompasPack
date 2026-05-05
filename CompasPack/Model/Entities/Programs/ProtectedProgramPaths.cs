using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Entities.Programs
{
    public class ProtectedProgramPaths : ICloneable
    {
        public string PathFolder { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
