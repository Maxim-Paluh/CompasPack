using System;

using CompasPack.Model.Settings;
using CompasPack.Helper.Service;

namespace CompasPack.Data.Providers
{
    public class ReportSettingsProvider : SettingsFileProviderBase<ReportSettings>
    {
        public ReportSettingsProvider(IFileSystemReaderWriter fileSystemReaderWriter, IMessageDialogService messageDialogService) : base(fileSystemReaderWriter, messageDialogService, "ReportSettings")
        {
        }
    }
}
