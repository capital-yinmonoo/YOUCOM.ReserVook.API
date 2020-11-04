using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;
using YOUCOM.ReserVook.API.Tools;
using YOUCOM.Commons.Extensions;

namespace YOUCOM.ReserVook.API.Services
{
    public class UserService : IUserService
    {
        private DBContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _memoryCacheLoginUsers;
        private readonly IMemoryCache _tryLoginInfos;

        private enum ResultValue
        {
            Success,
            ErrorTryCountOver,
            ErrorTryCountUnder,
            ErrorUnmatch,
            ErrorAuthorization,
            UntilAccountLock,
            UntilOtherMachineLogin
        }

        public UserService(DBContext context,
            IOptions<AppSettings> appSettings,
            IMemoryCache memoryCacheLoginUsers,
            IMemoryCache tryLoginInfos)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _memoryCacheLoginUsers = memoryCacheLoginUsers;
            _tryLoginInfos = tryLoginInfos;
        }

        #region マスタ関係
        // 情報取得(画面表示用)
        public async Task<List<MstUserInfo>> GetList()
        {
            string sql = "SELECT users.*, role.code_name, lost.code_name as lostflg_name, company.company_name";
            sql += " FROM mst_user users";

            sql += " LEFT JOIN mst_code_name role";
            sql += " ON role.company_no = users.company_no";
            sql += " AND role.division_code ='" + ((int)CommonEnum.CodeDivision.Role).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += " AND role.code = users.role_division";

            sql += " LEFT JOIN mst_company company";
            sql += " ON company.company_no = users.company_no";

            sql += " LEFT JOIN mst_code_name lost";
            sql += " ON lost.company_no = users.company_no";
            sql += " AND lost.division_code ='" + ((int)CommonEnum.CodeDivision.CleanRoomsManager).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += " AND lost.code = users.lost_flg";

            sql += " WHERE users.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY users.company_no ASC, users.role_division ASC ,users.user_email ASC";

            var lists = new List<MstUserInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _context.Database.OpenConnection();

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var list = new MstUserInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.UserEmail = reader["user_email"].ToString();
                            list.Password = reader["password"].ToString();
                            list.UserName = reader["user_name"].ToString();
                            list.RoleDivision = reader["role_division"].ToString();
                            list.LostFlg = reader["lost_flg"].ToString();
                            list.Status = reader["status"].ToString();
                            list.Version = int.Parse(reader["version"].ToString());
                            list.Creator = reader["creator"].ToString();
                            list.Updator = reader["updator"].ToString();
                            list.Cdt = reader["cdt"].ToString();
                            list.Udt = reader["udt"].ToString();

                            list.RoleName = reader["code_name"].ToString();
                            list.CompanyName = reader["company_name"].ToString();
                            list.LostFlgName = reader["lostflg_name"].ToString();

                            lists.Add(list);
                        }
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    _context.Database.CloseConnection();
                }
            }
            return lists;
        }

        // 情報取得(編集・削除用)
        public async Task<MstUserInfo> GetById(MstUserInfo userInfo)
        {
            string sql = "SELECT users.*";
            sql += " FROM mst_user users";
            sql += " WHERE users.company_no = '" + userInfo.CompanyNo + "'";
            sql += " AND users.user_email = '" + userInfo.UserEmail + "'";
            sql += " AND users.status <> '" + CommonConst.STATUS_UNUSED + "'";


            var list = new MstUserInfo();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _context.Database.OpenConnection();

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.CompanyNo = reader["company_no"].ToString();
                            list.UserEmail = reader["user_email"].ToString();
                            list.Password = reader["password"].ToString();
                            list.Password = MD5.Decrypt(list.Password);
                            list.UserName = reader["user_name"].ToString();
                            list.RoleDivision = reader["role_division"].ToString();
                            list.LostFlg = reader["lost_flg"].ToString();
                            list.Status = reader["status"].ToString();
                            list.Version = int.Parse(reader["version"].ToString());
                            list.Creator = reader["creator"].ToString();
                            list.Updator = reader["updator"].ToString();
                            list.Cdt = reader["cdt"].ToString();
                            list.Udt = reader["udt"].ToString();
                        }
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    _context.Database.CloseConnection();
                }
            }

            return list;
        }

        // 追加
        public async Task<int> AddUser(MstUserInfo userInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;
            // 会社番号,コードが一致するデータを取得
            var info = _context.UserInfo.Where(w => w.CompanyNo == userInfo.CompanyNo && w.UserEmail == userInfo.UserEmailOrigin).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                userInfo.Password = MD5.Encrypt(userInfo.Password);
                userInfo.Version = 0;
                userInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                userInfo.Udt = userInfo.Cdt;
                userInfo.Status = Context.CommonConst.STATUS_USED;

                _context.UserInfo.Add(userInfo);
                _context.SaveChanges();
            }
            else if (info.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」場合 → 更新
                bool addFlag = true;
                userInfo.Version = info.Version;
                userInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                userInfo.Status = Context.CommonConst.STATUS_USED;
                var updateInfo = await UpdateUser(userInfo, addFlag);
            }
            else
            {
                // データが存在し,Statusが「1」場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        // 更新
        public async Task<int> UpdateUser(MstUserInfo userInfo, bool addFlag)
        {
            try
            {
                // versionチェック
                if (!addFlag)
                {
                    if (!await UserCheckVer(userInfo)) { return -1; }

                    // メールアドレスを変更していなければ条件確認
                    if (userInfo.UserEmail != userInfo.UserEmailOrigin)
                    {
                        {
                            // 更新データのPKと同じPK(Statusが使用)を持つデータを取得する
                            int countOverUsed = _context.UserInfo.Count(w => w.CompanyNo == userInfo.CompanyNo && w.UserEmail == userInfo.UserEmail && w.Status == CommonConst.STATUS_USED);

                            if (countOverUsed > 0)
                            {
                                return -3;
                            }

                            // 更新データのPKと同じPK(Statusが未使用)を持つデータを取得する
                            int countOverUnUsed = _context.UserInfo.Count(w => w.CompanyNo == userInfo.CompanyNo && w.UserEmail == userInfo.UserEmail && w.Status == CommonConst.STATUS_UNUSED);
                            if (countOverUnUsed > 0)
                            {
                                var delInfo = _context.UserInfo.Single(d => d.CompanyNo == userInfo.CompanyNo && d.UserEmail == userInfo.UserEmail);
                                _context.UserInfo.Remove(delInfo);
                                _context.SaveChanges();
                            }
                        }
                    }
                    
                }
                
                userInfo.Password = MD5.Encrypt(userInfo.Password);
                userInfo.Version++;
                userInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                int count = await SaveUser(userInfo);
                return count;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // 削除
        public async Task<int> DelUser(MstUserInfo userInfo)
        {
            try
            {
                // versionチェック
                if (!await UserCheckVer(userInfo)) { return -1; }

                userInfo.Version++;
                userInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                userInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.UserInfo.Update(userInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        // バージョンチェック
        private async Task<bool> UserCheckVer(MstUserInfo userInfo)
        {
            try
            {
                // データ取得
                var info = _context.UserInfo.Where(w => w.UserEmail == userInfo.UserEmailOrigin).AsNoTracking().SingleOrDefault();

                // バージョン差異チェック
                if (userInfo.Version != info.Version)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        // ユーザー情報保存
        private async Task<int> SaveUser(MstUserInfo userInfo)
        {
            string singleQuoCheckPass = SqlUtils.GetStringWithSqlEscaped(userInfo.Password);
            string singleQuoCheckName = SqlUtils.GetStringWithSqlEscaped(userInfo.UserName);
            string singleQuoCheckCreator = SqlUtils.GetStringWithSqlEscaped(userInfo.Creator);
            string singleQuoCheckUpdator = SqlUtils.GetStringWithSqlEscaped(userInfo.Updator);
            int count = 0;
            string sql = "UPDATE mst_user";
            sql += " SET company_no = '{0}'";
            sql += " ,user_email = '{1}'";
            sql += " ,password = '{2}'";
            sql += " ,user_name = '{3}'";
            sql += " ,role_division = '{4}'";
            sql += " ,lost_flg = '{5}'";
            sql += " ,status = '{6}'";
            sql += " ,version = '{7}'";
            sql += " ,creator = '{8}'";
            sql += " ,updator = '{9}'";
            sql += " ,cdt = '{10}'";
            sql += " ,udt = '{11}'";

            sql += " WHERE company_no = '{0}'";
            sql += " AND mst_user.user_email = '{12}'";

            sql = sql.FillFormat(userInfo.CompanyNo, userInfo.UserEmail, singleQuoCheckPass, singleQuoCheckName, userInfo.RoleDivision, userInfo.LostFlg,
                                 userInfo.Status, userInfo.Version.ToString(), singleQuoCheckCreator, singleQuoCheckUpdator, userInfo.Cdt, userInfo.Udt, userInfo.UserEmailOrigin);

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _context.Database.OpenConnection();
                try
                {
                    count = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    _context.Database.CloseConnection();
                }
            }
            return count;
        }

        #endregion


        #region ログイン関係
        /// <summary>
        /// ログイン認証
        /// </summary>
        /// <param name="email">メールアドレス</param>
        /// <param name="pwd">パスワード(Hash)</param>
        /// <returns></returns>
        public LoginUser Login(string email, string pwd) {
            LoginUser loginUser;
            ResultValue resultValue = ResultValue.Success;

            var user = _context.UserInfo.Where(w => w.UserEmail == email && w.Password == pwd && w.Status != CommonConst.STATUS_UNUSED).SingleOrDefault();
            if (user == null) {
                loginUser = new LoginUser();
                loginUser.LoginResult = LoginFailed(email, out resultValue);
                loginUser.LoginResultValue = (int)resultValue;
                return loginUser;
            } else if (user.Status == CommonConst.STATUS_ACCOUNT_LOCK) {
                loginUser = new LoginUser();
                loginUser.LoginResult = "このアカウントはシステムによりロックされています。";
                loginUser.LoginResultValue = (int)ResultValue.UntilAccountLock;
                return loginUser;
            }

            var company = _context.CompanyInfo.Where(w => w.CompanyNo == user.CompanyNo && w.Status != CommonConst.STATUS_UNUSED).SingleOrDefault();
            if (company == null) {
                LoginFailed(email, out resultValue);
                loginUser = new LoginUser();
                loginUser.LoginResult = LoginFailed(email, out resultValue);
                loginUser.LoginResultValue = (int)resultValue;
                return loginUser;
            }

            loginUser = user.ToObject<LoginUser>();
            loginUser.CompanyInfo = company;

            try {
                var loginUserModel = new LoginUserMemoryCacheModel();
                _memoryCacheLoginUsers.TryGetValue(loginUser.UserEmail, out loginUserModel);

                if (loginUserModel == null) {
                    // 下記処理で _memoryCacheLoginUsers もRemoveされる
                    // ログイン成功でログイン試行メモリからは削除する
                    _tryLoginInfos.Remove(email);

                    // authentication successful so generate jwt token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, loginUser.UserEmail.ToString())
                        }),
                        Expires = DateTime.Now.AddDays(_appSettings.TokenExpiresDays),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    loginUser.JwtToken = tokenHandler.WriteToken(token);

                    //ログイン情報をキャッシュに保持
                    if (loginUserModel == null) {
                        loginUserModel = new LoginUserMemoryCacheModel();
                        _memoryCacheLoginUsers.Set(loginUser.UserEmail, loginUserModel);
                    }

                    loginUserModel.LastDateTime = DateTime.Now;
                } else {
                    //キャッシュに既にログイン情報がある場合、多重ログインとする
                    loginUser.LoginResult = "該当ユーザーは他の端末でログイン済です。";
                    loginUser.LoginResultValue = (int)ResultValue.UntilOtherMachineLogin;
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                loginUser = new LoginUser();
                loginUser.LoginResult = "ログイン認証に失敗しました。";
                loginUser.LoginResultValue = (int)ResultValue.ErrorAuthorization;
            }

            return loginUser;

        }

        /// <summary>
        /// ログイン失敗処理
        /// </summary>
        /// <param name="email"></param>
        private string LoginFailed(string email, out ResultValue resultValue) {
            const int TRY_COUNT = 5;

            try {
                var loginInfo = new LoginInfo();
                _tryLoginInfos.TryGetValue(email, out loginInfo);

                if (loginInfo != null) {
                    if (loginInfo.TryCount >= TRY_COUNT) {
                        //TRY_COUNT回、ログインに失敗した場合アカウントをロックする
                        var userinfo = _context.UserInfo.Where(w => w.UserEmail == email &&
                                                               !(w.Status == CommonConst.STATUS_UNUSED || w.Status == CommonConst.STATUS_ACCOUNT_LOCK))
                                                        .SingleOrDefault();
                        if (userinfo != null) {
                            userinfo.Status = CommonConst.STATUS_ACCOUNT_LOCK;
                            userinfo.Updator = CommonConst.SYSTEM_USER_NAME;
                            userinfo.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            _context.UserInfo.Update(userinfo);
                            _context.SaveChanges();
                        }

                        resultValue = ResultValue.ErrorTryCountOver;
                        return "ログイン試行回数が" + TRY_COUNT + "回を超えたため、\nアカウントはロックされました。";
                    } else {
                        loginInfo.TryCount++;
                        _tryLoginInfos.Remove(email);
                        _tryLoginInfos.Set(email, loginInfo);

                        resultValue = ResultValue.ErrorTryCountUnder;
                        return "メールアドレスとパスワードが一致しません。\n5回間違えた場合アカウントはロックされます。";
                    }
                } else {
                    loginInfo = new LoginInfo();
                    loginInfo.UserEmail = email;
                    loginInfo.TryCount = 1;
                    _tryLoginInfos.Set(email, loginInfo);

                    resultValue = ResultValue.ErrorUnmatch;
                    return "メールアドレスとパスワードが一致しません。";
                }

            } catch (Exception ex) {
                Console.WriteLine(ex.Message);

                resultValue = ResultValue.ErrorAuthorization;
                return "ログイン認証に失敗しました。";
            }

        }

        /// <summary>
        /// ログアウト処理
        /// </summary>
        /// <param name="id">ログアウトユーザのメールアドレス</param>
        /// <returns></returns>
        public async Task<bool> Logout(string id) {
            try {
                //ログイン管理キャッシュからユーザを削除
                _memoryCacheLoginUsers.Remove(id);
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        #endregion
    }
}
