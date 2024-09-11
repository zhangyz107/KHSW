using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.Models
{
    /// <summary>
    /// 接收消息信息
    /// </summary>
    public class ReceiveMessageInfo
    {
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime ReceiveTime { get; set; }

        /// <summary>
        /// 接收内容
        /// </summary>
        public string ReceiveContent { get; set; }
    }
}
