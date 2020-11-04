using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.Commons.Extensions;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Services
{
    public class SalesReportService : ISalesReportService
    {
        private DBContext _context;

        public SalesReportService(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 売上情報 取得
        /// </summary>
        /// <param name="cond">売上情報 検索条件</param>
        /// <returns></returns>
        public async Task<List<SalesReportInfo>> GetList(SalesReportCondition cond)
        {

            var lists = new List<SalesReportInfo>();
            var command = _context.Database.GetDbConnection().CreateCommand();

            string query = CreateQuery(cond);

            command.CommandText = query;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var info = new SalesReportInfo();
                        info.CompanyNo = reader["company_no"].ToString();
                        if (!cond.MonthlyFlag) { info.UseDate = reader["use_date"].ToString(); }
                        info.ItemDivisionName = reader["item_division_name"].ToString();
                        info.ItemCode = reader["item_code"].ToString();

                        info.PrintName = reader["print_name"].ToString();
                        info.UnitPrice = reader["unit_price"].ToString().ToDecimal_Or_Zero();
                        info.ItemNumber = reader["item_number"].ToString().ToInt_Or_Zero();
                        info.AmountPrice = reader["amount_price"].ToString().ToDecimal_Or_Zero();

                        info.InsideTaxPrice = reader["inside_tax_price"].ToString().ToDecimal_Or_Zero();
                        info.InsideServicePrice = reader["inside_service_price"].ToString().ToDecimal_Or_Zero();
                        info.OutsideServicePrice = reader["outside_service_price"].ToString().ToDecimal_Or_Zero();
                        info.DepositFlag = reader["deposit_flag"].ToString() == "1";
                        info.NetAmount = info.AmountPrice - info.InsideServicePrice - info.InsideTaxPrice;
                        if (info.DepositFlag) { info.ItemDivisionName = "入金"; }

                        lists.Add(info);
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

            return lists;
        }

        /// <summary>
        /// 売上情報取得用 Query作成
        /// </summary>
        /// <param name="cond">検索条件</param>
        /// <returns>query</returns>
        private static string CreateQuery(SalesReportCondition cond)
        {

            var startDate = cond.UseDate;
            var endDate = cond.UseDate;

            if (cond.MonthlyFlag)
            {
                startDate = cond.UseDate.Substring(0, 6) + "01";
                endDate = startDate.ToDate(CommonConst.DATE_FORMAT).AddMonths(1).AddDays(-1).ToString(CommonConst.DATE_FORMAT);
            }

            #region salesQuery
            string salesQuery = "SELECT";
            salesQuery += "  tsd.company_no";
            if (!cond.MonthlyFlag) { salesQuery += "  , tsd.use_date"; }
            salesQuery += "  , tsd.item_code";
            salesQuery += "  , MIN(mi.item_division) AS item_division";
            salesQuery += "  , MIN(mi.item_name) AS print_name";
            salesQuery += "  , MIN(mi.unit_price) AS unit_price";
            salesQuery += "  , SUM(tsd.item_number_m + tsd.item_number_f + tsd.item_number_c) AS item_number";
            salesQuery += "  , SUM(tsd.amount_price) AS amount_price";
            salesQuery += "  , SUM(tsd.inside_tax_price) AS inside_tax_price";
            salesQuery += "  , SUM(tsd.inside_service_price) AS inside_service_price";
            salesQuery += "  , SUM(tsd.outside_service_price) AS outside_service_price";
            salesQuery += "  , '0' AS deposit_flag ";
            salesQuery += "FROM";
            salesQuery += "  trn_sales_details tsd ";
            salesQuery += "  LEFT JOIN mst_item mi ";
            salesQuery += "    ON tsd.company_no = mi.company_no ";
            salesQuery += "    AND tsd.item_code = mi.item_code ";
            salesQuery += "WHERE";
            salesQuery += "  tsd.company_no = '" + cond.CompanyNo + "' ";
            salesQuery += "  AND tsd.use_date BETWEEN '" + startDate + "' AND '" + endDate + "' ";
            salesQuery += "  AND tsd.adjustment_flag = '1'";
            salesQuery += "  AND tsd.item_division <> '" + ((int)CommonEnum.ItemDivision.SetItem).ToString() + "'";
            salesQuery += "GROUP BY";
            salesQuery += "  tsd.company_no";
            if (!cond.MonthlyFlag) { salesQuery += "  , tsd.use_date"; }
            salesQuery += "  , tsd.item_code ";
            salesQuery += "ORDER BY";
            salesQuery += "  tsd.item_code ";
            #endregion

            #region depositQuery
            string depositQuery = "SELECT";
            depositQuery += "  tdd.company_no";
            if (!cond.MonthlyFlag) { depositQuery += "  , tdd.deposit_date AS use_date"; }
            depositQuery += "  , tdd.denomination_code AS item_code";
            depositQuery += "  , '' AS item_division";
            depositQuery += "  , MIN(md.denomination_name) AS print_name";
            depositQuery += "  , 0 AS unit_price";
            depositQuery += "  , COUNT(tdd.denomination_code) AS item_number";
            depositQuery += "  , SUM(tdd.amount_price) AS amount_price";
            depositQuery += "  , 0 AS inside_tax_price";
            depositQuery += "  , 0 AS inside_service_price";
            depositQuery += "  , 0 AS outside_service_price";
            depositQuery += "  , '1' AS deposit_flag ";
            depositQuery += "FROM";
            depositQuery += "  trn_deposit_details tdd ";
            depositQuery += "  LEFT JOIN mst_denomination md ";
            depositQuery += "    ON tdd.company_no = md.company_no ";
            depositQuery += "    AND tdd.denomination_code = md.denomination_code ";
            depositQuery += "WHERE";
            depositQuery += "  tdd.company_no = '" + cond.CompanyNo + "' ";
            depositQuery += "  AND tdd.deposit_date BETWEEN '" + startDate + "' AND '" + endDate + "' ";
            depositQuery += "  AND tdd.adjustment_flag = '1'";
            depositQuery += "GROUP BY";
            depositQuery += "  tdd.company_no";
            if (!cond.MonthlyFlag) { depositQuery += "  , tdd.deposit_date"; }
            depositQuery += "  , tdd.denomination_code ";
            depositQuery += "ORDER BY";
            depositQuery += "  tdd.denomination_code";
            #endregion

            string query = "SELECT ";
            query += "  details.* ";
            query += "  , msn.code_name AS item_division_name ";
            query += "FROM ( ";
            query += "      ( ";
            query += salesQuery;
            query += "      ) ";
            query += "UNION ALL";
            query += "      ( ";
            query += depositQuery;
            query += "      ) ";
            query += "     ) details ";

            query += "  LEFT JOIN mst_code_name msn ";
            query += "    ON details.company_no = msn.company_no ";
            query += "    AND msn.division_code = '" + ((int)CommonEnum.CodeDivision.ItemDivision).ToString(CommonConst.CODE_DIVISION_FORMAT) + "' ";
            query += "    AND details.item_division = msn.code ";
            query += "ORDER BY ";
            query += "  details.deposit_flag ";
            query += "  , details.item_division ";
            query += "  , details.item_code ";

            ((int)CommonEnum.CodeDivision.ItemDivision).ToString(CommonConst.CODE_DIVISION_FORMAT);

            return query;

        }
    }
}
