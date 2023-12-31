using CompasPack.View.Service;
using System;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using CompasPack.Settings;
using CompasPack.Helper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;

namespace CompasPack.ViewModel
{
    public class MonitorReportViewModel : ReportViewModelBase, IDetailViewModel
    {
        private MonitorMainViewModel _monitorMainViewModel;
        private MonitorOtherViewModel _monitorOtherViewModel;
        private MonitorAspectRatioViewModel _MonitorAspectRatioViewModel;
        private MonitorDiagonalViewModel _monitorDiagonalViewModel;
        private MonitorResolutionViewModel _monitorResolutionViewModel;

        public MonitorResolutionViewModel MonitorResolutionViewModel
        {
            get { return _monitorResolutionViewModel; }
            set { _monitorResolutionViewModel = value; }
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
        public MonitorOtherViewModel MonitorOtherViewModel
        {
            get { return _monitorOtherViewModel; }
            set { _monitorOtherViewModel = value; }
        }
        public MonitorMainViewModel MonitorMainViewModel
        {
            get { return _monitorMainViewModel; }
            set { _monitorMainViewModel = value; }
        }
        public MonitorReportViewModel(IIOHelper iOHelper, ReportSettings reportSettings, UserPath userPath, XDocument xDocument, IMessageDialogService messageDialogService) :
            base(iOHelper, reportSettings, userPath, xDocument, messageDialogService)
        {
            ReportPath = userPath.ReportPathSettings.MonitorReportPath;
            ReportPricePath = userPath.ReportPathSettings.MonitorPricePath;
            RPFFilePath = _userPath.ReportPathSettings.MonitorReportRPF;
        }
        public async Task LoadAsync(int? Id)
        {
            MonitorMainViewModel = new MonitorMainViewModel(_reportSettings.MonitorReportSettings, _xDocument);
            MonitorAspectRatioViewModel = new MonitorAspectRatioViewModel(_reportSettings.MonitorReportSettings, _xDocument);
            MonitorOtherViewModel = new MonitorOtherViewModel(_reportSettings.MonitorReportSettings, _xDocument);
            MonitorDiagonalViewModel = new MonitorDiagonalViewModel(_reportSettings.MonitorReportSettings, _xDocument);
            MonitorResolutionViewModel = new MonitorResolutionViewModel(_reportSettings.MonitorReportSettings, _xDocument);

            await Task.Factory.StartNew(() =>
            {
                MonitorMainViewModel.Load();
                MonitorAspectRatioViewModel.Load();
                MonitorOtherViewModel.Load();
                MonitorDiagonalViewModel.Load();
                MonitorResolutionViewModel.Load();
            });

            IndexReport = GetLastReport(ReportPath) + 1;
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

        protected override async Task WriteHTML(string path)
        {
            string html = $"<html> <head> <style> table {{ font-family: Arial; font-size: 13px; }} </style> " +
                $"</head> <body> <table> <tbody>" +
                $" <tr> <th>{IndexReport:000}</th> <td style=\"text-align:left;\"><b>{MonitorMainViewModel.Result}</b></td> <td style=\"background-color: red; text-align:center;\"><b>0</b></td> <td style=\"background-color: red; text-align:center;\"><b>0</b></td> <td style=\"text-align:center;\">{MonitorDiagonalViewModel.Result}</td> <td style=\"text-align:center;\">{MonitorAspectRatioViewModel.Result}</td> <td style=\"text-align:center;\">{MonitorResolutionViewModel.Result}</td> <td style=\"text-align:center;\">{DateTime.Now:dd.MM.yyyy}</td> </tr> " +
                $"</tbody> </table> </body> </html>";
            await _iOHelper.WriteAllTextAsync(path, html);
        }

        protected override string GetReplaceText(DocxReplaceTextEnum reportViewModelEnum)
        {
            switch (reportViewModelEnum)
            {
                case DocxReplaceTextEnum.Monitor_Brand:         return MonitorMainViewModel.Result;
                case DocxReplaceTextEnum.MonitorDiagonal:       return MonitorDiagonalViewModel.Result;
                case DocxReplaceTextEnum.MonitorAspectRatio:    return MonitorAspectRatioViewModel.Result;
                case DocxReplaceTextEnum.MonitorResolution:     return MonitorResolutionViewModel.Result;
                case DocxReplaceTextEnum.MonitorOther:          return MonitorOtherViewModel.Result;
                case DocxReplaceTextEnum.ReportId:              return $"{IndexReport:000}";

                default:
                    return null;
            }
        }
        public bool HasChanges()
        {
            return false;
        }
        public void Unsubscribe()
        {

        }
    }
}
