using Khsw.Instrument.Demo.Commons.Enums;

namespace Khsw.Instrument.Demo.Models
{
    /// <summary>
    /// 指令模型
    /// </summary>
    [Serializable]
    public partial class Command
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 指令名称
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// 指令头
        /// </summary>
        public string CommandHead { get; set; }

        /// <summary>
        /// 指令长度
        /// </summary>
        public short CommnadLength { get; set; }

        /// <summary>
        /// 指令Id
        /// </summary>
        public string CommandId { get; set; }

        /// <summary>
        /// 指令内容
        /// </summary>
        public string CommandContent { get; set; }

        /// <summary>
        /// 指令尾
        /// </summary>
        public string CommandEnd { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 输入模式
        /// </summary>
        public InputModeEnum? InputMode { get; set; }

        /// <summary>
        /// 下拉数据源类型（应该用外键关联）
        /// </summary>
        public ComboboxDataSourceTypeEnum? ComboboxDataSourceType { get; set; }
    }
}
