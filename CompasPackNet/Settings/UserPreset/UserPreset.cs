using CompasPack.Service;
using System;
using System.Collections.Generic;

namespace CompasPack.Settings
{
    public class UserPresetsCommon : ICloneable
    {
        public List<UserPreset> UserPresets { get; set; }
        public UserPresetsCommon() 
        {
            UserPresets = new List<UserPreset>();
        }
        public object Clone()
        {
            return new UserPresetsCommon()
            {
                UserPresets = (List<UserPreset>)UserPresets.Clone()
            };
        }
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
