using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Settings
{
    public class ProtectedProgram : ICloneable
    {
        public string Name { get; set; }
        public ProtectedProgramPaths ProtectedProgramPaths { get; set; }
        public ProtectedProgram() 
        {
            ProtectedProgramPaths = new ProtectedProgramPaths();
        }
        public object Clone()
        {
            var protectedProgram = (ProtectedProgram)MemberwiseClone();
            protectedProgram.ProtectedProgramPaths = (ProtectedProgramPaths)ProtectedProgramPaths.Clone();
            return protectedProgram;
        }
    }
}
