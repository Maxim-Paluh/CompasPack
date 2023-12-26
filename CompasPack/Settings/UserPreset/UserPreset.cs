using System;
using System.Collections.Generic;

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
