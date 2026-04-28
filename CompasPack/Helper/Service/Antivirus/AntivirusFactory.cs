using Autofac;
using CompasPack.Data.Providers.API;
using CompasPack.Helper.Service.Win;
using CompasPack.Model.ViewAndViewModel;
using CompasPack.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service.Antivirus
{
    public class AntivirusFactory : IAntivirusFactory
    {
        private readonly IComponentContext _context;
        public AntivirusFactory(IComponentContext context)
        {
            _context = context;
        }
        public IAntivirus Create(AntivirusInfo info)
        {
            var typeName = GetAntivirusType(info);
            return _context.ResolveKeyed<IAntivirus>(typeName.Name, new TypedParameter(typeof(AntivirusInfo), info));
        }

        private static Type GetAntivirusType(AntivirusInfo info)
        {
            if (info.DisplayName.Contains("Windows Defender"))
                return typeof(WinDefenderWin10Plus);
            else
                return typeof(AntivirusBase);
        }
    }
}
