using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Settings
{
    public class ProgramsPaths : ICloneable
    {
        public string PathFolderPrograms { get; set; }
        public string PathFolderImageProgram { get; set; }
        public string PathExampleFile { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
