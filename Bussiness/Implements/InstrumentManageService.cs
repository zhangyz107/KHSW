using Khsw.Instrument.Demo.Bussiness.Abstactions;
using Khsw.Instrument.Demo.Commons.Enums;
using Khsw.Instrument.Demo.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.Bussiness.Implements
{
    /// <summary>
    /// 设备管理服务
    /// </summary>
    public class InstrumentManageService : IInstrumentManageService
    {
        #region Fields
        private readonly IContainerExtension _container;
        private readonly IInstrumentConnectService _instrumentConnectService;

        private readonly ConcurrentBag<InstrumentBase> _instrument;
        #endregion

        public InstrumentManageService(IContainerExtension container, IInstrumentConnectService instrumentConnectService)
        {
            _container = container;
            _instrumentConnectService = instrumentConnectService;

            _instrument = new ConcurrentBag<InstrumentBase>();
        }


        public InstrumentBase ConnectInstrumentAndManage(IOTypeEnum type, string address)
        {
            var result = GetInstrumentByAddress(address);

            if (result != null)
                return result;

            result = _instrumentConnectService.ConnectInstrument(type, address);

            if (result == null)
                return result;

            _instrument.Add(result);

            return result;
        }


        public InstrumentBase GetInstrumentByAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            return _instrument.FirstOrDefault(x => x.Address.Equals(address));
        }

    }
}
