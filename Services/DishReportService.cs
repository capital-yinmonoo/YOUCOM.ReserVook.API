using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.Commons.Extensions;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Services
{
    public class DishReportService : IDishReportService
    {
        private DBContext _context;

        public DishReportService(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 料理日報一覧情報 取得
        /// </summary>
        /// <param name="cond">料理日報 検索条件</param>
        /// <returns></returns>
        public async Task<List<DishInfo>> GetList(DishReportCondition cond)
        {

            var lists = new List<DishInfo>();
            var daylists = new List<DishInfo>();
            string wkUseDate = string.Empty;

            var command = _context.Database.GetDbConnection().CreateCommand();

            string query = CreateQuery(cond);

            command.CommandText = query;
            _context.Database.OpenConnection();

            try
            {
                // 読込
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        if (lists.Any(x => x.ItemCode == reader["item_code"].ToString())) /* 料理行作成済み → 一致する料理&利用日に加算 */
                        {
                            var rowIdx = lists.FindIndex(f => f.ItemCode == reader["item_code"].ToString());

                            var colIdx = lists[rowIdx].DishDayInfo.FindIndex(f => f.UseDate == reader["use_date"].ToString());

                            lists[rowIdx].DishDayInfo[colIdx].MealNumber += reader["meal_number"].ToString().ToInt_Or_Zero();
                        }
                        else /* 料理行未作成 → 料理の行と日付範囲分の空リストを作成 */
                        {
                            var wkInfo = new DishInfo();
                            wkInfo.MealDivision = reader["meal_division"].ToString();
                            wkInfo.MealDivisionName = reader["meal_division_name"].ToString();
                            wkInfo.ItemCode = reader["item_code"].ToString();
                            wkInfo.MealName = reader["item_name"].ToString();
                            wkInfo.MealDisplayOrder = reader["meal_display_order"].ToString().ToInt_Or_Zero();
                            wkInfo.ItemDisplayOrder = reader["item_display_order"].ToString().ToInt_Or_Zero();
                            wkInfo.DishDayInfo = MakeDishDayInfo(cond);
                            lists.Add(wkInfo);

                            var rowIdx = lists.FindIndex(f => f.ItemCode == reader["item_code"].ToString());

                            var colIdx = lists[rowIdx].DishDayInfo.FindIndex(f => f.UseDate == reader["use_date"].ToString());

                            lists[rowIdx].DishDayInfo[colIdx].MealNumber = reader["meal_number"].ToString().ToInt_Or_Zero();

                        }

                    }
                }

                // データ無し→空リストを返却
                if (lists.Count == 0) { return lists; }

                // 合計計算
                lists = CalcSum(lists);

                // 商品Mの並び順で整列
                lists = lists.OrderBy(o => o.MealDisplayOrder).ThenBy(o => o.ItemDisplayOrder).ToList();

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
        /// 合計行算出
        /// </summary>
        /// <param name="lists"></param>
        private static List<DishInfo> CalcSum(List<DishInfo> lists)
        {
            try
            {
                // 総合計行 算出用
                var sumDishInfo = new DishInfo();
                sumDishInfo.MealDivision = string.Empty;
                sumDishInfo.MealDivisionName = string.Empty;
                sumDishInfo.ItemCode = string.Empty;
                sumDishInfo.MealName = " 合 計 ";
                sumDishInfo.SumMealNumber = 0;
                sumDishInfo.MealDisplayOrder = lists.Max(m => m.MealDisplayOrder) + 1;
                sumDishInfo.ItemDisplayOrder = lists.Max(m => m.ItemDisplayOrder) + 1;
                sumDishInfo.DishDayInfo = new List<DishDayInfo>();

                var isFirst = true;

                // 合計算出
                foreach (var item in lists)
                {
                    // 料理毎の合計
                    item.SumMealNumber = item.DishDayInfo.Sum(s => s.MealNumber);

                    foreach (var dayItem in item.DishDayInfo)
                    {
                        if (isFirst)
                        {
                            var wkInfo = new DishDayInfo();
                            wkInfo.UseDate = dayItem.UseDate;
                            wkInfo.MealNumber = dayItem.MealNumber;
                            sumDishInfo.SumMealNumber += dayItem.MealNumber;
                            sumDishInfo.DishDayInfo.Add(wkInfo);
                        }
                        else
                        {
                            var idx = sumDishInfo.DishDayInfo.FindIndex(f => f.UseDate == dayItem.UseDate);
                            if (idx > -1)
                            {
                                sumDishInfo.DishDayInfo[idx].MealNumber += dayItem.MealNumber;
                                sumDishInfo.SumMealNumber += dayItem.MealNumber;
                            }
                            else
                            {
                                throw new Exception("縦計計算エラー");
                            }
                        }
                    }

                    isFirst = false;
                }

                // 総合計行を追加
                lists.Add(sumDishInfo);

                return lists;
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        /// <summary>
        /// 料理日報取得用 Query作成
        /// </summary>
        /// <param name="cond">検索条件</param>
        /// <returns>query</returns>
        private string CreateQuery(DishReportCondition cond)
        {

            string query = "SELECT ";
            query += "  sale.use_date ";
            query += "  , sale.meal_division ";
            query += "  , code.code_name AS meal_division_name ";
            query += "  , code.display_order AS meal_display_order ";
            query += "  , item.item_code  ";
            query += "  , item.item_name  ";
            query += "  , sale.item_number_m + sale.item_number_f + sale.item_number_c AS meal_number ";
            query += "  , item.display_order AS item_display_order ";
            query += "FROM ";
            query += "  trn_sales_details sale  ";
            query += "  INNER JOIN trn_reserve_basic basic  ";
            query += "    ON sale.company_no = basic.company_no  ";
            query += "    AND sale.reserve_no = basic.reserve_no ";
            query += "  INNER JOIN mst_item item  ";
            query += "    ON sale.company_no = item.company_no  ";
            query += "    AND sale.item_code = item.item_code ";
            query += "  INNER JOIN mst_code_name code  ";
            query += "    ON sale.company_no = code.company_no  ";
            query += "    AND sale.meal_division = code.code  ";
            query += "    AND code.division_code = '" + ((int)CommonEnum.CodeDivision.MealDivision).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'  ";
            query += "    AND sale.item_code = item.item_code  ";
            query += "WHERE ";
            query += "  sale.company_no = '" + cond.CompanyNo + "'  ";
            query += "  AND sale.use_date BETWEEN '" + cond.FromUseDate + "' AND '" + cond.ToUseDate + "'  ";
            query += "  AND sale.item_division = '" + ((int)CommonEnum.ItemDivision.Meal).ToString() + "'  ";
            query += "  AND basic.reserve_state_division = '" + ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString() + "'  ";
            query += "ORDER BY ";
            query += "  sale.use_date ";
            query += "  , code.display_order ";
            query += "  , item.display_order ";
            
            return query;

        }

        /// <summary>
        /// 日付範囲分のリストを作成
        /// </summary>
        /// <param name="cond">検索条件</param>
        /// <returns></returns>
        private List<DishDayInfo> MakeDishDayInfo(DishReportCondition cond)
        {
            var list = new List<DishDayInfo>();

            try
            {
                var dateDiff = (int)DateAndTime.DateDiff(DateInterval.Day, cond.FromUseDate.ToDate(CommonConst.DATE_FORMAT), cond.ToUseDate.ToDate(CommonConst.DATE_FORMAT));
                for (int day = 0; day <= dateDiff; day++)
                {
                    var wkInfo = new DishDayInfo();
                    wkInfo.UseDate = cond.FromUseDate.ToDate(CommonConst.DATE_FORMAT).AddDays(day).ToString(CommonConst.DATE_FORMAT);
                    wkInfo.MealNumber = 0;
                    list.Add(wkInfo);
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

        }
    }
}
