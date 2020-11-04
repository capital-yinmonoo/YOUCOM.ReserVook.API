using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.Commons.Extensions;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;
using static YOUCOM.ReserVook.API.Context.CommonEnum;

namespace YOUCOM.ReserVook.API.Services
{
    public class RoomsService : IRoomsService
    {
        private DBContext _context;


        private enum RoomStatusValue {
            Cleaning,
            Cleaned,
            CheckIn,
            Stay,
            CheckOut,
            CheckOuted,
            ChengeFrom,
            ChengeTo,
            StayCleaning,
            StayCleaned
        }

        public RoomsService(DBContext context)
        {
            _context = context;
        }

        #region 部屋マスタ関連
        public async Task<int> AddRoomInfo(MstRoomsInfo roomInfo)
        {
            //エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;
            //会社番号と部屋番号の一致するデータを取得
            var info = _context.RoomsInfo.Where(w => w.CompanyNo == roomInfo.CompanyNo && w.RoomNo == roomInfo.RoomNo).AsNoTracking().SingleOrDefault();

            if (info == null) {
                //データが存在しなかった場合 → 追加
                roomInfo.Version = 0;
                roomInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                roomInfo.Udt = roomInfo.Cdt;
                roomInfo.Status = Context.CommonConst.STATUS_USED;

                _context.RoomsInfo.Add(roomInfo);
                _context.SaveChanges();
            } else if (info.Status == CommonConst.STATUS_UNUSED) {
                //データが存在し,Statusが「9」の場合 → 更新
                bool addFlag = true;
                roomInfo.Version = info.Version;
                roomInfo.Creator = info.Creator;
                roomInfo.Cdt = info.Cdt;
                roomInfo.Status = Context.CommonConst.STATUS_USED;
                var updateInfo = await UpdateRoomInfo(roomInfo, addFlag);
            } else {
                //データが存在し,Statusが「1」の場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        public async Task<int> UpdateRoomInfo(MstRoomsInfo roomInfo, bool addFlag)
        {
            try {
                // versionチェック
                if (!addFlag) {
                    if (!await RoomCheckVer(roomInfo)) { return -1; }
                }

                roomInfo.Version++;
                roomInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                _context.RoomsInfo.Update(roomInfo);
                return _context.SaveChanges();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        public async Task<int> DelRoomInfo(MstRoomsInfo roomInfo)
        {
            try {
                // versionチェック
                if (!await RoomCheckVer(roomInfo)) { return -1; }

                roomInfo.Version++;
                roomInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                roomInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.RoomsInfo.Update(roomInfo);
                return _context.SaveChanges();
            } catch (Exception e) {
                throw e;
            }
        }

        public async Task<RoomInfoView> GetRoomInfoById(MstRoomsInfo roomInfo)
        {
            var list = new RoomInfoView();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT room.*";
            sql += " FROM mst_rooms room";
            sql += " WHERE room.company_no = '" + roomInfo.CompanyNo + "'";
            sql += " AND room.room_no = '" + roomInfo.RoomNo + "'";
            sql += " AND room.status <> '" + CommonConst.STATUS_UNUSED + "'";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        list.CompanyNo = reader["company_no"].ToString();
                        list.RoomNo = reader["room_no"].ToString();
                        list.RoomName = reader["room_name"].ToString();
                        list.RoomTypeCode = reader["room_type_code"].ToString();
                        list.Floor = reader["floor"].ToString();
                        list.Remarks = reader["remarks"].ToString();
                        list.SmokingDivision = reader["smoking_division"].ToString();
                        list.RowIndex = reader["row_index"].ToString() == "" ? (int?)null : reader["row_index"].ToString().ToInt();
                        list.ColumnIndex = reader["column_index"].ToString() == "" ? (int?)null : reader["column_index"].ToString().ToInt();
                        list.Status = reader["status"].ToString();
                        list.Version = int.Parse(reader["version"].ToString());
                        list.Creator = reader["creator"].ToString();
                        list.Updator = reader["updator"].ToString();
                        list.Cdt = reader["cdt"].ToString();
                        list.Udt = reader["udt"].ToString();
                    }
                }
            } catch (Exception e) {

                Console.WriteLine(e.ToString());
            } finally {
                _context.Database.CloseConnection();
            }

            return list;
        }

        public async Task<List<RoomInfoView>> GetList(MstRoomsInfo roomInfo)
        {
            var lists = new List<RoomInfoView>();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT room.*, roomtype.code_name room_type_name, floor.code_name floor_name, smorking.code_name smorking_name";
            sql += " FROM mst_rooms room";

            sql += " LEFT JOIN mst_code_name roomtype";
            sql += " ON room.company_no =  roomtype.company_no";
            sql += " AND roomtype.division_code ='" + ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += " AND room.room_type_code =  roomtype.code";

            sql += " LEFT JOIN mst_code_name floor";
            sql += " ON room.company_no =  floor.company_no";
            sql += " AND floor.division_code = '" + ((int)CommonEnum.CodeDivision.Floor).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += " AND room.floor =  floor.code";

            sql += " LEFT JOIN mst_code_name smorking";
            sql += " ON room.company_no =  smorking.company_no";
            sql += " AND smorking.division_code = '" + ((int)CommonEnum.CodeDivision.IsForbid).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += " AND room.smoking_division =  smorking.code";

            sql += " WHERE room.company_no = '" + roomInfo.CompanyNo + "'";
            sql += " AND room.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY room.floor ASC ,room.room_no ASC";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var list = new RoomInfoView();
                        var sss = reader["row_index"].ToString();
                        list.CompanyNo = reader["company_no"].ToString();
                        list.RoomNo = reader["room_no"].ToString();
                        list.RoomName = reader["room_name"].ToString();
                        list.RoomTypeCode = reader["room_type_code"].ToString();
                        list.Floor = reader["floor"].ToString();
                        list.Remarks = reader["remarks"].ToString();
                        list.SmokingDivision = reader["smoking_division"].ToString();
                        list.RowIndex = reader["row_index"].ToString() == "" ? (int?)null : reader["row_index"].ToString().ToInt();
                        list.ColumnIndex = reader["column_index"].ToString() == "" ? (int?)null : reader["column_index"].ToString().ToInt();
                        list.Status = reader["status"].ToString();
                        list.Version = int.Parse(reader["version"].ToString());
                        list.Creator = reader["creator"].ToString();
                        list.Updator = reader["updator"].ToString();
                        list.Cdt = reader["cdt"].ToString();
                        list.Udt = reader["udt"].ToString();
                        list.RoomTypeName = reader["room_type_name"].ToString();
                        list.FloorName = reader["floor_name"].ToString();
                        list.SmokingDivisionName = reader["smorking_name"].ToString();

                        lists.Add(list);
                    }
                }
            } catch (Exception e) {

                Console.WriteLine(e.ToString());
            } finally {
                _context.Database.CloseConnection();
            }

            return lists;
        }

        public async Task<int> DeleteRoomCheckAssign(MstRoomsInfo roomInfo)
        {
            int count = 0;
            int countAssign = _context.ReserveAssignInfo.Count(w => w.CompanyNo == roomInfo.CompanyNo && w.RoomNo == roomInfo.RoomNo && w.RoomStateClass == CommonConst.ROOMSTATUS_ASSIGN);
            int countStay = _context.ReserveAssignInfo.Count(w => w.CompanyNo == roomInfo.CompanyNo && w.RoomNo == roomInfo.RoomNo && (w.RoomStateClass == CommonConst.ROOMSTATUS_STAY || w.RoomStateClass == CommonConst.ROOMSTATUS_STAYCLEANING) || w.RoomStateClass == CommonConst.ROOMSTATUS_STAYCLEANED);

            if (countAssign > 0 || countStay > 0) {
                count = countAssign + countStay;
                return count;
            } else {
                return count;
            }

        }

        public async Task<int> DeleteRoomCheckCleaned(MstRoomsInfo roomInfo)
        {
            int count = 0;
            int countCO = _context.ReserveAssignInfo.Count(w => w.CompanyNo == roomInfo.CompanyNo && w.RoomNo == roomInfo.RoomNo && (w.RoomStateClass == CommonConst.ROOMSTATUS_CO || w.RoomStateClass == CommonConst.ROOMSTATUS_CLEANING));
            int countCleaned = _context.ReserveAssignInfo.Count(w => w.CompanyNo == roomInfo.CompanyNo && w.RoomNo == roomInfo.RoomNo && w.RoomStateClass == CommonConst.ROOMSTATUS_CLEANED);

            if (countCO > 0 || countCleaned > 0) {
                count = countCO + countCleaned;
                return count;
            } else {
                return count;
            }
        }

        // バージョンチェック
        private async Task<bool> RoomCheckVer(MstRoomsInfo roomInfo)
        {
            try {
                // キーセット
                MstRoomsInfo keyInfo = new MstRoomsInfo() { CompanyNo = roomInfo.CompanyNo, RoomNo = roomInfo.RoomNo };

                // データ取得
                var info = await GetRoomInfoById(keyInfo);

                // バージョン差異チェック
                if (roomInfo.Version != info.Version) {
                    return false;
                }

                return true;
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        // 部屋取得(他ページ用)
        public async Task<List<MstRoomsInfo>> GetRoomList(string companyNo)
        {
            return _context.RoomsInfo
                    .Where(d => d.CompanyNo == companyNo && d.Status == CommonConst.STATUS_USED)
                    .OrderBy(d => d.RoomNo).ToList();

        }

        #endregion

        #region 部屋表示設定関連
        /// <summary>
        /// 部屋の画面位置更新
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> UpdateRoomDispLocation(List<MstRoomsInfo> list)
        {
            var ret = CommonEnum.DBUpdateResult.Error;

            try {
                using (var tran = _context.Database.BeginTransaction()) {
                    try {

                        foreach (var info in list) {

                            // versionチェック
                            var currentVer = _context.RoomsInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                        w.RoomNo == info.RoomNo)
                                                               .AsNoTracking()
                                                               .Select(s => s.Version)
                                                               .SingleOrDefault();

                            if (currentVer != info.Version) {
                                ret = CommonEnum.DBUpdateResult.VersionError;
                                throw new Exception("Version Error");
                            }

                            info.Version++;
                            info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

                            _context.RoomsInfo.Update(info);
                            if (_context.SaveChanges() != 1) {
                                ret = CommonEnum.DBUpdateResult.Error;
                                throw new Exception("Error");
                            }
                        }

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }

                }

                ret = CommonEnum.DBUpdateResult.Success;
                return ret;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                ret = CommonEnum.DBUpdateResult.Error;
                return ret;
            }
        }


        #endregion

        #region アサイン関連

        /// <summary>
        /// 指定日のアサイン情報を取得
        /// </summary>
        /// <param name="cond">検索条件</param>
        /// <returns></returns>
        public async Task<List<List<RoomsAssignedInfo>>> GetDailyAssign(RoomsAssignCondition cond)
        {
            try {

                List<RoomsAssignedInfo> assignDataList;

                // 指定日のアサイン情報を取得
                assignDataList = GetDailyAssignData(cond);

                // 部屋マスタ取得
                List<MstRoomsInfo> mstRoomsInfos = _context.RoomsInfo.Where(r => r.CompanyNo == cond.CompanyNo &&
                                                                                 r.Status != CommonConst.STATUS_UNUSED).ToList();
                // 部屋マスタの最大列インデックスを取得
                var maxColumns = mstRoomsInfos.Max(x => x.ColumnIndex);
                // 部屋マスタの最大行インデックスを取得
                var maxRows = mstRoomsInfos.Max(x => x.RowIndex);

                // 表示用アサインテーブルデータ作成
                var rooms = new List<List<RoomsAssignedInfo>>();
                for (var y = 0; y <= maxRows; y++) {
                    var row = new List<RoomsAssignedInfo>();
                    for (var x = 0; x <= maxColumns; x++) {
                        var info = new RoomsAssignedInfo() { RowIndex = y, ColumnIndex = x };

                        // 部屋が存在する場合、部屋情報をセット
                        var idx = assignDataList.FindIndex(f => f.RowIndex == y && f.ColumnIndex == x);
                        if (idx > -1) {
                            assignDataList[idx].RoomStateClass = assignDataList[idx].RoomStateClass.NullToEmpty();

                            var hollowInfo = assignDataList.Where(x => x.RoomNo == assignDataList[idx].RoomNo && x.ReserveNo != assignDataList[idx].ReserveNo && x.HollowStateClass == CommonConst.HOLLOWSTATUS_HOLLOW).FirstOrDefault();
                            if (hollowInfo != null) {
                                assignDataList[idx].HideReserveNo = hollowInfo.ReserveNo;
                                assignDataList[idx].HideHollowStateClass = hollowInfo.HollowStateClass;
                            }

                            info = assignDataList[idx];

                        }

                        row.Add(info);
                    }

                    rooms.Add(row);
                }

                return rooms;

            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        private List<RoomsAssignedInfo> GetDailyAssignData(RoomsAssignCondition cond)
        {
            string wkDate = cond.UseDate;
            // CO予定のみ表示
            if (cond.ViewCOFlg) {
                // 指定日前日のアサイン情報を取得
                wkDate = System.DateTime.ParseExact(cond.UseDate, CommonConst.DATE_FORMAT, null).AddDays(-1).ToString(CommonConst.DATE_FORMAT);
            }

            #region Create Query
            var sql = "SELECT";
            sql += "  tra.reserve_no";
            sql += "  , tra.use_date";
            sql += "  , mr.room_no";
            sql += "  , mr.room_type_code";
            sql += "  , tra.route_seq";
            sql += "  , tra.org_roomtype_code";
            sql += "  , tra.roomtype_seq";
            sql += "  , tra.room_state_class";
            sql += "  , tra.guest_name";
            sql += "  , tra.member_male";
            sql += "  , tra.member_female";
            sql += "  , tra.member_child_a";
            sql += "  , tra.member_child_b";
            sql += "  , tra.member_child_c";
            sql += "  , tra.company_no";
            sql += "  , tra.status";
            sql += "  , tra.version";
            sql += "  , tra.creator";
            sql += "  , tra.updator";
            sql += "  , tra.cdt";
            sql += "  , tra.udt";
            sql += "  , tra.cleaning_instruction";
            sql += "  , tra.cleaning_remarks";
            sql += "  , tra.email";
            sql += "  , tra.hollow_state_class";
            sql += "  , mr.row_index";
            sql += "  , mr.column_index";
            sql += "  , mr.room_name";
            sql += "  , roomtype.code_name AS room_type_name";
            sql += "  , roomtype.code_value AS room_type_division";
            sql += "  , mr.smoking_division AS smoking_division";
            sql += "  , trb.arrival_date";
            sql += "  , trb.departure_date";
            sql += "  , trb.stay_days";
            sql += " FROM ";
            sql += "   mst_rooms mr  ";
            sql += "   LEFT JOIN (  ";
            sql += "     SELECT ";
            sql += "       *  ";
            sql += "     FROM ";
            sql += "       trn_reserve_assign  ";
            sql += "     WHERE ";
            sql += "       company_no = '" + cond.CompanyNo + "'";
            sql += "       AND use_date = '" + wkDate + "'";
            sql += "   ) tra  ";
            sql += "     ON mr.company_no = tra.company_no  ";
            sql += "     AND mr.room_no = tra.room_no  ";
            sql += "   LEFT JOIN trn_reserve_basic trb  ";
            sql += "     ON tra.company_no = trb.company_no  ";
            sql += "     AND tra.reserve_no = trb.reserve_no  ";
            sql += "   LEFT JOIN (  ";
            sql += "     SELECT ";
            sql += "       company_no ";
            sql += "       , code ";
            sql += "       , code_name  ";
            sql += "       , code_value  ";
            sql += "     FROM ";
            sql += "       mst_code_name  ";
            sql += "     WHERE ";
            sql += "       division_code = '" + ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += "       AND status != '" + CommonConst.STATUS_UNUSED + "'";
            sql += "   ) roomtype  ";
            sql += "     ON roomtype.company_no = mr.company_no  ";
            sql += "     AND roomtype.code = mr.room_type_code  ";
            sql += " WHERE ";
            sql += "   mr.company_no = '" + cond.CompanyNo + "'";
            sql += "   AND mr.status != '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY ";
            sql += "   row_index ";
            sql += "   , column_index ";
//            sql += "   , room_state_class ";
            sql += "   , COALESCE(hollow_state_class,'')";
            #endregion

            var list = new List<RoomsAssignedInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand()) {

                command.CommandText = sql;
                _context.Database.OpenConnection();

                try {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var info = new RoomsAssignedInfo();

                            // Base
                            info.CompanyNo = reader["company_no"].ToString();
                            info.Status = reader["status"].ToString();
                            info.Version = reader["version"].ToString().ToInt_Or_Zero();
                            info.Creator = reader["creator"].ToString();
                            info.Updator = reader["updator"].ToString();
                            info.Cdt = reader["cdt"].ToString();
                            info.Udt = reader["udt"].ToString();

                            // 部屋マスタの基本情報
                            info.RoomNo = reader["room_no"].ToString();
                            info.RowIndex = reader["row_index"].ToString() == "" ? (int?)null : reader["row_index"].ToString().ToInt();
                            info.ColumnIndex = reader["column_index"].ToString() == "" ? (int?)null : reader["column_index"].ToString().ToInt();
                            info.RoomName = reader["room_name"].ToString();
                            info.RoomtypeCode = reader["room_type_code"].ToString();
                            info.RoomTypeDivision = reader["room_type_division"].ToString();
                            info.RoomTypeName = reader["room_type_name"].ToString();
                            info.SmokingDivision = (CommonEnum.SmokingDivision)reader["smoking_division"].ToString().ToInt_Or_Zero();

                            // 予約,アサインデータ情報
                            // CO表示に日帰りのCOは表示させない(前日まで宿泊のCOのみを表示させる)
                            if (!cond.ViewCOFlg ||
                                (cond.ViewCOFlg &&
                                reader["reserve_no"].ToString().IsNotBlanks() &&
                                reader["stay_days"].ToString().ToInt_Or_Zero() != 0 &&
                                //reader["departure_date"].ToString() == cond.UseDate &&
                                (reader["room_state_class"].ToString() == CommonConst.ROOMSTATUS_STAY ||
                                 reader["room_state_class"].ToString() == CommonConst.ROOMSTATUS_CO ||
                                 reader["room_state_class"].ToString() == CommonConst.ROOMSTATUS_CLEANING ||
                                 reader["room_state_class"].ToString() == CommonConst.ROOMSTATUS_CLEANED ||
                                 reader["room_state_class"].ToString() == CommonConst.ROOMSTATUS_STAYCLEANING ||
                                 reader["room_state_class"].ToString() == CommonConst.ROOMSTATUS_STAYCLEANED)
                                )) {
                                info.ReserveNo = reader["reserve_no"].ToString();
                                info.UseDate = reader["use_date"].ToString();
                                info.RouteSEQ = reader["route_seq"].ToString().ToInt_Or_Zero();
                                info.OrgRoomtypeCode = reader["org_roomtype_code"].ToString();
                                info.RoomtypeSeq = reader["roomtype_seq"].ToString().ToInt_Or_Zero();

                                //if (reader["room_state_class"].ToString() == CommonConst.ROOMSTATUS_STAYCLEANING || reader["room_state_class"].ToString() == CommonConst.ROOMSTATUS_STAYCLEANED) {
                                //    info.RoomStateClass = CommonConst.ROOMSTATUS_STAY;
                                //} else {
                                //    info.RoomStateClass = reader["room_state_class"].ToString();
                                //}
                                switch (reader["room_state_class"].ToString()) {
                                    case CommonConst.ROOMSTATUS_STAYCLEANING:
                                    case CommonConst.ROOMSTATUS_STAYCLEANED:
                                        info.RoomStateClass = CommonConst.ROOMSTATUS_STAY;
                                        break;

                                    case CommonConst.ROOMSTATUS_CLEANING:
                                        info.RoomStateClass = CommonConst.ROOMSTATUS_CO;
                                        break;

                                    default:
                                        info.RoomStateClass = reader["room_state_class"].ToString();
                                        break;
                                }

                                info.GuestName = reader["guest_name"].ToString();
                                info.MemberMale = reader["member_male"].ToString().ToInt_Or_Zero();
                                info.MemberFemale = reader["member_female"].ToString().ToInt_Or_Zero();
                                info.MemberChildA = reader["member_child_a"].ToString().ToInt_Or_Zero();
                                info.MemberChildB = reader["member_child_b"].ToString().ToInt_Or_Zero();
                                info.MemberChildC = reader["member_child_c"].ToString().ToInt_Or_Zero();

                                info.ArrivalDate = reader["arrival_date"].ToString();
                                info.DepartureDate = reader["departure_date"].ToString();
                                info.StayDays = reader["stay_days"].ToString().ToInt_Or_Zero();
                                info.MemberAdult = info.MemberMale + info.MemberFemale;
                                info.MemberChild = info.MemberChildA + info.MemberChildB + info.MemberChildC;

                                info.CleaningInstruction = reader["cleaning_instruction"].ToString();
                                info.CleaningRemarks = reader["cleaning_remarks"].ToString();
                                info.Email = reader["email"].ToString();
                                info.HollowStateClass = reader["hollow_state_class"].ToString();

                            }

                            list.Add(info);
                        }
                    }

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                } finally {
                    _context.Database.CloseConnection();
                }
            }

            return SetCheckInOutDayFlag(list, cond);

        }

        /// <summary>
        /// チェックイン・アウト日を判定してフラグをセット
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cond"></param>
        /// <returns></returns>
        private List<RoomsAssignedInfo> SetCheckInOutDayFlag(List<RoomsAssignedInfo> list, RoomsAssignCondition cond)
        {
            foreach (var item in list.Where(w => w.UseDate != null && w.UseDate != ""))
            {
                if (cond.ViewCOFlg)
                {
                    // チェックアウト日の判定
                    // 1.出発日-1日にアサインデータがある部屋(出発日にCO)
                    // 2.出発日以前にアサインデータがない部屋(途中でCO)
                    if (item.DepartureDate == cond.UseDate) 
                    {
                        item.CheckOutDay = true;
                    }
                    else
                    {
                        var tomorrow = System.DateTime.ParseExact(item.UseDate, CommonConst.DATE_FORMAT, null).AddDays(1).ToString(CommonConst.DATE_FORMAT);
                        var assignCount = _context.ReserveAssignInfo.AsNoTracking().Count(w => w.CompanyNo == item.CompanyNo
                                                                                            && w.ReserveNo == item.ReserveNo
                                                                                            && w.UseDate == tomorrow
                                                                                            && w.RouteSEQ == item.RouteSEQ);
                        if (assignCount == 0) 
                        {
                            item.CheckOutDay = true; 
                        }
                        else
                        {
                            // CO以外の部屋は表示しない(アサインデータを空にしておく)
                            item.ReserveNo = string.Empty;
                            item.UseDate = string.Empty;
                            item.RouteSEQ = 0;
                            item.OrgRoomtypeCode = string.Empty;
                            item.RoomtypeSeq = 0;
                            item.RoomStateClass = string.Empty;
                            item.GuestName = string.Empty;
                            item.MemberMale = 0;
                            item.MemberFemale = 0;
                            item.MemberChildA = 0;
                            item.MemberChildB = 0;
                            item.MemberChildC = 0;

                            item.ArrivalDate = string.Empty;
                            item.DepartureDate = string.Empty;
                            item.StayDays = 0;
                            item.MemberAdult = 0;
                            item.MemberChild = 0;

                            item.CleaningInstruction = string.Empty;
                            item.CleaningRemarks = string.Empty;
                            item.Email = string.Empty;
                            item.HollowStateClass = string.Empty;
                        }
                    }
                }
                else
                {
                    // チェックイン日の判定
                    // 1.到着日にアサインデータがある部屋(到着日からCI)
                    // 2.利用日以降でアサインデータがある部屋(途中からCI)
                    if (item.ArrivalDate == item.UseDate) 
                    {
                        item.CheckInDay = true;
                    }
                    else
                    {
                        var yesterday = System.DateTime.ParseExact(item.UseDate, CommonConst.DATE_FORMAT, null).AddDays(-1).ToString(CommonConst.DATE_FORMAT);
                        var assignCount = _context.ReserveAssignInfo.AsNoTracking().Count(w => w.CompanyNo == item.CompanyNo
                                                                                            && w.ReserveNo == item.ReserveNo
                                                                                            && w.UseDate == yesterday
                                                                                            && w.RouteSEQ == item.RouteSEQ);
                        if (assignCount == 0) { item.CheckInDay = true; }
                    }
                }

            }

            return list;
        }

        /// <summary>
        /// 利用日の未アサイン情報を取得
        /// </summary>
        /// <param name="cond">検索条件</param>
        /// <returns></returns>
        public async Task<List<NotAssignInfo>> GetDailyNotAssignInfo(RoomsAssignCondition cond)
        {
            try {
                // 予約アサインから未アサインデータを取得
                #region Create Query
                var sql = "	select	";
                sql += "	  tra.*	";
                sql += "	  , trb.arrival_date	";
                sql += "	  , trb.departure_date	";
                sql += "	  , trb.stay_days	";
                sql += "	  , roomtype.code_name as room_type_name	";
                sql += "	from	";
                sql += "	  trn_reserve_assign tra 	";
                sql += "	  inner join trn_reserve_basic trb 	";
                sql += "	    on tra.company_no = trb.company_no 	";
                sql += "	    and tra.reserve_no = trb.reserve_no 	";
                sql += "	  left join ( 	";
                sql += "	    select	";
                sql += "	      company_no	";
                sql += "	      , code	";
                sql += "	      , code_name 	";
                sql += "	    from	";
                sql += "	      mst_code_name 	";
                sql += "	    where	";
                sql += "          division_code = '" + ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
                sql += "          AND status != '" + CommonConst.STATUS_UNUSED + "'";
                sql += "	  ) roomtype 	";
                sql += "	    on roomtype.company_no = tra.company_no 	";
                sql += "	    and roomtype.code = tra.org_roomtype_code 	";
                sql += "	where	";
                sql += "	  tra.company_no = '" + cond.CompanyNo + "'";
                if (cond.UseDate.IsNotBlanks()) { sql += "      AND tra.use_date = '" + cond.UseDate + "'"; }
                sql += "      AND (tra.room_no = '' or tra.room_no is null)";
                sql += "      AND (tra.room_state_class = '' or tra.room_state_class is null)";
                sql += "	ORDER BY	";
                sql += "	  tra.use_date";
                sql += "	  , tra.reserve_no";
                sql += "      , tra.org_roomtype_code";
                sql += "      , tra.route_seq";


                #endregion

                var list = new List<NotAssignInfo>();
                using (var command = _context.Database.GetDbConnection().CreateCommand()) {

                    command.CommandText = sql;
                    _context.Database.OpenConnection();

                    try {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                var info = new NotAssignInfo();

                                info.ReserveNo = reader["reserve_no"].ToString();
                                info.UseDate = reader["use_date"].ToString();
                                info.RoomNo = reader["room_no"].ToString();
                                info.RoomtypeCode = reader["roomtype_code"].ToString();
                                info.OrgRoomtypeCode = reader["org_roomtype_code"].ToString();
                                info.RoomtypeSeq = reader["roomtype_seq"].ToString().ToInt_Or_Zero();
                                info.RouteSEQ = reader["route_seq"].ToString().ToInt_Or_Zero();
                                info.RoomStateClass = reader["room_state_class"].ToString();
                                info.GuestName = reader["guest_name"].ToString();
                                info.MemberMale = reader["member_male"].ToString().ToInt_Or_Zero();
                                info.MemberFemale = reader["member_female"].ToString().ToInt_Or_Zero();
                                info.MemberChildA = reader["member_child_a"].ToString().ToInt_Or_Zero();
                                info.MemberChildB = reader["member_child_b"].ToString().ToInt_Or_Zero();
                                info.MemberChildC = reader["member_child_c"].ToString().ToInt_Or_Zero();
                                info.CompanyNo = reader["company_no"].ToString();
                                info.Status = reader["status"].ToString();
                                info.Version = reader["version"].ToString().ToInt_Or_Zero();
                                info.Creator = reader["creator"].ToString();
                                info.Updator = reader["updator"].ToString();
                                info.Cdt = reader["cdt"].ToString();
                                info.Udt = reader["udt"].ToString();

                                info.MemberAdult = reader["member_male"].ToString().ToInt_Or_Zero() + reader["member_female"].ToString().ToInt_Or_Zero();
                                info.MemberChild = reader["member_child_a"].ToString().ToInt_Or_Zero() + reader["member_child_b"].ToString().ToInt_Or_Zero() + reader["member_child_c"].ToString().ToInt_Or_Zero();
                                info.ArrivalDate = reader["arrival_date"].ToString();
                                info.DepartureDate = reader["departure_date"].ToString();
                                info.StayDays = reader["stay_days"].ToString().ToInt_Or_Zero();
                                info.RoomTypeName = reader["room_type_name"].ToString();
                                info.CleaningInstruction = reader["cleaning_instruction"].ToString();
                                info.CleaningRemarks = reader["cleaning_remarks"].ToString();
                                info.HollowStateClass = reader["hollow_state_class"].ToString();

                                info.Email = reader["email"].ToString();


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
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// アサイン
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<int> AssignRoom(List<TrnReserveAssignInfo> list)
        {
            int ret = 0;

            try {
                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        string wkRoomNo = string.Empty;

                        foreach (var info in list.OrderBy(o => o.UseDate)) {
                            if (wkRoomNo.IsNotBlanks()) { info.RoomNo = wkRoomNo; }

                            // アサインチェック
                            var wkInfo = await CheckRoomStateAndSearchAssignableRoom(info);
                            if (wkInfo.RoomNo.IsBlanks()) {
                                // 同一部屋タイプが見つからない為、未アサイン
                                ret = 1;
                                continue;
                            }
                            if (info.RoomNo != wkInfo.RoomNo) {
                                // 同一部屋タイプの別部屋にアサイン
                                ret = 2;
                                wkRoomNo = wkInfo.RoomNo;
                            }

                            // versionチェック
                            if (!CheckVersionReserveAssignInfo(wkInfo)) {
                                ret = (int)CommonEnum.DBUpdateResult.VersionError;
                                throw new Exception("Version Error");
                            }

                            wkInfo.RoomStateClass = CommonConst.ROOMSTATUS_ASSIGN;
                            wkInfo.Version++;
                            wkInfo.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

                            _context.ReserveAssignInfo.Update(wkInfo);
                            if (_context.SaveChanges() != 1) {
                                ret = -99;
                                throw new Exception("Update Error");
                            }
                        }

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }

                }

                return ret;

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                ret = -99;
                return ret;
            }
        }

        /// <summary>
        /// アサイン解除
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> AssignCancel(TrnReserveAssignInfo cond)
        {

            try {
                // 2泊目以降のアサインをチェック
                var cancelList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                      w.ReserveNo == cond.ReserveNo &&
                                                                      w.RouteSEQ == cond.RouteSEQ &&
                                                                      w.UseDate.CompareTo(cond.UseDate) > 0 &&
                                                                      w.Status != CommonConst.STATUS_UNUSED)
                                                           .ToList();

                // versionチェック
                if (!CheckVersionReserveAssignInfo(cond)) { return CommonEnum.DBUpdateResult.VersionError; }

                cancelList.Add(cond);

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        foreach (var info in cancelList) {

                            info.RoomStateClass = CommonConst.ROOMSTATUS_NOT_ASSIGN;
                            info.HollowStateClass = CommonConst.HOLLOWSTATUS_DEFAULT;
                            info.RoomNo = string.Empty;
                            info.Version++;
                            info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            info.Updator = cond.Updator;

                            _context.ReserveAssignInfo.Update(info);
                            if (_context.SaveChanges() != 1) {
                                return CommonEnum.DBUpdateResult.Error;
                            }
                        }

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }

                }

                return CommonEnum.DBUpdateResult.Success;

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// 予約番号の指定日以降の未アサインを取得
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<List<TrnReserveAssignInfo>> GetReserveNotAssignInfo(TrnReserveAssignInfo info)
        {
            // 画面アサイン日以降で未アサインデータがあるかチェック
            // HACK:追加機能時考慮が必要
            return _context.ReserveAssignInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                            w.ReserveNo == info.ReserveNo &&
                                            w.UseDate.CompareTo(info.UseDate) > 0 &&
                                            w.RouteSEQ == info.RouteSEQ &&
                                            (w.RoomNo == null || w.RoomNo == "") &&
                                            w.Status != CommonConst.STATUS_UNUSED)
                                     .AsNoTracking()
                                     .ToList();
        }

        /// <summary>
        /// アサイン予定部屋がアサイン可能かチェック、不可能であれば同じ部屋タイプを検索
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<TrnReserveAssignInfo> CheckRoomStateAndSearchAssignableRoom(TrnReserveAssignInfo info)
        {
            var ret = info.Clone();

            // アサイン予定の部屋が他の予約でアサイン済になっていないかチェック
            var existAssign = _context.ReserveAssignInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                    w.UseDate == info.UseDate &&
                                                                    (w.HollowStateClass == null || w.HollowStateClass == CommonConst.HOLLOWSTATUS_DEFAULT) &&
                                                                    w.RoomNo == info.RoomNo &&
                                                                    w.Status != CommonConst.STATUS_UNUSED).Count();
            if (existAssign == 0) {
                ret.RoomtypeCode = _context.RoomsInfo.Where(w => w.CompanyNo == info.CompanyNo && w.Status != CommonConst.STATUS_UNUSED && w.RoomNo == info.RoomNo)
                                                        .AsNoTracking()
                                                        .Select(s => s.RoomTypeCode)
                                                        .SingleOrDefault();
                return ret;
            }

            // 同じ部屋タイプでアサインできる部屋を検索
            #region Create Query
            var sql = "SELECT";
            sql += "  mr.room_no ";
            sql += "FROM";
            sql += "  mst_rooms mr ";
            sql += "  LEFT JOIN ( ";
            sql += "    SELECT";
            sql += "      room_no ";
            sql += "    FROM";
            sql += "      trn_reserve_assign ";
            sql += "    WHERE";
            sql += "      company_no = '" + info.CompanyNo + "' ";
            sql += "      AND use_date = '" + info.UseDate + "' ";
            sql += "      AND status != '" + CommonConst.STATUS_UNUSED + "' ";
            sql += "      AND room_no != ''";
            sql += "  ) tra ";
            sql += "    ON mr.room_no = tra.room_no ";
            sql += "WHERE";
            sql += "  mr.company_no = '" + info.CompanyNo + "' ";
            sql += "  AND mr.status != '" + CommonConst.STATUS_UNUSED + "' ";
            sql += "  AND mr.room_type_code = '" + info.OrgRoomtypeCode + "' ";
            sql += "  AND (tra.room_no = '' OR tra.room_no IS NULL) ";
            sql += "ORDER BY";
            sql += "  mr.room_no";
            #endregion

            string roomNo = string.Empty;
            using (var command = _context.Database.GetDbConnection().CreateCommand()) {

                command.CommandText = sql;
                _context.Database.OpenConnection();

                try {
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            roomNo = reader["room_no"].ToString();
                        }
                    }

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                } finally {
                    _context.Database.CloseConnection();
                }
            }

            ret.RoomNo = roomNo;
            ret.RoomtypeCode = _context.RoomsInfo.Where(w => w.CompanyNo == info.CompanyNo && w.Status != CommonConst.STATUS_UNUSED && w.RoomNo == roomNo)
                                                        .AsNoTracking()
                                                        .Select(s => s.RoomTypeCode)
                                                        .SingleOrDefault();

            return ret;
        }

        #endregion

        #region チェックイン関連

        /// <summary>
        /// チェックイン
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<int> CheckIn(TrnReserveAssignInfo cond)
        {
            try
            {
                // 未アサイン日があるかチェック
                if (!await ExsitNonAssignRoom(cond)) { return -3; } /*未アサイン日あり*/

                // 同一予約の部屋を取得
                List<TrnReserveAssignInfo> checkinList = GetTargetCIRoomList(cond, false);
                if (checkinList == null || checkinList.Count() == 0) { return (int)CommonEnum.DBUpdateResult.Error; }


                // 前の利用者が清掃完了済かチェック
                var yesterday = System.DateTime.ParseExact(cond.UseDate, CommonConst.DATE_FORMAT, null).AddDays(-1).ToString(CommonConst.DATE_FORMAT);
                var beforeCOList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                         (w.RoomStateClass != CommonConst.ROOMSTATUS_CLEANED && w.RoomStateClass != CommonConst.ROOMSTATUS_NOT_ASSIGN && w.RoomStateClass != CommonConst.ROOMSTATUS_STAYCLEANED) &&
                                                                          w.UseDate == yesterday &&
                                                                          (w.RoomNo != null || w.RoomNo != string.Empty) &&
                                                                          w.Status != CommonConst.STATUS_UNUSED)
                                                             .AsNoTracking()
                                                             .Select(s => s.RoomNo)
                                                             .ToList();

                foreach (var wkRoomNo in beforeCOList)
                {
                    if (checkinList.Select(s => s.RoomNo).ToList().Contains(wkRoomNo)) { return -2; } /*前の利用者が清掃完了まで進んでいない*/
                }

                // versionチェック
                if (!CheckVersionReserveAssignInfo(cond)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        bool checkinFlag = false;
                        int rootSeq = -1;

                        foreach (var info in checkinList) {
                            // RouteSEQが変わったタイミングでチェックイン対象に強制的にセット
                            if (info.RouteSEQ != rootSeq)
                            {
                                checkinFlag = true;
                                rootSeq = info.RouteSEQ;
                            }


                            if (checkinFlag)
                            {
                                info.RoomStateClass = CommonConst.ROOMSTATUS_STAY;
                                info.Version++;
                                info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                                info.Updator = cond.Updator;

                                _context.ReserveAssignInfo.Update(info);
                                if (_context.SaveChanges() != 1) { return (int)CommonEnum.DBUpdateResult.Error; }
                            }


                            // 中抜け設定があった場合は、以降出てくる同一のルートSEQはチェックインしない
                            if (info.HollowStateClass.NullToEmpty() == CommonConst.HOLLOWSTATUS_HOLLOW) {
                                checkinFlag = false;
                            }
                        }

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }

                }

                return (int)CommonEnum.DBUpdateResult.Success;

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// チェックイン取消
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> CheckInCancel(TrnReserveAssignInfo cond)
        {
            try {
                // 同一予約の部屋を取得
                List<TrnReserveAssignInfo> checkinList = GetTargetCIRoomList(cond, true);
                if (checkinList == null || checkinList.Count() == 0) { return CommonEnum.DBUpdateResult.Error; }

                // versionチェック
                if (!CheckVersionReserveAssignInfo(cond)) { return CommonEnum.DBUpdateResult.VersionError; }

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        foreach (var info in checkinList) {
                            info.RoomStateClass = CommonConst.ROOMSTATUS_ASSIGN;
                            info.Version++;
                            info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            info.Updator = cond.Updator;

                            _context.ReserveAssignInfo.Update(info);
                            if (_context.SaveChanges() != 1) { return CommonEnum.DBUpdateResult.Error; }
                        }

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }

                }

                return CommonEnum.DBUpdateResult.Success;

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// 同一予約でCI日が同じ部屋をチェックイン対象として抽出
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="isCICancel">True: CICancel False:CI</param>
        /// <returns></returns>
        private List<TrnReserveAssignInfo> GetTargetCIRoomList(TrnReserveAssignInfo cond, bool isCICancel)
        {
            try
            {
                string roomStateClass; 
                if (isCICancel) { roomStateClass = CommonConst.ROOMSTATUS_STAY; }
                else { roomStateClass = CommonConst.ROOMSTATUS_ASSIGN; }

                // 同一予約でCI日が同じ部屋のRouteSEQ 取得
                var targetRouteSEQList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo
                                                                            && w.ReserveNo == cond.ReserveNo
                                                                            && w.RoomStateClass == roomStateClass
                                                                            && w.UseDate == cond.UseDate
                                                                            && w.Status != CommonConst.STATUS_UNUSED)
                                                                    .OrderBy(o => o.RouteSEQ)
                                                                    .AsNoTracking()
                                                                    .Select(s => s.RouteSEQ)
                                                                    .ToList();

                // 予約基本 出発日 取得
                var reserveBasic = _context.ReserveBasicInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.ReserveNo == cond.ReserveNo).AsNoTracking().SingleOrDefault();
                DateTime startDate = cond.UseDate.ToDate(CommonConst.DATE_FORMAT);
                DateTime endDate = reserveBasic.DepartureDate.ToDate(CommonConst.DATE_FORMAT);
                TimeSpan ts = endDate - startDate;

                // CI対象のアサインデータリストをセット
                var targetCIRooms = new List<TrnReserveAssignInfo>();
                foreach (var routeSEQ in targetRouteSEQList)
                {
                    for (int days = 0; days < ts.Days; days++) /*画面日付～予約基本出発日*/
                    {
                        string wkDate = startDate.AddDays(days).ToString(CommonConst.DATE_FORMAT);

                        // CI、CI取消はIN日が同じ部屋になるアサインデータが対象
                        var targetInfo = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo
                                                                            && w.ReserveNo == cond.ReserveNo
                                                                            && w.UseDate == wkDate
                                                                            && w.RouteSEQ == routeSEQ)
                                                                   .AsNoTracking()
                                                                   .SingleOrDefault();

                        if (targetInfo == null) { break; }

                        // CI取消時 前日よりCIしている部屋(CIが違う部屋)は一緒に取消しない
                        if (isCICancel && days == 0)
                        {
                            wkDate = startDate.AddDays(-1).ToString(CommonConst.DATE_FORMAT);
                            var exsitYesterday = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo
                                                                                    && w.ReserveNo == cond.ReserveNo
                                                                                    && w.UseDate == wkDate
                                                                                    && w.RouteSEQ == routeSEQ)
                                                                           .AsNoTracking()
                                                                           .SingleOrDefault();

                            if (exsitYesterday != null) { break; }
                        }

                        targetCIRooms.Add(targetInfo.Clone());
                    }
                }

                return targetCIRooms;

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 未アサイン部屋が存在するかチェック
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        private async Task<bool> ExsitNonAssignRoom(TrnReserveAssignInfo cond)
        {
            try
            {

                var assignRooms = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo
                                                                      && w.ReserveNo == cond.ReserveNo
                                                                      && w.RoomStateClass == CommonConst.ROOMSTATUS_NOT_ASSIGN
                                                                      && w.Status != CommonConst.STATUS_UNUSED)
                                                             .AsNoTracking()
                                                             .Count();

                if (assignRooms == 0) { return true; }
                else { return false; }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
        #endregion

        #region チェックアウト関連

        /// <summary>
        /// チェックアウト
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<int> CheckOut(TrnReserveAssignInfo cond)
        {
            try {
                // versionチェック
                if (!CheckVersionReserveAssignInfo(cond)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                // 予約基本取得
                var basic = _context.ReserveBasicInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                 w.ReserveNo == cond.ReserveNo &&
                                                                 w.Status != CommonConst.STATUS_UNUSED)
                                                     .SingleOrDefault();

                // 同一予約の部屋を取得
                var checkoutList = GetTargetCORoomList(cond, false);

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        foreach (var info in checkoutList) {

                            info.RoomStateClass = CommonConst.ROOMSTATUS_CO;
                            info.Version++;
                            info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            info.Updator = cond.Updator;

                            _context.ReserveAssignInfo.Update(info);
                            if (_context.SaveChanges() != 1) { return (int)CommonEnum.DBUpdateResult.Error; }
                        }

                        // 最後の部屋のC/Oかどうかを確認
                        var targetAssignList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                                w.ReserveNo == cond.ReserveNo &&
                                                                                w.Status != CommonConst.STATUS_UNUSED)
                                                                        .OrderBy(o => o.UseDate)
                                                                        .ThenBy(o => o.RouteSEQ)
                                                                        .AsNoTracking()
                                                                        .ToList();

                        // 客室状態が「CO」「Cleaning」「Cleaned」以外のものがある(＝滞在中の部屋がある)場合、利用実績は作成しない
                        if (targetAssignList.Any(x => x.RoomStateClass != CommonConst.ROOMSTATUS_CO &&
                                                      x.RoomStateClass != CommonConst.ROOMSTATUS_CLEANED &&
                                                      x.RoomStateClass != CommonConst.ROOMSTATUS_CLEANING)) {
                            // 処理なし

                        } else {
                            // 上記C/O処理で全部屋がC/Oになった(最後のチェックイン)

                            // 未精算チェック
                            var exsitSalesCnt = _context.SalesDetailsInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                                   w.ReserveNo == cond.ReserveNo &&
                                                                                   w.Status != CommonConst.STATUS_UNUSED)
                                                                         .Count();

                            if (exsitSalesCnt > 0 && basic.AdjustmentFlag == CommonConst.NOT_ADJUSTMENTED) {
                                // 未精算がある場合、エラー
                                tran.Rollback();
                                return -3;
                            } 

                            // 顧客情報・利用実績を作成
                            var customerNo = string.Empty;
                            if (basic.CustomerNo.IsBlanks()) {
                                // 顧客番号未登録の場合、顧客登録

                                // 氏名ファイル情報取得
                                var name = _context.NameFileInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                            w.ReserveNo == cond.ReserveNo &&
                                                                            w.UseDate == CommonConst.USE_DATE_EMPTY &&
                                                                            w.RouteSEQ == CommonConst.DEFAULT_ROUTE_SEQ &&
                                                                            w.Status != CommonConst.STATUS_UNUSED)
                                                                .SingleOrDefault();

                                // 備考情報取得
                                var remarksList = _context.ReserveNoteInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                                 w.ReserveNo == cond.ReserveNo &&
                                                                                 w.Status != CommonConst.STATUS_UNUSED)
                                                                            .OrderBy(x => x.NoteSeq)
                                                                            .ToList();

                                MstCustomerInfo customerInfo = new MstCustomerInfo() {
                                    CompanyNo = basic.CompanyNo,
                                    CustomerName = name.GuestName,
                                    CustomerKana = name.GuestKana,
                                    ZipCode = name.ZipCode,
                                    Address = name.Address,
                                    PhoneNo = name.PhoneNo,
                                    MobilePhoneNo = name.MobilePhoneNo,
                                    Email = name.Email,
                                    CompanyName = name.CompanyName,
                                    Remarks1 = (remarksList.Count > 0) ? remarksList[0].Remarks : string.Empty,
                                    Remarks2 = (remarksList.Count > 1) ? remarksList[1].Remarks : string.Empty,
                                    Remarks3 = (remarksList.Count > 2) ? remarksList[2].Remarks : string.Empty,
                                    Remarks4 = (remarksList.Count > 3) ? remarksList[3].Remarks : string.Empty,
                                    Remarks5 = (remarksList.Count > 4) ? remarksList[4].Remarks : string.Empty,
                                    Creator = cond.Updator,
                                    Updator = cond.Updator
                                };

                                // 顧客マスタへ登録
                                var customerService = new CustomerService(_context);
                                customerNo = customerService.InsertCustomer(customerInfo);

                                // 顧客登録した場合、予約基本・氏名ファイルの顧客番号を更新
                                // 予約基本 更新
                                basic.CustomerNo = customerNo;  // 採番した顧客番号
                                basic.Updator = cond.Updator;
                                basic.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                                _context.ReserveBasicInfo.Update(basic);
                                _context.SaveChanges();

                                // 氏名ファイル 更新
                                name.CustomerNo = customerNo;   // 採番した顧客番号
                                name.Updator = cond.Updator;
                                name.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                                _context.NameFileInfo.Update(name);
                                _context.SaveChanges();

                            } else {
                                // 顧客番号登録済みの場合、顧客番号取得
                                customerNo = basic.CustomerNo;
                            }

                            string curDateTime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                            // 売上明細情報(利用金額合計)取得
                            decimal useAmountTotal = _context.SalesDetailsInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                                          w.ReserveNo == cond.ReserveNo &&
                                                                                          w.SetItemDivision == ((int)CommonEnum.SetItemDivision.NormalItem).ToString() &&
                                                                                          w.Status != CommonConst.STATUS_UNUSED)
                                                                 .Sum(x => x.AmountPrice);

                            TrnUseResultsInfo userResultsInfo = new TrnUseResultsInfo() {
                                CompanyNo = cond.CompanyNo,
                                CustomerNo = customerNo,                // 採番した顧客番号
                                ReserveNo = cond.ReserveNo,
                                ArrivalDate = basic.ArrivalDate,
                                DepartureDate  = basic.DepartureDate,
                                UseAmount = useAmountTotal,
                                Status = Context.CommonConst.STATUS_USED,
                                Version = 0,
                                Creator = cond.Updator,
                                Cdt = curDateTime,
                                Updator = cond.Updator,
                                Udt = curDateTime
                            };

                            _context.UseResultsInfo.Add(userResultsInfo);
                            _context.SaveChanges();
                        }

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }

                }

                return (int)CommonEnum.DBUpdateResult.Success;

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// チェックアウト取消
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<int> CheckOutCancel(TrnReserveAssignInfo cond)
        {
            try {
                // versionチェック
                if (!CheckVersionReserveAssignInfo(cond)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                // 同一予約の部屋を取得
                var checkoutList = GetTargetCORoomList(cond, true);

                // 予約基本取得
                var basic = _context.ReserveBasicInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                 w.ReserveNo == cond.ReserveNo &&
                                                                 w.Status != CommonConst.STATUS_UNUSED)
                                                     .SingleOrDefault();

                // 部屋マスタ取得
                var mstRooms = _context.RoomsInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.Status != CommonConst.STATUS_UNUSED)
                                                    .AsNoTracking()
                                                    .Select(s => s.RoomNo)
                                                    .ToList();


                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        foreach (var info in checkoutList) {
                            // 部屋マスタで削除されていないかチェック
                            if (!mstRooms.Contains(info.RoomNo)) { return -3; } /*削除されているのでCO取消不可*/

                            info.RoomStateClass = CommonConst.ROOMSTATUS_STAY;
                            info.Version++;
                            info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            info.Updator = cond.Updator;

                            _context.ReserveAssignInfo.Update(info);
                            if (_context.SaveChanges() != 1) { return (int)CommonEnum.DBUpdateResult.Error; }
                        }

                        // 会社コード・顧客番号・予約番号で利用実績を検索、データがあれば削除
                        if (basic.CustomerNo.IsNotBlanks()) {
                            var useResultsInfo = _context.UseResultsInfo.Where(x => x.CompanyNo == cond.CompanyNo &&
                                                                               x.CustomerNo == basic.CustomerNo &&
                                                                               x.ReserveNo == cond.ReserveNo &&
                                                                               x.Status != CommonConst.STATUS_UNUSED)
                                                                        .SingleOrDefault();

                            if (useResultsInfo != null) {
                                _context.UseResultsInfo.Remove(useResultsInfo);
                                int result = _context.SaveChanges();

                                switch (result) {
                                    case 1: /*single delete*/
                                        break;

                                    case (int)CommonEnum.DBUpdateResult.VersionError:
                                        tran.Rollback();
                                        return (int)CommonEnum.DBUpdateResult.VersionError;

                                    case (int)CommonEnum.DBUpdateResult.UsedError:
                                        tran.Rollback();
                                        return (int)CommonEnum.DBUpdateResult.UsedError;

                                    default:
                                        tran.Rollback();
                                        return (int)CommonEnum.DBUpdateResult.Error;
                                }
                            }
                        }

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }

                }

                return (int)CommonEnum.DBUpdateResult.Success;

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// 同一予約でCO日が同じ部屋をチェックアウト対象として抽出
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="isCOCancel">True: CICancel False:CI</param>
        /// <returns></returns>
        private List<TrnReserveAssignInfo> GetTargetCORoomList(TrnReserveAssignInfo cond, bool isCOCancel)
        {
            try
            {
                // 同一予約で利用日が同じ部屋のRouteSEQ 取得
                List<int> targetRouteSEQList;
                if (isCOCancel) 
                {
                    targetRouteSEQList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo
                                                                            && w.ReserveNo == cond.ReserveNo
                                                                            && (w.RoomStateClass == CommonConst.ROOMSTATUS_CO || w.RoomStateClass == CommonConst.ROOMSTATUS_CLEANING || w.RoomStateClass == CommonConst.ROOMSTATUS_CLEANED)
                                                                            && w.UseDate == cond.UseDate
                                                                            && w.Status != CommonConst.STATUS_UNUSED)
                                                .OrderBy(o => o.RouteSEQ)
                                                .AsNoTracking()
                                                .Select(s => s.RouteSEQ)
                                                .ToList();
                }
                else
                {
                    targetRouteSEQList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo
                                                        && w.ReserveNo == cond.ReserveNo
                                                        && (w.RoomStateClass == CommonConst.ROOMSTATUS_STAY)
                                                        && w.UseDate == cond.UseDate
                                                        && w.Status != CommonConst.STATUS_UNUSED)
                            .OrderBy(o => o.RouteSEQ)
                            .AsNoTracking()
                            .Select(s => s.RouteSEQ)
                            .ToList();
                }
               
                // CO対象のアサインデータをセット
                var targetCORooms = new List<TrnReserveAssignInfo>();
                var tomorrow = System.DateTime.ParseExact(cond.UseDate, CommonConst.DATE_FORMAT, null).AddDays(1).ToString(CommonConst.DATE_FORMAT);
                foreach (var routeSEQ in targetRouteSEQList)
                {
                    // CO、CO取消は翌日にアサインデータがない部屋が対象
                    var targetInfo = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo
                                                                        && w.ReserveNo == cond.ReserveNo
                                                                        && w.UseDate == tomorrow
                                                                        && w.RouteSEQ == routeSEQ)
                                                               .AsNoTracking()
                                                               .SingleOrDefault();
                    if (targetInfo == null)
                    {
                        var wklist = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo
                                                                        && w.ReserveNo == cond.ReserveNo
                                                                        && w.RouteSEQ == routeSEQ)
                                                               .ToList();

                        // CO取消時 本日アサインデータがない=画面指定日より前にCO済の部屋は一緒に取消しない
                        if (isCOCancel)
                        {
                            var exsitToday = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo
                                                                                    && w.ReserveNo == cond.ReserveNo
                                                                                    && w.UseDate == cond.UseDate
                                                                                    && w.RouteSEQ == routeSEQ)
                                                                           .AsNoTracking()
                                                                           .SingleOrDefault();

                            if (exsitToday == null) { continue; }
                        }

                        targetCORooms.AddRange(wklist);
                    }
                    else
                    {
                        continue;
                    }

                }

                return targetCORooms;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        #endregion

        /// <summary>
        /// 中抜け
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<int> Hollow(TrnReserveAssignInfo cond)
        {

            try
            {
                // versionチェック
                if (!CheckVersionReserveAssignInfo(cond)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                // 更新対象のデータを取得
                var hollowList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                          w.ReserveNo == cond.ReserveNo &&
                                                                          w.UseDate.CompareTo(cond.UseDate) >= 0 &&
                                                                          w.RouteSEQ == cond.RouteSEQ &&
                                                                          w.Status != CommonConst.STATUS_UNUSED)
                                                            .OrderBy(w => w.UseDate)
                                                            .ToList();

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var info in hollowList)
                        {
                            if (info.UseDate == cond.UseDate)
                            {
                                info.HollowStateClass = CommonConst.HOLLOWSTATUS_HOLLOW;
                                info.Version++;
                                info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                                info.Updator = cond.Updator;

                                _context.ReserveAssignInfo.Update(info);
                                if (_context.SaveChanges() != 1) { return (int)CommonEnum.DBUpdateResult.Error; }
                            } else 
                            {
                                // 中抜けの設定が現れた場合はExit
                                if (info.HollowStateClass == CommonConst.HOLLOWSTATUS_HOLLOW)
                                {
                                    break;
                                }

                                if (info.RoomStateClass == CommonConst.ROOMSTATUS_STAY)
                                {
                                    info.RoomStateClass = CommonConst.ROOMSTATUS_ASSIGN;
                                    info.Version++;
                                    info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                                    info.Updator = cond.Updator;

                                    _context.ReserveAssignInfo.Update(info);
                                    if (_context.SaveChanges() != 1) { return (int)CommonEnum.DBUpdateResult.Error; }
                                }
                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }

                }

                return (int)CommonEnum.DBUpdateResult.Success;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// 中抜け取消
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<int> HollowCancel(TrnReserveAssignInfo cond)
        {

            try
            {
                // versionチェック
                if (!CheckVersionReserveAssignInfo(cond)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                // 更新対象のデータを取得
                var hollowList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                          w.ReserveNo == cond.ReserveNo &&
                                                                          w.UseDate.CompareTo(cond.UseDate) >= 0 &&
                                                                          w.RouteSEQ == cond.RouteSEQ &&
                                                                          w.Status != CommonConst.STATUS_UNUSED)
                                                            .OrderBy(w => w.UseDate)
                                                            .ToList();

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var roomStateClass = "";

                        foreach (var info in hollowList)
                        {
                            if (info.UseDate == cond.UseDate)
                            {
                                info.HollowStateClass = CommonConst.HOLLOWSTATUS_DEFAULT;
                                info.Version++;
                                info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                                info.Updator = cond.Updator;

                                _context.ReserveAssignInfo.Update(info);
                                if (_context.SaveChanges() != 1) { return (int)CommonEnum.DBUpdateResult.Error; }

                                roomStateClass = info.RoomStateClass;

                            }
                            else
                            {
                                // 中抜けの設定が現れた場合はExit
                                if (info.HollowStateClass == CommonConst.HOLLOWSTATUS_HOLLOW)
                                {
                                    break;
                                }

                                // 中抜けキャンセル実行日の部屋状態が滞在中の場合、翌日以降も滞在中に変更
                                if (roomStateClass == CommonConst.ROOMSTATUS_STAY)
                                {
                                    info.RoomStateClass = CommonConst.ROOMSTATUS_STAY;
                                    info.Version++;
                                    info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                                    info.Updator = cond.Updator;

                                    _context.ReserveAssignInfo.Update(info);
                                    if (_context.SaveChanges() != 1) { return (int)CommonEnum.DBUpdateResult.Error; }
                                }
                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }

                }

                return (int)CommonEnum.DBUpdateResult.Success;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }

        }

        /// <summary>
        /// 中抜けチェックイン
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>

        public async Task<int> HollowCheckin(TrnReserveAssignInfo cond)
        {

            try
            {

                // 前の利用者が清掃完了済かチェック
                var yesterday = System.DateTime.ParseExact(cond.UseDate, CommonConst.DATE_FORMAT, null).AddDays(-1).ToString(CommonConst.DATE_FORMAT);
                var beforeCOList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                         (w.RoomStateClass != CommonConst.ROOMSTATUS_CLEANED && w.RoomStateClass != CommonConst.ROOMSTATUS_NOT_ASSIGN && w.RoomStateClass != CommonConst.ROOMSTATUS_STAYCLEANED) &&
                                                                          w.UseDate == yesterday &&
                                                                          (w.RoomNo != null || w.RoomNo != string.Empty) &&
                                                                          (w.HollowStateClass == null || w.HollowStateClass == CommonConst.HOLLOWSTATUS_DEFAULT) &&
                                                                          w.Status != CommonConst.STATUS_UNUSED)
                                                             .AsNoTracking()
                                                             .Select(s => s.RoomNo)
                                                             .ToList();


                // versionチェック
                if (!CheckVersionReserveAssignInfo(cond)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                // 更新対象のデータを取得
                var hollowList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                          w.ReserveNo == cond.ReserveNo &&
                                                                          w.UseDate.CompareTo(cond.UseDate) >= 0 &&
                                                                          w.RouteSEQ == cond.RouteSEQ &&
                                                                          w.Status != CommonConst.STATUS_UNUSED)
                                                            .OrderBy(w => w.UseDate)
                                                            .ToList();

                foreach (var wkRoomNo in beforeCOList) {
                    if (hollowList.Select(s => s.RoomNo).ToList().Contains(wkRoomNo)) { return -2; } /*前の利用者が清掃完了まで進んでいない*/
                }

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var info in hollowList)
                        {
                            // 中抜けの設定が現れた場合はExit
                            if (info.HollowStateClass== CommonConst.HOLLOWSTATUS_HOLLOW ) {
                                break;
                            }

                            // 滞在中で更新
                            info.RoomStateClass = CommonConst.ROOMSTATUS_STAY;
                            info.Version++;
                            info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            info.Updator = cond.Updator;

                            _context.ReserveAssignInfo.Update(info);
                            if (_context.SaveChanges() != 1) { return (int)CommonEnum.DBUpdateResult.Error; }
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }

                }

                return (int)CommonEnum.DBUpdateResult.Success;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }

        }

        /// <summary>
        /// 中抜けCI可能かどうかの判定
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<bool> HollowIsCheckin(TrnReserveAssignInfo cond) {

            // 前日が中抜けかつ本日が中抜けではない、かつ本日がアサインの場合、中抜けチェックイン可能
            
            try {
                var yesterday = System.DateTime.ParseExact(cond.UseDate, CommonConst.DATE_FORMAT, null).AddDays(-1).ToString(CommonConst.DATE_FORMAT);

                var yesterdayHollowCount = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                             w.ReserveNo == cond.ReserveNo &&
                                                                             w.UseDate == yesterday &&
                                                                             w.RouteSEQ == cond.RouteSEQ &&
                                                                             w.HollowStateClass == CommonConst.HOLLOWSTATUS_HOLLOW &&
                                                                             (w.RoomNo != null || w.RoomNo != string.Empty) &&
                                                                             w.Status != CommonConst.STATUS_UNUSED)
                                                                     .Count();

                if (yesterdayHollowCount == 0) {
                    return false;
                } else {
                    var todayHollowCount = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                  w.ReserveNo == cond.ReserveNo &&
                                                                  w.UseDate == cond.UseDate &&
                                                                  w.RouteSEQ == cond.RouteSEQ &&
                                                                  w.RoomStateClass == CommonConst.ROOMSTATUS_ASSIGN &&
                                                                  (w.HollowStateClass == null || w.HollowStateClass == CommonConst.HOLLOWSTATUS_DEFAULT) &&
                                                                  w.Status != CommonConst.STATUS_UNUSED)
                                                                .Count();

                    if (todayHollowCount == 0) {
                        return false;
                    } else {
                        return true;
                    }
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }

        }
        /// <summary>
        /// 中抜けチェックイン
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>

        public async Task<int> HollowCheckinCancel(TrnReserveAssignInfo cond) {

            try {

                // versionチェック
                if (!CheckVersionReserveAssignInfo(cond)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                // 更新対象のデータを取得
                var hollowList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                          w.ReserveNo == cond.ReserveNo &&
                                                                          w.UseDate.CompareTo(cond.UseDate) >= 0 &&
                                                                          w.RouteSEQ == cond.RouteSEQ &&
                                                                          w.Status != CommonConst.STATUS_UNUSED)
                                                            .OrderBy(w => w.UseDate)
                                                            .ToList();

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        foreach (var info in hollowList) {
                            // 中抜けの設定が現れた場合はExit
                            if (info.HollowStateClass == CommonConst.HOLLOWSTATUS_HOLLOW) {
                                break;
                            }

                            // アサインで更新
                            info.RoomStateClass = CommonConst.ROOMSTATUS_ASSIGN;
                            info.Version++;
                            info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            info.Updator = cond.Updator;

                            _context.ReserveAssignInfo.Update(info);
                            if (_context.SaveChanges() != 1) { return (int)CommonEnum.DBUpdateResult.Error; }
                        }

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }

                }

                return (int)CommonEnum.DBUpdateResult.Success;

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// 中抜けCIキャンセル可能かどうかの判定
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<bool> HollowIsCheckinCancel(TrnReserveAssignInfo cond) {

            try {
                // 前日が中抜けかつ本日が中抜けではない、かつ本日が滞在中の場合、中抜けチェックインキャンセル可能

                var yesterday = System.DateTime.ParseExact(cond.UseDate, CommonConst.DATE_FORMAT, null).AddDays(-1).ToString(CommonConst.DATE_FORMAT);

                var yesterdayHollowCount = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                             w.ReserveNo == cond.ReserveNo &&
                                                                             w.UseDate == yesterday &&
                                                                             w.RouteSEQ == cond.RouteSEQ &&
                                                                             w.HollowStateClass == CommonConst.HOLLOWSTATUS_HOLLOW &&
                                                                             (w.RoomNo != null || w.RoomNo != string.Empty) &&
                                                                             w.Status != CommonConst.STATUS_UNUSED)
                                                                     .Count();

                if (yesterdayHollowCount == 0) {
                    return false;
                } else {
                    var todayHollowCount = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                  w.ReserveNo == cond.ReserveNo &&
                                                                  w.UseDate == cond.UseDate &&
                                                                  w.RouteSEQ == cond.RouteSEQ &&
                                                                  (w.RoomStateClass == CommonConst.ROOMSTATUS_STAY || w.RoomStateClass == CommonConst.ROOMSTATUS_STAYCLEANING || w.RoomStateClass == CommonConst.ROOMSTATUS_STAYCLEANED) &&
                                                                  (w.HollowStateClass == null || w.HollowStateClass == CommonConst.HOLLOWSTATUS_DEFAULT) &&
                                                                  w.Status != CommonConst.STATUS_UNUSED)
                                                                .Count();

                    if (todayHollowCount == 0) {
                        return false;
                    } else {
                        return true;
                    }
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }

        }

        #region 清掃関連

        /// <summary>
        /// 清掃完了
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> CleaningRoom(TrnReserveAssignInfo info)
        {
            try {
                // versionチェック
                var currentVer = _context.ReserveAssignInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                       w.ReserveNo == info.ReserveNo &&
                                                                       w.UseDate == info.UseDate &&
                                                                       w.RouteSEQ == info.RouteSEQ &&
                                                                       w.Status != CommonConst.STATUS_UNUSED)
                                                           .AsNoTracking()
                                                           .Select(s => s.Version)
                                                           .SingleOrDefault();

                if (currentVer != info.Version) { return CommonEnum.DBUpdateResult.VersionError; }

                // 更新対象のアサインデータを抽出
                var updList = _context.ReserveAssignInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                    w.ReserveNo == info.ReserveNo &&
                                                                    w.RouteSEQ == info.RouteSEQ &&
                                                                    w.Status != CommonConst.STATUS_UNUSED)
                                                        .ToList();

                //トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        foreach (var item in updList) {
                            item.RoomStateClass = CommonConst.ROOMSTATUS_CLEANED;
                            item.Version++;
                            item.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            item.Updator = info.Updator;

                            _context.ReserveAssignInfo.Update(item);
                            if (_context.SaveChanges() != 1) { throw new Exception("Version Error"); }

                        }

                        tran.Commit();

                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }
                }

                return CommonEnum.DBUpdateResult.Success;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return CommonEnum.DBUpdateResult.Error;
            }


        }

        /// <summary>
        /// 清掃完了(滞在部屋)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> CleaningStayRoom(TrnReserveAssignInfo info) {
            try {
                // versionチェック
                var currentVer = _context.ReserveAssignInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                       w.ReserveNo == info.ReserveNo &&
                                                                       w.UseDate == info.UseDate &&
                                                                       w.RouteSEQ == info.RouteSEQ &&
                                                                       w.Status != CommonConst.STATUS_UNUSED)
                                                           .AsNoTracking()
                                                           .FirstOrDefault();

                if (currentVer.Version != info.Version) { return CommonEnum.DBUpdateResult.VersionError; }

                //トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {

                        if (currentVer.RoomStateClass == CommonConst.ROOMSTATUS_STAY || currentVer.RoomStateClass == CommonConst.ROOMSTATUS_STAYCLEANING || currentVer.RoomStateClass == CommonConst.ROOMSTATUS_STAYCLEANED) {
                            currentVer.RoomStateClass = CommonConst.ROOMSTATUS_STAYCLEANED;
                        } else {
                            currentVer.RoomStateClass = CommonConst.ROOMSTATUS_CLEANED;
                        }
                        currentVer.Version++;
                        currentVer.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                        currentVer.Updator = info.Updator;

                        _context.ReserveAssignInfo.Update(currentVer);
                        if (_context.SaveChanges() != 1) { throw new Exception("Version Error"); }


                        tran.Commit();

                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }
                }

                return CommonEnum.DBUpdateResult.Success;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return CommonEnum.DBUpdateResult.Error;
            }


        }

        #endregion

        #region ルームチェンジ関連

        /// <summary>
        /// ルームチェンジ
        /// </summary>
        /// <param name="baseRoom">移動元部屋情報</param>
        /// <param name="targetRoom">移動先部屋情報</param>
        /// <returns></returns>
        public async Task<RoomChangeResult> RoomChange(RoomsAssignedInfo baseRoom, RoomsAssignedInfo targetRoom)
        {
            // return値 初期化
            var ret = new RoomChangeResult();
            ret.Result = CommonEnum.DBUpdateResult.Error;

            try {
                // versionチェック
                if (!CheckVersionReserveAssignInfo(baseRoom)) { ret.Result = CommonEnum.DBUpdateResult.VersionError; return ret; }
                if (!CheckVersionReserveAssignInfo(targetRoom)) { ret.Result = CommonEnum.DBUpdateResult.VersionError; return ret; }


                //トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {

                        // 一方向チェンジ
                        if (targetRoom.RoomStateClass == CommonConst.ROOMSTATUS_NOT_ASSIGN) {

                            // 連泊
                            if (baseRoom.StayDays > 1) {

                                string strEndDate = baseRoom.DepartureDate.ToDate(CommonConst.DATE_FORMAT).AddDays(-1).ToString(CommonConst.DATE_FORMAT);

                                // 指定日付～(1)まで Delete -> Insert
                                DateTime startDate = baseRoom.UseDate.ToDate(CommonConst.DATE_FORMAT);
                                DateTime endDate = strEndDate.ToDate(CommonConst.DATE_FORMAT);
                                TimeSpan ts = endDate - startDate;

                                for (int i = 0; i <= ts.Days; i++) {
                                    string wkDate = startDate.AddDays(i).ToString(CommonConst.DATE_FORMAT);

                                    // 移動先の部屋にアサインできる最大の日付を取得…(1)
                                    var exsitAssign = _context.ReserveAssignInfo.Where(w => w.CompanyNo == baseRoom.CompanyNo &&
                                                                                    w.RoomNo == targetRoom.RoomNo &&
                                                                                    w.UseDate == wkDate &&
                                                                                    (w.RoomStateClass != null || w.RoomStateClass != CommonConst.ROOMSTATUS_NOT_ASSIGN) &&
                                                                                    w.Status != CommonConst.STATUS_UNUSED)
                                                                        .AsNoTracking()
                                                                        .ToList();

                                    if (exsitAssign != null && exsitAssign.Count() != 0) { break; }

                                    baseRoom.UseDate = wkDate;
                                    var updBaseRoom = GetAssignInfo(baseRoom);

                                    // 指定日付～(1)までの間に未アサイン日があればそこまでとする
                                    if (updBaseRoom == null) { break; }

                                    _context.ReserveAssignInfo.Remove(updBaseRoom);
                                    if (_context.SaveChanges() != 1) { throw new Exception("Version Error"); }

                                    updBaseRoom.RoomNo = targetRoom.RoomNo;
                                    updBaseRoom.RoomtypeCode = targetRoom.RoomtypeCode;
                                    updBaseRoom.Version++;
                                    updBaseRoom.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                                    updBaseRoom.Updator = targetRoom.Updator;

                                    _context.ReserveAssignInfo.Add(updBaseRoom);
                                    if (_context.SaveChanges() != 1) { throw new Exception("Version Error"); }

                                    ret.BaseRoomNo = baseRoom.RoomNo;
                                    ret.TargetRoomNo = targetRoom.RoomNo;
                                    ret.IsMutualChange = false;
                                }


                            }
                            // 0泊 or 1泊
                            else {
                                // Delete -> Insert
                                var updBaseRoom = GetAssignInfo(baseRoom);

                                _context.ReserveAssignInfo.Remove(updBaseRoom);
                                if (_context.SaveChanges() != 1) { throw new Exception("Version Error"); }

                                updBaseRoom.RoomNo = targetRoom.RoomNo;
                                updBaseRoom.RoomtypeCode = targetRoom.RoomtypeCode;
                                updBaseRoom.Version++;
                                updBaseRoom.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                                updBaseRoom.Updator = targetRoom.Updator;

                                _context.ReserveAssignInfo.Add(updBaseRoom);
                                if (_context.SaveChanges() != 1) { throw new Exception("Version Error"); }

                                ret.BaseRoomNo = baseRoom.RoomNo;
                                ret.TargetRoomNo = targetRoom.RoomNo;
                                ret.IsMutualChange = false;
                            }

                        }
                        // 相互チェンジ
                        else {
                            var updBaseRoom = GetAssignInfo(baseRoom);
                            var updTargetRoom = GetAssignInfo(targetRoom);

                            // Delete -> Insert
                            _context.ReserveAssignInfo.Remove(updBaseRoom);
                            _context.ReserveAssignInfo.Remove(updTargetRoom);
                            if (_context.SaveChanges() != 2) { throw new Exception("Version Error"); }

                            updBaseRoom.RoomNo = targetRoom.RoomNo;
                            updBaseRoom.RoomtypeCode = targetRoom.RoomtypeCode;
                            updBaseRoom.Version++;
                            updBaseRoom.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            updBaseRoom.Updator = targetRoom.Updator;

                            updTargetRoom.RoomNo = baseRoom.RoomNo;
                            updTargetRoom.RoomtypeCode = baseRoom.RoomtypeCode;
                            updTargetRoom.Version++;
                            updTargetRoom.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            updTargetRoom.Updator = targetRoom.Updator;

                            _context.ReserveAssignInfo.Add(updBaseRoom);

                            _context.ReserveAssignInfo.Add(updTargetRoom);
                            if (_context.SaveChanges() != 2) { throw new Exception("Version Error"); }

                            ret.BaseRoomNo = baseRoom.RoomNo;
                            ret.TargetRoomNo = targetRoom.RoomNo;
                            ret.IsMutualChange = true;

                        }

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }

                }

                ret.Result = CommonEnum.DBUpdateResult.Success;
                return ret;

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                { ret.Result = CommonEnum.DBUpdateResult.Error; return ret; }
            }
        }

        /// <summary>
        /// 予約アサイン情報 バージョンチェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns>True: OK False: NG</returns>
        private bool CheckVersionReserveAssignInfo(TrnReserveAssignInfo info)
        {
            var currentVer = _context.ReserveAssignInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                    w.ReserveNo == info.ReserveNo &&
                                                                    w.UseDate == info.UseDate &&
                                                                    w.RouteSEQ == info.RouteSEQ &&
                                                                    w.Status != CommonConst.STATUS_UNUSED)
                                                       .AsNoTracking()
                                                       .Select(s => s.Version)
                                                       .SingleOrDefault();

            if (currentVer != info.Version) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 予約アサイン情報 取得
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private TrnReserveAssignInfo GetAssignInfo(TrnReserveAssignInfo info)
        {
            return _context.ReserveAssignInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                        w.ReserveNo == info.ReserveNo &&
                                                        w.UseDate == info.UseDate &&
                                                        w.RouteSEQ == info.RouteSEQ &&
                                                        w.Status != CommonConst.STATUS_UNUSED)
                                             .SingleOrDefault();
        }

        #endregion

        #region 連泊状況関連
        public async Task<List<BookingsInfo>> GetBookingsList(BookingsCondition cond)
        {
            string roomType_divison = ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT);

            var useDateList = new List<string>();
            var startDate = cond.StartDate.ToDate(CommonConst.DATE_FORMAT);
            for (var i = 0; i < cond.DisplayDays; i++) {
                var usedate = startDate.AddDays(i).ToString(CommonConst.DATE_FORMAT);
                useDateList.Add(usedate);
            }

            var sql = "Select room.*, code.code_name room_type_name";
            sql += ", assign.use_date, assign.reserve_no";
            sql += "  From mst_rooms room";

            sql += "  Left Join mst_code_name code";
            sql += "    On room.company_no      = code.company_no";
            sql += "   And room.room_type_code  = code.code";
            sql += "   And code.division_code   = '" + roomType_divison + "'";

            sql += "  Left Join trn_reserve_assign assign";
            sql += "    On room.company_no = assign.company_no";
            sql += "   And room.room_no    = assign.room_no";
            sql += " And assign.use_date  >= '" + cond.StartDate + "'";
            sql += " And assign.use_date  <= '" + cond.EndDate + "'";
            sql += " And assign.room_no   != ''";
            sql += " And coalesce(assign.hollow_state_class, '') != '" + CommonConst.HOLLOWSTATUS_HOLLOW +"'";

            sql += " Where room.company_no = '" + cond.CompanyNo + "'";
            sql += "   And room.status     = '" + CommonConst.STATUS_USED + "'";
            sql += " Order by to_number(room.floor,'99999'), to_number(room.room_no,'99999')";


            var list = new List<BookingsInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand()) {

                command.CommandText = sql;
                _context.Database.OpenConnection();

                try {
                    var order = 1;
                    using (var reader = command.ExecuteReader()) {

                        var info = new BookingsInfo();
                        var wkRoomNo = "";
                        var day = 0;

                        while (reader.Read()) {

                            wkRoomNo = reader["room_no"].ToString();
                            if (info.RoomNo != wkRoomNo) {

                                if (info.RoomNo.IsNotBlanks()) list.Add(info);

                                info = new BookingsInfo();
                                info.RoomNo = wkRoomNo;
                                info.RoomName = reader["room_name"].ToString();
                                info.RoomType = reader["room_type_code"].ToString();
                                info.RoomTypeName = reader["room_type_name"].ToString();
                                info.DisplayOrder = order;
                                info.AssignList = new List<BookingsAssignInfo>();

                                // アサインがない日のデータを作成
                                for (day = 0; day < cond.DisplayDays; day++) {
                                    var assign = new BookingsAssignInfo();
                                    assign.UseDate = useDateList[day];
                                    assign.ReserveNo = string.Empty;
                                    info.AssignList.Add(assign);
                                }
                                order++;
                            }

                            var wkUseDate = reader["use_date"].ToString();
                            if (wkUseDate.IsNotBlanks()) {
                                // アサインがある日の予約番号をセット
                                foreach (var assign in info.AssignList) {
                                    if (assign.UseDate != wkUseDate) continue;
                                    assign.ReserveNo = reader["reserve_no"].ToString();
                                    break;
                                }

                            }
                        }

                        list.Add(info);
                    }

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                } finally {
                    _context.Database.CloseConnection();
                }
            }

            return list;
        }
        #endregion

        #region クリーニングリスト関連
        public async Task<List<CleaningInfo>> GetCleaningList(CleaningCondition cond)
        {
            string roomType_divison = ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT);
            string floor_divison = ((int)CommonEnum.CodeDivision.Floor).ToString(CommonConst.CODE_DIVISION_FORMAT);
            string smoking_divison = ((int)CommonEnum.CodeDivision.IsForbid).ToString(CommonConst.CODE_DIVISION_FORMAT);
            string roomTypeDivision_divison = ((int)CommonEnum.CodeDivision.RoomTypeDivision).ToString(CommonConst.CODE_DIVISION_FORMAT);
            string realRoom_Code = ((int)CommonEnum.RoomTypeDivision.Real).ToString();

            // C/O予定取得用
            string yesterday = cond.UseDate.ToDate(CommonConst.DATE_FORMAT).AddDays(-1).ToString(CommonConst.DATE_FORMAT);
            string mainReserve = ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString();

            const string Div_Today = "3";
            const string Div_CODate = "2";
            const string Div_Uncleaned = "1";
            const string Div_RC = "0";
            const string Div_Hollow = "4";

            const string RC_Before = "before";
            const string RC_After = "after";


            var sql = "Select room.room_no, room.smoking_division";
            sql += ", roomType.code_name room_type_name";
            sql += ", floor.code_name floor_name";
            sql += ", smoking.code_name smoking_name";
            sql += ", assign.reserve_no, assign.route_seq, assign.use_date, assign.room_state_class";
            sql += ", assign.member_male, assign.member_female, assign.member_child_a, assign.member_child_b, assign.member_child_c";
            sql += ", assign.cleaning_instruction, assign.cleaning_remarks, assign.arrival_date, assign.stay_days, assign.co_flag";

            sql += "  From mst_rooms room";

            // 各種名称取得
            sql += "  Inner Join mst_code_name roomType";
            sql += "    On room.company_no        = roomType.company_no";
            sql += "   And room.room_type_code    = roomType.code";
            sql += "   And roomType.division_code = '" + roomType_divison + "'";

            sql += "  Inner Join mst_code_name roomTypeDiv";
            sql += "    On roomType.company_no        = roomTypeDiv.company_no";
            sql += "   And roomType.code_value        = roomTypeDiv.code";
            sql += "   And roomTypeDiv.division_code  = '" + roomTypeDivision_divison + "'";
            sql += "   And roomTypeDiv.code  = '" + realRoom_Code + "'";

            sql += "  Left Join mst_code_name floor";
            sql += "    On room.company_no     = floor.company_no";
            sql += "   And room.floor          = floor.code";
            sql += "   And floor.division_code = '" + floor_divison + "'";

            sql += "  Left Join mst_code_name smoking";
            sql += "    On room.company_no       = smoking.company_no";
            sql += "   And room.smoking_division = smoking.code";
            sql += "   And smoking.division_code = '" + smoking_divison + "'";

            sql += "  Left Join (";

            // 当日分
            sql += "  Select assign.*, basic.arrival_date, basic.stay_days";
            sql += "    , '" + Div_Today + "' co_flag";
            sql += "    From trn_reserve_assign      assign";
            sql += "    Left Join trn_reserve_basic  basic";
            sql += "     On assign.company_no             = basic.company_no";
            sql += "    And assign.reserve_no             = basic.reserve_no";
            sql += "    And basic.reserve_state_division  = '" + mainReserve + "'";
            sql += "    And basic.arrival_date           <= '" + cond.UseDate + "'";
            sql += "    And basic.departure_date         >= '" + cond.UseDate + "'";
            sql += "  Where assign.company_no = '" + cond.CompanyNo + "'";
            sql += "    And assign.room_no != ''";
            sql += "    And assign.use_date = '" + cond.UseDate + "'";
            sql += "    And COALESCE(assign.hollow_state_class, '') != '" + CommonConst.HOLLOWSTATUS_HOLLOW + "'";

            sql += "  Union All";

            // チェックアウト予定
            sql += "  Select assignCO.*, null arrival_date, null stay_days";
            sql += "    , '" + Div_CODate + "' co_flag";
            sql += "    From trn_reserve_assign      assignCO";
            sql += "   Inner Join trn_reserve_basic  basicCO";
            sql += "     On assignCO.company_no             = basicCO.company_no";
            sql += "    And assignCO.reserve_no             = basicCO.reserve_no";
            sql += "    And basicCO.departure_date          = '" + cond.UseDate + "'";
            sql += "    And basicCO.reserve_state_division  = '" + mainReserve + "'";
            sql += "    And basicCO.stay_days              != 0";
            sql += "  Where assignCO.company_no = '" + cond.CompanyNo + "'";
            sql += "    And assignCO.room_no != ''";
            sql += "    And assignCO.use_date = '" + yesterday + "'";
            sql += "    And (assignCO.room_state_class = '" + CommonConst.ROOMSTATUS_STAY + "'";
            sql += "     Or assignCO.room_state_class = '" + CommonConst.ROOMSTATUS_STAYCLEANING + "'";
            sql += "     Or assignCO.room_state_class = '" + CommonConst.ROOMSTATUS_STAYCLEANED + "')";

            sql += "  Union All";

            // チェックアウト予定(中抜け)
            sql += "  Select assignHOLLOW.*, null arrival_date, null stay_days";
            sql += "    , '" + Div_Hollow + "' co_flag";
            sql += "    From trn_reserve_assign      assignHOLLOW";
            sql += "  Where assignHOLLOW.company_no         = '" + cond.CompanyNo + "'";
            sql += "    And assignHOLLOW.room_no           != ''";
            sql += "    And assignHOLLOW.use_date           = '" + cond.UseDate + "'";
            sql += "    And assignHOLLOW.hollow_state_class = '" + CommonConst.HOLLOWSTATUS_HOLLOW + "'";

            sql += "  Union All";

            // 未清掃
            sql += "  Select assignUC.*, null arrival_date, null stay_days";
            sql += "    , '" + Div_Uncleaned + "' co_flag";
            sql += "    From trn_reserve_assign      assignUC";
            sql += "   Where Exists(";
            sql += "     Select assignUC_Sub.company_no, assignUC_Sub.room_no, max(assignUC_Sub.use_date) use_date";
            sql += "       From trn_reserve_assign   assignUC_Sub";
            sql += "    Where assignUC_Sub.company_no = '" + cond.CompanyNo + "'";
            sql += "      And assignUC_Sub.room_no != ''";
            sql += "      And assignUC_Sub.use_date <= '" + yesterday + "'";
            sql += "      And (assignUC_Sub.room_state_class = '" + CommonConst.ROOMSTATUS_CO + "'";
            sql += "       OR  assignUC_Sub.room_state_class = '" + CommonConst.ROOMSTATUS_STAYCLEANING + "'";
            sql += "       OR  assignUC_Sub.room_state_class = '" + CommonConst.ROOMSTATUS_CLEANING + "')";
            sql += "    Group by assignUC_Sub.company_no, assignUC_Sub.room_no";
            sql += "   Having assignUC.company_no = assignUC_Sub.company_no";
            sql += "      And assignUC.room_no = assignUC_Sub.room_no";
            sql += "      And assignUC.use_date = max(assignUC_Sub.use_date)";
            sql += "   )";

            sql += "  Union All";

            // R/C
            sql += "  Select assignRC.*, null arrival_date, null stay_days";
            sql += "    , '" + Div_RC + "' co_flag";
            sql += "    From trn_reserve_assign      assignRC";
            sql += "    Inner Join (";
            sql += "      Select * From trn_reserve_assign";
            sql += "       Where company_no        = '" + cond.CompanyNo + "'";
            sql += "         And room_no          != ''";
            sql += "         And use_date          = '" + cond.UseDate + "'";
            sql += "         And (room_state_class = '" + CommonConst.ROOMSTATUS_STAY + "'";
            sql += "          Or  room_state_class = '" + CommonConst.ROOMSTATUS_STAYCLEANING + "'";
            sql += "          Or  room_state_class = '" + CommonConst.ROOMSTATUS_STAYCLEANED + "'";
            sql += "          Or  room_state_class = '" + CommonConst.ROOMSTATUS_CLEANING + "'";
            sql += "          Or  room_state_class = '" + CommonConst.ROOMSTATUS_CO + "')";
            sql += "         And COALESCE(hollow_state_class, '') != '" + CommonConst.HOLLOWSTATUS_HOLLOW + "'";
           
            sql += "     ) assignRC_Sub";
            sql += "      ON assignRC.company_no  = assignRC_Sub.company_no";
            sql += "     And assignRC.reserve_no  = assignRC_Sub.reserve_no";
            sql += "     And assignRC.room_no    != assignRC_Sub.room_no";

            sql += "   Where assignRC.company_no  = '" + cond.CompanyNo + "'";
            sql += "     And assignRC.room_no    != ''";
            sql += "     And assignRC.use_date    = '" + yesterday + "'";
            sql += "     And (assignRC.room_state_class = '" + CommonConst.ROOMSTATUS_STAY + "'";
            sql += "      Or  assignRC.room_state_class = '" + CommonConst.ROOMSTATUS_STAYCLEANING + "'";
            sql += "      Or  assignRC.room_state_class = '" + CommonConst.ROOMSTATUS_STAYCLEANED + "'";
            sql += "      Or  assignRC.room_state_class = '" + CommonConst.ROOMSTATUS_CLEANING + "'";
            sql += "      Or  assignRC.room_state_class = '" + CommonConst.ROOMSTATUS_CO + "')";
            sql += "     And COALESCE(assignRC.hollow_state_class, '') != '" + CommonConst.HOLLOWSTATUS_HOLLOW + "'";

            sql += ") assign";

            sql += "    On room.company_no = assign.company_no";
            sql += "   And room.room_no    = assign.room_no";

            sql += " Where room.company_no = '" + cond.CompanyNo + "'";
            if (!cond.Floor.IsNullOrEmpty()) {
                sql += "   And room.floor = '" + cond.Floor + "'";
            }
            sql += "   And room.status = '" + CommonConst.STATUS_USED + "'";
            sql += " Order by to_number(room.floor,'99999'), to_number(room.room_no,'99999'), assign.co_flag";

            var list = new List<CleaningInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand()) {

                command.CommandText = sql;
                _context.Database.OpenConnection();


                var dictRC = new Dictionary<string, RoomChangeRoomInfo>();  //  予約番号+ルートSEQ, 部屋情報

                try {
                    using (var reader = command.ExecuteReader()) {

                        while (reader.Read()) {
                            var wkInfo = new CleaningInfo();

                            wkInfo.Floor = reader["floor_name"].ToString();
                            wkInfo.RoomNo = reader["room_no"].ToString();
                            wkInfo.RoomType = reader["room_type_name"].ToString();
                            wkInfo.Smoking = reader["smoking_division"].ToString();
                            wkInfo.SmokingName = reader["smoking_name"].ToString();
                            //info.DepatureTime = reader["departure_date"].ToString();
                            //info.ArrivalTime = reader["arrival_date"].ToString();

                            wkInfo.CleaningInstruction = reader["cleaning_instruction"].ToString();
                            wkInfo.CleaningRemarks = reader["cleaning_remarks"].ToString();
                            wkInfo.UseDate = reader["use_date"].ToString();

                            var roomState = string.Empty;

                            wkInfo.RoomStateDiv = reader["co_flag"].ToString().IsNullOrEmpty() ? null : reader["co_flag"].ToString();
                            if (wkInfo.RoomStateDiv != null) {
                                wkInfo.Man = reader["member_male"].ToString().IsNullOrEmpty() ? (int?)null : int.Parse(reader["member_male"].ToString());
                                wkInfo.Woman = reader["member_female"].ToString().IsNullOrEmpty() ? (int?)null : int.Parse(reader["member_female"].ToString());
                                wkInfo.ChildA = reader["member_child_a"].ToString().IsNullOrEmpty() ? (int?)null : int.Parse(reader["member_child_a"].ToString());
                                wkInfo.ChildB = reader["member_child_b"].ToString().IsNullOrEmpty() ? (int?)null : int.Parse(reader["member_child_b"].ToString());
                                wkInfo.ChildC = reader["member_child_c"].ToString().IsNullOrEmpty() ? (int?)null : int.Parse(reader["member_child_c"].ToString());
                                wkInfo.MemberTotal = wkInfo.Man + wkInfo.Woman + wkInfo.ChildA + wkInfo.ChildB + wkInfo.ChildC;
                                roomState = reader["room_state_class"].ToString();

                            } else {
                                wkInfo.Man = null;
                                wkInfo.Woman = null;
                                wkInfo.ChildA = null;
                                wkInfo.ChildB = null;
                                wkInfo.ChildC = null;
                                wkInfo.MemberTotal = null;
                                roomState = null;
                            }

                            var state = string.Empty;
                            RoomStatusValue stateValue = RoomStatusValue.Cleaning;
                            switch (roomState) {

                                case CommonConst.ROOMSTATUS_ASSIGN:
                                    if (cond.UseDate != reader["arrival_date"].ToString()) {
                                        state = "清掃済";
                                        stateValue = RoomStatusValue.Cleaned;
                                    } else {
                                        state = "チェックイン予定";
                                        stateValue = RoomStatusValue.CheckIn;
                                    }
                                    break;

                                case CommonConst.ROOMSTATUS_STAY:
                                    if (wkInfo.RoomStateDiv == Div_Today) {
                                        state = "滞在中";
                                        stateValue = RoomStatusValue.Stay;
                                    } else {
                                        state = "チェックアウト予定";
                                        stateValue = RoomStatusValue.CheckOut;
                                    }
                                    break;

                                case CommonConst.ROOMSTATUS_STAYCLEANING:
                                    state = "清掃開始";
                                    stateValue = RoomStatusValue.StayCleaning;
                                    break;

                                case CommonConst.ROOMSTATUS_STAYCLEANED:
                                    if (wkInfo.RoomStateDiv == Div_Today) {
                                        state = "滞在中";
                                        stateValue = RoomStatusValue.StayCleaned;
                                    } else {
                                        state = "チェックアウト予定";
                                        stateValue = RoomStatusValue.CheckOut;
                                    }
                                    break;

                                case CommonConst.ROOMSTATUS_CO:
                                    state = "チェックアウト済";
                                    stateValue = RoomStatusValue.CheckOuted;
                                    break;

                                case CommonConst.ROOMSTATUS_CLEANING:
                                    state = "清掃開始";
                                    stateValue = RoomStatusValue.Cleaning;
                                    break;

                                case CommonConst.ROOMSTATUS_CLEANED:
                                case null:
                                    state = "清掃済";
                                    stateValue = RoomStatusValue.Cleaned;

                                    break;

                            }

                            var reserveNo = reader["reserve_no"].ToString();
                            var routeSeq = reader["route_seq"].ToString();
                            var key = reserveNo + routeSeq;

                            var rcState = string.Empty;
                            if (wkInfo.RoomStateDiv == Div_RC) {
                                var rcInfo = new RoomChangeRoomInfo();
                                rcInfo.BeforeRoomNo = wkInfo.RoomNo;
                                dictRC.Add(key, rcInfo);
                                rcState = RC_Before;
                            }

                            wkInfo.RoomChangeKey = key;
                            wkInfo.RoomChangeState = rcState;
                            wkInfo.RoomStatus = state;
                            wkInfo.RoomStatusValue = (int)stateValue;

                            var stayFormat = "{0} / {1}";

                            if (!reader["stay_days"].ToString().IsNullOrEmpty()) {

                                if (int.Parse(reader["stay_days"].ToString()) == 0) {
                                    wkInfo.Nights = stayFormat.FillFormat("0", "0");    // 0泊

                                } else {
                                    var arrivalDate = reader["arrival_date"].ToString().ToDate(CommonConst.DATE_FORMAT);
                                    var useDate = cond.UseDate.ToDate(CommonConst.DATE_FORMAT);

                                    var diff = (int)DateAndTime.DateDiff(DateInterval.Day, arrivalDate, useDate);

                                    wkInfo.Nights = stayFormat.FillFormat((diff + 1).ToString(), reader["stay_days"].ToString());
                                }
                            }

                            if (state == "清掃済") {
                                wkInfo.Man = null;
                                wkInfo.Woman = null;
                                wkInfo.ChildA = null;
                                wkInfo.ChildB = null;
                                wkInfo.ChildC = null;
                                wkInfo.MemberTotal = null;
                                wkInfo.Nights = null;
                            }

                            list.Add(wkInfo);
                        }

                    }

                    // R/Cの場合、状態表示を上書き
                    var wkList = list.Where(x => x.RoomStateDiv == Div_Today).ToList();
                    foreach (var info in wkList) {

                        if (!dictRC.ContainsKey(info.RoomChangeKey)) continue;

                        info.RoomChangeState = RC_After;
                        dictRC[info.RoomChangeKey].AfterRoomNo = info.RoomNo;
                    }

                    var rcList = list.Where(x => x.RoomChangeState.Length > 0).ToList();
                    foreach (var rc in rcList) {

                        var roomInfo = dictRC[rc.RoomChangeKey];

                        if (rc.RoomChangeState == RC_Before) {
                            rc.RoomStatus = "チェンジ元(→{0})".FillFormat(roomInfo.AfterRoomNo);
                            rc.RoomStatusValue = (int)RoomStatusValue.ChengeFrom;

                        } else {
                            rc.RoomStatus = "チェンジ先(←{0})".FillFormat(roomInfo.BeforeRoomNo);
                            rc.RoomStatusValue = (int)RoomStatusValue.ChengeTo;
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
        #endregion

        public async Task<int> UpdateRoomStatus(TrnReserveAssignInfo assignInfo) {

            int count = 0;

            assignInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

            string sql = "UPDATE trn_reserve_assign";
            sql += " SET cleaning_instruction = '{0}'";
            sql += " , cleaning_remarks = '{1}'";
            if (assignInfo.IsStatusUpdateData) {
                sql += " , room_state_class = '{8}'";
            }
            sql += " , updator = '{2}'";
            sql += " , udt = '{3}'";
            sql += " WHERE company_no = '{4}'";
            sql += " AND reserve_no = '{5}'";
            sql += " AND use_date = '{6}'";
            sql += " AND route_seq = {7}";

            if (assignInfo.IsStatusUpdateData) {
                string roomStateClass;
                if (assignInfo.RoomStatusValue == (int)RoomStatusValue.Stay || assignInfo.RoomStatusValue == (int)RoomStatusValue.StayCleaning || assignInfo.RoomStatusValue == (int)RoomStatusValue.StayCleaned) {
                    if (assignInfo.RoomStateClass == CommonConst.ROOMSTATUS_CLEANING) {
                        roomStateClass = CommonConst.ROOMSTATUS_STAYCLEANING;
                    } else {
                        roomStateClass = CommonConst.ROOMSTATUS_STAYCLEANED;
                    }
                } else {
                    roomStateClass = assignInfo.RoomStateClass;
                }

                sql = sql.FillFormat(SqlUtils.GetStringWithSqlEscaped(assignInfo.CleaningInstruction), SqlUtils.GetStringWithSqlEscaped(assignInfo.CleaningRemarks), assignInfo.Updator, assignInfo.Udt, assignInfo.CompanyNo, assignInfo.ReserveNo, assignInfo.UseDate, assignInfo.RouteSEQ.ToString(), roomStateClass);
            } else {
                sql = sql.FillFormat(SqlUtils.GetStringWithSqlEscaped(assignInfo.CleaningInstruction), SqlUtils.GetStringWithSqlEscaped(assignInfo.CleaningRemarks), assignInfo.Updator, assignInfo.Udt, assignInfo.CompanyNo, assignInfo.ReserveNo, assignInfo.UseDate, assignInfo.RouteSEQ.ToString());

            }

            using (var command = _context.Database.GetDbConnection().CreateCommand()) {
                command.CommandText = sql;
                _context.Database.OpenConnection();
                try {
                    count = command.ExecuteNonQuery();
                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                } finally {
                    _context.Database.CloseConnection();
                }
            }
            return count;
        }

        #region 部屋割詳細
        /// <summary>
        /// 予約番号のアサインを取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<ReserveModel> GetAssignListByReserveNo(TrnReserveAssignInfo cond)
        {

            var reserveModel = new ReserveModel();

            // アサイン
            string roomType_divison = ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT);

            var sql = "Select assign.*, roomType.code_name room_type_name";

            sql += ", nameReserve.use_date R_use_date, nameReserve.route_seq R_route_seq";
            sql += ", nameReserve.guest_name R_guest_name, nameReserve.guest_kana R_guest_kana";
            sql += ", nameReserve.zip_code R_zip_code, nameReserve.address R_address";
            sql += ", nameReserve.phone_no R_phone_no, nameReserve.mobile_phone_no R_mobile_phone_no";
            sql += ", nameReserve.email R_email, nameReserve.company_name R_company_name, nameReserve.customer_no R_customer_no";
            sql += ", nameReserve.company_no R_company_no, nameReserve.reserve_no R_reserve_no, nameReserve.name_seq R_name_seq";
            sql += ", nameReserve.status R_status, nameReserve.version R_version";
            sql += ", nameReserve.creator R_creator, nameReserve.updator R_updator, nameReserve.cdt R_cdt, nameReserve.udt R_udt";


            sql += ", nameDaily.use_date D_use_date, nameDaily.route_seq D_route_seq";
            sql += ", nameDaily.guest_name D_guest_name, nameDaily.guest_kana D_guest_kana";
            sql += ", nameDaily.zip_code D_zip_code, nameDaily.address D_address";
            sql += ", nameDaily.phone_no D_phone_no, nameDaily.mobile_phone_no D_mobile_phone_no";
            sql += ", nameDaily.email D_email, nameDaily.company_name D_company_name, nameDaily.customer_no D_customer_no";

            sql += ", nameDaily.company_no D_company_no, nameDaily.reserve_no D_reserve_no, nameDaily.name_seq D_name_seq";
            sql += ", nameDaily.status D_status, nameDaily.version D_version";
            sql += ", nameDaily.creator D_creator, nameDaily.updator D_updator, nameDaily.cdt D_cdt, nameDaily.udt D_udt";

            sql += "  From trn_reserve_assign    assign";

            sql += "  Left Join mst_code_name    roomType";
            sql += "    On assign.company_no       = roomType.company_no";
            sql += "   And assign.roomtype_code    = roomType.code";
            sql += "   And roomType.division_code  = '" + roomType_divison + "'";

            sql += "  Left Join trn_name_file    nameReserve";
            sql += "    On assign.company_no       = nameReserve.company_no";
            sql += "   And assign.reserve_no       = nameReserve.reserve_no";
            sql += "   And nameReserve.route_seq     = 0";
            sql += "   And nameReserve.use_date      = '-'";

            sql += "  Left Join trn_name_file    nameDaily";
            sql += "    On assign.company_no       = nameDaily.company_no";
            sql += "   And assign.reserve_no       = nameDaily.reserve_no";
            sql += "   And assign.route_seq        = nameDaily.route_seq";
            sql += "   And assign.use_date         = nameDaily.use_date";

            sql += " Where assign.company_no       = '" + cond.CompanyNo + "'";
            sql += "   And assign.reserve_no       = '" + cond.ReserveNo + "'";
            sql += " Order By assign.use_date, assign.route_seq";

            var list = new List<TrnReserveAssignInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand()) {

                command.CommandText = sql;
                _context.Database.OpenConnection();

                try {
                    using (var reader = command.ExecuteReader()) {

                        while (reader.Read()) {
                            var info = new TrnReserveAssignInfo();

                            info.CompanyNo = reader["company_no"].ToString();
                            info.ReserveNo = reader["reserve_no"].ToString();
                            info.UseDate = reader["use_date"].ToString();
                            info.RouteSEQ = int.Parse(reader["route_seq"].ToString());
                            info.RoomNo = reader["room_no"].ToString();
                            info.RoomtypeCode = reader["roomtype_code"].ToString();
                            info.OrgRoomtypeCode = reader["org_roomtype_code"].ToString();
                            info.RoomtypeSeq = int.Parse(reader["roomtype_seq"].ToString());
                            info.RoomStateClass = reader["room_state_class"].ToString();

                            info.GuestName = reader["guest_name"].ToString();
                            info.MemberMale = int.Parse(reader["member_male"].ToString());
                            info.MemberFemale = int.Parse(reader["member_female"].ToString());
                            info.MemberChildA = int.Parse(reader["member_child_a"].ToString());
                            info.MemberChildB = int.Parse(reader["member_child_b"].ToString());
                            info.MemberChildC = int.Parse(reader["member_child_c"].ToString());

                            info.Email = reader["email"].ToString();
                            info.CleaningInstruction = reader["cleaning_instruction"].ToString();
                            info.CleaningRemarks = reader["cleaning_remarks"].ToString();

                            info.nameFileInfo = new GuestInfo();

                            if (reader["D_guest_name"].ToString().Length > 0) {
                                info.nameFileInfo.NameSeq = int.Parse(reader["D_name_seq"].ToString());
                                info.nameFileInfo.UseDate = reader["D_use_date"].ToString();
                                info.nameFileInfo.RouteSEQ = int.Parse(reader["D_route_seq"].ToString());

                                info.nameFileInfo.GuestName = reader["D_guest_name"].ToString();
                                info.nameFileInfo.GuestNameKana = reader["D_guest_kana"].ToString();
                                info.nameFileInfo.ZipCode = reader["D_zip_code"].ToString();
                                info.nameFileInfo.Address = reader["D_address"].ToString();
                                info.nameFileInfo.Phone = reader["D_phone_no"].ToString();
                                info.nameFileInfo.Cellphone = reader["D_mobile_phone_no"].ToString();
                                info.nameFileInfo.Email = reader["D_email"].ToString();
                                info.nameFileInfo.CompanyName = reader["D_company_name"].ToString();
                                info.nameFileInfo.CustomerNo = reader["D_customer_no"].ToString();

                                info.nameFileInfo.CompanyNo = reader["D_company_no"].ToString();
                                info.nameFileInfo.ReserveNo = reader["D_reserve_no"].ToString();

                                info.nameFileInfo.Status = reader["D_status"].ToString();
                                info.nameFileInfo.Version = int.Parse(reader["D_version"].ToString());
                                info.nameFileInfo.Creator = reader["D_creator"].ToString();
                                info.nameFileInfo.Updator = reader["D_updator"].ToString();
                                info.nameFileInfo.Cdt = reader["D_cdt"].ToString();
                                info.nameFileInfo.Udt = reader["D_udt"].ToString();
                            } else {
                                info.nameFileInfo.NameSeq = int.Parse(reader["R_name_seq"].ToString());
                                info.nameFileInfo.UseDate = reader["R_use_date"].ToString();
                                info.nameFileInfo.RouteSEQ = int.Parse(reader["R_route_seq"].ToString());

                                info.nameFileInfo.GuestName = reader["R_guest_name"].ToString();
                                info.nameFileInfo.GuestNameKana = reader["R_guest_kana"].ToString();
                                info.nameFileInfo.ZipCode = reader["R_zip_code"].ToString();
                                info.nameFileInfo.Address = reader["R_address"].ToString();
                                info.nameFileInfo.Phone = reader["R_phone_no"].ToString();
                                info.nameFileInfo.Cellphone = reader["R_mobile_phone_no"].ToString();
                                info.nameFileInfo.Email = reader["R_email"].ToString();
                                info.nameFileInfo.CompanyName = reader["R_company_name"].ToString();
                                info.nameFileInfo.CustomerNo = reader["R_customer_no"].ToString();

                                info.nameFileInfo.CompanyNo = reader["R_company_no"].ToString();
                                info.nameFileInfo.ReserveNo = reader["R_reserve_no"].ToString();

                                info.nameFileInfo.Status = reader["R_status"].ToString();
                                info.nameFileInfo.Version = int.Parse(reader["R_version"].ToString());
                                info.nameFileInfo.Creator = reader["R_creator"].ToString();
                                info.nameFileInfo.Updator = reader["R_updator"].ToString();
                                info.nameFileInfo.Cdt = reader["R_cdt"].ToString();
                                info.nameFileInfo.Udt = reader["R_udt"].ToString();
                            }

                            info.RoomtypeName = reader["room_type_name"].ToString();

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
            reserveModel.assignList = list;

            // 予約基本
            var reserveInfo = new ReserveInfo();
            reserveInfo.ReserveBasicInfo = _context.ReserveBasicInfo
                                    .Where(b => b.CompanyNo == cond.CompanyNo
                                             && b.ReserveNo == cond.ReserveNo)
                                    .SingleOrDefault();

            // 変換
            reserveModel.stayInfo = new StayInfo();
            reserveModel.stayInfo.ArrivalDate = reserveInfo.ReserveBasicInfo.ArrivalDate;
            reserveModel.stayInfo.StayDays = reserveInfo.ReserveBasicInfo.StayDays;
            reserveModel.stayInfo.DepartureDate = reserveInfo.ReserveBasicInfo.DepartureDate;
            reserveModel.stayInfo.ReserveDate = reserveInfo.ReserveBasicInfo.ReserveDate;
            reserveModel.stayInfo.MemberMale = reserveInfo.ReserveBasicInfo.MemberMale;
            reserveModel.stayInfo.MemberFemale = reserveInfo.ReserveBasicInfo.MemberFemale;
            reserveModel.stayInfo.MemberChildA = reserveInfo.ReserveBasicInfo.MemberChildA;
            reserveModel.stayInfo.MemberChildB = reserveInfo.ReserveBasicInfo.MemberChildB;
            reserveModel.stayInfo.MemberChildC = reserveInfo.ReserveBasicInfo.MemberChildC;
            reserveModel.stayInfo.AdjustmentFlag = reserveInfo.ReserveBasicInfo.AdjustmentFlag;
            reserveModel.stayInfo.ReserveStateDivision = reserveInfo.ReserveBasicInfo.ReserveStateDivision;

            // Base
            reserveModel.stayInfo.CompanyNo = reserveInfo.ReserveBasicInfo.CompanyNo;
            reserveModel.stayInfo.Status = reserveInfo.ReserveBasicInfo.Status;
            reserveModel.stayInfo.Version = reserveInfo.ReserveBasicInfo.Version;
            reserveModel.stayInfo.Creator = reserveInfo.ReserveBasicInfo.Creator;
            reserveModel.stayInfo.Updator = reserveInfo.ReserveBasicInfo.Updator;
            reserveModel.stayInfo.Cdt = reserveInfo.ReserveBasicInfo.Cdt;
            reserveModel.stayInfo.Udt = reserveInfo.ReserveBasicInfo.Udt;

            return reserveModel;
        }

        /// <summary>
        /// アサイン情報/氏名ファイルを更新
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> UpdateRoomDetails(UpdateRoomDetails info) {

            try {

                var updateDate = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction()) {
                    try {
                        foreach (var assignInfo in info.assignList) {

                            // versionチェック
                            if (!CheckVersionReserveAssignInfo(assignInfo)) return (int)DBUpdateResult.VersionError;

                            assignInfo.Version++;
                            assignInfo.Udt = updateDate;

                            _context.ReserveAssignInfo.Update(assignInfo);
                            if (_context.SaveChanges() != 1) {
                                return (int)CommonEnum.DBUpdateResult.Error;
                            }
                        }

                        foreach (var nameFileInfo in info.nameFileList) {

                            // 変換
                            nameFileInfo.GuestKana = nameFileInfo.GuestNameKana;
                            nameFileInfo.PhoneNo = nameFileInfo.Phone;
                            nameFileInfo.MobilePhoneNo = nameFileInfo.Cellphone;

                            // バージョンチェック
                            var wkInfo = _context.NameFileInfo.Where(x => x.CompanyNo == nameFileInfo.CompanyNo
                                                                    && x.ReserveNo == nameFileInfo.ReserveNo
                                                                    && x.UseDate == nameFileInfo.UseDate
                                                                    && x.RouteSEQ == nameFileInfo.RouteSEQ
                                                                    && x.NameSeq == nameFileInfo.NameSeq)
                                                            .AsNoTracking()
                                                            .SingleOrDefault();
                            var insertFlag = false;
                            if (wkInfo == null) {
                                insertFlag = true;

                            } else if (wkInfo.Version != nameFileInfo.Version) {
                                return (int)DBUpdateResult.VersionError;
                            }

                            if (insertFlag) {
                                // nameSeq 採番

                                nameFileInfo.CustomerNo = string.Empty;
                                nameFileInfo.Cdt = updateDate;
                                nameFileInfo.Udt = updateDate;

                                _context.NameFileInfo.Add(nameFileInfo);

                            } else {

                                nameFileInfo.Udt = updateDate;
                                nameFileInfo.Version += 1;

                                _context.NameFileInfo.Update(nameFileInfo);
                            }
                        }

                        tran.Commit();
                    } catch (Exception ex) {
                        tran.Rollback();
                        throw ex;
                    }
                }

                _context.SaveChanges();

                return (int)CommonEnum.DBUpdateResult.Success;


            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return (int)DBUpdateResult.Error;
            }

        }



        #endregion
    }
}

