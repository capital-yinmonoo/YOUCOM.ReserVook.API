using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.Commons.Extensions;

namespace YOUCOM.ReserVook.API.Services
{
    public class PointConvertService : IPointConvertService
    {

        private DBContext _context;

        public PointConvertService(DBContext context)
        {
            _context = context;
        }

        // 情報取得(画面表示用)
        public async Task<List<FrMScPointConvertInfo>> GetPointConvertList(FrMScPointConvertInfo pointConvertInfo)
        {
            string sql = "SELECT pointConvert.*, scName.content_1, siteConvert.sc_site_nm, denomination.denomination_name";
            sql += " FROM fr_m_sc_point_convert pointConvert";

            sql += " LEFT JOIN fr_m_sc_nm scName";
            sql += " ON pointConvert.company_no = scName.company_no";
            sql += " AND pointConvert.sc_cd = scName.sc_seg_cd";

            sql += " LEFT JOIN fr_m_sc_site_convert siteConvert";
            sql += " ON siteConvert.company_no = pointConvert.company_no";
            sql += " AND siteConvert.sc_cd = pointConvert.sc_cd";
            sql += " AND siteConvert.sc_site_cd = pointConvert.sc_site_cd";

            sql += " LEFT JOIN mst_denomination denomination";
            sql += " ON denomination.company_no = pointConvert.company_no";
            sql += " AND denomination.denomination_code = pointConvert.denomination_code";

            sql += " WHERE pointConvert.company_no = '" + pointConvertInfo.CompanyNo + "'";
            sql += " AND pointConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY pointConvert.sc_cd ASC, siteConvert.sc_site_nm ASC, pointConvert.sc_pnts_discnt_nm";

            var lists = new List<FrMScPointConvertInfo>();
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
                            var list = new FrMScPointConvertInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ScCd = reader["sc_cd"].ToString();
                            list.ScSiteCd = reader["sc_site_cd"].ToString();
                            list.ScPntsDiscntNm = reader["sc_pnts_discnt_nm"].ToString();
                            list.DenominationCode = reader["denomination_code"].ToString();
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
                            list.SiteCdName = reader["sc_site_nm"].ToString();
                            if (list.ScPntsDiscntNm == CommonConst.POINTS_DISCOUNTNAME_BLANK)
                            {
                                list.PntsDiscntName = "";
                            }
                            else
                            {
                                list.PntsDiscntName = list.ScPntsDiscntNm;
                            }
                            list.Denomination = reader["denomination_name"].ToString();

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
        public async Task<FrMScPointConvertInfo> GetPointConvertById(FrMScPointConvertInfo pointConvertInfo)
        {
            string singleQuoCheck = SqlUtils.GetStringWithSqlEscaped(pointConvertInfo.ScPntsDiscntNm);
            var list = new FrMScPointConvertInfo();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT pointConvert.*";
            sql += " FROM fr_m_sc_point_convert pointConvert";
            sql += " WHERE pointConvert.company_no = '{0}'";
            sql += " AND pointConvert.sc_cd = '{1}'";
            sql += " AND pointConvert.sc_site_cd = '{2}'";
            sql += " AND pointConvert.sc_pnts_discnt_nm = '{3}'";
            sql += " AND pointConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";

            sql = sql.FillFormat(pointConvertInfo.CompanyNo, pointConvertInfo.ScCd, pointConvertInfo.ScSiteCd, singleQuoCheck);

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
                        list.ScPntsDiscntNm = reader["sc_pnts_discnt_nm"].ToString();
                        list.DenominationCode = reader["denomination_code"].ToString();
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
                        if (list.ScPntsDiscntNm == CommonConst.POINTS_DISCOUNTNAME_BLANK)
                        {
                            list.PntsDiscntName = " ";
                        }
                        else
                        {
                            list.PntsDiscntName = list.ScPntsDiscntNm;
                        }
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
        public async Task<int> AddPointConvert(FrMScPointConvertInfo pointConvertInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;

            // 会社番号,SCコード,SCサイトコード,SCポイント割引・補助金名称の一致するデータを取得
            var info = _context.PointConvertInfo.Where(w => w.CompanyNo == pointConvertInfo.CompanyNo && w.ScCd == pointConvertInfo.ScCd && w.ScSiteCd == pointConvertInfo.ScSiteCd && w.ScPntsDiscntNm == pointConvertInfo.ScPntsDiscntNm).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                pointConvertInfo.UpdateCnt = 0;
                pointConvertInfo.CreateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                pointConvertInfo.CreateMachine = Environment.MachineName;
                pointConvertInfo.UpdateDatetime = pointConvertInfo.CreateDatetime;
                pointConvertInfo.UpdateMachine = pointConvertInfo.CreateMachine;
                pointConvertInfo.Status = Context.CommonConst.STATUS_USED;

                _context.PointConvertInfo.Add(pointConvertInfo);
                _context.SaveChanges();
            }
            else if (info.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」の場合 → 更新
                bool addFlag = true;
                pointConvertInfo.UpdateCnt = info.UpdateCnt;
                pointConvertInfo.CreateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                pointConvertInfo.CreateMachine = Environment.MachineName;
                var updateInfo = await UpdatePointConvert(pointConvertInfo, addFlag);
            }
            else
            {
                // データが存在し,Statusが「1」の場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        // 情報更新
        public async Task<int> UpdatePointConvert(FrMScPointConvertInfo pointConvertInfo, bool addFlag)
        {
            try
            {
                // 会社番号,SCコード,SCサイトコード,SCポイント割引・補助金名称の一致するデータを取得
                var info = _context.PointConvertInfo.Where(w => w.CompanyNo == pointConvertInfo.CompanyNo && w.ScCd == pointConvertInfo.ScCd && w.ScSiteCd == pointConvertInfo.ScSiteCd && w.ScPntsDiscntNm == pointConvertInfo.ScPntsDiscntNm).AsNoTracking().SingleOrDefault();

                if (!addFlag)
                {
                    // バージョンチェック
                    if (!await SiteConvertCheckVer(pointConvertInfo)) { return -1; }
                    pointConvertInfo.CreateClerk = info.CreateClerk;
                    pointConvertInfo.CreateDatetime = info.CreateDatetime;
                    pointConvertInfo.CreateMachine = info.CreateMachine;
                }
                pointConvertInfo.UpdateCnt++;
                pointConvertInfo.ProgramId = info.ProgramId;
                pointConvertInfo.CreateMachineNo = info.CreateMachineNo;
                pointConvertInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                pointConvertInfo.UpdateMachineNo = info.UpdateMachineNo;
                pointConvertInfo.UpdateMachine = Environment.MachineName;
                pointConvertInfo.Status = Context.CommonConst.STATUS_USED;

                _context.PointConvertInfo.Update(pointConvertInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // 情報削除
        public async Task<int> DelPointConvert(FrMScPointConvertInfo pointConvertInfo)
        {
            try
            {
                // バージョンチェック
                if (!await SiteConvertCheckVer(pointConvertInfo)) { return -1; }

                pointConvertInfo.UpdateCnt++;
                pointConvertInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                pointConvertInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.PointConvertInfo.Update(pointConvertInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // バージョンチェック
        private async Task<bool> SiteConvertCheckVer(FrMScPointConvertInfo pointConvertInfo)
        {
            try
            {
                // キーセット
                FrMScPointConvertInfo keyInfo = new FrMScPointConvertInfo() { CompanyNo = pointConvertInfo.CompanyNo, ScCd = pointConvertInfo.ScCd, ScSiteCd = pointConvertInfo.ScSiteCd, ScPntsDiscntNm = pointConvertInfo.ScPntsDiscntNm };

                // データ取得
                var info = await GetPointConvertById(keyInfo);

                // バージョン差異チェック
                if (pointConvertInfo.UpdateCnt != info.UpdateCnt)
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
    }
}