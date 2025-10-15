using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using System.Collections.Generic;

using Prism.Commands;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;

using CompasPack.View;
using CompasPack.Model.Enum;
using CompasPack.Helper.Service;
using CompasPack.Model.Settings;
using CompasPack.Data.Providers;

namespace CompasPack.ViewModel
{
    public abstract class ReportViewModelBase : ViewModelBase
    {
        protected readonly ReportSettings _reportSettings;
        protected IMessageDialogService _messageDialogService;
        protected IFileSystemReaderWriter _fileSystemReaderWriter;
        protected IFileSystemNavigator _fileSystemNavigator;

        private bool _isEnable;
        private int _indexReport;

        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged();
            }
        }
        public string ReportPath {get;}
        public string ReportPricePath { get; }
        public string RPFFilePath { get; } 
        public int IndexReport
        {
            get { return _indexReport; }
            set
            {
                _indexReport = value;
                OnPropertyChanged();
            }
        }
        public ReportViewModelBase(IFileSystemReaderWriter fileSystemReaderWriter, IFileSystemNavigator fileSystemNavigator, ReportSettingsProvider reportSettingsProvider, IMessageDialogService messageDialogService,
            string reportPath, string reportPricePath, string rPFFilePath)
        {
            IsEnable = false;
            _fileSystemReaderWriter = fileSystemReaderWriter;
            _fileSystemNavigator = fileSystemNavigator;
            _reportSettings = reportSettingsProvider.Settings;
            _messageDialogService = messageDialogService;

            ReportPath = reportPath;
            ReportPricePath = reportPricePath;
            RPFFilePath = rPFFilePath;

            IndexReport = GetLastReport(ReportPath)+1;

            SaveReportCommand = new DelegateCommand(OnSaveReport);

            OpenReportCommand = new DelegateCommand(OnOpenReport);
            OpenPriceCommand = new DelegateCommand(OnOpenPrice);
            OpenFolderCommand = new DelegateCommand(OnOpenFolder);
        }
        protected virtual async void OnSaveReport()
        {  
            if (IsError())
                return;
            
            string reportHmlPath = System.IO.Path.Combine(ReportPath, $"Report_{IndexReport:000}.htm");
            string reportHtmlPath = System.IO.Path.Combine(ReportPath, $"Report_{IndexReport:000}.html");
            string reportPricePath = System.IO.Path.Combine(ReportPath, $"Report_{IndexReport:000}.docx");

            bool checkHml = File.Exists(reportHmlPath);
            bool checkHtml = File.Exists(reportHtmlPath);
            bool checkDocx = File.Exists(reportPricePath);

            if (checkHml || checkHtml || checkDocx)
            {
                string listFile = string.Empty;
                if (checkHml)
                    listFile += $"{reportHmlPath}\n";
                if (checkHtml)
                    listFile += $"{reportHtmlPath}\n";
                if (checkDocx)
                    listFile += $"{reportPricePath}\n";

                var res = _messageDialogService.ShowYesNoDialog($"В папці призначення вже є файл(и):\n\n{listFile}\nВи хочете замінити його(їх)\n\n(Це невідворотня дія, зробіть їх копію!)", "Попередження!");
                if (res == MessageDialogResultEnum.No || res == MessageDialogResultEnum.Cancel)
                { return; }
            }
            IsEnable = false;
            try
            {
                await WriteHTML(reportHtmlPath);
            }
            catch (Exception exception)
            {
                ShowErrors(new Exception($"В процесі формування звіту HTML відбулася помилка.\nФайл: {reportHtmlPath} не створено!", exception));
            }
            try
            {
                await AidaReportHelper.GetAidaReport(_reportSettings.ReportPaths.AidaExeFilePath, System.IO.Path.Combine(ReportPath, $"Report_{IndexReport:000}."), "/HML", RPFFilePath, 240);
            }
            catch (Exception exception)
            {
                ShowErrors(new Exception($"В процесі формування звіту AIDA64 відбулася помилка.\nФайл: {reportHmlPath} не створено!", exception));
            }
            try
            {
                await WriteDOCX(reportPricePath);
            }
            catch (Exception exception)
            {
                ShowErrors(new Exception($"В процесі формування звіту docx відбулася помилка.\nФайл: { reportPricePath } може бути відсутнім або бути з помилками!", exception));
            }
            IsEnable = true;
        }

        private void ShowErrors(Exception exception)
        {
            List<Exception> exceptions = new List<Exception>();
            do
            {
                exceptions.Add(exception);
                exception = exception.InnerException;
            } while (exception != null);

            var Error = new ErrorsView(exceptions);
            Error.ShowDialog();
        }

        protected abstract bool IsError();
        protected abstract string GetHTML();
        protected async Task WriteHTML(string path)
        {
            var html = GetHTML();
            ClipboardHelper.CopyToClipboard(html, "");
            await _fileSystemReaderWriter.WriteAllTextAsync(path, html);
        }
        protected async Task WriteDOCX(string reportPricePath)
        {
            await Task.Factory.StartNew(() =>
            {
                if (!File.Exists(ReportPricePath))
                    throw new ArgumentException(($"{nameof(ReportPricePath)} Is Null Or White Space"));

                var pathDirectory = System.IO.Path.GetDirectoryName(reportPricePath);
                if (!Directory.Exists(pathDirectory))
                    Directory.CreateDirectory(pathDirectory);

                if(File.Exists(reportPricePath))
                    File.Delete(reportPricePath);
                File.Copy(ReportPricePath, reportPricePath);

                using (WordprocessingDocument doc = WordprocessingDocument.Open(reportPricePath, true))
                {
                    var document = doc.MainDocumentPart.Document;

                    foreach (var item in _reportSettings.DocxReplaceTextDictionary)
                    {
                        var replaceText = GetReplaceText(item.Key);
                        if (replaceText != null)
                        {
                            foreach (var graphic in document.Descendants<Graphic>()) // <<< Here
                            {
                                if (graphic.InnerText == item.Value)
                                    foreach (var text in graphic.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>()) // <<< Here
                                    {
                                        if (text.Text.Contains(item.Value))
                                        {
                                            text.Text = text.Text.Replace(item.Value, replaceText);
                                        }
                                    }
                            }
                        }
                    }

                }
            });

        }
        protected abstract string GetReplaceText(DocxReplaceTextEnum reportViewModelEnum);
        private int GetLastReport(string paht)
        {
            var lastString = _fileSystemReaderWriter.GetListFile(paht).Select(x => x = Regex.Match(x, "\\d+").Value).OrderBy(x => x).LastOrDefault();
            if (lastString != null)
            {
                if (int.TryParse(lastString, out int last))
                    return last;
                else
                    return -1;
            }
            else
                return -1;
        }
        protected void OnOpenReport()
        {
            if (!File.Exists($"{ReportPath}\\Report_{IndexReport:000}.html"))
                _messageDialogService.ShowInfoDialog("Такого файлу нема!", "Помилка!");
            _fileSystemNavigator.OpenFolderAndSelectFile($"{ReportPath}\\Report_{IndexReport:000}.html");
        }
        protected void OnOpenPrice()
        {
            if (!File.Exists($"{ReportPath}\\Report_{IndexReport:000}.docx"))
                _messageDialogService.ShowInfoDialog("Такого файлу нема!", "Помилка!");
            _fileSystemNavigator.OpenFolderAndSelectFile($"{ReportPath}\\Report_{IndexReport:000}.docx");
        }
        protected void OnOpenFolder()
        {
            _fileSystemNavigator.OpenCreateFolder(ReportPath);
        }
        public ICommand SaveReportCommand { get; set; }
        public ICommand OpenReportCommand { get; set; }
        public ICommand OpenPriceCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
    }
}
