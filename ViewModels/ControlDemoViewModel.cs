using Khsw.Instrument.Demo.Bussiness.Abstactions;
using Khsw.Instrument.Demo.Infrastructures;
using Khsw.Instrument.Demo.Models.Base;
using Khsw.Instrument.Demo.Views.Base;

namespace Khsw.Instrument.Demo.ViewModels
{
    public class ControlDemoViewModel : BindableBase
    {
        #region Fields
        private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _container;
        private readonly string _prefix = "ControlDemo";
        #endregion

        #region Properties

        public string InstrumentManangeRegionName
        {
            get => _prefix + RegionNames.InstrumentManangeRegion;
        }

        #endregion

        #region Commands

        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));


        #endregion


        public ControlDemoViewModel(IContainerExtension container, IModuleManager moduleManager, IRegionManager regionManager)
        {
            _moduleManager = moduleManager;
            _regionManager = regionManager;
            _container = container;
        }

        private void ExecuteLoadingCommand()
        {
            var instrument = new UdpInstrument();
            instrument.IpAddress = "192.168.1.199";
            instrument.Port = 5025;
            instrument.LocalPort = 12374;
            var parameters = new NavigationParameters();
            parameters.Add("instrument", instrument);

            _regionManager.RequestNavigate(InstrumentManangeRegionName, nameof(InstrumentManangeView), result =>
            {
                if (result.Success == false)
                {
                    // 输出导航错误的原因
                    var error = result.Exception;
                    Console.WriteLine(error?.Message);
                }
            }, parameters);
        }
    }
}
