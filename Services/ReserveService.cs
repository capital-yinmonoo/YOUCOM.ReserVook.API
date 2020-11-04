using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;
using YOUCOM.Commons.Extensions;

namespace YOUCOM.ReserVook.API.Services
{
    public class ReserveService : IReserveService
    {
        private DBContext _context;
        private IRoomsService _roomsService;
        private IReserveLogService _reserveLogService;

        public ReserveService(DBContext context, IRoomsService roomsService, IReserveLogService reserveLogService)
        {
            _context = context;
            _roomsService = roomsService;
            _reserveLogService = reserveLogService;
        }

        public async Task<ReserveModel> GetReserveInfoByPK(ReserveModel cond)
        {
            var reserveInfo = new ReserveInfo();
            var reserveModel = new ReserveModel();

            try
            {
                // 予約基本
                reserveInfo.ReserveBasicInfo = _context.ReserveBasicInfo
                                        .Where(b => b.CompanyNo == cond.CompanyNo
                                                 && b.ReserveNo == cond.ReserveNo)
                                        .SingleOrDefault();

                // 氏名ファイル
                reserveInfo.NameFileInfo = _context.NameFileInfo
                                            .Where(n => n.CompanyNo == cond.CompanyNo
                                                     && n.ReserveNo == cond.ReserveNo
                                                     && n.UseDate == CommonConst.USE_DATE_EMPTY
                                                     && n.RouteSEQ == CommonConst.DEFAULT_ROUTE_SEQ)
                                            .SingleOrDefault();

                // 氏名ファイル(部屋)
                reserveInfo.NameFileInfo_Room = _context.NameFileInfo
                            .Where(n => n.CompanyNo == cond.CompanyNo
                                     && n.ReserveNo == cond.ReserveNo
                                     && n.UseDate != CommonConst.USE_DATE_EMPTY
                                     && n.RouteSEQ != CommonConst.DEFAULT_ROUTE_SEQ)
                            .ToList();

                // 日別部屋タイプ
                reserveInfo.RoomTypeInfoList = await GetReserveRoomTypeInfoListByPK(cond);

                // 売上明細
                var itemInfoList = _context.SalesDetailsInfo
                                            .Where(sI => sI.CompanyNo == cond.CompanyNo
                                                      && sI.ReserveNo == cond.ReserveNo)
                                            .OrderBy(o => o.SetItemSeq)
                                            .ThenBy(o2 => o2.DetailsSeq)
                                            .ToList();

                // 入金明細
                reserveInfo.DepositInfoList = _context.DepositDetailsInfo
                                            .Where(d => d.CompanyNo == cond.CompanyNo
                                                      && d.ReserveNo == cond.ReserveNo)
                                            .OrderBy(o => o.DetailsSeq)
                                            .ThenBy(o2 => o2.DepositDate)
                                            .ToList();

                // 備考
                reserveInfo.RemarksInfoList = _context.ReserveNoteInfo
                                            .Where(r => r.CompanyNo == cond.CompanyNo
                                                      && r.ReserveNo == cond.ReserveNo)
                                            .OrderBy(o => o.NoteSeq)
                                            .ToList();
                // 変換
                reserveModel = await ConvertEntityToModel(reserveInfo, itemInfoList);


                // 日別アサイン
                reserveModel.assignList = _context.ReserveAssignInfo
                                            .Where(a => a.CompanyNo == cond.CompanyNo
                                                     && a.ReserveNo == cond.ReserveNo)
                                            .OrderBy(o => o.UseDate)
                                            .ThenBy(o2 => o2.RouteSEQ)
                                            .ToList();

                // 精算済みビルNo一覧
                reserveModel.adjustmentedBillNoCheckList = _context.SalesDetailsInfo
                                            .Where(i => i.CompanyNo == cond.CompanyNo
                                                      && i.ReserveNo == cond.ReserveNo
                                                      && i.AdjustmentFlag == CommonConst.ADJUSTMENTED)
                                            .GroupBy(g => g.BillSeparateSeq)
                                            .Select(s => s.Key)
                                            .ToList();

                // 会場予約一覧
                reserveModel.trnFacilityInfo = await GetReserveFacilityInfoListByPK(cond);

                if (reserveModel.XTravelAgncBkngNum != null) 
                {

                    // 電文SEQ取得
                    var webCond = new WebReserveBaseInfo();
                    webCond.CompanyNo = cond.CompanyNo;
                    webCond.XTravelAgncBkngNum = reserveModel.XTravelAgncBkngNum;
                    var webInfo = await GetWebReserveBaseSeqByXTravelAgncBkngNum(webCond);
                    reserveModel.ScCd = webInfo.ScCd;
                    reserveModel.XTravelAgncBkngSeq = webInfo.ScRcvSeq;

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return reserveModel;
        }

        /// <summary>
        /// 日別部屋タイプ
        /// </summary>
        /// <param name="cond"></param>
        private async Task<List<TrnReserveRoomtypeInfo>> GetReserveRoomTypeInfoListByPK(ReserveModel cond)
        {
            var sql = "SELECT rsv.* ";
            sql += " FROM trn_reserve_roomtype rsv";
            sql += " LEFT JOIN mst_code_name mst";
            sql += " ON rsv.company_no = mst.company_no";
            sql += " AND mst.division_code = '" + ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += " AND rsv.roomtype_code = mst.code";
            sql += " WHERE rsv.company_no = '" + cond.CompanyNo + "'";
            sql += " AND rsv.reserve_no = '" + cond.ReserveNo + "'";
            sql += " ORDER BY rsv.use_date, rsv.roomtype_seq ";

            var list = new List<TrnReserveRoomtypeInfo>();
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
                            var info = new TrnReserveRoomtypeInfo();

                            info.CompanyNo = reader["company_no"].ToString();
                            info.ReserveNo = reader["reserve_no"].ToString();
                            info.UseDate = reader["use_date"].ToString();
                            info.RoomtypeCode = reader["roomtype_code"].ToString();
                            info.Rooms = int.Parse(reader["rooms"].ToString());
                            info.RoomtypeSeq = int.Parse(reader["roomtype_seq"].ToString());
                            info.Status = reader["status"].ToString();
                            info.Version = int.Parse(reader["version"].ToString());
                            info.Creator = reader["creator"].ToString();
                            info.Updator = reader["updator"].ToString();
                            info.Cdt = reader["cdt"].ToString();
                            info.Udt = reader["udt"].ToString();
                            list.Add(info);
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

        /// <summary>
        /// 会場予約
        /// </summary>
        /// <param name="cond"></param>
        private async Task<List<TrnReserveFacilityInfo>> GetReserveFacilityInfoListByPK(ReserveModel cond)
        {
            var sql = "SELECT fac.*, mst.facility_name";
            sql += " FROM trn_reserve_facility fac";
            sql += " LEFT JOIN mst_facility mst";
            sql += "   ON fac.company_no = mst.company_no";
            sql += "  AND fac.facility_code = mst.facility_code";
            sql += " WHERE fac.company_no = '" + cond.CompanyNo + "'";
            sql += "   AND fac.reserve_no = '" + cond.ReserveNo + "'";
            sql += " ORDER BY fac.use_date, to_number(fac.start_time,'9999'), to_number(fac.end_time,'9999'), mst.display_order";

            var list = new List<TrnReserveFacilityInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand()) {

                command.CommandText = sql;
                _context.Database.OpenConnection();

                try {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var info = new TrnReserveFacilityInfo();

                            info.CompanyNo = reader["company_no"].ToString();
                            info.FacilityCode = reader["facility_code"].ToString();
                            info.FacilityName = reader["facility_name"].ToString();
                            info.UseDate = reader["use_date"].ToString();
                            info.FacilitySeq = int.Parse(reader["facility_seq"].ToString());
                            info.StartTime = reader["start_time"].ToString();
                            info.EndTime = reader["end_time"].ToString();
                            info.FacilityMember = int.Parse(reader["facility_member"].ToString());
                            info.FacilityRemarks = reader["facility_remarks"].ToString();
                            info.ReserveNo = reader["reserve_no"].ToString();
                            info.Status = reader["status"].ToString();
                            info.Version = int.Parse(reader["version"].ToString());
                            info.Creator = reader["creator"].ToString();
                            info.Updator = reader["updator"].ToString();
                            info.Cdt = reader["cdt"].ToString();
                            info.Udt = reader["udt"].ToString();
                            list.Add(info);
                        }
                    }

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                } finally {
                    _context.Database.CloseConnection();
                }
            }

            return list;
        }

        public async Task<WebReserveBaseInfo> GetWebReserveBaseSeqByXTravelAgncBkngNum(WebReserveBaseInfo cond)
        {
            string sql = "SELECT sc_cd, sc_rcv_seq";
            sql += " FROM fr_d_sc_rcv_base";
            sql += " WHERE company_no = '" + cond.CompanyNo + "'";
            sql += " AND x_travel_agnc_bkng_num = '" + cond.XTravelAgncBkngNum + "'";
            sql += " Order by sc_cd, sc_rcv_seq";
            sql += " Limit 1";  // 1件のみ取得

            var info = new WebReserveBaseInfo();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _context.Database.OpenConnection();

                try
                {
                    using (var reader = command.ExecuteReader())
                    {

                        reader.Read();      // 1行目しか読まない

                        info.ScCd = reader["sc_cd"].ToString();
                        info.ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString());
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

        #region -- ConvertEntityToModel --
        private async Task<ReserveModel> ConvertEntityToModel(ReserveInfo reserve
                                                            , List<TrnSalesDetailsInfo> itemInfoList)
        {
            var model = new ReserveModel();
            model.Version = reserve.ReserveBasicInfo.Version;
            model.Status = reserve.ReserveBasicInfo.Status;
            model.stayInfo = await ConvertEntityToModel_Stay(reserve);
            model.guestInfo = await ConvertEntityToModel_Guest(reserve);
            model.agentInfo = await ConvertEntityToModel_Agent(reserve);
            model.roomTypeInfo = await ConvertEntityToModel_RoomType(reserve);
            model.itemInfo = await ConvertEntityToModel_Item(itemInfoList);
            model.depositInfo = await ConvertEntityToModel_Deposit(reserve);
            model.remarksInfo = await ConvertEntityToModel_Remarks(reserve);

            // 部屋割詳細情報 上書きチェック用
            model.hasRoomsNameFile = reserve.NameFileInfo_Room.Count() > 0 ? true : false;

            // Web予約番号
            model.XTravelAgncBkngNum = reserve.ReserveBasicInfo.XTravelAgncBkngNum;

            return model;
        }

        private async Task<StayInfo> ConvertEntityToModel_Stay(ReserveInfo reserve)
        {
            var info = new StayInfo();
            try
            {
                info.ArrivalDate = reserve.ReserveBasicInfo.ArrivalDate;
                info.StayDays = reserve.ReserveBasicInfo.StayDays;
                info.DepartureDate = reserve.ReserveBasicInfo.DepartureDate;
                info.ReserveDate = reserve.ReserveBasicInfo.ReserveDate;
                info.MemberMale = reserve.ReserveBasicInfo.MemberMale;
                info.MemberFemale = reserve.ReserveBasicInfo.MemberFemale;
                info.MemberChildA = reserve.ReserveBasicInfo.MemberChildA;
                info.MemberChildB = reserve.ReserveBasicInfo.MemberChildB;
                info.MemberChildC = reserve.ReserveBasicInfo.MemberChildC;
                info.AdjustmentFlag = reserve.ReserveBasicInfo.AdjustmentFlag;
                info.ReserveStateDivision = reserve.ReserveBasicInfo.ReserveStateDivision;

                // Base
                info.CompanyNo = reserve.ReserveBasicInfo.CompanyNo;
                info.Status = reserve.ReserveBasicInfo.Status;
                info.Version = reserve.ReserveBasicInfo.Version;
                info.Creator = reserve.ReserveBasicInfo.Creator;
                info.Updator = reserve.ReserveBasicInfo.Updator;
                info.Cdt = reserve.ReserveBasicInfo.Cdt;
                info.Udt = reserve.ReserveBasicInfo.Udt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return info;
        }
        private async Task<GuestInfo> ConvertEntityToModel_Guest(ReserveInfo reserve)
        {
            var info = new GuestInfo();
            try
            {
                info.Phone = reserve.NameFileInfo.PhoneNo;
                info.Cellphone = reserve.NameFileInfo.MobilePhoneNo;
                info.GuestName = reserve.NameFileInfo.GuestName;
                info.GuestNameKana = reserve.NameFileInfo.GuestKana;
                info.CompanyName = reserve.NameFileInfo.CompanyName;
                info.Email = reserve.NameFileInfo.Email;
                info.ZipCode = reserve.NameFileInfo.ZipCode;
                info.Address = reserve.NameFileInfo.Address;
                info.CustomerNo = reserve.NameFileInfo.CustomerNo;

                info.UseDate = reserve.NameFileInfo.UseDate;
                info.RouteSEQ = reserve.NameFileInfo.RouteSEQ;
                info.NameSeq = reserve.NameFileInfo.NameSeq;

                // Base
                info.CompanyNo = reserve.NameFileInfo.CompanyNo;
                info.Status = reserve.NameFileInfo.Status;
                info.Version = reserve.NameFileInfo.Version;
                info.Creator = reserve.NameFileInfo.Creator;
                info.Updator = reserve.NameFileInfo.Updator;
                info.Cdt = reserve.NameFileInfo.Cdt;
                info.Udt = reserve.NameFileInfo.Udt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return info;

        }
        private async Task<AgentInfo> ConvertEntityToModel_Agent(ReserveInfo reserve)
        {
            var info = new AgentInfo();
            try
            {
                info.AgentCode = reserve.ReserveBasicInfo.AgentCode;
                info.AgentRemarks = reserve.ReserveBasicInfo.AgentRemarks;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return info;
        }

        private async Task<List<RoomTypeInfo>> ConvertEntityToModel_RoomType(ReserveInfo reserveList)
        {
            var list = new List<RoomTypeInfo>();
            try
            {
                foreach (var rooms in reserveList.RoomTypeInfoList)
                {

                    var info = new RoomTypeInfo();
                    info.UseDate = rooms.UseDate;
                    info.RoomType = rooms.RoomtypeCode;
                    info.Rooms = rooms.Rooms;
                    info.RoomtypeSeq = rooms.RoomtypeSeq;

                    // Base
                    info.CompanyNo = rooms.CompanyNo;
                    info.Status = rooms.Status;
                    info.Version = rooms.Version;
                    info.Creator = rooms.Creator;
                    info.Updator = rooms.Updator;
                    info.Cdt = rooms.Cdt;
                    info.Udt = rooms.Udt;
                    list.Add(info);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return list;
        }

        private async Task<List<SalesDetailsInfo>> ConvertEntityToModel_Item(List<TrnSalesDetailsInfo> itemInfoList)
        {
            var list = new List<SalesDetailsInfo>();
            try
            {
                foreach (var item in itemInfoList)
                {
                    var info = new SalesDetailsInfo();
                    info.DetailsSeq = item.DetailsSeq;
                    info.ItemDivision = item.ItemDivision;
                    info.MealDivision = item.MealDivision;
                    info.UseDate = item.UseDate;
                    info.ItemCode = item.ItemCode;
                    info.PrintName = item.PrintName;
                    info.UnitPrice = item.UnitPrice;
                    info.ItemNumberM = item.ItemNumberM;
                    info.ItemNumberF = item.ItemNumberF;
                    info.ItemNumberC = item.ItemNumberC;
                    info.AmountPrice = item.AmountPrice;
                    info.InsideTaxPrice = item.InsideTaxPrice;
                    info.InsideServicePrice = item.InsideServicePrice;
                    info.OutsideServicePrice = item.OutsideServicePrice;
                    info.TaxDivision = item.TaxDivision;
                    info.TaxRateDivision = item.TaxRateDivision;
                    info.ServiceDivision = item.ServiceDivision;
                    info.SetItemDivision = item.SetItemDivision;
                    info.SetItemSeq = item.SetItemSeq;
                    info.TaxRate = item.TaxRate;
                    info.BillSeparateSeq = item.BillSeparateSeq;
                    info.BillNo = item.BillNo;
                    info.AdjustmentFlag = item.AdjustmentFlag;

                    // Base
                    info.CompanyNo = item.CompanyNo;
                    info.Status = item.Status;
                    info.Version = item.Version;
                    info.Creator = item.Creator;
                    info.Updator = item.Updator;
                    info.Cdt = item.Cdt;
                    info.Udt = item.Udt;

                    list.Add(info);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return list;
        }

        private async Task<List<DepositInfo>> ConvertEntityToModel_Deposit(ReserveInfo reserveList)
        {
            var list = new List<DepositInfo>();
            try
            {
                foreach (var deposit in reserveList.DepositInfoList)
                {
                    var info = new DepositInfo();
                    info.BillSeparateSeq = deposit.BillSeparateSeq;
                    info.BillNo = deposit.BillNo;
                    info.DetailsSeq = deposit.DetailsSeq;
                    info.DepositDate = deposit.DepositDate;
                    info.DenominationCode = deposit.DenominationCode;
                    info.PrintName = deposit.PrintName;
                    info.Price = deposit.AmountPrice;
                    info.BillingRemarks = deposit.Remarks;
                    info.AdjustmentFlag = deposit.AdjustmentFlag;

                    // Base
                    info.CompanyNo = deposit.CompanyNo;
                    info.Status = deposit.Status;
                    info.Version = deposit.Version;
                    info.Creator = deposit.Creator;
                    info.Updator = deposit.Updator;
                    info.Cdt = deposit.Cdt;
                    info.Udt = deposit.Udt;
                    list.Add(info);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return list;
        }
        private async Task<List<RemarksInfo>> ConvertEntityToModel_Remarks(ReserveInfo reserveList)
        {
            var list = new List<RemarksInfo>();
            try
            {
                foreach (var remarks in reserveList.RemarksInfoList)
                {
                    var info = new RemarksInfo();
                    info.NoteSeq = remarks.NoteSeq;
                    info.Remarks = remarks.Remarks;
                    // Base
                    info.CompanyNo = remarks.CompanyNo;
                    info.Status = remarks.Status;
                    info.Version = remarks.Version;
                    info.Creator = remarks.Creator;
                    info.Updator = remarks.Updator;
                    info.Cdt = remarks.Cdt;
                    info.Udt = remarks.Udt;
                    list.Add(info);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return list;
        }
        #endregion

        #region -- マスタ類取得 --
        public async Task<List<MstCodeNameInfo>> GetMasterRoomTypeList(BaseInfo cond)
        {
            return _context.CodeNameInfo
                    .Where(t => t.CompanyNo == cond.CompanyNo
                             && t.DivisionCode == ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT)
                             && t.Status != CommonConst.STATUS_UNUSED)
                    .OrderBy(t => t.DisplayOrder).ToList();
        }

        public async Task<List<MstAgentInfo>> GetMasterAgentList(BaseInfo cond)
        {
            return _context.AgentInfo
                    .Where(t => t.CompanyNo == cond.CompanyNo
                            && t.Status != CommonConst.STATUS_UNUSED)
                    .OrderBy(t => t.DisplayOrder).ToList();
        }

        /// <summary>
        /// 宿泊商品 取得
        /// </summary>
        /// <returns>宿泊商品一覧</returns>
        public async Task<List<MstItemInfo>> GetMasterItemList_StayItem(BaseInfo cond)
        {
            return _context.ItemInfo
                    .Where(i => i.CompanyNo == cond.CompanyNo
                             && (i.ItemDivision == ((int)CommonEnum.ItemDivision.Stay).ToString())
                             && i.Status != CommonConst.STATUS_UNUSED)
                    .OrderBy(o => o.DisplayOrder)
                    .ToList();
        }

        /// <summary>
        /// その他商品 取得
        /// </summary>
        /// <returns>その他商品一覧</returns>
        public async Task<List<MstItemInfo>> GetMasterItemList_OtherItem(BaseInfo cond)
        {
            return _context.ItemInfo
                    .Where(i => i.CompanyNo == cond.CompanyNo
                             && i.ItemDivision != ((int)CommonEnum.ItemDivision.Stay).ToString()
                             && i.ItemDivision != ((int)CommonEnum.ItemDivision.SetItem).ToString()
                             && i.Status != CommonConst.STATUS_UNUSED)
                    .OrderBy(o => o.DisplayOrder)
                    .ToList();
        }

        /// <summary>
        /// セット商品(親) 取得
        /// </summary>
        /// <returns>セット商品(親)一覧</returns>
        public async Task<List<MstItemInfo>> GetMasterItemList_SetItem(BaseInfo cond)
        {
            return _context.ItemInfo
                    .Where(i => i.CompanyNo == cond.CompanyNo
                             && i.ItemDivision == ((int)CommonEnum.ItemDivision.SetItem).ToString()
                             && i.Status != CommonConst.STATUS_UNUSED)
                    .OrderBy(o => o.DisplayOrder)
                    .ToList();
        }

        /// <summary>
        /// セット商品(子) 取得
        /// </summary>
        /// <returns>セット商品(子)一覧</returns>
        public async Task<List<MstSetItemInfo>> GetMasterSetItemList(BaseInfo cond)
        {
            return _context.SetItemInfo
                    .Where(i => i.CompanyNo == cond.CompanyNo
                             && i.Status != CommonConst.STATUS_UNUSED)
                    .OrderBy(o => o.SetItemCode)
                    .ThenBy(o2 => o2.DisplayOrder)
                    .ThenBy(o3 => o3.Seq)
                    .AsNoTracking()
                    .ToList();
        }

        /// <summary>
        /// 金種 取得
        /// </summary>
        /// <returns>金種一覧</returns>
        public async Task<List<MstDenominationInfo>> GetMasterDenominationCodeList(BaseInfo cond)
        {
            return _context.DenominationInfo
                    .Where(t => t.CompanyNo == cond.CompanyNo
                             && t.Status != CommonConst.STATUS_UNUSED)
                    .OrderBy(t => t.DisplayOrder).ToList();
        }
        #endregion

        /// <summary>
        /// 採番
        /// </summary>
        /// <param name="companyNo">会社番号</param>
        public MstCompanyInfo Numbering(string companyNo)
        {
            var info = new MstCompanyInfo();

            // 採番
            info = _context.CompanyInfo.Single(x => x.CompanyNo == companyNo);

            // +1して更新
            info.LastReserveNo = (int.Parse(info.LastReserveNo) + 1).ToString("00000000");
            //info.LastCustomerNo = (int.Parse(info.LastCustomerNo) + 1).ToString("0000000000");

            _context.CompanyInfo.Update(info);
            _context.SaveChanges();

            return info;
        }

        /// <summary>
        /// 登録
        /// </summary>
        public async Task<ResultInfo> InsertReserveInfo(ReserveModel reserveModel)
        {
            // 予約番号・顧客番号 採番
            var info = Numbering(reserveModel.CompanyNo);
            reserveModel.ReserveNo = info.LastReserveNo;
            //reserveModel.guestInfo.CustomerNo = info.LastCustomerNo;

            reserveModel.Cdt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
            reserveModel.Udt = reserveModel.Cdt;

            var resultInfo = new ResultInfo();
            resultInfo.reserveNo = reserveModel.ReserveNo;
            resultInfo.assignResult = 0;

            // Model→Entityに変換
            var reserveInfo = ConvertModelToEntity(reserveModel, updateFlag: false);

            // トランザクション作成
            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                    // 変更履歴
                    var list = await _reserveLogService.MakeReserveLog(CommonEnum.ReserveLogProcessDivision.Add);
                    var logseq = await _reserveLogService.NumberingLogSeq(reserveModel.CompanyNo, reserveModel.ReserveNo);
                    var seqGroup = await _reserveLogService.NumberingSeqGroup(reserveModel.CompanyNo, reserveModel.ReserveNo);

                    foreach (var item in list)
                    {
                        item.CompanyNo = reserveModel.CompanyNo;
                        item.ReserveNo = reserveModel.ReserveNo;
                        item.Status = CommonConst.STATUS_USED;
                        item.Version = 0;
                        item.Creator = reserveModel.Creator;
                        item.Cdt = reserveModel.Cdt;
                        item.Updator = reserveModel.Creator;
                        item.Udt = reserveModel.Cdt;
                        item.ReserveLogSeq = logseq;
                        item.SeqGroup = seqGroup;

                        _context.ReserveLogInfo.Add(item);
                        _context.SaveChanges();
                        logseq++;
                    }

                    // 予約基本
                    _context.ReserveBasicInfo.Add(reserveInfo.ReserveBasicInfo);
                    _context.SaveChanges();

                    // 氏名ファイル
                    _context.NameFileInfo.Add(reserveInfo.NameFileInfo);
                    _context.SaveChanges();

                    // 部屋タイプ
                    foreach (var roomType in reserveInfo.RoomTypeInfoList)
                    {
                        _context.ReserveRoomtypeInfo.Add(roomType);
                        _context.SaveChanges();
                    }

                    // アサイン
                    foreach (var assign in reserveInfo.AssignInfoList)
                    {
                        if (!assign.RoomNo.IsNullOrEmpty()) {

                            // 部屋が空いていれば追加
                            var useCount = _context.ReserveAssignInfo.Where(x => x.CompanyNo == assign.CompanyNo
                                                                            && x.UseDate == assign.UseDate
                                                                            && x.RoomNo == assign.RoomNo)
                                                                     .Count();
                            if (useCount != 0) {

                                // アサインチェック
                                var wkInfo = await _roomsService.CheckRoomStateAndSearchAssignableRoom(assign);
                                if (wkInfo.RoomNo.IsBlanks()) {
                                    // 同一部屋タイプが見つからない為、未アサイン
                                    resultInfo.assignResult = 1;
                                    continue;
                                }

                                if (assign.RoomNo != wkInfo.RoomNo) {
                                    // 同一部屋タイプの別部屋にアサイン
                                    resultInfo.assignResult = 2;
                                    assign.RoomNo = wkInfo.RoomNo;
                                }

                            } 
                        }

                        _context.ReserveAssignInfo.Add(assign);
                        _context.SaveChanges();
                    }

                    // 商品情報
                    foreach (var salesDetails in reserveInfo.SalesDetailsInfoList)
                    {
                        _context.SalesDetailsInfo.Add(salesDetails);
                        _context.SaveChanges();
                    }

                    // 入金情報
                    foreach (var deposit in reserveInfo.DepositInfoList)
                    {
                        _context.DepositDetailsInfo.Add(deposit);
                        _context.SaveChanges();
                    }

                    // 備考
                    foreach (var remarks in reserveInfo.RemarksInfoList)
                    {
                        _context.ReserveNoteInfo.Add(remarks);
                        _context.SaveChanges();
                    }

                    tran.Commit();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    tran.Rollback();
                    return null;
                }
            }

            return resultInfo;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public async Task<ResultInfo> UpdateReserveInfo(ReserveModel reserveModel)
        {

            var resultInfo = new ResultInfo();
            resultInfo.reserveNo = reserveModel.ReserveNo;
            resultInfo.assignResult = 0;

            reserveModel.Udt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

            // Model→Entityに変換
            var reserveInfo = ConvertModelToEntity(reserveModel, updateFlag: true);

            // トランザクション作成
            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                    // 変更履歴
                    var list = await _reserveLogService.MakeReserveLog(CommonEnum.ReserveLogProcessDivision.Update, reserveInfo);
                    var logseq = await _reserveLogService.NumberingLogSeq(reserveModel.CompanyNo, reserveModel.ReserveNo);
                    var seqGroup = await _reserveLogService.NumberingSeqGroup(reserveModel.CompanyNo, reserveModel.ReserveNo);

                    foreach (var item in list)
                    {
                        item.CompanyNo = reserveModel.CompanyNo;
                        item.ReserveNo = reserveModel.ReserveNo;
                        item.Status = CommonConst.STATUS_USED;
                        item.Version = 0;
                        item.Creator = reserveModel.Updator;
                        item.Cdt = reserveModel.Udt;
                        item.Updator = reserveModel.Updator;
                        item.Udt = reserveModel.Udt;
                        item.ReserveLogSeq = logseq;
                        item.SeqGroup = seqGroup;

                        _context.ReserveLogInfo.Add(item);
                        _context.SaveChanges();
                        logseq++;
                    }

                    // ----- 予約基本 ( Update ) -----

                    var version = _context.ReserveBasicInfo
                                            .Where(x => x.CompanyNo == reserveModel.CompanyNo
                                                     && x.ReserveNo == reserveModel.ReserveNo)
                                            .Select(s => s.Version)
                                            .SingleOrDefault();

                    // 排他チェック
                    if (version != reserveModel.Version) {
                        resultInfo.reserveResult = (int)CommonEnum.DBUpdateResult.VersionError;
                        return resultInfo;
                    }

                    _context.ReserveBasicInfo.Update(reserveInfo.ReserveBasicInfo);
                    _context.SaveChanges();

                    // ----- 氏名ファイル ( Update ) -----
                    _context.NameFileInfo.Update(reserveInfo.NameFileInfo);
                    _context.SaveChanges();

                    if (reserveModel.guestInfo.OverwriteFlag) {
                        var query = "Delete From trn_name_file";
                        query += " Where company_no = '{0}'";
                        query += "   And reserve_no = '{1}'";
                        query += "   And use_date != '{2}'";
                        query += "   And route_seq != {3}";
                        query = query.FillFormat(reserveInfo.NameFileInfo.CompanyNo, reserveInfo.NameFileInfo.ReserveNo, CommonConst.USE_DATE_EMPTY, CommonConst.DEFAULT_ROUTE_SEQ.ToString());

                        using (var command = _context.Database.GetDbConnection().CreateCommand()) {
                            command.CommandText = query;
                            _context.Database.OpenConnection();
                            try {
                                var count = command.ExecuteNonQuery();
                            } catch (Exception e) {
                                Console.WriteLine(e.ToString());
                            } finally {
                                _context.Database.CloseConnection();
                            }
                        }

                    }

                    // ----- 部屋タイプ -----
                    // 削除
                    var deleteRoomTypeList = reserveInfo.RoomTypeInfoList.Where(x => x.DeleteFlag == true);
                    foreach (var roomType in deleteRoomTypeList)
                    {
                        _context.ReserveRoomtypeInfo.Remove(roomType);
                        _context.SaveChanges();
                    }
                    // 更新
                    var updateRoomTypeList = reserveInfo.RoomTypeInfoList.Where(x => x.UpdateFlag == true);
                    foreach (var roomType in updateRoomTypeList)
                    {
                        _context.ReserveRoomtypeInfo.Update(roomType);
                        _context.SaveChanges();
                    }
                    // 追加
                    var addRoomTypeList = reserveInfo.RoomTypeInfoList.Where(x => x.AddFlag == true);
                    foreach (var roomType in addRoomTypeList)
                    {
                        _context.ReserveRoomtypeInfo.Add(roomType);
                        _context.SaveChanges();
                    }

                    // ----- アサイン -----
                    // 更新
                    var updateAssignList = reserveInfo.AssignInfoList.Where(x => x.UpdateFlag == true);
                    foreach (var assign in updateAssignList) {
                        _context.ReserveAssignInfo.Update(assign);
                        _context.SaveChanges();
                    }

                    // アサイン部屋番号チェック用
                    var wkAssignList = _context.ReserveAssignInfo.Where(x => x.CompanyNo == reserveModel.CompanyNo
                                                                         && x.ReserveNo == reserveModel.ReserveNo)
                                                                 .AsNoTracking()
                                                                 .ToList();

                    // 削除
                    var deleteAssignList = reserveInfo.AssignInfoList.Where(x => x.DeleteFlag == true);
                    foreach (var assign in deleteAssignList) {

                        //_context.ReserveAssignInfo.Remove(assign);
                        //_context.SaveChanges();

                        var query = "Delete From trn_reserve_assign";
                        query += " Where company_no = '{0}'";
                        query += "   And reserve_no = '{1}'";
                        query += "   And use_date = '{2}'";
                        query += "   And route_seq = {3}";
                        query = query.FillFormat(assign.CompanyNo, assign.ReserveNo, assign.UseDate, assign.RouteSEQ.ToString());

                        using (var command = _context.Database.GetDbConnection().CreateCommand()) {
                            command.CommandText = query;
                            _context.Database.OpenConnection();
                            try {
                                var count = command.ExecuteNonQuery();
                            } catch (Exception e) {
                                Console.WriteLine(e.ToString());
                            } finally {
                                _context.Database.CloseConnection();
                            }
                        }
                    }
                    // 追加
                    var addAssignList = reserveInfo.AssignInfoList.Where(x => x.AddFlag == true);
                    foreach (var assign in addAssignList) {

                        _context.ReserveAssignInfo.Add(assign);
                        _context.SaveChanges();

                        if (wkAssignList.Count == 0) continue;

                        // 同じ部屋タイプSEQのリスト
                        var assignList = wkAssignList.Where(x => x.RoomtypeCode == assign.RoomtypeCode 
                                                              && x.RouteSEQ == assign.RouteSEQ).ToList();
                        if (assignList.Count == 0) continue;


                        // 利用日が一番早い部屋番号
                        var wkInfo = assignList.Where(x => x.RoomNo != null).OrderBy(y => y.UseDate).FirstOrDefault();
                        var wkRoomNo = (wkInfo != null) ? wkInfo.RoomNo : string.Empty;
                        var wkRoomState = wkInfo.RoomStateClass;
                        if (wkRoomNo.Length == 0) continue;

                        foreach (var info in assignList) {

                            if (info.RoomNo != null && info.RoomNo.Length > 0) {
                                wkRoomNo = info.RoomNo;
                                continue;

                            } else {

                                info.RoomNo = wkRoomNo;

                            }

                            // 部屋が空いていれば追加
                            var useCount = _context.ReserveAssignInfo.Where(x => x.CompanyNo == info.CompanyNo
                                                                            && x.UseDate == info.UseDate
                                                                            && x.RoomNo == info.RoomNo)
                                                                     .Count();
                            if (useCount != 0) {

                                // アサインチェック
                                var checkInfo = await _roomsService.CheckRoomStateAndSearchAssignableRoom((TrnReserveAssignInfo)info);
                                if (checkInfo.RoomNo.IsBlanks()) {
                                    // 同一部屋タイプが見つからない為、未アサイン
                                    resultInfo.assignResult = 1;
                                    continue;
                                }

                                if (info.RoomNo != checkInfo.RoomNo) {
                                    // 同一部屋タイプの別部屋にアサイン
                                    resultInfo.assignResult = 2;
                                    info.RoomNo = checkInfo.RoomNo;
                                }

                            }

                            info.RoomStateClass = wkRoomState;
                            info.Udt = reserveInfo.ReserveBasicInfo.Udt;
                            info.Updator = reserveInfo.ReserveBasicInfo.Updator;
                            info.Version += 1;

                            _context.ReserveAssignInfo.Update(info);
                            _context.SaveChanges();
                        }
                    }

                    // ----- 商品情報 -----
                    // 削除
                    var deleteItemList = reserveInfo.SalesDetailsInfoList.Where(x => x.DeleteFlag == true);
                    foreach (var salesDetails in deleteItemList)
                    {
                        _context.SalesDetailsInfo.Remove(salesDetails);
                        _context.SaveChanges();
                    }

                    // 更新
                    var updateItemList = reserveInfo.SalesDetailsInfoList.Where(x => x.UpdateFlag == true);
                    foreach (var salesDetails in updateItemList)
                    {
                        _context.SalesDetailsInfo.Update(salesDetails);
                        _context.SaveChanges();
                    }

                    // 追加
                    var addItemList = reserveInfo.SalesDetailsInfoList.Where(x => x.AddFlag == true);
                    foreach (var salesDetails in addItemList)
                    {
                        _context.SalesDetailsInfo.Add(salesDetails);
                        _context.SaveChanges();
                    }

                    // ----- 入金情報 -----
                    // 削除
                    var deleteDepositList = reserveInfo.DepositInfoList.Where(x => x.DeleteFlag == true);
                    foreach (var deposit in deleteDepositList)
                    {
                        _context.DepositDetailsInfo.Remove(deposit);
                        _context.SaveChanges();
                    }

                    // 更新
                    var updateDepositList = reserveInfo.DepositInfoList.Where(x => x.UpdateFlag == true);
                    foreach (var deposit in updateDepositList)
                    {
                        _context.DepositDetailsInfo.Update(deposit);
                        _context.SaveChanges();
                    }

                    // 追加
                    var addDepositList = reserveInfo.DepositInfoList.Where(x => x.AddFlag == true);
                    foreach (var deposit in addDepositList)
                    {
                        _context.DepositDetailsInfo.Add(deposit);
                        _context.SaveChanges();
                    }

                    // ----- 備考 -----

                    // 削除
                    var deleteRemarksList = reserveInfo.RemarksInfoList.Where(x => x.DeleteFlag == true);
                    foreach (var remarks in deleteRemarksList)
                    {
                        _context.ReserveNoteInfo.Remove(remarks);
                        _context.SaveChanges();
                    }

                    // 更新
                    var updateRemarksList = reserveInfo.RemarksInfoList.Where(x => x.UpdateFlag == true);
                    foreach (var remarks in updateRemarksList)
                    {
                        _context.ReserveNoteInfo.Update(remarks);
                        _context.SaveChanges();
                    }

                    // 追加
                    var addRemarksList = reserveInfo.RemarksInfoList.Where(x => x.AddFlag == true);
                    foreach (var remarks in addRemarksList)
                    {
                        _context.ReserveNoteInfo.Add(remarks);
                        _context.SaveChanges();
                    }

                    tran.Commit();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    tran.Rollback();
                    resultInfo.reserveResult = (int)CommonEnum.DBUpdateResult.Error;
                    return resultInfo;
                }
            }

            resultInfo.reserveResult = (int)CommonEnum.DBUpdateResult.Success;
            return resultInfo;
        }

        private ReserveInfo ConvertModelToEntity(ReserveModel reserveModel, bool updateFlag)
        {
            var reserveInfo = new ReserveInfo();

            reserveInfo.ReserveBasicInfo = ConvertModelToEntity_ReserveBasic(reserveModel, updateFlag);
            reserveInfo.NameFileInfo = ConvertModelToEntity_NameFile(reserveModel, updateFlag);
            reserveInfo.RoomTypeInfoList = ConvertModelToEntity_RoomType(reserveModel, updateFlag);
            reserveInfo.AssignInfoList = ConvertModelToEntity_Assign(reserveModel, updateFlag);
            reserveInfo.SalesDetailsInfoList = ConvertModelToEntity_SalesDetailsInfo(reserveModel, updateFlag);
            reserveInfo.DepositInfoList = ConvertModelToEntity_Deposit(reserveModel, updateFlag);
            reserveInfo.RemarksInfoList = ConvertModelToEntity_Remarks(reserveModel, updateFlag);

            return reserveInfo;
        }

        private TrnReserveBasicInfo ConvertModelToEntity_ReserveBasic(ReserveModel reserveModel, bool updateFlag)
        {
            var rsvBasic = new TrnReserveBasicInfo();
            var info = reserveModel.stayInfo;

            rsvBasic.CompanyNo = reserveModel.CompanyNo;
            rsvBasic.ReserveNo = reserveModel.ReserveNo;
            rsvBasic.Updator = reserveModel.Updator;
            rsvBasic.Udt = reserveModel.Udt;

            if (updateFlag)
            {
                rsvBasic.Creator = (info.Creator.IsNullOrEmpty()) ? reserveModel.Updator : info.Creator;
                rsvBasic.Cdt = (info.Cdt.IsNullOrEmpty()) ? reserveModel.Udt : info.Cdt;
                rsvBasic.Status = reserveModel.Status;
                rsvBasic.Version = reserveModel.Version + 1;
                rsvBasic.AdjustmentFlag = info.AdjustmentFlag;

            }
            else
            {
                rsvBasic.Creator = reserveModel.Creator;
                rsvBasic.Cdt = reserveModel.Cdt;
                rsvBasic.Status = CommonConst.STATUS_USED;
                rsvBasic.Version = reserveModel.Version;
                rsvBasic.AdjustmentFlag = CommonConst.NOT_ADJUSTMENTED;
            }

            rsvBasic.ReserveDate = info.ReserveDate;
            rsvBasic.ReserveStateDivision = ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString();
            rsvBasic.ArrivalDate = info.ArrivalDate;
            rsvBasic.DepartureDate = info.DepartureDate;
            rsvBasic.StayDays = info.StayDays;
            rsvBasic.MemberMale = info.MemberMale;
            rsvBasic.MemberFemale = info.MemberFemale;
            rsvBasic.MemberChildA = info.MemberChildA;
            rsvBasic.MemberChildB = info.MemberChildB;
            rsvBasic.MemberChildC = info.MemberChildC;
            rsvBasic.CustomerNo = reserveModel.guestInfo.CustomerNo;
            rsvBasic.AgentCode = reserveModel.agentInfo.AgentCode;
            rsvBasic.AgentRemarks = reserveModel.agentInfo.AgentRemarks;

            return rsvBasic;
        }

        private TrnNameFileInfo ConvertModelToEntity_NameFile(ReserveModel reserveModel, bool updateFlag)
        {
            var nameFile = new TrnNameFileInfo();
            var info = reserveModel.guestInfo;

            nameFile.CompanyNo = reserveModel.CompanyNo;
            nameFile.ReserveNo = reserveModel.ReserveNo;
            nameFile.CustomerNo = info.CustomerNo;
            nameFile.Updator = reserveModel.Updator;
            nameFile.Udt = reserveModel.Udt;

            if (updateFlag)
            {
                nameFile.NameSeq = info.NameSeq;
                nameFile.Creator = (info.Creator.IsNullOrEmpty()) ? reserveModel.Updator : info.Creator;
                nameFile.Cdt = (info.Cdt.IsNullOrEmpty()) ? reserveModel.Udt : info.Cdt;
                nameFile.Status = reserveModel.Status;
                nameFile.Version = reserveModel.Version + 1;
            }
            else
            {
                nameFile.NameSeq = 1;   // HACK： 追加機能時に実装が必要
                nameFile.Creator = reserveModel.Creator;
                nameFile.Cdt = reserveModel.Cdt;
                nameFile.Status = CommonConst.STATUS_USED;
                nameFile.Version = reserveModel.Version;
            }
            nameFile.GuestName = info.GuestName;
            nameFile.GuestKana = info.GuestNameKana;
            nameFile.ZipCode = info.ZipCode;
            nameFile.Address = info.Address;
            nameFile.PhoneNo = info.Phone;
            nameFile.MobilePhoneNo = info.Cellphone;
            nameFile.Email = info.Email;
            nameFile.CompanyName = info.CompanyName;

            nameFile.UseDate = info.UseDate;
            nameFile.RouteSEQ = info.RouteSEQ;

            return nameFile;
        }

        private List<TrnReserveRoomtypeInfo> ConvertModelToEntity_RoomType(ReserveModel reserveModel, bool updateFlag)
        {
            var roomTypeList = new List<TrnReserveRoomtypeInfo>();

            foreach (var info in reserveModel.roomTypeInfo)
            {

                var roomTypeInfo = new TrnReserveRoomtypeInfo();

                roomTypeInfo.CompanyNo = reserveModel.CompanyNo;
                roomTypeInfo.ReserveNo = reserveModel.ReserveNo;
                roomTypeInfo.Updator = reserveModel.Updator;
                roomTypeInfo.Udt = reserveModel.Udt;

                if (updateFlag)
                {
                    roomTypeInfo.Creator = (info.Creator.IsNullOrEmpty()) ? reserveModel.Updator : info.Creator;
                    roomTypeInfo.Cdt = (info.Cdt.IsNullOrEmpty()) ? reserveModel.Udt : info.Cdt;
                    roomTypeInfo.Status = reserveModel.Status;
                    roomTypeInfo.Version = info.Version + 1;
                }
                else
                {
                    roomTypeInfo.Creator = reserveModel.Creator;
                    roomTypeInfo.Cdt = reserveModel.Cdt;
                    roomTypeInfo.Status = CommonConst.STATUS_USED;
                    roomTypeInfo.Version = reserveModel.Version;
                }

                roomTypeInfo.UseDate = info.UseDate;
                roomTypeInfo.RoomtypeCode = info.RoomType;
                roomTypeInfo.Rooms = info.Rooms;
                roomTypeInfo.RoomtypeSeq = info.RoomtypeSeq;

                // 更新対象チェック用
                roomTypeInfo.AddFlag = info.AddFlag;
                roomTypeInfo.UpdateFlag = info.UpdateFlag;
                roomTypeInfo.DeleteFlag = info.DeleteFlag;

                roomTypeList.Add(roomTypeInfo);
            }

            return roomTypeList;
        }

        private List<TrnReserveAssignInfo> ConvertModelToEntity_Assign(ReserveModel reserveModel, bool updateFlag)
        {
            var assignList = new List<TrnReserveAssignInfo>();

            foreach (var info in reserveModel.assignList)
            {

                var assignInfo = new TrnReserveAssignInfo();

                assignInfo.CompanyNo = reserveModel.CompanyNo;
                assignInfo.ReserveNo = reserveModel.ReserveNo;
                assignInfo.Updator = reserveModel.Updator;
                assignInfo.Udt = reserveModel.Udt;
                assignInfo.Status = CommonConst.STATUS_USED;

                if (updateFlag)
                {
                    assignInfo.Creator = (info.Creator.IsNullOrEmpty()) ? reserveModel.Updator : info.Creator;
                    assignInfo.Cdt = (info.Cdt.IsNullOrEmpty()) ? reserveModel.Udt : info.Cdt;
                    assignInfo.Version = info.Version + 1;
                }
                else
                {
                    assignInfo.Creator = reserveModel.Creator;
                    assignInfo.Cdt = reserveModel.Cdt;
                    assignInfo.Version = reserveModel.Version;
                }

                assignInfo.UseDate = info.UseDate;
                assignInfo.RoomNo = info.RoomNo;
                assignInfo.RoomtypeCode = info.RoomtypeCode;
                assignInfo.OrgRoomtypeCode = info.OrgRoomtypeCode;
                assignInfo.RouteSEQ = info.RouteSEQ;
                assignInfo.RoomtypeSeq = info.RoomtypeSeq;

                assignInfo.RoomStateClass = info.RoomStateClass;

                assignInfo.Email = info.Email;

                // TODO: 追加機能実装時考慮が必要
                assignInfo.GuestName = info.GuestName;
                assignInfo.MemberMale = info.MemberMale;
                assignInfo.MemberFemale = info.MemberFemale;
                assignInfo.MemberChildA = info.MemberChildA;
                assignInfo.MemberChildB = info.MemberChildB;
                assignInfo.MemberChildC = info.MemberChildC;

                assignInfo.AddFlag = info.AddFlag;
                assignInfo.UpdateFlag = info.UpdateFlag;
                assignInfo.DeleteFlag = info.DeleteFlag;

                assignList.Add(assignInfo);
            }

            return assignList;
        }

        private List<TrnSalesDetailsInfo> ConvertModelToEntity_SalesDetailsInfo(ReserveModel reserveModel, bool updateFlag)
        {
            var salesDetailsList = new List<TrnSalesDetailsInfo>();

            foreach (var info in reserveModel.itemInfo) {

                var item = new TrnSalesDetailsInfo();

                item.CompanyNo = reserveModel.CompanyNo;
                item.ReserveNo = reserveModel.ReserveNo;
                item.Updator = reserveModel.Updator;
                item.Udt = reserveModel.Udt;
                item.Status = CommonConst.STATUS_USED;

                if (updateFlag) {
                    item.Creator = (info.Creator.IsNullOrEmpty()) ? reserveModel.Updator : info.Creator;
                    item.Cdt = (info.Cdt.IsNullOrEmpty()) ? reserveModel.Udt : info.Cdt;
                    item.Version = info.Version + 1;
                } else {
                    item.Creator = reserveModel.Creator;
                    item.Cdt = reserveModel.Cdt;
                    item.Version = reserveModel.Version;
                }

                item.DetailsSeq = info.DetailsSeq;
                item.ItemDivision = info.ItemDivision;
                item.MealDivision = info.MealDivision;
                item.UseDate = info.UseDate;

                item.ItemCode = info.ItemCode;
                item.PrintName = info.PrintName;
                item.UnitPrice = info.UnitPrice;
                item.ItemNumberM = info.ItemNumberM;
                item.ItemNumberF = info.ItemNumberF;
                item.ItemNumberC = info.ItemNumberC;
                item.AmountPrice = info.AmountPrice;
                item.MealDivision = info.MealDivision;
                item.InsideTaxPrice = info.InsideTaxPrice;
                item.InsideServicePrice = info.InsideServicePrice;
                item.OutsideServicePrice = info.OutsideServicePrice;
                item.TaxDivision = info.TaxDivision;
                item.ServiceDivision = info.ServiceDivision;
                item.SetItemDivision = info.SetItemDivision;
                item.SetItemSeq = info.SetItemSeq;
                item.TaxRateDivision = info.TaxRateDivision;
                item.TaxRate = info.TaxRate;
                item.BillSeparateSeq = info.BillSeparateSeq;
                item.BillNo = info.BillNo;
                item.AdjustmentFlag = info.AdjustmentFlag;

                // 更新対象チェック用
                item.AddFlag = info.AddFlag;
                item.UpdateFlag = info.UpdateFlag;
                item.DeleteFlag = info.DeleteFlag;

                salesDetailsList.Add(item);
            }

            return salesDetailsList;
        }


        private List<TrnDepositDetailsInfo> ConvertModelToEntity_Deposit(ReserveModel reserveModel, bool updateFlag)
        {
            var depositList = new List<TrnDepositDetailsInfo>();

            foreach (var info in reserveModel.depositInfo)
            {

                var depositInfo = new TrnDepositDetailsInfo();

                depositInfo.CompanyNo = reserveModel.CompanyNo;
                depositInfo.ReserveNo = reserveModel.ReserveNo;
                depositInfo.Updator = reserveModel.Updator;
                depositInfo.Udt = reserveModel.Udt;
                depositInfo.Status = CommonConst.STATUS_USED;

                if (updateFlag)
                {
                    depositInfo.Creator = (info.Creator.IsNullOrEmpty()) ? reserveModel.Updator : info.Creator;
                    depositInfo.Cdt = (info.Cdt.IsNullOrEmpty()) ? reserveModel.Udt : info.Cdt;
                    depositInfo.Version = info.Version + 1;
                }
                else
                {
                    depositInfo.Creator = reserveModel.Creator;
                    depositInfo.Cdt = reserveModel.Cdt;
                    depositInfo.Version = reserveModel.Version;
                }

                depositInfo.BillSeparateSeq = info.BillSeparateSeq;
                depositInfo.BillNo = info.BillNo;
                depositInfo.DetailsSeq = info.DetailsSeq;
                depositInfo.DepositDate = info.DepositDate;
                depositInfo.DenominationCode = info.DenominationCode;
                depositInfo.PrintName = info.PrintName;
                depositInfo.AmountPrice = info.Price;
                depositInfo.Remarks = info.BillingRemarks;
                depositInfo.AdjustmentFlag = info.AdjustmentFlag;

                // 更新対象チェック用
                depositInfo.AddFlag = info.AddFlag;
                depositInfo.UpdateFlag = info.UpdateFlag;
                depositInfo.DeleteFlag = info.DeleteFlag;

                depositList.Add(depositInfo);
            }

            return depositList;
        }

        private List<TrnReserveNoteInfo> ConvertModelToEntity_Remarks(ReserveModel reserveModel, bool updateFlag)
        {
            var roomTypeList = new List<TrnReserveNoteInfo>();

            foreach (var info in reserveModel.remarksInfo)
            {

                var remarksInfo = new TrnReserveNoteInfo();

                remarksInfo.CompanyNo = reserveModel.CompanyNo;
                remarksInfo.ReserveNo = reserveModel.ReserveNo;
                remarksInfo.Updator = reserveModel.Updator;
                remarksInfo.Udt = reserveModel.Udt;
                remarksInfo.Status = CommonConst.STATUS_USED;

                if (updateFlag)
                {
                    remarksInfo.Creator = (info.Creator.IsNullOrEmpty()) ? reserveModel.Updator : info.Creator;
                    remarksInfo.Cdt = (info.Cdt.IsNullOrEmpty()) ? reserveModel.Udt : info.Cdt;
                    remarksInfo.Version = info.Version + 1;
                }
                else
                {
                    remarksInfo.Creator = reserveModel.Creator;
                    remarksInfo.Cdt = reserveModel.Cdt;
                    remarksInfo.Version = reserveModel.Version;
                }

                remarksInfo.NoteSeq = info.NoteSeq;
                remarksInfo.Remarks = info.Remarks;

                // 更新対象チェック用
                remarksInfo.AddFlag = info.AddFlag;
                remarksInfo.UpdateFlag = info.UpdateFlag;
                remarksInfo.DeleteFlag = info.DeleteFlag;

                roomTypeList.Add(remarksInfo);
            }

            return roomTypeList;
        }


        public async Task<int> InsertSalesDetailsInfo(TrnSalesDetailsInfo salesDetailsInfo, MstUserInfo loginUser, string reserveNo)
        {
            _context.SalesDetailsInfo.RemoveRange(_context.SalesDetailsInfo.Where(t => t.ReserveNo == reserveNo));

            var dateTime = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
            var info = new TrnSalesDetailsInfo();

            // SEQ 
            info.DetailsSeq = salesDetailsInfo.DetailsSeq;
            // 商品区分 
            info.ItemDivision = salesDetailsInfo.ItemDivision;
            // 料理区分 
            info.MealDivision = salesDetailsInfo.MealDivision;
            // 利用日 
            info.UseDate = salesDetailsInfo.UseDate;
            // 商品コード 
            info.ItemCode = salesDetailsInfo.ItemCode;
            // 印字用名称 
            info.PrintName = salesDetailsInfo.PrintName;
            // 単価 
            info.UnitPrice = salesDetailsInfo.UnitPrice;
            // 人数(男) 
            info.ItemNumberM = salesDetailsInfo.ItemNumberM;
            // 人数(女) 
            info.ItemNumberF = salesDetailsInfo.ItemNumberF;
            // 人数(子) 
            info.ItemNumberC = salesDetailsInfo.ItemNumberC;
            // 金額 
            info.AmountPrice = salesDetailsInfo.AmountPrice;
            // 内消費税額 
            info.InsideTaxPrice = salesDetailsInfo.InsideServicePrice;
            // 内サービス料額 
            info.InsideServicePrice = salesDetailsInfo.InsideServicePrice;
            // 外サービス料額 
            info.OutsideServicePrice = salesDetailsInfo.OutsideServicePrice;
            // 消費税率 
            info.TaxRate = salesDetailsInfo.TaxRate;
            // ビル分割SEQ 
            info.BillSeparateSeq = salesDetailsInfo.BillSeparateSeq;
            // 精算フラグ 
            info.AdjustmentFlag = salesDetailsInfo.AdjustmentFlag;
            // 更新者
            info.Updator = loginUser.Updator;
            // 更新日時
            info.Udt = dateTime;

            if (info.ReserveNo == null)
            {
                // 会社番号 
                info.CompanyNo = salesDetailsInfo.CompanyNo;
                // 予約番号
                info.ReserveNo = reserveNo;

                // 作成者
                info.Creator = loginUser.Creator;
                // 作成日時
                info.Cdt = dateTime;

                _context.SalesDetailsInfo.Add(info);
            }

            return _context.SaveChanges();
        }

        /// <summary>
        /// 予約取消
        /// </summary>
        /// <param name="cond"></param>
        /// <returns>ture:正常, false:異常</returns>
        public async Task<int> UpdatetReserveInfo_ReserveCancel(StayInfo cond)
        {


            // トランザクション作成
            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                    // 変更履歴
                    var list = await _reserveLogService.MakeReserveLog(CommonEnum.ReserveLogProcessDivision.Delete);
                    var logseq = await _reserveLogService.NumberingLogSeq(cond.CompanyNo, cond.ReserveNo);
                    var seqGroup = await _reserveLogService.NumberingSeqGroup(cond.CompanyNo, cond.ReserveNo);

                    foreach (var item in list)
                    {
                        item.CompanyNo = cond.CompanyNo;
                        item.ReserveNo = cond.ReserveNo;
                        item.Status = CommonConst.STATUS_USED;
                        item.Version = 0;
                        item.Creator = cond.Updator;
                        item.Cdt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                        item.Updator = cond.Updator;
                        item.Udt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                        item.ReserveLogSeq = logseq;
                        item.SeqGroup = seqGroup;

                        _context.ReserveLogInfo.Add(item);
                        _context.SaveChanges();
                    }

                    // 予約基本
                    var basic = _context.ReserveBasicInfo
                                    .SingleOrDefault(x => x.CompanyNo == cond.CompanyNo
                                                       && x.ReserveNo == cond.ReserveNo
                                                       && x.Version == cond.Version);
                    if (basic == null) return (int)CommonEnum.DBUpdateResult.VersionError;

                    basic.ReserveStateDivision = cond.ReserveStateDivision;
                    basic.Updator = cond.Updator;
                    basic.Udt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                    basic.Version++;
                    _context.ReserveBasicInfo.Update(basic);
                    _context.SaveChanges();

                    // 日別アサイン
                    var assign = _context.ReserveAssignInfo
                                    .Where(x => x.CompanyNo == cond.CompanyNo
                                             && x.ReserveNo == cond.ReserveNo)
                                    .ToList();
                    assign.ForEach(a => _context.ReserveAssignInfo.Remove(a));
                    _context.SaveChanges();

                    // 会場予約
                    var facility = _context.ReserveFacilityInfo
                                    .Where(x => x.CompanyNo == cond.CompanyNo
                                             && x.ReserveNo == cond.ReserveNo)
                                    .ToList();
                    _context.ReserveFacilityInfo.RemoveRange(facility);
                    _context.SaveChanges();


                    tran.Commit();

                    return (int)CommonEnum.DBUpdateResult.Success;

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Console.WriteLine(ex.ToString());
                    return (int)CommonEnum.DBUpdateResult.Error; ;
                }
            }
        }

        /// <summary>
        /// 売上明細の精算フラグを更新
        /// </summary>
        /// <param name="cond"></param>
        /// <returns>ture:正常, false:異常</returns>
        public async Task<int> UpdatetSalesDetailsInfo_AdjustmentFlag(SalesDetailsInfo cond)
        {
            try
            {

                // 予約基本で排他チェック
                var version = _context.ReserveBasicInfo
                        .Where(x => x.CompanyNo == cond.CompanyNo
                                 && x.ReserveNo == cond.ReserveNo
                                 && x.Version == cond.Version)
                        .Select(s => s.Version)
                        .SingleOrDefault();
                if (version != cond.Version) return (int)CommonEnum.DBUpdateResult.VersionError;

                // 売上明細 / 入金明細
                List<TrnSalesDetailsInfo> salesDetailsList;
                List<TrnDepositDetailsInfo> depositDetailsList;

                if (cond.BillSeparateSeq == 0)
                {
                    // 売上明細
                    salesDetailsList = _context.SalesDetailsInfo
                                            .Where(x => x.CompanyNo == cond.CompanyNo
                                                      && x.ReserveNo == cond.ReserveNo
                                                      && x.AdjustmentFlag != cond.AdjustmentFlag)
                                            .ToList();

                    // 入金明細
                    depositDetailsList = _context.DepositDetailsInfo
                                            .Where(x => x.CompanyNo == cond.CompanyNo
                                                      && x.ReserveNo == cond.ReserveNo
                                                      && x.AdjustmentFlag != cond.AdjustmentFlag)
                                            .ToList();

                }
                else
                {
                    // 売上明細
                    salesDetailsList = _context.SalesDetailsInfo
                                            .Where(x => x.CompanyNo == cond.CompanyNo
                                                        && x.ReserveNo == cond.ReserveNo
                                                        && x.AdjustmentFlag != cond.AdjustmentFlag
                                                        && x.BillSeparateSeq == cond.BillSeparateSeq)
                                            .ToList();

                    // 入金明細
                    depositDetailsList = _context.DepositDetailsInfo
                                            .Where(x => x.CompanyNo == cond.CompanyNo
                                                        && x.ReserveNo == cond.ReserveNo
                                                        && x.AdjustmentFlag != cond.AdjustmentFlag
                                                        && x.BillSeparateSeq == cond.BillSeparateSeq)
                                            .ToList();

                }

                if (salesDetailsList == null || salesDetailsList.Count == 0) return (int)CommonEnum.DBUpdateResult.VersionError;
                if (depositDetailsList == null || depositDetailsList.Count == 0) return (int)CommonEnum.DBUpdateResult.VersionError;

                var udt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

                // 売上明細
                foreach (var info in salesDetailsList)
                {
                    info.BillNo = string.Empty;
                    info.AdjustmentFlag = cond.AdjustmentFlag;
                    info.Updator = cond.Updator;
                    info.Udt = udt;
                    info.Version++;
                    _context.SalesDetailsInfo.Update(info);
                }

                // 入金明細
                foreach (var info in depositDetailsList)
                {
                    info.BillNo = string.Empty;
                    info.AdjustmentFlag = cond.AdjustmentFlag;
                    info.Updator = cond.Updator;
                    info.Udt = udt;
                    info.Version++;
                    _context.DepositDetailsInfo.Update(info);
                }
                _context.SaveChanges();

                var result = (int)CommonEnum.DBUpdateResult.Error;
                var updateFlag = false;

                if (cond.AdjustmentFlag == CommonConst.ADJUSTMENTED)
                {
                    // すべての精算が終わったら予約基本の精算フラグも更新
                    var allCount = _context.SalesDetailsInfo
                    .Where(x => x.CompanyNo == cond.CompanyNo
                              && x.ReserveNo == cond.ReserveNo)
                    .Count();
                    var count = _context.SalesDetailsInfo
                                        .Where(x => x.CompanyNo == cond.CompanyNo
                                                  && x.ReserveNo == cond.ReserveNo
                                                  && x.AdjustmentFlag == cond.AdjustmentFlag)
                                        .Count();

                    if (count == allCount)
                    {
                        result = await UpdatetReserveInfo_AdjustmentFlag(cond);
                        updateFlag = true;
                    }

                }
                else if (cond.AdjustmentFlag != CommonConst.ADJUSTMENTED)
                {

                    // 精算取消したら予約基本の精算フラグも更新
                    var adjustmantFlag = _context.ReserveBasicInfo
                                            .Where(x => x.CompanyNo == cond.CompanyNo
                                                        && x.ReserveNo == cond.ReserveNo)
                                            .Select(y => y.AdjustmentFlag)
                                            .SingleOrDefault();

                    if (adjustmantFlag == CommonConst.ADJUSTMENTED)
                    {
                        result = await UpdatetReserveInfo_AdjustmentFlag(cond);
                        updateFlag = true;
                    }
                }

                if (!updateFlag)
                {
                    // 排他チェックのため予約基本のVersion等を更新
                    result = await UpdatetReserveInfo_Exclusive(cond);
                }

                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// 予約基本の精算フラグを更新
        /// </summary>
        /// <param name="cond"></param>
        /// <returns>DBUpdateResult Success = 0, VersionError = -1, Error = -99 </returns>
        private async Task<int> UpdatetReserveInfo_AdjustmentFlag(SalesDetailsInfo cond)
        {
            try
            {

                // 予約基本
                var basic = _context.ReserveBasicInfo
                                .Single(x => x.CompanyNo == cond.CompanyNo
                                          && x.ReserveNo == cond.ReserveNo
                                          && x.Version == cond.Version);
                if (basic == null) return (int)CommonEnum.DBUpdateResult.VersionError;
                basic.AdjustmentFlag = cond.AdjustmentFlag;
                basic.Updator = cond.Updator;
                basic.Udt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                basic.Version++;
                _context.ReserveBasicInfo.Update(basic);
                _context.SaveChanges();

                return (int)CommonEnum.DBUpdateResult.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// 排他チェックのため予約基本のVersion等を更新
        /// </summary>
        /// <param name="cond"></param>
        /// <returns>DBUpdateResult Success = 0, VersionError = -1, Error = -99 </returns>
        private async Task<int> UpdatetReserveInfo_Exclusive(SalesDetailsInfo cond)
        {
            try
            {

                // 予約基本
                var basic = _context.ReserveBasicInfo
                                .Single(x => x.CompanyNo == cond.CompanyNo
                                          && x.ReserveNo == cond.ReserveNo
                                          && x.Version == cond.Version);
                if (basic == null) return (int)CommonEnum.DBUpdateResult.VersionError;

                basic.Updator = cond.Updator;
                basic.Udt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                basic.Version++;
                _context.ReserveBasicInfo.Update(basic);
                _context.SaveChanges();

                return (int)CommonEnum.DBUpdateResult.Success;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }
        }

    }
}
