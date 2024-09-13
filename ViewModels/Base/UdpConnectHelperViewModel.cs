using Khsw.Instrument.Demo.Models.Base;
using Prism.Modularity;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Khsw.Instrument.Demo.ViewModels.Base
{
    public class UdpConnectHelperViewModel : BindableBase
    {
        #region Fields
        private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _container;
        private string _ipAddress;
        private int _port;
        private int _localPort;
        private InstrumentBase _instrument;
        #endregion

        #region Properties

        /// <summary>
        /// 设备地址
        /// </summary>
        public string IpAddress
        {
            get { return _ipAddress; }
            set
            {
                if (SetProperty(ref _ipAddress, value))
                {
                    _instrument.IpAddress = value;
                }
            }
        }

        /// <summary>
        /// 设备端口号
        /// </summary>
        public int Port
        {
            get { return _port; }
            set
            {
                if (SetProperty(ref _port, value))
                {
                    _instrument.Port = value;
                }
            }
        }

        /// <summary>
        /// 本地端口号
        /// </summary>
        public int LocalPort
        {
            get { return _localPort; }
            set
            {
                if (SetProperty(ref _localPort, value))
                {
                    _instrument.LocalPort = value;
                }
            }
        }


        public bool KeepAlive => true;

        #endregion

        public UdpConnectHelperViewModel(IContainerExtension container, IModuleManager moduleManager, IRegionManager regionManager)
        {
            _container = container;
            _moduleManager = moduleManager;
            _regionManager = regionManager;
        }

        #region Public
        /// <summary>
        /// 设置设备信息
        /// </summary>
        public void SetInstrument(InstrumentBase instrument)
        {
            if (instrument == null)
                return;

            _instrument = instrument;

            _ipAddress = instrument.IpAddress;
            _port = instrument.Port;
            _localPort = instrument.LocalPort;
            RaisePropertyChanged(nameof(IpAddress));
            RaisePropertyChanged(nameof(Port));
            RaisePropertyChanged(nameof(LocalPort));
        }
        #endregion

        #region Private

        #endregion
    }
}
