using Khsw.Instrument.Demo.Views;
using Khsw.Instrument.Demo.Views.Base;
using System.Windows;
using System.Windows.Controls.Primitives;

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
            //注册导航
            containerRegistry.RegisterForNavigation<InstrumentManangeView>();
            containerRegistry.RegisterForNavigation<UdpConnectHelperView>();
        }
    }

}
