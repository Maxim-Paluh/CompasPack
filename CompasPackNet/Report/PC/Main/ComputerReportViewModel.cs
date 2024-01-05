using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CompasPack.View.Service;
using System.IO.Compression;
using CompasPack.Settings;
using CompasPack.Helper;


namespace CompasPack.ViewModel
{
    public class ComputerReportViewModel : ReportViewModelBase, IDetailViewModel
    {
        private PCCaseViewModel _pCCaseViewModel;
        private CPUViewModel _CPUViewModel;
        private MotherboardViewModel _motherboardViewModel;
        private MemoryViewModel _memoryViewModel;
        private VideoControllerViewModel _videoControllerViewModel;
        private PhysicalDiskViewModel _physicalDiskViewModel;
        private PowerSupplyViewModel _powerSupplyView;
        public PCCaseViewModel PCCaseViewModel
        {
            get { return _pCCaseViewModel; }
            set
            {
                _pCCaseViewModel = value;
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
        public MotherboardViewModel MotherboardViewModel
        {
            get { return _motherboardViewModel; }
            set
            {
                _motherboardViewModel = value;
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
        public PowerSupplyViewModel PowerSupplyViewModel
        {
            get { return _powerSupplyView; }
            set
            {
                _powerSupplyView = value;
                OnPropertyChanged();
            }
        }

        public ComputerReportViewModel(IIOHelper iOHelper, ReportSettings reportSettings, UserPath userPath, XDocument xDocument, IMessageDialogService messageDialogService) :
            base(iOHelper, reportSettings, userPath, xDocument, messageDialogService)
        {
            ReportPath = userPath.ReportPathSettings.PCReportPath;
            ReportPricePath = userPath.ReportPathSettings.PCPricePath;
            RPFFilePath = _userPath.ReportPathSettings.LogInstallRPF;
        }
        public async Task LoadAsync(int? Id)
        {
            PCCaseViewModel = new PCCaseViewModel();
            CPUViewModel = new CPUViewModel(_reportSettings.CPUReportSettings);
            MotherboardViewModel = new MotherboardViewModel(_reportSettings.MotherboardReportSettings, _xDocument);
            MemoryViewModel = new MemoryViewModel(_reportSettings.MemoryReportSettings, _xDocument);
            VideoControllerViewModel = new VideoControllerViewModel(_reportSettings.VideoControllerReportSettings);
            PhysicalDiskViewModel = new PhysicalDiskViewModel(_xDocument);
            PowerSupplyViewModel = new PowerSupplyViewModel(_reportSettings.PCPowerSupply);

            await Task.Factory.StartNew(() =>
            {
                CPUViewModel.Load();
                MotherboardViewModel.Load();
                MemoryViewModel.Load();
                VideoControllerViewModel.Load();
                PhysicalDiskViewModel.Load();
                PowerSupplyViewModel.Load();
            });
            IndexReport = GetLastReport(ReportPath) + 1;

            IsEnable = true;
        }


        protected override bool IsError()
        {
            if (string.IsNullOrWhiteSpace(PCCaseViewModel.Name) || string.IsNullOrWhiteSpace(PowerSupplyViewModel.Name) || string.IsNullOrWhiteSpace(PowerSupplyViewModel.Power) || !PowerSupplyViewModel.Power.All(char.IsDigit))
            {
                _messageDialogService.ShowInfoDialog("Заповни всі поля виділені червоним", "Помилка!");
                return true;
            }
            return false;
        }

        protected override string  GetHTML()
        {
            return $"<html><head><style>table{{font-family: Arial;font-size: 13px;}}</style>" +
            $"</head><body><table><tbody>" +
            $"<tr><th>{PCCaseViewModel.Result}</th><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
            $"<tr><td>{CPUViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
            $"<tr><td>{MotherboardViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
            $"<tr><td>{MemoryViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
            $"<tr><td>{VideoControllerViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
            $"<tr><td>{PhysicalDiskViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
            $"<tr><td>{PowerSupplyViewModel.Result}</td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
            $"<tr><td><b>ID: {IndexReport:000} (Прийшов {DateTime.Now:dd.MM.yyyy})</b></td><td style=\"background-color: red;\">0</td><td></td><td style=\"background-color: #a0a0a4;\"/></tr>" +
            $"<tr><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"/><td style=\"background-color: #a0a0a4;\"> </td></tr></tbody></table></body></html>";
        }
        protected override string GetReplaceText(DocxReplaceTextEnum reportViewModelEnum)
        {
            switch (reportViewModelEnum)
            {
                case DocxReplaceTextEnum.CPU: return CPUViewModel.Result;
                case DocxReplaceTextEnum.Motherboard: return MotherboardViewModel.Result;
                case DocxReplaceTextEnum.Memory: return MemoryViewModel.Result;
                case DocxReplaceTextEnum.VideoController: return VideoControllerViewModel.Result;
                case DocxReplaceTextEnum.PhysicalDisk: return PhysicalDiskViewModel.Result;
                case DocxReplaceTextEnum.PowerSupply: return PowerSupplyViewModel.Result;
                case DocxReplaceTextEnum.ReportId: return $"{IndexReport:000}";

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
