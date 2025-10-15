using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using CompasPack.ViewModel;
using CompasPack.Model.Settings;
using CompasPack.Helper.Extension;
using CompasPack.Model.Enum;

namespace CompasPack.Helper.Service
{
    public class ProgramsHelper
    {
        public static void SetRootPath(string pathRoot, object obj)
        {
            foreach (var item in obj.GetType().GetProperties())
            {
                if (item.PropertyType == typeof(string))
                {
                    var tempValue = (string)item.GetValue(obj);
                    if (!string.IsNullOrEmpty(tempValue))
                        item.SetValue(obj, Path.Combine(pathRoot, tempValue));
                }
                else
                    SetRootPath(pathRoot, item.GetValue(obj));
            }
        }

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

        public static void CheckInstallPrograms(IList<GroupProgramViewModel> groupProgramViewModels, WinArchitectureEnum winArchitecture)
        {
            var tempListPrograms = WinInfoHelper.ListInstallPrograms(winArchitecture);
            foreach (var program in groupProgramViewModels.SelectMany(group => group.ProgramViewModels))
                program.CheckInstall(tempListPrograms);
        }

        public static string[] GetExeMsiFile(IFileSystemReaderWriter fileSystemReaderWriter,string fileName, string folderPath)
        {
            return fileSystemReaderWriter.GetListFile(folderPath).Where(x => x.Contains(fileName, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => x.Contains("exe", StringComparison.InvariantCultureIgnoreCase) || x.Contains("msi", StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

    }
}
