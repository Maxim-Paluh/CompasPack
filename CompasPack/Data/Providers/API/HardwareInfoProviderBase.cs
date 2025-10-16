using System;
using System.Linq;
using System.Windows;
using System.Management;
using System.Collections.Generic;

using CompasPack.Model.ViewAndViewModel;

namespace CompasPack.Data.Providers
{
    public abstract class HardwareInfoProviderBase : IHardwareInfoProvider
    {
        public virtual CPUInfo GetCPUInfo()
        {
            CPUInfo info = new CPUInfo();
            ManagementObjectSearcher processors = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            var processor = processors.Get().Cast<ManagementObject>().First();
            info.Name += processor["Name"];
            var tempClock = processor["MaxClockSpeed"];
            if (tempClock != null)
                info.Clock += $"{(double.Parse(tempClock.ToString()) / 1000).ToString().Replace(',', '.')}GHz";
            return info;
        }
        public virtual MemoryInfo GetMemoryInfo()
        {
            MemoryInfo memoryInfo = new MemoryInfo();

            ManagementObjectSearcher memorys = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            UInt64 total = 0;
            foreach (ManagementObject ram in memorys.Get())
                total += (UInt64)ram.GetPropertyValue("Capacity");
            memoryInfo.Size = total / 1073741824 + "Gb";

            return memoryInfo;
        }
        public virtual Size GetScreenResolution()
        {
            uint maxHResolution = 0;
            uint maxVResolution = 0;
            uint maxHForMaxVResolution = 0;
            var searcher = new ManagementObjectSearcher("SELECT * FROM CIM_VideoControllerResolution");

            foreach (ManagementObject item in searcher.Get())
            {
                if ((uint)item["HorizontalResolution"] >= maxHResolution)
                {
                    maxHResolution = (uint)item["HorizontalResolution"];
                    if ((uint)item["VerticalResolution"] > maxVResolution || maxHForMaxVResolution != maxHResolution)
                    {
                        maxVResolution = (uint)item["VerticalResolution"];
                        maxHForMaxVResolution = (uint)item["HorizontalResolution"];
                    }
                }
            }

            return new Size(maxHResolution, maxVResolution);
        }
        public virtual List<DiskInfo> GetDiskInfos()
        {
            return new List<DiskInfo>();
        }

        public List<VideoControllerInfo> GetVideoControllers()
        {
            var tempVideoController = new List<VideoControllerInfo>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject mo in searcher.Get())
            {
                var tempVideoControllerName = string.Empty;
                var tempVideoControllerSize = string.Empty;

                var tempDescription = mo["Description"];
                var tempAdapterRAM = mo["AdapterRAM"];
                if (tempDescription != null)
                    tempVideoControllerName += tempDescription.ToString();
                else
                    tempVideoControllerSize = "Not found";
                if (tempAdapterRAM != null)
                    tempVideoControllerSize += $"({Math.Round(double.Parse(tempAdapterRAM.ToString()) / 1073741824, 1, MidpointRounding.AwayFromZero)}Gb)";
                else
                    tempVideoControllerSize = "Not found";
                tempVideoController.Add(new VideoControllerInfo() { Name = tempVideoControllerName, MemorySize = tempVideoControllerSize });
            }
            return tempVideoController;
        }
    }
}
