using Autofac;
using CompasPack.View.Service;
using CompasPack.ViewModel;
using Prism.Events;
using CompasPack.Settings;
using CompasPack.Helper;
using CompasPack.Settings.Portable;

namespace CompasPack.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();
            /////////////////////////////////////////////////////////////////////////////////
            // Register View Model
            builder.RegisterType<MainWindowView>().AsSelf();
            builder.RegisterType<MainWindowViewModel>().AsSelf();

            builder.RegisterType<LoadViewModel>().Keyed<IDetailViewModel>(nameof(LoadViewModel));

            builder.RegisterType<ProgramsViewModel>().Keyed<IDetailViewModel>(nameof(ProgramsViewModel));
            builder.RegisterType<ReportViewModel>().Keyed<IDetailViewModel>(nameof(ReportViewModel));
            
            builder.RegisterType<MainSettingsView>().AsSelf();
            builder.RegisterType<MainSettingsViewModel>().AsSelf();
            builder.RegisterType<UserPathSettingsTabViewModel>().Keyed<ISettingsViewModel>(nameof(UserPathSettingsTabViewModel));


            /////////////////////////////////////////////////////////////////////////////////
            // Register Event Aggregator
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            /////////////////////////////////////////////////////////////////////////////////
            // Register classes for working with data

            //builder.RegisterType<Context>().AsSelf().ExternallyOwned();
            //builder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            /////////////////////////////////////////////////////////////////////////////////
            // Register Controller
            builder.RegisterType<IOHelper>().As<IIOHelper>().SingleInstance();
            /////////////////////////////////////////////////////////////////////////////////
            // Register API

            /////////////////////////////////////////////////////////////////////////////////
            // Register Service and Helper
            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();
            
            
            builder.RegisterType<UserPathSettingsHelper>().AsSelf().SingleInstance();
            builder.RegisterType<UserProgramsSettingsHelper>().AsSelf().SingleInstance();
            builder.RegisterType<UserPresetSettingsHelper>().AsSelf().SingleInstance();
            builder.RegisterType<ReportSettingsHelper>().AsSelf().SingleInstance();
            builder.RegisterType<PortableProgramsSettingsHelper>().AsSelf().SingleInstance();
            return builder.Build();
            //.ExternallyOwned()
        }
    }
}
