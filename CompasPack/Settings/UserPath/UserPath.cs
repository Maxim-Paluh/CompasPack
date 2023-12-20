using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings
{
    public class UserPath : ICloneable
    {
        public string PathFolder { get; set; }
        public string FileImage { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public override bool Equals(object? obj)
        {
            if (obj is UserPath userPath)
                return PathFolder == userPath.PathFolder && FileImage == userPath.FileImage;
            else
                return false;
        }
    }
}
