using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BD_Protocol
{
    using UINT = System.UInt32;
    using UCHR = System.Byte;
    partial class BD
    {
        static UINT error_count = 0;
        public void Receive_Protocol()
        {
            UINT i = rebuff.rp;
            if (rebuff.rp != rebuff.wp)
            {
                if (error_count >= 1024)
                {
                    //TODO
                    MessageBox.Show("error_count=" + error_count, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    rebuff.rp = rebuff.wp = 0;
                    error_count = 0;
                    return;
                }
                if (Check_overflow(7))//检查是否符合读取条件，7为读取到长度的字节数。&XXXXMM
                {
                    if (rebuff.buffer[i] == '$')
                    {

                        if (rebuff.buffer[(i + 1) & RE_BUFFER_SIZE] == 'G'
                            && rebuff.buffer[(i + 2) & RE_BUFFER_SIZE] == 'N'
                            && rebuff.buffer[(i + 3) & RE_BUFFER_SIZE] == 'T'
                            && rebuff.buffer[(i + 4) & RE_BUFFER_SIZE] == 'X')//GNTX
                        {
                            UINT _lenth = 0;
                            _lenth = (UINT)rebuff.buffer[(i + 5) & RE_BUFFER_SIZE] * 256 + (UINT)rebuff.buffer[(i + 6) & RE_BUFFER_SIZE];
                            if (Check_overflow(_lenth))
                            {

                                if (rebuff.buffer[(rebuff.rp + _lenth - 1) & RE_BUFFER_SIZE] == Xor_checksum(ref rebuff.buffer, i, _lenth - 1))
                                {
                                    Extract_GNTX(rebuff.buffer, i);
                                }
                                rebuff.rp = (rebuff.rp + _lenth) & RE_BUFFER_SIZE;
                                error_count = 0;
                            }
                            else
                            {
                                ++error_count;
                                return;
                            }
                        }
                        else if (rebuff.buffer[(i + 1) & RE_BUFFER_SIZE] == 'G'
                            && rebuff.buffer[(i + 2) & RE_BUFFER_SIZE] == 'N'
                            && rebuff.buffer[(i + 3) & RE_BUFFER_SIZE] == 'P'
                            && rebuff.buffer[(i + 4) & RE_BUFFER_SIZE] == 'X')//GNPX
                        {
                            UINT _lenth = 0;

                            _lenth = GNPX_LENTH;

                           // _lenth = (UINT)rebuff.buffer[(i + 5) & RE_BUFFER_SIZE] * 256 + (UINT)rebuff.buffer[(i + 6) & RE_BUFFER_SIZE];

                            if (Check_overflow( _lenth))
                            {

                                if (rebuff.buffer[(rebuff.rp + _lenth - 1) & RE_BUFFER_SIZE] == Xor_checksum(ref rebuff.buffer, i, _lenth - 1))
                                {
                                    Extract_GNPX(rebuff.buffer, i);
                                }
                                rebuff.rp = (rebuff.rp + _lenth) & RE_BUFFER_SIZE;
                                error_count = 0;
                            }
                            else
                            {
                                ++error_count;
                                return;
                            }
                        }
                        else if (rebuff.buffer[(i + 1) & RE_BUFFER_SIZE] == 'G'
                            && rebuff.buffer[(i + 2) & RE_BUFFER_SIZE] == 'N'
                            && rebuff.buffer[(i + 3) & RE_BUFFER_SIZE] == 'V'
                            && rebuff.buffer[(i + 4) & RE_BUFFER_SIZE] == 'X')//GNVX
                        {
                            UINT _lenth = 0;

                           // _lenth = GNVX_LENTH;

                            _lenth = (UINT)rebuff.buffer[(i + 5) & RE_BUFFER_SIZE] * 256 + (UINT)rebuff.buffer[(i + 6) & RE_BUFFER_SIZE];

                            if (Check_overflow( _lenth))
                            {

                                if (rebuff.buffer[(rebuff.rp + _lenth - 1) & RE_BUFFER_SIZE] == Xor_checksum(ref rebuff.buffer, i, _lenth - 1))
                                {
                                    Extract_GNVX(rebuff.buffer, i);
                                }
                                rebuff.rp = (rebuff.rp + _lenth) & RE_BUFFER_SIZE;
                                error_count = 0;
                            }
                            else
                            {
                                ++error_count;
                                return;
                            }
                        }
                        else if (rebuff.buffer[(i + 1) & RE_BUFFER_SIZE] == 'D'
                            && rebuff.buffer[(i + 2) & RE_BUFFER_SIZE] == 'W'
                            && rebuff.buffer[(i + 3) & RE_BUFFER_SIZE] == 'X'
                            && rebuff.buffer[(i + 4) & RE_BUFFER_SIZE] == 'X')//定位信息
                        {
                            UINT _lenth = 0;

                            _lenth = DWXX_LENTH;

                           // _lenth = (UINT)rebuff.buffer[(i + 5) & RE_BUFFER_SIZE] * 256 + (UINT)rebuff.buffer[(i + 6) & RE_BUFFER_SIZE];

                            if (Check_overflow( _lenth))
                            {
                                if (rebuff.buffer[(rebuff.rp + _lenth - 1) & RE_BUFFER_SIZE] == Xor_checksum(ref rebuff.buffer, i, _lenth - 1))
                                {
                                    Extract_DWXX(rebuff.buffer, i);
                                }

                                rebuff.rp = (rebuff.rp + _lenth) & RE_BUFFER_SIZE;
                                error_count = 0;
                            }
                            else
                            {
                                ++error_count;
                                return;
                            }

                        }
                        else if (rebuff.buffer[(i + 1) & RE_BUFFER_SIZE] == 'T'
                            && rebuff.buffer[(i + 2) & RE_BUFFER_SIZE] == 'X'
                            && rebuff.buffer[(i + 3) & RE_BUFFER_SIZE] == 'X'
                            && rebuff.buffer[(i + 4) & RE_BUFFER_SIZE] == 'X')//通信信息
                        {
                            UINT _lenth = 0;

                           // _lenth = TXXX_LENTH;

                            _lenth = (UINT)rebuff.buffer[(i + 5) & RE_BUFFER_SIZE] * 256 + (UINT)rebuff.buffer[(i + 6) & RE_BUFFER_SIZE];

                            if (Check_overflow( _lenth))
                            {
                                if (rebuff.buffer[(rebuff.rp + _lenth - 1) & RE_BUFFER_SIZE] == Xor_checksum(ref rebuff.buffer, i, _lenth - 1))
                                {
                                    Extract_TXXX(rebuff.buffer, i);
                                }

                                rebuff.rp = (rebuff.rp + _lenth) & RE_BUFFER_SIZE;
                                error_count = 0;
                            }
                            else
                            {
                                ++error_count;
                                return;
                            }
                        }
                        else if (rebuff.buffer[(i + 1) & RE_BUFFER_SIZE] == 'I'
                            && rebuff.buffer[(i + 2) & RE_BUFFER_SIZE] == 'C'
                            && rebuff.buffer[(i + 3) & RE_BUFFER_SIZE] == 'X'
                            && rebuff.buffer[(i + 4) & RE_BUFFER_SIZE] == 'X')//IC信息
                        {
                            UINT _lenth = 0;
                            _lenth = ICXX_LENTH;
                            //_lenth = (UINT)rebuff.buffer[(i + 5) & RE_BUFFER_SIZE] * 256 + (UINT)rebuff.buffer[(i + 6) & RE_BUFFER_SIZE];
                            if (Check_overflow( _lenth))
                            {
                                if (rebuff.buffer[(rebuff.rp + _lenth - 1) & RE_BUFFER_SIZE] == Xor_checksum(ref rebuff.buffer, i, _lenth - 1))
                                {
                                    Extract_ICXX(rebuff.buffer, i);
                                }

                                rebuff.rp = (rebuff.rp + _lenth) & RE_BUFFER_SIZE;
                                error_count = 0;
                            }
                            else
                            {
                                ++error_count;
                                return;
                            }
                        }
                        else if (rebuff.buffer[(i + 1) & RE_BUFFER_SIZE] == 'Z'
                            && rebuff.buffer[(i + 2) & RE_BUFFER_SIZE] == 'J'
                            && rebuff.buffer[(i + 3) & RE_BUFFER_SIZE] == 'X'
                            && rebuff.buffer[(i + 4) & RE_BUFFER_SIZE] == 'X')//自检信息
                        {
                            UINT _lenth = 0;

                            _lenth = ZJXX_LENTH;

                           // _lenth = (UINT)rebuff.buffer[(i + 5) & RE_BUFFER_SIZE] * 256 + (UINT)rebuff.buffer[(i + 6) & RE_BUFFER_SIZE];

                            if (Check_overflow( _lenth))
                            {
                                if (rebuff.buffer[(rebuff.rp + _lenth - 1) & RE_BUFFER_SIZE] == Xor_checksum(ref rebuff.buffer, i, _lenth - 1))
                                {
                                    Extract_ZJXX(rebuff.buffer, i);
                                }

                                rebuff.rp = (rebuff.rp + _lenth) & RE_BUFFER_SIZE;
                                error_count = 0;
                            }
                            else
                            {
                                ++error_count;
                                return;
                            }
                        }
                        else if (rebuff.buffer[(i + 1) & RE_BUFFER_SIZE] == 'S'
                            && rebuff.buffer[(i + 2) & RE_BUFFER_SIZE] == 'J'
                            && rebuff.buffer[(i + 3) & RE_BUFFER_SIZE] == 'X'
                            && rebuff.buffer[(i + 4) & RE_BUFFER_SIZE] == 'X')//时间信息
                        {
                            UINT _lenth = 0;

                            _lenth = SJXX_LENTH;

                            //_lenth = (UINT)rebuff.buffer[(i + 5) & RE_BUFFER_SIZE] * 256 + (UINT)rebuff.buffer[(i + 6) & RE_BUFFER_SIZE];

                            if (Check_overflow( _lenth))
                            {
                                if (rebuff.buffer[(rebuff.rp + _lenth - 1) & RE_BUFFER_SIZE] == Xor_checksum(ref rebuff.buffer, i, _lenth - 1))
                                {
                                    Extract_SJXX(rebuff.buffer, i);
                                }

                                rebuff.rp = (rebuff.rp + _lenth) & RE_BUFFER_SIZE;
                                error_count = 0;
                            }
                            else
                            {
                                ++error_count;
                                return;
                            }
                        }
                        else if (rebuff.buffer[(i + 1) & RE_BUFFER_SIZE] == 'B'
                            && rebuff.buffer[(i + 2) & RE_BUFFER_SIZE] == 'B'
                            && rebuff.buffer[(i + 3) & RE_BUFFER_SIZE] == 'X'
                            && rebuff.buffer[(i + 4) & RE_BUFFER_SIZE] == 'X')//版本信息
                        {

                        }
                        else if (rebuff.buffer[(i + 1) & RE_BUFFER_SIZE] == 'F'
                            && rebuff.buffer[(i + 2) & RE_BUFFER_SIZE] == 'K'
                            && rebuff.buffer[(i + 3) & RE_BUFFER_SIZE] == 'X'
                            && rebuff.buffer[(i + 4) & RE_BUFFER_SIZE] == 'X')//反馈信息
                        {
                            UINT _lenth = 0;
                            _lenth = FKXX_LENTH;
                           // _lenth = (UINT)rebuff.buffer[(i + 5) & RE_BUFFER_SIZE] * 256 + (UINT)rebuff.buffer[(i + 6) & RE_BUFFER_SIZE];
                            if (Check_overflow( _lenth))
                            {
                                if (rebuff.buffer[(rebuff.rp + _lenth - 1) & RE_BUFFER_SIZE] == Xor_checksum(ref rebuff.buffer, i, _lenth - 1))
                                {
                                    Extract_FKXX(rebuff.buffer, i);
                                }

                                rebuff.rp = (rebuff.rp + _lenth) & RE_BUFFER_SIZE;
                                error_count = 0;
                            }
                            else
                            {
                                ++error_count;
                                return;
                            }
                        }
                        else
                        {
                            //不可解析
                            ++error_count;
                            rebuff.rp = (rebuff.rp + 1) & RE_BUFFER_SIZE;
                        }
                    }
                    else
                    {
                        ++error_count;
                        rebuff.rp = (rebuff.rp + 1) & RE_BUFFER_SIZE;
                    }
                }
                else
                {
                    ++error_count;
                }
            }
        }
    }
}
