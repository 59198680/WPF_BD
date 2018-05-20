/***********************Project Version1.1*************************
@项目名:北斗传输4.0(C#)
@File:BaiduAPI.cs
@File_Version:1.1
@Author:lys
@QQ:591986780
@UpdateTime:2018年5月21日03:19:42


@说明:实现调用百度API实现坐标转换,地址输出

本程序基于.Net4.6.1编写的北斗短报文传输程序
界面使用WPF框架编写
在vs2017里运行通过

******************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace BD_Protocol
{
    class BaiduAPI
    {
        public static bool Geocoding_API(string lat, string lng, ref string result)
        {
            //string lat = Convert.ToString(39.9550185185185);
            //string lng = Convert.ToString(116.798907407407);
            if (GetCoords(ref lat, ref lng, ref result))
            {
                if (GetAddress(lat, lng, ref result))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        private static bool GetAddress(string lat, string lng, ref string result)
        {
            try
            {
                string url = @"http://api.map.baidu.com/geocoder/v2/?callback=renderReverse&location=" + lat + "," + lng + @"&output=xml&pois=0&ak=tS0gGsylUKisLfWLovFslvjjZ465LuAv";
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";

                WebResponse response = request.GetResponse() as HttpWebResponse;
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream, System.Text.Encoding.GetEncoding("utf-8"));
                int count = 0;
                StringBuilder sb = new StringBuilder();
                char[] leftBuffer = new char[5000];
                while ((count = reader.Read(leftBuffer, 0, leftBuffer.Length)) > 0)
                {
                    sb.Append(new String(leftBuffer, 0, count));
                }
                reader.Close();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sb.ToString());
                string status = xml.DocumentElement.SelectSingleNode("status").InnerText;
                if (status == "0")
                {

                    XmlNodeList nodes = xml.DocumentElement.GetElementsByTagName("formatted_address");
                    if (nodes.Count > 0)
                    {
                        result = nodes[0].InnerText;
                        return true;
                    }
                    else
                    {
                        result = "未获取到位置信息,nodes.Count=" + nodes.Count;
                        return false;
                    }

                }
                else
                {
                    result = "未获取到位置信息,错误码" + Convert.ToUInt16(status);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                //return "未获取到位置信息,连接出错";
                result = "API调用失败";
                return false;
            }
        }
        private static bool GetCoords(ref string lat, ref string lng, ref string result)
        {
            try
            {
                string url = @"http://api.map.baidu.com/geoconv/v1/?coords=" + lng + "," + lat + @"&from=1&to=5&output=xml&ak=tS0gGsylUKisLfWLovFslvjjZ465LuAv";
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                WebResponse response = request.GetResponse() as HttpWebResponse;
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream, System.Text.Encoding.GetEncoding("utf-8"));
                int count = 0;
                StringBuilder sb = new StringBuilder();
                char[] leftBuffer = new char[5000];
                while ((count = reader.Read(leftBuffer, 0, leftBuffer.Length)) > 0)
                {
                    sb.Append(new String(leftBuffer, 0, count));
                }
                reader.Close();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sb.ToString());
                string status = xml.DocumentElement.SelectSingleNode("status").InnerText;
                if (status == "0")
                {
                    XmlNodeList nodes = xml.DocumentElement.GetElementsByTagName("y");
                    if (nodes.Count > 0)
                    {
                        lat = nodes[0].InnerText;
                    }
                    else
                    {
                        result = "未获取y,nodes.Count=" + nodes.Count;
                        return false;
                    }
                    XmlNodeList nodes1 = xml.DocumentElement.GetElementsByTagName("x");
                    if (nodes1.Count > 0)
                    {
                        lng = nodes1[0].InnerText;
                    }
                    else
                    {
                        result = "未获取到x,nodes.Count=" + nodes.Count;
                        return false;
                    }
                    return true;
                }
                else
                {
                    result = "未获取到位置信息,错误码" + Convert.ToUInt16(status);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                result = "API调用失败";
                return false;
            }
        }
    }
}
