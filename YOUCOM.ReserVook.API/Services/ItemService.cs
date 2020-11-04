using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Services
{
    public class ItemService : IItemService
    {
        private DBContext _context;

        public ItemService(DBContext context)
        {
            _context = context;
        }

        public async Task<int> AddItem(MstItemInfo itemInfo)
        {
            var info = _context.ItemInfo.Where(w => w.CompanyNo == itemInfo.CompanyNo && w.ItemCode == itemInfo.ItemCode).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                itemInfo.Version = 0;
                itemInfo.Cdt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                itemInfo.Udt = itemInfo.Cdt;
                itemInfo.Status = Context.CommonConst.STATUS_USED;

                _context.ItemInfo.Add(itemInfo);
                return _context.SaveChanges();
            }
            else if (info.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」場合 → 更新
                itemInfo.Version = info.Version;
                itemInfo.Cdt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                itemInfo.Status = Context.CommonConst.STATUS_USED;
                return await UpdateItem(itemInfo, true);
            }
            else
            {
                // 重複エラー
                return -1;
            }

        }

        public async Task<int> CheckDelete(MstItemInfo itemInfo)
        {
            return await CheckDelete(itemInfo, false);
        }

        /// <summary>
        /// 削除時チェック
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns>
        /// 0:削除可
        /// 1:削除確認
        /// -1:削除不可(本予約かつ未精算)
        /// </returns>
        public async Task<int> CheckDelete(MstItemInfo itemInfo, bool skipSetItemDivCheck)
        {
            try
            {
                // データ取得
                var salesInfo = await GetSalesDetailsById(itemInfo);

                // 本予約かつ清算済みのデータ
                int countAdjust = salesInfo.Where(x => x.ReserveStateDivision == ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString() && x.AdjustmentFlag == CommonConst.ADJUSTMENTED).Count();

                // 本予約かつ未清算のデータ
                int countNotAdjust = salesInfo.Where(x => x.ReserveStateDivision == ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString() && x.AdjustmentFlag == CommonConst.NOT_ADJUSTMENTED).Count();

                // セット商品マスタのデータ
                int countMstSetItem = _context.SetItemInfo.Where(w => w.CompanyNo == itemInfo.CompanyNo 
                                                                   && w.ItemCode == itemInfo.ItemCode
                                                                   && w.Status != CommonConst.STATUS_UNUSED)
                                                          .AsNoTracking().Count();

                // プラン変換マスタのデータ
                int countPlanConv = _context.PlanConvertInfo.Where(w => w.CompanyNo == itemInfo.CompanyNo
                                                                     && w.ItemCode == itemInfo.ItemCode
                                                                     && w.Status != CommonConst.STATUS_UNUSED)
                                                            .AsNoTracking().Count();

                if (!skipSetItemDivCheck && itemInfo.ItemDivision == ((int)CommonEnum.ItemDivision.SetItem).ToString())
                {
                    // 削除不可(セット商品確認)
                    return -2;
                }
                else if (countNotAdjust > 0)
                {
                    // 削除不可(本予約かつ未精算)
                    return -1;
                }
                else if (countMstSetItem > 0)
                {
                    // 削除不可(セット商品マスタ使用中)
                    return -3;
                }
                else if (countPlanConv > 0)
                {
                    // 削除不可(プランマスタ使用中)
                    return -4;
                }
                else if (countAdjust > 0)
                {
                    // 削除確認
                    return 1;
                }
                else
                {
                    // 削除可
                    return 0;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -99;
            }
        }

        public async Task<int> DeleteItem(MstItemInfo itemInfo)
        {
            try
            {
                // versionチェック
                if (!await CheckVersion(itemInfo)) { return -1; }

                itemInfo.Version++;
                itemInfo.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                itemInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.ItemInfo.Update(itemInfo);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<MstItemInfo> GetItemByPK(MstItemInfo itemInfo)
        {
            MstItemInfo info = _context.ItemInfo.AsNoTracking().Where(w => w.CompanyNo == itemInfo.CompanyNo && w.ItemCode == itemInfo.ItemCode && w.Status != CommonConst.STATUS_UNUSED).SingleOrDefault();
            return info;
        }

        public async Task<ItemInfoView> GetItemByPKView(MstItemInfo itemInfo)
        {
            var list = new ItemInfoView();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT item.*, item.tax_division || item.service_division AS tax_service_division ";
            sql += " FROM mst_item item";
            sql += " WHERE item.company_no = '" + itemInfo.CompanyNo + "'";
            sql += " AND item.item_code = '" + itemInfo.ItemCode + "'";
            sql += " AND item.status <> '" + CommonConst.STATUS_UNUSED + "'";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.CompanyNo = reader["company_no"].ToString();
                        list.ItemCode = reader["item_code"].ToString();
                        list.ItemDivision = reader["item_division"].ToString();
                        list.ItemName = reader["item_name"].ToString();
                        list.ItemKana = reader["item_kana"].ToString();
                        list.PrintName = reader["print_name"].ToString();
                        list.UnitPrice = int.Parse(reader["unit_price"].ToString());
                        list.TaxDivision = reader["tax_division"].ToString();
                        list.TaxrateDivision = reader["taxrate_division"].ToString();
                        list.ServiceDivision = reader["service_division"].ToString();
                        list.DisplayOrder = int.Parse(reader["display_order"].ToString());
                        list.Status = reader["status"].ToString();
                        list.Version = int.Parse(reader["version"].ToString());
                        list.Creator = reader["creator"].ToString();
                        list.Updator = reader["updator"].ToString();
                        list.Cdt = reader["cdt"].ToString();
                        list.Udt = reader["udt"].ToString();
                        list.TaxServiceDivision = reader["tax_service_division"].ToString();
                        list.MealDivision = reader["meal_division"].ToString();
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

        public async Task<List<MstItemInfo>> GetList(MstItemInfo itemInfo)
        {
            var lists = _context.ItemInfo.Where(w => w.CompanyNo == itemInfo.CompanyNo && w.Status != CommonConst.STATUS_UNUSED).OrderBy(o => o.DisplayOrder).ThenBy(n => n.ItemKana).ToList();
            return lists;
        }

        public async Task<List<ItemInfoView>> GetListView(MstItemInfo itemInfo)
        {
            var lists = new List<ItemInfoView>();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT item.*, taxservice.display_name, code.code_name, meal.code_name meal_name ";
            sql += " FROM mst_item item";
            sql += " LEFT JOIN mst_tax_service taxservice";
            sql += " ON item.company_no =  taxservice.company_no";
            sql += " AND item.tax_division =  taxservice.tax_division";
            sql += " AND item.service_division =  taxservice.service_division";
            sql += " LEFT JOIN mst_code_name code";
            sql += " ON item.company_no = code.company_no";
            sql += " AND item.item_division = code.code";
            sql += " LEFT JOIN mst_code_name meal";
            sql += " ON item.company_no = meal.company_no";
            sql += " AND item.meal_division = meal.code";
            sql += " WHERE item.company_no = '" + itemInfo.CompanyNo + "'";
            sql += " AND item.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " AND code.division_code = '" + ((int)CommonEnum.CodeDivision.ItemDivision).ToString(CommonConst.CODE_DIVISION_FORMAT) + "' ";
            sql += " AND meal.division_code = '" + ((int)CommonEnum.CodeDivision.MealDivision).ToString(CommonConst.CODE_DIVISION_FORMAT) + "' ";
            sql += " ORDER BY item.display_order, item.item_code";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var list = new ItemInfoView();
                        list.CompanyNo = reader["company_no"].ToString();
                        list.ItemCode = reader["item_code"].ToString();
                        list.ItemDivision = reader["item_division"].ToString();
                        list.ItemDivisionName = reader["code_name"].ToString();
                        list.ItemName = reader["item_name"].ToString();
                        list.ItemKana = reader["item_kana"].ToString();
                        list.PrintName = reader["print_name"].ToString();
                        list.UnitPrice = int.Parse(reader["unit_price"].ToString());
                        list.TaxDivision = reader["tax_division"].ToString();
                        list.TaxrateDivision = reader["taxrate_division"].ToString();
                        list.ServiceDivision = reader["service_division"].ToString();
                        list.DisplayOrder = int.Parse(reader["display_order"].ToString());
                        list.Status = reader["status"].ToString();
                        list.Version = int.Parse(reader["version"].ToString());
                        list.Creator = reader["creator"].ToString();
                        list.Updator = reader["updator"].ToString();
                        list.Cdt = reader["cdt"].ToString();
                        list.Udt = reader["udt"].ToString();
                        list.TaxServiceDivisionName = reader["display_name"].ToString();
                        list.MealDivision = reader["meal_division"].ToString();
                        list.MealDivisionName = reader["meal_name"].ToString();

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
            return lists;
        }

        public async Task<int> UpdateItem(MstItemInfo itemInfo)
        {
            return await UpdateItem(itemInfo, false);
        }

        public async Task<int> UpdateItem(MstItemInfo itemInfo, bool addFlag)
        {
            try
            {
                // versionチェック
                if (!addFlag && !await CheckVersion(itemInfo)) { return -1; }

                // セット商品チェック
                if (!addFlag && !await CheckModifyTaxDiv(itemInfo)) { return -2; }

                itemInfo.Version++;
                itemInfo.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

                _context.ItemInfo.Update(itemInfo);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -99;
            }

        }

        /// <summary>
        /// バージョンチェック
        /// </summary>
        /// <param name="itemInfo">商品情報(Versionはチェック後でカウントアップする)</param>
        /// <returns>True:Ok, False:NG</returns>
        private async Task<bool> CheckVersion(MstItemInfo itemInfo)
        {
            try
            {
                // キーセット
                MstItemInfo keyinfo = new MstItemInfo() { CompanyNo = itemInfo.CompanyNo, ItemCode = itemInfo.ItemCode };

                // データ取得
                var info = await GetItemByPK(keyinfo);

                // バージョン差異チェック
                if (itemInfo.Version != info.Version)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

        }


        /// <summary>
        /// 情報取得 削除チェック用
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        private async Task<List<TrnSalesDetailsInfo>> GetSalesDetailsById(MstItemInfo itemInfo)
        {
            string sql = "SELECT sales.*, reserve.reserve_state_division";
            sql += " FROM trn_sales_details sales";
            sql += " LEFT JOIN trn_reserve_basic reserve";
            sql += " ON reserve.company_no =  sales.company_no";
            sql += " AND reserve.reserve_no = sales.reserve_no";
            sql += " WHERE sales.company_no = '" + itemInfo.CompanyNo + "'";
            sql += " AND sales.item_code = '" + itemInfo.ItemCode + "'";


            var info = new List<TrnSalesDetailsInfo>();
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
                            var list = new TrnSalesDetailsInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ReserveNo = reader["reserve_no"].ToString();
                            list.BillSeparateSeq = int.Parse(reader["bill_separate_seq"].ToString());
                            list.BillNo = reader["bill_no"].ToString();
                            list.DetailsSeq = int.Parse(reader["details_seq"].ToString());
                            list.UseDate = reader["use_date"].ToString();
                            list.ItemCode = reader["item_code"].ToString();
                            list.PrintName = reader["print_name"].ToString();
                            list.AmountPrice = decimal.Parse(reader["amount_price"].ToString());
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

        /// <summary>
        /// 税区分変更とセット商品をチェック
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        private async Task<bool> CheckModifyTaxDiv(MstItemInfo itemInfo)
        {
            // 登録済のデータ取得
            var orgInfo = await GetItemByPK(itemInfo);

            if (orgInfo == null)
            {
                throw new Exception("Version Error");
            }
            else if (itemInfo.TaxDivision == orgInfo.TaxDivision)
            {
                // 税区分の変更なし→OK
                return true;
            }
            else
            {
                // セット商品マスタで使用済か探索
                var setItems = _context.SetItemInfo.Where(w => w.CompanyNo == itemInfo.CompanyNo && w.ItemCode == itemInfo.ItemCode)
                                                   .AsNoTracking()
                                                   .Count();

                if (setItems == 0)
                {
                    // セット商品では未使用→OK
                    return true;
                }
                else
                {
                    // セット商品で使用→変更NG
                    return false;
                }

            }



        }
    }

}
