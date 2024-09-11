using Khsw.Instrument.Demo.Bussiness.Abstactions;
using Khsw.Instrument.Demo.Commons.Enums;
using Khsw.Instrument.Demo.Models.Base;

namespace Khsw.Instrument.Demo.Bussiness.Implements
{
    /// <summary>
    /// 设备连接服务
    /// </summary>
    public class InstrumentConnectService : IInstrumentConnectService
    {
        #region Fields
        private readonly IContainerExtension _container;
        #endregion

        public InstrumentConnectService(IContainerExtension container)
        {
            _container = container;
        }

        #region Public
        public InstrumentBase ConnectInstrument(IOTypeEnum type, string address)
        {
            InstrumentBase result = null;

            switch (type)
            {
                case IOTypeEnum.UDP:
                    result = ConnectUdpInstrument(address);
                    break;
                case IOTypeEnum.TCP:
                    result = ConnectTcpInstrument(address);
                    break;
                case IOTypeEnum.COM:
                    result = ConnectComInstrument(address);
                    break;
            }

            return result;
        }

        /// <summary>
        /// 将设备地址转换成Ip地址、端口号、本机端口号
        /// </summary>
        public bool PraseIPAddress(string address, out string ipAddress, out int port, out int localPort)
        {
            string[] addrarray = address.Split(':');
            ipAddress = "";
            port = 0;
            localPort = 0;
            if (addrarray.Length >= 3)
            {
                ipAddress = addrarray[1];
                port = Convert.ToInt32(addrarray[2]);
                localPort = Convert.ToInt32(addrarray[3]);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Private
        private InstrumentBase ConnectUdpInstrument(string address)
        {
            if (string.IsNullOrEmpty(address))
                return null;

            if (PraseIPAddress(address, out string ipAddress, out int port, out int localPort))
            {
                var instrument = new UdpInstrument();
                instrument.Address = address;
                instrument.IpAddress = ipAddress;
                instrument.Port = port;
                instrument.LocalPort = localPort;
                instrument.CreateConnect();
                return instrument;
            }

            return null;

        }

        private InstrumentBase ConnectTcpInstrument(string address)
        {
            throw new NotImplementedException();
        }

        private InstrumentBase ConnectComInstrument(string address)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
