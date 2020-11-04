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
    public class DenominationService : IDenominationService
    {

        private DBContext _context;

        public DenominationService(DBContext context)
        {
            _context = context;
        }

        // 金種取得(別ページ用)
        public async Task<List<MstDenominationInfo>> GetDenominationList(string companyNo)
        {
            return _context.DenominationInfo
                    .Where(d => d.CompanyNo == companyNo && d.Status == CommonConst.STATUS_USED)
                    .OrderBy(d => d.DisplayOrder).ToList();
        }

        // 情報取得(画面表示用)
        public async Task<List<MstDenominationInfo>> GetDenominationList(MstDenominationInfo denominationInfo)
        {
            string sql = "SELECT denomination.*";
            sql += " FROM mst_denomination denomination";
            sql += " WHERE denomination.company_no = '" + denominationInfo.CompanyNo + "'";
            sql += " AND denomination.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY denomination.display_order ASC ,denomination.denomination_code ASC";

            var lists = new List<MstDenominationInfo>();
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
                            var list = new MstDenominationInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.DenominationCode = reader["denomination_code"].ToString();
                            list.DenominationName = reader["denomination_name"].ToString();
                            list.PrintName = reader["print_name"].ToString();
                            list.ReceiptDiv = reader["receipt_div"].ToString();
                            list.DisplayOrder = int.Parse(reader["display_order"].ToString());
                            list.Status = reader["status"].ToString();
                            list.Version = int.Parse(reader["version"].ToString());
                            list.Creator = reader["creator"].ToString();
                            list.Updator = reader["updator"].ToString();
                            list.Cdt = reader["cdt"].ToString();
                            list.Udt = reader["udt"].ToString();

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

        // 情報取得(編集,削除用)
        public async Task<MstDenominationInfo> GetDenominationById(MstDenominationInfo denominationInfo)
        {
            string sql = "SELECT denomination.*";
            sql += " FROM mst_denomination denomination";
            sql += " WHERE denomination.company_no = '" + denominationInfo.CompanyNo + "'";
            sql += " AND denomination.denomination_code = '" + denominationInfo.DenominationCode + "'";
            sql += " AND denomination.status <> '" + CommonConst.STATUS_UNUSED + "'";


            var list = new MstDenominationInfo();
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
                            list.DenominationCode = reader["denomination_code"].ToString();
                            list.DenominationName = reader["denomination_name"].ToString();
                            list.PrintName = reader["print_name"].ToString();
                            list.ReceiptDiv = reader["receipt_div"].ToString();
                            list.DisplayOrder = int.Parse(reader["display_order"].ToString());
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

        // 情報追加
        public async Task<int> AddDenomination(MstDenominationInfo denominationInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;
            // 会社番号,コード一致するデータを取得
            var info = _context.DenominationInfo.Where(w => w.CompanyNo == denominationInfo.CompanyNo && w.DenominationCode == denominationInfo.DenominationCode).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                denominationInfo.Version = 0;
                denominationInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                denominationInfo.Udt = denominationInfo.Cdt;
                denominationInfo.Status = Context.CommonConst.STATUS_USED;

                _context.DenominationInfo.Add(denominationInfo);
                _context.SaveChanges();
            }
            else if (info.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」場合 → 更新
                bool addFlag = true;
                denominationInfo.Version = info.Version;
                denominationInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                denominationInfo.Status = Context.CommonConst.STATUS_USED;
                var updateInfo = await UpdateDenomination(denominationInfo, addFlag);
            }
            else
            {
                // データが存在し,Statusが「1」場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        // 情報更新
        public async Task<int> UpdateDenomination(MstDenominationInfo denominationInfo, bool addFlag)
        {
            try
            {
                // versionチェック
                if (!addFlag)
                {
                    if (!await DenominationCheckVer(denominationInfo)) { return -1; }
                }

                denominationInfo.Version++;
                denominationInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                _context.DenominationInfo.Update(denominationInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // 情報削除
        public async Task<int> DelDenomination(MstDenominationInfo denominationInfo)
        {
            try
            {
                // versionチェック
                if (!await DenominationCheckVer(denominationInfo)) { return -1; }

                denominationInfo.Version++;
                denominationInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                denominationInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.DenominationInfo.Update(denominationInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        // バージョンチェック
        private async Task<bool> DenominationCheckVer(MstDenominationInfo denominationInfo)
        {
            try
            {
                // キーセット
                MstDenominationInfo keyInfo = new MstDenominationInfo() { CompanyNo = denominationInfo.CompanyNo, DenominationCode = denominationInfo.DenominationCode };

                // データ取得
                var info = await GetDenominationById(keyInfo);

                // バージョン差異チェック
                if (denominationInfo.Version != info.Version)
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

        // 削除チェック
        public async Task<int> DeleteDenominationCheck(MstDenominationInfo denominationInfo)
        {
            try
            {
                // キーセット
                MstDenominationInfo keyInfo = new MstDenominationInfo() { CompanyNo = denominationInfo.CompanyNo, DenominationCode = denominationInfo.DenominationCode };

                // データ取得
                var depositInfo = await GetDepositDetailsById(keyInfo);

                // 支払方法変換マスタで使用中のデータ
                int countPayment = _context.PaymentConvertInfo.Count(w => w.CompanyNo == denominationInfo.CompanyNo && w.DenominationCode == denominationInfo.DenominationCode && w.Status == CommonConst.STATUS_USED);

                // ポイント変換マスタで使用中のデータ
                int countPoint = _context.PointConvertInfo.Count(w => w.CompanyNo == denominationInfo.CompanyNo && w.DenominationCode == denominationInfo.DenominationCode && w.Status == CommonConst.STATUS_USED);

                // 本予約かつ未精算のデータ
                int countMain = depositInfo.Where(x => x.ReserveStateDivision == ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString() && x.AdjustmentFlag == CommonConst.NOT_ADJUSTMENTED).Count();

                // 本予約かつ清算済みのデータ
                int countNotAdjust = depositInfo.Where(x => x.ReserveStateDivision == ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString() && x.AdjustmentFlag == CommonConst.ADJUSTMENTED).Count();


                if (countPayment > 0) 
                {
                    // 削除不可(支払方法変換マスタで使用中)
                    return 1;
                }
                else if (countPoint > 0)
                {
                    // 削除不可(ポイント変換マスタで使用中)
                    return 2;
                }
                else if (countMain > 0)
                {
                    // 削除不可(本予約かつ未精算)
                    return 3;
                }
                else if (countNotAdjust > 0)
                {
                    // 削除確認
                    return 4;
                }
                else
                {
                    // 削除可
                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // 情報取得(削除チェック用)
        private async Task<List<TrnDepositDetailsInfo>> GetDepositDetailsById(MstDenominationInfo denominationInfo)
        {
            string sql = "SELECT depositDetails.*, reserve.reserve_state_division";
            sql += " FROM trn_deposit_details depositDetails";

            sql += " LEFT JOIN trn_reserve_basic reserve";
            sql += " ON reserve.company_no =  depositDetails.company_no";
            sql += " AND reserve.reserve_no = depositDetails.reserve_no";

            sql += " WHERE depositDetails.company_no = '" + denominationInfo.CompanyNo + "'";
            sql += " AND depositDetails.denomination_code = '" + denominationInfo.DenominationCode + "'";


            var info = new List<TrnDepositDetailsInfo>();
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
                            var list = new TrnDepositDetailsInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ReserveNo = reader["reserve_no"].ToString();
                            list.BillSeparateSeq = int.Parse(reader["bill_separate_seq"].ToString());
                            list.BillNo = reader["bill_no"].ToString();
                            list.DetailsSeq = int.Parse(reader["details_seq"].ToString());
                            list.DepositDate = reader["deposit_date"].ToString();
                            list.DenominationCode = reader["denomination_code"].ToString();
                            list.PrintName = reader["print_name"].ToString();
                            list.AmountPrice = decimal.Parse(reader["amount_price"].ToString());
                            list.Remarks = reader["remarks"].ToString();
                            list.AdjustmentFlag = reader["adjustment_flag"].ToString();
                            list.Status = reader["status"].ToString();
                            list.Version = int.Parse(reader["version"].ToString());
                            list.Creator = reader["creator"].ToString();
                            list.Updator = reader["updator"].ToString();
                            list.Cdt = reader["cdt"].ToString();
                            list.Udt = reader["udt"].ToString();

                            list.ReserveStateDivision = reader["reserve_state_division"].ToString();

                            info.Add(list);
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

            return info;
        }
    }
}