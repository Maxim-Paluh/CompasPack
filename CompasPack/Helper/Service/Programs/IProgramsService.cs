using CompasPack.Data.Providers.API;
using CompasPack.Helper.Extension;
using CompasPack.Model.Entities.Programs;
using CompasPack.Model.Enum;
using CompasPack.Model.Wrapper;
using CompasPack.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public interface IProgramsService
    {
        void CombinePath(IList<GroupProgramsWrapper> groupPrograms, ProgramsPaths programsPaths);
        void CheckInstallPrograms(IList<GroupProgramsWrapper> groupProgramViewModels, WinArchitectureEnum winArchitecture);

        Task<double> SpeedTest();
        Task OnAntiviruses(List<IAntivirus> antiviruses);
        Task OffAntiviruses(List<IAntivirus> antiviruses);

        Task InstallPrograms(IList<ProgramWrapper> selectedPrograms);
        Task OpenProtectedProgram(ProtectedProgram protectedProgram);
    }
}
