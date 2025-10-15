using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using Prism.Commands;

using CompasPack.Model.Enum;
using Autofac.Features.Indexed;
using CompasPack.Helper.Service;
using CompasPack.Model.Settings;
using CompasPack.Data.Providers;

namespace CompasPack.ViewModel
{
    public class ReportViewModel : ViewModelBase, IViewModel
    {
        private TypeReportEnum _reportType;
        private IViewModelReport _reportformViewModel;
        private readonly IFileSystemReaderWriter _fileSystemReaderWriter;
        private readonly IFileSystemNavigator _fileSystemNavigator;
        private IMessageDialogService _messageDialogService;
        private readonly ReportSettingsProvider _reportSettingsProvider;
        private readonly IIndex<string, IViewModelReport> _formViewModelCreator;
        private ReportPaths _reportPaths;
        private XDocument _xDocument;
        private bool _isEnabled;

        public TypeReportEnum ReportType
        {
            get { return _reportType; }
            set
            {
                _reportType = value;
                IsEnabled = true;
                OnPropertyChanged();
            }
        }
        public IEnumerable<TypeReportEnum> ReportTypeValues
        {
            get
            {
                return System.Enum.GetValues(typeof(TypeReportEnum)).Cast<TypeReportEnum>();
            }
        }
        public IViewModelReport ReportFormViewModel
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

        public ReportViewModel(IFileSystemReaderWriter fileSystemReaderWriter, IFileSystemNavigator fileSystemNavigator,  IMessageDialogService messageDialogService, ReportSettingsProvider reportSettingsProvider,
             IIndex<string, IViewModelReport> formViewModelCreator)
        {
            _fileSystemReaderWriter = fileSystemReaderWriter;
            _fileSystemNavigator = fileSystemNavigator;
            _messageDialogService = messageDialogService;
            _reportSettingsProvider = reportSettingsProvider;
            _formViewModelCreator = formViewModelCreator;

            IsEnabled = true;
            IsChanges = false;

            GenerateReportCommand = new DelegateCommand(OnGenerateReport);
            SelectReportTypeCommand = new DelegateCommand(OnSelectReportType);
        }

        public Task LoadAsync()
        {
            _reportPaths = (ReportPaths)_reportSettingsProvider.Settings.ReportPaths.Clone();
            ProgramsHelper.SetRootPath(_fileSystemReaderWriter.PathRoot, _reportPaths); // TODO
            return Task.CompletedTask;
        }

        private void OnSelectReportType()
        {
            switch (ReportType)
            {
                case TypeReportEnum.Computer:
                    if (ReportFormViewModel is ComputerReportViewModel)
                        IsEnabled = false;
                    break;
                case TypeReportEnum.Laptop:
                    if (ReportFormViewModel is LaptopReportViewModel)
                        IsEnabled = false;
                    break;
                case TypeReportEnum.Monitor:
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
            var tempLoad = (LoadViewModel)_formViewModelCreator[typeof(LoadViewModel).Name];
            tempLoad.Message = "Генерування та завантаження звіту Aida...";

            ReportFormViewModel = tempLoad;

            bool tempAidaReport = false;
            if (_xDocument == null)
            {
               
#if DEBUG
                if (!File.Exists(Path.Combine(_fileSystemReaderWriter.CompasPackLog, "Report.xml")))
                {
                    try
                    {
                        await AidaReportHelper.GetAidaReport(_reportPaths.AidaExeFilePath, Path.Combine(_fileSystemReaderWriter.CompasPackLog, "Report."),
                                                                           "/XML", _reportPaths.ReportRPF,120);
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
                        await AidaReportHelper.GetAidaReport(_reportPaths.AidaExeFilePath, Path.Combine(_fileSystemReaderWriter.CompasPackLog, "Report."),
                                                                           "/XML", _reportPaths.ReportRPF, 120);
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
                using (var stream = new StreamReader($"{Path.Combine(_fileSystemReaderWriter.CompasPackLog, "Report.xml")}", Encoding.GetEncoding("windows-1251")))
                {

                    _xDocument = await Task.Factory.StartNew(() => XDocument.Load(stream, LoadOptions.PreserveWhitespace));
                }
            }

            if (_xDocument != null)
            {
                IViewModelReport tempReportFormViewModel;
                tempLoad.Message = "Побудова звіту...";
                switch (ReportType)
                {
                    case TypeReportEnum.Computer:
                        tempReportFormViewModel = _formViewModelCreator[typeof(ComputerReportViewModel).Name];
                        break;
                    case TypeReportEnum.Laptop:
                        tempReportFormViewModel = _formViewModelCreator[typeof(LaptopReportViewModel).Name];
                        break;
                    case TypeReportEnum.Monitor:
                        tempReportFormViewModel = _formViewModelCreator[typeof(MonitorReportViewModel).Name];
                        break;
                    default:
                        tempReportFormViewModel = null;
                        break;
                }
                if(tempReportFormViewModel != null)
                    await tempReportFormViewModel.LoadAsync(_xDocument);

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
