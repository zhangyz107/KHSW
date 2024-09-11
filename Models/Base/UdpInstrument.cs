using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Windows.Interop;
using Khsw.Instrument.Demo.Commons.Enums;

namespace Khsw.Instrument.Demo.Models.Base
{
    /// <summary>
    /// Udp设备
    /// </summary>
    public partial class UdpInstrument
    {
        #region Field
        //发送队列
        private readonly ConcurrentQueue<string> _sendQueue = new ConcurrentQueue<string>();
        private readonly ConcurrentQueue<ReceiveMessageInfo> _receiveQueue = new ConcurrentQueue<ReceiveMessageInfo>();
        #endregion

        #region Properties
        //接收数据事件
        public Action<UdpInstrument> ReceiveMessageEvent = null;
        #endregion

        #region Public
        /// <summary>
        /// 创建连接
        /// </summary>
        public bool CreateConnect()
        {
            this.IsConnected = false;
            try
            {
                if (string.IsNullOrEmpty(IpAddress))
                    throw new ArgumentNullException(nameof(IpAddress), "当前设备地址为空！");

                if (!IsMulticasst && !IsHostOnline(IPAddress.Parse(IpAddress)))
                    throw new ArgumentException($"当前IP地址:{IpAddress}无法Ping通！");

                if (UdpClient == null)
                {
                    UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, this.LocalPort));
                    var targetEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
                    UdpClient.Connect(targetEndPoint);
                    if (IsMulticasst)
                    {
                        UdpClient.JoinMulticastGroup(targetEndPoint.Address);
                    }
                    UdpClient.Client.SendBufferSize = int.MaxValue;
                    UdpClient.Client.ReceiveBufferSize = int.MaxValue;
                }

                IsConnected = true;

                StartSendQueueTask();
                StartReceiveTask();
            }
            catch (Exception ex)
            {
                //todo:记录日志 
                Console.WriteLine(ex.Message);
            }

            return IsConnected;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public bool DestroyConnect()
        {
            bool isSuccess = false;
            try
            {
                if (UdpClient != null)
                    UdpClient.Close();
                UdpClient = null;
                IsConnected = false;
                isSuccess = true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                UdpClient = null;
                //todo:记录日志 
                Console.WriteLine(ex.Message);
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="data"></param>
        public void Send(string data)
        {
            if (string.IsNullOrEmpty(data)) { return; }

            if (_sendQueue.Count > MaxSendCount)
            {
                _sendQueue.TryDequeue(out string msg);
                //todo:记录被剔除的消息
                Console.WriteLine($"发送内容:{msg}被剔除发送队列");
            }
            _sendQueue.Enqueue(data);
        }

        /// <summary>
        /// 发送命令行
        /// </summary>
        public void SendLine(string data)
        {
            if (string.IsNullOrEmpty(data)) { return; }

            if (!data.EndsWith(this.StringEncoder.GetString(Delimiter)))
            {
                Send(data + StringEncoder.GetString(Delimiter));
            }
            else
            {
                Send(data);
            }
        }

        /// <summary>
        /// 从队列中获取消息
        /// </summary>
        public ReceiveMessageInfo GetReceiveMessageFromQueue()
        {
            _receiveQueue.TryDequeue(out ReceiveMessageInfo msg);
            return msg;
        }

        /// <summary>
        /// 清空接收消息
        /// </summary>
        public void ClearReceiveMessage()
        {
            _receiveQueue.Clear();
        }

        #endregion


        #region Private
        private bool IsHostOnline(IPAddress ipAddress)
        {
            Ping pingtest = new Ping();
            PingOptions myOptions = new PingOptions();
            myOptions.DontFragment = true;
            string data = "test";
            byte[] buff = Encoding.ASCII.GetBytes(data);
            PingReply reply = pingtest.Send(ipAddress, 1000, buff, myOptions);
            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 开启发送队列任务
        /// </summary>
        private void StartSendQueueTask()
        {
            Task.Factory.StartNew(async () =>
            {
                while (IsConnected)
                {
                    try
                    {
                        if (_sendQueue.TryDequeue(out string msg) && !string.IsNullOrEmpty(msg))
                            Send(StringEncoder.GetBytes(msg));
                        else
                            await Task.Delay(10);

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

            }, TaskCreationOptions.LongRunning);
        }

        private void Send(byte[] data)
        {
            try
            {
                if (UdpClient == null)
                {
                    throw new Exception("无法发送到空设备 (检查设备是否正常连接)！");
                }
                UdpClient.Client.SendTimeout = this.SendTimeOut;
                UdpClient.Client.Send(data);
            }
            catch
            {
                throw;
            }
        }

        private void StartReceiveTask()
        {
            Task.Factory.StartNew(async () =>
            {
                while (IsConnected)
                {
                    try
                    {
                        var result = await UdpClient.ReceiveAsync();
                        if (result.Buffer.Any())
                        {
                            var message = Encoding.UTF8.GetString(result.Buffer);
                            if (_receiveQueue.Count > MaxSendCount)
                            {
                                _receiveQueue.TryDequeue(out ReceiveMessageInfo msg);
                                //todo:记录被剔除的消息
                                Console.WriteLine($"接收时间:{msg.ReceiveTime},内容为:{msg.ReceiveContent}被剔除接收消息队列");
                            }

                            _receiveQueue.Enqueue(new ReceiveMessageInfo()
                            {
                                ReceiveTime = DateTime.Now,
                                ReceiveContent = message
                            });

                            if (ReceiveMessageEvent != null)
                                ReceiveMessageEvent(this);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }

            }, TaskCreationOptions.LongRunning);
        }

        #endregion




    }
}
