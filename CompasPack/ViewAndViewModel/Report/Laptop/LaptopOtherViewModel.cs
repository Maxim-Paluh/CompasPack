using CompasPack.Data.Constants;
using CompasPack.Data.Providers;
using CompasPack.Helper.Service;
using CompasPack.Model.Enum;
using CompasPack.Model.Settings;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    class LaptopOtherViewModel : ReportHardwareViewModelBase<LaptopInterface>, IDataErrorInfo
    {
        private HardwareStatusEnum? _webCam;
        private HardwareStatusEnum? _microphone;
        private string _laptopMonitorResolution; 
        private readonly IHardwareInfoProvider _hardwareInfoProvider;
        public string LaptopMonitorResolution
        {
            get { return _laptopMonitorResolution; }
            set
            {
                _laptopMonitorResolution = value;
                OnPropertyChanged();
            }
        }
        public HardwareStatusEnum? WebCam
        {
            get { return _webCam; }
            set
            {
                _webCam = value;
                OnPropertyChanged();
            }
        }
        public HardwareStatusEnum? Microphone
        {
            get { return _microphone; }
            set
            {
                _microphone = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<HardwareStatusEnum> HardwareStatusValues
        {
            get
            {
                return System.Enum.GetValues(typeof(HardwareStatusEnum)).Cast<HardwareStatusEnum>();
            }
        }
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "WebCam":
                        if (WebCam == null)
                            error = "Введи значення";
                        break;
                    case "Microphone":
                        if (Microphone == null)
                            error = "Введи значення";
                        break;
                }
                return error;
            }
        }
        public string Error => throw new NotImplementedException();
        public LaptopOtherViewModel(ReportSettingsProvider reportSettingsProvider, IHardwareInfoProvider hardwareInfoProvider)
        {
            Settings = reportSettingsProvider.Settings.LaptopInterface;
            _hardwareInfoProvider = hardwareInfoProvider;

            TestWebCamCommand = new DelegateCommand(OnTestWebCam);
            TestMicrophoneCommand = new DelegateCommand(OnTestMicrophone);
            ChangeHardwareCommand = new DelegateCommand(OnChangeHardware);
        }
        public void Load()
        {
            var resolution = _hardwareInfoProvider.GetScreenResolution();
            var nameResolution = string.Join(", ", ResolutionNameList.GetNameResolution(resolution));

            LaptopMonitorResolution = $"{resolution.Width}x{resolution.Height}" + (string.IsNullOrWhiteSpace(nameResolution) ? string.Empty : $" {nameResolution}");
            OnChangeHardware();
        }
        private void OnChangeHardware()
        {
            Result = string.Join(", ", new[] { LaptopMonitorResolution }
                            .Concat(Settings.LaptopPorts.Where(x => x.IsSelect).Select(c => c.Name))
                            .Where(s => !string.IsNullOrWhiteSpace(s)))+".";
        }
        private void OnTestMicrophone()
        {
            NetworkService.OpenUrl(Settings.TestMicrophoneURL);
        }
        private void OnTestWebCam()
        {
            NetworkService.OpenUrl(Settings.TestWebCamURL);
        }
        
        public ICommand TestWebCamCommand { get; set; }
        public ICommand TestMicrophoneCommand { get; set; }
        public ICommand ChangeHardwareCommand { get; set; }
    }
}
