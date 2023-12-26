using CompasPack.Settings;
using CompasPack.ViewModel;
using CompasPakc.BL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.BL
{
    public class ProgramsHelper
    {
        public static void CombinePathFolderAndImage(IList<GroupProgramViewModel> groupPrograms, UserPath userPath, IIOManager iOManager)
        {
            foreach (var item in groupPrograms.SelectMany(group => group.UserProgramViewModels))
            {
                item.UserProgram.PathFolder = Path.Combine(iOManager.PathRoot, userPath.PathFolderPrograms, item.UserProgram.PathFolder);
                item.UserProgram.FileImage = Path.Combine(iOManager.PathRoot, userPath.PathFolderImageProgram, item.UserProgram.FileImage);
            }
        }

        public static void CheckInstallPrograms(IList<GroupProgramViewModel> groupProgramViewModels)
        {
            var tempListPrograms = WinInfo.ListInstallPrograms();
            foreach (var program in groupProgramViewModels.SelectMany(group => group.UserProgramViewModels))
                program.CheckInstall(tempListPrograms);
        }

    }
}
