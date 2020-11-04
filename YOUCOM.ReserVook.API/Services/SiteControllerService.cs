using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.Commons.Extensions;
using System.Collections.Generic;

namespace YOUCOM.ReserVook.API.Services
{
    public class SiteControllerService : ISiteControllerService
    {

        private DBContext _context;

        public SiteControllerService(DBContext context)
        {
            _context = context;
        }

        // 情報取得(一覧用)
        public async Task<List<FrMScSiteControllerInfo>> GetSiteControllerList(FrMScSiteControllerInfo siteControllerInfo) {

            string sql = "SELECT siteController.*, content_1";
            sql += " FROM fr_m_sc_site_controller siteController";

            sql += " LEFT JOIN fr_m_sc_nm scName";
            sql += " ON siteController.company_no =  scName.company_no";
            sql += " AND siteController.sc_cd = scName.sc_seg_cd";

            sql += " WHERE siteController.company_no = '" + siteControllerInfo.CompanyNo + "'";

            var list = new List<FrMScSiteControllerInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand()) {
                command.CommandText = sql;
                _context.Database.OpenConnection();

                try {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {

                            var info = new FrMScSiteControllerInfo();
                            info.CompanyNo = reader["company_no"].ToString();
                            info.ScCd = reader["sc_cd"].ToString();
                            info.ScUseFlg = reader["sc_use_flg"].ToString();
                            info.ScSystemId = reader["sc_system_id"].ToString();
                            info.ScUsrId = reader["sc_usr_id"].ToString();
                            info.ScUsrPassword = reader["sc_usr_password"].ToString();
                            info.ScNewRcvFlg = reader["sc_new_rcv_flg"].ToString();
                            info.ScCancellationRcvFlg = reader["sc_cancellation_rcv_flg"].ToString();
                            info.ScReservationRcvUrl = reader["sc_reservation_rcv_url"].ToString();
                            info.ScReservationRcvCompUrl = reader["sc_reservation_rcv_comp_url"].ToString();
                            info.UpdateCnt = int.Parse(reader["update_cnt"].ToString());
                            info.CreateMachine = reader["create_machine"].ToString();
                            info.CreateClerk = reader["create_clerk"].ToString();
                            info.CreateDatetime = reader["create_datetime"].ToString();
                            info.UpdateMachine = reader["update_machine"].ToString();
                            info.UpdateClerk = reader["update_clerk"].ToString();
                            info.UpdateDatetime = reader["update_datetime"].ToString();

                            info.CdName = reader["content_1"].ToString();
                            if (info.ScUseFlg == "0") {
                                info.DispScUseFlg = "利用しない";
                            } else {
                                info.DispScUseFlg = "利用する";
                            }
                            if (info.ScNewRcvFlg == "0") {
                                info.DispScNewRcvFlg = "取り込まない";
                            } else {
                                info.DispScNewRcvFlg = "取り込む";
                            }
                            if (info.ScCancellationRcvFlg == "0") {
                                info.DispScCancellationRcvFlg = "取り込まない";
                            } else {
                                info.DispScCancellationRcvFlg = "取り込む";
                            }

                            list.Add(info);
                        }
                    }
                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                } finally {
                    _context.Database.CloseConnection();
                }
            }
            return list;
        }

        // 情報取得(編集用)
        public async Task<FrMScSiteControllerInfo> GetSiteControllerById(FrMScSiteControllerInfo siteControllerInfo)
        {
            string sql = "SELECT siteController.*, content_1";
            sql += " FROM fr_m_sc_site_controller siteController";

            sql += " LEFT JOIN fr_m_sc_nm scName";
            sql += " ON siteController.company_no =  scName.company_no";
            sql += " AND siteController.sc_cd = scName.sc_seg_cd";

            sql += " WHERE siteController.company_no = '" + siteControllerInfo.CompanyNo + "'";
            sql += " AND siteController.sc_cd = '" + siteControllerInfo.ScCd + "'";

            var list = new FrMScSiteControllerInfo();
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
                            list.ScCd = reader["sc_cd"].ToString();
                            list.ScUseFlg = reader["sc_use_flg"].ToString();
                            list.ScSystemId = reader["sc_system_id"].ToString();
                            list.ScUsrId = reader["sc_usr_id"].ToString();
                            list.ScUsrPassword = reader["sc_usr_password"].ToString();
                            list.ScNewRcvFlg = reader["sc_new_rcv_flg"].ToString();
                            list.ScCancellationRcvFlg = reader["sc_cancellation_rcv_flg"].ToString();
                            list.ScReservationRcvUrl = reader["sc_reservation_rcv_url"].ToString();
                            list.ScReservationRcvCompUrl = reader["sc_reservation_rcv_comp_url"].ToString();
                            list.UpdateCnt = int.Parse(reader["update_cnt"].ToString());
                            list.UpdateMachine = reader["update_machine"].ToString();
                            list.UpdateClerk = reader["update_clerk"].ToString();
                            list.UpdateDatetime = reader["update_datetime"].ToString();

                            list.CdName = reader["content_1"].ToString();
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

        // 情報更新
        public async Task<int> UpdateSiteController(FrMScSiteControllerInfo siteControllerInfo)
        {
            try
            {
                // versionチェック                
                if (!await SiteControllerCheckVer(siteControllerInfo)) { return -1; }

                siteControllerInfo.UpdateCnt++;
                siteControllerInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                siteControllerInfo.UpdateMachine = Environment.MachineName;

                int count = await SaveSiteController(siteControllerInfo);

                return count;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // バージョンチェック
        private async Task<bool> SiteControllerCheckVer(FrMScSiteControllerInfo siteControllerInfo)
        {
            try
            {
                // キーセット
                FrMScSiteControllerInfo keyInfo = new FrMScSiteControllerInfo() { CompanyNo = siteControllerInfo.CompanyNo, ScCd = siteControllerInfo.ScCd };

                // データ取得
                var info = await GetSiteControllerById(keyInfo);

                // バージョン差異チェック
                if (siteControllerInfo.UpdateCnt != info.UpdateCnt)
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

        // 保存
        public async Task<int> SaveSiteController(FrMScSiteControllerInfo siteControllerInfo)
        {
            string singleQuoCheckPass = SqlUtils.GetStringWithSqlEscaped(siteControllerInfo.ScUsrPassword);
            string singleQuoCheckUrl = SqlUtils.GetStringWithSqlEscaped(siteControllerInfo.ScReservationRcvUrl);
            string singleQuoCheckCompUrl = SqlUtils.GetStringWithSqlEscaped(siteControllerInfo.ScReservationRcvCompUrl);
            int count = 0;
            string sql = "UPDATE fr_m_sc_site_controller";
            sql += " SET sc_use_flg = '{0}'";
            sql += " ,update_cnt = '{1}'";
            sql += " ,update_machine = '{2}'";
            sql += " ,update_clerk = '{3}'";
            sql += " ,update_datetime = '{4}'";

            if (siteControllerInfo.ScUseFlg == "1")
            {
                sql += " ,sc_system_id = '{5}'";
                sql += " ,sc_usr_id = '{6}'";
                sql += " ,sc_usr_password = '{7}'";
                sql += " ,sc_new_rcv_flg = '{8}'";
                sql += " ,sc_cancellation_rcv_flg = '{9}'";
                sql += " ,sc_reservation_rcv_url = '{10}'";
                sql += " ,sc_reservation_rcv_comp_url = '{11}'";
            }
            sql += " WHERE company_no = '{12}'";
            sql += " AND fr_m_sc_site_controller.sc_cd = '{13}'";

            sql = sql.FillFormat(siteControllerInfo.ScUseFlg, siteControllerInfo.UpdateCnt.ToString(),siteControllerInfo.UpdateMachine, 
                                 siteControllerInfo.UpdateClerk,siteControllerInfo.UpdateDatetime, siteControllerInfo.ScSystemId, 
                                 siteControllerInfo.ScUsrId, singleQuoCheckPass, siteControllerInfo.ScNewRcvFlg, 
                                 siteControllerInfo.ScCancellationRcvFlg, singleQuoCheckUrl, singleQuoCheckCompUrl, siteControllerInfo.CompanyNo, siteControllerInfo.ScCd);

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
    }
}