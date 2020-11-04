using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Services
{
    public class WebReserveService : IWebReserveService
    {

        private DBContext _context;

        public WebReserveService(DBContext context)
        {
            _context = context;
        }

        public async Task<List<WebReserveBaseInfo>> GetWebReserveBaseList(WebReserveBaseInfo cond)
        {

            var lists = new List<WebReserveBaseInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                _context.Database.OpenConnection();

                try
                {

                    string sql = "";
                    int newCnt = 0;
                    int cancelCnt = 0;

                    // 件数取得
                    sql = "SELECT x_data_clsfic, count(*) as cnt FROM fr_d_sc_rcv_base";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND x_travel_agnc_bkng_date = '" + cond.XTravelAgncBkngDate + "'";
                    sql += " GROUP BY x_data_clsfic";
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            switch (reader["x_data_clsfic"].ToString())
                            {
                                case "NewBookReport":
                                    newCnt = int.Parse(reader["cnt"].ToString());
                                    break;
                                case "CancellationReport":
                                    cancelCnt = int.Parse(reader["cnt"].ToString());
                                    break;
                            }
                        }
                    }

                    // データ取得
                    sql = "SELECT * FROM fr_d_sc_rcv_base";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND x_travel_agnc_bkng_date = '" + cond.XTravelAgncBkngDate + "'";
                    sql += " ORDER BY sc_cd, sc_rcv_seq";
                    command.CommandText = sql;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var list = new WebReserveBaseInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                XDataClsfic = reader["x_data_clsfic"].ToString(),
                                XDataId = reader["x_data_id"].ToString(),

                                XSalesOfcCompanyCd = reader["x_sales_ofc_company_cd"].ToString(),
                                XSalesOfcCompanyNm = reader["x_sales_ofc_company_nm"].ToString(),
                                XSalesOfcNm = reader["x_sales_ofc_nm"].ToString(),
                                XSalesOfcCd = reader["x_sales_ofc_cd"].ToString(),
                                XSalesOfcPrsnInChg = reader["x_sales_ofc_prsn_in_chg"].ToString(),
                                XSalesOfcEmail = reader["x_sales_ofc_email"].ToString(),
                                XSalesOfcPhnNum = reader["x_sales_ofc_phn_num"].ToString(),

                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),
                                XTravelAgncBkngDate = reader["x_travel_agnc_bkng_date"].ToString(),
                                XTravelAgncBkngTime = reader["x_travel_agnc_bkng_time"].ToString(),
                                XTravelAgncReportNum = reader["x_travel_agnc_report_num"].ToString(),

                                XGstOrGpNmSnglBt = reader["x_gst_or_gp_nm_sngl_bt"].ToString(),
                                XGstOrGpNmDoubleBt = reader["x_gst_or_gp_nm_double_bt"].ToString(),
                                XGstOrGpNmKanjiNm = reader["x_gst_or_gp_nm_kanji_nm"].ToString(),

                                XCheckInDate = reader["x_check_in_date"].ToString(),
                                XCheckInTime = reader["x_check_in_time"].ToString(),
                                XCheckOutDate = reader["x_check_out_date"].ToString(),
                                XCheckOutTime = reader["x_check_out_time"].ToString(),
                                XNights = int.Parse(reader["x_nights"].ToString()),

                                XTtlRmCnt = int.Parse(reader["x_ttl_rm_cnt"].ToString()),
                                XGrandTtlPaxCnt = int.Parse(reader["x_grand_ttl_pax_cnt"].ToString()),

                                XPackagePlanNm = reader["x_package_plan_nm"].ToString(),
                                XPackagePlanCd = reader["x_package_plan_cd"].ToString(),

                                ReservationNo = reader["reservation_no"].ToString(),
                                ScProcessedCd = reader["sc_processed_cd"].ToString(),
                                ScProcessedMessage = reader["sc_processed_message"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };

                            // 補完
                            switch (list.XDataClsfic)
                            {
                                case "NewBookReport":
                                    list.Type = "新規";
                                    list.TypeColor = "primary";
                                    break;
                                case "CancellationReport":
                                    list.Type = "取消";
                                    list.TypeColor = "warn";
                                    break;
                            }
                            list.NewCount = newCnt;
                            list.CancelCount = cancelCnt;

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

        public async Task<WebReserveInfo> GetWebReserveById(WebReserveBaseInfo cond)
        {
            var reserve = new WebReserveInfo();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                _context.Database.OpenConnection();

                try
                {
                    string sql = "";

                    // ＳＣ受信基本
                    sql = "SELECT * FROM fr_d_sc_rcv_base";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reserve.FrDScRcvBase = new FrDScRcvBaseInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                XDataFr = reader["x_data_fr"].ToString(),
                                XDataClsfic = reader["x_data_clsfic"].ToString(),
                                XDataId = reader["x_data_id"].ToString(),
                                XSystemDate = reader["x_system_date"].ToString(),
                                XSystemTime = reader["x_system_time"].ToString(),

                                XAccmArea = reader["x_accm_area"].ToString(),
                                XAccmNm = reader["x_accm_nm"].ToString(),
                                XAccmCd = reader["x_accm_cd"].ToString(),
                                XChainNm = reader["x_chain_nm"].ToString(),
                                XAccmPrsnInChg = reader["x_accm_prsnIn_chg"].ToString(),
                                XAccmEmail = reader["x_accm_email"].ToString(),
                                XAccmPhnNum = reader["x_accm_phn_num"].ToString(),

                                XSalesOfcCompanyCd = reader["x_sales_ofc_company_cd"].ToString(),
                                XSalesOfcCompanyNm = reader["x_sales_ofc_company_nm"].ToString(),
                                XSalesOfcNm = reader["x_sales_ofc_nm"].ToString(),
                                XSalesOfcCd = reader["x_sales_ofc_cd"].ToString(),
                                XSalesOfcPrsnInChg = reader["x_sales_ofc_prsn_in_chg"].ToString(),
                                XSalesOfcEmail = reader["x_sales_ofc_email"].ToString(),
                                XSalesOfcPhnNum = reader["x_sales_ofc_phn_num"].ToString(),
                                XSalesOfcStatePrvdnc = reader["x_sales_ofc_state_prvdnc"].ToString(),
                                XSalesOfcCityNm = reader["x_sales_ofc_city_nm"].ToString(),
                                XSalesOfcAddressLine = reader["x_sales_ofc_address_line"].ToString(),
                                XSalesOfcStreetNum = reader["x_sales_ofc_street_num"].ToString(),
                                XSalesOfcPostCd = reader["x_sales_ofc_post_cd"].ToString(),

                                XRetailerCompanyNm = reader["x_retailer_company_nm"].ToString(),
                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),
                                XTravelAgncBkngDate = reader["x_travel_agnc_bkng_date"].ToString(),
                                XTravelAgncBkngTime = reader["x_travel_agnc_bkng_time"].ToString(),
                                XTravelAgncReportNum = reader["x_travel_agnc_report_num"].ToString(),

                                XGstOrGpNmSnglBt = reader["x_gst_or_gp_nm_sngl_bt"].ToString(),
                                XGstOrGpNmDoubleBt = reader["x_gst_or_gp_nm_double_bt"].ToString(),
                                XGstOrGpNmKanjiNm = reader["x_gst_or_gp_nm_kanji_nm"].ToString(),

                                XCheckInDate = reader["x_check_in_date"].ToString(),
                                XCheckInTime = reader["x_check_in_time"].ToString(),
                                XCheckOutDate = reader["x_check_out_date"].ToString(),
                                XCheckOutTime = reader["x_check_out_time"].ToString(),
                                XNights = int.Parse(reader["x_nights"].ToString()),
                                XTrnsprt = reader["x_trnsprt"].ToString(),

                                XTtlRmCnt = int.Parse(reader["x_ttl_rm_cnt"].ToString()),
                                XGrandTtlPaxCnt = int.Parse(reader["x_grand_ttl_pax_cnt"].ToString()),
                                XTtlPaxMaleCnt = int.Parse(reader["x_ttl_pax_male_cnt"].ToString()),
                                XTtlPaxFemaleCnt = int.Parse(reader["x_ttl_pax_female_cnt"].ToString()),
                                XTtlChildA70Cnt = int.Parse(reader["x_ttl_child_a_70_cnt"].ToString()),
                                XTtlChildB50Cnt = int.Parse(reader["x_ttl_child_b_50_cnt"].ToString()),
                                XTtlChildC30Cnt = int.Parse(reader["x_ttl_child_c_30_cnt"].ToString()),
                                XTtlCchildDNoneCnt = int.Parse(reader["x_ttl_child_d_none_cnt"].ToString()),
                                XTtlChildENoneCnt = int.Parse(reader["x_ttl_child_e_none_cnt"].ToString()),
                                XTtlChildFNoneCnt = int.Parse(reader["x_ttl_child_f_none_cnt"].ToString()),
                                XTtlCchildOtherCnt = int.Parse(reader["x_ttl_child_other_cnt"].ToString()),

                                XTypeOfGp = reader["x_type_of_gp"].ToString(),
                                XStatus = reader["x_status"].ToString(),
                                XPackageType = reader["x_package_type"].ToString(),
                                XPackagePlanNm = reader["x_package_plan_nm"].ToString(),
                                XPackagePlanCd = reader["x_package_plan_cd"].ToString(),
                                XPackagePlanContent = reader["x_package_plan_content"].ToString(),

                                XMealCond = reader["x_meal_cond"].ToString(),
                                XSpecMealCond = reader["x_spec_meal_cond"].ToString(),
                                XMealPlace = reader["x_meal_place"].ToString(),
                                XModPnt = reader["x_mod_pnt"].ToString(),
                                XCancellationNum = reader["x_cancellation_num"].ToString(),
                                XSpecialSrvcReq = reader["x_special_srvc_req"].ToString(),
                                XOtherSrvcIfrm = reader["x_other_srvc_ifrm"].ToString(),
                                XOtherSrvcIfrm2 = reader["x_other_srvc_ifrm_2"].ToString(),
                                XFollowUpIfrm = reader["x_follow_up_ifrm"].ToString(),

                                XTravelAgncEmail = reader["x_travel_agnc_email"].ToString(),
                                XRmrtOrprsnalrt = reader["x_rmrtOrprsnalrt"].ToString(),
                                XTaxSrvcFee = reader["x_tax_srvc_fee"].ToString(),
                                XPayment = reader["x_payment"].ToString(),
                                XBareNetRt = int.Parse(reader["x_bare_net_rt"].ToString()),
                                XCreditCardAuthority = reader["x_credit_card_authority"].ToString(),
                                XCreditCardNum = reader["x_credit_card_num"].ToString(),

                                XTtlAccmChg = int.Parse(reader["x_ttl_accm_chg"].ToString()),
                                XTtlAccmCnsmptTax = int.Parse(reader["x_ttl_accm_cnsmpt_tax"].ToString()),
                                XTtlAccmHotsprTax = int.Parse(reader["x_ttl_accm_hotspr_tax"].ToString()),
                                XTtlAccmSrvcFee = int.Parse(reader["x_ttl_accm_srvc_fee"].ToString()),
                                XTtlAccmOtherFee = int.Parse(reader["x_ttl_accm_other_fee"].ToString()),
                                XCmmsnPercentage = double.Parse(reader["x_cmmsn_percentage"].ToString()),
                                XTtlAccmCmmsnAmnt = int.Parse(reader["x_ttl_accm_cmmsn_amnt"].ToString()),

                                OnlineDate = reader["online_date"].ToString(),
                                BatchDate = reader["batch_date"].ToString(),
                                ReservationNo = reader["reservation_no"].ToString(),
                                CheckoutDate = reader["checkout_date"].ToString(),

                                ScProcessedCd = reader["sc_processed_cd"].ToString(),
                                ScProcessedMessage = reader["sc_processed_message"].ToString(),
                                XDataId2 = long.Parse(reader["x_data_id_2"].ToString()),
                                XDataClsficOdr = reader["x_data_clsfic_odr"].ToString(),

                                UpdateDatetime = reader["update_datetime"].ToString()
                            };

                            // 補完
                            switch (reserve.FrDScRcvBase.XDataClsfic)
                            {
                                case "NewBookReport":
                                    reserve.FrDScRcvBase.Type = "新規";
                                    reserve.FrDScRcvBase.TypeColor = "primary";
                                    break;
                                case "CancellationReport":
                                    reserve.FrDScRcvBase.Type = "取消";
                                    reserve.FrDScRcvBase.TypeColor = "warn";
                                    break;
                            }
                            if (reserve.FrDScRcvBase.XCheckInTime != null && reserve.FrDScRcvBase.XCheckInTime.Length > 0)
                                reserve.FrDScRcvBase.CheckInTimeDisplay = reserve.FrDScRcvBase.XCheckInTime.Substring(0, 2) + ":" + reserve.FrDScRcvBase.XCheckInTime.Substring(2, 2);
                            else
                                reserve.FrDScRcvBase.CheckInTimeDisplay = "";
                            if (reserve.FrDScRcvBase.XCheckOutTime != null && reserve.FrDScRcvBase.XCheckOutTime.Length > 0)
                                reserve.FrDScRcvBase.CheckOutTimeDisplay = reserve.FrDScRcvBase.XCheckOutTime.Substring(0, 2) + ":" + reserve.FrDScRcvBase.XCheckOutTime.Substring(2, 2);
                            else
                                reserve.FrDScRcvBase.CheckOutTimeDisplay = "";
                            reserve.FrDScRcvBase.TotalPoint = 0;
                        }
                    }

                    // ＳＣ受信エージェント情報
                    sql = "SELECT * FROM fr_d_sc_rcv_agent_if";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reserve.FrDScRcvAgentIf = new FrDScRcvAgentIfInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                ScRcvAgentSeq = int.Parse(reader["sc_rcv_agent_seq"].ToString()),
                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),

                                XPntDiv = reader["x_pnt_div"].ToString(),
                                XPntNm = reader["x_pnt_nm"].ToString(),
                                XPnts = int.Parse(reader["x_pnts"].ToString()),
                                XTtlAccmDeclPnts = int.Parse(reader["x_ttl_accm_decl_pnts"].ToString()),
                                XTtlAccmCnsmptTax = int.Parse(reader["x_ttl_accm_cnsmpt_tax"].ToString()),
                                XAmntClaimed = int.Parse(reader["x_amnt_claimed"].ToString()),

                                XVipCd = reader["x_vip_cd"].ToString(),
                                XAgoRsvNum = reader["x_ago_rsv_num"].ToString(),
                                XFrRsvNum = reader["x_fr_rsv_num"].ToString(),
                                XTodayReserve = reader["x_today_reserve"].ToString(),

                                XTtlMaleCnt = int.Parse(reader["x_ttl_male_cnt"].ToString()),
                                XTtlFemaleCnt = int.Parse(reader["x_ttl_female_cnt"].ToString()),

                                XSettlementDiv = reader["x_settlement_div"].ToString(),
                                XCancellationChg = reader["x_cancellation_chg"].ToString(),
                                XCancellationNotice = reader["x_cancellation_notice"].ToString(),

                                ReservationNo = reader["reservation_no"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };
                            // 補完
                            switch (reserve.FrDScRcvAgentIf.XSettlementDiv)
                            {
                                case "1":
                                    reserve.FrDScRcvAgentIf.PntDivDisplay = "1:法人利用";
                                    break;
                                case "2":
                                    reserve.FrDScRcvAgentIf.PntDivDisplay = "2:カード決済み";
                                    break;
                                case "3":
                                    reserve.FrDScRcvAgentIf.PntDivDisplay = "3:現地払い";
                                    break;
                                case "4":
                                    reserve.FrDScRcvAgentIf.PntDivDisplay = "4:ツアー会社";
                                    break;
                                case "5":
                                    reserve.FrDScRcvAgentIf.PntDivDisplay = "5:一部精算";
                                    break;
                                case "6":
                                    reserve.FrDScRcvAgentIf.PntDivDisplay = "6:エージェント精算";
                                    break;
                                default:
                                    reserve.FrDScRcvAgentIf.PntDivDisplay = "";
                                    break;
                            }
                        }
                    }

                    // ＳＣ受信メンバ情報
                    sql = "SELECT * FROM fr_d_sc_rcv_member_if";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reserve.FrDScRcvMemberIf = new FrDScRcvMemberIfInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                ScRcvMemberSeq = int.Parse(reader["sc_rcv_member_seq"].ToString()),
                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),

                                XUsrNm = reader["x_usr_nm"].ToString(),
                                XUsrKana = reader["x_usr_kana"].ToString(),
                                XUsrTel = reader["x_usr_tel"].ToString(),
                                XUsrMailAddr = reader["x_usr_mail_addr"].ToString(),
                                XUsrZip = reader["x_usr_zip"].ToString(),
                                XUsrAddr = reader["x_usr_addr"].ToString(),
                                XUsrCorp = reader["x_usr_corp"].ToString(),
                                XUsrDep = reader["x_usr_dep"].ToString(),
                                XUsrId = reader["x_usr_id"].ToString(),

                                XUsrGivingPnts = int.Parse(reader["x_usr_giving_pnts"].ToString()),
                                XUsrUsePnts = int.Parse(reader["x_usr_use_pnts"].ToString()),

                                XUsrType = reader["x_usr_type"].ToString(),
                                XUsrDateOfBirth = reader["x_usr_date_of_birth"].ToString(),
                                XUsrGendar = reader["x_usr_gendar"].ToString(),
                                XUsrEmergencyPhnNum = reader["x_usr_emergency_phn_num"].ToString(),
                                XUsrOccupation = reader["x_usr_occupation"].ToString(),
                                XUsrMailMgznFrAccm = reader["x_usr_mail_mgzn_fr_accm"].ToString(),
                                XUsrPost = reader["x_usr_post"].ToString(),
                                XUsrOfcAddr = reader["x_usr_ofc_addr"].ToString(),
                                XUsrOfcPhn = reader["x_usr_ofc_phn"].ToString(),
                                XUsrTtlPnt = int.Parse(reader["x_usr_ttl_pnt"].ToString()),
                                XUsrCorpId = reader["x_usr_corp_id"].ToString(),
                                XUsrCorpKana = reader["x_usr_corp_kana"].ToString(),
                                XMemberOfcPostCd = reader["x_member_ofc_post_cd"].ToString(),

                                XGstReq = reader["x_gst_req"].ToString(),
                                XAdditionalIfrm = reader["x_additional_ifrm"].ToString(),
                                XTtlAccmSrvcChg = int.Parse(reader["x_ttl_accm_srvc_chg"].ToString()),
                                XTtlAccmDeclPnts = int.Parse(reader["x_ttl_accm_decl_pnts"].ToString()),
                                XAmntClaimed = int.Parse(reader["x_amnt_claimed"].ToString()),

                                ReservationNo = reader["reservation_no"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };
                        }
                    }

                    // ＳＣ受信オプション情報
                    sql = "SELECT * FROM fr_d_sc_rcv_opt_if";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    sql += " ORDER BY sc_rcv_opt_if_seq";
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FrDScRcvOptIfInfo data = new FrDScRcvOptIfInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                ScRcvOptIfSeq = int.Parse(reader["sc_rcv_opt_if_seq"].ToString()),
                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),

                                XOptDate = reader["x_opt_date"].ToString(),
                                XNm = reader["x_nm"].ToString(),
                                XNmReq = reader["x_nm_req"].ToString(),
                                XOptCnt = int.Parse(reader["x_opt_cnt"].ToString()),
                                XOptRt = int.Parse(reader["x_opt_rt"].ToString()),
                                XOptCd = reader["x_opt_cd"].ToString(),

                                ReservationNo = reader["reservation_no"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };
                            // 補完
                            data.XOptDateDisplayYM = data.XOptDate.Substring(5);
                            reserve.FrDScRcvOptIf.Add(data);
                        }
                    }

                    // ＳＣ受信ポイント情報
                    sql = "SELECT * FROM fr_d_sc_rcv_pnt_if";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    sql += " ORDER BY sc_rcv_pnt_seq";
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FrDScRcvPntIfInfo data = new FrDScRcvPntIfInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                ScRcvPntSeq = int.Parse(reader["sc_rcv_pnt_seq"].ToString()),
                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),

                                XPntsDiv = int.Parse(reader["x_pnts_div"].ToString()),
                                XPntsDiscntNm = reader["x_pnts_discnt_nm"].ToString(),
                                XPntsDiscnt = int.Parse(reader["x_pnts_discnt"].ToString()),

                                ReservationNo = reader["reservation_no"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };
                            reserve.FrDScRcvPntIf.Add(data);
                        }
                    }
                    if (reserve.FrDScRcvPntIf.Count != 0) reserve.FrDScRcvBase.TotalPoint = reserve.FrDScRcvPntIf.Sum(s => s.XPntsDiscnt);

                    // ＳＣ受信部屋情報
                    sql = "SELECT * FROM fr_d_sc_rcv_rm_if";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    sql += " ORDER BY sc_rcv_rm_if_seq";
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FrDScRcvRmIfInfo data = new FrDScRcvRmIfInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                ScRcvRmIfSeq = int.Parse(reader["sc_rcv_rm_if_seq"].ToString()),
                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),

                                XRmTypeCd = reader["x_rm_type_cd"].ToString(),
                                XRmTypeNm = reader["x_rm_type_nm"].ToString(),
                                XRmCategory = reader["x_rm_category"].ToString(),
                                XViewType = reader["x_view_type"].ToString(),
                                XSmokingOrNonSmoking = reader["x_smoking_or_non_smoking"].ToString(),

                                XPerRmPaxCnt = int.Parse(reader["x_per_rm_pax_cnt"].ToString()),
                                XRmPaxMaleCnt = int.Parse(reader["x_rm_pax_male_cnt"].ToString()),
                                XRmPaxFemaleCnt = int.Parse(reader["x_rm_pax_female_cnt"].ToString()),
                                XRmChildA70Cnt = int.Parse(reader["x_rm_child_a_70_cnt"].ToString()),
                                XRmChildB50Cnt = int.Parse(reader["x_rm_child_b_50_cnt"].ToString()),
                                XRmChildC30Cnt = int.Parse(reader["x_rm_child_c_30_cnt"].ToString()),
                                XRmChildDNoneCnt = int.Parse(reader["x_rm_child_d_none_cnt"].ToString()),
                                XRmChildENoneCnt = int.Parse(reader["x_rm_child_e_none_cnt"].ToString()),
                                XRmChildFNoneCnt = int.Parse(reader["x_rm_child_f_none_cnt"].ToString()),
                                XRmChildOtherCnt = int.Parse(reader["x_rm_child_other_cnt"].ToString()),

                                XRmByRmStatus = reader["x_rm_by_rm_status"].ToString(),
                                XFacilities = reader["x_facilities"].ToString(),
                                XAssignedRmNum = reader["x_assigned_rm_num"].ToString(),
                                XRmSpecialReq = reader["x_rm_special_req"].ToString(),

                                ScRcvRmRtIfCntr = int.Parse(reader["sc_rcv_rm_rt_if_cntr"].ToString()),
                                ScRcvRmGstIfCntr = int.Parse(reader["sc_rcv_rm_gst_if_cntr"].ToString()),

                                ReservationNo = reader["reservation_no"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };
                            reserve.FrDScRcvRmIf.Add(data);
                        }
                    }

                    // ＳＣ受信部屋料金情報
                    sql = "SELECT * FROM fr_d_sc_rcv_rm_rt_if";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    sql += " ORDER BY sc_rcv_rm_if_seq, sc_rcv_rm_rt_if_seq";
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FrDScRcvRmRtIfInfo data = new FrDScRcvRmRtIfInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                ScRcvRmIfSeq = int.Parse(reader["sc_rcv_rm_if_seq"].ToString()),
                                ScRcvRmRtIfSeq = int.Parse(reader["sc_rcv_rm_rt_if_seq"].ToString()),
                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),

                                XRmDate = reader["x_rm_date"].ToString(),
                                XPerPaxRt = int.Parse(reader["x_per_pax_rt"].ToString()),
                                XPerChildA70Rt = int.Parse(reader["x_per_child_a_70_rt"].ToString()),
                                XPerChildB50Rt = int.Parse(reader["x_per_child_b_50_rt"].ToString()),
                                XPerChildC30Rt = int.Parse(reader["x_per_child_c_30_rt"].ToString()),
                                XPerChildDRt = int.Parse(reader["x_per_child_d_rt"].ToString()),
                                XPerChildERt = int.Parse(reader["x_per_child_e_rt"].ToString()),
                                XPerChildFRt = int.Parse(reader["x_per_child_f_rt"].ToString()),
                                XPerChildOtherRt = int.Parse(reader["x_per_child_other_rt"].ToString()),

                                XTtlPerRmRt = int.Parse(reader["x_ttl_per_rm_rt"].ToString()),
                                XTtlPerRmCnsmptTax = int.Parse(reader["x_ttl_per_rm_cnsmpt_tax"].ToString()),
                                XTtlRmHotsprTax = int.Parse(reader["x_ttl_rm_hotspr_tax"].ToString()),
                                XTtlPerRmSrvcFee = int.Parse(reader["x_ttl_per_rm_srvc_fee"].ToString()),

                                ScRmDateGstListCntr = int.Parse(reader["sc_rm_date_gst_list_cntr"].ToString()),

                                ReservationNo = reader["reservation_no"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };

                            // 補完
                            data.XRmDateDisplayMD = data.XRmDate.Substring(4, 2) + "/" + data.XRmDate.Substring(6, 2);

                            reserve.FrDScRcvRmRtIf.Add(data);
                        }
                    }

                    // ＳＣ受信メンバ情報
                    sql = "SELECT * FROM fr_d_sc_rcv_xml";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reserve.FrDScRcvXml = new FrDScRcvXmlInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),

                                ScRcvXml = reader["sc_rcv_xml"].ToString(),
                                ScRcvFileNm = reader["sc_rcv_file_nm"].ToString(),

                                ReservationNo = reader["reservation_no"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };
                        }
                    }

                    // ＳＣ受信リザプリ基本
                    sql = "SELECT * FROM fr_d_sc_rcv_rp_base";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reserve.FrDScRcvRpBase = new FrDScRcvRpBaseInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),

                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),
                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),

                                XTelegramData = reader["x_telegram_data"].ToString(),
                                XTelegramData2 = reader["x_telegram_data_2"].ToString(),

                                XPhnNum = reader["x_phn_num"].ToString(),
                                XEmail = reader["x_email"].ToString(),
                                XPostCd = reader["x_post_cd"].ToString(),
                                XAddress = reader["x_address"].ToString(),
                                XTtlPaxManCnt = int.Parse(reader["x_ttl_pax_man_cnt"].ToString()),

                                XBranchFaxNum = reader["x_branch_fax_num"].ToString(),
                                XVer = reader["x_ver"].ToString(),
                                XRprsnttvMiddleNm = reader["x_rprsnttv_middle_nm"].ToString(),
                                XRprsnttvPhnType = reader["x_rprsnttv_phn_type"].ToString(),
                                XRprsnttvAge = reader["x_rprsnttv_age"].ToString(),
                                XRprsnttvCellularPhn = reader["x_rprsnttv_cellular_phn"].ToString(),
                                XRprsnttvOfficialPhn = reader["x_rprsnttv_official_phn"].ToString(),
                                XRprsnttvGeneration = reader["x_rprsnttv_generation"].ToString(),
                                xRprsnttvGendar = reader["x_rprsnttv_gendar"].ToString(),

                                XAccmId = reader["x_accm_id"].ToString(),
                                XAssignDiv = reader["x_assign_div"].ToString(),
                                XGenderDiv = reader["x_gender_div"].ToString(),
                                XHandleDiv = reader["x_handle_div"].ToString(),
                                XRsvUsrDiv = reader["x_rsv_usr_div"].ToString(),
                                XUseDiv = reader["x_use_div"].ToString(),

                                ScRcvRpRmIfCntr = int.Parse(reader["sc_rcv_rp_rm_if_cntr"].ToString()),

                                ReservationNo = reader["reservation_no"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };
                        }
                    }

                    // ＳＣ受信リザプリ部屋情報
                    sql = "SELECT * FROM fr_d_sc_rcv_rp_rm_if";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    sql += " ORDER BY sc_rcv_rp_rm_if_seq";
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FrDScRcvRpRmIfInfo data = new FrDScRcvRpRmIfInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                ScRcvRpRmIfSeq = int.Parse(reader["sc_rcv_rp_rm_if_seq"].ToString()),
                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),

                                XRmTypeCd = reader["x_rm_type_cd"].ToString(),
                                XRmTypeNm = reader["x_rm_type_nm"].ToString(),
                                XRmCategory = reader["x_rm_category"].ToString(),
                                XViewType = reader["x_view_type"].ToString(),
                                XSmokingOrNonSmoking = reader["x_smoking_or_non_smoking"].ToString(),

                                XPerRmPaxCnt = int.Parse(reader["x_per_rm_pax_cnt"].ToString()),
                                XRmPaxMaleCnt = int.Parse(reader["x_rm_pax_male_cnt"].ToString()),
                                XRmPaxFemaleCnt = int.Parse(reader["x_rm_pax_female_cnt"].ToString()),
                                XRmChildA70Cnt = int.Parse(reader["x_rm_child_a_70_cnt"].ToString()),
                                XRmChildB50Cnt = int.Parse(reader["x_rm_child_b_50_cnt"].ToString()),
                                XRmChildC30Cnt = int.Parse(reader["x_rm_child_c_30_cnt"].ToString()),
                                XRmChildDNoneCnt = int.Parse(reader["x_rm_child_d_none_cnt"].ToString()),

                                XFacilities = reader["x_facilities"].ToString(),
                                XAssignedRmNum = reader["x_assigned_rm_num"].ToString(),
                                XRmSpecialReq = reader["x_rm_special_req"].ToString(),

                                XRmPaxMaleReq = reader["x_rm_pax_male_req"].ToString(),
                                XRmPaxFemaleReq = reader["x_rm_pax_female_req"].ToString(),
                                XRmChildAReq = reader["x_rm_child_a_req"].ToString(),
                                XRmChildBReq = reader["x_rm_child_b_req"].ToString(),
                                XRmChildCReq = reader["x_rm_child_c_req"].ToString(),
                                XRmChildDNoneReq = reader["x_rm_child_d_none_req"].ToString(),

                                XRmTypeAgent = reader["sc_cd"].ToString(),
                                XRmFrame = reader["sc_cd"].ToString(),
                                XNetRmTypeGpCd = reader["sc_cd"].ToString(),
                                XPlanGpCd = reader["sc_cd"].ToString(),
                                XRprsnttvPrsnNm = reader["sc_cd"].ToString(),

                                ReservationNo = reader["reservation_no"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };

                            reserve.FrDScRcvRpRmIf.Add(data);
                        }
                    }

                    // ＳＣ受信リザプリ部屋料金情報
                    sql = "SELECT * FROM fr_d_sc_rcv_rp_rm_rt_if";
                    sql += " WHERE company_no = '" + cond.CompanyNo + "'";
                    sql += " AND sc_cd = '" + cond.ScCd + "'";
                    sql += " AND sc_rcv_seq = " + cond.ScRcvSeq.ToString();
                    sql += " ORDER BY sc_rcv_rp_rm_if_seq, sc_rcv_rp_rm_rt_if_seq";
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FrDScRcvRpRmRtIfInfo data = new FrDScRcvRpRmRtIfInfo
                            {
                                CompanyNo = reader["company_no"].ToString(),
                                ScCd = reader["sc_cd"].ToString(),
                                ScRcvSeq = int.Parse(reader["sc_rcv_seq"].ToString()),

                                ScRcvRpRmIfSeq = int.Parse(reader["sc_rcv_rp_rm_if_seq"].ToString()),
                                ScRcvRpRmRtIfSeq = int.Parse(reader["sc_rcv_rp_rm_rt_if_seq"].ToString()),
                                XTravelAgncBkngNum = reader["x_travel_agnc_bkng_num"].ToString(),

                                XRmDate = reader["x_rm_date"].ToString(),

                                XPerPaxRt = int.Parse(reader["x_per_pax_rt"].ToString()),
                                XPerChildA70Rt = int.Parse(reader["x_per_child_a_70_rt"].ToString()),
                                XPerChildB50Rt = int.Parse(reader["x_per_child_b_50_rt"].ToString()),
                                XPerChildC30Rt = int.Parse(reader["x_per_child_c_30_rt"].ToString()),
                                XPerChildDRt = int.Parse(reader["x_per_child_d_rt"].ToString()),

                                XTtlPerRmRt = int.Parse(reader["x_ttl_per_rm_rt"].ToString()),
                                XTtlPerRmCnsmptTax = int.Parse(reader["x_ttl_per_rm_cnsmpt_tax"].ToString()),
                                XTtlRmHotsprTax = int.Parse(reader["x_ttl_rm_hotspr_tax"].ToString()),
                                XTtlPerRmSrvcFee = int.Parse(reader["x_ttl_per_rm_srvc_fee"].ToString()),

                                XPerMaleRt = int.Parse(reader["x_per_male_rt"].ToString()),
                                XPerFemaleRt = int.Parse(reader["x_per_female_rt"].ToString()),
                                XRmRtPaxMaleCnt = int.Parse(reader["x_rm_rt_pax_male_cnt"].ToString()),
                                XRmRtPaxFemaleCnt = int.Parse(reader["x_rm_rt_pax_female_cnt"].ToString()),
                                XRmRtChildA70Cnt = int.Parse(reader["x_rm_rt_child_a_70_cnt"].ToString()),
                                XRmRtChildB50Cnt = int.Parse(reader["x_rm_rt_child_b_50_cnt"].ToString()),
                                XRmRtChildC30Cnt = int.Parse(reader["x_rm_rt_child_c_30_cnt"].ToString()),
                                XRmRtChildDNoneCnt = int.Parse(reader["x_rm_rt_child_d_none_cnt"].ToString()),


                                XRmRtPaxMaleReq = reader["x_rm_rt_pax_male_req"].ToString(),
                                XRmRtPaxFemaleReq = reader["x_rm_rt_pax_female_req"].ToString(),
                                XRmRtChildA70Req = reader["x_rm_rt_child_a_70_req"].ToString(),
                                XRmRtChildB50Req = reader["x_rm_rt_child_b_50_req"].ToString(),
                                XRmRtChildC30Req = reader["x_rm_rt_child_c_30_req"].ToString(),
                                XRmRtChildDNoneReq = reader["x_rm_rt_child_d_none_req"].ToString(),

                                ReservationNo = reader["reservation_no"].ToString(),
                                UpdateDatetime = reader["update_datetime"].ToString()
                            };
                            // 補完
                            data.XRmDateDisplayMD = data.XRmDate.Substring(4, 2) + "/" + data.XRmDate.Substring(6, 2);

                            reserve.FrDScRcvRpRmRtIf.Add(data);
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

            return reserve;
        }
    }
}
