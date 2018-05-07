using BD_Protocol;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp_BD
{
    /// <summary>
    /// COMInit.xaml 的交互逻辑
    /// </summary>
    public partial class COMInit : Window
    {

        SerialPort ComPort = new SerialPort();//声明一个串口      
        private string[] ports;//可用串口数组
        IList<customer> comList = new List<customer>();//可用串口集合
        public COMInit()
        {
            InitializeComponent();
        }
        public class customer//各下拉控件访问接口
        {

            public string com { get; set; }//可用串口
            public string com1 { get; set; }//可用串口
            public string BaudRate { get; set; }//波特率
            //public string Parity { get; set; }//校验位
            //public string ParityValue { get; set; }//校验位对应值
            //public string Dbits { get; set; }//数据位
            //public string Sbits { get; set; }//停止位


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //↓↓↓↓↓↓↓↓↓可用串口下拉控件↓↓↓↓↓↓↓↓↓
            ports = SerialPort.GetPortNames();//获取可用串口
            if (ports.Length > 0)//ports.Length > 0说明有串口可用
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    comList.Add(new customer() { com = ports[i] });//下拉控件里添加可用串口
                }
                AvailableComCbobox.ItemsSource = comList;//资源路劲
                AvailableComCbobox.DisplayMemberPath = "com";//显示路径
                AvailableComCbobox.SelectedValuePath = "com";//值路径
                AvailableComCbobox.SelectedValue = ports[0];//默认选第1个串口
            }
            else//未检测到串口
            {
                MessageBox.Show("无可用串口");
            }
            //↑↑↑↑↑↑↑↑↑可用串口下拉控件↑↑↑↑↑↑↑↑↑

            //↓↓↓↓↓↓↓↓↓波特率下拉控件↓↓↓↓↓↓↓↓↓
            IList<customer> rateList = new List<customer>();//可用波特率集合
            rateList.Add(new customer() { BaudRate = "1200" });
            rateList.Add(new customer() { BaudRate = "2400" });
            rateList.Add(new customer() { BaudRate = "4800" });
            rateList.Add(new customer() { BaudRate = "9600" });
            rateList.Add(new customer() { BaudRate = "19200" });
            rateList.Add(new customer() { BaudRate = "38400" });
            rateList.Add(new customer() { BaudRate = "57600" });
            rateList.Add(new customer() { BaudRate = "115200" });
            RateListCbobox.ItemsSource = rateList;
            RateListCbobox.DisplayMemberPath = "BaudRate";
            RateListCbobox.SelectedValuePath = "BaudRate";
            //↑↑↑↑↑↑↑↑↑波特率下拉控件↑↑↑↑↑↑↑↑↑
            //↓↓↓↓↓↓↓↓↓默认设置↓↓↓↓↓↓↓↓↓
            RateListCbobox.SelectedValue = "19200";//波特率默认设置19200
            ComPort.ReadTimeout = 8000;//串口读超时8秒
            ComPort.WriteTimeout = 8000;//串口写超时8秒，在1ms自动发送数据时拔掉串口，写超时5秒后，会自动停止发送，如果无超时设定，这时程序假死
            ComPort.ReadBufferSize = 1024;//数据读缓存
            ComPort.WriteBufferSize = 1024;//数据写缓存
            //↑↑↑↑↑↑↑↑↑默认设置↑↑↑↑↑↑↑↑↑
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();//先停止线程,然后终止进程.
            Environment.Exit(0);//直接终止进程.
        }

        private void cb_open_Click(object sender, RoutedEventArgs e)
        {
            MynewCOM mycom;
            BD bdxx = new BD();
            if (AvailableComCbobox.SelectedValue == null)//先判断是否有可用串口
            {
                MessageBox.Show("无可用串口，无法打开!");
                return;//没有串口，提示后直接返回
            }
            try//尝试打开串口
            {
                ComPort.PortName = AvailableComCbobox.SelectedValue.ToString();//设置要打开的串口
                ComPort.BaudRate = Convert.ToInt32(RateListCbobox.SelectedValue);//设置当前波特率
                ComPort.Parity = (Parity)0;//设置当前校验位
                ComPort.DataBits = 8;//设置当前数据位
                ComPort.StopBits = (StopBits)1;//设置当前停止位                    
                //ComPort.Open();//打开串口
                mycom = new MynewCOM(AvailableComCbobox.SelectedValue.ToString(), Convert.ToInt32(RateListCbobox.SelectedValue), bdxx);
                mycom.Open();
                bdxx.Init(mycom);
                this.Hide();
                Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
                MainWindow window = new MainWindow(bdxx);
                window.ShowDialog();
            }
            catch//如果串口被其他占用，则无法打开
            {
                MessageBox.Show("无法打开串口,请检测此串口是否有效或被其他占用！");
                GetPort();//刷新当前可用串口
                return;//无法打开串口，提示后直接返回
            }


        }
        private void GetPort()//刷新可用串口
        {

            comList.Clear();//情况控件链接资源
            AvailableComCbobox.DisplayMemberPath = "com1";
            AvailableComCbobox.SelectedValuePath = null;//路径都指为空，清空下拉控件显示，下面重新添加

            ports = new string[SerialPort.GetPortNames().Length];//重新定义可用串口数组长度
            ports = SerialPort.GetPortNames();//获取可用串口
            if (ports.Length > 0)//有可用串口
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    comList.Add(new customer() { com = ports[i] });//下拉控件里添加可用串口
                }
                AvailableComCbobox.ItemsSource = comList;//可用串口下拉控件资源路径
                AvailableComCbobox.DisplayMemberPath = "com";//可用串口下拉控件显示路径
                AvailableComCbobox.SelectedValuePath = "com";//可用串口下拉控件值路径
            }


        }
    }
}
