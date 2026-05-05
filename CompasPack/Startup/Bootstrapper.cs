using Autofac;
using Autofac.Core;
using CompasPack.Data.Providers;
using CompasPack.Data.Providers.API;
using CompasPack.Helper.Service;
using CompasPack.Helper.Service.Win;
using CompasPack.Model.Support;
using CompasPack.Settings;
using CompasPack.ViewModel;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Windows;


namespace CompasPack.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WinInfoProvider>().As<IWinInfoProvider>().SingleInstance();
            // Register WinInfo Instance
            builder.Register(c => c.Resolve<IWinInfoProvider>().GetWinInfo()).AsSelf().SingleInstance();

            // Register View Model -----------------------------------------------------------------------------------------------------------------------------
            builder.RegisterType<MainWindowView>().AsSelf();
            builder.RegisterType<MainWindowViewModel>().AsSelf();

            builder.RegisterType<LoadViewModel>().Keyed<IViewModel>(nameof(LoadViewModel));
            builder.RegisterType<ProgramsViewModel>().Keyed<IViewModel>(nameof(ProgramsViewModel));
            builder.RegisterType<ReportViewModel>().Keyed<IViewModel>(nameof(ReportViewModel));

            builder.RegisterType<LoadViewModel>().Keyed<IViewModelReport>(nameof(LoadViewModel));
            builder.RegisterType<LaptopReportViewModel>().Keyed<IViewModelReport>(nameof(LaptopReportViewModel));
            builder.RegisterType<MonitorReportViewModel>().Keyed<IViewModelReport>(nameof(MonitorReportViewModel));
            builder.RegisterType<ComputerReportViewModel>().Keyed<IViewModelReport>(nameof(ComputerReportViewModel));

            //Common Hardwares ViewModel
            builder.RegisterType<CPUViewModel>().AsSelf();
            builder.RegisterType<MemoryViewModel>().AsSelf();
            builder.RegisterType<MonitorDiagonalViewModel>().AsSelf();
            builder.RegisterType<PhysicalDiskViewModel>().AsSelf();
            builder.RegisterType<VideoControllerViewModel>().AsSelf();
            //Laptop Hardwares ViewModel
            builder.RegisterType<LaptopBatteryViewModel>().AsSelf();
            builder.RegisterType<LaptopMainViewModel>().AsSelf();
            builder.RegisterType<LaptopOtherViewModel>().AsSelf();
            //Monitor Hardwares ViewModel
            builder.RegisterType<MonitorAspectRatioViewModel>().AsSelf();
            builder.RegisterType<MonitorMainViewModel>().AsSelf();
            builder.RegisterType<MonitorOtherViewModel>().AsSelf();
            builder.RegisterType<MonitorResolutionViewModel>().AsSelf();
            //PC Hardwares ViewModel
            builder.RegisterType<MotherboardViewModel>().AsSelf();
            builder.RegisterType<PCCaseViewModel>().AsSelf();
            builder.RegisterType<PowerSupplyViewModel>().AsSelf();

            builder.RegisterType<MainSettingsView>().AsSelf();
            builder.RegisterType<MainSettingsViewModel>().AsSelf();

            // Register Event Aggregator -----------------------------------------------------------------------------------------------------------------------
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            // Register classes for working with data-----------------------------------------------------------------------------------------------------------
            builder.RegisterType<ProgramsSettingsProvider>().AsSelf().SingleInstance();
            builder.RegisterType<ReportSettingsProvider>().AsSelf().SingleInstance();
            builder.RegisterType<PortableProgramsSettingsProvider>().AsSelf().SingleInstance();
            // тут спосіб отримати вміст контейнера (WinInfo через c.Resolve<WinInfo>()) до виконання Build через лямбда вираз в методі Register
            builder.Register<IHardwareInfoProvider>(c =>
            {
                var type = GetHardwareInfoProviderType(c.Resolve<WinInfo>());
                return (IHardwareInfoProvider)Activator.CreateInstance(type);
            }).SingleInstance();

            builder.Register<IWinSettingsLauncher>(c =>
            {
                var type = GetWinSettingsLauncherType(c.Resolve<WinInfo>());
                return (IWinSettingsLauncher)Activator.CreateInstance(type);
            }).SingleInstance();

            // Register Helper/Service
            builder.RegisterType<ProgramsService>().As<IProgramsService>();
            builder.RegisterType<FileSystemReaderWriter>().As<IFileSystemReaderWriter>().SingleInstance();
            builder.RegisterType<FileSystemNavigator>().As<IFileSystemNavigator>().SingleInstance();
            builder.RegisterType<FileArchiver>().As<IFileArchiver>().SingleInstance();
            builder.RegisterType<ConsoleBuffer>().As<IConsoleBuffer>().SingleInstance();
            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>().SingleInstance();

            builder.RegisterType<WinDefenderWin10Plus>();
            builder.RegisterType<UnmanagedAntivirus>();
            
            builder.Register(c =>
            {
                var antiviruses = new List<IAntivirus>();

                foreach (var antivirusinfo in SoftwareInfoProvider.GetAntivirusProducts())
                {
                    var antivirusType = GetAntivirusType(antivirusinfo);

                    var avInstance = (IAntivirus)c.Resolve(antivirusType, new TypedParameter(typeof(AntivirusInfo), antivirusinfo));
                    antiviruses.Add(avInstance);
                }

                return antiviruses;
            }).As<IEnumerable<IAntivirus>>().SingleInstance();

            return builder.Build();
        }

        private static Type GetHardwareInfoProviderType(WinInfo winInfoProvider)
        {
            if (winInfoProvider.WinVer >= Model.Enum.WinVersionEnum.Win_8)
                return typeof(HardwareInfoProviderWin8Plus);
            else
                return typeof(HardwareInfoProviderBase);
        }
        private static Type GetWinSettingsLauncherType(WinInfo winInfoProvider)
        {
            if (winInfoProvider.WinVer >= Model.Enum.WinVersionEnum.Win_10)
                return typeof(WinSettingsLauncherWin10Plus);
            else
                return typeof(WinSettingsLauncherBase);
        }

        private static Type GetAntivirusType(AntivirusInfo antivirusinfo)
        {
            if (antivirusinfo.DisplayName.Contains("Windows Defender"))
                return typeof(WinDefenderWin10Plus);
            else
                return typeof(UnmanagedAntivirus);
        }
    }
}
