using CompasPack.Data.Constants;
using CompasPack.Data.Providers;
using CompasPack.Helper.Service;
using CompasPack.Model.Enum;
using CompasPack.Model.Settings;
using CompasPack.Model.ViewAndViewModel;
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
    public class PhysicalDiskViewModel : ReportHardwareViewModelBase<ReportSettings>
    {
        public ObservableCollection<DiskInfo> DiskInfos { get; set; }
        private IHardwareInfoProvider _hardwareInfoProvider;

        public PhysicalDiskViewModel(IHardwareInfoProvider hardwareInfoProvider)
        {
            _hardwareInfoProvider = hardwareInfoProvider;
            DiskInfos = new ObservableCollection<DiskInfo>();
            SelectDiskCommand = new DelegateCommand(OnSelectDisk);
        }
        public void Load()
        {
            var tempListDisk = _hardwareInfoProvider.GetDiskInfos();
            foreach (var disk in tempListDisk.OrderBy(x => x.Order)) DiskInfos.Add(disk);
            if (tempListDisk.Count == 0)
                Result = "Дисків не знайдено, заповніть поле вручну!!!";
            else 
                OnSelectDisk();
        }
        private void OnSelectDisk()
        {
            Result = string.Join(" | ", DiskInfos.Where(x => x.IsSelect).Select(x => $"{x.Type}-{x.Size}"));
        }
        public ICommand SelectDiskCommand { get; }
    }

    
}


