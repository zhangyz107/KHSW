using Khsw.Instrument.Demo.Commons.Enums;
using Khsw.Instrument.Demo.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.ViewModels.Base
{
    public class InstrumentManangeViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        #region Fields
        private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        private string _ipAddress;
        private IOTypeEnum iOType = IOTypeEnum.UDP;
        #endregion

        #region Properties

        public string IpAddress
        {
            get { return _ipAddress; }
            set { SetProperty(ref _ipAddress, value); }
        }

        public IOTypeEnum IOType
        {
            get
            {
                return iOType;
            }

            set
            {
                this.SetProperty(ref iOType, value);
            }
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
                        { "TCP协议",IOTypeEnum.TCP},
                        { "串口",IOTypeEnum.COM },
                };
            }
        }

        public bool KeepAlive => true;


        #endregion

        public InstrumentManangeViewModel(IModuleManager moduleManager, IRegionManager regionManager)
        {
            _moduleManager = moduleManager;
            _regionManager = regionManager;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var ipAddress = navigationContext.Parameters["ipAddress"] as string;
            if (ipAddress != null)
                IpAddress = ipAddress;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
