/***********************Project Version1.5*************************
@项目名:北斗传输4.0(C#)
@File:MyDataBase.cs
@File_Version:1.5a
@Author:lys
@QQ:591986780
@UpdateTime:2018年5月28日15:55:01

@说明:打印错误log至TXT

本程序基于.Net4.6.1编写的北斗短报文传输程序
界面使用WPF框架编写
在vs2017里运行通过
数据库使用SQL SERVER 2017
******************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_BD
{
    class WriteLog
    {
        private static StreamWriter streamWriter; //写文件    

        public static void WriteError(Exception ex)
        {
            try
            {
                //DateTime dt = new DateTime();  
                string directPath = "./";    //在获得文件夹路径  
                if (!Directory.Exists(directPath))   //判断文件夹是否存在，如果不存在则创建  
                {
                    Directory.CreateDirectory(directPath);
                }
                directPath += string.Format(@"\WpfApp_BD_{0}_ERROR.log", DateTime.Now.ToString("yyyy-MM-dd"));
                if (streamWriter == null)
                {
                    streamWriter = !File.Exists(directPath) ? File.CreateText(directPath) : File.AppendText(directPath);    //判断文件是否存在如果不存在则创建，如果存在则添加。  
                }
                streamWriter.WriteLine("***********************************************************************");
                streamWriter.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
               // streamWriter.WriteLine("输出信息：错误信息");
                if (ex != null)
                {
                    streamWriter.WriteLine("异常信息：\r\n" + ex.Message);
                    streamWriter.WriteLine("异常对象：\r\n" + ex.Source);
                    streamWriter.WriteLine("调用堆栈：\r\n" + ex.StackTrace.Trim());
                    streamWriter.WriteLine("触发方法：\r\n" + ex.TargetSite);

                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Flush();
                    streamWriter.Dispose();
                    streamWriter = null;
                }
            }
        }
    }
}
