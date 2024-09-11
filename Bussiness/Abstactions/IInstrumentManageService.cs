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
        /// 通过设备地址获取设备
        /// </summary>
        InstrumentBaseModel GetInstrumentByAddress(string address);
    }
}
