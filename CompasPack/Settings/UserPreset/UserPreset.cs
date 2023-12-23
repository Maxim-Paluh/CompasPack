using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings
{
    public class UserPresetsCommon
    {
        public List<UserPreset> UserPresets { get; set; }
    }
    public class UserPreset : ICloneable
    {
        public string Name { get; set; }
        public List<string> InstallProgramName { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
