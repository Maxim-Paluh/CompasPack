using System;

using CompasPack.Model.Settings;
using CompasPack.Helper.Service;

namespace CompasPack.Data.Providers
{
    public class ProgramsSettingsProvider : SettingsFileProviderBase<ProgramsSettings>
    {
        public ProgramsSettingsProvider(IFileSystemReaderWriter fileSystemReaderWriter, IMessageDialogService messageDialogService) : base(fileSystemReaderWriter, messageDialogService, "ProgramsSettings")
        {
        }
    }
}
