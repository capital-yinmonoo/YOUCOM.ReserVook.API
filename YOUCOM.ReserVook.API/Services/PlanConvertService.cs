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
    public class PlanConvertService : IPlanConvertService
    {

        private DBContext _context;

        public PlanConvertService(DBContext context)
        {
            _context = context;
        }

        // 情報取得(画面表示用)
        public async Task<List<FrMScPlanConvertInfo>> GetPlanConvertList(FrMScPlanConvertInfo planConvertInfo)
        {
            string sql = "SELECT planConvert.*, scName.content_1, item.item_name";
            sql += " FROM fr_m_sc_plan_convert planConvert";

            sql += " LEFT JOIN fr_m_sc_nm scName";
            sql += " ON planConvert.company_no = scName.company_no";
            sql += " AND planConvert.sc_cd = scName.sc_seg_cd";

            sql += " LEFT JOIN mst_item item";
            sql += " ON item.company_no = planConvert.company_no";
            sql += " AND item.item_code = planConvert.item_code";

            sql += " WHERE planConvert.company_no = '" + planConvertInfo.CompanyNo + "'";
            sql += " AND planConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY planConvert.sc_cd, planConvert.sc_package_plan_cd, planConvert.sc_meal_cond, planConvert.sc_spec_meal_cond, planConvert.item_code";

            var lists = new List<FrMScPlanConvertInfo>();
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
                            var list = new FrMScPlanConvertInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ScCd = reader["sc_cd"].ToString();

                            list.ScPackagePlanCd = reader["sc_package_plan_cd"].ToString();
                            list.ScMealCond = reader["sc_meal_cond"].ToString();
                            list.ScSpecMealCond = reader["sc_spec_meal_cond"].ToString();
                            list.ItemCode = reader["item_code"].ToString();

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
                            list.ItemName = reader["item_name"].ToString();

                            if (list.ScPackagePlanCd == CommonConst.PLAN_CODE_BLANK)
                            {
                                list.ScPackagePlanCd = " ";
                            }

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
        public async Task<FrMScPlanConvertInfo> GetPlanConvertById(FrMScPlanConvertInfo planConvertInfo)
        {
            var list = new FrMScPlanConvertInfo();
            var plancd = planConvertInfo.ScPackagePlanCd.Trim();
            if (plancd.Length == 0) plancd = CommonConst.PLAN_CODE_BLANK;
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT *";
            sql += " FROM fr_m_sc_plan_convert";
            sql += " WHERE company_no = '{0}'";
            sql += " AND sc_cd = '{1}'";
            sql += " AND sc_package_plan_cd = '{2}'";
            sql += " AND sc_meal_cond = '{3}'";
            sql += " AND sc_spec_meal_cond = '{4}'";
            sql += " AND status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql = sql.FillFormat(planConvertInfo.CompanyNo, planConvertInfo.ScCd, plancd, planConvertInfo.ScMealCond, planConvertInfo.ScSpecMealCond);

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
                        list.ScPackagePlanCd = reader["sc_package_plan_cd"].ToString();
                        list.ScMealCond = reader["sc_meal_cond"].ToString();
                        list.ScSpecMealCond = reader["sc_spec_meal_cond"].ToString();
                        list.ItemCode = reader["item_code"].ToString();
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

                        if (list.ScPackagePlanCd == CommonConst.PLAN_CODE_BLANK)
                        {
                            list.ScPackagePlanCd = " ";
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
        public async Task<int> AddPlanConvert(FrMScPlanConvertInfo planConvertInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;
            planConvertInfo.ScPackagePlanCd = planConvertInfo.ScPackagePlanCd.Trim();
            if (planConvertInfo.ScPackagePlanCd.Length == 0) planConvertInfo.ScPackagePlanCd = CommonConst.PLAN_CODE_BLANK;

            var info = _context.PlanConvertInfo.Where(w => w.CompanyNo == planConvertInfo.CompanyNo && w.ScCd == planConvertInfo.ScCd
                                                        && w.ScPackagePlanCd == planConvertInfo.ScPackagePlanCd && w.ScMealCond == planConvertInfo.ScMealCond && w.ScSpecMealCond == planConvertInfo.ScSpecMealCond).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                planConvertInfo.UpdateCnt = 0;
                planConvertInfo.CreateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                planConvertInfo.CreateMachine = Environment.MachineName;
                planConvertInfo.UpdateDatetime = planConvertInfo.CreateDatetime;
                planConvertInfo.UpdateMachine = planConvertInfo.CreateMachine;
                planConvertInfo.Status = Context.CommonConst.STATUS_USED;

                _context.PlanConvertInfo.Add(planConvertInfo);
                _context.SaveChanges();
            }
            else if (info.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」の場合 → 更新
                bool addFlag = true;
                planConvertInfo.UpdateCnt = info.UpdateCnt;
                planConvertInfo.CreateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                planConvertInfo.CreateMachine = Environment.MachineName;
                var updateInfo = await UpdatePlanConvert(planConvertInfo, addFlag);
            }
            else
            {
                // データが存在し,Statusが「1」の場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        // 情報更新
        public async Task<int> UpdatePlanConvert(FrMScPlanConvertInfo planConvertInfo, bool addFlag)
        {
            //List<TrnSalesDetailsInfo> sales = null;
            planConvertInfo.ScPackagePlanCd = planConvertInfo.ScPackagePlanCd.Trim();
            if (planConvertInfo.ScPackagePlanCd.Length == 0) planConvertInfo.ScPackagePlanCd = CommonConst.PLAN_CODE_BLANK;
            var info = _context.PlanConvertInfo.Where(w => w.CompanyNo == planConvertInfo.CompanyNo && w.ScCd == planConvertInfo.ScCd
                                                        && w.ScPackagePlanCd == planConvertInfo.ScPackagePlanCd && w.ScMealCond == planConvertInfo.ScMealCond && w.ScSpecMealCond == planConvertInfo.ScSpecMealCond).AsNoTracking().SingleOrDefault();
            var item = _context.ItemInfo.Where(s => s.CompanyNo == planConvertInfo.CompanyNo && s.ItemCode == planConvertInfo.ItemCode).AsNoTracking().SingleOrDefault();

            //// 自動生成したマスタの変更のみ一括変換対象として、対象となるリストを取得する
            //if (info.ItemCode == CommonConst.AUTO_CONVERT_MASTER_CODE && item.ItemDivision != ((int)CommonEnum.ItemDivision.SetItem).ToString()) 
            //    sales = GetConvertSalesList(info);

            // トランザクション作成
            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                    // マスタ更新

                    if (!addFlag)
                    {
                        // バージョンチェック
                        if (!await CheckVer(planConvertInfo)) { return -1; }
                        planConvertInfo.CreateClerk = info.CreateClerk;
                        planConvertInfo.CreateDatetime = info.CreateDatetime;
                        planConvertInfo.CreateMachine = info.CreateMachine;
                    }
                    planConvertInfo.UpdateCnt++;
                    planConvertInfo.ProgramId = info.ProgramId;
                    planConvertInfo.CreateMachineNo = info.CreateMachineNo;
                    planConvertInfo.UpdateDatetime = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                    planConvertInfo.UpdateMachineNo = info.UpdateMachineNo;
                    planConvertInfo.UpdateMachine = Environment.MachineName;
                    planConvertInfo.Status = Context.CommonConst.STATUS_USED;

                    _context.PlanConvertInfo.Update(planConvertInfo);
                    if (_context.SaveChanges() != 1)
                    {
                        throw new Exception("Update Error");
                    }

                    //// 自動生成で設定された同じ変換済みデータを一括置換する
                    //if (sales != null)
                    //{
                    //    foreach (var data in sales)
                    //    {
                    //        data.ItemCode = planConvertInfo.ItemCode;
                    //        data.PrintName = planConvertInfo.ItemName;
                    //        data.Version++;
                    //        data.Updator = planConvertInfo.UpdateClerk;
                    //        data.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                    //        _context.SalesDetailsInfo.Update(data);
                    //        if (_context.SaveChanges() != 1)
                    //        {
                    //            throw new Exception("Update Error");
                    //        }
                    //    }
                    //}

                    tran.Commit();

                    return 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    tran.Rollback();
                    return -99;
                }
            }
        }

        // 情報削除
        public async Task<int> DelPlanConvert(FrMScPlanConvertInfo planConvertInfo)
        {
            try
            {
                planConvertInfo.ScPackagePlanCd = planConvertInfo.ScPackagePlanCd.Trim();
                if (planConvertInfo.ScPackagePlanCd.Length == 0) planConvertInfo.ScPackagePlanCd = CommonConst.PLAN_CODE_BLANK;

                // バージョンチェック
                if (!await CheckVer(planConvertInfo)) { return -1; }

                planConvertInfo.UpdateCnt++;
                planConvertInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                planConvertInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.PlanConvertInfo.Update(planConvertInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // バージョンチェック
        private async Task<bool> CheckVer(FrMScPlanConvertInfo planConvertInfo)
        {
            try
            {
                // キーセット
                FrMScPlanConvertInfo keyInfo = new FrMScPlanConvertInfo() {
                    CompanyNo = planConvertInfo.CompanyNo,
                    ScCd = planConvertInfo.ScCd,
                    ScPackagePlanCd = planConvertInfo.ScPackagePlanCd,
                    ScMealCond = planConvertInfo.ScMealCond,
                    ScSpecMealCond = planConvertInfo.ScSpecMealCond
                };

                // データ取得
                var info = await GetPlanConvertById(keyInfo);

                // バージョン差異チェック
                if (planConvertInfo.UpdateCnt != info.UpdateCnt)
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

        //// 一括変更対象の取得
        //private List<TrnSalesDetailsInfo> GetConvertSalesList(FrMScPlanConvertInfo planConvertInfo)
        //{
        //    string sql = "SELECT * FROM trn_sales_details";
        //    sql += "  WHERE (company_no, reserve_no) IN";
        //    sql += "  (";
        //    sql += "    SELECT company_no, reserve_no FROM trn_reserve_basic";
        //    sql += "    WHERE (company_no, reserve_no) IN";
        //    sql += "    (";
        //    sql += "      SELECT company_no, reservation_no FROM fr_d_sc_rcv_base";
        //    sql += "      WHERE company_no = '{0}'";
        //    sql += "      AND x_data_clsfic = '" + CommonConst.XDATACLSFIC_NEWDATA + "'";
        //    sql += "      AND x_package_plan_cd = '{1}'";
        //    sql += "      AND x_meal_cond = '{2}'";
        //    sql += "      AND x_spec_meal_cond = '{3}'";
        //    sql += "      AND sc_processed_cd = '" + CommonConst.SCPROCESSEDCD_SCCESS + "'";
        //    sql += "    )";
        //    sql += "    AND reserve_state_division = '" + ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString() + "'";
        //    sql += "    AND status <> '" + CommonConst.STATUS_UNUSED + "'";
        //    sql += "  )";
        //    sql += "  AND item_division = '" + ((int)CommonEnum.ItemDivision.Stay).ToString() + "'";
        //    sql += "  AND item_code = '{4}'";
        //    sql += "  AND status <> '" + CommonConst.STATUS_UNUSED + "'";
        //    sql = sql.FillFormat(planConvertInfo.CompanyNo, planConvertInfo.ScPackagePlanCd, planConvertInfo.ScMealCond, planConvertInfo.ScSpecMealCond, planConvertInfo.ItemCode);

        //    var list = new List<TrnSalesDetailsInfo>();
        //    var command = _context.Database.GetDbConnection().CreateCommand();

        //    command.CommandText = sql;
        //    _context.Database.OpenConnection();

        //    try
        //    {
        //        using (var reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var data = new TrnSalesDetailsInfo();

        //                data.CompanyNo = reader["company_no"].ToString();
        //                data.ReserveNo = reader["reserve_no"].ToString();
        //                data.DetailsSeq = int.Parse(reader["details_seq"].ToString());

        //                data.ItemDivision = reader["item_division"].ToString();
        //                data.MealDivision = reader["meal_division"].ToString();
        //                data.UseDate = reader["use_date"].ToString();
        //                data.ItemCode = reader["item_code"].ToString();
        //                data.PrintName = reader["print_name"].ToString();
        //                data.UnitPrice = decimal.Parse(reader["unit_price"].ToString());
        //                data.ItemNumberM = int.Parse(reader["item_number_m"].ToString());
        //                data.ItemNumberF = int.Parse(reader["item_number_f"].ToString());
        //                data.ItemNumberC = int.Parse(reader["item_number_c"].ToString());
        //                data.AmountPrice = decimal.Parse(reader["amount_price"].ToString());
        //                data.InsideTaxPrice = decimal.Parse(reader["inside_tax_price"].ToString());
        //                data.InsideServicePrice = decimal.Parse(reader["inside_service_price"].ToString());
        //                data.OutsideServicePrice = decimal.Parse(reader["outside_service_price"].ToString());
        //                data.TaxRate = decimal.Parse(reader["tax_rate"].ToString());
        //                data.BillSeparateSeq = int.Parse(reader["bill_separate_seq"].ToString());
        //                data.BillNo = reader["bill_no"].ToString();
        //                data.TaxDivision = reader["tax_division"].ToString();
        //                data.TaxRateDivision = reader["taxrate_division"].ToString();
        //                data.ServiceDivision = reader["service_division"].ToString();
        //                data.SetItemDivision = reader["set_item_division"].ToString();
        //                data.SetItemSeq = int.Parse(reader["set_item_seq"].ToString());
        //                data.AdjustmentFlag = reader["adjustment_flag"].ToString();

        //                data.Status = reader["status"].ToString();
        //                data.Version = int.Parse(reader["version"].ToString());
        //                data.Creator = reader["creator"].ToString();
        //                data.Updator = reader["updator"].ToString();
        //                data.Cdt = reader["cdt"].ToString();
        //                data.Udt = reader["udt"].ToString();

        //                list.Add(data);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //    finally
        //    {
        //        _context.Database.CloseConnection();
        //    }
        //    return list;
        //}
    }
}