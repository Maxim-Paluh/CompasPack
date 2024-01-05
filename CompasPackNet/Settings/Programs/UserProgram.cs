using System;
using System.Collections.Generic;

namespace CompasPack.Settings
{
    public class UserProgram : ICloneable
    {
        public string ProgramName { get; set; }
        public string? InstallProgramName { get; set; }
        public bool IsFree { get; set; }
        public bool DisableDefender { get; set; }
        public string Description { get; set; }
        public string PathFolder { get; set; }  
        public string FileImage { get; set; }
        public string Architecture { get; set; }
        public string FileName { get; set; }
        public List<string> Arguments { get; set; }
        public OnlineInstaller? OnlineInstaller { get; set; }
        public object Clone()
        {
            var UserProgram = (UserProgram)MemberwiseClone();
            if (OnlineInstaller != null)
                UserProgram.OnlineInstaller = (OnlineInstaller?)OnlineInstaller.Clone();
            return UserProgram;
        }
    }
    public class OnlineInstaller : ICloneable
    {
        public string FileName { get; set; }
        public List<string> Arguments { get; set; }
        public object Clone()
        {
            return (OnlineInstaller)MemberwiseClone();
        }
    }
}
