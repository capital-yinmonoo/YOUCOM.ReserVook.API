using Microsoft.EntityFrameworkCore;
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
    public class LedgerService : ILedgerService
    {
        private DBContext _context;

        public LedgerService(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 台帳情報 取得
        /// </summary>
        /// <param name="cond">台帳情報 検索条件</param>
        /// <returns></returns>
        public async Task<List<LedgerInfo>> GetList(LedgerInfo cond)
        {

            var lists = new List<LedgerInfo>();
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
                        var info = new LedgerInfo();
                        info.CompanyNo = reader["company_no"].ToString();
                        info.ReserveNo = reader["reserve_no"].ToString();
                        info.ReserveDate = reader["reserve_date"].ToString();
                        info.ReserveStateDivision = reader["reserve_state_division"].ToString();
                        info.ArrivalDate = reader["arrival_date"].ToString();
                        info.DepartureDate = reader["departure_date"].ToString();
                        info.StayDays = reader["stay_days"].ToString().ToInt_Or_Zero();
                        info.AdjustmentFlag = reader["adjustment_flag"].ToString();
                        info.CustomerNo = reader["customer_no"].ToString();
                        info.AgentCode = reader["agent_code"].ToString();
                        info.AgentRemarks = reader["agent_remarks"].ToString();
                        info.XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString();
                        info.Status = reader["status"].ToString();
                        info.Version = reader["version"].ToString().ToInt_Or_Zero();
                        info.Creator = reader["creator"].ToString();
                        info.Updator = reader["updator"].ToString();
                        info.Cdt = reader["cdt"].ToString();
                        info.Udt = reader["udt"].ToString();
                        info.GuestName = reader["guest_name"].ToString();
                        info.GuestKana = reader["guest_kana"].ToString();
                        info.AmountPrice = reader["amount_price"].ToString().ToDecimal_Or_Zero();
                        info.InsideTaxPrice = reader["inside_tax_price"].ToString().ToDecimal_Or_Zero();

                        lists.Add(info);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            finally
            {
                _context.Database.CloseConnection();
            }

            try
            {
                // 部屋マスタ 取得
                var mstRooms = _context.RoomsInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.Status != CommonConst.STATUS_UNUSED).AsNoTracking().ToList();

                foreach (var info in lists)
                {
                    // 宿泊かつCO日は前日の予約アサインから取得
                    var date = cond.UseDate;
                    if (date == info.DepartureDate && info.StayDays != 0)
                    {
                        date = date.ToDate(CommonConst.DATE_FORMAT).AddDays(-1).ToString(CommonConst.DATE_FORMAT);
                    }

                    // 人数をセット
                    info.MemberMale = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.ReserveNo == info.ReserveNo && w.UseDate == date)
                                                                   .AsNoTracking()
                                                                   .Sum(s => s.MemberMale);
                    info.MemberFemale = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.ReserveNo == info.ReserveNo && w.UseDate == date)
                                                                   .AsNoTracking()
                                                                   .Sum(s => s.MemberFemale);
                    info.MemberChildA = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.ReserveNo == info.ReserveNo && w.UseDate == date)
                                                                   .AsNoTracking()
                                                                   .Sum(s => s.MemberChildA);
                    info.MemberChildB = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.ReserveNo == info.ReserveNo && w.UseDate == date)
                                                                   .AsNoTracking()
                                                                   .Sum(s => s.MemberChildB);
                    info.MemberChildC = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.ReserveNo == info.ReserveNo && w.UseDate == date)
                                                                   .AsNoTracking()
                                                                   .Sum(s => s.MemberChildC);

                    // 部屋名をセット
                    var assignRoomNos = _context.ReserveAssignInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.ReserveNo == info.ReserveNo && w.UseDate == date)
                                                                    .AsNoTracking()
                                                                    .Select(s => s.RoomNo)
                                                                    .ToList();
                    var wklist = new List<string>();
                    foreach (var roomNo in assignRoomNos)
                    {
                        string roomName = mstRooms.Where(w => w.RoomNo == roomNo).Select(s => s.RoomName).SingleOrDefault();
                        wklist.Add(roomName);
                    }

                    info.AssignRoomList = wklist;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

            return lists;
        }

        /// <summary>
        /// 台帳情報取得用 Query作成
        /// </summary>
        /// <param name="cond">検索条件</param>
        /// <returns>query</returns>
        private static string CreateQuery(LedgerInfo cond)
        {
            var mealDivision = ((int)CommonEnum.CodeDivision.MealDivision).ToString(CommonConst.CODE_DIVISION_FORMAT);

            string query = " SELECT ";
            query += "   basic.* ";
            query += "   , name.guest_name ";
            query += "   , name.guest_kana ";
            query += "   , sales.amount_price ";
            query += "   , sales.inside_tax_price  ";
            query += " FROM ";
            query += "   trn_reserve_basic basic  ";
            query += "   LEFT JOIN trn_name_file name  ";
            query += "     ON basic.company_no = name.company_no  ";
            query += "     AND basic.reserve_no = name.reserve_no  ";
            query += "     AND name.use_date = '"  + CommonConst.USE_DATE_EMPTY + "'";
            query += "     AND name.route_seq = " + CommonConst.DEFAULT_ROUTE_SEQ;
            query += "   LEFT JOIN (  ";
            query += "     SELECT ";
            query += "       company_no ";
            query += "       , reserve_no ";
            query += "       , SUM(amount_price) AS amount_price ";
            query += "       , SUM(inside_tax_price) AS inside_tax_price  ";
            query += "     FROM ";
            query += "       trn_sales_details ";
            query += "     WHERE ";
            query += "       company_no = '" + cond.CompanyNo + "'  ";
            query += "       AND use_date = '" + cond.UseDate + "'  ";
            query += "       AND item_division <> '" + ((int)CommonEnum.ItemDivision.SetItem).ToString() + "'  "; ;
            query += "     GROUP BY ";
            query += "       company_no ";
            query += "       , reserve_no ";
            query += "   ) sales  ";
            query += "     ON basic.company_no = sales.company_no  ";
            query += "     AND basic.reserve_no = sales.reserve_no  ";
            query += " WHERE ";
            query += "   basic.company_no = '" + cond.CompanyNo + "'  ";
            query += "   AND basic.reserve_state_division = '" + ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString() + "' ";
            query += "   AND (  ";
            query += "     (  ";
            query += "       basic.stay_days = 0  ";
            query += "       AND basic.arrival_date = '" + cond.UseDate + "' ";
            query += "     )  ";
            query += "     OR (  ";
            query += "       basic.stay_days <> 0  ";
            query += "       AND basic.arrival_date <= '" + cond.UseDate + "'  ";
            query += "       AND '" + cond.UseDate + "' <= basic.departure_date ";
            query += "     ) ";
            query += "   )  ";
            query += " ORDER BY ";
            query += "   basic.company_no ";
            query += "   , basic.reserve_no ";

            return query;

        }
    }
}
