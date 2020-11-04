using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.Commons.Extensions;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Services
{
    public class ReserveLogService : IReserveLogService
    {

        private DBContext _context;

        public ReserveLogService(DBContext context)
        {
            _context = context;
        }

        #region 変更履歴作成
        /// <summary>
        /// 予約変更履歴データ作成
        /// </summary>
        /// <param name="processDivision">予約更新:Update</param>
        /// <param name="afterInfo">予約更新時、更新データを渡す</param>
        /// <returns></returns>
        public async Task<List<TrnReserveLogInfo>> MakeReserveLog(CommonEnum.ReserveLogProcessDivision processDivision, ReserveInfo afterInfo)
        {

            List<TrnReserveLogInfo> writeList = new List<TrnReserveLogInfo>();

            try
            {
                switch (processDivision)
                {
                    case CommonEnum.ReserveLogProcessDivision.Update: /*予約更新*/

                        // 予約基本
                        writeList.AddRange(MakeReserveLog_Basic(afterInfo.ReserveBasicInfo));

                        // 予約部屋タイプ
                        writeList.AddRange(MakeReserveLog_RoomType(afterInfo.RoomTypeInfoList));

                        // 商品情報
                        writeList.AddRange(MakeReserveLog_Sales(afterInfo.SalesDetailsInfoList));

                        // 入金情報
                        writeList.AddRange(MakeReserveLog_Deposit(afterInfo.DepositInfoList));

                        break;

                    default:
                        throw new Exception("不正な処理区分です。");

                }

                return writeList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 予約変更履歴データ作成
        /// </summary>
        /// <param name="processDivision">新規予約:Add, 予約取消:Delete</param>
        /// <returns></returns>
        public async Task<List<TrnReserveLogInfo>> MakeReserveLog(CommonEnum.ReserveLogProcessDivision processDivision)
        {

            List<TrnReserveLogInfo> writeList = new List<TrnReserveLogInfo>();

            try
            {
                switch (processDivision)
                {
                    case CommonEnum.ReserveLogProcessDivision.Add: /* 新規予約 */

                        writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Add
                                                        , "予約"
                                                        , string.Empty
                                                        , "新規予約"));
                        break;

                    case CommonEnum.ReserveLogProcessDivision.Delete: /* 予約取消 */

                        writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Delete
                                                        , "予約"
                                                        , string.Empty
                                                        , "予約取消"));

                        break;

                    default: /* エラー */
                        throw new Exception("不正な処理区分です。");

                }

                return writeList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 予約変更履歴情報 作成
        /// </summary>
        /// <param name="processDivision"></param>
        /// <param name="changeItem"></param>
        /// <param name="beforeVal"></param>
        /// <param name="afterVal"></param>
        /// <returns></returns>
        private TrnReserveLogInfo MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision processDivision, string changeItem, string beforeVal, string afterVal)
        {
            var writeInfo = new TrnReserveLogInfo();
            writeInfo.ProcessDivision = ((int)processDivision).ToString();
            writeInfo.ChangeItem = changeItem;
            writeInfo.BeforeValue = beforeVal;
            writeInfo.AfterValue = afterVal;

            return writeInfo;
        }

        /// <summary>
        /// 採番(ReserveLogSeq)
        /// </summary>
        /// <param name="companyNo"></param>
        /// <param name="reserveNo"></param>
        /// <returns></returns>
        public async Task<int> NumberingLogSeq(string companyNo, string reserveNo)
        {

            var info = _context.ReserveLogInfo.Where(x => x.CompanyNo == companyNo
                                                       && x.ReserveNo == reserveNo)
                                              .AsNoTracking()
                                              .ToList();

            if (info == null || info.Count == 0)
            {
                return 1;
            }
            else
            {
                return info.Max(m => m.ReserveLogSeq) + 1;
            }

        }

        /// <summary>
        /// 採番(SeqGroup)
        /// </summary>
        /// <param name="companyNo"></param>
        /// <param name="reserveNo"></param>
        /// <returns></returns>
        public async Task<int> NumberingSeqGroup(string companyNo, string reserveNo)
        {
            var info = _context.ReserveLogInfo.Where(x => x.CompanyNo == companyNo
                                                       && x.ReserveNo == reserveNo)
                                              .AsNoTracking()
                                              .ToList();

            if (info == null || info.Count == 0)
            {
                return 1;
            }
            else
            {
                return info.Max(m => m.SeqGroup) + 1;
            }
        }

        #region 各テーブルのデータを作成
        /// <summary>
        /// 予約基本から変更履歴を作成
        /// </summary>
        /// <param name="afterInfo"></param>
        /// <returns></returns>
        /// <remarks>出発日,到着日,泊数,人数が対象</remarks>
        private List<TrnReserveLogInfo> MakeReserveLog_Basic(TrnReserveBasicInfo afterInfo)
        {
            try
            {
                var writeList = new List<TrnReserveLogInfo>();

                // 元の値を取得
                var beforeInfo = _context.ReserveBasicInfo.Where(w => w.CompanyNo == afterInfo.CompanyNo
                                                   && w.ReserveNo == afterInfo.ReserveNo)
                                          .AsNoTracking()
                                          .Single();

                // 指定カラムの値に変化があった場合、変更履歴を残す
                // 到着日
                if (beforeInfo.ArrivalDate != afterInfo.ArrivalDate)
                {
                    writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                     , "到着日"
                                                     , DateTime.ParseExact(beforeInfo.ArrivalDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd")
                                                     , DateTime.ParseExact(afterInfo.ArrivalDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd")));
                }

                // 出発日
                if (beforeInfo.DepartureDate != afterInfo.DepartureDate)
                {
                    writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                     , "出発日"
                                                     , DateTime.ParseExact(beforeInfo.DepartureDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd")
                                                     , DateTime.ParseExact(afterInfo.DepartureDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd")));
                }

                // 泊数
                if (beforeInfo.StayDays != afterInfo.StayDays)
                {
                    writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                     , "泊数"
                                                     , beforeInfo.StayDays.ToString()
                                                     , afterInfo.StayDays.ToString()));
                }

                // 人数(大人男)
                if (beforeInfo.MemberMale != afterInfo.MemberMale) {
                    writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                     , "大人男"
                                                     , beforeInfo.MemberMale.ToString()
                                                     , afterInfo.MemberMale.ToString()));
                }

                // 人数(大人女)
                if (beforeInfo.MemberFemale != afterInfo.MemberFemale) {
                    writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                     , "大人女"
                                                     , beforeInfo.MemberFemale.ToString()
                                                     , afterInfo.MemberFemale.ToString()));
                }

                // 人数(子供A)
                if (beforeInfo.MemberChildA != afterInfo.MemberChildA) {
                    writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                     , "子供A"
                                                     , beforeInfo.MemberChildA.ToString()
                                                     , afterInfo.MemberChildA.ToString()));
                }

                // 人数(子供B)
                if (beforeInfo.MemberChildB != afterInfo.MemberChildB) {
                    writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                     , "子供B"
                                                     , beforeInfo.MemberChildB.ToString()
                                                     , afterInfo.MemberChildB.ToString()));
                }

                // 人数(子供C)
                if (beforeInfo.MemberChildC != afterInfo.MemberChildC) {
                    writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                     , "子供C"
                                                     , beforeInfo.MemberChildC.ToString()
                                                     , afterInfo.MemberChildC.ToString()));
                }


                return writeList;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 予約部屋タイプから変更履歴を作成
        /// </summary>
        /// <param name="afterlist"></param>
        /// <returns></returns>
        /// <remarks>部屋タイプ,室数が対象</remarks>
        private List<TrnReserveLogInfo> MakeReserveLog_RoomType(List<TrnReserveRoomtypeInfo> afterlist)
        {
            try
            {
                var writeList = new List<TrnReserveLogInfo>();

                foreach (var afterInfo in afterlist)
                {

                    if (afterInfo.UpdateFlag)
                    {
                        // 元の値を取得
                        var beforeInfo = _context.ReserveRoomtypeInfo.Where(w => w.CompanyNo == afterInfo.CompanyNo
                                                                              && w.ReserveNo == afterInfo.ReserveNo
                                                                              && w.UseDate == afterInfo.UseDate
                                                                              && w.RoomtypeSeq == afterInfo.RoomtypeSeq)
                                                                     .AsNoTracking()
                                                                     .Single();

                        if (beforeInfo == null) { continue; }

                        // 指定カラムの値に変化があった場合、変更履歴を残す
                        // 部屋タイプ
                        if (beforeInfo.RoomtypeCode != afterInfo.RoomtypeCode)
                        {
                            writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                             , "部屋タイプ(" + afterInfo.RoomtypeSeq.ToString() + ")"
                                                             , beforeInfo.RoomtypeCode + " (" + DateTime.ParseExact(beforeInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")"
                                                             , afterInfo.RoomtypeCode + " (" + DateTime.ParseExact(afterInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")"));
                        }

                        // 室数
                        if (beforeInfo.Rooms != afterInfo.Rooms)
                        {
                            writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                             , "室数(" + afterInfo.RoomtypeSeq.ToString() + ")"
                                                             , beforeInfo.Rooms.ToString() + " (" + DateTime.ParseExact(beforeInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")"
                                                             , afterInfo.Rooms.ToString() + " (" + DateTime.ParseExact(afterInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")"));
                        }

                    }
                    else if (afterInfo.AddFlag)
                    {
                        // 変更履歴を残す
                        // 部屋タイプ
                        writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Add
                                                            , "部屋タイプ(" + afterInfo.RoomtypeSeq.ToString() + ")"
                                                            , string.Empty
                                                            , afterInfo.RoomtypeCode + " (" + DateTime.ParseExact(afterInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")"));

                        // 室数
                        writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Add
                                                            , "室数(" + afterInfo.RoomtypeSeq.ToString() + ")"
                                                            , string.Empty
                                                            , afterInfo.Rooms.ToString() + " (" + DateTime.ParseExact(afterInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")"));
                    }
                    else if (afterInfo.DeleteFlag)
                    {
                        // 変更履歴を残す
                        // 部屋タイプ
                        writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Delete
                                                            , "部屋タイプ(" + afterInfo.RoomtypeSeq.ToString() + ")"
                                                            , afterInfo.RoomtypeCode + " (" + DateTime.ParseExact(afterInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")"
                                                            , string.Empty));

                        // 室数
                        writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Delete
                                                            , "室数(" + afterInfo.RoomtypeSeq.ToString() + ")"
                                                            , afterInfo.Rooms.ToString() + " (" + DateTime.ParseExact(afterInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")"
                                                            , string.Empty));
                    }
                    else
                    {
                        // 変更なし
                    }

                }

                return writeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 売上明細から変更履歴を作成
        /// </summary>
        /// <param name="afterlist"></param>
        /// <returns></returns>
        /// <remarks>印字名称,単価,数量が対象</remarks>
        private List<TrnReserveLogInfo> MakeReserveLog_Sales(List<TrnSalesDetailsInfo> afterlist)
        {
            try
            {
                var writeList = new List<TrnReserveLogInfo>();

                foreach (var afterInfo in afterlist.Where(w => w.SetItemDivision == ((int)CommonEnum.SetItemDivision.NormalItem).ToString()))
                {

                    if (afterInfo.UpdateFlag)
                    {
                        // 元の値を取得
                        var beforeInfo = _context.SalesDetailsInfo.Where(w => w.CompanyNo == afterInfo.CompanyNo
                                                                           && w.ReserveNo == afterInfo.ReserveNo
                                                                           && w.DetailsSeq == afterInfo.DetailsSeq)
                                                                  .AsNoTracking()
                                                                  .Single();

                        if (beforeInfo == null) { continue; }

                        // 指定カラムの値に変化があった場合、変更履歴を残す
                        var beforeItemNumber = beforeInfo.ItemNumberM + beforeInfo.ItemNumberF + beforeInfo.ItemNumberC;
                        var afterItemNumber = afterInfo.ItemNumberM + afterInfo.ItemNumberF + afterInfo.ItemNumberC;

                        if ((beforeInfo.PrintName != afterInfo.PrintName) || (beforeInfo.UnitPrice != afterInfo.UnitPrice) || (beforeItemNumber != afterItemNumber))
                        {
                            var beforeVal = beforeInfo.PrintName;
                            beforeVal += " 単価:" + beforeInfo.UnitPrice.ToString("#,0");
                            beforeVal += " 数量:" + beforeItemNumber.ToString("#,0");
                            beforeVal += " (" + DateTime.ParseExact(beforeInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")";

                            var afterVal = afterInfo.PrintName;
                            afterVal += " 単価:" + afterInfo.UnitPrice.ToString("#,0");
                            afterVal += " 数量:" + afterItemNumber.ToString("#,0");
                            afterVal += " (" + DateTime.ParseExact(afterInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")";


                            writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                            , "商品"
                                                            , beforeVal
                                                            , afterVal));
                        }

                    }
                    else if (afterInfo.AddFlag)
                    {
                        // 変更履歴を残す
                        var afterItemNumber = afterInfo.ItemNumberM + afterInfo.ItemNumberF + afterInfo.ItemNumberC;

                        var afterVal = afterInfo.PrintName;
                        afterVal += " 単価:" + afterInfo.UnitPrice.ToString("#,0");
                        afterVal += " 数量:" + afterItemNumber.ToString("#,0");
                        afterVal += " (" + DateTime.ParseExact(afterInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")";


                        writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Add
                                                        , "商品"
                                                        , string.Empty
                                                        , afterVal));

                    }
                    else if (afterInfo.DeleteFlag)
                    {
                        // 変更履歴を残す
                        var beforeItemNumber = afterInfo.ItemNumberM + afterInfo.ItemNumberF + afterInfo.ItemNumberC;

                        var beforeVal = afterInfo.PrintName;
                        beforeVal += " 単価:" + afterInfo.UnitPrice.ToString("#,0");
                        beforeVal += " 数量:" + beforeItemNumber.ToString("#,0");
                        beforeVal += " (" + DateTime.ParseExact(afterInfo.UseDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd") + ")";


                        writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Delete
                                                        , "商品"
                                                        , beforeVal
                                                        , string.Empty));
                    }
                    else
                    {
                        // 変更なし
                    }

                }

                return writeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 入金明細から変更履歴を作成
        /// </summary>
        /// <param name="afterlist"></param>
        /// <returns></returns>
        /// <remarks>金種名,入金日,金額が対象</remarks>
        private List<TrnReserveLogInfo> MakeReserveLog_Deposit(List<TrnDepositDetailsInfo> afterlist)
        {
            try
            {
                var writeList = new List<TrnReserveLogInfo>();

                foreach (var afterInfo in afterlist)
                {

                    if (afterInfo.UpdateFlag)
                    {
                        // 元の値を取得
                        var beforeInfo = _context.DepositDetailsInfo.Where(w => w.CompanyNo == afterInfo.CompanyNo
                                                                             && w.ReserveNo == afterInfo.ReserveNo
                                                                             && w.BillSeparateSeq == afterInfo.BillSeparateSeq
                                                                             && w.DetailsSeq == afterInfo.DetailsSeq)
                                                                    .AsNoTracking()
                                                                    .Single();

                        if (beforeInfo == null) { continue; }

                        // 指定カラムの値に変化があった場合、変更履歴を残す
                        if ((beforeInfo.PrintName != afterInfo.PrintName) || (beforeInfo.AmountPrice != afterInfo.AmountPrice) || (beforeInfo.DepositDate != afterInfo.DepositDate))
                        {
                            var beforeVal = beforeInfo.PrintName;
                            beforeVal += " 入金日:" + DateTime.ParseExact(beforeInfo.DepositDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd");
                            beforeVal += " 金額:" + beforeInfo.AmountPrice.ToString("#,0");

                            var afterVal = afterInfo.PrintName;
                            afterVal += " 入金日:" + DateTime.ParseExact(afterInfo.DepositDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd");
                            afterVal += " 金額:" + afterInfo.AmountPrice.ToString("#,0");


                            writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Update
                                                            , "入金"
                                                            , beforeVal
                                                            , afterVal));
                        }

                    }
                    else if (afterInfo.AddFlag)
                    {

                        var afterVal = afterInfo.PrintName;
                        afterVal += " 入金日:" + DateTime.ParseExact(afterInfo.DepositDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd");
                        afterVal += " 金額:" + afterInfo.AmountPrice.ToString("#,0");


                        writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Add
                                                        , "入金"
                                                        , string.Empty
                                                        , afterVal));

                    }
                    else if (afterInfo.DeleteFlag)
                    {
                        var beforeVal = afterInfo.PrintName;
                        beforeVal += " 入金日:" + DateTime.ParseExact(afterInfo.DepositDate, CommonConst.DATE_FORMAT, null).ToString("yyyy/MM/dd");
                        beforeVal += " 金額:" + afterInfo.AmountPrice.ToString("#,0");


                        writeList.Add(MakeReserveLogInfo(CommonEnum.ReserveLogProcessDivision.Delete
                                                        , "入金"
                                                        , beforeVal
                                                        , string.Empty));
                    }
                    else
                    {
                        // 変更なし
                    }

                }

                return writeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #endregion

        /// <summary>
        /// 予約変更履歴 一覧取得
        /// </summary>
        /// <param name="cond">検索条件</param>
        /// <returns></returns>
        public async Task<List<TrnReserveLogInfo>> GetReserveLogList(TrnReserveLogInfo cond)
        {

            return _context.ReserveLogInfo.Where(w => w.CompanyNo == cond.CompanyNo
                                                   && w.ReserveNo == cond.ReserveNo)
                                          .OrderBy(o => o.ReserveLogSeq)
                                          .AsNoTracking()
                                          .ToList();

        }
    }
}