using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Data.Constants
{
    public static class PhysicalDiskTypeList
    {
        public static readonly Dictionary<ushort, string> BusTypeMap = new Dictionary<ushort, string>()
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
        public static readonly Dictionary<ushort, string> MediaTypeMap = new Dictionary<ushort, string>()
        {
            {0, "Unspecified"},
            {3, "HDD"},
            {4, "SSD"},
            {5, "SCM"},
        };
    }
}
