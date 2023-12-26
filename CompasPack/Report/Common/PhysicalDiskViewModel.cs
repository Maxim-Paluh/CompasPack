using CompasPack.Settings;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class PhysicalDiskViewModel : ReportHardWareViewModelBase<ReportSettings>, IReportViewModel
    {
        private static readonly Dictionary<ushort, string> busTypeMap = new Dictionary<ushort, string>()
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
        private static readonly Dictionary<ushort, string> mediaTypeMap = new Dictionary<ushort, string>()
        {
            {0, "Unspecified"},
            {3, "HDD"},
            {4, "SSD"},
            {5, "SCM"},
        };
        private static object _lock = new object();
        public ObservableCollection<Disk> Disks { get; set; }
        public PhysicalDiskViewModel(XDocument xDocument)
        {
            Document = xDocument;
            Disks = new ObservableCollection<Disk>();
            SelectDiskCommand = new DelegateCommand(OnSelectDisk);
            BindingOperations.EnableCollectionSynchronization(Disks, _lock);
        }
        public void Load()
        {
            var scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
            scope.Connect();

            var searcher = new ManagementObjectSearcher("SELECT * FROM MSFT_PhysicalDisk");
            searcher.Scope = scope;
            List<Disk> tempListDisk = new List<Disk>();
            foreach (var drive in searcher.Get())
            {
                string model = "";
                if (drive["Model"] != null)
                    model = drive["Model"].ToString();

                string Size = "";
                if (drive["Size"] != null)
                {
                    var temp = UInt64.Parse(drive.Properties["Size"].Value.ToString());
                    if(temp / 1000000000<1000)
                        Size = $"{temp/1000000000}Gb";
                    else
                    {
                        double tempTb = (double)temp / 1000000000 / 1000;
                        Size = $"{Math.Round(tempTb, 3)}Tb";
                    }
                }

                ushort busBuff=0;
                if (drive["BusType"] != null)
                    ushort.TryParse(drive["BusType"].ToString(), out busBuff);

                ushort mediaBuff=0;
                if (drive["MediaType"] != null)
                    ushort.TryParse(drive["MediaType"].ToString(), out mediaBuff);


                if (busBuff == 17) //NVMe
                    tempListDisk.Add(new Disk() { Name = model, Size = Size, Type = busTypeMap[busBuff], IsSelect = true, Order = 0 });
                else if (busBuff == 3 || busBuff == 8 || busBuff == 11) //ATA RAID SATA
                {
                    var temp = new Disk() { Name = model, Size = Size, Type = mediaTypeMap[mediaBuff], IsSelect = true };
                    if (mediaBuff == 4) // SSD
                        temp.Order = 1;
                    if (mediaBuff == 3) // HDD
                        temp.Order = 2;
                    tempListDisk.Add(temp);
                }
                else //Other
                    tempListDisk.Add(new Disk() { Name = model, Size = Size, Type = $"{busTypeMap[busBuff]} {mediaTypeMap[mediaBuff]}", IsSelect = false, Order = 3});   
            }

            foreach (var disk in tempListDisk.OrderBy(x => x.Order))
                Disks.Add(disk);
            OnSelectDisk();
        }
        private void OnSelectDisk()
        {
            Result = string.Empty;
            foreach (var item in Disks.Where(x => x.IsSelect))
                Result += $"{item.Type}-{item.Size} | ";
            Result = Result.TrimEnd(new char[] { ' ', '|' });
        }
        public ICommand SelectDiskCommand { get; }
    }

    public class Disk : ViewModelBase
    {
        private bool _isSelect;
        public bool IsSelect
        {
            get { return _isSelect; }
            set
            {
                _isSelect = value;
                OnPropertyChanged();
            }
        }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }    
    }
}


