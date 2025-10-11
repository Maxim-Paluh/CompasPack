using CompasPack.Settings;
using CompasPack.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CompasPack.Service;
using CompasPack.Settings.Programs;

namespace CompasPack.Helper
{
    public class ProgramsHelper
    {
        public static void CombinePathFolderAndImage(IList<GroupProgramViewModel> groupPrograms, ProgramsPaths programsPaths)
        {
            if (programsPaths.PathFolderPrograms != null && programsPaths.PathFolderImageProgram != null)
            {
                foreach (var item in groupPrograms.SelectMany(group => group.ProgramViewModels))
                {
                    item.Program.PathFolder = Path.Combine(programsPaths.PathFolderPrograms, item.Program.PathFolder);
                    item.Program.FileImage = Path.Combine(programsPaths.PathFolderImageProgram, item.Program.FileImage);
                }
            }
        }

        public static void CheckInstallPrograms(IList<GroupProgramViewModel> groupProgramViewModels)
        {
            var tempListPrograms = WinInfoHelper.ListInstallPrograms();
            foreach (var program in groupProgramViewModels.SelectMany(group => group.ProgramViewModels))
                program.CheckInstall(tempListPrograms);
        }

        public static string[] GetExeMsiFile(IIOHelper iOHelper,string fileName, string folderPath)
        {
            return iOHelper.GetListFile(folderPath).Where(x => x.Contains(fileName, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => x.Contains("exe", StringComparison.InvariantCultureIgnoreCase) || x.Contains("msi", StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

    }
}
