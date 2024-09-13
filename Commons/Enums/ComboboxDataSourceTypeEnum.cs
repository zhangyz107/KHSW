using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.Commons.Enums
{
    /// <summary>
    /// 下拉数据源类型
    /// </summary>
    public enum ComboboxDataSourceTypeEnum
    {
        //None = 0,
        /// <summary>
        /// 子载波间隔
        /// </summary>
        SubcarrierSpacing = 1,

        /// <summary>
        /// Crc模式
        /// </summary>
        CrcMode = 2,

        /// <summary>
        /// LDPC RV
        /// </summary>
        LDPCRV = 3,

        /// <summary>
        /// 调制方式
        /// </summary>
        Modulation = 4,

    }
}
