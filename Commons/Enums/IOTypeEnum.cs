using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.Commons.Enums
{
    /// <summary>
    /// 设备连接的类型
    /// </summary>
    public enum IOTypeEnum
    {
        /// <summary>
        /// Udp连接方式
        /// </summary>
        UDP = 1,
        /// <summary>
        /// Tcp连接方式
        /// </summary>
        TCP = 2,
        /// <summary>
        /// 串口连接方式
        /// </summary>
        COM = 3,
        /// <summary>
        /// 未知
        /// </summary>
        UnKnow = 4,
    }
}
