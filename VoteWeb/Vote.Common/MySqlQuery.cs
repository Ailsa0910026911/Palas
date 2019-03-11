using MySql.Data.MySqlClient;
using MySQLHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Vote.Common
{
    public class MySqlQuery
    {

        public static Dictionary<int, int> dic = new Dictionary<int, int>();
        public static object obj1 = new object();
        public DataTable GetAward()
        {
            MemoryCache cache2 = MemoryCache.Default;
            if (!cache2.Contains("Index"))
            {
                lock (obj1)
                {
                    if (!cache2.Contains("Index"))
                    {
                        string strText = "SELECT Award FROM `tabcandidate` GROUP BY Award";
                        DataTable dt= MySQLCommon.ExecuteDataTable(strText);
                        cache2.Set("Index", dt, DateTimeOffset.Now.AddDays(1));
                    }
                }
            }
            DataTable dt2 = cache2["Index"] as DataTable;

            return dt2;
        }


        public static object obj = new object();

        public static object obj2 = new object();
        /// <summary>
        /// 查询所有参选人的信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetTabCandidate(string Award)
        {
            string key = getMd5Hash(Award);
            MemoryCache cache3 = MemoryCache.Default;
            if (!cache3.Contains(key))
            {
                lock (obj)
                {
                    if (!cache3.Contains(key))
                    {
                        string strText = "SELECT Id,`Name`,Img,Votes,Rank FROM tabcandidate WHERE Award = '" + Award + "' ORDER BY Votes DESC";
                        DataTable dt = MySQLCommon.ExecuteDataTable(strText);
                        int count = -1;
                        int rank = 1;

                        foreach (DataRow row in dt.Rows)
                        {
                            Int32 votes = 0;
                            Int32.TryParse(row["Votes"].ToString(), out votes);
                            row["Votes"] = votes;
                            if (count <= votes)
                            {
                                count = votes;
                                row["Rank"] = rank;
                            }
                            else
                            {
                                count = votes;
                                rank++;
                                row["Rank"] = rank;
                            }

                            if (dic.ContainsKey(Convert.ToInt32(row["Id"])))
                                dic[Convert.ToInt32(row["Id"])] = Convert.ToInt32(row["Rank"]);
                            else
                                dic.Add(Convert.ToInt32(row["Id"]), Convert.ToInt32(row["Rank"]));

                        }

                        cache3.Set(key, dt, DateTimeOffset.Now.AddMinutes(3));
                    }
                }
            }

            DataTable dt2 = cache3[key] as DataTable;

            
            return dt2;
        }

        public static string getMd5Hash(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }

            //处理百度贴吧回帖的问题
            if (input.Contains("tieba") && input.Contains("?"))
                input = input.Substring(0, input.LastIndexOf("?"));

            var benchStr = input.Trim();
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(benchStr));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// 根据Id查询参选人的信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DataTable GetTabCandidateWhere(int Id)
        {
            MemoryCache cache4 = MemoryCache.Default;
            if (!cache4.Contains("xy" + Id.ToString()))
            {
                lock (obj2)
                {
                    if (!cache4.Contains("xy" + Id.ToString()))
                    {
                        string strText = "SELECT Id,`Name`,Sex,Age,School,Img,Player,Awards,Evaluation,Story,Reason,Votes,Rank FROM tabcandidate WHERE Id=@Id";
                        MySqlParameter param = new MySqlParameter() { ParameterName = "Id", Value = Id, MySqlDbType = MySqlDbType.Int32 };
                        DataTable dt = MySQLCommon.ExecuteDataTable(strText, param);

                        cache4.Set("xy" + Id.ToString(), dt, DateTimeOffset.Now.AddMinutes(1));
                    }
                }
            }
            DataTable dt2 = cache4["xy" + Id.ToString()] as DataTable;
            return dt2;
        }
        /// <summary>
        /// 添加投票信息
        /// </summary>
        /// <param name="vote"></param>
        /// <returns></returns>
        public int Addvote(TabVoteItems vote)
        {
            string strText = "insert into tabvote values(@Id,@Ip,@session,@user_agent,@Votetime,@TabCanId)";
            MySqlParameter[] param = new MySqlParameter[] { 
                new MySqlParameter(){ParameterName="Id",Value=vote.Id,MySqlDbType=MySqlDbType.Int32},
                new MySqlParameter(){ParameterName="Ip",Value=vote.Ip,MySqlDbType=MySqlDbType.VarChar},
                new MySqlParameter(){ParameterName="session",Value=vote.session,MySqlDbType=MySqlDbType.VarChar},
                new MySqlParameter(){ParameterName="user_agent",Value=vote.user_agent,MySqlDbType=MySqlDbType.VarChar},
                new MySqlParameter(){ParameterName="Votetime",Value=vote.Votetime,MySqlDbType=MySqlDbType.DateTime},
                new MySqlParameter(){ParameterName="TabCanId",Value=vote.TabCanId,MySqlDbType=MySqlDbType.Int32}
            };
            return MySQLCommon.ExecuteNonQuery(strText, param);
        }



        public void Getcount(int ID)
        {
            MemoryCache cache1 = MemoryCache.Default;
            if (!cache1.Contains("tp"+ID.ToString()))
            {
                lock (ID.ToString())
                {
                    if (!cache1.Contains("tp" + ID.ToString()))
                    {
                        string strtext = "select count(*) from tabvote where TabCanId=@ID";
                        MySqlParameter param = new MySqlParameter() { ParameterName = "ID", Value = ID, MySqlDbType = MySqlDbType.Int32 };
                        object count = MySQLCommon.ExecuteScalar(strtext, param);

                        string strtxt2 = "update tabcandidate set Votes=@Count where Id=@ID";
                        MySqlParameter[] param2 = new MySqlParameter[] { 
                        new MySqlParameter(){ ParameterName = "ID", Value = ID, MySqlDbType = MySqlDbType.Int32 },
                        new MySqlParameter(){ ParameterName="Count",Value=Convert.ToInt32(count),MySqlDbType=MySqlDbType.Int32}};
                        MySQLCommon.ExecuteNonQuery(strtxt2, param2);

                        cache1.Set("tp" + ID.ToString(), ID, DateTimeOffset.Now.AddMinutes(5));  
                    }
                }
            }
        }
    }
}
