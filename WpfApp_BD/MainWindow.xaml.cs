using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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
using System.Windows.Threading;
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
        BD bdxx;
        DispatcherTimer autoTick = new DispatcherTimer();//定时发送
        public MainWindow(BD b)
        {
            bdxx = b;
            InitializeComponent();

        }
        System.Timers.Timer timer_extract;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            autoTick.Tick += new EventHandler(Seamphore_thread);//定时中断
            autoTick.Interval = TimeSpan.FromMilliseconds(200);//设置自动间隔
                                                               //autoTick.Start();//开启自动
            timer_extract = new System.Timers.Timer(500);//实例化Timer类，设置间隔时间为100毫秒；     
            timer_extract.Elapsed += new System.Timers.ElapsedEventHandler(Seamphore_thread);//到达时间的时候执行事件；   
            timer_extract.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；     
            timer_extract.Enabled = true;//需要调用 timer.Start()或者timer.Enabled = true来启动它， timer.Start()的内部原理还是设置timer.Enabled = true;
        }

        public static class DispatcherHelper
        {
            [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public static void DoEvents()
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
                try { Dispatcher.PushFrame(frame); }
                catch (InvalidOperationException) { }
            }
            private static object ExitFrames(object frame)
            {
                ((DispatcherFrame)frame).Continue = false;
                return null;
            }
        }
        void UIAction(Action action)//在主线程外激活线程方法
        {
            System.Threading.SynchronizationContext.SetSynchronizationContext(new System.Windows.Threading.DispatcherSynchronizationContext(App.Current.Dispatcher));
            System.Threading.SynchronizationContext.Current.Post(_ => action(), null);
        }
        static int tem = 0;
        public void Seamphore_thread(object sender, EventArgs e)
        {
            // BD bdxx = ((Box)obj1).bdxx;
            // MainWindow win = ((Box)obj1).win;
            //tem++;
            //if (tem % 2 == 0)
            //    MessageBox.Show((tem / 2).ToString());
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
                UIAction(() =>
                {
                    Label_Init_text.Content = str;
                });
                //new Thread(() =>
                //{
                //    Dispatcher.BeginInvoke(new Action(() =>
                //    {

                //        Label_Init_text.Content = str;
                //    }));
                //}).Start();
                bdxx.print_flag &= ~BD.PRINT_STATUS;
            }
            if ((bdxx.print_flag & BD.PRINT_DWXX) != 0)
            {

            }
            if ((bdxx.print_flag & BD.PRINT_BLOCK) != 0)
            {
                //new Thread(() =>
                //{
                //    Dispatcher.Invoke(new Action(() =>
                //    {

                //        label_djs_text.Content = Convert.ToString(bdxx.SEND_BLOCKTIME) + "s";
                //    }));
                //}).Start();
                UIAction(() =>
                {
                    label_djs_text.Content = Convert.ToString(bdxx.SEND_BLOCKTIME) + "s";
                });
                bdxx.print_flag &= ~BD.PRINT_BLOCK;

            }
            if ((bdxx.print_flag & BD.PRINT_TXXX) != 0)
            {
                //new Thread(() =>
                //{
                //    Dispatcher.BeginInvoke(new Action(() =>
                //    {

                //        label_txxx_xxlb_text.Content = Convert.ToString(bdxx.txxx.xxlb, 2);
                //        label_txxx_fxfd_text.Content = Convert.ToString(bdxx.txxx.fxfdz[0] * 256 * 256 + bdxx.txxx.fxfdz[1] * 256 + bdxx.txxx.fxfdz[2]);
                //        label_txxx_fxsj_text.Content = Convert.ToString(bdxx.txxx.fxsj_h) + "时" + Convert.ToString(bdxx.txxx.fxsj_m) + "分";
                //        label_txxx_dwcd_text.Content = Convert.ToString(bdxx.txxx.dwcd / 8.0) + "bytes(" + Convert.ToString(bdxx.txxx.dwcd) + "bits)";
                //        label_txxx_crc_text.Content = Convert.ToString(bdxx.txxx.crc);
                //        label_txxx_lasttime_text.Content = Convert.ToString(bdxx.gntx.year) + "年" + Convert.ToString(bdxx.gntx.month) + "月" + Convert.ToString(bdxx.gntx.day) + "日" + Convert.ToString(bdxx.gntx.hour) + ":" + Convert.ToString(bdxx.gntx.minute) + ":" + Convert.ToString(bdxx.gntx.second);
                //        if (cb_txxx_hexordec.IsChecked == true)
                //        {
                //            StringBuilder sb = new StringBuilder();
                //            for (int i = 0; i < bdxx.txxx.dwnr.Length; i++)
                //            {
                //                sb.AppendFormat("{0:x2}" + " ", bdxx.txxx.dwnr[i]);
                //            }
                //            textbox_txxx_dwnr.Text = sb.ToString().ToUpper();
                //        }
                //        else
                //        {
                //            textbox_txxx_dwnr.Text = new ASCIIEncoding().GetString(bdxx.txxx.dwnr);
                //        }
                //            // textbox_txxx_dwnr.Text = Convert.ToString();

                //        }));
                //}).Start();
                UIAction(() =>
                {
                    label_txxx_xxlb_text.Content = Convert.ToString(bdxx.txxx.xxlb, 2);
                    label_txxx_fxfd_text.Content = Convert.ToString(bdxx.txxx.fxfdz[0] * 256 * 256 + bdxx.txxx.fxfdz[1] * 256 + bdxx.txxx.fxfdz[2]);
                    label_txxx_fxsj_text.Content = Convert.ToString(bdxx.txxx.fxsj_h) + "时" + Convert.ToString(bdxx.txxx.fxsj_m) + "分";
                    label_txxx_dwcd_text.Content = Convert.ToString(bdxx.txxx.dwcd / 8.0) + "bytes(" + Convert.ToString(bdxx.txxx.dwcd) + "bits)";
                    label_txxx_crc_text.Content = Convert.ToString(bdxx.txxx.crc);
                    label_txxx_lasttime_text.Content = Convert.ToString(bdxx.gntx.year) + "年" + Convert.ToString(bdxx.gntx.month) + "月" + Convert.ToString(bdxx.gntx.day) + "日" + Convert.ToString(bdxx.gntx.hour) + String.Format(":{0:x2}", bdxx.gntx.minute) + String.Format(":{0:x2}", bdxx.gntx.second);
                    if (cb_txxx_hexordec.IsChecked == true)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < bdxx.txxx.dwnr.Length; i++)
                        {
                            sb.AppendFormat("{0:x2}" + " ", bdxx.txxx.dwnr[i]);
                        }
                        textbox_txxx_dwnr.Text = sb.ToString().ToUpper();
                    }
                    else
                    {
                        textbox_txxx_dwnr.Text = new ASCIIEncoding().GetString(bdxx.txxx.dwnr);
                    }
                });
                bdxx.print_flag &= ~BD.PRINT_TXXX;
            }
            if ((bdxx.print_flag & BD.PRINT_ICXX) != 0)
            {
                //new Thread(() =>
                //{
                //    Dispatcher.BeginInvoke(new Action(() =>
                //    {
                //        label_icxx_yhid_text.Content = Convert.ToString(bdxx.icxx.yhdz[0] * 256 * 256 + bdxx.icxx.yhdz[1] * 256 + bdxx.icxx.yhdz[2]);
                //        label_icxx_zh_text.Content = Convert.ToString(bdxx.icxx.zh);
                //        label_icxx_tbid_text.Content = Convert.ToString(bdxx.icxx.tbid);
                //        label_icxx_yhtz_text.Content = Convert.ToString(bdxx.icxx.yhtz);
                //        label_icxx_fwpd_text.Content = Convert.ToString(bdxx.icxx.fwpd);
                //        label_icxx_txdj_text.Content = Convert.ToString(bdxx.icxx.txdj);
                //        label_icxx_jmbz_text.Content = Convert.ToString(bdxx.icxx.jmbz);
                //        label_icxx_xsyhs_text.Content = Convert.ToString(bdxx.icxx.xsyhzs);
                //    }));
                //}).Start();
                UIAction(() =>
                {
                    label_icxx_yhid_text.Content = Convert.ToString(bdxx.icxx.yhdz[0] * 256 * 256 + bdxx.icxx.yhdz[1] * 256 + bdxx.icxx.yhdz[2]);
                    label_icxx_zh_text.Content = Convert.ToString(bdxx.icxx.zh);
                    label_icxx_tbid_text.Content = Convert.ToString(bdxx.icxx.tbid);
                    label_icxx_yhtz_text.Content = Convert.ToString(bdxx.icxx.yhtz);
                    label_icxx_fwpd_text.Content = Convert.ToString(bdxx.icxx.fwpd);
                    label_icxx_txdj_text.Content = Convert.ToString(bdxx.icxx.txdj);
                    label_icxx_jmbz_text.Content = Convert.ToString(bdxx.icxx.jmbz);
                    label_icxx_xsyhs_text.Content = Convert.ToString(bdxx.icxx.xsyhzs);
                });
                bdxx.print_flag &= ~BD.PRINT_ICXX;
            }
            if ((bdxx.print_flag & BD.PRINT_ZJXX) != 0)
            {
                //new Thread(() =>
                //{
                //    Dispatcher.BeginInvoke(new Action(() =>
                //    {
                //        label_zjxx_iczt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.iczt, 16);
                //        label_zjxx_yjzt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.yjzt, 16);
                //        label_zjxx_dczt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.dcdl, 16);
                //        label_zjxx_rzzt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.rzzt, 16);
                //        label_zjxx_bsgl1_text.Content = Convert.ToString(bdxx.zjxx.bsgl[0]);
                //        label_zjxx_bsgl2_text.Content = Convert.ToString(bdxx.zjxx.bsgl[1]);
                //        label_zjxx_bsgl3_text.Content = Convert.ToString(bdxx.zjxx.bsgl[2]);
                //        label_zjxx_bsgl4_text.Content = Convert.ToString(bdxx.zjxx.bsgl[3]);
                //        label_zjxx_bsgl5_text.Content = Convert.ToString(bdxx.zjxx.bsgl[4]);
                //        label_zjxx_bsgl6_text.Content = Convert.ToString(bdxx.zjxx.bsgl[5]);
                //    }));
                //}).Start();
                UIAction(() =>
                {
                    label_zjxx_iczt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.iczt, 16);
                    label_zjxx_yjzt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.yjzt, 16);
                    label_zjxx_dczt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.dcdl, 16);
                    label_zjxx_rzzt_text.Content = "0x" + Convert.ToString(bdxx.zjxx.rzzt, 16);
                    label_zjxx_bsgl1_text.Content = Convert.ToString(bdxx.zjxx.bsgl[0]);
                    label_zjxx_bsgl2_text.Content = Convert.ToString(bdxx.zjxx.bsgl[1]);
                    label_zjxx_bsgl3_text.Content = Convert.ToString(bdxx.zjxx.bsgl[2]);
                    label_zjxx_bsgl4_text.Content = Convert.ToString(bdxx.zjxx.bsgl[3]);
                    label_zjxx_bsgl5_text.Content = Convert.ToString(bdxx.zjxx.bsgl[4]);
                    label_zjxx_bsgl6_text.Content = Convert.ToString(bdxx.zjxx.bsgl[5]);
                });
                bdxx.print_flag &= ~BD.PRINT_ZJXX;
            }
            if ((bdxx.print_flag & BD.PRINT_SJXX) != 0)
            {

            }
            if ((bdxx.print_flag & BD.PRINT_FKXX) != 0)
            {
                string str = "";
                if (bdxx.fkxx.flbz == 0)
                {
                    str = "成功,指令:" + (char)(bdxx.fkxx.fjxx[0]) + (char)(bdxx.fkxx.fjxx[1]) + (char)(bdxx.fkxx.fjxx[2]) + (char)(bdxx.fkxx.fjxx[3]);
                    // bdxx.SEND_BLOCKTIME = 60;
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
                //new Thread(() =>
                //{
                //    Dispatcher.BeginInvoke(new Action(() =>
                //    {

                //        listbox_fkxx.Items.Add(new ListBoxItem().Content = str);
                //    }));
                //}).Start();
                UIAction(() =>
                {
                    listbox_fkxx.Items.Insert(0, new ListBoxItem().Content = str);
                });
                bdxx.print_flag &= ~BD.PRINT_FKXX;
            }
            if ((bdxx.print_flag & BD.PRINT_GNTX) != 0)
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
                //new Thread(() =>
                //{
                //    Dispatcher.Invoke(new Action(() =>
                //    {


                //        label_gntx_sq_text.Content = str + Convert.ToString(sq) + "区";
                //        label_gntx_sj_text.Content = Convert.ToString(bdxx.gntx.year) + "年" + Convert.ToString(bdxx.gntx.month) + "月" + Convert.ToString(bdxx.gntx.day) + "日" + Convert.ToString(bdxx.gntx.hour) + ":" + Convert.ToString(bdxx.gntx.minute) + ":" + Convert.ToString(bdxx.gntx.second);
                //    }));
                //}).Start();
                UIAction(() =>
                {
                    label_gntx_sq_text.Content = str + Convert.ToString(sq) + "区";
                    label_gntx_sj_text.Content = Convert.ToString(bdxx.gntx.year) + "年" + Convert.ToString(bdxx.gntx.month) + "月" + Convert.ToString(bdxx.gntx.day) + "日" + Convert.ToString(bdxx.gntx.hour) + String.Format(":{0:x2}", bdxx.gntx.minute) + String.Format(":{0:x2}", bdxx.gntx.second);
                });
                bdxx.print_flag &= ~BD.PRINT_GNTX;
            }
            if ((bdxx.print_flag & BD.PRINT_GNVX) != 0)
            {
                //new Thread(() =>
                //{
                //    Dispatcher.BeginInvoke(new Action(() =>
                //    {
                //        label_gnvx_gwxgs_text.Content = Convert.ToString(bdxx.gnvx.gps_wxgs);
                //        label_gnvx_bwxgs_text.Content = Convert.ToString(bdxx.gnvx.bds_wxgs);
                //        listbox_gnvx_bwxxx.Items.Clear();
                //        listbox_gnvx_bwxxx.Items.Add(new ListBoxItem().Content = "(卫星编号)(卫星仰角)(方位角)(信噪比)");
                //        listbox_gnvx_gwxxx.Items.Clear();
                //        listbox_gnvx_gwxxx.Items.Add(new ListBoxItem().Content = "(卫星编号)(卫星仰角)(方位角)(信噪比)");
                //        for (int i = 0; i < bdxx.gnvx.bds_wxgs; ++i)
                //            listbox_gnvx_bwxxx.Items.Add(new ListBoxItem().Content = "(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].wxbh) + ")(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].wxyj) + "°)(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].fwj) + "°)(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].xzb) + "db)");
                //        for (int i = 0; i < bdxx.gnvx.gps_wxgs; ++i)
                //            listbox_gnvx_gwxxx.Items.Add(new ListBoxItem().Content = "(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].wxbh) + ")(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].wxyj) + "°)(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].fwj) + "°)(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].xzb) + "db)");
                //    }));
                //}).Start();
                UIAction(() =>
                {
                    label_gnvx_gwxgs_text.Content = Convert.ToString(bdxx.gnvx.gps_wxgs);
                    label_gnvx_bwxgs_text.Content = Convert.ToString(bdxx.gnvx.bds_wxgs);
                    listbox_gnvx_bwxxx.Items.Clear();
                    listbox_gnvx_bwxxx.Items.Add(new ListBoxItem().Content = "(卫星编号)(卫星仰角)(方位角)(信噪比)");
                    listbox_gnvx_gwxxx.Items.Clear();
                    listbox_gnvx_gwxxx.Items.Add(new ListBoxItem().Content = "(卫星编号)(卫星仰角)(方位角)(信噪比)");
                    for (int i = 0; i < bdxx.gnvx.bds_wxgs; ++i)
                        listbox_gnvx_bwxxx.Items.Add(new ListBoxItem().Content = "(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].wxbh) + ")(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].wxyj) + "°)(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].fwj) + "°)(" + Convert.ToString(bdxx.gnvx.bds_wxxx[i].xzb) + "db)");
                    for (int i = 0; i < bdxx.gnvx.gps_wxgs; ++i)
                        listbox_gnvx_gwxxx.Items.Add(new ListBoxItem().Content = "(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].wxbh) + ")(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].wxyj) + "°)(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].fwj) + "°)(" + Convert.ToString(bdxx.gnvx.gps_wxxx[i].xzb) + "db)");
                });
                bdxx.print_flag &= ~BD.PRINT_GNVX;
            }
            if ((bdxx.print_flag & BD.PRINT_GNPX) != 0)
            {
                // float wd, jd;
                // wd= (float)bdxx.gnpx.wxm / 60
                //new Thread(() =>
                //{
                //    Dispatcher.BeginInvoke(new Action(() =>
                //    {
                //        label_gnpx_jdfw_text.Content = (char)bdxx.gnpx.jdfw;
                //        label_gnpx_jd_text.Content = Convert.ToString(bdxx.gnpx.jd);
                //        label_gnpx_jf_text.Content = Convert.ToString(bdxx.gnpx.jf);
                //        label_gnpx_jm_text.Content = Convert.ToString(bdxx.gnpx.jm);
                //        label_gnpx_jxm_text.Content = Convert.ToString(bdxx.gnpx.jxm);
                //        label_gnpx_wdfw_text.Content = (char)bdxx.gnpx.wdfw;
                //        label_gnpx_wd_text.Content = Convert.ToString(bdxx.gnpx.wd);
                //        label_gnpx_wf_text.Content = Convert.ToString(bdxx.gnpx.wf);
                //        label_gnpx_wm_text.Content = Convert.ToString(bdxx.gnpx.wm);
                //        label_gnpx_wxm_text.Content = Convert.ToString(bdxx.gnpx.wxm);
                //        label_gnpx_gd_text.Content = Convert.ToString(bdxx.gnpx.gd) + "m";
                //        label_gnpx_sd_text.Content = Convert.ToString(bdxx.gnpx.sd / 10.0) + "m/s";
                //        label_gnpx_fx_text.Content = Convert.ToString(bdxx.gnpx.fx) + "°";
                //        label_gnpx_wxs_text.Content = Convert.ToString(bdxx.gnpx.wxs);
                //        label_gnpx_zt_text.Content = bdxx.gnpx.zt == 1 ? "已定位" : "未定位";
                //        label_gnpx_jdxs_text.Content = Convert.ToString(bdxx.gnpx.jdxs);
                //        label_gnpx_gjwc_text.Content = Convert.ToString(bdxx.gnpx.gjwc / 10.0) + "m";
                //        textbox_gnpx_zhzb.Text = Convert.ToString((((bdxx.gnpx.wxm / 60.0) + bdxx.gnpx.wm) / 60.0 + bdxx.gnpx.wf) / 60.0 + bdxx.gnpx.wd) + "," + Convert.ToString((((bdxx.gnpx.jxm / 60.0) + bdxx.gnpx.jm) / 60.0 + bdxx.gnpx.jf) / 60.0 + bdxx.gnpx.jd);
                //    }));
                //}).Start();
                UIAction(() =>
                {
                    label_gnpx_jdfw_text.Content = (char)bdxx.gnpx.jdfw;
                    label_gnpx_jd_text.Content = Convert.ToString(bdxx.gnpx.jd);
                    label_gnpx_jf_text.Content = Convert.ToString(bdxx.gnpx.jf);
                    label_gnpx_jm_text.Content = Convert.ToString(bdxx.gnpx.jm);
                    label_gnpx_jxm_text.Content = Convert.ToString(bdxx.gnpx.jxm);
                    label_gnpx_wdfw_text.Content = (char)bdxx.gnpx.wdfw;
                    label_gnpx_wd_text.Content = Convert.ToString(bdxx.gnpx.wd);
                    label_gnpx_wf_text.Content = Convert.ToString(bdxx.gnpx.wf);
                    label_gnpx_wm_text.Content = Convert.ToString(bdxx.gnpx.wm);
                    label_gnpx_wxm_text.Content = Convert.ToString(bdxx.gnpx.wxm);
                    label_gnpx_gd_text.Content = Convert.ToString(bdxx.gnpx.gd) + "m";
                    label_gnpx_sd_text.Content = Convert.ToString(bdxx.gnpx.sd / 10.0) + "m/s";
                    label_gnpx_fx_text.Content = Convert.ToString(bdxx.gnpx.fx) + "°";
                    label_gnpx_wxs_text.Content = Convert.ToString(bdxx.gnpx.wxs);
                    label_gnpx_zt_text.Content = bdxx.gnpx.zt == 1 ? "已定位" : "未定位";
                    label_gnpx_jdxs_text.Content = Convert.ToString(bdxx.gnpx.jdxs);
                    label_gnpx_gjwc_text.Content = Convert.ToString(bdxx.gnpx.gjwc / 10.0) + "m";
                    textbox_gnpx_zhzb.Text = Convert.ToString((((bdxx.gnpx.wxm / 60.0) + bdxx.gnpx.wm) / 60.0 + bdxx.gnpx.wf) / 60.0 + bdxx.gnpx.wd) + "," + Convert.ToString((((bdxx.gnpx.jxm / 60.0) + bdxx.gnpx.jm) / 60.0 + bdxx.gnpx.jf) / 60.0 + bdxx.gnpx.jd);
                });
                bdxx.print_flag &= ~BD.PRINT_GNPX;

            }

            //Thread.Sleep(100);
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // this.s();Application
            // Window.
            // myStartMain.app.Shutdown();
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

            if (textbox_txxx_dwnr.Text != string.Empty)
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

        private void btn_setcombaud_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
