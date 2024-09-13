using Khsw.Instrument.Demo.Bussiness.Abstactions;
using Khsw.Instrument.Demo.Commons.Enums;
using Khsw.Instrument.Demo.Infrastructures;
using Khsw.Instrument.Demo.Models.Base;
using Khsw.Instrument.Demo.Views;
using Khsw.Instrument.Demo.Views.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Khsw.Instrument.Demo.ViewModels.Base
{
    public class InstrumentManangeViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        #region Fields
        private readonly IContainerExtension _container;
        private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        private InstrumentBase _instrument;
        private IOTypeEnum _ioType = IOTypeEnum.UDP;
        private string _address;
        private SolidColorBrush _colorState;
        private string _isConnected;
        private ConnectStateEnum _connectState = ConnectStateEnum.UnKown;
        private UdpConnectHelperView _udpConnectHelperView;
        private object _connectHelperView;
        #endregion

        #region Properties

        public IOTypeEnum IOType
        {
            get
            {
                return _ioType;
            }

            set
            {
                this.SetProperty(ref _ioType, value);
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        /// <summary>
        /// 颜色状态
        /// </summary>
        public SolidColorBrush ColorState
        {
            get
            {
                return _colorState;
            }

            set
            {
                this.SetProperty(ref _colorState, value);
            }
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        public string IsConnected
        {
            get
            {
                return _isConnected;
            }

            set
            {
                SetProperty(ref _isConnected, value);
            }
        }

        public ConnectStateEnum ConnectState
        {
            get
            {
                return _connectState;
            }

            set
            {
                this.SetProperty(ref _connectState, value);
            }
        }

        /// <summary>
        /// 连接帮助视图
        /// </summary>
        public object ConnectHelperView
        {
            get { return _connectHelperView; }
            set { SetProperty(ref _connectHelperView, value); }
        }


        /// <summary>
        /// 设备连接的类型显示字典
        /// </summary>
        public Dictionary<string, IOTypeEnum> IOTypeDisplayDic
        {
            get
            {
                return new Dictionary<string, IOTypeEnum>
                {
                        { "UDP协议",IOTypeEnum.UDP},
                        //{ "TCP协议",IOTypeEnum.TCP},
                        //{ "串口",IOTypeEnum.COM },
                };
            }
        }

        public bool KeepAlive => true;


        #endregion

        #region Commands

        private DelegateCommand _ioTypeChangedCommand;
        public DelegateCommand IOTypeChangedCommand =>
            _ioTypeChangedCommand ?? (_ioTypeChangedCommand = new DelegateCommand(ExecuteIOTypeChangedCommand));


        private DelegateCommand _connectCommand;

        public DelegateCommand ConnectCommand =>
            _connectCommand ?? (_connectCommand = new DelegateCommand(ExecuteConnectCommand));

        #endregion

        public InstrumentManangeViewModel(IContainerExtension container, IModuleManager moduleManager, IRegionManager regionManager)
        {
            _container = container;
            _moduleManager = moduleManager;
            _regionManager = regionManager;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var instrument = navigationContext.Parameters["instrument"] as InstrumentBase;
            if (instrument != null)
            {
                _instrument = instrument;
                _ioType = _instrument.ConnectType;
                ExecuteIOTypeChangedCommand();
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        #region Private
        private void ExecuteIOTypeChangedCommand()
        {

            switch (_ioType)
            {
                case IOTypeEnum.UDP:
                    if (_udpConnectHelperView == null)
                        _udpConnectHelperView = _container.Resolve<UdpConnectHelperView>();

                    if (_udpConnectHelperView != null)
                    {
                        ConnectHelperView = _udpConnectHelperView;
                        _udpConnectHelperView.SetInstrument(_instrument);
                    }
                    break;
                case IOTypeEnum.TCP:
                    ConnectHelperView = null;
                    break;
                case IOTypeEnum.COM:
                    break;
            }
        }

        private void ExecuteConnectCommand()
        {
            var instrumentManage = _container.Resolve<IInstrumentManageService>();
            if (instrumentManage != null)
            {
                var instrument = instrumentManage.ConnectInstrumentAndManage(_ioType, _instrument.Address);
                IsConnected = instrument?.IsConnected == true ? "已连接" : "未连接";
                ColorState = instrument?.IsConnected == true ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                ConnectState = instrument?.IsConnected == true ? ConnectStateEnum.Connect : ConnectStateEnum.Disconnect;
            }
            Address = _instrument.Address;
        }
        #endregion
    }
}
