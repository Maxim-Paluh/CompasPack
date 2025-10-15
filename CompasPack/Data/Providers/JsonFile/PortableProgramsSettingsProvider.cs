using System;

using CompasPack.Model.Settings;
using CompasPack.Helper.Service;

namespace CompasPack.Data.Providers
{
    public class PortableProgramsSettingsProvider : SettingsFileProviderBase<PortableProgramsSettings>
    {
        public PortableProgramsSettingsProvider(IFileSystemReaderWriter fileSystemReaderWriter, IMessageDialogService messageDialogService) : base(fileSystemReaderWriter, messageDialogService, "PortableProgramsSettings")
        {
        }
    }
}
