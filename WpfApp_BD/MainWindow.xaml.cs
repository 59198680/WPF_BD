using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BD_Protocol;
namespace WpfApp_BD
{
    public class Box
    {
        public BD bdxx;
        public MainWindow win;
        public Box(MainWindow m, BD bd)
        {
            win = m;
            bdxx = bd;

        }

    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //public static Semaphore sema = new Semaphore(0, 1);
        public BD bdxx;

        public void fun()
        {
            int i = 0;
            while (true)
            {
                // sema.WaitOne();
                i++;
                new Thread(() =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {

                        label_gntx_sj_text.Content = Convert.ToString(i);
                    }));
                }).Start();
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            //Thread asd = new Thread(new ThreadStart(fun));
            //asd.Start();
            Thread print = new Thread(new ParameterizedThreadStart(Seamphore_thread));
            // InitializeComponent();
            bdxx = myStartMain.bdxx;
            print.Start(new Box(this, bdxx));
            //this.Dispatcher.BeginInvoke(new Action(delegate
            //{
            //    while (true)
            //    {
            //        // Console.WriteLine("不带参数的线程函数");
            //        //if (obj != null)
            //        {
            //            //MainWindow temp = (MainWindow)obj;
            //            //if (bdxx != null && temp != null)
            //            Seamphore_thread(new Box(this, bdxx));
            //        }
            //        Thread.Sleep(1000);
            //    }
            //    //这里写代码      
            //}));
            //this.Dispatcher.BeginInvoke(new Action(delegate {
            //    while (true)
            //    {
            //        // Console.WriteLine("不带参数的线程函数");
            //        //if (obj != null)
            //        {
            //            //MainWindow temp = (MainWindow)obj;
            //            //if (bdxx != null && temp != null)
            //            this.Label_djs_text.Content = bdxx.SEND_BLOCKTIME.ToString();
            //        }
            //        Thread.Sleep(1000);
            //    }
            //    //这里写代码      
            //}));
        }
        public static void Seamphore_thread(object obj1)
        {
            BD bdxx = ((Box)obj1).bdxx;
            MainWindow win = ((Box)obj1).win;
            while (true)
            {
                //sema.WaitOne();
                if ((bdxx.print_flag & BD.PRINT_STATUS) != 0)
                {
                    string str = "";
                    if ((bdxx.status & BD.STATUS_BIT_STEP) == BD.STEP_NONE)
                    {
                        str = "未初始化";
                    }
                    else if ((bdxx.status & BD.STATUS_BIT_STEP) == BD.STEP_ICJC)
                    {
                        str = "IC检测";
                    }
                    else if ((bdxx.status & BD.STATUS_BIT_STEP) == BD.STEP_READY)
                    {
                        str = "就绪";
                    }
                    else if ((bdxx.status & BD.STATUS_BIT_STEP) == BD.STEP_SJSC)
                    {
                        str = "时间输出";
                    }
                    else if ((bdxx.status & BD.STATUS_BIT_STEP) == BD.STEP_XJZJ)
                    {
                        str = "系统自检";
                    }
                    else if ((bdxx.status & BD.STATUS_BIT_STEP) == BD.STEP_GNPS)
                    {
                        str = "定位信息";
                    }
                    else if ((bdxx.status & BD.STATUS_BIT_STEP) == BD.STEP_GNTS)
                    {
                        str = "时间信息";
                    }
                    else if ((bdxx.status & BD.STATUS_BIT_STEP) == BD.STEP_GNVS)
                    {
                        str = "可视卫星";
                    }
                    new Thread(() =>
                    {
                        win.Dispatcher.Invoke(new Action(() =>
                        {

                            win.Label_Init_text.Content = str;
                        }));
                    }).Start();
                    bdxx.print_flag &= ~BD.PRINT_STATUS;
                }
                if ((bdxx.print_flag & BD.PRINT_DWXX) != 0)
                {

                }
                if ((bdxx.print_flag & BD.PRINT_BLOCK) != 0)
                {
                    new Thread(() =>
                    {
                        win.Dispatcher.Invoke(new Action(() =>
                        {

                            win.label_djs_text.Content = Convert.ToString(bdxx.SEND_BLOCKTIME) + "s";
                        }));
                    }).Start();
                    bdxx.print_flag &= ~BD.PRINT_BLOCK;
                }
                if ((bdxx.print_flag & BD.PRINT_TXXX) != 0)
                {
                    new Thread(() =>
                    {
                        win.Dispatcher.Invoke(new Action(() =>
                        {

                            win.label_txxx_xxlb_text.Content = Convert.ToString(bdxx.txxx.xxlb, 2);
                            win.label_txxx_fxfd_text.Content = Convert.ToString(bdxx.txxx.fxfdz[0] * 256 * 256 + bdxx.txxx.fxfdz[1] * 256 + bdxx.txxx.fxfdz[2]);
                            win.label_txxx_fxsj_text.Content = Convert.ToString(bdxx.txxx.fxsj_h) + "时" + Convert.ToString(bdxx.txxx.fxsj_m) + "分";
                            win.label_txxx_dwcd_text.Content = Convert.ToString(bdxx.txxx.dwcd / 8.0) + "bytes(" + Convert.ToString(bdxx.txxx.dwcd) + "bits)";
                            win.label_txxx_crc_text.Content = Convert.ToString(bdxx.txxx.crc);
                            win.label_txxx_lasttime_text.Content = Convert.ToString(bdxx.gntx.year) + "年" + Convert.ToString(bdxx.gntx.month) + "月" + Convert.ToString(bdxx.gntx.day) + "日" + Convert.ToString(bdxx.gntx.hour) + ":" + Convert.ToString(bdxx.gntx.minute) + ":" + Convert.ToString(bdxx.gntx.second);
                            if (win.cb_txxx_hexordec.IsChecked == true)
                            {
                                StringBuilder sb = new StringBuilder();
                                for (int i = 0; i < bdxx.txxx.dwnr.Length; i++)
                                {
                                    sb.AppendFormat("{0:x2}" + " ", bdxx.txxx.dwnr[i]);
                                }
                                win.textbox_txxx_dwnr.Text = sb.ToString().ToUpper();
                            }
                            else
                            {
                                win.textbox_txxx_dwnr.Text = new ASCIIEncoding().GetString(bdxx.txxx.dwnr);
                            }
                            // win.textbox_txxx_dwnr.Text = Convert.ToString();

                        }));
                    }).Start();
                    bdxx.print_flag &= ~BD.PRINT_TXXX;
                }
                if ((bdxx.print_flag & BD.PRINT_ICXX) != 0)
                {
                    new Thread(() =>
                    {
                        win.Dispatcher.Invoke(new Action(() =>
                        {
                            win.label_icxx_yhid_text.Content = Convert.ToString(bdxx.icxx.yhdz[0] * 256 * 256 + bdxx.icxx.yhdz[1] * 256 + bdxx.icxx.yhdz[2]);
                            win.label_icxx_zh_text.Content = Convert.ToString(bdxx.icxx.zh);
                            win.label_icxx_tbid_text.Content = Convert.ToString(bdxx.icxx.tbid);
                            win.label_icxx_yhtz_text.Content = Convert.ToString(bdxx.icxx.yhtz);
                            win.label_icxx_fwpd_text.Content = Convert.ToString(bdxx.icxx.fwpd);
                            win.label_icxx_txdj_text.Content = Convert.ToString(bdxx.icxx.txdj);
                            win.label_icxx_jmbz_text.Content = Convert.ToString(bdxx.icxx.jmbz);
                            win.label_icxx_xsyhs_text.Content = Convert.ToString(bdxx.icxx.xsyhzs);
                        }));
                    }).Start();
                    bdxx.print_flag &= ~BD.PRINT_ICXX;
                }
                if ((bdxx.print_flag & BD.PRINT_ZJXX) != 0)
                {
                    new Thread(() =>
                    {
                        win.Dispatcher.Invoke(new Action(() =>
                        {
                            win.label_zjxx_iczt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.iczt, 16);
                            win.label_zjxx_yjzt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.yjzt, 16);
                            win.label_zjxx_dczt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.dcdl, 16);
                            win.label_zjxx_rzzt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.rzzt, 16);
                            win.label_zjxx_bsgl1_text.Content = Convert.ToString(bdxx.zjxx.bsgl[0]);
                            win.label_zjxx_bsgl2_text.Content = Convert.ToString(bdxx.zjxx.bsgl[1]);
                            win.label_zjxx_bsgl3_text.Content = Convert.ToString(bdxx.zjxx.bsgl[2]);
                            win.label_zjxx_bsgl4_text.Content = Convert.ToString(bdxx.zjxx.bsgl[3]);
                            win.label_zjxx_bsgl5_text.Content = Convert.ToString(bdxx.zjxx.bsgl[4]);
                            win.label_zjxx_bsgl6_text.Content = Convert.ToString(bdxx.zjxx.bsgl[5]);
                        }));
                    }).Start();
                    bdxx.print_flag &= ~BD.PRINT_ZJXX;
                }
                if ((bdxx.print_flag & BD.PRINT_SJXX) != 0)
                {

                }
                if ((bdxx.print_flag & BD.PRINT_FKXX) != 0)
                {
                    new Thread(() =>
                    {
                        win.Dispatcher.Invoke(new Action(() =>
                        {
                            string str = "";
                            if (bdxx.fkxx.flbz == 0)
                            {
                                str = "成功,指令:" + (char)(bdxx.fkxx.fjxx[0]) + (char)(bdxx.fkxx.fjxx[1]) + (char)(bdxx.fkxx.fjxx[2]) + (char)(bdxx.fkxx.fjxx[3]);
                                bdxx.SEND_BLOCKTIME = 60;
                            }
                            else if (bdxx.fkxx.flbz == 1)
                                str = "失败,指令:" + (char)(bdxx.fkxx.fjxx[0]) + (char)(bdxx.fkxx.fjxx[1]) + (char)(bdxx.fkxx.fjxx[2]) + (char)(bdxx.fkxx.fjxx[3]);
                            else if (bdxx.fkxx.flbz == 2)
                                str = "信号未锁定";
                            else if (bdxx.fkxx.flbz == 3)
                                str = "电量不足";
                            else if (bdxx.fkxx.flbz == 4)
                                str = "发射频度未到,时间:" + Convert.ToString(bdxx.fkxx.fjxx[3]) + "秒";
                            else if (bdxx.fkxx.flbz == 5)
                                str = "加解密错误";
                            else if (bdxx.fkxx.flbz == 6)
                                str = "CRC错误,指令:" + (char)(bdxx.fkxx.fjxx[0]) + (char)(bdxx.fkxx.fjxx[1]) + (char)(bdxx.fkxx.fjxx[2]) + (char)(bdxx.fkxx.fjxx[3]);
                            else if (bdxx.fkxx.flbz == 7)
                                str = "用户级被抑制";
                            else if (bdxx.fkxx.flbz == 8)
                                str = "抑制解除\n";
                            str += "  " + Convert.ToString(bdxx.gntx.hour) + ":" + Convert.ToString(bdxx.gntx.minute) + ":" + Convert.ToString(bdxx.gntx.second);
                            win.listbox_fkxx.Items.Add(new ListBoxItem().Content = str);
                        }));
                    }).Start();
                    bdxx.print_flag &= ~BD.PRINT_FKXX;
                }
                if ((bdxx.print_flag & BD.PRINT_GNTX) != 0)
                {
                    new Thread(() =>
                    {
                        win.Dispatcher.Invoke(new Action(() =>
                        {
                            sbyte sq = bdxx.gntx.sqlx;
                            string str = "";
                            if (sq >= 0)
                            {
                                str = "东";
                            }
                            else
                            {
                                str = "西";
                                sq *= -1;
                            }

                            win.label_gntx_sq_text.Content = str + Convert.ToString(sq) + "区";
                            win.label_gntx_sj_text.Content = Convert.ToString(bdxx.gntx.year) + "年" + Convert.ToString(bdxx.gntx.month) + "月" + Convert.ToString(bdxx.gntx.day) + "日" + Convert.ToString(bdxx.gntx.hour) + ":" + Convert.ToString(bdxx.gntx.minute) + ":" + Convert.ToString(bdxx.gntx.second);
                        }));
                    }).Start();
                    bdxx.print_flag &= ~BD.PRINT_GNTX;
                }
                if ((bdxx.print_flag & BD.PRINT_GNVX) != 0)
                {
                    new Thread(() =>
                    {
                        win.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            win.label_gnvx_gwxgs_text.Content = Convert.ToString(bdxx.gnvx.gps_wxgs);
                            win.label_gnvx_bwxgs_text.Content = Convert.ToString(bdxx.gnvx.bds_wxgs);
                            win.listbox_gnvx_bwxxx.Items.Clear();
                            win.listbox_gnvx_bwxxx.Items.Add(new ListBoxItem().Content = "(卫星编号)(卫星仰角)(方位角)(信噪比)");
                            win.listbox_gnvx_gwxxx.Items.Clear();
                            win.listbox_gnvx_gwxxx.Items.Add(new ListBoxItem().Content = "(卫星编号)(卫星仰角)(方位角)(信噪比)");
                            for (int i = 0; i < bdxx.gnvx.bds_wxgs; ++i)
                                win.listbox_gnvx_bwxxx.Items.Add(new ListBoxItem().Content = "(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].wxbh) + ")(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].wxyj) + "°)(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].fwj) + "°)(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].xzb) + "db)");
                            for (int i = 0; i < bdxx.gnvx.gps_wxgs; ++i)
                                win.listbox_gnvx_gwxxx.Items.Add(new ListBoxItem().Content = "(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].wxbh) + ")(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].wxyj) + "°)(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].fwj) + "°)(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].xzb) + "db)");
                        }));
                    }).Start();
                    bdxx.print_flag &= ~BD.PRINT_GNVX;
                }
                if ((bdxx.print_flag & BD.PRINT_GNPX) != 0)
                {
                    // float wd, jd;
                    // wd= (float)bdxx.gnpx.wxm / 60
                    new Thread(() =>
                    {
                        win.Dispatcher.Invoke(new Action(() =>
                        {
                            win.label_gnpx_jdfw_text.Content = (char)bdxx.gnpx.jdfw;
                            win.label_gnpx_jd_text.Content = Convert.ToString(bdxx.gnpx.jd);
                            win.label_gnpx_jf_text.Content = Convert.ToString(bdxx.gnpx.jf);
                            win.label_gnpx_jm_text.Content = Convert.ToString(bdxx.gnpx.jm);
                            win.label_gnpx_jxm_text.Content = Convert.ToString(bdxx.gnpx.jxm);
                            win.label_gnpx_wdfw_text.Content = (char)bdxx.gnpx.wdfw;
                            win.label_gnpx_wd_text.Content = Convert.ToString(bdxx.gnpx.wd);
                            win.label_gnpx_wf_text.Content = Convert.ToString(bdxx.gnpx.wf);
                            win.label_gnpx_wm_text.Content = Convert.ToString(bdxx.gnpx.wm);
                            win.label_gnpx_wxm_text.Content = Convert.ToString(bdxx.gnpx.wxm);
                            win.label_gnpx_gd_text.Content = Convert.ToString(bdxx.gnpx.gd) + "m";
                            win.label_gnpx_sd_text.Content = Convert.ToString(bdxx.gnpx.sd / 10.0) + "m/s";
                            win.label_gnpx_fx_text.Content = Convert.ToString(bdxx.gnpx.fx) + "°";
                            win.label_gnpx_wxs_text.Content = Convert.ToString(bdxx.gnpx.wxs);
                            win.label_gnpx_zt_text.Content = bdxx.gnpx.zt == 1 ? "已定位" : "未定位";
                            win.label_gnpx_jdxs_text.Content = Convert.ToString(bdxx.gnpx.jdxs);
                            win.label_gnpx_gjwc_text.Content = Convert.ToString(bdxx.gnpx.gjwc / 10.0) + "m";
                            win.textbox_gnpx_zhzb.Text = Convert.ToString((((bdxx.gnpx.wxm / 60.0) + bdxx.gnpx.wm) / 60.0 + bdxx.gnpx.wf) / 60.0 + bdxx.gnpx.wd) + "," + Convert.ToString((((bdxx.gnpx.jxm / 60.0) + bdxx.gnpx.jm) / 60.0 + bdxx.gnpx.jf) / 60.0 + bdxx.gnpx.jd);
                        }));
                    }).Start();
                    bdxx.print_flag &= ~BD.PRINT_GNPX;

                }



            }

        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // this.s();Application
            // Window.
            myStartMain.app.Shutdown();
            System.Environment.Exit(0);
        }


        /// <summary>
        /// 字符串转换16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private string strToHexByte(string hexString)
        {
            byte[] bs = null;
            //System.Text.Encoding.Convert(System.Text.Encoding.Unicode,System.Text.Encoding.ASCII,bs);
            bs = System.Text.Encoding.ASCII.GetBytes(hexString.Trim());
            // hexString= new ASCIIEncoding().GetString(data)
            //hexString = hexString.Replace(" ", "");
            //if ((hexString.Length % 2) != 0) hexString += " ";
            //byte[] returnBytes = new byte[hexString.Length / 2];
            //for (int i = 0; i < returnBytes.Length; i++)
            //    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bs.Length; i++)
            {
                sb.AppendFormat("{0:x2}" + " ", bs[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 将一条十六进制字符串转换为ASCII
        /// </summary>
        /// <param name="hexstring">一条十六进制字符串</param>
        /// <returns>返回一条ASCII码</returns>
        public static string HexStringToASCII(string hexstring)
        {
            byte[] bt = HexStringToBinary(hexstring);
            string lin = "";
            for (int i = 0; i < bt.Length; i++)
            {
                lin = lin + bt[i] + " ";
            }


            string[] ss = lin.Trim().Split(new char[] { ' ' });
            char[] c = new char[ss.Length];
            int a;
            for (int i = 0; i < c.Length; i++)
            {
                a = Convert.ToInt32(ss[i]);
                c[i] = Convert.ToChar(a);
            }

            string b = new string(c);
            return b;
        }
        /**/
        /// <summary>
        /// 16进制字符串转换为二进制数组
        /// </summary>
        /// <param name="hexstring">用空格切割字符串</param>
        /// <returns>返回一个二进制字符串</returns>
        public static byte[] HexStringToBinary(string hexstring)
        {
            string[] tmpary = hexstring.Trim().Split(' ');
            byte[] buff = new byte[tmpary.Length];
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = Convert.ToByte(tmpary[i], 16);
            }
            return buff;
        }



        private void cb_txxx_hexordec_Click(object sender, RoutedEventArgs e)
        {
            if (this.cb_txxx_hexordec.IsChecked == true)
            {
                cb_txxx_hexordec.Content = "十六进制";
            }
            else
            {
                cb_txxx_hexordec.Content = "ASCII";
            }

            if (textbox_txxx_dwnr.Text != "")
            {
                if (this.cb_txxx_hexordec.IsChecked == true)
                {

                    textbox_txxx_dwnr.Text = strToHexByte(textbox_txxx_dwnr.Text);
                }
                else
                {

                    textbox_txxx_dwnr.Text = HexStringToASCII(textbox_txxx_dwnr.Text);
                }
            }

        }
    }
}
