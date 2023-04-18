using CompasPack.View.Service;
using CompasPakc.BL;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public abstract class ReportViewModelBase : ViewModelBase
    {
        protected SettingsReportViewModel _settingsReportViewModel;
        protected XDocument _xDocument;
        protected IIOManager _ioManager;
        protected IMessageDialogService _messageDialogService;

        private bool _isEnable;
        private string _reportPath;
        private int _indexReport;

        public ReportViewModelBase(IIOManager iOManager, SettingsReportViewModel settingsReportViewModel, XDocument xDocument, IMessageDialogService messageDialogService)
        {
            IsEnable = false;
            _ioManager = iOManager;
            _settingsReportViewModel = settingsReportViewModel;
            _xDocument = xDocument;
            _messageDialogService = messageDialogService;
            SaveReportCommand = new DelegateCommand(OnSaveReport);

            OpenReportCommand = new DelegateCommand(OnOpenReport);
            OpenPriceCommand = new DelegateCommand(OnOpenPrice);
            OpenFolderCommand = new DelegateCommand(OnOpenFolder);
        }

        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged();
            }
        }
        public string ReportPath
        {
            get { return _reportPath; }
            set
            {
                _reportPath = value;
                OnPropertyChanged();
            }
        }
        public int IndexReport
        {
            get { return _indexReport; }
            set
            {
                _indexReport = value;
                OnPropertyChanged();
            }
        }
       
        public ReportViewModelBase()
        {
            SaveReportCommand = new DelegateCommand(OnSaveReport);

            OpenReportCommand = new DelegateCommand(OnOpenReport);
            OpenPriceCommand = new DelegateCommand(OnOpenPrice);
            OpenFolderCommand = new DelegateCommand(OnOpenFolder);
        }  
        protected abstract void OnSaveReport();


        protected void OnOpenReport()
        {
            if (!File.Exists($"{ReportPath}\\Report_{IndexReport:000}.html"))
                _messageDialogService.ShowInfoDialog("Такого файлу нема!", "Помилка!");
            _ioManager.OpenFolderAndSelectFile($"{ReportPath}\\Report_{IndexReport:000}.html");
        }
        protected void OnOpenPrice()
        {
            if (!File.Exists($"{ReportPath}\\Report_{IndexReport:000}.docx"))
                _messageDialogService.ShowInfoDialog("Такого файлу нема!", "Помилка!");
            _ioManager.OpenFolderAndSelectFile($"{ReportPath}\\Report_{IndexReport:000}.docx");
        }
        protected void OnOpenFolder()
        {
            _ioManager.OpenFolder(ReportPath);
        }

        public ICommand SaveReportCommand { get; set; }
        public ICommand OpenReportCommand { get; set; }
        public ICommand OpenPriceCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
    }
}
