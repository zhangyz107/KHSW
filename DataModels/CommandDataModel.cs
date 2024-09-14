using Khsw.Instrument.Demo.Commons.Enums;
using Khsw.Instrument.Demo.Commons.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.DataModels
{
    /// <summary>
    /// 指令数据模型
    /// </summary>
    public class CommandDataModel : BindableBase
    {
        #region Fields
        private int _index;
        private string _commandName;
        private string _commandHead;
        private short _commnadLength;
        private string _commandId;
        private string _commnadContent = string.Empty;
        private bool _contentEnable = false;
        private string _commandEnd;
        private string _remark;
        private InputModeEnum? _inputMode = InputModeEnum.Direct;
        private ComboboxDataSourceTypeEnum? _comboboxDataSourceType;
        private object _comboboxDataSource;
        #endregion

        #region Properties
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// 序号
        /// </summary>
        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        /// <summary>
        /// 指令名称
        /// </summary>
        public string CommandName
        {
            get => _commandName;
            set => SetProperty(ref _commandName, value);
        }

        /// <summary>
        /// 指令头
        /// </summary>
        public string CommandHead
        {
            get => _commandHead;
            set => SetProperty(ref _commandHead, value);
        }

        /// <summary>
        /// 指令长度
        /// </summary>
        public short CommnadLength
        {
            get => _commnadLength;
            set => SetProperty(ref _commnadLength, value);
        }

        /// <summary>
        /// 指令Id
        /// </summary>
        public string CommandId
        {
            get => _commandId;
            set => SetProperty(ref _commandId, value);
        }

        /// <summary>
        /// 指令内容
        /// </summary>
        public string CommandContent
        {
            get => _commnadContent;
            set => SetProperty(ref _commnadContent, value);
        }

        /// <summary>
        /// 指令尾
        /// </summary>
        public string CommandEnd
        {
            get => _commandEnd;
            set => SetProperty(ref _commandEnd, value);
        }

        /// <summary>
        /// 内容设置使能
        /// </summary>
        public bool ContentEnable
        {
            get => _contentEnable;
            set => SetProperty(ref _contentEnable, value);
        }

        public bool IsReadOnly { get => !ContentEnable; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }

        /// <summary>
        /// 输入模式
        /// </summary>
        public InputModeEnum? InputMode
        {
            get => _inputMode;
            set => SetProperty(ref _inputMode, value);
        }

        /// <summary>
        /// 下拉数据源类型(仅对下拉型输入模式有用)
        /// </summary>
        public ComboboxDataSourceTypeEnum? ComboboxDataSourceType
        {
            get => _comboboxDataSourceType;
            set => SetProperty(ref _comboboxDataSourceType, value);
        }

        /// <summary>
        /// 下拉数据源
        /// </summary>
        public object ComboboxDataSource
        {
            get => _comboboxDataSource;
            set => SetProperty(ref _comboboxDataSource, value);
        }

        #endregion
    }
}
