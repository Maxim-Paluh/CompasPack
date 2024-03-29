﻿using CompasPack.Settings;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;

namespace CompasPack.ViewModel
{
    public class VideoControllerViewModel : ReportHardWareViewModelBase<VideoControllerReportSettings>, IReportViewModel
    {
        private VideoAdapter _selectedVideoAdapter;
        private static object _lock = new object();
        public ObservableCollection<VideoAdapter> VideoAdapters { get; set; }
        public VideoAdapter SelectedVideoAdapter
        {
            get { return _selectedVideoAdapter; }
            set
            {
                _selectedVideoAdapter = value;
                OnPropertyChanged();
            }
        }
        public VideoControllerViewModel(VideoControllerReportSettings videoControllerReportSettings)
        {
            Settings = videoControllerReportSettings;
            VideoAdapters = new ObservableCollection<VideoAdapter>();
            SelectVideoAdapterCommand = new DelegateCommand(OnSelectVideoAdapter);
            BindingOperations.EnableCollectionSynchronization(VideoAdapters, _lock);
        }
        public void Load()
        {
            VideoAdapters.Clear();
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
                    tempVideoControllerSize += $"({double.Parse(tempAdapterRAM.ToString()) / 1073741824}Gb)";
                else
                    tempVideoControllerSize = "Not found";
                VideoAdapters.Add(new VideoAdapter() { Name = tempVideoControllerName, Size = tempVideoControllerSize });
            }
            SelectedVideoAdapter = VideoAdapters.First();
            OnSelectVideoAdapter();
        }
        private void OnSelectVideoAdapter()
        {
            var tempVideoAdapter = SelectedVideoAdapter.Name;
            foreach (var item in Settings.Regex)
                tempVideoAdapter = Regex.Replace(tempVideoAdapter, item, "");
            Result = $"{tempVideoAdapter} {SelectedVideoAdapter.Size}";
        }
        public ICommand SelectVideoAdapterCommand { get; }
    }
    public class VideoAdapter
    {
        public string Name { get; set; }
        public string Size { get; set; }
    }
}
