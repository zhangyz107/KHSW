using Khsw.Instrument.Demo.Commons.Enums;
using Khsw.Instrument.Demo.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.Bussiness.Abstactions
{
    /// <summary>
    /// 仪器仪表连接服务
    /// </summary>
    public interface IInstrumentConnectService
    {
        /// <summary>
        /// 连接设备
        /// </summary>
        InstrumentBase ConnectInstrument(IOTypeEnum type, string address);

        /// <summary>
        /// 解析Ip地址（一般用于Udp、Tcp连接中）
        /// </summary>
        bool PraseIPAddress(string address, out string ipAddress, out int port, out int localPort);
    }
}
