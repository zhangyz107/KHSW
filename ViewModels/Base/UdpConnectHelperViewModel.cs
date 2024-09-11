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
    public class UdpConnectHelperViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        #region Fields
        private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _container;
        private string _ipAddress;
        private string _port;
        private string _localPort;
        #endregion

        #region Properties

        /// <summary>
        /// 设备地址
        /// </summary>
        public string IpAddress
        {
            get { return _ipAddress; }
            set { SetProperty(ref _ipAddress, value); }
        }

        /// <summary>
        /// 设备端口号
        /// </summary>
        public string Port
        {
            get { return _port; }
            set 
            { SetProperty(ref _port, value); }
        }

        /// <summary>
        /// 本地端口号
        /// </summary>
        public string LocalPort
        {
            get { return _localPort; }
            set 
            {
                SetProperty(ref _localPort, value); 
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


        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }
}
