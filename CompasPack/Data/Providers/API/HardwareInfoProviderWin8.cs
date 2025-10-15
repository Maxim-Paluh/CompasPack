using CompasPack.Data.Constants;
using CompasPack.Model.ViewAndViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Windows;

namespace CompasPack.Data.Providers
{
    public class HardwareInfoProviderWin8 : HardwareInfoProviderBase
    {
        public override List<DiskInfo> GetDiskInfos()
        {
            var tempListDisk = new List<DiskInfo>();

            var scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
            scope.Connect();

            var searcher = new ManagementObjectSearcher("SELECT * FROM MSFT_PhysicalDisk");
            searcher.Scope = scope;

            foreach (var drive in searcher.Get())
            {
                string model = "";
                if (drive["Model"] != null)
                    model = drive["Model"].ToString();

                string Size = "";
                if (drive["Size"] != null)
                {
                    var temp = UInt64.Parse(drive.Properties["Size"].Value.ToString());
                    if (temp / 1000000000 < 1000)
                        Size = $"{temp / 1000000000}Gb";
                    else
                    {
                        double tempTb = (double)temp / 1000000000 / 1000;
                        Size = $"{Math.Round(tempTb, 3)}Tb";
                    }
                }

                ushort busBuff = 0;
                if (drive["BusType"] != null)
                    ushort.TryParse(drive["BusType"].ToString(), out busBuff);

                ushort mediaBuff = 0;
                if (drive["MediaType"] != null)
                    ushort.TryParse(drive["MediaType"].ToString(), out mediaBuff);


                if (busBuff == 17) //NVMe
                    tempListDisk.Add(new DiskInfo() { Name = model, Size = Size, Type = PhysicalDiskTypeList.BusTypeMap[busBuff], IsSelect = true, Order = 0 });
                else if (busBuff == 3 || busBuff == 8 || busBuff == 11) //ATA RAID SATA
                {
                    var temp = new DiskInfo() { Name = model, Size = Size, Type = PhysicalDiskTypeList.MediaTypeMap[mediaBuff], IsSelect = true };
                    if (mediaBuff == 4) // SSD
                        temp.Order = 1;
                    if (mediaBuff == 3) // HDD
                        temp.Order = 2;
                    tempListDisk.Add(temp);
                }
                else //Other
                    tempListDisk.Add(new DiskInfo() { Name = model, Size = Size, Type = $"{PhysicalDiskTypeList.BusTypeMap[busBuff]} {PhysicalDiskTypeList.MediaTypeMap[mediaBuff]}", IsSelect = false, Order = 3 });
            }

            return tempListDisk;
        }
    }
}
