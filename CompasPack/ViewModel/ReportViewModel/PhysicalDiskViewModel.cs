using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class PhysicalDiskViewModel : ReportViewModelBase, IReportViewModel
    {
        private Disk _selectedDisk;
        public PhysicalDiskViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
            Disks = new ObservableCollection<Disk>();
        }
        public ObservableCollection<Disk> Disks { get; set; }

        public void Load()
        {
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
                    Size = (UInt64.Parse(drive.Properties["Size"].Value.ToString()) / 1000000000).ToString();

                ushort busBuff=0;
                if (drive["BusType"] != null) ;
                    ushort.TryParse(drive["BusType"].ToString(), out busBuff);

                ushort mediaBuff=0;
                if (drive["MediaType"] != null)
                    ushort.TryParse(drive["MediaType"].ToString(), out mediaBuff);


                if (busBuff == 17)
                {
                    Disks.Add(new Disk() { Name = model, Size = Size, Type = busTypeMap[busBuff], IsSelect = true });

                }
                else if (busBuff == 3 || busBuff == 8 || busBuff == 11)
                {
                    Disks.Add(new Disk() { Name = model, Size = Size, Type = mediaTypeMap[mediaBuff], IsSelect = true });
                }
                else
                {
                    Disks.Add(new Disk() { Name = model, Size = Size, Type = $"{busTypeMap[busBuff]} {mediaTypeMap[mediaBuff]}", IsSelect = false });
                }
            }
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

    public class Disk : ViewModelBase
    {
        private bool _isSelect;

        public string Name { get; set; }
        public string Size { get; set; }


        public bool IsSelect
        {
            get { return _isSelect; }
            set
            {
                _isSelect = value;
                OnPropertyChanged();
            }
        }

        public string Type { get; set; }
    }
}


