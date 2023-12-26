using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings
{
    public class UserPath : ICloneable
    {
        public string PathFolderPrograms { get; set; }
        public string PathFolderImageProgram { get; set; }
        public string PathExampleFile { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public override bool Equals(object? obj)
        {
            if (obj is UserPath userPath)
                return PathFolderPrograms == userPath.PathFolderPrograms && PathFolderImageProgram == userPath.PathFolderImageProgram;
            else
                return false;
        }
    }
}
