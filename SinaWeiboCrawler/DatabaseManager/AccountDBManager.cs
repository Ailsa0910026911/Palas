using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Palas.Common.Lib.Entity;
using Palas.Common.Data;
using SinaWeiboCrawler.Utility;
using HooLab.Log;
using System.Threading;
using Palas.Common.Lib.Business;
using Palas.Common.Lib.DAL;
using NetDimension.Weibo;

namespace SinaWeiboCrawler.DatabaseManager
{
    /// <summary>
    /// LoginAccount与数据库交互的类
    /// </summary>
    class AccountDBManager
    {
        /// <summary>
        /// 初始化所有任务
        /// </summary>
        public static void SetAllSinceIDToNull() 
        {
            string where = "SubscribeStatus>=0";
            var accList = LoginAccountBusiness.GetAllByWhere(where, null, LoginAccountMySqlDAL.OrderColumn.Default);
            foreach (var acc in accList) 
            {
                where = string.Format("AccountID='{0}'", acc.AccountID);
                var param = new LoginAccountMySqlDAL.LoginAccountParameter[3];
                param[0] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.SubscribeSinceID, null);
                param[1] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.SubscribeNextTime, Utilities.Epoch);
                param[2] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.Status, Enums.CrawlStatus.Normal);
                LoginAccountBusiness.UpdateWhere(param, where, null);
            }
        }

        /// <summary>
        /// 获取下一个用僵尸刷订阅微博的任务
        /// </summary>
        /// <returns>满足任务触发条件的僵尸账户</returns>
        public static LoginAccountEntity GetNextSubscribeJob() 
        {
            LoginAccountEntity acc = null;
            string where;
            try
            {
                where = string.Format("IsDisabled=FALSE AND SubscribeStatus>=0 AND Status={0} AND FollowCount>0 AND (SubscribeNextTime IS NULL OR SubscribeNextTime<'{1}')", (sbyte)Enums.CrawlStatus.Normal, DateTime.Now);
                acc = LoginAccountBusiness.GetTopByWhere(where, null, Palas.Common.Lib.DAL.LoginAccountMySqlDAL.OrderColumn.Default);
                if (acc == null) return null;
                acc.Status = (sbyte)Enums.CrawlStatus.Crawling;
                return acc;
            }
            catch (Exception ex) 
            {
                Logger.Error(ex.ToString());
                return null; 
            }
            finally 
            {
                if (acc != null)
                {
                    where = string.Format("AccountID='{0}'", acc.AccountID);
                    var param = new LoginAccountMySqlDAL.LoginAccountParameter[1];
                    param[0] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.Status, (sbyte)Enums.CrawlStatus.Crawling);
                    LoginAccountBusiness.UpdateWhere(param, where, null);
                }
            }
        }

        /// <summary>
        /// 僵尸刷订阅微博的任务完成后，处理后事
        /// </summary>
        /// <param name="a">僵尸账户</param>
        public static void PushbackSubscribeJob(LoginAccountEntity acc) 
        {
            try
            {
                string where = string.Format("AccountID='{0}'", acc.AccountID);
                var param = new LoginAccountMySqlDAL.LoginAccountParameter[4];
                param[0] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.Status, (sbyte)Enums.CrawlStatus.Normal);
                param[1] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.SubscribeLastTime, DateTime.Now);
                param[2] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.SubscribeNextTime, DateTime.Now.AddMinutes(acc.SubscribeIntervalMins));
                param[3] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.SubscribeSinceID, acc.SubscribeSinceID);
                LoginAccountBusiness.UpdateWhere(param, where, null);
            }
            catch (Exception ex) 
            {
                Logger.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 获取某个僵尸账户的密码
        /// </summary>
        /// <param name="username">僵尸用户名</param>
        /// <returns>僵尸密码</returns>
        public static string GetPassword(string username)
        {
            try
            {
                string where = string.Format("UserName='{0}'", username);
                var result = LoginAccountBusiness.GetTopByWhere(where, null, LoginAccountMySqlDAL.OrderColumn.Default);
                if (result == null) return null;
                return result.Password;
            }
            catch (Exception ex) 
            {
                Logger.Error(ex.ToString());
                return null; 
            }
        }
        
        /// <summary>
        /// 取消对某个用户的关注
        /// </summary>
        /// <param name="username">关注某个用户的僵尸账号</param>
        /// <param name="targetAuthorID">需要被取消的用户ID</param>
        /// <returns>是否取消成功</returns>
        public static bool UnFollowUser(string targetAuthorID) 
        {
            string username = AuthorDBManager.GetUsernameFollowing(targetAuthorID);
            if (username == null) return false;
            string where = string.Format("UserName='{0}'", username);
            LoginAccountEntity acc = null;
            try
            {
                acc = LoginAccountBusiness.GetTopByWhere(where, null, LoginAccountMySqlDAL.OrderColumn.Default);
                if (acc == null) return false;

                try
                {
                    WeiboAPI.UnFollowUser(username, targetAuthorID);
                }
                catch (WeiboException ex) 
                {
                    Logger.Error(ex.ToString());
                    acc.FailCount++;
                    if (acc.FailCount > DefaultSettings.MaxFailureCount)
                        acc.IsDisabled = true;
                    return false;
                }
                acc.FollowCount--;
                AuthorDBManager.UnSubscribeAuthor(targetAuthorID);
                return true;
            }
            catch (Exception ex) 
            {
                Logger.Error(ex.ToString()); 
                return false; 
            }
            finally 
            {
                if (acc != null)
                {
                    var param = new LoginAccountMySqlDAL.LoginAccountParameter[3];
                    param[0] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.FollowCount, acc.FollowCount);
                    param[1] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.FailCount, acc.FailCount);
                    param[2] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.IsDisabled, acc.IsDisabled);
                    LoginAccountBusiness.UpdateWhere(param, where, null);
                }
            }
        }

        /// <summary>
        /// 定期检查账号情况
        /// </summary>
        private static void NormalizeAccount()
        {
            string where;
            try
            {
                #region 初始化账号
                where = "IsDisabled=FALSE AND SubscribeStatus>=0 AND AuthorID IS NULL";
                var result = LoginAccountBusiness.GetAllByWhere(where, null, LoginAccountMySqlDAL.OrderColumn.Default);
                foreach (var account in result)
                {
                    try
                    {
                        account.AuthorID = WeiboAPI.GetAuthorID(account.UserName);
                        var author = WeiboAPI.GetAuthorInfo(account.AuthorID);
                        var list = WeiboAPI.GetFriendsIDs(account.AuthorID, Enums.SampleMethod.All);
                        account.FollowCount = 0;
                        foreach (var id in list)
                        {
                            AuthorDBManager.SubscribeAuthor(account.UserName, id);
                            account.FollowCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(string.Format("Err@ Account: {0}, message: {1}", account.UserName, ex.ToString()));
                        account.FailCount++;
                        if (account.FailCount > DefaultSettings.MaxFailureCount)
                            account.IsDisabled = true;
                        LoginAccountBusiness.UpdateByAccountID(account);
                        continue;
                    }
                    account.FailCount = 0;
                    account.SubscribeIntervalMins = 15;
                    account.SubscribeLastTime = Utilities.Epoch;
                    account.SubscribeNextTime = Utilities.Epoch;
                    account.SubscribeSinceID = null;
                    account.FollowCountLastClearTime = DateTime.Now;
                    account.FollowCountToday = 0;
                    LoginAccountBusiness.UpdateByAccountID(account);
                }
                #endregion

                #region 关注次数和失效次数清零（每天一次）
                DateTime refreshtime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
                where = string.Format("IsDisabled=FALSE AND SubscribeStatus>=0 AND (FollowCountLastClearTime IS NULL OR FollowCountLastClearTime<'{0}')", refreshtime);
                result = LoginAccountBusiness.GetAllByWhere(where, null, LoginAccountMySqlDAL.OrderColumn.Default);
                foreach (var account in result)
                {
                    account.FollowCountLastClearTime = DateTime.Now;
                    account.FollowCountToday = 0;
                    account.FailCount = 0;
                    LoginAccountBusiness.UpdateByAccountID(account);
                }
                #endregion
            }
            catch (Exception ex) 
            {
                Logger.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 关注用户的结果，成功、异常、缺账号
        /// </summary>
        public enum FollowStatus 
        {
            Succ = 0,
            Exception = 1, 
            Shortage = 2
        }

        /// <summary>
        /// 订阅某个用户
        /// </summary>
        /// <param name="AuthorID">待订阅的用户ID</param>
        /// <returns>订阅结果枚举</returns>
        public static FollowStatus FollowUser(string AuthorID) 
        {
            string where;
            LoginAccountEntity acc = null;
            try
            {
                where = string.Format("IsDisabled=FALSE AND AuthorID IS NOT NULL AND SubscribeStatus>=0 AND FollowCountToday<{0} AND FollowCount<{1} AND SubscribeLastTime<'{2}'", DefaultSettings.FollowLimitPerDay, DefaultSettings.MaxFriendsCnt, DateTime.Now.Subtract(new TimeSpan(0, 6, 0)));
                acc = LoginAccountBusiness.GetTopByWhere(where, null, LoginAccountMySqlDAL.OrderColumn.Default);
                if (acc == null)
                    return FollowStatus.Shortage;

                try
                {
                    Console.WriteLine(string.Format("使用账号{0}进行关注", acc.UserName));
                    WeiboAPI.FollowUser(acc.UserName, AuthorID);
                }
                catch (WeiboException ex)
                {
                    switch (ex.ErrorCode)
                    {
                        case "20036":
                            Console.WriteLine("关注次数IP限制");
                            break;
                        case "20003":
                            AuthorDBManager.UnSubscribeAuthor(AuthorID);
                            Logger.Error(ex.ToString());
                            acc.FailCount++;
                            if (acc.FailCount > DefaultSettings.MaxFailureCount)
                                acc.IsDisabled = true;
                            Console.WriteLine("出现\"用户不存在\"异常，已取消关注该用户，请手工确保账号有效");
                            break;
                        default:
                            acc.FailCount++;
                            if (acc.FailCount > DefaultSettings.MaxFailureCount)
                                acc.IsDisabled = true;
                            acc.FollowCountToday = DefaultSettings.FollowLimitPerDay;
                            Logger.Error(ex.ToString());
                            break;
                    }
                    return FollowStatus.Exception;
                }
                catch (Exception ex) 
                {
                    Logger.Error(ex.ToString()); return FollowStatus.Exception;
                }
                acc.FollowCount++;
                acc.FollowCountToday++;
                acc.SubscribeLastTime = DateTime.Now;

                AuthorDBManager.SubscribeAuthor(acc.UserName, AuthorID);
                return FollowStatus.Succ;
            }
            catch (Exception ex) { Logger.Error(ex.ToString()); return FollowStatus.Exception; }
            finally 
            {
                if (acc != null) 
                {
                    where = string.Format("AccountID='{0}'", acc.AccountID);
                    var param = new LoginAccountMySqlDAL.LoginAccountParameter[5];
                    param[0] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.FollowCount, acc.FollowCount);
                    param[1] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.FollowCountToday, acc.FollowCountToday);
                    param[2] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.FailCount, acc.FailCount);
                    param[3] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.IsDisabled, acc.IsDisabled);
                    param[4] = new LoginAccountMySqlDAL.LoginAccountParameter(Palas.Common.Lib.DTO.LoginAccountDTO.Column.SubscribeLastTime, acc.SubscribeLastTime);
                    LoginAccountBusiness.UpdateWhere(param, where, null);
                }
            }
        }

        private static DateTime lastNormalizeTime = Utilities.Epoch;
        public static string GetNextFollowJob()
        {
            if (DateTime.Now - lastNormalizeTime > new TimeSpan(1, 0, 0))
            {
                NormalizeAccount();
                lastNormalizeTime = DateTime.Now;
            }

            return AuthorDBManager.GetAuthorToBeFollowed();
        }
    }
}
