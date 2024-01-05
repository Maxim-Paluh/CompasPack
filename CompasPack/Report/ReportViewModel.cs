using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using CompasPack.Enum;
using CompasPack.Helper;
using CompasPack.Settings;
using CompasPack.View.Service;
using Prism.Commands;

namespace CompasPack.ViewModel
{
    public class ReportViewModel : ViewModelBase, IDetailViewModel
    {
        private TypeReport _reportType;
        private IDetailViewModel _reportformViewModel;
        private readonly IIOHelper _iOHelper;
        private IMessageDialogService _messageDialogService;
        private readonly ReportSettingsSettingsHelper _reportSettingsSettingsHelper;
        private readonly UserPathSettingsHelper _userPathSettingsHelper;
        private UserPath _userPath;
        private XDocument _xDocument;
        private bool _isEnabled;

        public TypeReport ReportType
        {
            get { return _reportType; }
            set
            {
                _reportType = value;
                IsEnabled = true;
                OnPropertyChanged();
            }
        }
        public IEnumerable<TypeReport> ReportTypeValues
        {
            get
            {
                return System.Enum.GetValues(typeof(TypeReport)).Cast<TypeReport>();
            }
        }
        public IDetailViewModel ReportFormViewModel
        {
            get { return _reportformViewModel; }
            private set
            {
                _reportformViewModel = value;
                OnPropertyChanged();
            }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsChanges { get; set; } 
        public ReportViewModel(IIOHelper iOHelper, IMessageDialogService messageDialogService, ReportSettingsSettingsHelper reportSettingsSettingsHelper, UserPathSettingsHelper userPathSettingsHelper)
        {
            _iOHelper = iOHelper;
            GenerateReportCommand = new DelegateCommand(OnGenerateReport);
            SelectReportTypeCommand = new DelegateCommand(OnSelectReportType);
            _messageDialogService = messageDialogService;
            _reportSettingsSettingsHelper = reportSettingsSettingsHelper;
            _userPathSettingsHelper = userPathSettingsHelper;
            IsEnabled = true;
            IsChanges = false;
        }

        public Task LoadAsync(int? Id)
        {
            _userPath = (UserPath)_userPathSettingsHelper.Settings.Clone();
            PathHelper.SetRootPath(_iOHelper.PathRoot, _userPath);
            return Task.CompletedTask;
        }


        private void OnSelectReportType()
        {
            switch (ReportType)
            {
                case TypeReport.Computer:
                    if (ReportFormViewModel is ComputerReportViewModel)
                        IsEnabled = false;
                    break;
                case TypeReport.Laptop:
                    if (ReportFormViewModel is LaptopReportViewModel)
                        IsEnabled = false;
                    break;
                case TypeReport.Monitor:
                    if (ReportFormViewModel is MonitorReportViewModel)
                        IsEnabled = false;
                    break;
                default:
                    break;
            }
        }

        private async void OnGenerateReport()
        {
            IsEnabled = false;
            IsChanges = true;
            var tempLoad = new LoadViewModel { Message = "Генерування та завантаження звіту Aida..." };
            ReportFormViewModel = tempLoad;

            bool tempAidaReport = false;
            if (_xDocument == null)
            {
               
#if DEBUG
                if (!File.Exists(Path.Combine(_iOHelper.CompasPackLog, "Report.xml")))
                {
                    try
                    {
                        await AidaReportHelper.GetAidaReport(_userPath.ReportPathSettings.AidaExeFilePath, Path.Combine(_iOHelper.CompasPackLog, "Report."),
                                                                           "/XML", _userPath.ReportPathSettings.ReportRPF);
                        tempAidaReport = true;
                    }
                    catch (Exception)
                    {
                        tempAidaReport = false;
                    }   
                }
                else
                    tempAidaReport = true;
#else
                    try
                    {
                        await AidaReportHelper.GetAidaReport(_userPath.ReportPathSettings.AidaExeFilePath, Path.Combine(_iOHelper.CompasPackLog, "Report."),
                                                                           "/XML", _userPath.ReportPathSettings.ReportRPF);
                        tempAidaReport = true;
                    }
                    catch (Exception)
                    {
                        tempAidaReport = false;
                    }         
#endif
            }
            
            if (tempAidaReport)
            {
               // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using (var stream = new StreamReader($"{Path.Combine(_iOHelper.CompasPackLog, "Report.xml")}", Encoding.GetEncoding("windows-1251")))
                {

                    _xDocument = await Task.Factory.StartNew(() => XDocument.Load(stream, LoadOptions.PreserveWhitespace));
                }
            }

            if (_xDocument != null)
            {
                IDetailViewModel tempReportFormViewModel;
                tempLoad.Message = "Побудова звіту...";
                switch (ReportType)
                {
                    case TypeReport.Computer:
                        tempReportFormViewModel = new ComputerReportViewModel(_iOHelper, _reportSettingsSettingsHelper.Settings, _userPath, _xDocument, _messageDialogService);
                        break;
                    case TypeReport.Laptop:
                        tempReportFormViewModel = new LaptopReportViewModel(_iOHelper, _reportSettingsSettingsHelper.Settings, _userPath, _xDocument, _messageDialogService);
                        break;
                    case TypeReport.Monitor:
                        tempReportFormViewModel = new MonitorReportViewModel(_iOHelper, _reportSettingsSettingsHelper.Settings, _userPath, _xDocument, _messageDialogService);
                        break;
                    default:
                        tempReportFormViewModel = null;
                        break;
                }
                if(tempReportFormViewModel != null)
                    await tempReportFormViewModel.LoadAsync(null);

                ReportFormViewModel = tempReportFormViewModel;
            }
            else
            {
                tempLoad.Message = "Неможливо сформувати звіт, оскільки не вдалося створити звіт Aida... Перевірте налаштування шляхів і шаблонів для звіту!";
                tempLoad.IsActive = false;
            }
            IsChanges = false;
        }
        public bool HasChanges()
        {
            if (ReportFormViewModel != null)
                return IsChanges || ReportFormViewModel.HasChanges();
            else return IsChanges;
        }
        public void Unsubscribe()
        {

        }
        public ICommand GenerateReportCommand { get; }
        public ICommand SelectReportTypeCommand { get; }
    }
}
