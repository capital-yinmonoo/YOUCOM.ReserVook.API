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
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Services {
    public class DataExportService : IDataExportService {

        private DBContext _context;

        public DataExportService(DBContext context) {
            _context = context;
        }

        // 予約情報取得
        public async Task<List<ExportReserveInfo>> GetReserveDataList(ExportReserveCondition cond) {

            var list = new List<ExportReserveInfo>();
            var command = _context.Database.GetDbConnection().CreateCommand();
            
            string sql = "SELECT basic.reserve_no, basic.arrival_date, basic.stay_days, basic.departure_date, basic.reserve_date,";
            sql += " basic.member_male, basic.member_female, basic.member_child_a, basic.member_child_b, basic.member_child_c,";
            sql += " basic.agent_code, agent.agent_name, basic.agent_remarks,";
            sql += " name.phone_no, name.mobile_phone_no, name.guest_name, name.guest_kana, name.company_name,";
            sql += " name.zip_code, name.email, name.address, name.customer_no, COALESCE(sales.use_amount_total, 0) use_amount_total";
            sql += " FROM trn_reserve_basic basic ";

            sql += " LEFT JOIN mst_agent agent ";
            sql += " ON basic.company_no = agent.company_no AND basic.agent_code = agent.agent_code";
            sql += " AND agent.status <> '" + CommonConst.STATUS_UNUSED + "'";

            sql += " INNER JOIN (SELECT company_no, reserve_no, phone_no, mobile_phone_no, guest_name, guest_kana, ";
            sql += " company_name, zip_code, email, address, customer_no FROM trn_name_file ";
            sql += " WHERE company_no = '" + cond.CompanyNo + "' AND use_date = '" + CommonConst.USE_DATE_EMPTY + "' ";
            sql += " AND route_seq = " + CommonConst.DEFAULT_ROUTE_SEQ +") name ";
            sql += " ON basic.company_no = name.company_no AND basic.reserve_no = name.reserve_no";

            sql += " LEFT JOIN (SELECT company_no, reserve_no, sum(amount_price) use_amount_total ";
            sql += " FROM trn_sales_details WHERE set_item_division = '" + ((int)CommonEnum.SetItemDivision.NormalItem).ToString() + "'";
            sql += " GROUP BY company_no, reserve_no) sales";
            sql += " ON basic.company_no = sales.company_no AND basic.reserve_no = sales.reserve_no";

            sql += " WHERE basic.company_no = '" + cond.CompanyNo + "'";

            if (cond.ArrivalDateFrom.IsNotBlanks() && cond.ArrivalDateTo.IsNotBlanks()) {
                // 到着日(開始・終了)どちらも入力
                sql += " AND basic.arrival_date BETWEEN '" + cond.ArrivalDateFrom + "' AND '" + cond.ArrivalDateTo + "'";
            } else if (cond.ArrivalDateFrom.IsNotBlanks() && cond.ArrivalDateTo.IsBlanks()) {
                // 到着日(開始)のみ入力
                sql += " AND basic.arrival_date >= '" + cond.ArrivalDateFrom + "'";
            } else if (cond.ArrivalDateFrom.IsBlanks() && cond.ArrivalDateTo.IsNotBlanks()) {
                // 到着日(終了)のみ入力
                sql += " AND basic.arrival_date <= '" + cond.ArrivalDateTo + "'";
            } else {
                // 到着日(開始・終了)どちらも未入力
                // 処理なし
            }

            if (cond.DepartureDateFrom.IsNotBlanks() && cond.DepartureDateTo.IsNotBlanks()) {
                // 出発日(開始・終了)どちらも入力
                sql += " AND basic.departure_date BETWEEN '" + cond.DepartureDateFrom + "' AND '" + cond.DepartureDateTo + "'";
            } else if (cond.DepartureDateFrom.IsNotBlanks() && cond.DepartureDateTo.IsBlanks()) {
                // 出発日(開始)のみ入力
                sql += " AND basic.departure_date >= '" + cond.DepartureDateFrom + "'";
            } else if (cond.DepartureDateFrom.IsBlanks() && cond.DepartureDateTo.IsNotBlanks()) {
                // 出発日(終了)のみ入力
                sql += " AND basic.departure_date <= '" + cond.DepartureDateTo + "'";
            } else {
                // 出発日(開始・終了)どちらも未入力
                // 処理なし
            }

            sql += " ORDER BY basic.reserve_no ";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var exInfo = new ExportReserveInfo();

                        // 宿泊者情報
                        exInfo.ReserveNo = reader["reserve_no"].ToString();
                        var arrivalDate = reader["arrival_date"].ToString();
                        exInfo.ArrivalDate = arrivalDate.ToDate(CommonConst.DATE_FORMAT).ToString("yyyy/MM/dd");
                        exInfo.StayDays = reader["stay_days"].ToString().ToInt_Or_Zero();
                        var departureDate = reader["departure_date"].ToString();
                        exInfo.DepartureDate = departureDate.ToDate(CommonConst.DATE_FORMAT).ToString("yyyy/MM/dd");
                        var reserveDate = reader["reserve_date"].ToString();
                        exInfo.ReserveDate = reserveDate.ToDate(CommonConst.DATE_FORMAT).ToString("yyyy/MM/dd");
                        exInfo.MemberMale = int.Parse(reader["member_male"].ToString());
                        exInfo.MemberFemale = int.Parse(reader["member_female"].ToString());
                        exInfo.MemberChildA = int.Parse(reader["member_child_a"].ToString());
                        exInfo.MemberChildB = int.Parse(reader["member_child_b"].ToString());
                        exInfo.MemberChildC = int.Parse(reader["member_child_c"].ToString());

                        // エージェント情報
                        exInfo.AgentCode = reader["agent_code"].ToString();
                        exInfo.AgentName = reader["agent_name"].ToString();
                        exInfo.AgentRemarks = reader["agent_remarks"].ToString();

                        // 利用者情報
                        exInfo.PhoneNo = reader["phone_no"].ToString();
                        exInfo.MobilePhoneNo = reader["mobile_phone_no"].ToString();
                        exInfo.GuestName = reader["guest_name"].ToString();
                        exInfo.GuestKana = reader["guest_kana"].ToString();
                        exInfo.CompanyName = reader["company_name"].ToString();
                        exInfo.ZipCode = reader["zip_code"].ToString();
                        exInfo.Email = reader["email"].ToString();
                        exInfo.Address = reader["address"].ToString();
                        exInfo.CustomerNo = reader["customer_no"].ToString();

                        // 商品情報の合計金額
                        exInfo.UseAmountTotal = reader["use_amount_total"].ToString().ToDecimal_Or_Zero();
                        
                        list.Add(exInfo);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return new List<ExportReserveInfo>();
            } finally {
                _context.Database.CloseConnection();
            }

            return list;
        }

        public async Task<List<ExportCustomerInfo>> GetCustomerDataList(ExportCustomerCondition cond) {

            var list = new List<ExportCustomerInfo>();
            var command = _context.Database.GetDbConnection().CreateCommand();

            string sql = "SELECT cst.customer_no, cst.customer_name, cst.customer_kana, cst.zip_code,";
            sql += " cst.address, cst.phone_no, cst.mobile_phone_no, cst.email, cst.company_name,";
            sql += " cst.remarks1, cst.remarks2, cst.remarks3, cst.remarks4, cst.remarks5";
            sql += " FROM mst_customer cst";

            if (cond.UseDateFrom.IsNotBlanks() || cond.UseDateTo.IsNotBlanks()) {
                sql += " INNER JOIN (SELECT company_no, customer_no, SUM(use_amount)";
                sql += " FROM trn_use_results WHERE company_no = '" + cond.CompanyNo + "'";

                if (cond.UseDateFrom.IsNotBlanks() && cond.UseDateTo.IsNotBlanks()) {
                    // 利用日付(開始・終了)どちらも入力
                    sql += " AND (arrival_date BETWEEN '" + cond.UseDateFrom + "' AND '" + cond.UseDateTo + "'";
                    sql += " OR departure_date BETWEEN '" + cond.UseDateFrom + "' AND '" + cond.UseDateTo + "'";
                    sql += " OR (arrival_date <= '" + cond.UseDateFrom + "' AND '" + cond.UseDateTo + "' <= departure_date))";
                } else if (cond.UseDateFrom.IsNotBlanks() && cond.UseDateTo.IsBlanks()) {
                    // 利用日付(開始)のみ入力
                    sql += " AND NOT departure_date < '" + cond.UseDateFrom + "'";
                } else if (cond.UseDateFrom.IsBlanks() && cond.UseDateTo.IsNotBlanks()) {
                    // 利用日付(終了)のみ入力
                    sql += " AND NOT '" + cond.UseDateTo + "' < arrival_date ";
                } else {
                    // 利用日付(開始・終了)どちらも未入力
                    // 処理なし
                }

                sql += " GROUP BY company_no, customer_no";

                if (cond.UseAmountMin.IsNotBlanks() && cond.UseAmountMax.IsNotBlanks()) {
                    // 利用金額(下限・上限)どちらも入力
                    sql += " HAVING SUM(use_amount) BETWEEN " + cond.UseAmountMin.ToInt_Or_Zero().ToString() + " AND " + cond.UseAmountMax.ToInt_Or_Zero().ToString();
                } else if (cond.UseAmountMin.IsNotBlanks() && cond.UseAmountMax.IsBlanks()) {
                    // 利用金額(下限)のみ入力
                    sql += " HAVING SUM(use_amount) >= " + cond.UseAmountMin.ToInt_Or_Zero().ToString();
                } else if (cond.UseAmountMin.IsBlanks() && cond.UseAmountMax.IsNotBlanks()) {
                    // 利用金額(上限)のみ入力
                    sql += " HAVING SUM(use_amount) <= " + cond.UseAmountMax.ToInt_Or_Zero().ToString();
                } else {
                    // 利用金額(下限・上限)どちらも未入力
                    // 処理なし
                }
                sql += " ) rslt";

                sql += " ON cst.company_no = rslt.company_no AND cst.customer_no = rslt.customer_no";
            }

            sql += " WHERE cst.company_no = '" + cond.CompanyNo + "'";
            sql += " AND status <> '" + CommonConst.STATUS_UNUSED + "'";
            if (cond.GuestKana.IsNotBlanks()) {
                string name = SqlUtils.GetStringContainsPattern(cond.GuestKana);
                sql += " AND cst.customer_kana LIKE '" + name + "'";
            }
            if (cond.PhoneNo.IsNotBlanks()) {
                string phone = SqlUtils.GetStringContainsPattern(cond.PhoneNo);
                sql += " AND (replace(replace(cst.phone_no,'-',''),'+','') LIKE '" + phone + "' OR replace(replace(cst.mobile_phone_no,'-',''),'+','') LIKE '" + phone + "')";
            }

            sql += " ORDER BY cst.customer_no ";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var exInfo = new ExportCustomerInfo();

                        // 利用者情報
                        exInfo.CustomerNo = reader["customer_no"].ToString();
                        exInfo.CustomerName = reader["customer_name"].ToString();
                        exInfo.CustomerKana = reader["customer_kana"].ToString();
                        exInfo.ZipCode = reader["zip_code"].ToString();
                        exInfo.Address = reader["address"].ToString();
                        exInfo.PhoneNo = reader["phone_no"].ToString();
                        exInfo.MobilePhoneNo = reader["mobile_phone_no"].ToString();
                        exInfo.Email = reader["email"].ToString();
                        exInfo.CompanyName = reader["company_name"].ToString();
                        exInfo.Remarks1 = reader["remarks1"].ToString();
                        exInfo.Remarks2 = reader["remarks2"].ToString();
                        exInfo.Remarks3 = reader["remarks3"].ToString();
                        exInfo.Remarks4 = reader["remarks4"].ToString();
                        exInfo.Remarks5 = reader["remarks5"].ToString();

                        list.Add(exInfo);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return new List<ExportCustomerInfo>();
            } finally {
                _context.Database.CloseConnection();
            }

            return list;
        }
    }
}