/***********************Project Version1.4*************************
@项目名:北斗传输4.0(C#)
@File:MyDataBase.cs
@File_Version:1.4a
@Author:lys
@QQ:591986780
@UpdateTime:2018年5月28日15:24:34

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
        public MyDataBase(string str)
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
                //MessageBox.Show(ex.ToString(), "数据库连接失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                //MessageBox.Show(ex.ToString(), "数据库读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                //MessageBox.Show(ex.ToString(), "数据库修改失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                conn.Close();
            }
            return ret;
        }

        public bool MyCreate()
        {
            bool ret = false;
            try
            {
                comm.ExecuteNonQuery();
                ret = true;//不报错就是执行建表成功
                //dr.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "数据库修改失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                //MessageBox.Show(ex.ToString(), "数据库查询失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        public static DataSet Select_UserId()
        {
            return (new MyDataBase("select BD_Card_ID from ID").MySelect());
        }
        public static DataSet Select_Data_For_Time(int ID, DateTime start, DateTime end)
        {
            return (new MyDataBase("select Temperature, Humidity,MQ135,dateTime from UserID_" + ID + "_Data where BD_Card_ID='" + ID + "' " + "and dateTime>'" + start.ToString() + "' and dateTime<'" + end.ToString() + "'").MySelect());
        }

        public static DataSet Select_Data_Top(int ID, int num)
        {
            return (new MyDataBase("select Temperature,Humidity, MQ135,dateTime from UserID_" + ID + "_Data where  BD_Card_ID='" + ID + "'" + " and num > (select max(num)-" + num + " from UserID_" + ID + "_Data )").MySelect());
        }
        public static DataSet Select_Data_Addr(int ID)
        {
            return (new MyDataBase("select  Location from UserID_" + ID + "_Data where  BD_Card_ID='" + ID + "'" + " and num > (select max(num)-" + 1 + " from UserID_" + ID + "_Data )").MySelect());
        }
        public static bool CheckBDID_exist(int id)
        {
            bool ret = false;
            return ret = (new MyDataBase("select * from ID where '" + id + "'=BD_Card_ID ").MyRead());
        }
        public static DataSet Select_ID_Info(int ID)
        {
            return (new MyDataBase("select * from ID where '" + ID + "'=BD_Card_ID ").MySelect());
        }

        public static DataSet Select_Alarm(int ID)
        {
            return (new MyDataBase("select Temper_min,Temper_max,Humi_min,Humi_max, MQ135_min,MQ135_max from RangeOfAlarm where  BD_Card_ID='" + ID + "'").MySelect());
        }
        public static bool CheckAlarm_isexist(int id)
        {
            bool ret = false;
            return ret = (new MyDataBase("select * from RangeOfAlarm where '" + id + "'=BD_Card_ID ").MyRead());
        }
        public static bool UpdateAlarm(int ID, float tma, float tmi, float hma, float hmi, float mq135ma, float mq135mi)
        {
            bool ret = false;
            return ret = (new MyDataBase("update RangeOfAlarm set Temper_min='" + tmi.ToString() + "',Temper_max='" + tma.ToString() + "',Humi_max='" + hma + "',Humi_min='" + hmi + "',MQ135_min='" + mq135mi + "',MQ135_max='" + mq135ma.ToString() + "' where BD_Card_ID='" + ID + "'").MyModify());
        }
        public static bool InsertAlarm(int ID, float tma, float tmi, float hma, float hmi, float mq135ma, float mq135mi)
        {
            bool ret = false;
            return ret = (new MyDataBase("insert into RangeOfAlarm(BD_Card_ID,Temper_min,Temper_max,Humi_max,Humi_min,MQ135_min,MQ135_max) values('" + ID + "','" + tmi.ToString() + "','" + tma.ToString() + "','" + mq135mi.ToString() + "','" + mq135ma.ToString() + "')").MyModify());
        }
        public static bool Insertidcard(int ID, DateTime time)
        {
            bool ret = false;
            return ret = (new MyDataBase("insert into ID(BD_Card_ID,Last_connettime) values('" + ID + "','" + time.ToString() + "')").MyModify());
        }
        public static bool Insertidcard(int ID, DateTime time, int iczt, int yjzt, int fwpd, int txdj)
        {
            bool ret = false;
            return ret = (new MyDataBase("insert into ID(BD_Card_ID,Last_connettime,iczt,yjzt,fwpd,txdj) values('" + ID + "','" + time.ToString() + "','" + iczt + "','" + yjzt + "','" + fwpd + "','" + txdj + "')").MyModify());
        }
        public static bool Insertid_ToAlarm(int ID)
        {
            bool ret = false;
            return ret = (new MyDataBase("insert into RangeOfAlarm(BD_Card_ID) values('" + ID + "')").MyModify());
        }
        public static bool Updateidcard(int ID, DateTime time)
        {
            bool ret = false;
            return ret = (new MyDataBase("update ID set Last_connettime='" + time.ToString() + "' where BD_Card_ID='" + ID + "'").MyModify());
        }
        public static bool Updateidcard(int ID, DateTime time, int iczt, int yjzt, int fwpd, int txdj)
        {
            bool ret = false;
            return ret = (new MyDataBase("update ID set Last_connettime='" + time.ToString() + "',iczt='" + iczt + "',yjzt='" + yjzt + "',fwpd='" + fwpd + "',txdj='" + txdj + "' where BD_Card_ID='" + ID + "'").MyModify());
        }
        public static bool InsertData(int ID, float temp, float humi, float mq135, byte wd, byte wf, byte wm, byte jd, byte jf, byte jm, char wdfw, char jdfw, DateTime time, string Location)
        {
            bool ret = false;
            return ret = (new MyDataBase("insert into UserID_" + Convert.ToString(ID) + "_Data(BD_Card_ID,Temperature,Humidity,MQ135,wd,wf,wm,jd,jf,jm,wdfw,jdfw,Location,dateTime) values('" + ID + "','" + temp + "','" + humi + "','" + mq135 + "','" + wd + "','" + wf
                + "','" + wm + "','" + jd + "','" + jf + "','" + jm + "','" + wdfw.ToString() + "','" + jdfw.ToString() + "','" + Location + "','" + time.ToString() + "')").MyModify());
        }
        public SqlConnection conn;
        public SqlCommand comm;
        public SqlDataReader dr;
        public SqlDataAdapter da;
        public DataSet ds;
        public string connectionstr;
        public string cmdstr;

        /// <summary>  
        /// 判断数据库表是否存在  
        /// </summary>  
        /// <param name="db">数据库</param>  
        /// <param name="tb">数据表名</param>  
        /// <param name="connKey">连接数据库的key</param>  
        /// <returns>true:表示数据表已经存在；false，表示数据表不存在</returns>  
        public static bool IsTableExist(string tb)
        {
            string createDbStr = "select 1 from  sysobjects where  id = object_id('" + tb + "') ";
            return new MyDataBase(createDbStr).MyRead();

        }
        public static bool IsViewExist(string view)
        {
            string createDbStr = "select 1 from  sysobjects where  id = object_id('" + view + "') ";
            return new MyDataBase(createDbStr).MyRead();

        }
        public static bool New_tb_for_id_data(int id)
        {
            string createDbStr = "create table UserID_" + Convert.ToString(id) + "_Data(num bigint identity(1,1) not null primary key,BD_Card_ID int not null FOREIGN KEY REFERENCES ID(BD_Card_ID),Temperature float,Humidity float,MQ135 float,dateTime datetime not null,Location varchar(50),wd smallint,wf smallint,wm smallint,jd smallint,jf smallint ,jm smallint,wdfw char(1),jdfw char(1));";
            return new MyDataBase(createDbStr).MyCreate();
        }

        public static bool DeleteView(string view)
        {
            string createDbStr = "drop view " + view + " ";
            return new MyDataBase(createDbStr).MyCreate();

        }
        public static bool CreateView(int ID, float tma, float tmi, float hma, float hmi, float mq135ma, float mq135mi)
        {
            string createDbStr = "CREATE VIEW View_" + ID + "_Alarm AS " +
                "SELECT num,BD_Card_ID,Temperature,Humidity,MQ135,Location,dateTime " +
                "FROM UserID_" + ID + "_Data " +
                "WHERE BD_Card_ID='" + ID + "' " +
                "and ( Temperature>'" + tma + "' " +
                "or Temperature<'" + tmi + "' " +
                "or Humidity>'" + hma + "' " +
                "or Humidity<'" + hmi + "' " +
                "or MQ135>'" + mq135ma + "' " +
                "or MQ135<'" + mq135mi + "' )";
            return new MyDataBase(createDbStr).MyCreate();

        }
        public static DataSet Select_View(int ID)
        {
            return (new MyDataBase("select * from View_" + ID + "_Alarm").MySelect());
        }
    }
}
