using AutoMapper;
using Khsw.Instrument.Demo.Bussiness.Abstactions;
using Khsw.Instrument.Demo.Bussiness.Implements;
using Khsw.Instrument.Demo.Commons.Provider;
using Khsw.Instrument.Demo.ViewModels;
using Khsw.Instrument.Demo.ViewModels.Dialogs;
using Khsw.Instrument.Demo.Views;
using Khsw.Instrument.Demo.Views.Base;
using Khsw.Instrument.Demo.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Khsw.Instrument.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<UdpConnectHelperView>();
            containerRegistry.Register<CommandInformationView>();

            //注册全局命令
            containerRegistry.RegisterSingleton<IInstrumentConnectService, InstrumentConnectService>();
            containerRegistry.RegisterSingleton<IInstrumentManageService, InstrumentManageService>();

            //注册AutoMapper
            containerRegistry.RegisterSingleton<IAutoMapperProvider, AutoMapperProvider>();
            containerRegistry.Register(typeof(IMapper), GetMapper);

            //注册导航
            containerRegistry.RegisterForNavigation<InstrumentManangeView>();

            //注册对话框
            containerRegistry.RegisterDialog<AlertDialog, AlertDialogViewModel>();
            containerRegistry.RegisterDialog<SuccessDialog, SuccessDialogViewModel>();
            containerRegistry.RegisterDialog<WarningDialog, WarningDialogViewModel>();

            containerRegistry.RegisterDialog<ViewDataDetailView, ViewDataDetailViewModel>();
        }

        private IMapper GetMapper(IContainerProvider container)
        {
            var provider = container.Resolve<IAutoMapperProvider>();
            return provider.GetMapper();
        }


        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            regionAdapterMappings.RegisterMapping(typeof(StackPanel), Container.Resolve<StackPanelRegionAdapter>());
        }
    }

}
