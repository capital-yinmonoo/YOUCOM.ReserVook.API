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
    public class RoomInfoService : IRoomInfoService
    {

        private DBContext _context;

        public RoomInfoService(DBContext context)
        {
            _context = context;
        }

        
        // 情報取得
        public async Task<MstRoomsInfo> GetRoomInfo(MstRoomsInfo roomInfo)
        {
            var list = new MstRoomsInfo();
            var command = _context.Database.GetDbConnection().CreateCommand();
            //string sql = "SELECT room.room_no,room.room_name,reserve.reserve_no,reserve.member_male";
            //sql += ",reserve.member_female,reserve.member_child_a";
            //sql += ",reserve.member_child_b,reserve.member_child_c";
            //sql += ",reserve.cleaning_remarks,reserve.cleaning_instruction,reserve.room_state_class, reserve.route_seq";
            //sql += " FROM mst_rooms room";

            //sql += " LEFT JOIN trn_reserve_assign reserve";
            //sql += " ON reserve.company_no = room.company_no";
            //sql += " AND reserve.room_no = room.room_no";
            //sql += " AND reserve.use_date = '" + roomInfo.Today + "'";

            //sql += " WHERE room.company_no = '" + roomInfo.CompanyNo + "'";
            //sql += " AND room.room_no = '" + roomInfo.RoomNo + "'";
            //sql += " AND room.status <> '" + CommonConst.STATUS_UNUSED + "'";

            string roomType_divison = ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT);
            string floor_divison = ((int)CommonEnum.CodeDivision.Floor).ToString(CommonConst.CODE_DIVISION_FORMAT);
            string smoking_divison = ((int)CommonEnum.CodeDivision.IsForbid).ToString(CommonConst.CODE_DIVISION_FORMAT);
            string roomTypeDivision_divison = ((int)CommonEnum.CodeDivision.RoomTypeDivision).ToString(CommonConst.CODE_DIVISION_FORMAT);
            string realRoom_Code = ((int)CommonEnum.RoomTypeDivision.Real).ToString();

            // C/O予定取得用
            string yesterday = roomInfo.Today.ToDate(CommonConst.DATE_FORMAT).AddDays(-1).ToString(CommonConst.DATE_FORMAT);
            string mainReserve = ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString();

            const string Div_Today = "3";
            const string Div_CODate = "2";
            const string Div_Uncleaned = "1";
            const string Div_RC = "0";
            const string Div_Hollow = "4";


            var sql = "Select room.room_no, room.room_name";
            sql += ", assign.reserve_no, assign.route_seq, assign.use_date, assign.room_state_class";
            sql += ", assign.member_male, assign.member_female, assign.member_child_a, assign.member_child_b, assign.member_child_c";
            sql += ", assign.cleaning_instruction, assign.cleaning_remarks, assign.arrival_date, assign.stay_days, assign.co_flag";
            sql += "  From mst_rooms room";
            sql += "  Left Join (";
            // 当日分
            sql += "  Select assign.*, basic.arrival_date, basic.stay_days";
            sql += "    , '" + Div_Today + "' co_flag";
            sql += "    From trn_reserve_assign      assign";
            sql += "    Left Join trn_reserve_basic  basic";
            sql += "     On assign.company_no             = basic.company_no";
            sql += "    And assign.reserve_no             = basic.reserve_no";
            sql += "    And basic.reserve_state_division  = '" + mainReserve + "'";
            sql += "    And basic.arrival_date           <= '" + roomInfo.Today + "'";
            sql += "    And basic.departure_date         >= '" + roomInfo.Today + "'";
            sql += "  Where assign.company_no = '" + roomInfo.CompanyNo + "'";
            sql += "    And assign.room_no = '" + roomInfo.RoomNo + "'";
            sql += "    And assign.use_date = '" + roomInfo.Today + "'";
            sql += "    And COALESCE(assign.hollow_state_class, '') != '" + CommonConst.HOLLOWSTATUS_HOLLOW + "'";

            sql += "  Union All";

            // チェックアウト予定
            sql += "  Select assignCO.*, null arrival_date, null stay_days";
            sql += "    , '" + Div_CODate + "' co_flag";
            sql += "    From trn_reserve_assign      assignCO";
            sql += "   Inner Join trn_reserve_basic  basicCO";
            sql += "     On assignCO.company_no             = basicCO.company_no";
            sql += "    And assignCO.reserve_no             = basicCO.reserve_no";
            sql += "    And basicCO.departure_date          = '" + roomInfo.Today + "'";
            sql += "    And basicCO.reserve_state_division  = '" + mainReserve + "'";
            sql += "    And basicCO.stay_days              != 0";
            sql += "  Where assignCO.company_no = '" + roomInfo.CompanyNo + "'";
            sql += "    And assignCO.room_no = '" + roomInfo.RoomNo + "'";
            sql += "    And assignCO.use_date = '" + yesterday + "'";
            sql += "    And (assignCO.room_state_class = '" + CommonConst.ROOMSTATUS_STAY + "'";
            sql += "     Or assignCO.room_state_class = '" + CommonConst.ROOMSTATUS_STAYCLEANING + "'";
            sql += "     Or assignCO.room_state_class = '" + CommonConst.ROOMSTATUS_STAYCLEANED + "')";

            sql += "  Union All";

            // チェックアウト予定(中抜け)
            sql += "  Select assignHOLLOW.*, null arrival_date, null stay_days";
            sql += "    , '" + Div_Hollow + "' co_flag";
            sql += "    From trn_reserve_assign      assignHOLLOW";
            sql += "  Where assignHOLLOW.company_no         = '" + roomInfo.CompanyNo + "'";
            sql += "    And assignHOLLOW.room_no           = '" + roomInfo.RoomNo + "'";
            sql += "    And assignHOLLOW.use_date           = '" + roomInfo.Today + "'";
            sql += "    And assignHOLLOW.hollow_state_class = '" + CommonConst.HOLLOWSTATUS_HOLLOW + "'";

            sql += "  Union All";

            // 未清掃
            sql += "  Select assignUC.*, null arrival_date, null stay_days";
            sql += "    , '" + Div_Uncleaned + "' co_flag";
            sql += "    From trn_reserve_assign      assignUC";
            sql += "   Where Exists(";
            sql += "     Select assignUC_Sub.company_no, assignUC_Sub.room_no, max(assignUC_Sub.use_date) use_date";
            sql += "       From trn_reserve_assign   assignUC_Sub";
            sql += "    Where assignUC_Sub.company_no = '" + roomInfo.CompanyNo + "'";
            sql += "      And assignUC_Sub.room_no = '" + roomInfo.RoomNo + "'";
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
            sql += "       Where company_no        = '" + roomInfo.CompanyNo + "'";
            sql += "         And room_no          = '" + roomInfo.RoomNo + "'";
            sql += "         And use_date          = '" + roomInfo.Today + "'";
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

            sql += "   Where assignRC.company_no  = '" + roomInfo.CompanyNo + "'";
            sql += "     And assignRC.room_no    = '" + roomInfo.RoomNo + "'";
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

            sql += " Where room.company_no = '" + roomInfo.CompanyNo + "'";
            if (!roomInfo.Floor.IsNullOrEmpty()) {
                sql += "   And room.floor = '" + roomInfo.Floor + "'";
            }
            sql += "   And room.room_no = '" + roomInfo.RoomNo + "'";
            sql += "   And room.status = '" + CommonConst.STATUS_USED + "'";
            sql += " Order by to_number(room.floor,'99999'), to_number(room.room_no,'99999'), assign.co_flag";


            command.CommandText = sql;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.RoomNo = reader["room_no"].ToString();
                        list.RoomName = reader["room_name"].ToString();
                        list.ReserveNo = reader["reserve_no"].ToString();
                        list.MemberMale = int.Parse(reader["member_male"].ToString());
                        list.MemberFemale = int.Parse(reader["member_female"].ToString());
                        list.MemberChildA = int.Parse(reader["member_child_a"].ToString());
                        list.MemberChildB = int.Parse(reader["member_child_b"].ToString());
                        list.MemberChildC = int.Parse(reader["member_child_c"].ToString());
                        list.CleaningRemarks = reader["cleaning_remarks"].ToString();
                        list.CleaningInstruction = reader["cleaning_instruction"].ToString();
                        list.RoomStateClass = reader["room_state_class"].ToString();
                        list.RouteSeq = int.Parse(reader["route_seq"].ToString());
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

        // 更新
        public async Task<int> UpdateRoomInfo(MstRoomsInfo roomsInfo) {

            int count = 0;

            roomsInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

            string sql = "UPDATE trn_reserve_assign";
            sql += " SET cleaning_instruction = '{0}'";
            sql += " , cleaning_remarks = '{1}'";
            if (roomsInfo.RoomStateClass != null) {
                sql += " , room_state_class = '{8}'";
            }
            sql += " , updator = '{2}'";
            sql += " , udt = '{3}'";
            sql += " WHERE company_no = '{4}'";
            sql += " AND reserve_no = '{5}'";
            sql += " AND use_date = '{6}'";
            sql += " AND route_seq = '{7}'";

            if (roomsInfo.RoomStateClass != null) {
                string roomStateClass;
                if (roomsInfo.OrgRoomStateClass == CommonConst.ROOMSTATUS_STAY || roomsInfo.OrgRoomStateClass == CommonConst.ROOMSTATUS_STAYCLEANING || roomsInfo.OrgRoomStateClass == CommonConst.ROOMSTATUS_STAYCLEANED) {
                    if (roomsInfo.RoomStateClass == CommonConst.ROOMSTATUS_CLEANING) {
                        roomStateClass = CommonConst.ROOMSTATUS_STAYCLEANING;
                    } else {
                        roomStateClass = CommonConst.ROOMSTATUS_STAYCLEANED;
                    }
                } else {
                    roomStateClass = roomsInfo.RoomStateClass;
                }

                sql = sql.FillFormat(SqlUtils.GetStringWithSqlEscaped(roomsInfo.CleaningInstruction), SqlUtils.GetStringWithSqlEscaped(roomsInfo.CleaningRemarks), roomsInfo.Updator, roomsInfo.Udt, roomsInfo.CompanyNo, roomsInfo.ReserveNo, roomsInfo.Today, roomsInfo.RouteSeq.ToString(), roomStateClass);
            } else {
                sql = sql.FillFormat(SqlUtils.GetStringWithSqlEscaped(roomsInfo.CleaningInstruction), SqlUtils.GetStringWithSqlEscaped(roomsInfo.CleaningRemarks), roomsInfo.Updator, roomsInfo.Udt, roomsInfo.CompanyNo, roomsInfo.ReserveNo, roomsInfo.Today, roomsInfo.RouteSeq.ToString());

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

    }
}