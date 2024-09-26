using AutoMapper;
using Khsw.Instrument.Demo.Bussiness.Abstactions;
using Khsw.Instrument.Demo.Commons.Helper;
using Khsw.Instrument.Demo.DataModels;
using Khsw.Instrument.Demo.Infrastructures;
using Khsw.Instrument.Demo.Models;
using Khsw.Instrument.Demo.Models.Base;
using Khsw.Instrument.Demo.Views.Base;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Khsw.Instrument.Demo.ViewModels
{
    public class ControlDemoViewModel : BindableBase
    {
        #region Fields
        private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _container;
        private readonly IDialogService _dialogService;
        private readonly IMapper _mapper;
        private readonly string _prefix = "ControlDemo";
        private readonly string _commandHead = "0xEB90";
        private readonly string _commandEnd = "0xDEAD";
        private readonly string _commandListFileName = $"{nameof(ControlDemoViewModel)}CommandList.xml";
        //private readonly string _commandListFileName = $"{nameof(ControlDemoViewModel)}CommandList.xml";
        private readonly int _defaultLength = 5;
        private readonly int _sendByteMax = 1024;
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

        private DelegateCommand<CommandDataModel> _importDataCommand;
        public DelegateCommand<CommandDataModel> ImportDataCommand =>
            _importDataCommand ?? (_importDataCommand = new DelegateCommand<CommandDataModel>(ExecuteImportDataCommand));

        private DelegateCommand<CommandDataModel> _viewDetailCommand;
        public DelegateCommand<CommandDataModel> ViewDetailCommand =>
            _viewDetailCommand ?? (_viewDetailCommand = new DelegateCommand<CommandDataModel>(ExecuteViewDetailCommand));
        #endregion


        public ControlDemoViewModel(
            IContainerExtension container,
            IModuleManager moduleManager,
            IRegionManager regionManager,
            IDialogService dialogService,
            IMapper mapper)
        {
            _container = container;
            _moduleManager = moduleManager;
            _regionManager = regionManager;
            _dialogService = dialogService;
            _mapper = mapper;
        }

        private void ExecuteLoadingCommand()
        {
            //初始化设备信息
            _instrument = InitInstrumentInfo();

            //初始化指令
            LoadCommandList();

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

        internal void SaveData()
        {
            var commandList = _mapper.Map<IList<CommandDataModel>, IList<Command>>(CommandList);

            try
            {
                XmlHelper.SerializeToXml(commandList, _commandListFileName);
            }
            catch (Exception)
            {
                //todo:记录日志保存数据失败
            }
        }

        private UdpInstrument InitInstrumentInfo()
        {
            var instrument = new UdpInstrument();
            instrument.IpAddress = "192.168.10.14";
            instrument.Port = 9000;
            instrument.LocalPort = 12374;
            return instrument;
        }

        private void LoadCommandList()
        {
            var initialCommandList = InitCommandList();
            var loadCommandList = new List<CommandDataModel>();
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(dir, _commandListFileName);
            if (File.Exists(filePath))
            {
                var commandList = XmlHelper.DeserializeListFromXml<Command>(filePath);

                if (commandList != null && commandList.Any())
                {
                    foreach (var command in commandList)
                    {
                        var dataModel = _mapper.Map<CommandDataModel>(command);
                        if (dataModel != null)
                        {
                            if (dataModel.InputMode == Commons.Enums.InputModeEnum.Combobox)
                            {
                                switch (dataModel.ComboboxDataSourceType)
                                {
                                    case Commons.Enums.ComboboxDataSourceTypeEnum.SubcarrierSpacing:
                                        dataModel.ComboboxDataSource = SubcarrierSpacingDic;
                                        break;
                                    case Commons.Enums.ComboboxDataSourceTypeEnum.CrcMode:
                                        dataModel.ComboboxDataSource = CrcModeDic;
                                        break;
                                    case Commons.Enums.ComboboxDataSourceTypeEnum.LDPCRV:
                                        dataModel.ComboboxDataSource = LDPCRVCDic;
                                        break;
                                    case Commons.Enums.ComboboxDataSourceTypeEnum.Modulation:
                                        dataModel.ComboboxDataSource = ModulationModeDic;
                                        break;
                                }
                            }
                            loadCommandList.Add(dataModel);
                        }
                    }
                }
            }

            var joinedCommandIndex = loadCommandList.Select(x => x.Index);
            var toAddCommand = initialCommandList.Where(x => !joinedCommandIndex.Contains(x.Index));
            foreach (var command in toAddCommand)
                loadCommandList.Add(command);

            var orderCommandList = loadCommandList.OrderBy(x => x.Index);

            foreach (var command in orderCommandList)
                CommandList.Add(command);
        }

        private List<CommandDataModel> InitCommandList()
        {
            var index = 1;
            var commandList = new List<CommandDataModel>();
            var command1 = GetStartSignalCommand(index++);
            commandList.Add(command1);

            var command2 = GetLogicResetCommand(index++);
            commandList.Add(command2);

            var command3 = GetSubcarrierSpacingCommand(index++);
            commandList.Add(command3);

            var command4 = GetTBSizeCommand(index++);
            commandList.Add(command4);

            var command5 = GetCrcModeCommand(index++);
            commandList.Add(command5);

            var command6 = GetCbConfigurationCommand(index++);
            commandList.Add(command6);

            var command7 = GetFillIn0Command(index++);
            commandList.Add(command7);

            var command8 = GetLDPCEncodingConfigurationCommand(index++);
            commandList.Add(command8);

            var command9 = GetLDPCRVCommand(index++);
            commandList.Add(command9);

            var command10 = GetLDPCRateMatchingSettingsCommand(index++);
            commandList.Add(command10);

            var command11 = GetInterweavingSettingsCommand(index++);
            commandList.Add(command11);

            var command12 = GetModulationModeCommand(index++);
            commandList.Add(command12);

            var command13 = GetScramblingRandomSeedCommand(index++);
            commandList.Add(command13);

            var command14 = GetFftLengthCommand(index++);
            commandList.Add(command14);

            var command15 = GetDmrsSettingsCommand(index++);
            commandList.Add(command15);

            var command16 = GetCpSettingsCommand(index++);
            commandList.Add(command16);

            var command17 = GetPdschFormCommand(index++);
            commandList.Add(command17);

            var command18 = GetDmrsFormCommand(index++);
            commandList.Add(command18);

            var command19 = GetRBSelect(index++);
            commandList.Add(command19);

            return commandList;
        }


        private void ExecuteSendCommand(CommandDataModel model)
        {
            //长度反序（大端字节序）
            var length = BitConverter.GetBytes(model.CommnadLength).Reverse().ToArray();
            var id = model.CommandId.ToByteArray();
            var data = string.IsNullOrEmpty(model.CommandContent) ? null : model.CommandContent.ToByteArray();
            var commandList = new List<byte[]>();

            if (data != null && data.Length > _sendByteMax)
            {
                var span = data.AsSpan();
                var count = data.Length % _sendByteMax != 0 ? data.Length / _sendByteMax + 1 : data.Length / _sendByteMax;
                for (var i = 0; i < count; i++)
                {
                    var tempData = i == count - 1 ? span.Slice(i * _sendByteMax) : span.Slice(i * _sendByteMax, _sendByteMax);
                    var tempLength = BitConverter.GetBytes((short)tempData.Length).Reverse().ToArray();
                    commandList.Add(GetCommand(tempLength, id, tempData.ToArray()));
                }
            }
            else
                commandList.Add(GetCommand(length, id, data));
#if SIMULATION
            try
            {
                foreach (var command in commandList)
                {
                    ((CommandInformationView)_commandInformationView)?.AppendWriteLine(new RecordMessageDataModel()
                    {
                        RecordTime = DateTime.Now,
                        RecordMessage = $"发送消息:{command?.ToAppendString()}"
                    });
                }
            }
            catch (Exception e)
            {
                //todo:记录日志 
                _dialogService.ShowDialog("AlertDialog", new DialogParameters($"message={e.Message}"));
            }
#else
            try
            {
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
                    _dialogService.ShowDialog("AlertDialog", new DialogParameters($"message=未能找到已连接的设备"));
                    return;
                }

                foreach (var command in commandList)
                {
                    _connectedInstrument.Send(command);
                }
            }
            catch (Exception e)
            {
                //todo:记录日志 
                _dialogService.ShowDialog("AlertDialog", new DialogParameters($"message={e.Message}"));

            }
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

            //Board ID
            cmd[pos++] = (byte)BoardID;

            //指令长度
            Buffer.BlockCopy(length, 0, cmd, pos, length.Length);
            pos = pos + length.Length;

            //指令ID
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

        private void ExecuteImportDataCommand(CommandDataModel model)
        {
            var charLength = model.CommnadLength * 2;
            var content = new StringBuilder();
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            bool? result = openFileDialog.ShowDialog();
            // 检查用户是否选择了文件
            if (result == true)
            {
                // 获取用户选择的文件路径
                string filePath = openFileDialog.FileName;

                try
                {
                    // 读取文件内容
                    FileReadHelper.ReadFileByLines(filePath, (rowContent) =>
                    {
                        var lastLength = charLength - content.Length;
                        var result = true;
                        if (lastLength >= rowContent.Length)
                        {
                            content.Append(rowContent);
                        }
                        else if (lastLength < rowContent.Length)
                        {
                            content.Append(rowContent.Take(lastLength));
                            result = false;
                        }
                        return result;
                    });

                    model.CommandContent = $"0x{content.ToString()}";

                    ((CommandInformationView)_commandInformationView)?.AppendWriteLine(new RecordMessageDataModel()
                    {
                        RecordTime = DateTime.Now,
                        RecordMessage = $"导入数据成功！"
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"读取文件时出错：{ex.Message}");
                }
            }
        }

        private void ExecuteViewDetailCommand(CommandDataModel model)
        {
            if (string.IsNullOrEmpty(model.CommandContent))
                _dialogService.ShowDialog("AlertDialog", new DialogParameters($"message=无法查看空的数据详情"));
            else
                //todo:记录日志 
                _dialogService.ShowDialog("ViewDataDetailView", new DialogParameters($"commandContent={model.CommandContent}"));
        }

        #region 遥测指令
        /// <summary>
        /// 启动信号
        /// </summary>
        private CommandDataModel GetStartSignalCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "启动信号",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                CommandId = "0x0119",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }

        /// <summary>
        /// 逻辑复位
        /// </summary>
        private CommandDataModel GetLogicResetCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "逻辑复位",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                CommandId = "0x0120",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }

        /// <summary>
        /// 子载波间隔
        /// </summary>
        private CommandDataModel GetSubcarrierSpacingCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "子载波间隔",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                CommandId = "0x0121",
                CommandContent = "0x00",
                CommnadLength = 1,
                ContentEnable = true,
                InputMode = Commons.Enums.InputModeEnum.Combobox,
                ComboboxDataSourceType = Commons.Enums.ComboboxDataSourceTypeEnum.SubcarrierSpacing,
                ComboboxDataSource = SubcarrierSpacingDic

            };
            return command;
        }

        private Dictionary<string, string> SubcarrierSpacingDic { get; } = new Dictionary<string, string>()
        {
            {"0x00","15kHz" },
            {"0x01","30kHz" },
            {"0x02","60kHz" },
            {"0x03","120kHz" },
            {"0x04","240kHz" },
            {"0x05","480kHz" },
            {"0x06","960kHz" },
        };


        /// <summary>
        /// TB大小
        /// </summary>
        private CommandDataModel GetTBSizeCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "TB大小",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = true,
                CommnadLength = 4,
                CommandId = "0x0122",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }

        /// <summary>
        /// Crc模式
        /// </summary>
        private CommandDataModel GetCrcModeCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "Crc模式",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                CommandId = "0x0123",
                CommandContent = "0x00",
                CommnadLength = 1,
                ContentEnable = true,
                InputMode = Commons.Enums.InputModeEnum.Combobox,
                ComboboxDataSourceType = Commons.Enums.ComboboxDataSourceTypeEnum.CrcMode,
                ComboboxDataSource = CrcModeDic
            };
            return command;
        }

        private Dictionary<string, string> CrcModeDic { get; } = new Dictionary<string, string>()
        {
            {"0x00","CRC24A" },
            {"0x01","CRC24B" },
            {"0x02","CRC24C" },
            {"0x03","CRC16" }
        };

        /// <summary>
        /// Cb配置
        /// </summary>
        private CommandDataModel GetCbConfigurationCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "Cb配置",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = true,
                CommnadLength = 4,
                CommandId = "0x0124",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }


        /// <summary>
        /// 填0数
        /// </summary>
        private CommandDataModel GetFillIn0Command(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "填0数",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = true,
                CommnadLength = 2,
                CommandId = "0x0125",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }

        /// <summary>
        /// LDPC编码配置
        /// </summary>
        private CommandDataModel GetLDPCEncodingConfigurationCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "LDPC编码配置",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = true,
                CommnadLength = 2,
                CommandId = "0x0126",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }

        /// <summary>
        /// LDPCR VC
        /// </summary>
        private CommandDataModel GetLDPCRVCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "LDPCR VC",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                CommandId = "0x0127",
                CommandContent = "0x00",
                CommnadLength = 1,
                ContentEnable = true,
                InputMode = Commons.Enums.InputModeEnum.Combobox,
                ComboboxDataSourceType = Commons.Enums.ComboboxDataSourceTypeEnum.LDPCRV,
                ComboboxDataSource = LDPCRVCDic
            };
            return command;
        }

        private Dictionary<string, string> LDPCRVCDic { get; } = new Dictionary<string, string>()
        {
            {"0x00","RV0" },
            {"0x01","RV1" },
            {"0x02","RV2" },
            {"0x03","RV3" }
        };

        /// <summary>
        /// LDPC速率匹配设置
        /// </summary>
        private CommandDataModel GetLDPCRateMatchingSettingsCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "LDPC速率匹配设置",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = true,
                CommnadLength = 4,
                CommandId = "0x0128",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }

        /// <summary>
        /// 交织设置
        /// </summary>
        private CommandDataModel GetInterweavingSettingsCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "交织设置",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = true,
                CommnadLength = 4,
                CommandId = "0x0129",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }

        /// <summary>
        /// 调制方式
        /// </summary>
        private CommandDataModel GetModulationModeCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "调制方式",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                CommandId = "0x012a",
                CommandContent = "0x00",
                CommnadLength = 1,
                ContentEnable = true,
                InputMode = Commons.Enums.InputModeEnum.Combobox,
                ComboboxDataSourceType = Commons.Enums.ComboboxDataSourceTypeEnum.Modulation,
                ComboboxDataSource = ModulationModeDic
            };
            return command;
        }

        private Dictionary<string, string> ModulationModeDic { get; } = new Dictionary<string, string>()
        {
            {"0x00","Pi/2-BPSK" },
            {"0x01","BPSK" },
            {"0x02","QPSK" },
            {"0x03","16QAM" },
            {"0x04","64QAM" },
            {"0x05","256QAM" },
            {"0x06","1024QAM" }
        };

        /// <summary>
        /// 扰码随机种子
        /// </summary>
        private CommandDataModel GetScramblingRandomSeedCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "扰码随机种子",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = true,
                CommnadLength = 8,
                CommandId = "0x012b",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }

        /// <summary>
        /// Fft长度
        /// </summary>
        private CommandDataModel GetFftLengthCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "Fft长度",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = true,
                CommnadLength = 2,
                CommandId = "0x012c",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }

        /// <summary>
        /// Dmrs设置
        /// </summary>
        private CommandDataModel GetDmrsSettingsCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "Dmrs设置",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = true,
                CommnadLength = 25,
                CommandId = "0x012d",
                InputMode = Commons.Enums.InputModeEnum.Dialog
            };
            return command;
        }

        /// <summary>
        /// Cp配置
        /// </summary>
        private CommandDataModel GetCpSettingsCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "Cp配置",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = true,
                CommnadLength = 20,
                CommandId = "0x012e",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }

        /// <summary>
        /// Pdsch表格
        /// </summary>
        private CommandDataModel GetPdschFormCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "Pdsch表格",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = false,
                CommnadLength = 1792 * 4,
                CommandId = "0x012f",
                InputMode = Commons.Enums.InputModeEnum.Dialog
            };
            return command;
        }

        /// <summary>
        /// Dmrs表格
        /// </summary>
        private CommandDataModel GetDmrsFormCommand(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "Dmrs表格",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = false,
                CommnadLength = 1792 * 4,
                CommandId = "0x0136",
                InputMode = Commons.Enums.InputModeEnum.Dialog
            };
            return command;
        }


        /// <summary>
        /// RB选择
        /// </summary>
        private CommandDataModel GetRBSelect(int index)
        {
            var command = new CommandDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                Index = index,
                CommandName = "RB选择",
                CommandHead = _commandHead,
                CommandEnd = _commandEnd,
                ContentEnable = false,
                CommnadLength = 4,
                CommandId = "0x013d",
                InputMode = Commons.Enums.InputModeEnum.Direct
            };
            return command;
        }
        #endregion
    }
}
