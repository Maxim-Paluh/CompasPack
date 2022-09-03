using Autofac;
using CompasPac.BL;
using CompasPac.View.Service;
using CompasPac.ViewModel;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPac.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();
            /////////////////////////////////////////////////////////////////////////////////
            // Register View Model
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainWindowViewModel>().AsSelf();


            /////////////////////////////////////////////////////////////////////////////////
            // Register Event Aggregator
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            /////////////////////////////////////////////////////////////////////////////////
            // Register classes for working with data

            //builder.RegisterType<Context>().AsSelf().ExternallyOwned();
            //builder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            /////////////////////////////////////////////////////////////////////////////////
            // Register Controller
            builder.RegisterType<IOManager>().As<IIOManager>();
            /////////////////////////////////////////////////////////////////////////////////
            // Register API

            /////////////////////////////////////////////////////////////////////////////////
            // Register Service
            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();

            return builder.Build();
            //.ExternallyOwned()
        }
    }
}
