using System;
using System.Xml.Linq;
using System.Threading.Tasks;


using CompasPack.Model.Enum;
using CompasPack.Helper.Service;
using CompasPack.Data.Providers;

namespace CompasPack.ViewModel
{
    public class MonitorReportViewModel : ReportViewModelBase, IViewModelReport
    {
        private MonitorMainViewModel _monitorMainViewModel;
        private MonitorDiagonalViewModel _monitorDiagonalViewModel;
        private MonitorAspectRatioViewModel _MonitorAspectRatioViewModel;
        private MonitorResolutionViewModel _monitorResolutionViewModel;
        private MonitorOtherViewModel _monitorOtherViewModel;

        public MonitorMainViewModel MonitorMainViewModel
        {
            get { return _monitorMainViewModel; }
            set { _monitorMainViewModel = value; }
        }
        public MonitorDiagonalViewModel MonitorDiagonalViewModel
        {
            get { return _monitorDiagonalViewModel; }
            set
            {
                _monitorDiagonalViewModel = value;
                OnPropertyChanged();
            }
        }
        public MonitorAspectRatioViewModel MonitorAspectRatioViewModel
        {
            get { return _MonitorAspectRatioViewModel; }
            set { _MonitorAspectRatioViewModel = value; }
        }
        public MonitorResolutionViewModel MonitorResolutionViewModel
        {
            get { return _monitorResolutionViewModel; }
            set { _monitorResolutionViewModel = value; }
        }
        public MonitorOtherViewModel MonitorOtherViewModel
        {
            get { return _monitorOtherViewModel; }
            set { _monitorOtherViewModel = value; }
        }      

        public MonitorReportViewModel(IFileSystemReaderWriter fileSystemReaderWriter, IFileSystemNavigator fileSystemNavigator, ReportSettingsProvider reportSettingsProvider, IMessageDialogService messageDialogService,
            MonitorMainViewModel mMVM, MonitorAspectRatioViewModel mARVM, MonitorOtherViewModel mOVM, MonitorDiagonalViewModel mDVM, MonitorResolutionViewModel mRVM) :
            base(fileSystemReaderWriter, fileSystemNavigator, reportSettingsProvider, messageDialogService,
                reportSettingsProvider.Settings.ReportPaths.MonitorReportPath,
                reportSettingsProvider.Settings.ReportPaths.MonitorPricePath,
                reportSettingsProvider.Settings.ReportPaths.MonitorReportRPF
                )
        {
            MonitorMainViewModel = mMVM;
            MonitorAspectRatioViewModel = mARVM;
            MonitorOtherViewModel = mOVM;
            MonitorDiagonalViewModel = mDVM;
            MonitorResolutionViewModel = mRVM;
        }
        public async Task LoadAsync(XDocument xDocument)
        {
            await Task.Factory.StartNew(() =>
            {
                MonitorMainViewModel.Load(xDocument);
                MonitorAspectRatioViewModel.Load(xDocument);
                MonitorOtherViewModel.Load();
                MonitorDiagonalViewModel.Load(xDocument);
                MonitorResolutionViewModel.Load(xDocument);
            });
            IsEnable = true;
        }
        protected override bool IsError()
        {
            if (string.IsNullOrWhiteSpace(MonitorMainViewModel.Brand) || string.IsNullOrWhiteSpace(MonitorOtherViewModel.Result))
            {
                _messageDialogService.ShowInfoDialog("Заповни всі поля виділені червоним", "Помилка!");
                return true;
            }
            return false;

        }

        protected override string GetHTML()
        {
            return $"<html> <head> <style> table {{ font-family: Arial; font-size: 13px; }} </style> " +
            $"</head> <body> <table> <tbody>" +
            $" <tr> <th>{IndexReport:000}</th> <td style=\"text-align:left;\"><b>{MonitorMainViewModel.Result}</b></td> <td style=\"background-color: red; text-align:center;\"><b>0</b></td> <td style=\"background-color: red; text-align:center;\"><b>0</b></td> <td style=\"text-align:center;\">{MonitorDiagonalViewModel.Result}</td> <td style=\"text-align:center;\">{MonitorAspectRatioViewModel.Result}</td> <td style=\"text-align:center;\">{MonitorResolutionViewModel.Result}</td> <td style=\"text-align:center;\">{DateTime.Now:dd.MM.yyyy}</td> </tr> " +
            $"</tbody> </table> </body> </html>";
        }

        protected override string GetReplaceText(DocxReplaceTextEnum reportViewModelEnum)
        {
            switch (reportViewModelEnum)
            {
                case DocxReplaceTextEnum.Monitor_Brand: return MonitorMainViewModel.Result;
                case DocxReplaceTextEnum.MonitorDiagonal: return MonitorDiagonalViewModel.Result;
                case DocxReplaceTextEnum.MonitorAspectRatio: return MonitorAspectRatioViewModel.Result;
                case DocxReplaceTextEnum.MonitorResolution: return MonitorResolutionViewModel.Result;
                case DocxReplaceTextEnum.MonitorOther: return MonitorOtherViewModel.Result;
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
