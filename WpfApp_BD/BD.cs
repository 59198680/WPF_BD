/***********************Project Version1.0*************************
@项目名:北斗传输4.0(C#)

@File_name:BD.cs

@File_UpdateTime:2018年5月16日02:25:23

@File_Version:1.0a

@说明:实现基本功能

本程序基于.Net4.6.1编写的北斗短报文传输程序
界面使用WPF框架编写
在vs2017里运行通过

******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WpfApp_BD;
namespace BD_Protocol
{
    using UINT = System.UInt32;
    using UCHR = System.Byte;
    public partial class BD
    {
        public UINT print_flag = 0;
        public const UINT PRINT_DWXX = (1 << 0);
        public const UINT PRINT_TXXX = (1 << 1);
        public const UINT PRINT_ICXX = (1 << 2);
        public const UINT PRINT_ZJXX = (1 << 3);
        public const UINT PRINT_SJXX = (1 << 4);
        public const UINT PRINT_FKXX = (1 << 5);
        public const UINT PRINT_GNTX = (1 << 6);
        public const UINT PRINT_GNVX = (1 << 7);
        public const UINT PRINT_GNPX = (1 << 8);
        public const UINT PRINT_STATUS = (1 << 9);
        public const UINT PRINT_BLOCK = (1 << 10);
        public const int DATA_FIRM_LENTH = 6;
        public const int BD_PRINT_TIME_SEQ = 8;
        public const int BD_PRINT_GNPX_SEQ = 1;
        public const int BD_PRINT_GNTX_ZONE = 8;
        public const int BD_PRINT_GNTX_SEQ = 1;
        public const int BD_PRINT_GNVX_SEQ = 1;
        public const int RE_BUFFER_SIZE = 4095;
        //extern int (* myprint) (_In_z_ _Printf_format_string_ char const* const, ...);
        public UCHR SEND_BLOCKTIME = 0;
        //第八位为回复位，第七位为确认位,第六位预留,1-5位步骤
        public const UCHR STEP_NONE = 0;
        public const UCHR STEP_ICJC = 5;
        public const UCHR STEP_XJZJ = 2;
        public const UCHR STEP_SJSC = 0X0F;
        public const UCHR STEP_GNPS = 4;
        public const UCHR STEP_GNVS = 3;
        public const UCHR STEP_GNTS = 1;
        public const UCHR STEP_READY = 6;
        public const UCHR STATUS_BIT_STEP = 0x1f;

        public UCHR STATUS_BIT_ANSWER = (1 << 7);
        public UCHR STATUS_BIT_NO_ANSWER = (0 << 7);
        public const UCHR STATUS_BIT_CONFIRM = (1 << 6);
        public const UCHR STATUS_BIT_NO_CONFIRM = (0 << 6);
        public UCHR status = 0x80;
        public UCHR error_flag = 0;
        public UCHR BSGL = 0;//波束功率0-5位
        System.Timers.Timer timer_extract;
        System.Timers.Timer timer_check;
        public struct RE_BUFFER
        {
            public UINT wp;
            public UINT rp;
            public UCHR[] buffer;
        };

        public RE_BUFFER rebuff;
        MynewCOM mycom;

        public struct BD_GNTX
        {
            public SByte sqlx;//时区类型
            public UINT year;
            public UCHR month;
            public UCHR day;
            public UCHR hour;
            public UCHR minute;
            public UCHR second;
        };
        public const UCHR GNPX_LENTH = 0x20;

        public struct BD_GNPX //BDS&GPS定位信息
        {
            public UCHR jdfw;//经度范围
            public UCHR jd;//经度
            public UCHR jf;//经分
            public UCHR jm;//经秒
            public UCHR jxm;//经小秒
            public UCHR wdfw;//纬度范围
            public UCHR wd;//纬度
            public UCHR wf;//纬分
            public UCHR wm;//纬秒
            public UCHR wxm;//纬小秒
            public int gd;//高度
            public UINT sd;//速度
            public UINT fx; //方向
            public UCHR wxs;//卫星数
            public UCHR zt;//状态
            public UCHR jdxs;//精度系数
            public UINT gjwc;//估计误差
        };
        //const UCHR GNVX_LENTH 
        public struct WXXX
        {
            public UCHR wxbh;//卫星编号
            public UCHR wxyj;//卫星仰角
            public UINT fwj;//方位角
            public UCHR xzb;//信噪比
        };
        public struct BD_GNVX
        {
            public UCHR wxlb;//卫星类别
            public UCHR gps_wxgs;//gps卫星个数
            public WXXX[] gps_wxxx;//gps卫星信息
            public UCHR bds_wxgs;//bds卫星个数
            public WXXX[] bds_wxxx;//bds卫星信息
        };
        public struct BD_DWSQ  //定位申请
        {
            public const UINT lenth = 22;
            UCHR xxlb;//信息类别
            UINT gcsstxg;//高程数据和天线高
            UINT qyss;//气压数据
            UINT rzpd;//入站频度
        };

        public struct BD_TXSQ //通信申请
        {
            public const UINT lenth = 18;//不包含电文内容
            UCHR xxlb;//信息类别
            UCHR[] yhdz;//用户地址
            UINT dwcd;//电文长度
            UCHR sfyd;//是否应答
                      //UCHR dwnr[1680];//电文内容
        };

        public struct BD_CKSC //串口输出
        {
            public const UINT lenth = 12;
            UCHR cssl;//传输速率
        };

        public struct BD_ICJC //ic检测
        {
            public const UINT lenth = 12;
            UCHR zh;//帧号
        };

        public struct BD_XTZJ //系统自检
        {
            public const UINT lenth = 13;
            UINT zjpd;//自检频度
        };

        public struct BD_SJSC //时间输出
        {
            public const UINT lenth = 13;
            UINT scpd;//输出频度
        };

        public struct BD_BBDQ
        {
            public const UINT lenth = 11;
        };
        public const UCHR DWXX_LENTH = (11 + 1 + 3 + 4 + 4 + 4 + 2 + 2);
        public struct BD_DWXX
        {
            public UCHR xxlb;//信息类别
            public UCHR[] cxdz;//查询地址
            public UINT T;//定位时刻
            public UINT L;//用户位置的大地经度数据
            public UINT B;//用户位置的大地纬度数据
            public UINT H;//用户位置的大地高程数据
            public UINT S;//用户位置的高程异常值
        };


        public struct BD_TXXX
        {
            public UCHR xxlb;//信息类别
            public UCHR[] fxfdz;//发信方地址
            public UCHR fxsj_h;//发信时间（小时）
            public UCHR fxsj_m;//发信时间（分钟）
            public UINT dwcd;//电文长度
            public UCHR[] dwnr;//电文内容
            public UCHR crc;//CRC标志
        };

        public const UCHR ICXX_LENTH = (11 + 1 + 3 + 1 + 2 + 1 + 1 + 2);
        public struct BD_ICXX
        {
            public UCHR[] yhdz;//用户地址
            public UCHR zh;//帧号
            public UINT tbid;//通播ID
            public UCHR yhtz;//用户特征
            public UINT fwpd;//服务频度
            public UCHR txdj;//通信等级
            public UCHR jmbz;//加密标志
            public UINT xsyhzs;//下属用户总数
        };

        public const UCHR ZJXX_LENTH = (11 + 1 + 1 + 1 + 1 + 6);
        public struct BD_ZJXX //自检信息
        {
            public UCHR iczt;//IC卡状态
            public UCHR yjzt;//硬件状态
            public UCHR dcdl;//电池电量
            public UCHR rzzt;//入站状态
            public UCHR[] bsgl;//波束功率1-6
        };

        public const UCHR SJXX_LENTH = (11 + 2 + 1 + 1 + 1 + 1 + 1);
        public struct BD_SJXX //时间信息
        {
            public UINT year;
            public UCHR month;
            public UCHR day;
            public UCHR hour;
            public UCHR minute;
            public UCHR second;
        };

        struct BD_BBXX // 版本信息
        {

        };

        const UCHR FKXX_LENTH = (11 + 1 + 4);

        public struct BD_FKXX //反馈信息
        {
            public UCHR flbz;//反馈标志
            public UCHR[] fjxx;//附加信息
        };

        //BD_BBDQ bbdq;
        //public BD_BBXX bbxx;
        //BD_CKSC cksc;
        // BD_DWSQ dwsq;
        public BD_DWXX dwxx;
        public BD_FKXX fkxx;
        // BD_ICJC icjc;
        public BD_ICXX icxx;
        // BD_SJSC sjsc;
        public BD_SJXX sjxx;
        // BD_TXSQ txsq;
        public BD_TXXX txxx;
        // BD_XTZJ xtzj;
        public BD_ZJXX zjxx;
        public BD_GNTX gntx;
        public BD_GNPX gnpx;
        public BD_GNVX gnvx;
        public BD()
        {
            //bbxx = new BD_BBXX();
            dwxx = new BD_DWXX();
            fkxx = new BD_FKXX();
            icxx = new BD_ICXX();
            sjxx = new BD_SJXX();
            txxx = new BD_TXXX();
            zjxx = new BD_ZJXX();
            gntx = new BD_GNTX();
            gnpx = new BD_GNPX();
            gnvx = new BD_GNVX();
            fkxx.fjxx = new UCHR[4];
            rebuff = new RE_BUFFER();
            rebuff.buffer = new UCHR[RE_BUFFER_SIZE + 1];
            dwxx.cxdz = new UCHR[3];
            icxx.yhdz = new UCHR[3];
            txxx.fxfdz = new UCHR[3];
            zjxx.bsgl = new UCHR[6];

        }
        public void Init(MynewCOM com)
        {
            mycom = com;
            //结构体初始化0 TODO
            rebuff.rp = 0;
            rebuff.wp = 0;

            timer_extract = new System.Timers.Timer(100);//实例化Timer类，设置间隔时间为100毫秒；     
            timer_extract.Elapsed += new System.Timers.ElapsedEventHandler(Thread_Extract);//到达时间的时候执行事件；   
            timer_extract.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；     
            timer_extract.Enabled = true;//需要调用 timer.Start()或者timer.Enabled = true来启动它， timer.Start()的内部原理还是设置timer.Enabled = true;

            timer_check = new System.Timers.Timer(1000);//实例化Timer类，设置间隔时间为1000毫秒；     
            timer_check.Elapsed += new System.Timers.ElapsedEventHandler(Thread_Check);//到达时间的时候执行事件；   
            timer_check.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；     
            timer_check.Enabled = true;//需要调用 timer.Start()或者timer.Enabled = true来启动它， timer.Start()的内部原理还是设置timer.Enabled = true;


        }

        UINT check_status_count = 0;

        void Thread_Extract(object source, System.Timers.ElapsedEventArgs e)
        {
            Receive_Protocol();
        }
        void Thread_Check(object source, System.Timers.ElapsedEventArgs e)
        {
            Check_status();
            if (SEND_BLOCKTIME != 0)
            {
                --SEND_BLOCKTIME;
                print_flag |= PRINT_BLOCK;
            }
        }

        public void Check_status()
        {
            //UINT check_status_count = 0;
            if (check_status_count >= 10)
            {
                check_status_count = 0;
                MessageBox.Show("check_status_count=" + check_status_count, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                status &= 0;
                status |= STATUS_BIT_ANSWER;

            }

            if ((status & STATUS_BIT_ANSWER) != 0)//有回复
            {
                check_status_count = 0;
                switch (status & ~STATUS_BIT_ANSWER)//检查确认位
                {
                    case (STEP_NONE | STATUS_BIT_NO_CONFIRM):
                        status = (byte)((STEP_NONE + 1) | STATUS_BIT_ANSWER);//初始化
                        break;
                    case (STEP_ICJC | STATUS_BIT_CONFIRM):
                        status = (byte)((STEP_ICJC + 1) | STATUS_BIT_ANSWER);
                        break;
                    case (STEP_ICJC | STATUS_BIT_NO_CONFIRM):
                        status = (byte)((STEP_NONE + 1) | STATUS_BIT_ANSWER);
                        break;
                    case (STEP_GNPS | STATUS_BIT_CONFIRM):
                        status = (byte)((STEP_GNPS + 1) | STATUS_BIT_ANSWER);
                        break;
                    case (STEP_GNPS | STATUS_BIT_NO_CONFIRM):
                        status = (byte)((STEP_NONE + 1) | STATUS_BIT_ANSWER);
                        break;
                    case (STEP_GNVS | STATUS_BIT_CONFIRM):
                        status = (byte)((STEP_GNVS + 1) | STATUS_BIT_ANSWER);
                        break;
                    case (STEP_GNVS | STATUS_BIT_NO_CONFIRM):
                        status = (byte)((STEP_NONE + 1) | STATUS_BIT_ANSWER);
                        break;
                    case (STEP_GNTS | STATUS_BIT_CONFIRM):
                        status = (byte)((STEP_GNTS + 1) | STATUS_BIT_ANSWER);
                        break;
                    case (STEP_GNTS | STATUS_BIT_NO_CONFIRM):
                        status = (byte)((STEP_NONE + 1) | STATUS_BIT_ANSWER);
                        break;
                    case (STEP_XJZJ | STATUS_BIT_CONFIRM):
                        status = (byte)((STEP_XJZJ + 1) | STATUS_BIT_ANSWER);
                        break;
                    case (STEP_XJZJ | STATUS_BIT_NO_CONFIRM):
                        status = (byte)((STEP_NONE + 1) | STATUS_BIT_ANSWER);
                        break;
                    case (STEP_SJSC | STATUS_BIT_CONFIRM):
                        status = (byte)((STEP_SJSC + 1) | STATUS_BIT_ANSWER);//就绪
                        break;
                    case (STEP_SJSC | STATUS_BIT_NO_CONFIRM):
                        status = (byte)((STEP_NONE + 1) | STATUS_BIT_ANSWER);
                        break;
                    default:
                        byte[] aaa = new byte[255];
                        for (int i = 0; i < 10; ++i)
                        {
                            aaa[i] = (byte)(i + 0x30);
                        }
                        this.BD_send(ref aaa, (UINT)(10), this.icxx.yhdz);
                        break;
                }
            }
            if ((status & (STATUS_BIT_STEP | STATUS_BIT_ANSWER)) == (STEP_ICJC | STATUS_BIT_ANSWER))
            {
                status &= (byte)~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);
                print_flag |= PRINT_STATUS;
                ICJC_send();
            }
            else if ((status & (STATUS_BIT_STEP | STATUS_BIT_ANSWER)) == (STEP_XJZJ | STATUS_BIT_ANSWER))
            {
                status &= (byte)~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);
                XTZJ_send();
                print_flag |= PRINT_STATUS;
            }
            else if ((status & (STATUS_BIT_STEP | STATUS_BIT_ANSWER)) == (STEP_SJSC | STATUS_BIT_ANSWER))
            {
                status &= (byte)~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);
                SJSC_send();
                print_flag |= PRINT_STATUS;
            }
            else if ((status & (STATUS_BIT_STEP | STATUS_BIT_ANSWER)) == (STEP_GNPS | STATUS_BIT_ANSWER))
            {
                status &= (byte)~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);
                GNPS_send();
                print_flag |= PRINT_STATUS;
            }
            else if ((status & (STATUS_BIT_STEP | STATUS_BIT_ANSWER)) == (STEP_GNVS | STATUS_BIT_ANSWER))
            {
                status &= (byte)~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);
                GNVS_send();
                print_flag |= PRINT_STATUS;
            }
            else if ((status & (STATUS_BIT_STEP | STATUS_BIT_ANSWER)) == (STEP_GNTS | STATUS_BIT_ANSWER))
            {
                status &= (byte)~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);
                GNTS_send();
                print_flag |= PRINT_STATUS;
            }
            else if ((status & (STATUS_BIT_STEP | STATUS_BIT_ANSWER)) == (STEP_READY | STATUS_BIT_ANSWER))
            {
                print_flag |= PRINT_STATUS;
            }
            else
            {
                ++check_status_count;
            }
        }


        public void ICJC_send()
        {
            UCHR[] sendbuffer = System.Text.Encoding.Default.GetBytes("$ICJC0?0000?");
            sendbuffer[5] = 0;
            sendbuffer[6] = 12;
            sendbuffer[7] = 0;
            sendbuffer[8] = 0;
            sendbuffer[9] = 0;
            sendbuffer[10] = 0;
            sendbuffer[11] = this.Xor_checksum(ref sendbuffer, 11);
            mycom.Send(sendbuffer);
        }

        void GNPS_send()
        {
            UCHR[] sendbuffer = System.Text.Encoding.Default.GetBytes("$GNPS0?0000??");
            sendbuffer[5] = 0;
            sendbuffer[6] = 0X0D;
            sendbuffer[7] = this.icxx.yhdz[0];
            sendbuffer[8] = this.icxx.yhdz[1];
            sendbuffer[9] = this.icxx.yhdz[2];
            sendbuffer[10] = Convert.ToByte((UINT)BD_PRINT_GNPX_SEQ >> 8);
            sendbuffer[11] = Convert.ToByte((UINT)BD_PRINT_GNPX_SEQ & 0xff);
            sendbuffer[12] = this.Xor_checksum(ref sendbuffer, 12);
            mycom.Send(sendbuffer);
        }

        void GNVS_send()
        {
            UCHR[] sendbuffer = System.Text.Encoding.Default.GetBytes("$GNVS0?0000??");
            sendbuffer[5] = 0;
            sendbuffer[6] = 0X0D;
            sendbuffer[7] = this.icxx.yhdz[0];
            sendbuffer[8] = this.icxx.yhdz[1];
            sendbuffer[9] = this.icxx.yhdz[2];
            sendbuffer[10] = Convert.ToByte((UINT)BD_PRINT_GNVX_SEQ >> 8);
            sendbuffer[11] = Convert.ToByte(((UINT)BD_PRINT_GNVX_SEQ & 0xff));
            sendbuffer[12] = this.Xor_checksum(ref sendbuffer, 12);
            mycom.Send(sendbuffer);
        }

        void GNTS_send()
        {
            UCHR[] sendbuffer = System.Text.Encoding.Default.GetBytes("$GNTS0?0000???");
            sendbuffer[5] = 0;
            sendbuffer[6] = 0X0E;
            sendbuffer[7] = this.icxx.yhdz[0];
            sendbuffer[8] = this.icxx.yhdz[1];
            sendbuffer[9] = this.icxx.yhdz[2];
            sendbuffer[10] = BD_PRINT_GNTX_ZONE;
            sendbuffer[11] = Convert.ToByte((UINT)BD_PRINT_GNTX_SEQ >> 8);
            sendbuffer[12] = Convert.ToByte((UINT)BD_PRINT_GNTX_SEQ & 0xff);
            sendbuffer[13] = this.Xor_checksum(ref sendbuffer, 13);
            mycom.Send(sendbuffer);
        }

        void XTZJ_send()
        {
            UCHR[] sendbuffer = new UCHR[13];
            // UCHR []sendbuffer= Encoding.Default.GetBytes("$XTZJ0?00000?");
            sendbuffer[0] = (byte)'$';
            sendbuffer[1] = (byte)'X';
            sendbuffer[2] = (byte)'T';
            sendbuffer[3] = (byte)'Z';
            sendbuffer[4] = (byte)'J';
            sendbuffer[5] = 0;
            sendbuffer[6] = Convert.ToByte(BD_XTZJ.lenth);
            sendbuffer[7] = icxx.yhdz[0];
            sendbuffer[8] = icxx.yhdz[1];
            sendbuffer[9] = icxx.yhdz[2];
            sendbuffer[10] = Convert.ToByte((UINT)BD_PRINT_TIME_SEQ >> 8);
            sendbuffer[11] = Convert.ToByte((UINT)BD_PRINT_TIME_SEQ & 0xff);
            sendbuffer[12] = Xor_checksum(ref sendbuffer, 12);
            mycom.Send(sendbuffer);
        }

        void SJSC_send()
        {
            UCHR[] sendbuffer = Encoding.Default.GetBytes("$SJSC0?00000?");
            sendbuffer[5] = 0;
            sendbuffer[6] = 13;
            sendbuffer[7] = icxx.yhdz[0];
            sendbuffer[8] = icxx.yhdz[1];
            sendbuffer[9] = icxx.yhdz[2];
            sendbuffer[10] = Convert.ToByte((UINT)BD_PRINT_TIME_SEQ >> 8);
            sendbuffer[11] = Convert.ToByte((UINT)BD_PRINT_TIME_SEQ & 0xff);
            sendbuffer[12] = Xor_checksum(ref sendbuffer, 12);
            mycom.Send(sendbuffer);
        }


        UCHR[] Data_encapsulation(UCHR[] send_buffer, ref UCHR[] data, ref UINT length_data)
        {

            UINT i, length = 2 + length_data, crc = 0;
            for (i = 0; i < length_data; ++i)
                crc += data[i];
            crc += (length >> 8);
            crc += (length & 0xff);
            send_buffer[0] = 0x11;
            send_buffer[1] = (UCHR)(length >> 8);
            send_buffer[2] = (UCHR)(length & 0xff);
            send_buffer[length_data + DATA_FIRM_LENTH - 3] = Convert.ToByte((crc >> 8) & 0x0ff);
            send_buffer[length_data + DATA_FIRM_LENTH - 2] = Convert.ToByte(crc & 0x0ff);
            send_buffer[length_data + DATA_FIRM_LENTH - 1] = 0x16;
            for (i = 0; i < length_data; ++i)
                send_buffer[3 + i] = data[i];
            return send_buffer;
        }

        public void BD_send(ref UCHR[] buffer, UINT len, UCHR[] dis)
        {
            if (BSGL != 0 && error_flag == 0 && SEND_BLOCKTIME == 0 && len > 0)
            {
                UCHR[] send_buffer;
                send_buffer = new UCHR[len + DATA_FIRM_LENTH];
                TXSQ_send(Data_encapsulation(send_buffer, ref buffer, ref len), len + DATA_FIRM_LENTH, dis);
                //txsq_send(buffer, len, dis);
                //if (send_buffer != null)
                //    free(send_buffer);
            }
            else
            {
                //if (BSGL!=0)
                //    myprint("无波束  ");
                //if (error_flag)
                //    myprint("硬件错误  ");
                //if (SEND_BLOCKTIME)
                //    myprint("频度未到:%d秒  ", SEND_BLOCKTIME);
                //if (!len)
                //    myprint("发送长度为0  ");
                //myprint("\n");
            }
        }

        void TXSQ_send(UCHR[] buffer, UINT len, UCHR[] dis)
        {
            UINT lenth = BD_TXSQ.lenth + len, i = 0; ;
            // UCHR[] sendbuffer = Encoding.Default.GetBytes("$TXSQ0?00000?");//最多只能发送210字节,这张卡时628bit
            UCHR[] sendbuffer = new UCHR[lenth];
            sendbuffer[0] = (byte)'$';
            sendbuffer[1] = (byte)'T';
            sendbuffer[2] = (byte)'X';
            sendbuffer[3] = (byte)'S';
            sendbuffer[4] = (byte)'Q';
            sendbuffer[5] = Convert.ToByte(lenth >> 8);
            sendbuffer[6] = Convert.ToByte(lenth & 0xff);
            sendbuffer[7] = icxx.yhdz[0];
            sendbuffer[8] = icxx.yhdz[1];
            sendbuffer[9] = icxx.yhdz[2];
            sendbuffer[10] = 0B01000110;
            sendbuffer[11] = dis[0];
            sendbuffer[12] = dis[1];
            sendbuffer[13] = dis[2];
            sendbuffer[14] = Convert.ToByte((len * 8) >> 8);
            sendbuffer[15] = Convert.ToByte((len * 8) & 0xff);
            sendbuffer[16] = 0;
            for (; i < len; ++i)
                sendbuffer[17 + i] = buffer[i];
            sendbuffer[lenth - 1] = Xor_checksum(ref sendbuffer, lenth - 1);
            mycom.Send(sendbuffer);
        }

        void DWSQ_send()
        {
            //TODO
            //UINT lenth = bdxx.txsq.lenth + len, i = 0;;
            //UCHR sendbuffer[255] = "$TXSQ0?00000?";//最多只能发送210字节
            //sendbuffer[5] = lenth >> 8;
            //sendbuffer[6] = lenth & 0xff;
            //sendbuffer[7] = bdxx.icxx.yhdz[0];
            //sendbuffer[8] = bdxx.icxx.yhdz[1];
            //sendbuffer[9] = bdxx.icxx.yhdz[2];
            //sendbuffer[10] = 0B01000110;
            //sendbuffer[11] = dis[0];
            //sendbuffer[12] = dis[1];
            //sendbuffer[13] = dis[2];
            //sendbuffer[14] = 0;
            //sendbuffer[15] = (len * 8) >> 8;
            //sendbuffer[16] = (len * 8) & 0xff;
            //for (; i < len; ++i)
            //	sendbuffer[17 + i] = *(buffer + i);
            //sendbuffer[lenth - 1] = xor_checksum2(sendbuffer, lenth - 1);
            //mySerialPort.WriteData(sendbuffer, lenth);
        }
        /*
        接收用
        异或校验和算法
        */
        UCHR Xor_checksum(ref UCHR[] buf, UINT location, UINT len)
        {
            UINT i, ii;
            UCHR checksum = 0;

            for (i = 0; i < len; ++i)
            {
                ii = (location + i) & RE_BUFFER_SIZE;
                checksum ^= buf[ii];
            }

            return checksum;
        }

        /*
        发送用
        异或校验和算法
        */
        UCHR Xor_checksum(ref UCHR[] buf, UINT len)
        {
            UINT i;
            UCHR checksum = 0;

            for (i = 0; i < len; ++i)
            {
                checksum ^= buf[i];
            }

            return checksum;
        }


        void Handle_ZJXX()
        {
            if ((zjxx.iczt != 0) || (zjxx.yjzt != 0) || (zjxx.rzzt != 0x02))
                error_flag = 1;
            else
                error_flag = 0;
            for (int i = 0; i < 6; ++i)
            {
                if (zjxx.bsgl[i] != 0)
                {
                    BSGL |= Convert.ToByte((1 << i) & 0xff);
                }
                else
                {
                    BSGL &= Convert.ToByte((~(1 << i)) & 0xff);
                }
            }

        }

        void Handle_FXXX()
        {
            if (fkxx.flbz == 4)//发射频度未到
            {
                SEND_BLOCKTIME = fkxx.fjxx[3];
            }
            if (fkxx.flbz == 1)
            {
                SEND_BLOCKTIME = 60;
            }
        }


        public UINT UCHRtoUINT(UCHR a, UCHR b)
        {
            return (UINT)a * 256 + (UINT)b;
        }

        /*
        对于有符号参数，第 1 位符号位统一规定为“0”表示“+”，
        “1”表示“-”，其后位数为参数值，用原码表示。
        */
        public int UCHRtoINT(UCHR a, UCHR b)
        {
            int result = 0, flag = 0;
            if ((a & 0x80) != 0)
            {
                flag = -1;
            }
            else
            {
                flag = 1;
            }
            for (int i = 0; i < 7; ++i)
            {
                if ((a & (1 << i)) != 0)
                    result |= (1 << (i + 8));
            }
            for (int i = 0; i < 8; ++i)
            {
                if ((b & (1 << i)) != 0)
                    result |= (1 << i);
            }
            return result * flag;
        }

        bool Check_overflow(UINT value)
        {
            if (rebuff.rp <= rebuff.wp)//说明写指针未回溯
            {
                if (rebuff.rp + value <= rebuff.wp)
                    return true;
                else
                    return false;
            }
            else
            {
                if (rebuff.rp + value <= (rebuff.wp + RE_BUFFER_SIZE))
                    return true;
                else
                    return false;
            }
        }

        void Analysis_data(ref UCHR[] fxfdz, ref UCHR h, ref UCHR m, ref UCHR[] buffer, ref UINT package_length)
        {
            UINT crc = 0, crc1 = 0;
            UINT length, length1;

            if (buffer[package_length - 1] != 0x16)
            {
                return;
            }
            else if (buffer[0] == 0x11)
            {
                length1 = length = ((UINT)buffer[1] << 8) + (UINT)buffer[2];
                crc = Convert.ToUInt32(((((UINT)buffer[length + 1]) << 8) & 0xff00) + (buffer[length + 2] & 0x00ff));
                if (length1 < package_length)
                {
                    for (; length1 > 0; length1--)
                    {
                        crc1 += buffer[length1];
                    }
                    if (crc == crc1)
                    {

                        UINT _LENGTH = length - 2;
                        UCHR[] exdata = new UCHR[_LENGTH];
                        //memcpy(exdata, buffer[3], length - 2);
                        System.Buffer.BlockCopy(buffer, 3, exdata, 0, Convert.ToInt32(_LENGTH));
                        DATA_Handler(ref fxfdz, ref h, ref m, ref exdata, ref _LENGTH);
                        //free(exdata);
                    }

                }
            }
        }

        void DATA_Handler(ref UCHR[] fxfdz, ref UCHR h, ref UCHR m, ref UCHR[] data, ref UINT lenth)
        {
            //TODO
            //for (UINT i = 0; i<lenth; ++i)
            //    printf("%c", * data + i);
            //    printf("\n");
        }
    }


}
