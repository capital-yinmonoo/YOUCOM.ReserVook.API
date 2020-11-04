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
    public class RemarksConvertService : IRemarksConvertService
    {

        private DBContext _context;

        public RemarksConvertService(DBContext context)
        {
            _context = context;
        }

        // 情報取得(画面表示用)
        public async Task<List<FrMScRemarksConvertInfo>> GetRemarksConvertList(FrMScRemarksConvertInfo remarksConvertInfo)
        {
            string sql = "SELECT remarksConvert.*, scName.content_1";
            sql += " FROM fr_m_sc_remarks_convert remarksConvert";

            sql += " LEFT JOIN fr_m_sc_nm scName";
            sql += " ON remarksConvert.company_no = scName.company_no";
            sql += " AND remarksConvert.sc_cd = scName.sc_seg_cd";

            sql += " WHERE remarksConvert.company_no = '" + remarksConvertInfo.CompanyNo + "'";
            sql += " AND remarksConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY remarksConvert.sc_cd ASC, case when remarksConvert.sc_remarks_set_location = '0' then '1' else '0'";
            sql += " end, remarksConvert.sc_remarks_set_location ASC, remarksConvert.sc_remarks_priority_odr ASC, remarksConvert.sc_x_clmn";

            var lists = new List<FrMScRemarksConvertInfo>();
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
                            var list = new FrMScRemarksConvertInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ScCd = reader["sc_cd"].ToString();
                            list.ScXClmn = reader["sc_x_clmn"].ToString();
                            list.ScXClmnKanji = reader["sc_x_clmn_kanji"].ToString();
                            list.ScRemarksSetLocation = int.Parse(reader["sc_remarks_set_location"].ToString());
                            list.ScRemarksPriorityOdr = int.Parse(reader["sc_remarks_priority_odr"].ToString());
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
        public async Task<FrMScRemarksConvertInfo> GetRemarksConvertById(FrMScRemarksConvertInfo remarksConvertInfo)
        {
            var list = new FrMScRemarksConvertInfo();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT remarksConvert.*";
            sql += " FROM fr_m_sc_remarks_convert remarksConvert";
            sql += " WHERE remarksConvert.company_no = '" + remarksConvertInfo.CompanyNo + "'";
            sql += " AND remarksConvert.sc_cd = '" + remarksConvertInfo.ScCd + "'";
            sql += " AND remarksConvert.sc_x_clmn = '" + remarksConvertInfo.ScXClmn + "'";
            sql += " AND remarksConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";

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
                        list.ScXClmn = reader["sc_x_clmn"].ToString();
                        list.ScXClmnKanji = reader["sc_x_clmn_kanji"].ToString();
                        list.ScRemarksSetLocation = int.Parse(reader["sc_remarks_set_location"].ToString());
                        list.ScRemarksPriorityOdr = int.Parse(reader["sc_remarks_priority_odr"].ToString());
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

        // 情報更新
        public async Task<int> UpdateRemarksConvert(FrMScRemarksConvertInfo remarksConvertInfo)
        {
            try
            {
                // 会社番号,SCコード,SCサイトコード,SCポイント割引・補助金名称の一致するデータを取得
                var info = _context.RemarksConvertInfo.Where(w => w.CompanyNo == remarksConvertInfo.CompanyNo && w.ScCd == remarksConvertInfo.ScCd && w.ScXClmn == remarksConvertInfo.ScXClmn).AsNoTracking().SingleOrDefault();

                // バージョンチェック
                if (!await SiteConvertCheckVer(remarksConvertInfo)) { return -1; }

                remarksConvertInfo.UpdateCnt++;
                remarksConvertInfo.ProgramId = info.ProgramId;
                remarksConvertInfo.CreateMachineNo = info.CreateMachineNo;
                remarksConvertInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                remarksConvertInfo.UpdateMachineNo = info.UpdateMachineNo;
                remarksConvertInfo.UpdateMachine = Environment.MachineName;
                remarksConvertInfo.Status = Context.CommonConst.STATUS_USED;

                _context.RemarksConvertInfo.Update(remarksConvertInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // バージョンチェック
        private async Task<bool> SiteConvertCheckVer(FrMScRemarksConvertInfo remarksConvertInfo)
        {
            try
            {
                // キーセット
                FrMScRemarksConvertInfo keyInfo = new FrMScRemarksConvertInfo() { CompanyNo = remarksConvertInfo.CompanyNo, ScCd = remarksConvertInfo.ScCd, ScXClmn = remarksConvertInfo.ScXClmn };

                // データ取得
                var info = await GetRemarksConvertById(keyInfo);

                // バージョン差異チェック
                if (remarksConvertInfo.UpdateCnt != info.UpdateCnt)
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