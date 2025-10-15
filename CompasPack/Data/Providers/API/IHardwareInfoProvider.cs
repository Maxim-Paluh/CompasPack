using System;
using System.Windows;
using System.Collections.Generic;

using CompasPack.Model.ViewAndViewModel;

namespace CompasPack.Data.Providers
{
    public interface IHardwareInfoProvider
    {
        CPUInfo GetCPUInfo();
        MemoryInfo GetMemoryInfo();
        Size GetScreenResolution();
        List<DiskInfo> GetDiskInfos();
        List<VideoControllerInfo> GetVideoControllers();
    }
}
