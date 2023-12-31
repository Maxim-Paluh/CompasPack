﻿using CompasPack.Settings;
using CompasPack.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompasPack.Helper
{
    public class ProgramsHelper
    {
        public static void CombinePathFolderAndImage(IList<GroupProgramViewModel> groupPrograms, UserPath userPath)
        {
            if (userPath.PathFolderPrograms != null && userPath.PathFolderImageProgram != null)
            {
                foreach (var item in groupPrograms.SelectMany(group => group.UserProgramViewModels))
                {
                    item.UserProgram.PathFolder = Path.Combine(userPath.PathFolderPrograms, item.UserProgram.PathFolder);
                    item.UserProgram.FileImage = Path.Combine(userPath.PathFolderImageProgram, item.UserProgram.FileImage);
                }
            }
        }

        public static void CheckInstallPrograms(IList<GroupProgramViewModel> groupProgramViewModels)
        {
            var tempListPrograms = WinInfoHelper.ListInstallPrograms();
            foreach (var program in groupProgramViewModels.SelectMany(group => group.UserProgramViewModels))
                program.CheckInstall(tempListPrograms);
        }

    }
}
