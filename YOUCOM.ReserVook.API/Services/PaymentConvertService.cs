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
    public class PaymentConvertService : IPaymentConvertService
    {

        private DBContext _context;

        public PaymentConvertService(DBContext context)
        {
            _context = context;
        }

        // 情報取得(画面表示用)
        public async Task<List<FrMScPaymentConvertInfo>> GetPaymentConvertList(FrMScPaymentConvertInfo paymentConvertInfo)
        {
            string sql = "SELECT paymentConvert.*, scName.content_1, siteConvert.sc_site_nm, tlPayment.code_name, denomination.denomination_name";
            sql += " FROM fr_m_sc_payment_convert paymentConvert";

            sql += " LEFT JOIN fr_m_sc_nm scName";
            sql += " ON paymentConvert.company_no = scName.company_no";
            sql += " AND paymentConvert.sc_cd = scName.sc_seg_cd";

            sql += " LEFT JOIN fr_m_sc_site_convert siteConvert";
            sql += " ON siteConvert.company_no = paymentConvert.company_no";
            sql += " AND siteConvert.sc_cd = paymentConvert.sc_cd";
            sql += " AND siteConvert.sc_site_cd = paymentConvert.sc_site_cd";

            sql += " LEFT JOIN mst_code_name tlPayment";
            sql += " ON tlPayment.company_no = paymentConvert.company_no";
            sql += " AND tlPayment.division_code ='" + ((int)CommonEnum.CodeDivision.TLPayment).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += " AND tlPayment.code = paymentConvert.sc_payment_opts";

            sql += " LEFT JOIN mst_denomination denomination";
            sql += " ON denomination.company_no = paymentConvert.company_no";
            sql += " AND denomination.denomination_code = paymentConvert.denomination_code";

            sql += " WHERE paymentConvert.company_no = '" + paymentConvertInfo.CompanyNo + "'";
            sql += " AND paymentConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY paymentConvert.sc_cd ASC, siteConvert.sc_site_nm ASC, paymentConvert.sc_payment_opts";

            var lists = new List<FrMScPaymentConvertInfo>();
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
                            var list = new FrMScPaymentConvertInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ScCd = reader["sc_cd"].ToString();
                            list.ScSiteCd = reader["sc_site_cd"].ToString();
                            list.ScPaymentOpts = reader["sc_payment_opts"].ToString();
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
                            list.ScPaymentOptsName = reader["code_name"].ToString();
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
        public async Task<FrMScPaymentConvertInfo> GetPaymentConvertById(FrMScPaymentConvertInfo paymentConvertInfo)
        {
            var list = new FrMScPaymentConvertInfo();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT paymentConvert.*";
            sql += " FROM fr_m_sc_payment_convert paymentConvert";
            sql += " WHERE paymentConvert.company_no = '" + paymentConvertInfo.CompanyNo + "'";
            sql += " AND paymentConvert.sc_cd = '" + paymentConvertInfo.ScCd + "'";
            sql += " AND paymentConvert.sc_site_cd = '" + paymentConvertInfo.ScSiteCd + "'";
            sql += " AND paymentConvert.sc_payment_opts = '" + paymentConvertInfo.ScPaymentOpts + "'";
            sql += " AND paymentConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";

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
                        list.ScPaymentOpts = reader["sc_payment_opts"].ToString();
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
        public async Task<int> AddPaymentConvert(FrMScPaymentConvertInfo paymentConvertInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;
            // 会社番号,SCコード,SCサイトコード,SC決済方法の一致するデータを取得
            var info = _context.PaymentConvertInfo.Where(w => w.CompanyNo == paymentConvertInfo.CompanyNo && w.ScCd == paymentConvertInfo.ScCd && w.ScSiteCd == paymentConvertInfo.ScSiteCd && w.ScPaymentOpts == paymentConvertInfo.ScPaymentOpts).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                paymentConvertInfo.UpdateCnt = 0;
                paymentConvertInfo.CreateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                paymentConvertInfo.CreateMachine = Environment.MachineName;
                paymentConvertInfo.UpdateDatetime = paymentConvertInfo.CreateDatetime;
                paymentConvertInfo.UpdateMachine = paymentConvertInfo.CreateMachine;
                paymentConvertInfo.Status = Context.CommonConst.STATUS_USED;

                _context.PaymentConvertInfo.Add(paymentConvertInfo);
                _context.SaveChanges();
            }
            else if (info.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」の場合 → 更新
                bool addFlag = true;
                paymentConvertInfo.UpdateCnt = info.UpdateCnt;
                paymentConvertInfo.CreateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                paymentConvertInfo.CreateMachine = Environment.MachineName;
                var updateInfo = await UpdatePaymentConvert(paymentConvertInfo, addFlag);
            }
            else
            {
                // データが存在し,Statusが「1」の場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        // 情報更新
        public async Task<int> UpdatePaymentConvert(FrMScPaymentConvertInfo paymentConvertInfo, bool addFlag)
        {
            try
            {
                // 会社番号,SCコード,SCサイトコード,SC決済方法の一致するデータを取得
                var info = _context.PaymentConvertInfo.Where(w => w.CompanyNo == paymentConvertInfo.CompanyNo && w.ScCd == paymentConvertInfo.ScCd && w.ScSiteCd == paymentConvertInfo.ScSiteCd && w.ScPaymentOpts == paymentConvertInfo.ScPaymentOpts).AsNoTracking().SingleOrDefault();

                if (!addFlag)
                {
                    // バージョンチェック
                    if (!await SiteConvertCheckVer(paymentConvertInfo)) { return -1; }
                    paymentConvertInfo.CreateClerk = info.CreateClerk;
                    paymentConvertInfo.CreateDatetime = info.CreateDatetime;
                    paymentConvertInfo.CreateMachine = info.CreateMachine;
                }
                paymentConvertInfo.UpdateCnt++;
                paymentConvertInfo.ProgramId = info.ProgramId;
                paymentConvertInfo.CreateMachineNo = info.CreateMachineNo;
                paymentConvertInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                paymentConvertInfo.UpdateMachineNo = info.UpdateMachineNo;
                paymentConvertInfo.UpdateMachine = Environment.MachineName;
                paymentConvertInfo.Status = Context.CommonConst.STATUS_USED;

                _context.PaymentConvertInfo.Update(paymentConvertInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // 情報削除
        public async Task<int> DelPaymentConvert(FrMScPaymentConvertInfo paymentConvertInfo)
        {
            try
            {
                // バージョンチェック
                if (!await SiteConvertCheckVer(paymentConvertInfo)) { return -1; }

                paymentConvertInfo.UpdateCnt++;
                paymentConvertInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                paymentConvertInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.PaymentConvertInfo.Update(paymentConvertInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // バージョンチェック
        private async Task<bool> SiteConvertCheckVer(FrMScPaymentConvertInfo paymentConvertInfo)
        {
            try
            {
                // キーセット
                FrMScPaymentConvertInfo keyInfo = new FrMScPaymentConvertInfo() { CompanyNo = paymentConvertInfo.CompanyNo, ScCd = paymentConvertInfo.ScCd, ScSiteCd = paymentConvertInfo.ScSiteCd, ScPaymentOpts = paymentConvertInfo.ScPaymentOpts };

                // データ取得
                var info = await GetPaymentConvertById(keyInfo);

                // バージョン差異チェック
                if (paymentConvertInfo.UpdateCnt != info.UpdateCnt)
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