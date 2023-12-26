using Autofac;
using CompasPack.BL;
using CompasPack.View.Service;
using CompasPack.ViewModel;
using CompasPack;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompasPakc.BL;
using CompasPack.Main;
using CompasPack.Settings;
using CompasPack.Settings.Programs;

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


            /////////////////////////////////////////////////////////////////////////////////
            // Register Event Aggregator
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            /////////////////////////////////////////////////////////////////////////////////
            // Register classes for working with data

            //builder.RegisterType<Context>().AsSelf().ExternallyOwned();
            //builder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            /////////////////////////////////////////////////////////////////////////////////
            // Register Controller
            builder.RegisterType<IOManager>().As<IIOManager>().SingleInstance();
            /////////////////////////////////////////////////////////////////////////////////
            // Register API

            /////////////////////////////////////////////////////////////////////////////////
            // Register Service and Helper
            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();
            
            
            builder.RegisterType<UserPathSettingsHelper>().AsSelf().SingleInstance();
            builder.RegisterType<UserProgramsSettingsHelper>().AsSelf().SingleInstance();
            builder.RegisterType<UserPresetSettingsHelper>().AsSelf().SingleInstance();
            builder.RegisterType<ReportSettingsSettingsHelper>().AsSelf().SingleInstance();

            return builder.Build();
            //.ExternallyOwned()
        }
    }
}
