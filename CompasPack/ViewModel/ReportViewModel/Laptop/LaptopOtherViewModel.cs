using CompasPack.BL;
using CompasPack.Enum;
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
    class LaptopOtherViewModel : ReportHardWareViewModelBase, IReportViewModel, IDataErrorInfo
    {
        private Hardware? _webCam;
        private Hardware? _microphone;

        private string _laptopMonitorResolution; 
        public string LaptopMonitorResolution
        {
            get { return _laptopMonitorResolution; }
            set
            {
                _laptopMonitorResolution = value;
                OnPropertyChanged();
            }
        }

        public Hardware? WebCam
        {
            get { return _webCam; }
            set
            {
                _webCam = value;
                OnPropertyChanged();
            }
        }
        public Hardware? Microphone
        {
            get { return _microphone; }
            set
            {
                _microphone = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<Hardware> HardwareValues
        {
            get
            {
                return System.Enum.GetValues(typeof(Hardware)).Cast<Hardware>();
            }
        }
        public LaptopOtherViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;

            TestWebCamCommand = new DelegateCommand(OnTestWebCam);
            TestMicrophoneCommand = new DelegateCommand(OnTestMicrophone);
            ChangeHardwareCommand = new DelegateCommand(OnChangeHardware);
        }

        private void OnChangeHardware()
        {
            if (!string.IsNullOrWhiteSpace(LaptopMonitorResolution))
                Result = $"{LaptopMonitorResolution}, {string.Join(", ", SettingsReport.LaptopHardWares.Where(x => x.IsSelect).Select(c => c.Name))}";
            else
                Result = string.Join(", ", SettingsReport.LaptopHardWares.Where(x => x.IsSelect).Select(c => c.Name));
        }

        private void OnTestMicrophone()
        {
            OpenUrl("https://webcammictest.com/check-mic.html");
        }
        private void OnTestWebCam()
        {
            OpenUrl("https://webcammictest.com");
        }
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
        public void Load()
        {
            var resolution = MonitorHelper.GetOptimalScreenResolution();

            LaptopMonitorResolution = $"{resolution.Width}x{resolution.Height}";

            var nameResolution = string.Join(", ", MonitorHelper.GetNameResolution(resolution));
            if (!string.IsNullOrWhiteSpace(nameResolution))
                LaptopMonitorResolution += $" {nameResolution}";

            if (!string.IsNullOrWhiteSpace(LaptopMonitorResolution))
                Result = $"{LaptopMonitorResolution}, {string.Join(", ", SettingsReport.LaptopHardWares.Where(x => x.IsSelect).Select(c => c.Name))}";
            else
                Result = string.Join(", ", SettingsReport.LaptopHardWares.Where(x => x.IsSelect).Select(c => c.Name));
        }
       
        public string Error => throw new NotImplementedException();
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

        public ICommand TestWebCamCommand { get; set; }
        public ICommand TestMicrophoneCommand { get; set; }
        public ICommand ChangeHardwareCommand { get; set; }
    }
}
