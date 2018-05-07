using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
//using System.Windows;
using System.Windows.Forms;
using BD_Protocol;

namespace WpfApp_BD
{
    public class mySerialPort
    {
        private SerialPort ComDevice = new SerialPort();
        public mySerialPort()
        {
           // InitializeComponent();
            this.Init();
        }
        public void Init()
        {
            //btnSend.Enabled = false;
            //cbbComList.Items.AddRange(SerialPort.GetPortNames());
            //if (cbbComList.Items.Count > 0)
            //{
            //    cbbComList.SelectedIndex = 0;
            //}
            //cbbBaudRate.SelectedIndex = 5;
            //cbbDataBits.SelectedIndex = 0;
            //cbbParity.SelectedIndex = 0;
            //cbbStopBits.SelectedIndex = 0;
            //pictureBox1.BackgroundImage = Properties.Resources.red;

            ComDevice.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);//绑定事件
            if (ComDevice.IsOpen == false)
            {
                //ComDevice.PortName = cbbComList.SelectedItem.ToString();
                //ComDevice.BaudRate = Convert.ToInt32(cbbBaudRate.SelectedItem.ToString());
                //ComDevice.Parity = (Parity)Convert.ToInt32(cbbParity.SelectedIndex.ToString());
                //ComDevice.DataBits = Convert.ToInt32(cbbDataBits.SelectedItem.ToString());
                //ComDevice.StopBits = (StopBits)Convert.ToInt32(cbbStopBits.SelectedItem.ToString());
                ComDevice.PortName = "COM4";
                ComDevice.BaudRate = 19200;
                ComDevice.Parity = (Parity)0;
                ComDevice.DataBits = 8;
                ComDevice.StopBits = (StopBits)1;
                try
                {
                    ComDevice.Open();
                    //btnSend.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //btnOpen.Text = "关闭串口";
                //pictureBox1.BackgroundImage = Properties.Resources.green;
            }
            else
            {
                try
                {
                    ComDevice.Close();
                    // btnSend.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                //btnOpen.Text = "打开串口";
                //pictureBox1.BackgroundImage = Properties.Resources.red;
            }

        }



        /// <summary>
        /// 关闭串口
        /// </summary>
        public void ClearSelf()
        {
            if (ComDevice.IsOpen)
            {
                ComDevice.Close();
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public bool SendData(byte[] data)
        {
            if (ComDevice.IsOpen)
            {
                try
                {
                    ComDevice.Write(data, 0, data.Length);//发送数据
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("串口未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

 

        /// <summary>
        /// 字符串转换16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0) hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
            return returnBytes;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] ReDatas = new byte[ComDevice.BytesToRead];
            ComDevice.Read(ReDatas, 0, ReDatas.Length);//读取数据
            for(int i=0;i<ReDatas.Length;++i)
            {
                if (((myStartMain.bdxx.rebuff.wp + 1) & BD.RE_BUFFER_SIZE) != myStartMain.bdxx.rebuff.rp)
                {
                    myStartMain.bdxx.rebuff.buffer[myStartMain.bdxx.rebuff.wp++] = ReDatas[i];
                    if (myStartMain.bdxx.rebuff.wp == BD.RE_BUFFER_SIZE + 1)
                        myStartMain.bdxx.rebuff.wp = 0;
                }
            }
        }

    }
}
