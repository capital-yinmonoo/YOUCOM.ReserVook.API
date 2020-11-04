using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Services
{
    public class SiteConvertService : ISiteConvertService
    {

        private DBContext _context;

        public SiteConvertService(DBContext context)
        {
            _context = context;
        }

        // 情報取得(画面表示用)
        public async Task<List<FrMScSiteConvertInfo>> GetSiteConvertList(FrMScSiteConvertInfo siteConvertInfo)
        {
            string sql = "SELECT siteConvert.*, agent.agent_name, scName.content_1";
            sql += " FROM fr_m_sc_site_convert siteConvert";

            sql += " LEFT JOIN fr_m_sc_nm scName";
            sql += " ON siteConvert.company_no =  scName.company_no";
            sql += " AND siteConvert.sc_cd = scName.sc_seg_cd";

            sql += " LEFT JOIN mst_agent agent";
            sql += " ON siteConvert.company_no = agent.company_no";
            sql += " AND siteConvert.travel_agnc_cd = agent.agent_code";

            sql += " WHERE siteConvert.company_no = '" + siteConvertInfo.CompanyNo + "'";
            sql += " AND siteConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY siteConvert.sc_cd ASC, siteConvert.sc_site_cd ASC";

            var lists = new List<FrMScSiteConvertInfo>();
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
                            var list = new FrMScSiteConvertInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ScCd = reader["sc_cd"].ToString();
                            list.ScSiteCd = reader["sc_site_cd"].ToString();
                            list.ScSiteNm = reader["sc_site_nm"].ToString();
                            list.TravelAgncCd = reader["travel_agnc_cd"].ToString();
                            list.ScPositionMan = reader["sc_position_man"].ToString();
                            list.ScPositionWoman = reader["sc_position_woman"].ToString();
                            list.ScPositionChildA = reader["sc_position_child_a"].ToString();
                            list.ScPositionChildB = reader["sc_position_child_b"].ToString();
                            list.ScPositionChildC = reader["sc_position_child_c"].ToString();
                            list.ScPositionChildD = reader["sc_position_child_d"].ToString();
                            list.ScPositionChildE = reader["sc_position_child_e"].ToString();
                            list.ScPositionChildF = reader["sc_position_child_f"].ToString();
                            list.ScPositionChildOther = reader["sc_position_child_other"].ToString();
                            list.ScPersonCalcSeg = reader["sc_person_calc_seg"].ToString();
                            list.UpdateCnt = int.Parse(reader["update_cnt"].ToString());
                            list.ProgramId = reader["program_id"].ToString();
                            list.CreateClerk = reader["create_clerk"].ToString();
                            list.CreateMachineNo = reader["create_machine_no"].ToString();
                            list.CreateMachine = reader["create_machine"].ToString();
                            list.CreateDatetime = reader["create_datetime"].ToString();
                            list.UpdateClerk = reader["update_clerk"].ToString();
                            list.UpdateMachineNo = reader["update_machine_no"].ToString();
                            list.UpdateMachine = reader["update_machine"].ToString();
                            list.UpdateDatetime = reader["update_datetime"].ToString();
                            list.Status = reader["status"].ToString();

                            list.CdName = reader["content_1"].ToString();
                            list.TravelAgncName = reader["agent_name"].ToString();

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

        // 情報取得(編集用)
        public async Task<FrMScSiteConvertInfo> GetSiteConvertById(FrMScSiteConvertInfo siteConvertInfo)
        {
            var list = new FrMScSiteConvertInfo();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT siteConvert.*";
            sql += " FROM fr_m_sc_site_convert siteConvert";
            sql += " WHERE siteConvert.company_no = '" + siteConvertInfo.CompanyNo + "'";
            sql += " AND siteConvert.sc_site_cd = '" + siteConvertInfo.ScSiteCd + "'";
            sql += " AND siteConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";

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
                        list.ScSiteCd = reader["sc_site_cd"].ToString();
                        list.ScSiteNm = reader["sc_site_nm"].ToString();
                        list.TravelAgncCd = reader["travel_agnc_cd"].ToString();
                        list.ScPositionMan = reader["sc_position_man"].ToString();
                        list.ScPositionWoman = reader["sc_position_woman"].ToString();
                        list.ScPositionChildA = reader["sc_position_child_a"].ToString();
                        list.ScPositionChildB = reader["sc_position_child_b"].ToString();
                        list.ScPositionChildC = reader["sc_position_child_c"].ToString();
                        list.ScPositionChildD = reader["sc_position_child_d"].ToString();
                        list.ScPositionChildE = reader["sc_position_child_e"].ToString();
                        list.ScPositionChildF = reader["sc_position_child_f"].ToString();
                        list.ScPositionChildOther = reader["sc_position_child_other"].ToString();
                        list.ScPersonCalcSeg = reader["sc_person_calc_seg"].ToString();
                        list.UpdateCnt = int.Parse(reader["update_cnt"].ToString());
                        list.ProgramId = reader["program_id"].ToString();
                        list.CreateClerk = reader["create_clerk"].ToString();
                        list.CreateMachineNo = reader["create_machine_no"].ToString();
                        list.CreateMachine = reader["create_machine"].ToString();
                        list.CreateDatetime = reader["create_datetime"].ToString();
                        list.UpdateClerk = reader["update_clerk"].ToString();
                        list.UpdateMachineNo = reader["update_machine_no"].ToString();
                        list.UpdateMachine = reader["update_machine"].ToString();
                        list.UpdateDatetime = reader["update_datetime"].ToString();
                        list.Status = reader["status"].ToString();
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

            return list;
        }

        // 情報追加
        public async Task<int> AddSiteConvert(FrMScSiteConvertInfo siteConvertInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;
            // 会社番号,SCコード,SCサイトコードの一致するデータを取得
            var info = _context.SiteConvertInfo.Where(w => w.CompanyNo == siteConvertInfo.CompanyNo && w.ScCd == siteConvertInfo.ScCd && w.ScSiteCd == siteConvertInfo.ScSiteCd).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                siteConvertInfo.UpdateCnt = 0;
                siteConvertInfo.CreateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                siteConvertInfo.CreateMachine = Environment.MachineName;
                siteConvertInfo.UpdateDatetime = siteConvertInfo.CreateDatetime;
                siteConvertInfo.UpdateMachine = siteConvertInfo.CreateMachine;
                siteConvertInfo.Status = Context.CommonConst.STATUS_USED;

                _context.SiteConvertInfo.Add(siteConvertInfo);
                _context.SaveChanges();
            }
            else if (info.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」の場合 → 更新
                bool addFlag = true;
                siteConvertInfo.UpdateCnt = info.UpdateCnt;
                siteConvertInfo.CreateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                siteConvertInfo.CreateMachine = Environment.MachineName;
                var updateInfo = await UpdateSiteConvert(siteConvertInfo, addFlag);
            }
            else
            {
                // データが存在し,Statusが「1」の場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        // 情報更新
        public async Task<int> UpdateSiteConvert(FrMScSiteConvertInfo siteConvertInfo, bool addFlag)
        {
            try
            {
                // 会社番号,SCコード,SCサイトコードの一致するデータを取得
                var info = _context.SiteConvertInfo.Where(w => w.CompanyNo == siteConvertInfo.CompanyNo && w.ScCd == siteConvertInfo.ScCd && w.ScSiteCd == siteConvertInfo.ScSiteCd).AsNoTracking().SingleOrDefault();

                if (!addFlag)
                {
                    // バージョンチェック
                    if (!await SiteConvertCheckVer(siteConvertInfo)) { return -1; }
                    siteConvertInfo.CreateClerk = info.CreateClerk;
                    siteConvertInfo.CreateDatetime = info.CreateDatetime;
                    siteConvertInfo.CreateMachine = info.CreateMachine;
                }
                siteConvertInfo.UpdateCnt++;
                siteConvertInfo.ProgramId = info.ProgramId;
                siteConvertInfo.CreateMachineNo = info.CreateMachineNo;
                siteConvertInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                siteConvertInfo.UpdateMachineNo = info.UpdateMachineNo;
                siteConvertInfo.UpdateMachine = Environment.MachineName;
                siteConvertInfo.Status = Context.CommonConst.STATUS_USED;

                _context.SiteConvertInfo.Update(siteConvertInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // 情報削除
        public async Task<int> DelSiteConvert(FrMScSiteConvertInfo siteConvertInfo)
        {
            try
            {
                // バージョンチェック
                if (!await SiteConvertCheckVer(siteConvertInfo)) { return -1; }

                siteConvertInfo.UpdateCnt++;
                siteConvertInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                siteConvertInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.SiteConvertInfo.Update(siteConvertInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // バージョンチェック
        private async Task<bool> SiteConvertCheckVer(FrMScSiteConvertInfo siteConvertInfo)
        {
            try
            {
                // キーセット
                FrMScSiteConvertInfo keyInfo = new FrMScSiteConvertInfo() { CompanyNo = siteConvertInfo.CompanyNo, ScCd = siteConvertInfo.ScCd, ScSiteCd = siteConvertInfo.ScSiteCd };

                // データ取得
                var info = await GetSiteConvertById(keyInfo);

                // バージョン差異チェック
                if (siteConvertInfo.UpdateCnt != info.UpdateCnt)
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

        // サイト名取得(別ページ用)
        public async Task<List<FrMScSiteConvertInfo>> GetSiteCodeList(string companyNo, string scCd)
        {
            return _context.SiteConvertInfo
                    .Where(d => d.CompanyNo == companyNo && d.ScCd == scCd && d.Status == CommonConst.STATUS_USED)
                    .OrderBy(d => d.ScSiteNm).ToList();

        }

        // 削除チェック
        public async Task<int> DeleteSiteCdCheck(FrMScSiteConvertInfo siteConvertInfo)
        {            
            // 支払方法変換マスタで使用中のデータ
            int countPayment = _context.PaymentConvertInfo.Count(w => w.CompanyNo == siteConvertInfo.CompanyNo && w.ScSiteCd == siteConvertInfo.ScSiteCd && w.Status == CommonConst.STATUS_USED);

            // ポイント変換マスタで使用中のデータ
            int countPoint = _context.PointConvertInfo.Count(w => w.CompanyNo == siteConvertInfo.CompanyNo && w.ScSiteCd == siteConvertInfo.ScSiteCd && w.Status == CommonConst.STATUS_USED);

            if(countPayment > 0)
            {
                // 削除不可(支払方法変換マスタで使用中)
                return 1;
            }
            else if (countPoint > 0)
            {
                // 削除不可(ポイント変換マスタで使用中)
                return 2;
            }
            else
            {
                // 削除可
                return 0;
            }
        }
    }
}