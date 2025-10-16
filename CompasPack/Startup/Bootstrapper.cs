using Autofac;
using CompasPack.Data.Providers;
using CompasPack.Data.Providers.API;
using CompasPack.Helper.Service;
using CompasPack.Helper.Service.Win;
using CompasPack.Settings;
using CompasPack.ViewModel;
using Prism.Events;
using System;


namespace CompasPack.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            var winInfoProvider = new WinInfoProvider();    
            builder.RegisterInstance(winInfoProvider).As<IWinInfoProvider>().SingleInstance();

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

            builder.RegisterType(GetHardwareInfoProviderType(winInfoProvider)).As<IHardwareInfoProvider>().SingleInstance();
            builder.RegisterType(GetWinSettingsLauncherType(winInfoProvider)).As<IWinSettingsLauncher>().SingleInstance();

            // Register Helper/Service
            builder.RegisterType<FileSystemReaderWriter>().As<IFileSystemReaderWriter>().SingleInstance();
            builder.RegisterType<FileSystemNavigator>().As<IFileSystemNavigator>().SingleInstance();
            builder.RegisterType<FileArchiver>().As<IFileArchiver>().SingleInstance();
            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();
            
            builder.RegisterType<WinDefenderWin10Plus>().Keyed<IWinAntivirus>(nameof(WinDefenderWin10Plus));


            return builder.Build();
        }

        private Type GetHardwareInfoProviderType(WinInfoProvider winInfoProvider)
        {
            if (winInfoProvider.WinVer >= Model.Enum.WinVersionEnum.Win_8)
                return typeof(HardwareInfoProviderWin8Plus);
            else
                return typeof(HardwareInfoProviderBase);
        }
        private Type GetWinSettingsLauncherType(WinInfoProvider winInfoProvider)
        {
            if (winInfoProvider.WinVer >= Model.Enum.WinVersionEnum.Win_10)
                return typeof(WinSettingsLauncherWin10Plus);
            else
                return typeof(WinSettingsLauncherBase);
        }
    }
}
