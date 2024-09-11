using Khsw.Instrument.Demo.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.Models.Base
{
    /// <summary>
    /// Udp设备
    /// </summary>
    public partial class UdpInstrument : InstrumentBase
    {
        /// <summary>
        /// Udp连接客户端
        /// </summary>
        public UdpClient UdpClient { get; private set; }


        /// <summary>
        /// 接收分隔符
        /// </summary>
        public override byte[] Delimiter => Encoding.UTF8.GetBytes("\r\n");

        /// <summary>
        /// 连接类型
        /// </summary>
        public override IOTypeEnum ConnectType => IOTypeEnum.UDP;

        /// <summary>
        /// 编码格式
        /// </summary>
        public override Encoding StringEncoder => Encoding.UTF8;
    }
}
