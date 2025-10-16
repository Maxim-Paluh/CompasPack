using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public interface IWinSettingsLauncher
    {
        void OpenDefaultProgramsSettings();
        void OpenIconSettings();
        void OpenAUCSettings();
        void OpenDesktopIconSettings();
    }
}
