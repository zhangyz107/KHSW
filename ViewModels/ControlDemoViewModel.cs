using Khsw.Instrument.Demo.Bussiness.Abstactions;
using Khsw.Instrument.Demo.Commons.Helper;
using Khsw.Instrument.Demo.DataModels;
using Khsw.Instrument.Demo.Infrastructures;
using Khsw.Instrument.Demo.Models;
using Khsw.Instrument.Demo.Models.Base;
using Khsw.Instrument.Demo.Views.Base;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text;
using System.Windows.Interop;
using static MaterialDesignThemes.Wpf.Theme;

namespace Khsw.Instrument.Demo.ViewModels
{
    public class ControlDemoViewModel : BindableBase
    {
        #region Fields
        private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _container;
        private readonly IDialogService _dialogService;
        private readonly string _prefix = "ControlDemo";
        private readonly string _commandHead = "0xEB90";
        private readonly string _commandEnd = "0xDEAD";
        private readonly int _defaultLength = 5;
        private object _commandInformationView;
        private int _boardID;
        private InstrumentBase _instrument;
        private UdpInstrument _connectedInstrument;
        #endregion

        #region Properties

        public ObservableCollection<CommandDataModel> CommandList { get; } = new ObservableCollection<CommandDataModel>();

        public string InstrumentManangeRegionName
        {
            get => _prefix + RegionNames.InstrumentManangeRegion;
        }

        public object CommandInformationView
        {
            get => _commandInformationView;
            set => SetProperty(ref _commandInformationView, value);
        }

        public int BoardID
        {
            get { return _boardID; }
            set { SetProperty(ref _boardID, value); }
        }

        public byte[] CommandHeadByte => _commandHead?.ToByteArray();
        public byte[] CommandEndByte => _commandEnd?.ToByteArray();
        #endregion

        #region Commands

        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));


        private DelegateCommand<CommandDataModel> _sendCommand;
        public DelegateCommand<CommandDataModel> SendCommand =>
            _sendCommand ?? (_sendCommand = new DelegateCommand<CommandDataModel>(ExecuteSendCommand));


        #endregion


        public ControlDemoViewModel(
            IContainerExtension container,
            IModuleManager moduleManager,
            IRegionManager regionManager,
            IDialogService dialogService)
        {
            _container = container;
            _moduleManager = moduleManager;
            _regionManager = regionManager;
            _dialogService = dialogService;
        }

        private void ExecuteLoadingCommand()
        {
            //初始化设备信息
            _instrument = InitInstrumentInfo();

            //初始化指令
            InitCommandList();

            #region 初始化界面显示
            var parameters = new NavigationParameters();
            parameters.Add("instrument", _instrument);

            _regionManager.RequestNavigate(InstrumentManangeRegionName, nameof(InstrumentManangeView), result =>
            {
                if (result.Success == false)
                {
                    // 输出导航错误的原因
                    var error = result.Exception;
                    Console.WriteLine(error?.Message);
                }
            }, parameters);

            if (_commandInformationView == null)
                CommandInformationView = _container.Resolve<CommandInformationView>();
            #endregion
        }

        private UdpInstrument InitInstrumentInfo()
        {
            var instrument = new UdpInstrument();
            instrument.IpAddress = "192.168.10.14";
            instrument.Port = 9000;
            instrument.LocalPort = 12374;
            return instrument;
        }

        private void InitCommandList()
        {
            var index = 1;

            var command1 = GetStartSignal(index++);
            CommandList.Add(command1);

            var command2 = GetLogicReset(index++);
            CommandList.Add(command2);

            var command3 = GetSubcarrierSpacing_15(index++);
            CommandList.Add(command3);

        }

        private void ExecuteSendCommand(CommandDataModel model)
        {
            var length = BitConverter.GetBytes(model.CommnadLength);
            var id = model.CommandId.ToByteArray();
            var data = string.IsNullOrEmpty(model.CommandContent) ? null : model.CommandContent.ToByteArray();
            var command = GetCommand(length, id, data);

            //((CommandInformationView)_commandInformationView)?.AppendWriteLine(new RecordMessageDataModel()
            //{
            //    RecordTime = DateTime.Now,
            //    RecordMessage = $"发送消息:{command.ToAppendString()}" 
            //});

#if DEBUG

            if (_connectedInstrument == null)
            {
                var instrumentManage = _container.Resolve<IInstrumentManageService>();
                _connectedInstrument = instrumentManage?.GetInstrumentByAddress(_instrument?.Address) as UdpInstrument;

                if (_connectedInstrument != null)
                {
                    _connectedInstrument.SendMessageEvent += (byte[] msg) =>
                    {
                        ((CommandInformationView)_commandInformationView)?.AppendWriteLine(new RecordMessageDataModel()
                        {
                            RecordTime = DateTime.Now,
                            RecordMessage = $"发送消息:{msg.ToAppendString()}"
                        });
                    };

                    _connectedInstrument.ReceiveMessageEvent += (UdpInstrument instrument) =>
                    {
                        var message = instrument.GetReceiveMessageFromQueue();
                        if (message != null)
                            ((CommandInformationView)_commandInformationView)?.AppendWriteLine(new RecordMessageDataModel()
                            {
                                RecordTime = message.RecordTime,
                                RecordMessage = $"接收消息:{message.RecordMessage}"
                            });
                    };
                }
            }

            if (_connectedInstrument == null)
            {
                //todo:记录日志 
                _dialogService.ShowDialog("AlertDialog", new DialogParameters($"message未能找到已连接的设备"));
                return;
            }

            _connectedInstrument.Send(command);

#endif
        }

        public byte[] GetCommand(byte[] length, byte[] cmdID, byte[] data)
        {
            var dataLength = data == null ? 0 : data.Length;
            int len = length.Length + cmdID.Length + dataLength + _defaultLength + 2;

            byte[] cmd = new byte[len];
            int pos = 0;
            Buffer.BlockCopy(CommandHeadByte, 0, cmd, pos, CommandHeadByte.Length);
            pos = pos + CommandHeadByte.Length;

            Buffer.BlockCopy(length, 0, cmd, pos, length.Length);
            pos = pos + length.Length;

            cmd[pos++] = (byte)BoardID;


            Buffer.BlockCopy(cmdID, 0, cmd, pos, cmdID.Length);
            pos += cmdID.Length;

            //占位符
            cmd[pos++] = (byte)0;
            cmd[pos++] = (byte)0;

            if (data != null && data.Any())
            {
                Buffer.BlockCopy(data, 0, cmd, pos, data.Length);
                pos += data.Length;
            }
            Buffer.BlockCopy(CommandEndByte, 0, cmd, pos, CommandEndByte.Length);

            return cmd;
        }

        #region 遥测指令
        /// <summary>
        /// 启动信号
        /// </summary>
        private CommandDataModel GetStartSignal(int index)
        {
            var command = new CommandDataModel()
            {
                Index = index,
                CommandName = "启动信号",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                CommnadLength = 0,
                CommandId = "0x0119"
            };
            return command;
        }

        /// <summary>
        /// 逻辑复位
        /// </summary>
        private CommandDataModel GetLogicReset(int index)
        {
            var command = new CommandDataModel()
            {
                Index = index,
                CommandName = "逻辑复位",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                CommnadLength = 0,
                CommandId = "0x0120"
            };
            return command;
        }

        /// <summary>
        /// 子载波间隔15khZ
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private CommandDataModel GetSubcarrierSpacing_15(int index)
        {
            var command = new CommandDataModel()
            {
                Index = index,
                CommandName = "子载波间隔",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                CommnadLength = 1,
                CommandId = "0x0121",
                CommandContent = "0x00",
                Remark = "15kHz"
            };
            return command;
        }
        #endregion
    }
}
