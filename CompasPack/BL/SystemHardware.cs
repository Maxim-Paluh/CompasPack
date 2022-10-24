using CompasPack.Data;
using CompasPack.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.BL
{
    public static class SystemHardware
    {
        public static List<PhysicalDiskViewModel> GetPhysicalDisks()
        {
            List<PhysicalDiskViewModel> physicalDisks = new List<PhysicalDiskViewModel>();

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
                if (drive["DeviceID"] != null)
                    Size = (UInt64.Parse(drive.Properties["Size"].Value.ToString()) / 1000000000).ToString();

                string busType = "";
                ushort busBuff;
                if ((drive["BusType"] != null) &&
                    ushort.TryParse(drive["BusType"].ToString(), out busBuff) &&
                    busTypeMap.Keys.Contains(busBuff))
                    busType = busTypeMap[busBuff];

                string mediaType = "";
                ushort mediaBuff;
                if ((drive["MediaType"] != null) &&
                    ushort.TryParse(drive["MediaType"].ToString(), out mediaBuff) &&
                    mediaTypeMap.Keys.Contains(mediaBuff))
                    mediaType = mediaTypeMap[mediaBuff];
            }

            return physicalDisks;
        }


        private static readonly Dictionary<ushort, string> busTypeMap =
        new Dictionary<ushort, string>()
        {
            {0, "Unknown"},
            {1, "SCSI"},
            {2, "ATAPI"},
            {3, "ATA"},
            {4, "IEEE 1394"},
            {5, "SSA"},
            {6, "Fibre Channel"},
            {7, "USB"},
            {8, "RAID"},
            {9, "iSCSI"},
            {10, "SAS"},
            {11, "SATA"},
            {12, "SD"},
            {13, "MMC"},
            {14, "reserved"},
            {15, "File-Backed Virtual"},
            {16, "Storage Spaces"},
            {17, "NVMe"},
            {18, "Reserved"},
        };

        private static readonly Dictionary<ushort, string> mediaTypeMap =
         new Dictionary<ushort, string>()
         {
            {0, "Unspecified"},
            {3, "HDD"},
            {4, "SSD"},
            {5, "SCM"},
         };
    }

   
}
