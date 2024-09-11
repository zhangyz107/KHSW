﻿using Khsw.Instrument.Demo.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.Models.Base
{
    public abstract class InstrumentBase
    {
        public InstrumentBase()
        {
        }

        /// <summary>
        /// 设备地址
        /// </summary>
        public string Address => $"{ConnectType.ToString()}:{IpAddress}:{Port}:{LocalPort}";

        /// <summary>
        /// Ip地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 本机端口号
        /// </summary>
        public int LocalPort { get; set; }

        /// <summary>
        /// 发送超时,多少ms
        /// </summary>
        public int SendTimeOut { get; set; } = 5000;

        /// <summary>
        /// 发送队列最多支持命令数量
        /// </summary>
        public int MaxSendCount { get; set; } = 100;

        /// <summary>
        /// 接收超时,多少ms
        /// </summary>
        public int ReceiveTimeOut { get; set; } = 10000;

        /// <summary>
        /// 接收队列最多可以接收数量
        /// </summary>
        public int MaxReceiveCount { get; set; } = 100;

        /// <summary>
        /// 
        /// </summary>
        public bool IsMulticasst { get; set; } = false;

        /// <summary>
        /// 是否链接
        /// </summary>
        public bool IsConnected { get; internal set; }

        /// <summary>
        /// 接收时的分隔符
        /// </summary>
        public abstract byte[] Delimiter { get; }

        /// <summary>
        /// 连接类型
        /// </summary>
        public abstract IOTypeEnum ConnectType { get; }

        /// <summary>
        /// 编码格式
        /// </summary>
        public abstract Encoding StringEncoder { get; }

    }
}
