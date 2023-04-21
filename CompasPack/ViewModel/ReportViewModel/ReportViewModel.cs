﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using CompasPack.Enum;
using CompasPack.View.Service;
using CompasPakc.BL;
using Prism.Commands;

namespace CompasPack.ViewModel
{
    public class ReportViewModel : ViewModelBase, IDetailViewModel
    {
        private TypeReport _reportType;
        private IDetailViewModel? _reportformViewModel;
        private readonly IIOManager _ioManager;
        private IMessageDialogService _messageDialogService;

        private SettingsReportViewModel _settingsReportViewModel;
        private XDocument _xDocument;

        private bool _isEnable;

        public bool IsEnable
        {
            get { return _isEnable; }
            set { _isEnable = value;
                OnPropertyChanged();
            }
        }


        public ReportViewModel(IIOManager iOManager, IMessageDialogService messageDialogService)
        {
            IsEnable = false;
            _ioManager = iOManager;
            GenerateReportCommand = new DelegateCommand(OnGenerateReport);
            _messageDialogService = messageDialogService;
        }

        private async void OnGenerateReport()
        {
            switch (ReportType)
            {
                case TypeReport.Computer:
                    ReportFormViewModel = new ComputerReportViewModel(_ioManager, _settingsReportViewModel, _xDocument, _messageDialogService);
                    break;
                case TypeReport.Laptop:
                    ReportFormViewModel = new LaptopReportViewModel(_ioManager, _settingsReportViewModel, _xDocument, _messageDialogService);
                    break;
                case TypeReport.Monitor:
                    ReportFormViewModel = new MonitorReportViewModel(_ioManager, _settingsReportViewModel, _xDocument, _messageDialogService);
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
            return false;
        }

        public async Task LoadAsync(int? Id)
        {
            _ioManager.CheckReportFolders();

            _xDocument = await _ioManager.GetXDocument();
            _settingsReportViewModel = await _ioManager.GetSettingsReport();

            if (_settingsReportViewModel != null && _xDocument != null)
                IsEnable = true;
            else
            {
                IsEnable = false;
                _messageDialogService.ShowInfoDialog("Через наявність помилок, заборонено формувати звіти!","Помилка!");
            }

        }
        public void Unsubscribe()
        {
           
        }

        public ICommand GenerateReportCommand { get; }
    }
}
