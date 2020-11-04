using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.Commons.Extensions;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Controllers;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Services {
    public class UseResultsService : IUseResultsService {

        private DBContext _context;

        public UseResultsService(DBContext context) {
            _context = context;
        }

        // 利用実績の情報取得
        public async Task<List<TrnUseResultsInfo>> GetUseResultsList(TrnUseResultsInfo useResultsInfo) {

            var lists = new List<TrnUseResultsInfo>();
            var command = _context.Database.GetDbConnection().CreateCommand();
            
            string sql = "SELECT *";
            sql += " FROM trn_use_results ";
            sql += " WHERE company_no = '" + useResultsInfo.CompanyNo + "'";
            sql += " AND customer_no = '" + useResultsInfo.CustomerNo + "'";
            sql += " ORDER BY arrival_date, departure_date, reserve_no ";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var cInfo = new TrnUseResultsInfo();

                        cInfo.CompanyNo = reader["company_no"].ToString();
                        cInfo.CustomerNo = reader["customer_no"].ToString();
                        cInfo.ReserveNo = reader["reserve_no"].ToString();
                        cInfo.ArrivalDate = reader["arrival_date"].ToString();
                        cInfo.DepartureDate = reader["departure_date"].ToString();
                        cInfo.UseAmount = reader["use_amount"].ToString().ToDecimal_Or_Zero();
                        cInfo.Status = reader["status"].ToString();
                        cInfo.Version = int.Parse(reader["version"].ToString());
                        cInfo.Creator = reader["creator"].ToString();
                        cInfo.Updator = reader["updator"].ToString();
                        cInfo.Cdt = reader["cdt"].ToString();
                        cInfo.Udt = reader["udt"].ToString();

                        cInfo.DisplayArrivalDate = cInfo.ArrivalDate.ToDate(CommonConst.DATE_FORMAT).ToString("yyyy/MM/dd");
                        cInfo.DisplayDepartureDate = cInfo.DepartureDate.ToDate(CommonConst.DATE_FORMAT).ToString("yyyy/MM/dd");

                        lists.Add(cInfo);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            } finally {
                _context.Database.CloseConnection();
            }

            return lists;
        }

        // 利用実績情報追加
        public async Task<int> AddUseResults(TrnUseResultsInfo useResultsInfo) {

            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;

            // 会社番号,顧客番号,予約番号の一致するデータを取得
            var info = _context.UseResultsInfo.Where(w => w.CompanyNo == useResultsInfo.CompanyNo && w.CustomerNo == useResultsInfo.CustomerNo && w.ReserveNo == useResultsInfo.ReserveNo).AsNoTracking().SingleOrDefault();

            if (info == null) {
                // データが存在しなかった場合 → 追加

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        useResultsInfo.Version = 0;
                        useResultsInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                        useResultsInfo.Udt = useResultsInfo.Cdt;
                        useResultsInfo.Status = Context.CommonConst.STATUS_USED;

                        _context.UseResultsInfo.Add(useResultsInfo);
                        _context.SaveChanges();

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }
                }
            } else {
                // データが存在し,Statusが「1」の場合 → エラー
                errFlg = 1;
            }

            return errFlg;
        }

        // 利用実績情報削除
        public async Task<int> DelUseResults(TrnUseResultsInfo useResultsInfo) {

            try {
                // versionチェック
                if (!await UseResultsCheckVer(useResultsInfo)) { return -1; }

                _context.UseResultsInfo.Remove(useResultsInfo);
                return _context.SaveChanges();

            } catch (Exception e) {
                throw e;
            }
        }

        // バージョンチェック
        private async Task<bool> UseResultsCheckVer(TrnUseResultsInfo useResultsInfo) {

            try {
                // データ取得
                var info = _context.UseResultsInfo.Where(w => w.CompanyNo == useResultsInfo.CompanyNo && w.CustomerNo == useResultsInfo.CustomerNo && w.ReserveNo == useResultsInfo.ReserveNo).AsNoTracking().SingleOrDefault();

                // バージョン差異チェック
                if (useResultsInfo.Version != info.Version) {
                    return false;
                }

                return true;
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}