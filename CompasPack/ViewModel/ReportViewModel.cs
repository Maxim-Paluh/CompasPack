using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CompasPack.Enum;
using CompasPakc.BL;
using Prism.Commands;

namespace CompasPack.ViewModel
{
    public class ReportViewModel : ViewModelBase, IDetailViewModel
    {
        private string _lastIdReport;
        private TypeReport _reportType;

        private IDetailViewModel? _reportformViewModel;
        private readonly IIOManager _iOManager;

        public ReportViewModel(IIOManager iOManager)
        {
            _iOManager = iOManager;
            GenerateReportCommand = new DelegateCommand(OnGenerateReport);
        }

        private async void OnGenerateReport()
        {
            switch (ReportType)
            {
                case TypeReport.Computer:
                    ReportFormViewModel = new ComputerReportViewModel(_iOManager);
                    break;
                case TypeReport.Laptop:
                    ReportFormViewModel = new LaptopReportViewModel();
                    break;
                case TypeReport.Monitor:
                    ReportFormViewModel = new MonitorReportViewModel();
                    break;
                default:
                    ReportFormViewModel = null;
                    break;
            }

            if (ReportFormViewModel != null)
                await ReportFormViewModel.LoadAsync(null);
        }

        public TypeReport ReportType
        {
            get { return _reportType; }
            set
            {
                _reportType = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<TypeReport> ReportTypeValues
        {
            get
            {
                var t = System.Enum.GetValues(typeof(TypeReport)).Cast<TypeReport>();
                return t;
            }
        }

        public IDetailViewModel? ReportFormViewModel
        {
            get { return _reportformViewModel; }
            private set
            {
                _reportformViewModel = value;
                OnPropertyChanged();
            }
        }
        public bool HasChanges()
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync(int? Id)
        {

            //var cpu = new ManagementObjectSearcher("select * from Win32_Processor").Get().Cast<ManagementObject>().First()["MaxClockSpeed"];


           //var f = new ManagementObjectSearcher("SELECT * FROM Win32_SystemEnclosure").Get().Cast<ManagementObject>().FirstOrDefault()["ChassisTypes"];

           // foreach (ManagementObject processor in SystemEnclosure.)
           // {

           //     foreach (PropertyData property in processor.Properties)
           //     {
           //         if (property != null)
                    

           //             return;
                    

           //     }
           // }
        }
        public void Unsubscribe()
        {
           
        }

        public ICommand GenerateReportCommand { get; }
    }
}
