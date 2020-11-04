using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;
using YOUCOM.Commons.Extensions;

namespace YOUCOM.ReserVook.API.Services
{
    public class NameSearchService : INameSearchService
    {
        private DBContext _context;

        public NameSearchService(DBContext context)
        {
            _context = context;
        }

        public async Task<List<NameSearchInfo>> GetNameSearchList(NameSearchCondition cond)
        {
            var lists = new List<NameSearchInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                _context.Database.OpenConnection();

                try
                {
                    string sql = "";
                    int dummy;
                    string name = SqlUtils.GetStringContainsPattern(SqlUtils.GetStringWithSqlWildcardsEscaped(SqlUtils.GetStringWithSqlEscaped(cond.Name.Trim())));
                    string phone = SqlUtils.GetStringContainsPattern(SqlUtils.GetStringWithSqlWildcardsEscaped(SqlUtils.GetStringWithSqlEscaped(cond.Phone.Trim().Replace("-", "").Replace("+", ""))));
                    string[] keywords = cond.Keyword.Trim().Replace("　"," ").Split(' ');
                    keywords = keywords.Where(s => s != "").ToArray();
                    for (int i = 0; i < keywords.Count(); i++)
                    {
                        keywords[i] = SqlUtils.GetStringContainsPattern(SqlUtils.GetStringWithSqlWildcardsEscaped(SqlUtils.GetStringWithSqlEscaped(keywords[i])));
                    }

                    // データ取得
                    sql = "SELECT rsv.reserve_no, rsv.arrival_date, rsv.departure_date, rsv.stay_days, ";
                    sql += "name.guest_name, name.guest_kana, name.phone_no, assign.persons, assign.status,";
                    sql += "(rsv.member_male + rsv.member_female + rsv.member_child_a + rsv.member_child_b + rsv.member_child_c) as rsv_persons";
                    sql += " FROM trn_reserve_basic rsv INNER JOIN";
                    sql += "  (SELECT company_no, reserve_no, guest_name, guest_kana, phone_no FROM trn_name_file name";
                    sql += "  WHERE (company_no, reserve_no) IN";
                    sql += "  (";
                    sql += "    SELECT company_no, reserve_no FROM trn_reserve_basic";
                    sql += "    WHERE company_no = '{0}'";
                    sql += "    AND departure_date >= '{1}'";
                    sql += "    AND arrival_date <= '{2}'";
                    sql += "  )";
                    sql += "  AND status = '1'";
                    if (cond.ReserveOnly) {
                        sql += "  AND use_date = '" + CommonConst.USE_DATE_EMPTY + "'";
                    }

                    if (cond.Name.Trim().Length > 0) {
                        var wkSql = "  AND (guest_name LIKE '" + name + "' OR guest_kana LIKE '" + name + "')".FillFormat(name);
                        sql += wkSql;
                    }
                    if (cond.Phone.Trim().Length > 0)
                    {
                        var wkSql = "  AND (replace(replace(phone_no,'-',''),'+','') LIKE '{0}'";
                        wkSql += "    OR replace(replace(mobile_phone_no,'-',''),'+','') LIKE '{0}')";
                        sql += wkSql.FillFormat(phone);
                    }
                    sql += "  ) name";
                    sql += " ON rsv.company_no = name.company_no AND rsv.reserve_no = name.reserve_no";

                    sql += " LEFT JOIN";
                    sql += "  (SELECT company_no, reserve_no, SUM(member_male + member_female + member_child_a + member_child_b + member_child_c) as persons,";
                    sql += "   MAX(CASE room_state_class WHEN 'Assign' THEN 1 WHEN 'Stay' THEN 2 WHEN 'CO' THEN 3 WHEN 'Cleaned' THEN 4 ELSE 0 END) as status";
                    sql += "   FROM trn_reserve_assign";
                    sql += "  WHERE (company_no, reserve_no, use_date) IN";
                    sql += "  (";
                    sql += "    SELECT company_no, reserve_no, arrival_date FROM trn_reserve_basic";
                    sql += "    WHERE company_no = '{0}'";
                    sql += "    AND departure_date >= '{1}'";
                    sql += "    AND arrival_date <= '{2}'";
                    sql += "  )";
                    sql += "  AND status = '1'";
                    sql += "  GROUP BY company_no, reserve_no";
                    sql += "  ) assign";
                    sql += " ON rsv.company_no = assign.company_no AND rsv.reserve_no = assign.reserve_no";

                    sql += " WHERE rsv.company_no = '{0}'";
                    sql += " AND rsv.departure_date >= '{1}'";
                    sql += " AND rsv.arrival_date <= '{2}'";
                    sql += " AND rsv.status = '1'";
                    if (keywords.Count() > 0)
                    {
                        sql += " AND (rsv.company_no, rsv.reserve_no) IN ";
                        sql += " (";
                        sql += "  (";
                        sql += "  SELECT company_no, reserve_no FROM trn_reserve_note";
                        sql += "  WHERE (company_no, reserve_no) IN";
                        sql += "  (";
                        sql += "    SELECT company_no, reserve_no FROM trn_reserve_basic";
                        sql += "    WHERE company_no = '{0}'";
                        sql += "    AND departure_date >= '{1}'";
                        sql += "    AND arrival_date <= '{2}'";
                        sql += "  )";
                        sql += "  AND status = '1'";
                        foreach (var words in keywords)
                        {
                            var wkSql = "  AND remarks LIKE '{0}'".FillFormat(words);
                            sql += wkSql;
                        }
                        sql += "  ) UNION (";
                        sql += "  SELECT company_no, reserve_no FROM trn_name_file";
                        sql += "  WHERE (company_no, reserve_no) IN";
                        sql += "  (";
                        sql += "    SELECT company_no, reserve_no FROM trn_reserve_basic";
                        sql += "    WHERE company_no = '{0}'";
                        sql += "    AND departure_date >= '{1}'";
                        sql += "    AND arrival_date <= '{2}'";
                        sql += "  )";
                        sql += "  AND status = '1'";
                        foreach (var words in keywords)
                        {
                            var wkSql = "  AND address LIKE '{0}'".FillFormat(words);
                            sql += wkSql;
                        }
                        sql += "  )";
                        sql += " )";
                    }

                    sql += " GROUP BY rsv.reserve_no, rsv.arrival_date, rsv.departure_date, rsv.stay_days";
                    sql += ", name.guest_name, name.guest_kana, name.phone_no, assign.persons, assign.status";
                    sql += ", (rsv.member_male + rsv.member_female + rsv.member_child_a + rsv.member_child_b + rsv.member_child_c)";

                    sql += " ORDER BY name.guest_kana, rsv.arrival_date, rsv.reserve_no";

                    // インジェクション対策
                    sql = sql.FillFormat(cond.CompanyNo, cond.UseDateFrom, cond.UseDateTo);

                    command.CommandText = sql;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var list = new NameSearchInfo
                            {
                                //RoomNo = reader["room_no"].ToString(),
                                NameKanji = reader["guest_name"].ToString(),
                                NameKana = reader["guest_kana"].ToString(),
                                Phone = reader["phone_no"].ToString(),
                                ArrivalDate = reader["arrival_date"].ToString(),
                                DepartureDate = reader["departure_date"].ToString(),
                                StayDays = int.Parse(reader["stay_days"].ToString()),
                                //Rooms = int.TryParse(reader["rooms"].ToString(), out dummy) ? int.Parse(reader["rooms"].ToString()) : 0,
                                Persons = int.TryParse(reader["persons"].ToString(), out dummy) ? int.Parse(reader["persons"].ToString()) : 0,
                                Status = "",
                                ReserveNo = reader["reserve_no"].ToString()
                            };

                            // 補完
                            switch (reader["status"].ToString())
                            {
                                case "0":
                                    list.Status = "予約";
                                    break;
                                case "1":
                                    list.Status = "アサイン";
                                    break;
                                case "2":
                                    if (list.DepartureDate == DateTime.Now.ToString("yyyyMMdd")) list.Status = "出発予定"; else list.Status = "滞在中";
                                    break;
                                case "3":
                                    list.Status = "出発済";
                                    break;
                                case "4":
                                    list.Status = "出発済";
                                    break;
                                default:
                                    list.Status = "キャンセル";
                                    list.Persons = int.TryParse(reader["rsv_persons"].ToString(), out dummy) ? int.Parse(reader["rsv_persons"].ToString()) : 0;
                                    break;
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
    }
}
