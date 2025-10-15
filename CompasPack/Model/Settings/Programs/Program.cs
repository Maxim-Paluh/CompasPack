using System;
using System.Collections.Generic;

namespace CompasPack.Model.Settings
{
    public class Program : ICloneable
    {
        public string ProgramName { get; set; }
        public string InstallProgramName { get; set; }
        public bool IsFree { get; set; }
        public bool DisableDefender { get; set; }
        public string Description { get; set; }
        public string PathFolder { get; set; }  
        public string FileImage { get; set; }
        public string FileName { get; set; }
        public List<string> Arguments { get; set; }
        public OnlineInstaller OnlineInstaller { get; set; }
        public object Clone()
        {
            var program = (Program)MemberwiseClone();
            if (OnlineInstaller != null)
                program.OnlineInstaller = (OnlineInstaller)OnlineInstaller.Clone();
            return program;
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
