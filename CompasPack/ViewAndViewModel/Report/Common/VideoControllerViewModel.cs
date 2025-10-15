using CompasPack.Data.Providers;
using CompasPack.Model.Settings;
using CompasPack.Model.ViewAndViewModel;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;

namespace CompasPack.ViewModel
{
    public class VideoControllerViewModel : ReportHardwareViewModelBase<VideoController>
    {
        private VideoControllerInfo _selectedVideoController;
        public VideoControllerInfo SelectedVideoController
        {
            get { return _selectedVideoController; }
            set
            {
                _selectedVideoController = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<VideoControllerInfo> videoControllerInfos { get; set; }
        private IHardwareInfoProvider _hardwareInfoProvider;
        public VideoControllerViewModel(ReportSettingsProvider reportSettingsProvider, IHardwareInfoProvider hardwareInfoProvider)
        {
            Settings = reportSettingsProvider.Settings.VideoController;
            _hardwareInfoProvider = hardwareInfoProvider;

            videoControllerInfos = new ObservableCollection<VideoControllerInfo>();
            SelectVideoControllerCommand = new DelegateCommand(OnSelectVideoController);
        }
        public void Load()
        {
            videoControllerInfos.Clear();
            var tempListVideoController = _hardwareInfoProvider.GetVideoControllers();
            foreach (var videoController in tempListVideoController) videoControllerInfos.Add(videoController);
            if(videoControllerInfos.Count!=0)
                SelectedVideoController = videoControllerInfos.First();
            OnSelectVideoController();
        }
        private void OnSelectVideoController()
        {
            var tempVideoAdapter = Settings.Regex.Aggregate(SelectedVideoController.Name, (current, pattern) => Regex.Replace(current, pattern, ""));
            Result = $"{tempVideoAdapter} {SelectedVideoController.MemorySize}";
        }
        public ICommand SelectVideoControllerCommand { get; }
    }
    
}
