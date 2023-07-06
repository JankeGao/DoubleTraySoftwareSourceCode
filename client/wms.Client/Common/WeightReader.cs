using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using HP.Utility.Data;

namespace wms.Client.Common
{
    /// <summary>电子称数据读取类</summary>
    public class WeightReader : IDisposable
    {
        /// <summary>电子秤接口信息类，封装COM口数据</summary>
        public class WeightInformation
        {
            /// <summary>获取或设置重量</summary>
            public decimal WData { get; set; }
            /// <summary>获取或设置数量</summary>
            public string QData { get; set; }
            /// <summary>获取或设置百分数</summary>
            public string Percentage { get; set; }
        }

        #region 成员
        SerialPort serialPort;

        int speed = 300;
        /// <summary>获取或设置电脑取COM数据缓冲时间，单位毫秒</summary>
        public int Speed
        {
            get { return speed; }
            set
            {
                if (value < 300)
                    throw new Exception("串口读取缓冲时间不能小于300毫秒!");
                speed = value;
            }
        }

        /// <summary></summary>
        public string StartKey = "wm";
        /// <summary></summary>
        public string UnitKey = "kg";
        /// <summary></summary>
        public string MatchPattern = @"wn\w+.\w+kg";

        WeightInformation weightInformation = new WeightInformation();
        /// <summary></summary>
        public WeightInformation WeightInformationObj
        {
            get { return weightInformation; }
        }

        /// <summary>页变化时引发的事件</summary>
        public event EventHandler Changed;
        /// <summary>引发Changed事件</summary>
        protected void OnChanged()
        {
            try
            {
                if (Changed != null)
                    Changed(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        #endregion 成员

        #region 构造与析构
        /// <summary></summary>
        public void Dispose()
        {
            Close();
            serialPort = null;
        }
        #endregion 构造与析构

        /// <summary>初始化串口</summary>
        /// <param name="portName">数据传输端口</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="speed">串口读数缓冲时间</param>
        /// <returns></returns>
        public bool Open(string portName, int baudRate = 9600, int speed = 300, int readTimeout = 600, int writeTimeout = 1200)
        {
            Close();
            try
            {
                serialPort = new SerialPort();
                serialPort.PortName = portName;
                serialPort.BaudRate = Convert.ToInt32(baudRate);
                serialPort.Parity = (Parity)Convert.ToInt32(0);
                serialPort.DataBits = Convert.ToInt32(8);
                serialPort.StopBits = (StopBits)Convert.ToInt32(1);
                serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                serialPort.ReadTimeout = 600;
                serialPort.WriteTimeout = 1200;

                if (!serialPort.IsOpen)
                    serialPort.Open();
                return true;
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("无法初始化串口{0}!", portName), exp);
            }
        }

        /// <summary>由于总总原因导致连接断开，重连串口</summary>
        public bool ReOpen()
        {
            try
            {
                if (serialPort == null)
                    return false;

                if (!serialPort.CtsHolding)
                {
                    if (serialPort.IsOpen)
                        serialPort.Close();
                    serialPort.Open();
                }
                return true;
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("无法打开串口{0}!", serialPort.PortName), exp);
            }
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (!serialPort.IsOpen)
                    return;
                int byteNumber = serialPort.BytesToRead; ;

                Thread.Sleep(50);
                //延时等待数据接收完毕。
                while ((byteNumber < serialPort.BytesToRead) && (serialPort.BytesToRead < 4800))
                {
                    byteNumber = serialPort.BytesToRead;
                    Thread.Sleep(50);
                }

                int n = serialPort.BytesToRead; //记录下缓冲区的字节个数 
                byte[] ReDatas = new byte[n]; //声明一个临时数组存储当前来的串口数据 
                serialPort.Read(ReDatas, 0, n); //读取缓冲数据到buf中，同时将这串数据从缓冲区移除 
                if (ReDatas.Length ==9)
                {
                    var tempature = (ReDatas[5] * 256 + ReDatas[6]) / 100.00;
                    weightInformation.WData = decimal.Parse(tempature.ToString());
                    OnChanged();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
              //  throw new Exception(string.Format(ex.Message));
            }

        }

        /// <summary></summary>
        public void Close()
        {
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();
        }


        /// <summary>
        /// 字符串转换16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] strToHexByte(string hexString)
        {
            try
            {
                hexString = hexString.Replace(" ", "");
                if ((hexString.Length % 2) != 0) hexString += " ";
                byte[] returnBytes = new byte[hexString.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
                return returnBytes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public DataResult SendData(byte[] data)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(data, 0, data.Length);//发送数据
                    return DataProcess.Success();
                }
                catch (Exception ex)
                {
                    return DataProcess.Failure("错误"+ex.Message);

                  //  MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                return DataProcess.Failure("串口未打开");
            }
            return DataProcess.Failure("串口未打开");
        }
    }
}
