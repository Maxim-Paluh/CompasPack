using System;

namespace CompasPack.Settings
{
    public class UserPath : ICloneable
    {
        public string PathFolderPrograms { get; set; }
        public string PathFolderImageProgram { get; set; }
        public string PathExampleFile { get; set; }
        public ReportPathSettings ReportPathSettings { get; set; }
        public PortablePathSettings PortablePathSettings { get; set; }
        public UserPath() 
        {
            ReportPathSettings = new ReportPathSettings();
            PortablePathSettings = new PortablePathSettings();
        }
        public object Clone()
        {
            var UserPath = (UserPath)MemberwiseClone();
            UserPath.ReportPathSettings = (ReportPathSettings)ReportPathSettings.Clone();
            UserPath.PortablePathSettings = (PortablePathSettings)PortablePathSettings.Clone();
            return UserPath;
        }
        public override bool Equals(object? obj)
        {
            if (obj is UserPath userPath)
                return PathFolderPrograms == userPath.PathFolderPrograms && PathFolderImageProgram == userPath.PathFolderImageProgram;
            else
                return false;
        }
    }
    public class ReportPathSettings: ICloneable 
    {
        public string AidaExeFilePath { get; set; }
        public string LogInstallRPF { get; set; }
        public string ReportRPF { get; set; }
        public string MonitorReportRPF { get; set; }

        public string LaptopReportPath { get; set; }
        public string PCReportPath { get; set; }
        public string MonitorReportPath { get; set; }

        public string LaptopPricePath { get; set; }
        public string PCPricePath { get; set; }
        public string MonitorPricePath { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
    public class PortablePathSettings : ICloneable
    {
        public string RarPath { get; set; }
        public string KMSAutoPath { get; set; }
        public string KMSAutoRarPath { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

}
