using CompasPack.Helper;
using CompasPack.Settings;
using CompasPack.View.Service;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Drawing;

namespace CompasPack.ViewModel
{
    internal class LaptopReportViewModel : ReportViewModelBase, IDetailViewModel
    {   
        private LaptopMainViewModel _laptopMainViewModel;
        private MonitorDiagonalViewModel _monitorDiagonalViewModel;
        private CPUViewModel _CPUViewModel;
        private MemoryViewModel _memoryViewModel;
        private VideoControllerViewModel _videoControllerViewModel;
        private PhysicalDiskViewModel _physicalDiskViewModel;
        private LaptopBatteryViewModel _laptopBatteryViewModel;
        private LaptopOtherViewModel _laptopOtherViewModel;

        public MonitorDiagonalViewModel MonitorDiagonalViewModel
        {
            get { return _monitorDiagonalViewModel; }
            set
            {
                _monitorDiagonalViewModel = value;
                OnPropertyChanged();
            }
        }
        public CPUViewModel CPUViewModel
        {
            get { return _CPUViewModel; }
            set
            {
                _CPUViewModel = value;
                OnPropertyChanged();
            }
        }
        public MemoryViewModel MemoryViewModel
        {
            get { return _memoryViewModel; }
            set
            {
                _memoryViewModel = value;
                OnPropertyChanged();
            }
        }
        public VideoControllerViewModel VideoControllerViewModel
        {
            get { return _videoControllerViewModel; }
            set
            {
                _videoControllerViewModel = value;
                OnPropertyChanged();
            }
        }
        public PhysicalDiskViewModel PhysicalDiskViewModel
        {
            get { return _physicalDiskViewModel; }
            set
            {
                _physicalDiskViewModel = value;
                OnPropertyChanged();
            }
        }
        public LaptopMainViewModel LaptopMainViewModel
        {
            get { return _laptopMainViewModel; }
            set
            {
                _laptopMainViewModel = value;
                OnPropertyChanged();
            }
        }
        public LaptopBatteryViewModel LaptopBatteryViewModel
        {
            get { return _laptopBatteryViewModel; }
            set { _laptopBatteryViewModel = value; }
        }
        public LaptopOtherViewModel LaptopOtherViewModel
        {
            get { return _laptopOtherViewModel; }
            set
            {
                _laptopOtherViewModel = value;
                OnPropertyChanged();
            }
        }
        public LaptopReportViewModel(IIOHelper iOHelper, ReportSettings reportSettings, UserPath userPath, XDocument xDocument, IMessageDialogService messageDialogService) :
            base(iOHelper, reportSettings, userPath, xDocument, messageDialogService)
        {
            ReportPath = userPath.ReportPathSettings.LaptopReportPath;
            ReportPricePath = userPath.ReportPathSettings.LaptopPricePath;
            RPFFilePath = _userPath.ReportPathSettings.LogInstallRPF;
        }
        public async Task LoadAsync(int? Id)
        {
            LaptopMainViewModel = new LaptopMainViewModel(_reportSettings.LaptopsBrandAndModel);
            MonitorDiagonalViewModel = new MonitorDiagonalViewModel(_reportSettings.MonitorReportSettings, _xDocument);
            CPUViewModel = new CPUViewModel(_reportSettings.CPUReportSettings);
            MemoryViewModel = new MemoryViewModel(_reportSettings.MemoryReportSettings, _xDocument);
            VideoControllerViewModel = new VideoControllerViewModel(_reportSettings.VideoControllerReportSettings);
            PhysicalDiskViewModel = new PhysicalDiskViewModel(_xDocument);
            LaptopOtherViewModel = new LaptopOtherViewModel(_reportSettings.LaptopHardWares, _xDocument);
            LaptopBatteryViewModel = new LaptopBatteryViewModel(_reportSettings.LaptopBatteryReportSettings, _xDocument);

            await Task.Factory.StartNew(() =>
            {
                CPUViewModel.Load();
                MonitorDiagonalViewModel.Load();
                MemoryViewModel.Load();
                VideoControllerViewModel.Load();
                PhysicalDiskViewModel.Load();
                LaptopBatteryViewModel.Load();
                LaptopOtherViewModel.Load();
            });

            IndexReport = GetLastReport(ReportPath) + 1;
            IsEnable = true;
        }

        protected override bool IsError()
        {
            if (string.IsNullOrWhiteSpace(LaptopMainViewModel.Brand) || string.IsNullOrWhiteSpace(LaptopMainViewModel.Model) || LaptopOtherViewModel.Microphone == null || LaptopOtherViewModel.WebCam == null)
            {
                _messageDialogService.ShowInfoDialog("Заповни всі поля виділені червоним", "Помилка!");
                return true;
            }
            return false;
        }

        protected override async Task WriteHTML(string path)
        {
            string html = $"<html> <head> <style> table {{ font-family: Arial; font-size: 13px; }} </style> </head> <body> <table> <tbody> " +
                $"<tr> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> </tr>" +
                $" <tr> <td style=\"text-align:right;\">{IndexReport:000}</td> <td style=\"text-align:left;\"><b>{LaptopMainViewModel.Result}</b></td> <td style=\"background-color: red; text-align:center;\"><b>0</b></td> <td style=\"background-color: red; text-align:center;\"><b>0</b></td> <td style=\"text-align:center;\">{DateTime.Now:dd.MM.yyyy}</td> <td></td> <td></td> <td></td> </tr>" +
                $"<tr> <td style=\"text-align:right;\">Cam {LaptopOtherViewModel.WebCam}</td> <td style=\"text-align:left;\">{CPUViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
                $"<tr> <td style=\"text-align:right;\">Mic {LaptopOtherViewModel.Microphone}</td> <td style=\"text-align:left;\">{MemoryViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
                $"<tr> <td></td> <td style=\"text-align:left;\">{VideoControllerViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
                $"<tr> <td></td> <td style=\"text-align:left;\">{PhysicalDiskViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
                $"<tr> <td></td> <td style=\"text-align:left;\">{LaptopOtherViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
                $"<tr> <td></td> <td style=\"text-align:left;\">{LaptopBatteryViewModel.Result}</td> <td></td> <td></td> <td></td> <td></td> <td></td> <td></td> </tr>" +
                $"<tr> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> <td style=\"background-color: #808080;\" /> </tr>" +
                $"</tbody> </table> </body> </html>";
            await _iOHelper.WriteAllTextAsync(path, html);

        }
        protected override string GetReplaceText(DocxReplaceTextEnum reportViewModelEnum)
        {
            switch (reportViewModelEnum)
            {
                case DocxReplaceTextEnum.CPU:                       return CPUViewModel.Result;
                case DocxReplaceTextEnum.Memory:                    return MemoryViewModel.Result;
                case DocxReplaceTextEnum.MonitorDiagonal:           return MonitorDiagonalViewModel.Result;
                case DocxReplaceTextEnum.PhysicalDisk:              return PhysicalDiskViewModel.Result;
                case DocxReplaceTextEnum.VideoController:           return VideoControllerViewModel.Result;

                case DocxReplaceTextEnum.LaptopBattery:             return LaptopBatteryViewModel.Result;
                case DocxReplaceTextEnum.Laptop_Brand:              return LaptopMainViewModel.Brand;
                case DocxReplaceTextEnum.Laptop_Line_Model:          
                    if(string.IsNullOrWhiteSpace(LaptopMainViewModel.Line))
                        return LaptopMainViewModel.Model;
                    else 
                        return $"{LaptopMainViewModel.Line} {LaptopMainViewModel.Model}";
                case DocxReplaceTextEnum.LaptopOther:               return LaptopOtherViewModel.Result;
                case DocxReplaceTextEnum.ReportId:                  return $"{IndexReport:000}";
                default:
                    return null;
            }
        }
        public bool HasChanges()
        {
            return !IsEnable;
        }
        public void Unsubscribe()
        {

        }
    }
}
