using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.DataModels
{
    /// <summary>
    /// 报文数据模型
    /// </summary>
    public class MessageDataModel : BindableBase
    {
        #region Fields
        private string _title;
        private string _content;
        #endregion

        #region Properties
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// 报文内容
        /// </summary>
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }
        #endregion
    }
}
