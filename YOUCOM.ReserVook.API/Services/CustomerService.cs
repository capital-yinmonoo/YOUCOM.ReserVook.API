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
    public class CustomerService : ICustomerService {

        private DBContext _context;

        public CustomerService(DBContext context) {
            _context = context;
        }

        // 顧客の情報取得(画面表示,削除用)
        public async Task<List<MstCustomerInfo>> GetCustomerList(MstCustomerInfo customerInfo) {

            var lists = new List<MstCustomerInfo>();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT customer.*";
            sql += " FROM mst_customer customer";
            sql += " WHERE customer.company_no = '" + customerInfo.CompanyNo + "'";
            sql += " AND customer.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY customer.customer_no ASC";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var cInfo = new MstCustomerInfo();

                        cInfo.CompanyNo = reader["company_no"].ToString();
                        cInfo.CustomerNo = reader["customer_no"].ToString();
                        cInfo.CustomerName = reader["customer_name"].ToString();
                        cInfo.CustomerKana = reader["customer_kana"].ToString();
                        cInfo.ZipCode = reader["zip_code"].ToString();
                        cInfo.Address = reader["address"].ToString();
                        cInfo.PhoneNo = reader["phone_no"].ToString();
                        cInfo.MobilePhoneNo = reader["mobile_phone_no"].ToString();
                        cInfo.Email = reader["email"].ToString();
                        cInfo.CompanyName = reader["company_name"].ToString();
                        cInfo.Remarks1 = reader["remarks1"].ToString();
                        cInfo.Remarks2 = reader["remarks2"].ToString();
                        cInfo.Remarks3 = reader["remarks3"].ToString();
                        cInfo.Remarks4 = reader["remarks4"].ToString();
                        cInfo.Remarks5 = reader["remarks5"].ToString();
                        cInfo.Status = reader["status"].ToString();
                        cInfo.Version = int.Parse(reader["version"].ToString());
                        cInfo.Creator = reader["creator"].ToString();
                        cInfo.Updator = reader["updator"].ToString();
                        cInfo.Cdt = reader["cdt"].ToString();
                        cInfo.Udt = reader["udt"].ToString();

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

        // 顧客の情報取得(編集用)
        public async Task<MstCustomerInfo> GetCustomerById(MstCustomerInfo customerInfo) {

            var cInfo = new MstCustomerInfo();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT customer.*";
            sql += " FROM mst_customer customer";
            sql += " WHERE customer.company_no = '" + customerInfo.CompanyNo + "'";
            sql += " AND customer.customer_no = '" + customerInfo.CustomerNo + "'";
            sql += " AND customer.status <> '" + CommonConst.STATUS_UNUSED + "'";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {

                        cInfo.CompanyNo = reader["company_no"].ToString();
                        cInfo.CustomerNo = reader["customer_no"].ToString();
                        cInfo.CustomerName = reader["customer_name"].ToString();
                        cInfo.CustomerKana = reader["customer_kana"].ToString();
                        cInfo.ZipCode = reader["zip_code"].ToString();
                        cInfo.Address = reader["address"].ToString();
                        cInfo.PhoneNo = reader["phone_no"].ToString();
                        cInfo.MobilePhoneNo = reader["mobile_phone_no"].ToString();
                        cInfo.Email = reader["email"].ToString();
                        cInfo.CompanyName = reader["company_name"].ToString();
                        cInfo.Remarks1 = reader["remarks1"].ToString();
                        cInfo.Remarks2 = reader["remarks2"].ToString();
                        cInfo.Remarks3 = reader["remarks3"].ToString();
                        cInfo.Remarks4 = reader["remarks4"].ToString();
                        cInfo.Remarks5 = reader["remarks5"].ToString();
                        cInfo.Status = reader["status"].ToString();
                        cInfo.Version = int.Parse(reader["version"].ToString());
                        cInfo.Creator = reader["creator"].ToString();
                        cInfo.Updator = reader["updator"].ToString();
                        cInfo.Cdt = reader["cdt"].ToString();
                        cInfo.Udt = reader["udt"].ToString();
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            } finally {
                _context.Database.CloseConnection();
            }

            return cInfo;
        }

        // 顧客の情報取得(バージョンチェック用)
        private async Task<MstCustomerInfo> GetCustomerVer(MstCustomerInfo customerInfo) {
            MstCustomerInfo info = _context.CustomerInfo.AsNoTracking().Where(w => w.CompanyNo == customerInfo.CompanyNo && w.CustomerNo == customerInfo.CustomerNo && w.Status != CommonConst.STATUS_UNUSED).SingleOrDefault();
            return info;
        }

        // 顧客情報追加
        public async Task<int> AddCustomer(MstCustomerInfo customerInfo) {

            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;

            // 会社番号,顧客Noの一致するデータを取得
            var info = _context.CustomerInfo.Where(w => w.CompanyNo == customerInfo.CompanyNo && w.CustomerNo == customerInfo.CustomerNo).AsNoTracking().SingleOrDefault();

            if (info == null) {
                // データが存在しなかった場合 → 追加

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        // 顧客マスタ 新規登録
                        var customerNo = InsertCustomer(customerInfo);

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }
                }
            } else if (info.Status == CommonConst.STATUS_UNUSED) {
                // データが存在し,Statusが「9」の場合 → 更新
                bool addFlag = true;
                customerInfo.Version = info.Version;
                customerInfo.Creator = info.Creator;
                customerInfo.Cdt = info.Cdt;
                customerInfo.Status = Context.CommonConst.STATUS_USED;
                var updateInfo = await UpdateCustomer(customerInfo, addFlag);
            } else {
                // データが存在し,Statusが「1」の場合 → エラー
                errFlg = 1;
            }

            return errFlg;
        }

        // 顧客情報追加
        public async Task<CustomerRegInfo> AddCustomerForReserve(MstCustomerInfo customerInfo) {

            var retInfo = new CustomerRegInfo();

            // 会社番号,顧客Noの一致するデータを取得
            var info = _context.CustomerInfo.Where(w => w.CompanyNo == customerInfo.CompanyNo && w.CustomerNo == customerInfo.CustomerNo).AsNoTracking().SingleOrDefault();

            if (info == null) {
                // データが存在しなかった場合 → 追加

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        // 顧客マスタ 新規登録
                        var customerNo = InsertCustomer(customerInfo);

                        retInfo.CustomerNo = customerNo;
                        retInfo.ResultCode = 0;

                        tran.Commit();

                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }
                }
            } else {
                // データが存在し,Statusが「1」の場合 → エラー
                retInfo.CustomerNo = "";
                retInfo.ResultCode = 1;
            }

            return retInfo;
        }

        /// <summary>
        /// 顧客マスタ 新規登録
        /// </summary>
        /// <param name="customerInfo">登録マスタ情報</param>
        /// <returns>顧客番号</returns>
        public string InsertCustomer(MstCustomerInfo customerInfo) {

            var customerNo = "";

            try {
                // 顧客番号採番
                customerNo = Numbering(customerInfo.CompanyNo);

                customerInfo.CustomerNo = customerNo;
                customerInfo.Version = 0;
                customerInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                customerInfo.Udt = customerInfo.Cdt;
                customerInfo.Status = Context.CommonConst.STATUS_USED;

                _context.CustomerInfo.Add(customerInfo);
                _context.SaveChanges();

                return customerNo;

            } catch(Exception ex) {
                throw ex;
            }
        }

        // 顧客情報更新
        public async Task<int> UpdateCustomer(MstCustomerInfo customerInfo, bool addFlag) {

            try {
                // versionチェック
                if (!addFlag) {
                    if (!await CustomerCheckVer(customerInfo)) { return -1; }
                }

                customerInfo.Version++;
                customerInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                _context.CustomerInfo.Update(customerInfo);
                return _context.SaveChanges();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // 顧客情報削除
        public async Task<int> DelCustomer(MstCustomerInfo customerInfo) {

            try {
                // versionチェック
                if (!await CustomerCheckVer(customerInfo)) { return -1; }

                customerInfo.Version++;
                customerInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                customerInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.CustomerInfo.Update(customerInfo);
                return _context.SaveChanges();

            } catch (Exception e) {
                throw e;
            }
        }

        // バージョンチェック
        private async Task<bool> CustomerCheckVer(MstCustomerInfo customerInfo) {
            try {
                // キーセット
                MstCustomerInfo keyInfo = new MstCustomerInfo() { CompanyNo = customerInfo.CompanyNo, CustomerNo = customerInfo.CustomerNo };

                // データ取得
                var info = await GetCustomerById(keyInfo);

                // バージョン差異チェック
                if (customerInfo.Version != info.Version) {
                    return false;
                }

                return true;
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 顧客番号の新規採番
        /// </summary>
        /// <param name="companyNo">会社番号</param>
        public string Numbering(string companyNo) {
            var info = new MstCompanyInfo();

            // 採番
            info = _context.CompanyInfo.Single(x => x.CompanyNo == companyNo);

            var ret = info.LastCustomerNo;

            // +1して更新
            info.LastCustomerNo = (info.LastCustomerNo.ToInt_Or_Zero() + 1).ToString("0000000000");

            _context.CompanyInfo.Update(info);
            _context.SaveChanges();

            return ret;
        }
    }
}