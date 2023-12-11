﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;

namespace CompasPack.ViewModel
{
    public class MonitorMainViewModel : ReportHardWareViewModelBase, IReportViewModel, IDataErrorInfo
    {
        private string _brand;
        private string _model;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                OnPropertyChanged();
                Result = $"{Brand} {Model}".Trim();
            }
        }
        public string Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "Brand":
                        if (string.IsNullOrWhiteSpace(Brand))
                            error = "Введи значення";
                        break;
                }
                return error;
            }
        }

        public MonitorMainViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
        }
        public void Load()
        {
            var tempName = Document.XPathSelectElement(SettingsReport.Monitor.MonitorName.XPath);
            if (tempName != null)
                Name = tempName.Value;
            else
                Name = string.Empty;
            
            var tempModel = Document.XPathSelectElement(SettingsReport.Monitor.MonitorModel.XPath);
            if (tempModel != null)
                Model = tempModel.Value;
            else
                Model = string.Empty;


            if (!string.IsNullOrWhiteSpace(Name))
                Brand = SettingsReport.Monitors.FirstOrDefault(x => Name.Contains(x, StringComparison.InvariantCultureIgnoreCase));

            if(string.IsNullOrWhiteSpace(Brand))
                if(!string.IsNullOrWhiteSpace(Model))
                    Brand = SettingsReport.Monitors.FirstOrDefault(x => Model.Contains(x, StringComparison.InvariantCultureIgnoreCase));

            if(!string.IsNullOrWhiteSpace(Brand))
            if (Model.Contains(Brand))
                Model = Model.Replace(Brand, "", StringComparison.InvariantCultureIgnoreCase).Trim();

            Result = $"{Brand} {Model}".Trim();
        }
    }
}