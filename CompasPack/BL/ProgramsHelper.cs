using CompasPack.Settings;
using CompasPakc.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.BL
{
    public class ProgramsHelper
    {
        public static void CombinePathFolderAndImage(List<GroupPrograms> groupPrograms, UserPath userPath, IIOManager iOManager)
        {
            foreach (var program in groupPrograms.SelectMany(group => group.UserPrograms))
            {
                program.PathFolder = Path.Combine(userPath.PathFolder, program.PathFolder);
                program.FileImage = Path.Combine(userPath.FileImage, program.FileImage);
            }
        }
             
}
}
