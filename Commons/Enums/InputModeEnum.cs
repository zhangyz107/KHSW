using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.Commons.Enums
{
    /// <summary>
    /// 输入模式
    /// </summary>
    public enum InputModeEnum
    {
        //None = 0,
        /// <summary>
        /// 直接输入
        /// </summary>
        Direct = 1,

        /// <summary>
        /// 下拉框
        /// </summary>
        Combobox = 2,

        /// <summary>
        /// 对话框
        /// </summary>
        Dialog= 3,
    }
}
