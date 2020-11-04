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
    public class NameFileService : INameFileService
    {
        private DBContext _context;

        public NameFileService(DBContext context)
        {
            _context = context;
        }

        public async Task<List<GuestInfo>> GetGuestInfoList(GuestInfo cond)
        {
            var list = new List<GuestInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                _context.Database.OpenConnection();

                try
                {
                    var wherePhone = "";
                    var whereGuestKana = "";
                    var whereCustomerKana = "";

                    if (cond.GuestNameKana.Length > 0) {
                        var kana = SqlUtils.GetStringWithSqlEscaped(cond.GuestNameKana);
                        kana = SqlUtils.GetStringWithSqlWildcardsEscaped(kana);

                        whereGuestKana += " AND guest_kana LIKE '%{0}%'".FillFormat(kana);
                        whereCustomerKana += " AND customer_kana LIKE '%{0}%'".FillFormat(kana);
                    }

                    if (cond.Phone.Length > 0) {
                        var phone = cond.Phone.Trim().Replace("-", "").Replace("+", "");
                        phone = SqlUtils.GetStringWithSqlEscaped(phone);
                        phone = SqlUtils.GetStringWithSqlWildcardsEscaped(phone);

                        wherePhone += " AND (replace(replace(phone_no,'-',''),'+','') like '%{0}%'".FillFormat(phone);
                        wherePhone += " OR replace(replace(mobile_phone_no,'-',''),'+','') like '%{0}%')".FillFormat(phone);
                    }

                    var sql = "SELECT name1.*, name2.customer_no ";
                    sql += " FROM (SELECT guest_name,guest_kana,phone_no,mobile_phone_no ,company_name,email,zip_code,address ";
                    sql += " FROM trn_name_file WHERE company_no = '" + cond.CompanyNo + "'";
                    if (whereGuestKana.IsNotBlanks()) {
                        sql += whereGuestKana;
                    }
                    if (wherePhone.IsNotBlanks()) {
                        sql += wherePhone;
                    }
                    sql += " GROUP BY guest_name, guest_kana, phone_no, mobile_phone_no, company_name, email, zip_code, address) name1";

                    sql += " LEFT JOIN (SELECT * FROM trn_name_file name WHERE company_no = '" + cond.CompanyNo + "'";
                    if (whereGuestKana.IsNotBlanks()) {
                        sql += whereGuestKana;
                    }
                    if (wherePhone.IsNotBlanks()) {
                        sql += wherePhone;
                    }
                    sql += " AND coalesce(customer_no, '') <> '') name2";

                    sql += " ON name1.guest_name = name2.guest_name AND name1.guest_kana = name2.guest_kana";
                    sql += " AND name1.phone_no = name2.phone_no AND name1.mobile_phone_no = name2.mobile_phone_no";
                    sql += " AND name1.company_name = name2.company_name AND name1.email = name2.email";
                    sql += " AND name1.zip_code = name2.zip_code AND name1.address = name2.address";

                    sql += " UNION ";

                    sql += " SELECT customer_name guest_name, customer_kana guest_kana, phone_no, mobile_phone_no, company_name, email, zip_code, address, customer_no FROM mst_customer ";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "' AND status = '" + CommonConst.STATUS_USED + "'";
                    if (whereCustomerKana.IsNotBlanks()) {
                        sql += whereCustomerKana;
                    }
                    if (wherePhone.IsNotBlanks()) {
                        sql += wherePhone;
                    }

                    sql += " ORDER BY guest_kana, phone_no, mobile_phone_no";

                    command.CommandText = sql;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var info = new GuestInfo {

                                GuestName = reader["guest_name"].ToString(),
                                GuestNameKana = reader["guest_kana"].ToString(),
                                Phone = reader["phone_no"].ToString(),
                                Cellphone = reader["mobile_phone_no"].ToString(),
                                CompanyName = reader["company_name"].ToString(),
                                Email = reader["email"].ToString(),
                                ZipCode = reader["zip_code"].ToString(),
                                Address = reader["address"].ToString(),
                                CustomerNo = reader["customer_no"].ToString(),
                            };

                            list.Add(info);
                        }
                    }

                    foreach (var info in list) {
                        if (info.CustomerNo.IsNotBlanks()) {
                            var cInfo = _context.CustomerInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.CustomerNo == info.CustomerNo).AsNoTracking().SingleOrDefault();
                            if (cInfo != null) {
                                info.Remarks1 = cInfo.Remarks1;
                                info.Remarks2 = cInfo.Remarks2;
                                info.Remarks3 = cInfo.Remarks3;
                                info.Remarks4 = cInfo.Remarks4;
                                info.Remarks5 = cInfo.Remarks5;
                            }
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
            }

            return list;
        }
    }
}
