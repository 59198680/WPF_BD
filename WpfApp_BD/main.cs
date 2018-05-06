using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using BD_Protocol;

namespace WpfApp_BD
{
    static class myStartMain
    {
        public static void ThreadMethod1()
        {
            while(true)
            {
                bdxx.Receive_Protocol();
               // Console.WriteLine("不带参数的线程函数");
                Thread.Sleep(50);
            }
           
        }
        public static void ThreadMethod2(object obj)
        {
            MainWindow win =  (MainWindow)obj;
            while (true)
            {
                // Console.WriteLine("不带参数的线程函数");
                bdxx.Check_status();
                if (bdxx.SEND_BLOCKTIME!=0)
                {
                    --bdxx.SEND_BLOCKTIME;
                    bdxx.print_flag |= BD.PRINT_BLOCK;
                }
                    
                Thread.Sleep(1000);
            }
        }


        static public WpfApp_BD.App app;
        static public mySerialPort mycom;
        static public BD bdxx;
       [STAThread]
        static void Main(string[] args)
        {
            mycom = new mySerialPort();
            bdxx = new BD(mycom);
            MainWindow windows = new MainWindow();
            Thread t1 = new Thread(new ThreadStart(ThreadMethod1));
            Thread t2 = new Thread(new ParameterizedThreadStart(ThreadMethod2));//ParameterizedThreadStart
            t1.Start();
            t2.Start(windows);

            //mycom = new mySerialPort();
            app = new WpfApp_BD.App();
            app.InitializeComponent();
            
            app.MainWindow = windows;
            //app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            app.Run();
           
        }
    }
}
