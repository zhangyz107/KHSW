using Khsw.Instrument.Demo.Commons.Enums;
using Khsw.Instrument.Demo.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.Bussiness.Abstactions
{
    /// <summary>
    /// 设备管理服务
    /// </summary>
    public interface IInstrumentManageService
    {
        /// <summary>
        /// 连接设备并管理
        /// </summary>
        InstrumentBase ConnectInstrumentAndManage(IOTypeEnum type, string address);

        /// <summary>
        /// 通过设备地址获取设备
        /// </summary>
        InstrumentBase GetInstrumentByAddress(string address);
    }
}
