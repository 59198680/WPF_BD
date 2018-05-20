/***********************Project Version1.1*************************
@项目名:北斗传输4.0(C#)
@File:MyDataBase.cs
@File_Version:1.1
@Author:lys
@QQ:591986780
@UpdateTime:2018年5月21日03:20:37

@说明:实现基本的数据库功能

本程序基于.Net4.6.1编写的北斗短报文传输程序
界面使用WPF框架编写
在vs2017里运行通过
数据库使用SQL SERVER 2017
******************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp_BD
{
    class MyDataBase
    {
        public MyDataBase( string str)
        {
            try
            {
                connectionstr = "Data Source=.;Initial Catalog=BD_TEST;User ID=admin;Password=591986780 ";
                conn = new SqlConnection(connectionstr);
                cmdstr = str;
                conn.Open();
                comm = new SqlCommand(cmdstr, conn);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "数据库连接失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool MyRead()
        {
            bool ret = false;
            try
            {
                SqlDataReader dr = comm.ExecuteReader();
                ret = dr.Read();
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "数据库读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                conn.Close();
            }
            return ret;
        }

        public bool MyModify()
        {
            bool ret = false;
            try
            {
                ret = (1 == comm.ExecuteNonQuery());
                //dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "数据库修改失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                conn.Close();
            }
            return ret;
        }

        public DataSet MySelect()
        {
            try
            {
                da = new SqlDataAdapter();
                da.SelectCommand = comm;
                ds = new DataSet();
                da.Fill(ds);
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "数据库查询失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        //public static bool deletemess( string id)
        //{
        //    bool ret = false;
        //    return ret = (new MyDataBase(  "DELETE FROM Message WHERE m_id = '" + id + "'").MyModify());
        //}
        //public static DataSet searchm_id( string id)
        //{
        //    return (new MyDataBase(  "select * from Message where '" + id + "'=m_name order by m_id  desc").MySelect());
        //}
        //public static DataSet searchm_mess( string key)
        //{
        //    return (new MyDataBase(  "select * from Message where m_message like'%" + key + "%' order by m_id  desc").MySelect());
        //}
        //public static DataSet selectmessage( )
        //{
        //    return (new MyDataBase(  "select * from Message  order by m_id desc").MySelect());
        //}
        public static bool CheckBDID_exist( string id)
        {
            bool ret = false;
            return ret = (new MyDataBase(  "select * from ID where '" + id + "'=BD_CARD_ID ").MyRead());
        }
        public static bool Insertidcard( int ID, DateTime time)
        {
            bool ret = false;
            return ret = (new MyDataBase("insert into ID(BD_CARD_ID,Last_connettime) values(" + ID + "','" + time.ToString() + "')").MyModify());
        }
        public static bool Updateidcard(int ID, DateTime time)
        {
            bool ret = false;
            return ret = (new MyDataBase("update ID set Last_connettime='"+ time.ToString() + "' where BD_CARD_ID='"+ID+"'").MyModify());
        }
        public static bool InsertData(int ID, float temp,float mq135,byte wd,byte wf,byte wm, byte jd,byte jf, byte jm,char wdfw,char jdfw,DateTime time,string Location)
        {
            bool ret = false;
            return ret = (new MyDataBase("insert into DATA(BD_Card_ID,Temper,MQ135,wd,wf,wm,jd,jf,jm,wdfw,jdfw,Location,dataTime) values('" + ID + "','" + temp + "','" + mq135 + "','" + wd + "','" + wf
                + "','" + wm + "','" + jd + "','" + jf + "','" + jm + "','" + wdfw.ToString() + "','" + jdfw.ToString() + "','" + Location + "','" + time.ToString() + "')").MyModify());
        }
        public SqlConnection conn;
        public SqlCommand comm;
        public SqlDataReader dr;
        public SqlDataAdapter da;
        public DataSet ds;
        public string connectionstr;
        public string cmdstr;
    }
}
