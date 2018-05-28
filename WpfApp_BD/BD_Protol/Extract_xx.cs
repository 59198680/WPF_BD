/***********************Project Version1.5*************************
@项目名:北斗传输4.0(C#)
@File:Extract_xx.cs
@File_Version:1.1a
@Author:lys
@QQ:591986780
@UpdateTime:2018年5月21日03:35:28

@说明:实现接收数据的解析

本程序基于.Net4.6.1编写的北斗短报文传输程序
界面使用WPF框架编写
在vs2017里运行通过

******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD_Protocol
{
    using UINT = System.UInt32;
    using UCHR = System.Byte;
    partial class BD
    {
        void Extract_DWXX(UCHR[] buf, UINT i)
        {
            dwxx.xxlb = buf[((i + 10) & RE_BUFFER_SIZE)];
            dwxx.cxdz[0] = buf[((i + 11) & RE_BUFFER_SIZE)];
            dwxx.cxdz[1] = buf[((i + 12) & RE_BUFFER_SIZE)];
            dwxx.cxdz[2] = buf[((i + 13) & RE_BUFFER_SIZE)];
            dwxx.T = buf[(((i + 14) & RE_BUFFER_SIZE) << 24) & 0xff000000];
            dwxx.T += buf[(((i + 15) & RE_BUFFER_SIZE) << 16) & 0xff0000];
            dwxx.T += buf[(((i + 16) & RE_BUFFER_SIZE) << 8) & 0xff00];
            dwxx.T += buf[((i + 17) & RE_BUFFER_SIZE) & 0xff];
            dwxx.L = buf[(((i + 18) & RE_BUFFER_SIZE) << 24) & 0xff000000];
            dwxx.L += buf[(((i + 19) & RE_BUFFER_SIZE) << 16) & 0xff0000];
            dwxx.L += buf[(((i + 20) & RE_BUFFER_SIZE) << 8) & 0xff00];
            dwxx.L += buf[((i + 21) & RE_BUFFER_SIZE) & 0xff];
            dwxx.B = buf[(((i + 22) & RE_BUFFER_SIZE) << 24) & 0xff000000];
            dwxx.B += buf[(((i + 23) & RE_BUFFER_SIZE) << 16) & 0xff0000];
            dwxx.B += buf[(((i + 24) & RE_BUFFER_SIZE) << 8) & 0xff00];
            dwxx.B += buf[((i + 25) & RE_BUFFER_SIZE) & 0x000000ff];
            dwxx.H = UCHRtoUINT(buf[((i + 26) & RE_BUFFER_SIZE)], buf[((i + 27) & RE_BUFFER_SIZE)]);
            dwxx.S = UCHRtoUINT(buf[((i + 28) & RE_BUFFER_SIZE)], buf[((i + 29) & RE_BUFFER_SIZE)]);
            //print_dwxx();
            print_flag |= PRINT_DWXX;
        }

        void Extract_TXXX(UCHR[] buf, UINT i)
        {
            txxx.xxlb = buf[((i + 10) & RE_BUFFER_SIZE)];
            txxx.fxfdz[0] = buf[((i + 11) & RE_BUFFER_SIZE)];
            txxx.fxfdz[1] = buf[((i + 12) & RE_BUFFER_SIZE)];
            txxx.fxfdz[2] = buf[((i + 13) & RE_BUFFER_SIZE)];
            txxx.fxsj_h = buf[((i + 14) & RE_BUFFER_SIZE)];
            txxx.fxsj_m = buf[((i + 15) & RE_BUFFER_SIZE)];
            txxx.dwcd = UCHRtoUINT(buf[((i + 16) & RE_BUFFER_SIZE)], buf[((i + 17) & RE_BUFFER_SIZE)]);
            txxx.dwnr = new UCHR[txxx.dwcd / 8];
            for (UINT ii = 0; ii * 8 < txxx.dwcd; ++ii)
            {
                txxx.dwnr[ii] = buf[((i + 18 + ii) & RE_BUFFER_SIZE)];
            }
            UINT temp = txxx.dwcd / 8;
            
            Analysis_data(ref txxx.fxfdz, ref txxx.fxsj_h, ref txxx.fxsj_m, ref txxx.dwnr, ref temp);
            //注意有长度无内容的情况 TODO
            //注意长度不是字节整数
            if (txxx.dwcd % 8 == 0)
                txxx.crc = buf[((i + 18 + txxx.dwcd / 8) & RE_BUFFER_SIZE)];
            else
                txxx.crc = buf[((i + 18 + txxx.dwcd / 8 + 1) & RE_BUFFER_SIZE)];
            //print_txxx();
            print_flag |= PRINT_TXXX;

        }

        void Extract_ICXX(UCHR[] buf, UINT i)
        {
            icxx.yhdz[0] = buf[((i + 7) & RE_BUFFER_SIZE)];
            icxx.yhdz[1] = buf[((i + 8) & RE_BUFFER_SIZE)];
            icxx.yhdz[2] = buf[((i + 9) & RE_BUFFER_SIZE)];
            icxx.zh = buf[((i + 10) & RE_BUFFER_SIZE)];
            icxx.tbid = UCHRtoUINT(buf[((i + 11) & RE_BUFFER_SIZE)], Convert.ToByte(UCHRtoUINT(buf[((i + 12) & RE_BUFFER_SIZE)], buf[((i + 13) & RE_BUFFER_SIZE)])));
            icxx.yhtz = buf[((i + 14) & RE_BUFFER_SIZE)];
            icxx.fwpd = UCHRtoUINT(buf[((i + 15) & RE_BUFFER_SIZE)], buf[((i + 16) & RE_BUFFER_SIZE)]);
            icxx.txdj = buf[((i + 17) & RE_BUFFER_SIZE)];
            icxx.jmbz = buf[((i + 18) & RE_BUFFER_SIZE)];
            icxx.xsyhzs = UCHRtoUINT(buf[((i + 19) & RE_BUFFER_SIZE)], buf[((i + 20) & RE_BUFFER_SIZE)]);
            if ((status & ~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM)) == STEP_ICJC)
                status |= Convert.ToByte(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);
            // print_icxx();
            print_flag |= PRINT_ICXX;
        }

        void Extract_ZJXX(UCHR[] buf, UINT i)
        {
            zjxx.iczt = buf[((i + 10) & RE_BUFFER_SIZE)];
            zjxx.yjzt = buf[((i + 11) & RE_BUFFER_SIZE)];
            zjxx.dcdl = buf[((i + 12) & RE_BUFFER_SIZE)];
            zjxx.rzzt = buf[((i + 13) & RE_BUFFER_SIZE)];
            for (int ii = 0; ii < 6; ++ii)
            {
                zjxx.bsgl[ii] = buf[((i + 14 + ii) & RE_BUFFER_SIZE)];
            }
            Handle_ZJXX();
            // print_zjxx();
            print_flag |= PRINT_ZJXX;
            if ((status & ~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM)) == STEP_XJZJ)
            {
                status |= Convert.ToByte(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);

            }
        }

        void Extract_SJXX(UCHR[] buf, UINT i)
        {
            sjxx.year = UCHRtoUINT(buf[((i + 10) & RE_BUFFER_SIZE)], buf[((i + 11) & RE_BUFFER_SIZE)]);
            sjxx.month = buf[((i + 12) & RE_BUFFER_SIZE)];
            sjxx.day = buf[((i + 13) & RE_BUFFER_SIZE)];
            sjxx.hour = buf[((i + 14) & RE_BUFFER_SIZE)];
            sjxx.minute = buf[((i + 15) & RE_BUFFER_SIZE)];
            sjxx.second = buf[((i + 16) & RE_BUFFER_SIZE)];
            // print_sjxx();
            print_flag |= PRINT_SJXX;
            if ((status & ~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM)) == STEP_SJSC)
                status |= Convert.ToByte(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);
        }

        void Extract_FKXX(UCHR[] buf, UINT i)
        {

            fkxx.flbz = buf[((i + 10) & RE_BUFFER_SIZE)];
            fkxx.fjxx[0] = buf[((i + 11) & RE_BUFFER_SIZE)];
            fkxx.fjxx[1] = buf[((i + 12) & RE_BUFFER_SIZE)];
            fkxx.fjxx[2] = buf[((i + 13) & RE_BUFFER_SIZE)];
            fkxx.fjxx[3] = buf[((i + 14) & RE_BUFFER_SIZE)];
            Handle_FXXX();
            print_flag |= PRINT_FKXX;
            // print_fkxx();

        }

        void Extract_GNTX(UCHR[] buf, UINT i)
        {

            gntx.sqlx = Convert.ToSByte(buf[((i + 10) & RE_BUFFER_SIZE)]);
            gntx.year = Convert.ToUInt32(buf[((i + 11) & RE_BUFFER_SIZE)]) + 2000;
            gntx.month = buf[((i + 12) & RE_BUFFER_SIZE)];
            gntx.day = buf[((i + 13) & RE_BUFFER_SIZE)];
            gntx.hour = buf[((i + 14) & RE_BUFFER_SIZE)];
            gntx.minute = buf[((i + 15) & RE_BUFFER_SIZE)];
            gntx.second = buf[((i + 16) & RE_BUFFER_SIZE)];
            // print_gntx();
            print_flag |= PRINT_GNTX;
            if ((status & ~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM)) == STEP_GNTS)
                status |= Convert.ToByte(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);
        }

        void Extract_GNPX(UCHR[] buf, UINT i)
        {

            gnpx.jdfw = buf[((i + 10) & RE_BUFFER_SIZE)];
            gnpx.jd = buf[((i + 11) & RE_BUFFER_SIZE)];
            gnpx.jf = buf[((i + 12) & RE_BUFFER_SIZE)];
            gnpx.jm = buf[((i + 13) & RE_BUFFER_SIZE)];
            gnpx.jxm = buf[((i + 14) & RE_BUFFER_SIZE)];
            gnpx.wdfw = buf[((i + 15) & RE_BUFFER_SIZE)];
            gnpx.wd = buf[((i + 16) & RE_BUFFER_SIZE)];
            gnpx.wf = buf[((i + 17) & RE_BUFFER_SIZE)];
            gnpx.wm = buf[((i + 18) & RE_BUFFER_SIZE)];
            gnpx.wxm = buf[((i + 19) & RE_BUFFER_SIZE)];
            gnpx.gd = UCHRtoINT(buf[((i + 20) & RE_BUFFER_SIZE)], buf[((i + 21) & RE_BUFFER_SIZE)]);
            gnpx.sd = UCHRtoUINT(buf[((i + 22) & RE_BUFFER_SIZE)], buf[((i + 23) & RE_BUFFER_SIZE)]);
            gnpx.fx = UCHRtoUINT(buf[((i + 24) & RE_BUFFER_SIZE)], buf[((i + 25) & RE_BUFFER_SIZE)]);
            gnpx.wxs = buf[((i + 26) & RE_BUFFER_SIZE)];
            gnpx.zt = buf[((i + 27) & RE_BUFFER_SIZE)];
            gnpx.jdxs = buf[((i + 28) & RE_BUFFER_SIZE)];
            gnpx.gjwc = UCHRtoUINT(buf[((i + 29) & RE_BUFFER_SIZE)], buf[((i + 30) & RE_BUFFER_SIZE)]);
            // print_gnpx();
            Handle_GNPX();
            print_flag |= PRINT_GNPX;
            if ((status & ~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM)) == STEP_GNPS)
                status |= Convert.ToByte(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);
        }

        void Extract_GNVX(UCHR[] buf, UINT i)
        {

            gnvx.wxlb = buf[((i + 10) & RE_BUFFER_SIZE)];
            if (buf[((i + 10) & RE_BUFFER_SIZE)] == 2)
            {
                gnvx.gps_wxgs = buf[((i + 11) & RE_BUFFER_SIZE)];
                gnvx.gps_wxxx = new WXXX[gnvx.gps_wxgs];
                for (UINT _i = 0; _i < gnvx.gps_wxgs; ++_i)
                {
                    UINT j = i + 12 + _i * 5;
                    gnvx.gps_wxxx[_i].wxbh = buf[(j & RE_BUFFER_SIZE)];
                    gnvx.gps_wxxx[_i].wxyj = buf[((j + 1) & RE_BUFFER_SIZE)];
                    gnvx.gps_wxxx[_i].fwj = UCHRtoUINT(buf[((j + 2) & RE_BUFFER_SIZE)], buf[((j + 3) & RE_BUFFER_SIZE)]);
                    gnvx.gps_wxxx[_i].xzb = buf[((j + 4) & RE_BUFFER_SIZE)];
                }
            }
            if (buf[((i + 10) & RE_BUFFER_SIZE)] == 1)
            {
                gnvx.bds_wxgs = buf[((i + 11) & RE_BUFFER_SIZE)];
                gnvx.bds_wxxx = new WXXX[gnvx.bds_wxgs];
                for (UINT _i = 0; _i < gnvx.bds_wxgs; ++_i)
                {
                    UINT j = i + 12 + _i * 5;
                    gnvx.bds_wxxx[_i].wxbh = buf[(j & RE_BUFFER_SIZE)];
                    gnvx.bds_wxxx[_i].wxyj = buf[((j + 1) & RE_BUFFER_SIZE)];
                    gnvx.bds_wxxx[_i].fwj = UCHRtoUINT(buf[((j + 2) & RE_BUFFER_SIZE)], buf[((j + 3) & RE_BUFFER_SIZE)]);
                    gnvx.bds_wxxx[_i].xzb = buf[((j + 4) & RE_BUFFER_SIZE)];
                }
            }

            //print_gnvx();
            print_flag |= PRINT_GNVX;
            if ((status & ~(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM)) == STEP_GNVS)
                status |= Convert.ToByte(STATUS_BIT_ANSWER | STATUS_BIT_CONFIRM);

        }
    }
}
