using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using YOUCOM.Commons.Extensions;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;
using Microsoft.VisualBasic;

namespace YOUCOM.ReserVook.API.Services
{
    public class ConfirmIncomeService : IConfirmIncomeService
    {

        private DBContext _context;

        public class DepositDetailsInfo : BaseInfo {

            /// <summary>
            /// (当日入金)金種コード
            /// </summary>
            public string DenominationCode { get; set; }

            /// <summary>
            /// (当日入金)金種
            /// </summary>
            public string DenominationName { get; set; }

            /// <summary>
            /// (当日入金)金額
            /// </summary>
            public decimal AmountPrice { get; set; }
        }

        public class ReserveBasicInfoEx : TrnReserveBasicInfo {

            public string GuestName { get; set; }

            public decimal DayBeforeSales { get; set; }

            public decimal TodaySales { get; set; }

            public decimal DayBeforeDeposit { get; set; }

            public List<DepositDetailsInfo> DepositDetailsInfo { get; set; }

            public decimal Balance { get; set; }
        }

        public ConfirmIncomeService(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 入金額
        /// </summary>
        /// <returns></returns>
        //public async Task<List<ConfirmIncomeInfo>> GetIncomeList(string companyNo, string date)
        //{
        //    //var context = new ReserVookContext();
        //    var command = _context.Database.GetDbConnection().CreateCommand();
        //    var sr = new StringBuilder();

        //    sr.Append("select dd.reserve_no,nf.guest_name,dd.denomination_code,d.denomination_name,sum(dd.amount_price) as amount_price");
        //    sr.Append(" from trn_deposit_details dd");
        //    sr.Append(" left join trn_reserve_basic rb on dd.company_no = rb.company_no and dd.reserve_no = rb.reserve_no");
        //    sr.Append(" left join trn_name_file nf on rb.company_no = nf.company_no and rb.customer_no = nf.customer_no");
        //    sr.Append(" left join mst_denomination d on dd.company_no = d.company_no and dd.denomination_code = d.denomination_code");
        //    sr.Append(" where dd.company_no = '{0}' and dd.deposit_date = '{1}' and dd.adjustment_flag = '1'");
        //    sr.Append(" group by dd.reserve_no,nf.guest_name,dd.denomination_code,d.denomination_name");
        //    sr.Append(" order by dd.reserve_no");
        //    command.CommandText = sr.ToString().FillFormat(companyNo, date);

        //    var total = new Dictionary<string, decimal>();

        //    var lists = new List<ConfirmIncomeInfo>();
        //    try
        //    {
        //        _context.Database.OpenConnection();
        //        using (var reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var ci = new ConfirmIncomeInfo();

        //                ci.ReserveNo = reader["reserve_no"].ToString();
        //                ci.GuestName = reader["guest_name"].ToString();
        //                ci.DenominationCode = reader["denomination_code"].ToString();
        //                ci.DenominationName = reader["denomination_name"].ToString();
        //                ci.AmountPrice = reader["amount_price"].ToString().ToDecimal_Or_Zero();

        //                lists.Add(ci);

        //                if (total.ContainsKey(ci.DenominationName))
        //                {
        //                    total[ci.DenominationName] += ci.AmountPrice;
        //                } else
        //                {
        //                    total.Add(ci.DenominationName, ci.AmountPrice);
        //                }
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

        //    if (lists.Count >0 )
        //    {
        //        foreach (var info in total)
        //        {
        //            var ci = new ConfirmIncomeInfo();
        //            ci.ReserveNo = "";
        //            ci.GuestName = "";
        //            ci.DenominationCode = "";
        //            ci.DenominationName = "【{0} 合計】".FillFormat(info.Key);
        //            ci.AmountPrice = info.Value;
        //            lists.Add(ci);
        //        }
        //    }

        //    return lists;
        //}


        public async Task<List<ConfirmIncomeInfo>> GetIncomeList(string companyNo, string date)
        {
            var command = _context.Database.GetDbConnection().CreateCommand();
            var sr = new StringBuilder();
            var ddic = new Dictionary<string, string>();    // 金種情報
            var total = new Dictionary<string, decimal>();  // 合計集計用
            var listRb = new List<ReserveBasicInfoEx>();    // 予約情報
            var listSd = new List<TrnSalesDetailsInfo>();   // 売上情報
            var listDd = new List<TrnDepositDetailsInfo>(); // 入金情報
            var lists = new List<ConfirmIncomeInfo>();      // 入金情報一覧
            decimal balanceTotal = 0;

            try
            {
                _context.Database.OpenConnection();

                // 金種一覧を取得
                sr.Clear();
                sr.Append("select denomination_code, denomination_name");
                sr.Append(" from mst_denomination");
                sr.Append(" where company_no = '{0}'");
                sr.Append(" order by denomination_code");
                command.CommandText = sr.ToString().FillFormat(companyNo);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ddic.Add(reader["denomination_code"].ToString(), reader["denomination_name"].ToString());
                    }
                }

                // 当日滞在の部屋一覧を取得
                sr.Clear();
                sr.Append("select rb.reserve_no, rb.arrival_date, rb.departure_date, rb.stay_days, rb.customer_no, nf.guest_name");
                sr.Append(" from trn_reserve_basic rb");
                sr.Append(" left join trn_name_file nf on rb.company_no = nf.company_no and rb.reserve_no = nf.reserve_no");
                sr.Append(" and nf.route_seq = " + CommonConst.DEFAULT_ROUTE_SEQ  + "  and nf.use_date = '" + CommonConst.USE_DATE_EMPTY + "'");
                sr.Append(" where rb.company_no = '{0}'");
                sr.Append(" and rb.arrival_date <= '{1}' and rb.departure_date >= '{1}'");
                sr.Append(" order by rb.reserve_no");
                command.CommandText = sr.ToString().FillFormat(companyNo, date);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ci = new ReserveBasicInfoEx();
                        ci.ReserveNo = reader["reserve_no"].ToString();
                        ci.ArrivalDate = reader["arrival_date"].ToString();
                        ci.DepartureDate = reader["departure_date"].ToString();
                        ci.StayDays = reader["stay_days"].ToString().ToInt_Or_Zero();
                        ci.CustomerNo = reader["customer_no"].ToString();
                        ci.GuestName = reader["guest_name"].ToString();

                        listRb.Add(ci);
                    }
                }

                foreach (var rb in listRb)
                {
                    listSd.Clear();
                    listDd.Clear();

                    // 売上を取得
                    sr.Clear();
                    sr.Append("select use_date, sum(amount_price) as amount_price, sum(outside_service_price) as outside_service_price,");
                    sr.Append(" sum(inside_tax_price) as inside_tax_price, service_division, taxrate_division, tax_rate");
                    sr.Append(" from trn_sales_details");
                    sr.Append(" where company_no = '{0}'");
                    sr.Append(" and reserve_no = '{1}'");
                    sr.Append(" and use_date <= '{2}'");
                    sr.Append(" and item_division <> '" + ((int)CommonEnum.ItemDivision.SetItem).ToString() + "'");
                    sr.Append(" group by use_date, service_division, taxrate_division, tax_rate");
                    command.CommandText = sr.ToString().FillFormat(companyNo, rb.ReserveNo, date);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dateIndex = listSd.FindIndex(s => s.UseDate == reader["use_date"].ToString());

                            if (dateIndex == -1)
                            {
                                var sd = new TrnSalesDetailsInfo();
                                sd.UseDate = reader["use_date"].ToString();
                                sd.AmountPrice = reader["amount_price"].ToString().ToDecimal_Or_Zero();
                                sd.AmountPrice += reader["outside_service_price"].ToString().ToDecimal_Or_Zero();

                                // 外サ分の消費税額を求めるて加える
                                if (int.Parse(reader["service_division"].ToString()) == (int)CommonEnum.ServiceDivision.OutsideService
                                    && int.Parse(reader["taxrate_division"].ToString()) == (int)CommonEnum.TaxDivision.InSideTax)
                                {
                                    // 内税
                                    sd.AmountPrice += reader["inside_tax_price"].ToString().ToDecimal_Or_Zero();
                                    sd.AmountPrice -= Math.Truncate(reader["amount_price"].ToString().ToDecimal_Or_Zero() * reader["tax_rate"].ToString().ToDecimal_Or_Zero() / (100 + reader["tax_rate"].ToString().ToDecimal_Or_Zero()));
                                }

                                listSd.Add(sd);
                            }
                            else
                            {
                                listSd[dateIndex].AmountPrice += reader["amount_price"].ToString().ToDecimal_Or_Zero();
                                listSd[dateIndex].AmountPrice += reader["outside_service_price"].ToString().ToDecimal_Or_Zero();

                                // 外サ分の消費税額を求めるて加える
                                if (int.Parse(reader["service_division"].ToString()) == (int)CommonEnum.ServiceDivision.OutsideService
                                    && int.Parse(reader["taxrate_division"].ToString()) == (int)CommonEnum.TaxDivision.InSideTax)
                                {
                                    // 内税
                                    listSd[dateIndex].AmountPrice += reader["inside_tax_price"].ToString().ToDecimal_Or_Zero();
                                    listSd[dateIndex].AmountPrice -= Math.Truncate(reader["amount_price"].ToString().ToDecimal_Or_Zero() * reader["tax_rate"].ToString().ToDecimal_Or_Zero() / (100 + reader["tax_rate"].ToString().ToDecimal_Or_Zero()));
                                }
                            }
                        }
                    }

                    // 入金を取得
                    sr.Clear();
                    sr.Append("select dd.deposit_date, dd.denomination_code, d.denomination_name, sum(dd.amount_price) as amount_price");
                    sr.Append(" from trn_deposit_details dd");
                    sr.Append(" left join mst_denomination d on dd.company_no = d.company_no and dd.denomination_code = d.denomination_code");
                    sr.Append(" where dd.company_no = '{0}'");
                    sr.Append(" and dd.reserve_no = '{1}'");
                    sr.Append(" and dd.deposit_date <= '{2}'");
                    sr.Append(" group by dd.deposit_date, d.denomination_name, dd.denomination_code");
                    command.CommandText = sr.ToString().FillFormat(companyNo, rb.ReserveNo, date);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dd = new TrnDepositDetailsInfo();
                            dd.DepositDate = reader["deposit_date"].ToString();
                            dd.DenominationCode = reader["denomination_code"].ToString();
                            dd.AmountPrice = reader["amount_price"].ToString().ToDecimal_Or_Zero();
                            listDd.Add(dd);
                        }
                    }

                    // 前日迄売上を取得
                    rb.DayBeforeSales = listSd.Where(x => x.UseDate != date).Sum(x => x.AmountPrice);

                    // 本日売上を取得
                    rb.TodaySales = listSd.Where(x => x.UseDate == date).Sum((x => x.AmountPrice));

                    // 前日迄入金を取得
                    rb.DayBeforeDeposit = listDd.Where(x => x.DepositDate != date).Sum((x => x.AmountPrice));

                    // 本日入金情報を取得
                    rb.DepositDetailsInfo = new List<DepositDetailsInfo>();
                    foreach (TrnDepositDetailsInfo b in listDd.Where(x => x.DepositDate == date).ToList())
                    {
                        var dd = new DepositDetailsInfo();

                        dd.DenominationCode += b.DenominationCode;
                        dd.DenominationName += ddic[b.DenominationCode] ;
                        dd.AmountPrice = b.AmountPrice;
                        rb.DepositDetailsInfo.Add(dd);


                        //合計出力用のDictionalyを設定
                        if (total.ContainsKey(dd.DenominationName))
                        {
                            total[dd.DenominationName] += dd.AmountPrice;
                        }
                        else
                        {
                            total.Add(dd.DenominationName, dd.AmountPrice);
                        }
                    }

                    // 残高セット
                    rb.Balance = rb.DayBeforeSales + rb.TodaySales - rb.DepositDetailsInfo.Sum(x => x.AmountPrice) - rb.DayBeforeDeposit;
                    balanceTotal += rb.Balance;
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

            int index = 0;
            var cii = new ConfirmIncomeInfo();

            if (listRb.Count > 0)
            {
                foreach (var rb in listRb)
                {
                    index = 0;

                    if (rb.DepositDetailsInfo.Count ==0)
                    {
                        cii = new ConfirmIncomeInfo();      // 入金情報一覧
                        cii.Init();

                        cii.ReserveNo = rb.ReserveNo;
                        cii.ArrivalDate = rb.ArrivalDate;
                        cii.DepartureDate = rb.DepartureDate;

                        if (rb.StayDays == 0)
                        {
                            cii.StayDays = "0";
                        }
                        else
                        {
                            if (cii.DepartureDate != date) {
                                var diff = (int)DateAndTime.DateDiff(DateInterval.Day, cii.ArrivalDate.ToDate(CommonConst.DATE_FORMAT), date.ToDate(CommonConst.DATE_FORMAT));
                                cii.StayDays = "{0}/{1}".FillFormat((diff + 1).ToString(), rb.StayDays.ToString());
                            } else
                            {
                                cii.StayDays = "";
                            }
                        }

                        cii.CustomerNo = rb.CustomerNo;
                        cii.GuestName = rb.GuestName;
                        cii.DayBeforeSales = rb.DayBeforeSales.ToString();
                        cii.TodaySales = rb.TodaySales.ToString();
                        cii.DayBeforeDeposit = rb.DayBeforeDeposit.ToString();
                        cii.Balance = rb.Balance.ToString();
                        lists.Add(cii);
                    }
                    else
                    {
                        foreach (var dd in rb.DepositDetailsInfo)
                        {
                            cii = new ConfirmIncomeInfo();      // 入金情報一覧
                            cii.Init();

                            if (index == 0)
                            {
                                cii.ReserveNo = rb.ReserveNo;
                                cii.ArrivalDate = rb.ArrivalDate;
                                cii.DepartureDate = rb.DepartureDate;

                                if (rb.StayDays == 0)
                                {
                                    cii.StayDays = "0";
                                }
                                else
                                {
                                    var diff = (int)DateAndTime.DateDiff(DateInterval.Day, cii.ArrivalDate.ToDate(CommonConst.DATE_FORMAT), date.ToDate(CommonConst.DATE_FORMAT));
                                    cii.StayDays = "{0}/{1}".FillFormat((diff + 1).ToString(), rb.StayDays.ToString());
                                }

                                cii.CustomerNo = rb.CustomerNo;
                                cii.GuestName = rb.GuestName;
                                cii.DayBeforeSales = rb.DayBeforeDeposit.ToString();
                                cii.TodaySales = rb.TodaySales.ToString();
                                cii.DayBeforeDeposit = rb.DayBeforeDeposit.ToString();
                                cii.Balance = rb.Balance.ToString();
                            }

                            cii.DenominationCode = dd.DenominationCode;
                            cii.DenominationName = dd.DenominationName;
                            cii.AmountPrice = dd.AmountPrice.ToString();

                            lists.Add(cii);
                            index += 1;
                        }
                    }
                }

                // 当日者出発残高出力
                cii = new ConfirmIncomeInfo();      // 入金情報一覧
                cii.Init(true);
                cii.DenominationName = "残高";
                cii.Balance = balanceTotal.ToString();
                lists.Add(cii);

                if (total.Count > 0)
                {

                    // 当日金種毎入金情報
                    foreach (var info in total)
                    {
                        cii = new ConfirmIncomeInfo();
                        cii.Init(true);
                        cii.DenominationName = "{0}".FillFormat(info.Key);
                        cii.AmountPrice = info.Value.ToString();
                        lists.Add(cii);
                    }

                }
            }

            return lists;
        }


        /// <summary>
        /// チェックイン済みかどうかを判定
        /// </summary>
        /// <param name="companyNo">会社コード</param>
        /// <param name="reserveNo">予約番号</param>
        /// <param name="date">利用日</param>
        /// <returns>True:チェックイン済み, False:未チェックイン</returns>
        private bool isCheckIn(string roomStateClass)
        {

            switch (roomStateClass)
            {
                case CommonConst.ROOMSTATUS_STAY:
                    return true;
                case CommonConst.ROOMSTATUS_CO:
                    return true;
                case CommonConst.ROOMSTATUS_CLEANING:
                    return true;
                case CommonConst.ROOMSTATUS_CLEANED:
                    return true;
                case CommonConst.ROOMSTATUS_STAYCLEANING:
                    return true;
                case CommonConst.ROOMSTATUS_STAYCLEANED:
                    return true;
                default:
                    return false;
            }

        }
    }
}
