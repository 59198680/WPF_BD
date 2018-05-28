/***********************Project Version1.4*************************
@项目名:北斗传输4.0(C#)
@File:COMInit.xmal.cs
@File_Version:1.0a
@Author:lys
@QQ:591986780
@UpdateTime:2018年5月16日02:25:23

@说明:实现串口的基本功能

本程序基于.Net4.6.1编写的北斗短报文传输程序
界面使用WPF框架编写
在vs2017里运行通过

******************************************************************/
using BD_Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WpfApp_BD
{
    public class MynewCOM
    {
        BD bdxx;
        SerialPort ComPort = new SerialPort();//声明一个串口      
        //private bool WaitClose = false;//invoke里判断是否正在关闭串口是否正在关闭串口，执行Application.DoEvents，并阻止再次invoke ,解决关闭串口时，程序假死，具体参见http://news.ccidnet.com/art/32859/20100524/2067861_4.html 仅在单线程收发使用，但是在公共代码区有相关设置，所以未用#define隔离
        private static bool Sending = false;//正在发送数据状态字
        private static Thread _ComSend;//发送数据线程
        public MynewCOM(string PortName, int BaudRate, BD b)
        {
            bdxx = b;
            ComPort.PortName = PortName;
            ComPort.BaudRate = BaudRate;
            ComPort.Parity = (Parity)0;
            ComPort.DataBits = 8;
            ComPort.StopBits = (StopBits)1;
            ComPort.ReadTimeout = 8000;//串口读超时8秒
            ComPort.WriteTimeout = 8000;//串口写超时8秒，在1ms自动发送数据时拔掉串口，写超时5秒后，会自动停止发送，如果无超时设定，这时程序假死
            ComPort.ReadBufferSize = 1024;//数据读缓存
            ComPort.WriteBufferSize = 1024;//数据写缓存
            ComPort.DataReceived += new SerialDataReceivedEventHandler(ComReceive);//串口接收中断
        }
        public bool Open()
        {
            bool res = true;
            try
            {
                ComPort.Open();//打开串口
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + "无法打开，原因未知！");
                //MessageBox.Show("无法打开串口,请检测此串口是否有效或被其他占用！");
                res = false;
                throw;
            }
            return res;

        }
        public bool Close()
        {
            bool res = true;
            try
            {
                ComPort.Close();//打开串口
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + "无法关闭，原因未知！");
                //MessageBox.Show("无法打开串口,请检测此串口是否有效或被其他占用！");
                res = false;
                throw;
            }
            return res;

        }
        public void Send(byte[] sendbuffer)//发送数据，分为多线程方式和单线程方式
        {
            //if (Sending == true) return;//如果当前正在发送，则取消本次发送，本句注释后，可能阻塞在ComSend的lock处
            _ComSend = new Thread(new ParameterizedThreadStart(ComSend)); //new发送线程
            _ComSend.Start(sendbuffer);//发送线程启动
        }

        private void ComSend(Object obj)//发送数据 独立线程方法 发送数据时UI可以响应
        {

            lock (this)//由于send()中的if (Sending == true) return，所以这里不会产生阻塞，如果没有那句，多次启动该线程，会在此处排队
            {
                Sending = true;//正在发生状态字
                byte[] sendBuffer = obj as byte[];//发送数据缓冲区
                                                  // string sendData = SendSet.SendSetData;//复制发送数据，以免发送过程中数据被手动改变

                try//尝试发送数据
                {//如果发送字节数大于1000，则每1000字节发送一次
                    int sendTimes = (sendBuffer.Length / 1000);//发送次数
                    for (int i = 0; i < sendTimes; i++)//每次发生1000Bytes
                    {
                        ComPort.Write(sendBuffer, i * 1000, 1000);//发送sendBuffer中从第i * 1000字节开始的1000Bytes

                    }
                    if (sendBuffer.Length % 1000 != 0)//发送字节小于1000Bytes或上面发送剩余的数据
                    {
                        ComPort.Write(sendBuffer, sendTimes * 1000, sendBuffer.Length % 1000);
                    }


                }
                catch (Exception e)//如果无法发送，产生异常
                {
                    MessageBox.Show(e.ToString() + "无法接收数据，原因未知！");
                }
                //sendScrol.ScrollToBottom();//发送数据区滚动到底部
                Sending = false;//关闭正在发送状态
               // _ComSend.Abort();//终止本线程
            }

        }

        private void ComReceive(object sender, SerialDataReceivedEventArgs e)//接收数据 中断只标志有数据需要读取，读取操作在中断外进行
        {
            //if (WaitClose) return;//如果正在关闭串口，则直接返回
            //Thread.Sleep(10);//发送和接收均为文本时，接收中为加入判断是否为文字的算法，发送你（C4E3），接收可能识别为C4,E3，可用在这里加延时解决
            byte[] recBuffer;//接收缓冲区
            try
            {
                recBuffer = new byte[ComPort.BytesToRead];//接收数据缓存大小
                ComPort.Read(recBuffer, 0, recBuffer.Length);//读取数据
                for (int i = 0; i < recBuffer.Length; ++i)
                {
                    if (((bdxx.rebuff.wp + 1) & BD.RE_BUFFER_SIZE) != bdxx.rebuff.rp)
                    {
                        bdxx.rebuff.buffer[bdxx.rebuff.wp++] = recBuffer[i];
                        if (bdxx.rebuff.wp == BD.RE_BUFFER_SIZE + 1)
                            bdxx.rebuff.wp = 0;
                    }
                }
            }
            catch
            {

                MessageBox.Show(e.ToString() + "无法接收数据，原因未知！");
            }
        }
    }
}
