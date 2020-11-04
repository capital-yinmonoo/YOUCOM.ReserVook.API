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
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Net;
using System.Net.Cache;
using YOUCOM.ReserVook.API.Tools;
using Microsoft.Extensions.Options;
using System.Threading;

namespace YOUCOM.ReserVook.API.Services
{
    public class TrustyouService : ITrustyouService
    {

        private readonly AppSettings _appSettings;

        private const string STATUS_CANCEL = "9";
        private const string RESULT_SUCCESS = "0";
        private const string RESULT_ERROR = "1";
        private const string RESULT_WARNING = "2";

        private const string TRUSTYOU_CONNECT_APL_PATH = "C:\\YOUCOM\\ReserVook\\TrustYouConnect";
        private const string TRUSTYOU_CONNECT_APL_NAME = "YOUCOM.ReserVook.TrustYouConnect.exe";
        private const string TRUSTYOU_CONNECT_SEND_FILE = "SendXML.xml";
        private const string TRUSTYOU_CONNECT_RECV_FILE = "RecvXML.xml";

        /// <summary>
        /// 送信用XMLのHeaderルートノード要素名
        /// </summary>
        private const string _xmlReqHeaderNodeTagName= "wsse:UsernameToken";

        /// <summary>
        /// 送信用XMLのHeaderのユーザー名ノード要素名
        /// </summary>
        private const string _xmlReqHeaderUsernameNode  = "wsse:Username";

        /// <summary>
        /// 送信用XMLのHeaderのパスワードノード要素名
        /// </summary>
        private const string _xmlReqHeaderPasswordNode= "wsse:Password";

        /// <summary>
        /// 送信用XMLのBodyルートノード要素名
        /// </summary>
        private const string _xmlReqBodyNodeTagName  = "OTA_HotelResNotifRQ";

        /// <summary>
        /// 受信XMLのルートノード要素名
        /// </summary>
        private const string _xmlResNodeTagName = "OTA_HotelResNotifRS";

        /// <summary>
        /// データ送信結果:成功
        /// </summary>
        private const string _sendResultSuccess  = "成功";

        /// <summary>
        /// データ送信結果:エラー
        /// </summary>
        private const string _sendResultError = "エラー";

        /// <summary>
        /// データ送信結果:警告
        /// </summary>
        private const string _sendResultWarning  = "警告";

        /// <summary>
        /// 受信XMLノード要素名:エラー
        /// </summary>
        private const string _resultElmSuccess= "Success";

        /// <summary>
        /// 受信XMLノード要素名:エラー
        /// </summary>
        private const string _resultElmErrors = "Errors";

        /// <summary>
        /// 受信XMLノード要素名:警告
        /// </summary>
        private const string _resultElmWarnings = "Warnings";

        /// <summary>
        /// ログメッセージ：送信処理開始
        /// </summary>
        private const string _logMessageStart  = "TrustYou連携送信処理を開始しました";

        /// <summary>
        /// ログメッセージ：送信処理終了
        /// </summary>
        private const string _logMessageEnd = "TrustYou連携送信処理を終了しました";

        /// <summary>
        /// ログメッセージ：送信
        /// </summary>
        private const string _logMessageSend = "宿泊者名:{0} を送信";

        /// <summary>
        /// ログメッセージ：受信
        /// </summary>
        private const string _logMessageReceive = "宿泊者名:{0} の送信結果：{1}";

        /// <summary>
        /// ログメッセージ：データ更新
        /// </summary>
        private const string _logMessageUpdate = "宿泊者名:{0} の送信結果を更新";

        private DBContext _context;

        public TrustyouService(DBContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        #region TrustYouデータ一覧取得
        private List<TrustyouInfo> GetTrustyouListByStayDays(TrustyouCondition cond, bool isDayUse) {

            var sql = "SELECT tra.company_no, tra.reserve_no, tra.room_no, tra.guest_name out_guest_name, tra.email out_email,";
            sql += " tra2.arrival_date, tra2.departure_date, ttd.company_no t_company_no,  ttd.guest_name send_guest_name, ttd.email send_email,";
            sql += " ttd.language_code, ttd.send_date, ttd.send_time, ttd.send_result,";
            sql += " ttd.status, ttd.version, ttd.creator, ttd.updator, ttd.cdt, ttd.udt";

            sql += " FROM trn_reserve_assign tra INNER JOIN trn_reserve_basic trb";
            sql += " ON tra.company_no = trb.company_no AND tra.reserve_no = trb.reserve_no";

            sql += " INNER JOIN (SELECT A.company_no, A.reserve_no, A.route_seq, min(A.use_date) arrival_date, max(A.use_date) departure_date";
            sql += " FROM trn_reserve_assign A INNER JOIN (SELECT company_no, reserve_no, route_seq FROM trn_reserve_assign";
            sql += " WHERE use_date BETWEEN '" + cond.UseDateFrom + "' AND '" + cond.UseDateTo + "' ";
            sql += " AND room_state_class IN ('" + CommonConst.ROOMSTATUS_CO + "', '" + CommonConst.ROOMSTATUS_CLEANED + "', '" + CommonConst.ROOMSTATUS_CLEANING + "') ";
            sql += " AND COALESCE(room_no, '') <> '' AND status <> '" + CommonConst.STATUS_UNUSED +"' ";
            sql += " ORDER BY company_no, reserve_no, route_seq) B";
            sql += " ON A.company_no = B.company_no AND A.reserve_no = B.reserve_no AND A.route_seq = B.route_seq";
            sql += " GROUP BY A.company_no, A.reserve_no, A.route_seq";
            sql += " HAVING max(A.use_date) <= '" + cond.UseDateTo + "' ";
            sql += " ORDER BY A.company_no, A.reserve_no, A.route_seq) tra2";
            sql += " ON tra.company_no = tra2.company_no AND tra.reserve_no = tra2.reserve_no AND tra.route_seq = tra2.route_seq AND tra.use_date = tra2.departure_date";
            if (isDayUse) {
                sql += " LEFT JOIN trn_trustyou_data ttd";
                sql += " ON tra.company_no = ttd.company_no AND tra.reserve_no = ttd.reserve_no AND tra.room_no = ttd.room_no AND tra2.departure_date = ttd.departure_date";
            } else {
                sql += " LEFT JOIN (select *, to_char(to_timestamp(departure_date, 'YYYYMMDD') + '-1 days', 'YYYYMMDD') join_departure_date from trn_trustyou_data) ttd";
                sql += " ON tra.company_no = ttd.company_no AND tra.reserve_no = ttd.reserve_no AND tra.room_no = ttd.room_no AND tra2.departure_date = ttd.join_departure_date";
            }
            if (isDayUse) {
                sql += " WHERE trb.stay_days = 0";
            } else {
                sql += " WHERE trb.stay_days <> 0";
            }
            sql += " AND tra.company_no = '" + cond.CompanyNo + "'";
            sql += " ORDER BY departure_date, reserve_no, room_no";

            var list = new List<TrustyouInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand()) {

                command.CommandText = sql;
                _context.Database.OpenConnection();

                try {
                    using (var reader = command.ExecuteReader()) {

                        while (reader.Read()) {
                            var wkInfo = new TrustyouInfo();

                            wkInfo.CompanyNo = reader["company_no"].ToString();
                            wkInfo.ReserveNo = reader["reserve_no"].ToString();
                            wkInfo.RoomNo = reader["room_no"].ToString();
                            wkInfo.ArrivalDate = reader["arrival_date"].ToString();
                            var depDate = reader["departure_date"].ToString();
                            if (isDayUse) {
                                wkInfo.DepartureDate = depDate;
                            } else {
                                wkInfo.DepartureDate = depDate.ToDate(CommonConst.DATE_FORMAT).AddDays(1).ToString(CommonConst.DATE_FORMAT);
                            }
                            wkInfo.OutGuestName = reader["out_guest_name"].ToString();
                            wkInfo.OutEmail = reader["out_email"].ToString();
                            wkInfo.TCompanyNo = reader["t_company_no"].ToString();
                            wkInfo.SendGuestName = reader["send_guest_name"].ToString();
                            wkInfo.SendEmail = reader["send_email"].ToString();
                            wkInfo.LanguageCode = reader["language_code"].ToString();
                            wkInfo.SendDate = reader["send_date"].ToString();
                            wkInfo.SendTime = reader["send_time"].ToString();
                            wkInfo.SendResult = reader["send_result"].ToString();
                            wkInfo.Status = reader["status"].ToString();
                            wkInfo.Version = reader["version"].ToString().IsNullOrEmpty() ? (int?)null : int.Parse(reader["version"].ToString());
                            wkInfo.Creator = reader["creator"].ToString();
                            wkInfo.Updator = reader["updator"].ToString();
                            wkInfo.Cdt = reader["cdt"].ToString();
                            wkInfo.Udt =  reader["udt"].ToString();

                            if (wkInfo.Status == STATUS_CANCEL) {
                                wkInfo.DisplayStatus = "C";
                            } else {
                                if (wkInfo.SendResult.IsNotBlanks()) {
                                    wkInfo.DisplayStatus = "済";
                                } else {
                                    wkInfo.DisplayStatus = "未";
                                }
                            }

                            wkInfo.DisplayArrivalDate = wkInfo.ArrivalDate.ToDate(CommonConst.DATE_FORMAT).ToString("yyyy/MM/dd");
                            wkInfo.DisplayDepartureDate = wkInfo.DepartureDate.ToDate(CommonConst.DATE_FORMAT).ToString("yyyy/MM/dd");

                            String sendDatetime  = String.Empty;
                            DateTime dt = new DateTime();

                            if (DateTime.TryParseExact(wkInfo.SendDate, CommonConst.DATE_FORMAT, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt)) {
                                sendDatetime = dt.ToString("yyyy/MM/dd");
                            }

                            if (DateTime.TryParseExact(wkInfo.SendTime, CommonConst.TIME_FORMAT, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt)) {
                                sendDatetime += " " + dt.ToString("HH:mm:ss");
                            }
                            wkInfo.SendDateTime = sendDatetime;

                            switch (wkInfo.SendResult) {
                                case RESULT_SUCCESS:
                                    wkInfo.DisplaySendResult = "成功";
                                    break;
                                case RESULT_ERROR:
                                    wkInfo.DisplaySendResult = "エラー";
                                    break;
                                case RESULT_WARNING:
                                    wkInfo.DisplaySendResult = "警告";
                                    break;
                                default:
                                    wkInfo.DisplaySendResult = string.Empty;
                                    break;
                            }

                            wkInfo.HasTempData = wkInfo.TCompanyNo.IsNotBlanks();

                            list.Add(wkInfo);
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


        public async Task<List<TrustyouInfo>> GetTrustyouList(TrustyouCondition cond) {

            var trustyouList = new List<TrustyouInfo>();

            // 日帰り予約の取得
            var dayUseList = GetTrustyouListByStayDays(cond, true);
            if (dayUseList != null && dayUseList.Count > 0) {
                trustyouList.AddRange(dayUseList);
            }

            // 宿泊予約の取得(condの利用日付(開始・終了)を1日前に変更する)
            cond.UseDateFrom = cond.UseDateFrom.ToDate(CommonConst.DATE_FORMAT).AddDays(-1).ToString(CommonConst.DATE_FORMAT);
            cond.UseDateTo = cond.UseDateTo.ToDate(CommonConst.DATE_FORMAT).AddDays(-1).ToString(CommonConst.DATE_FORMAT);
            var stayList = GetTrustyouListByStayDays(cond, false);
            if (stayList != null && stayList.Count > 0) {
                trustyouList.AddRange(stayList);
            }

            // 並び順設定
            trustyouList = trustyouList.OrderBy(x => x.DepartureDate).ThenBy(x => x.ReserveNo).ThenBy(x => x.RoomNo).ToList();

            // No設定
            int seq = 0;
            foreach (var info in trustyouList) {
                info.Seq = seq++;
            }

            return trustyouList;
        }
        #endregion

        #region TrustYou連携データ送受信
        /// <summary>
        /// TrustYou連携データ送受信
        /// </summary>
        /// <param name="sendDataList">連携データリスト</param>
        /// <returns>ログリスト</returns>
        public async Task<List<TrnTrustyouLogInfo>> SendRecvTrustyouData(TrustyouSendRcvCondition sendRecvCond) {

            List<TrustyouInfo> sendDataList = sendRecvCond.SendDataList;
            Boolean isCanceld = sendRecvCond.IsCanceled;

            var companyNo = sendDataList.First().CompanyNo;
            var processUser = sendDataList.First().ProcessUser;

            List<TrnTrustyouLogInfo> logList = new List<TrnTrustyouLogInfo>();

            try {
                MstTrustyouInfo masterInfo = _context.SetTrustyouInfo.Where(w => w.CompanyNo == companyNo).SingleOrDefault();

                // ログ設定
                logList.Add(CreateLogInfo("", _logMessageStart));

                foreach (var sendInfo in sendDataList) {

                    // データ送受信
                    SendRecv(masterInfo, sendInfo, isCanceld, logList);

                    // データ更新
                    UpdateSendData(sendInfo, isCanceld, logList);
                }

                // ログ設定
                logList.Add(CreateLogInfo("", _logMessageEnd));

                // ログ更新
                foreach (var logInfo in logList) {
                    logInfo.CompanyNo = companyNo;
                    logInfo.ProcessUser = processUser;
                    logInfo.Creator = processUser;
                    logInfo.Updator = processUser;
                }
                UpdateLogData(logList);

            } catch (AggregateException ae) {
                Console.WriteLine(ae.InnerExceptions);
                foreach (var exInfo in ae.InnerExceptions) {
                    logList.Add(CreateLogInfo("", "AE ：" + exInfo.ToString()));
                    logList.Add(CreateLogInfo("", "AE ：" + exInfo.Message));
                    logList.Add(CreateLogInfo("", "AE ：" + exInfo.StackTrace));
                    logList.Add(CreateLogInfo("", "AE ：" + exInfo.Source));
                }
                logList.Add(CreateLogInfo("", "AE ：" + ae.ToString()));
                logList.Add(CreateLogInfo("", "システム管理者にご連絡ください。"));
                return logList;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                logList.Add(CreateLogInfo("", "想定外のエラー：" + e.ToString()));
                logList.Add(CreateLogInfo("", "想定外のエラー：" + e.Message));
                logList.Add(CreateLogInfo("", "システム管理者にご連絡ください。"));
                return logList;
            }

            return logList;

        }

        #region データ送受信
        private void SendRecv(MstTrustyouInfo masterInfo, TrustyouInfo sendInfo, Boolean isCanceld, List<TrnTrustyouLogInfo> logList) {

            // TrustYou連携画面で選択したデータを送信用データクラスにセット
            Requestinfo requestData = SetSendInfo(masterInfo, sendInfo, isCanceld);

            // XMLデータ生成
            String xmlString = CreateSendXML(requestData);

            if (xmlString.IsBlanks()) {
                return;
            }

            // データ送信
            XmlDocument rcvXML = new XmlDocument();

            string sendPath = "";
            string recvPath = "";

            // ログ設定
            logList.Add(CreateLogInfo("", _logMessageSend.FillFormat(sendInfo.SendGuestName)));

            string errMsg = "";

            try {

                // ファイル出力
                var path = _appSettings.TrustYouConnectAplPath;
                if (path.IsBlanks()) {
                    path = TRUSTYOU_CONNECT_APL_PATH;
                }
                var dataPath = Path.Combine(path, "data");

                // 送信・受信フォルダ存在チェック
                sendPath = Path.Combine(dataPath, masterInfo.CompanyNo, "Send");
                recvPath = Path.Combine(dataPath, masterInfo.CompanyNo, "Recv");
                if (!Directory.Exists(sendPath)) {
                    Directory.CreateDirectory(sendPath);
                }
                if (!Directory.Exists(recvPath)) {
                    Directory.CreateDirectory(recvPath);
                }

                // 送付用XML出力
                using (System.IO.StreamWriter writer = new StreamWriter(Path.Combine(sendPath, TRUSTYOU_CONNECT_SEND_FILE), false)) {
                    writer.WriteLine(xmlString);
                }

                // 受信XML読込or指定秒数経過するまでループ
                int timeOutSec = _appSettings.TrustYouConnectTimeOutSeconds.ToInt_Or_Zero();
                bool existsRecvXml = false;
                bool isOverSeconds = false;

                DateTime startDt = DateTime.Now;
                while (!existsRecvXml && !isOverSeconds) {

                    Thread.Sleep(1000);

                    // 受信XML存在チェック
                    if (File.Exists(Path.Combine(recvPath, TRUSTYOU_CONNECT_RECV_FILE))) {
                        existsRecvXml = true;
                    }

                    DateTime endDt = DateTime.Now;
                    System.TimeSpan ts = endDt - startDt; // 時間の差分を取得
                    if (ts.TotalSeconds >= timeOutSec) {
                        isOverSeconds = true;
                    }
                }

                // 受信XML存在チェック
                if (existsRecvXml) {
                    try {
                        // 受信XML読込
                        rcvXML.Load(Path.Combine(recvPath, TRUSTYOU_CONNECT_RECV_FILE));
                    } catch (Exception e) {
                        throw;
                    }
                } else {
                    // 受信XMLが取得できない場合
                    errMsg = "受信データの取得に失敗しました。";
                    rcvXML = null;
                }

            } catch (Exception ex) {
                errMsg = ex.Message;
                rcvXML = null;
            } finally {
                // 受信XML存在チェック
                FileInfo fi = new FileInfo(Path.Combine(recvPath, TRUSTYOU_CONNECT_RECV_FILE));
                if (fi.Exists) {
                    if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                        fi.Attributes = FileAttributes.Normal;
                    }
                    fi.Delete();
                }
            }

            // 受信XML解析
            String result = "";
            List<TrnTrustyouLogInfo> errorList = new List<TrnTrustyouLogInfo>();

            if (rcvXML != null && AnalysisXML(rcvXML, errorList, out result)) {

                // ログ設定
                logList.Add(CreateLogInfo("", _logMessageReceive.FillFormat(sendInfo.SendGuestName, result)));

                switch (result) {
                    case _sendResultSuccess:
                        sendInfo.SendResult = "0";
                        break;
                    case _sendResultError:
                        sendInfo.SendResult = "1";

                        // ログ設定
                        foreach (var eInfo in errorList) {
                            logList.Add(CreateLogInfo(eInfo.ErrorCode, "エラーコード：" + eInfo.ErrorCode + "  エラー内容：" + eInfo.LogMessage));
                        }

                        break;
                    case _sendResultWarning:
                        sendInfo.SendResult = "2";

                        // ログ設定
                        foreach (var eInfo in errorList) {
                            logList.Add(CreateLogInfo(eInfo.ErrorCode, "警告コード：" + eInfo.ErrorCode + "  警告内容：" + eInfo.LogMessage));
                        }

                        break;
                    default:
                        sendInfo.SendResult = "1";
                        break;
                }
            } else {
                if (rcvXML == null) {
                    // 失敗
                    // ログ設定
                    sendInfo.SendResult = "1";
                    logList.Add(CreateLogInfo("", _logMessageReceive.FillFormat(sendInfo.SendGuestName, _sendResultError)));
                    logList.Add(CreateLogInfo("", "想定外のエラー：" + errMsg));
                    logList.Add(CreateLogInfo("", "システム管理者にご連絡ください。"));
                } else {
                    // 失敗
                    // ログ設定
                    sendInfo.SendResult = "1";
                    logList.Add(CreateLogInfo("", _logMessageReceive.FillFormat(sendInfo.SendGuestName, _sendResultError)));
                    logList.Add(CreateLogInfo("", "想定外のエラー：" + result));
                    logList.Add(CreateLogInfo("", "システム管理者にご連絡ください。"));
                }
            }

            return;

        }
        #endregion

        #region データセット
        private Requestinfo SetSendInfo(MstTrustyouInfo masterInfo, TrustyouInfo sendInfo, Boolean isCanceld) {

            Requestinfo retInfo = new Requestinfo();

            retInfo.UserName = masterInfo.UserName;
            retInfo.Password = masterInfo.Password;

            // OTA_HotelResNotifRQ
            Guid guidValue = Guid.NewGuid();
            retInfo.EchoToken = guidValue.ToString();
            retInfo.CorrelationID = guidValue.ToString();

            // 初回送信なら「Commit」、再送信なら「Modify」、キャンセルなら「Cancel」をセット
            if (isCanceld) {
                // キャンセル
                retInfo.ResStatus = "Cancel";
            } else if (sendInfo.SendResult.IsBlanks()) {
                // 初回送信
                retInfo.ResStatus = "Commit";
            } else {
                // 再送信
                retInfo.ResStatus = "Modify";
            }

            // OTA_HotelResNotifRQ/RequestorID
            retInfo.RequestorID.ID = Environment.MachineName;

            // OTA_HotelResNotifRQ/HotelReservations/HotelReservation
            if (sendInfo.Status.IsBlanks()) {
                // 初回送信時、システム日付・時刻をセット
                DateTime nowDate = DateTime.Now;
                sendInfo.Cdt = nowDate.ToString("yyyyMMdd HHmmss");
                sendInfo.Udt = nowDate.ToString("yyyyMMdd HHmmss");
                sendInfo.SendDate = nowDate.ToString("yyyyMMdd");
                sendInfo.SendTime = nowDate.ToString("HHmmss");

                retInfo.HotelReservations.HotelReservation.CreateDateTime = nowDate.ToString("yyyy-MM-dd") + "T" + nowDate.ToString("HH:mm:ss");
                retInfo.HotelReservations.HotelReservation.LastModifyDateTime = nowDate.ToString("yyyy-MM-dd") + "T" + nowDate.ToString("HH:mm:ss");
            } else {
                DateTime nowDate = DateTime.Now;

                //CDTは前回登録したものを使用
                String cdtDate  = sendInfo.Cdt.Substring(0, 8).ToDate("yyyyMMdd").ToString("yyyy-MM-dd");
                String cdtTime  = sendInfo.Cdt.Substring(9, 6).ToDate("HHmmss").ToString("HH:mm:ss");

                sendInfo.Udt = nowDate.ToString("yyyyMMdd HHmmss");
                sendInfo.SendDate = nowDate.ToString("yyyyMMdd");
                sendInfo.SendTime = nowDate.ToString("HHmmss");

                retInfo.HotelReservations.HotelReservation.CreateDateTime = cdtDate + "T" + cdtTime;
                retInfo.HotelReservations.HotelReservation.LastModifyDateTime = nowDate.ToString("yyyy-MM-dd") + "T" + nowDate.ToString("HH:mm:ss");
            }

            retInfo.HotelReservations.HotelReservation.ResStatus = "Reserved";

            // OTA_HotelResNotifRQ/HotelReservations/HotelReservation/UniqueID
            retInfo.HotelReservations.HotelReservation.UniqueID.ID = sendInfo.ReserveNo;

            // OTA_HotelResNotifRQ/HotelReservations/HotelReservation/RoomStays/RoomStay/TimeSpan
            retInfo.HotelReservations.HotelReservation.RoomStays.RoomStay.TimeSpan.Start = sendInfo.ArrivalDate.ToDate("yyyyMMdd").ToString("yyyy-MM-dd");
            retInfo.HotelReservations.HotelReservation.RoomStays.RoomStay.TimeSpan.End = sendInfo.DepartureDate.ToDate("yyyyMMdd").ToString("yyyy-MM-dd");

            // OTA_HotelResNotifRQ/HotelReservations/HotelReservation/RoomStays/RoomStay/BasicPropertyInfo
            retInfo.HotelReservations.HotelReservation.RoomStays.RoomStay.BasicPropertyInfo.HotelCode = masterInfo.HotelCode;

            // OTA_HotelResNotifRQ/HotelReservations/ResGuests/ResGuest/Profiles/ProfileInfo/UniqueID
            // 本来は顧客コードだがReserVookは顧客コードが実質使われていないので、Guidをセット
            Guid guidValue2 = Guid.NewGuid();
            retInfo.HotelReservations.HotelReservation.ResGuests.ResGuest.Profiles.ProfileInfo.UniqueID.ID = guidValue2.ToString();

            // OTA_HotelResNotifRQ/HotelReservations/ResGuests/ResGuest/Profiles/ProfileInfo/
            retInfo.HotelReservations.HotelReservation.ResGuests.ResGuest.Profiles.ProfileInfo.Profile.ProfileType = "1";

            // OTA_HotelResNotifRQ/HotelReservations/ResGuests/ResGuest/Profiles/ProfileInfo/Profile/Customer
            // 本来は予約者の地域から言語コードへ変換するが、ReserVookは地域情報を持っていないので"ja"固定
            sendInfo.LanguageCode = "ja";
            retInfo.HotelReservations.HotelReservation.ResGuests.ResGuest.Profiles.ProfileInfo.Profile.Customer.Language = "ja";

            // OTA_HotelResNotifRQ/HotelReservations/ResGuests/ResGuest/Profiles/ProfileInfo/Profile/Customer/PersonName/Surname 
            retInfo.HotelReservations.HotelReservation.ResGuests.ResGuest.Profiles.ProfileInfo.Profile.Customer.PersonName.Surname = sendInfo.SendGuestName.Trim();

            // OTA_HotelResNotifRQ/HotelReservations/ResGuests/ResGuest/Profiles/ProfileInfo/Profile/Customer/Email
            retInfo.HotelReservations.HotelReservation.ResGuests.ResGuest.Profiles.ProfileInfo.Profile.Customer.Email = sendInfo.SendEmail.Trim();

            return retInfo;
        }
        #endregion

        #region 送信用XMLデータ作成
        private string CreateSendXML(Requestinfo requestData) {

            String xmlString = "";

            // XMLドキュメントのインスタンス生成
            XmlDocument doc = new XmlDocument();

            try {
                // テンプレートXML読込
                doc.Load(Path.Combine(Directory.GetCurrentDirectory(), "Resource", "Template.xml"));

                // XMLにrequestDataのデータをセット
                EditXML(requestData, doc);

                xmlString = doc.InnerXml;

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return xmlString;
        }
        #endregion

        #region 送信用XMLに選択したデータをセット
        private void EditXML(Requestinfo requestData, XmlDocument doc) {

            XmlNode nodeInfo;

            // ヘッダー情報
            XmlNode headerRootNode = doc.GetElementsByTagName(_xmlReqHeaderNodeTagName).Item(0);
            foreach (XmlNode headerInfo in headerRootNode.ChildNodes) {
                if (headerInfo.Name == _xmlReqHeaderUsernameNode) {
                    // ユーザー名
                    headerInfo.FirstChild.Value = requestData.UserName;
                } else if (headerInfo.Name == _xmlReqHeaderPasswordNode) {
                    // パスワード
                    headerInfo.FirstChild.Value = requestData.Password;
                } else {
                    // 他項目は変更なし
                }
            }

            // ボディ情報
            XmlNode bodyRootNode = doc.GetElementsByTagName(_xmlReqBodyNodeTagName).Item(0);

            // OTA_HotelResNotifRQ
            nodeInfo = bodyRootNode.Attributes.GetNamedItem("EchoToken");
            nodeInfo.Value = requestData.EchoToken;
            nodeInfo = bodyRootNode.Attributes.GetNamedItem("CorrelationID");
            nodeInfo.Value = requestData.CorrelationID;
            nodeInfo = bodyRootNode.Attributes.GetNamedItem("ResStatus");
            nodeInfo.Value = requestData.ResStatus;


            // OTA_HotelResNotifRQ/RequestorID
            XmlNode ReqIdNode = bodyRootNode.FirstChild;            // RequestorID
            nodeInfo = ReqIdNode.Attributes.GetNamedItem("ID");
            nodeInfo.Value = requestData.RequestorID.ID;


            // OTA_HotelResNotifRQ/HotelReservations/HotelReservation
            XmlNode HotelRsvNode = bodyRootNode.LastChild.FirstChild;       // HotelReservation
            nodeInfo = HotelRsvNode.Attributes.GetNamedItem("CreateDateTime");
            nodeInfo.Value = requestData.HotelReservations.HotelReservation.CreateDateTime;
            nodeInfo = HotelRsvNode.Attributes.GetNamedItem("LastModifyDateTime");
            nodeInfo.Value = requestData.HotelReservations.HotelReservation.LastModifyDateTime;
            nodeInfo = HotelRsvNode.Attributes.GetNamedItem("ResStatus");
            nodeInfo.Value = requestData.HotelReservations.HotelReservation.ResStatus;


            //  OTA_HotelResNotifRQ/HotelReservations/HotelReservation/UniqueID
            XmlNode HRUniqueIdNode = HotelRsvNode.ChildNodes.Item(0);                       // UniqueID
            nodeInfo = HRUniqueIdNode.Attributes.GetNamedItem("ID");
            nodeInfo.Value = requestData.HotelReservations.HotelReservation.UniqueID.ID;


            // OTA_HotelResNotifRQ/HotelReservations/HotelReservation/RoomStays/RoomStay/TimeSpan
            XmlNode TimeSpanNode = HotelRsvNode.ChildNodes.Item(1).FirstChild.FirstChild;               // TimeSpan
            nodeInfo = TimeSpanNode.Attributes.GetNamedItem("Start");
            nodeInfo.Value = requestData.HotelReservations.HotelReservation.RoomStays.RoomStay.TimeSpan.Start;
            nodeInfo = TimeSpanNode.Attributes.GetNamedItem("End");
            nodeInfo.Value = requestData.HotelReservations.HotelReservation.RoomStays.RoomStay.TimeSpan.End;


            // OTA_HotelResNotifRQ/HotelReservations/HotelReservation/RoomStays/RoomStay/BasicPropertyInfo
            XmlNode BasicPropInfoNode   = HotelRsvNode.ChildNodes.Item(1).FirstChild.LastChild;             //  BasicPropertyInfo
            nodeInfo = BasicPropInfoNode.Attributes.GetNamedItem("HotelCode");
            nodeInfo.Value = requestData.HotelReservations.HotelReservation.RoomStays.RoomStay.BasicPropertyInfo.HotelCode;


            // OTA_HotelResNotifRQ/HotelReservations/ResGuests/ResGuest/Profiles/ProfileInfo/UniqueID
            XmlNode ResGuestsNode  = HotelRsvNode.ChildNodes.Item(2);                                   // ResGuests
            XmlNode ProfileInfoNode  = ResGuestsNode.FirstChild.FirstChild.FirstChild;                  // ProfileInfo
            XmlNode PIUniqueIdNode = ProfileInfoNode.FirstChild;                                        // UniqueID
            nodeInfo = PIUniqueIdNode.Attributes.GetNamedItem("ID");
            nodeInfo.Value = requestData.HotelReservations.HotelReservation.ResGuests.ResGuest.Profiles.ProfileInfo.UniqueID.ID;


            // OTA_HotelResNotifRQ/HotelReservations/ResGuests/ResGuest/Profiles/ProfileInfo/Profile
            XmlNode ProfileNode = ProfileInfoNode.LastChild;                                            // Profile
            nodeInfo = ProfileNode.Attributes.GetNamedItem("ProfileType");
            nodeInfo.Value = requestData.HotelReservations.HotelReservation.ResGuests.ResGuest.Profiles.ProfileInfo.Profile.ProfileType;


            // OTA_HotelResNotifRQ/HotelReservations/ResGuests/ResGuest/Profiles/ProfileInfo/Profile/Customer
            XmlNode CustomerNode  = ProfileNode.FirstChild;                                                     // Customer
            nodeInfo = CustomerNode.Attributes.GetNamedItem("Language");
            nodeInfo.Value = requestData.HotelReservations.HotelReservation.ResGuests.ResGuest.Profiles.ProfileInfo.Profile.Customer.Language;


            // OTA_HotelResNotifRQ/HotelReservations/ResGuests/ResGuest/Profiles/ProfileInfo/Profile/Customer/PersonName/Surname
            XmlNode SurnameNode = CustomerNode.FirstChild.FirstChild;                              // Surname
            SurnameNode.FirstChild.Value = requestData.HotelReservations.HotelReservation.ResGuests.ResGuest.Profiles.ProfileInfo.Profile.Customer.PersonName.Surname;


            // OTA_HotelResNotifRQ/HotelReservations/ResGuests/ResGuest/Profiles/ProfileInfo/Profile/Customer/Email
            XmlNode EmailNode = CustomerNode.LastChild;                                           // Email
            EmailNode.FirstChild.Value = requestData.HotelReservations.HotelReservation.ResGuests.ResGuest.Profiles.ProfileInfo.Profile.Customer.Email;

        }
        #endregion

        #region 受信XMLの解析
        private Boolean AnalysisXML(XmlDocument doc, List<TrnTrustyouLogInfo> errorList, out String result) {

            try {
                XmlNode nodeInfo;
                XmlNode rootNode = doc.GetElementsByTagName(_xmlResNodeTagName).Item(0);

                XmlNode resultNode = rootNode.FirstChild;

                switch (resultNode.Name) {
                    case _resultElmSuccess:
                        // 成功
                        result = _sendResultSuccess;
                        break;
                    case _resultElmErrors:
                        // エラー

                        // エラー内容の取得
                        XmlNodeList errorChildNodes = resultNode.ChildNodes;
                        foreach (XmlNode node in errorChildNodes) {

                            TrnTrustyouLogInfo info = new TrnTrustyouLogInfo();

                            nodeInfo = node.Attributes.GetNamedItem("Code");
                            if (nodeInfo != null) {
                                info.ErrorCode = nodeInfo.Value;
                            }
                            nodeInfo = node.Attributes.GetNamedItem("ShortText");
                            if (nodeInfo != null) {
                                info.LogMessage = nodeInfo.Value;
                            }

                            if (info.ErrorCode.IsBlanks() && info.LogMessage.IsBlanks()) {
                                // タイプを取得
                                nodeInfo = node.Attributes.GetNamedItem("Type");
                                if (nodeInfo != null) {
                                    info.ErrorCode = nodeInfo.Value;
                                    info.LogMessage = node.InnerText;
                                    errorList.Add(info);
                                }
                            } else {
                                errorList.Add(info);
                            }
                        }

                        result = _sendResultError;

                        break;
                    case _resultElmWarnings:
                        // 警告

                        // 警告内容の取得
                        XmlNodeList warnChildNodes = resultNode.ChildNodes;

                        foreach (XmlNode node in warnChildNodes) {

                            TrnTrustyouLogInfo info = new TrnTrustyouLogInfo();

                            nodeInfo = node.Attributes.GetNamedItem("Code");
                            if (nodeInfo != null) {
                                info.ErrorCode = nodeInfo.Value;
                            }
                            nodeInfo = node.Attributes.GetNamedItem("ShortText");
                            if (nodeInfo != null) {
                                info.LogMessage = nodeInfo.Value;
                            }

                            if (info.ErrorCode.IsBlanks() && info.LogMessage.IsBlanks()) {
                                // タイプを取得
                                nodeInfo = node.Attributes.GetNamedItem("Type");
                                if (nodeInfo != null) {
                                    info.ErrorCode = nodeInfo.Value;
                                    info.LogMessage = node.InnerText;
                                    errorList.Add(info);
                                }
                            } else {
                                errorList.Add(info);
                            }
                        }

                        result = _sendResultWarning;

                        break;
                    default:
                        result = "";
                        return false;
                }

                return true;
            } catch(Exception ex) {
                result = ex.Message.Length > 100 ? ex.Message.Substring(0, 100) : ex.Message;
                return false;
            }
        }
        #endregion

        #region ログ情報生成
        private TrnTrustyouLogInfo CreateLogInfo(string errorCode, string logMessage) {

            DateTime nowDate = DateTime.Now;
            TrnTrustyouLogInfo logInfo = new TrnTrustyouLogInfo();

            // CompanyNo,LogSeq,ProcessUser,Creator,Updator は更新前にまとめてセット
            logInfo.ProcessDate = nowDate.ToString(CommonConst.DATE_FORMAT);
            logInfo.ProcessTime = nowDate.ToString(CommonConst.TIME_FORMAT);
            logInfo.DisplayProcessDate = nowDate.ToString("yyyy/MM/dd");
            logInfo.DisplayProcessTime = nowDate.ToString("HH:mm:ss");

            logInfo.ErrorCode = errorCode;
            logInfo.LogMessage = logMessage;
            logInfo.Status = Context.CommonConst.STATUS_USED;
            logInfo.Version = 0;
            logInfo.Cdt = nowDate.ToString(CommonConst.DATETIME_FORMAT);
            logInfo.Udt = nowDate.ToString(CommonConst.DATETIME_FORMAT);

            return logInfo;
        }
        #endregion

        private void UpdateSendData(TrustyouInfo sendInfo, Boolean isCanceled, List<TrnTrustyouLogInfo> logList) {

            // 更新用データを作成
            TrnTrustyouInfo updateData = GenerateUpdateData(sendInfo, isCanceled);

            try {
                int intRet = UpdateTrustyouData(updateData);

                // ログ設定
                logList.Add(CreateLogInfo("", _logMessageUpdate.FillFormat(sendInfo.SendGuestName)));

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        private TrnTrustyouInfo GenerateUpdateData(TrustyouInfo sendInfo, Boolean isCanceled) {

            TrnTrustyouInfo tmpInfo = new TrnTrustyouInfo();

            try {
                Boolean isNewData = (sendInfo.TCompanyNo == null || sendInfo.TCompanyNo.IsBlanks());
                tmpInfo.CompanyNo = isNewData ? sendInfo.CompanyNo : sendInfo.TCompanyNo;
                tmpInfo.ReserveNo = sendInfo.ReserveNo;
                tmpInfo.ArrivalDate = sendInfo.ArrivalDate;
                tmpInfo.DepartureDate = sendInfo.DepartureDate;
                tmpInfo.RoomNo = sendInfo.RoomNo;
                tmpInfo.GuestName  = sendInfo.SendGuestName;
                tmpInfo.Email = sendInfo.SendEmail;
                tmpInfo.LanguageCode = sendInfo.LanguageCode;
                tmpInfo.SendDate = sendInfo.SendDate;
                tmpInfo.SendTime = sendInfo.SendTime;
                tmpInfo.SendResult = sendInfo.SendResult;
                tmpInfo.Status = isCanceled ? "9" : "0";
                tmpInfo.Version = isNewData ? 0 : (int)sendInfo.Version;
                tmpInfo.Creator = isNewData ? sendInfo.ProcessUser : sendInfo.Creator;
                tmpInfo.Updator = sendInfo.ProcessUser;
                String nowString  = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                tmpInfo.Cdt = isNewData  ? nowString : sendInfo.Cdt;
                tmpInfo.Udt = nowString;
                tmpInfo.IsNewData = isNewData;

                return tmpInfo;

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private int UpdateTrustyouData(TrnTrustyouInfo updateData) {

            try {
                if (updateData.IsNewData) {
                    _context.TrustyouInfo.Add(updateData);
                    return _context.SaveChanges();
                } else {
                    updateData.Version++;
                    _context.TrustyouInfo.Update(updateData);
                    return _context.SaveChanges();
                }

            } catch (Exception) {
                throw;
            }
        }

        private void UpdateLogData(List<TrnTrustyouLogInfo> logList) {

            try {
                foreach (var logInfo in logList) {
                    var result = AddLog(logInfo);
                }
            } catch (Exception) {
                throw;
            }
        }

        public int AddLog(TrnTrustyouLogInfo logInfo) {

            var list = _context.TrustyouLogInfo.Where(w => w.CompanyNo == logInfo.CompanyNo).AsNoTracking().ToList();

            int seq = 1;
            if (list != null && list.Count != 0) { seq = list.Max(m => m.LogSeq) + 1; }

            try {
                // 追加
                logInfo.LogSeq = seq;

                _context.TrustyouLogInfo.Add(logInfo);
                return _context.SaveChanges();

            } catch(Exception ex) {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        #endregion


        #region TrustYouログデータ取得
        public async Task<List<TrnTrustyouLogInfo>> GetTrustyouLogList(TrustyouLogCondition cond) {

            List<TrnTrustyouLogInfo> trustyouLogList = _context.TrustyouLogInfo.Where(w => w.CompanyNo == cond.CompanyNo && w.ProcessDate == cond.ProcessDate).OrderBy(n => n.LogSeq).ToList();

            foreach (var info in trustyouLogList) {

                info.DisplayProcessDate = info.ProcessDate.ToDate(CommonConst.DATE_FORMAT).ToString("yyyy/MM/dd");

                DateTime dt = new DateTime();
                if (DateTime.TryParseExact(info.ProcessTime, CommonConst.TIME_FORMAT, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt)) {
                    info.DisplayProcessTime = dt.ToString("HH:mm:ss");
                }
            }

            return trustyouLogList;
        }
        #endregion


        #region TrustYouデータ一時保存
        public async Task<int> SaveTemporarilyData(List<TrustyouInfo> tempList) {

            int retVal = 0;

            foreach (var info in tempList) {

                TrnTrustyouInfo trustyouInfo = CreateTemporarilyData(info);

                if (info.Cdt.IsBlanks()) {
                    // 新規登録
                    trustyouInfo.Version = 0;

                    trustyouInfo.Creator = info.ProcessUser;
                    trustyouInfo.Updator = info.ProcessUser;

                    String nowString = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                    trustyouInfo.Cdt = nowString;
                    trustyouInfo.Udt = nowString;

                    // 追加処理
                    retVal += AddTemporarilyData(trustyouInfo);
                } else {
                    // 更新
                    trustyouInfo.Version = (int)info.Version + 1;

                    trustyouInfo.Updator = info.ProcessUser;

                    String nowString = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                    trustyouInfo.Udt = nowString;

                    // 更新処理
                    retVal += UpdateTemporarilyData(trustyouInfo);
                }

                retVal++;
            }

            return retVal;
        }

        private TrnTrustyouInfo CreateTemporarilyData(TrustyouInfo tempInfo) {

            TrnTrustyouInfo retInfo = new TrnTrustyouInfo();

            retInfo.CompanyNo = tempInfo.CompanyNo;
            retInfo.ReserveNo = tempInfo.ReserveNo;
            retInfo.ArrivalDate = tempInfo.ArrivalDate;
            retInfo.DepartureDate = tempInfo.DepartureDate;
            retInfo.RoomNo = tempInfo.RoomNo;
            retInfo.GuestName = tempInfo.SendGuestName;
            retInfo.Email = tempInfo.SendEmail;
            retInfo.LanguageCode = retInfo.LanguageCode;
            retInfo.SendDate = tempInfo.SendDate;
            retInfo.SendTime = tempInfo.SendTime;
            retInfo.SendResult = tempInfo.SendResult;
            retInfo.Status = tempInfo.Status;
            retInfo.Creator = tempInfo.Creator;
            retInfo.Updator = tempInfo.Updator;
            retInfo.Cdt = tempInfo.Cdt;
            retInfo.Udt = tempInfo.Udt;

            return retInfo;

        }


        public int AddTemporarilyData(TrnTrustyouInfo tempInfo) {

            try {
                // 追加
                _context.TrustyouInfo.Add(tempInfo);
                return _context.SaveChanges();

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        public int UpdateTemporarilyData(TrnTrustyouInfo tempInfo) {

            try {
                // 更新
                _context.TrustyouInfo.Update(tempInfo);
                return _context.SaveChanges();

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        #endregion

        #region 送信用データクラス
        public class Requestinfo {

            //ヘッダー
            public String UserName { get; set; }
            public String Password { get; set; }

            //ボディー
            public String EchoToken { get; set; }
            public String CorrelationID { get; set; }
            public String ResStatus { get; set; }
            public RequestorID RequestorID { get; set; }
            public HotelReservations HotelReservations { get; set; }

            public Requestinfo() {
                //ヘッダー
                UserName = "";
                Password = "";

                //ボディー
                EchoToken = "";
                CorrelationID = "";
                ResStatus = "";
                RequestorID = new RequestorID();
                HotelReservations = new HotelReservations();
            }
        }

        public class RequestorID {
            public String ID { get; set; }

            public RequestorID() {
                ID = "";
            }
        }

        public class HotelReservations {
            public HotelReservation HotelReservation { get; set; }

            public HotelReservations() {
                HotelReservation = new HotelReservation();
            }
        }

        public class HotelReservation {
            public String CreateDateTime { get; set; }
            public String LastModifyDateTime { get; set; }
            public String ResStatus { get; set; }
            public UniqueID UniqueID { get; set; }
            public RoomStays RoomStays { get; set; }
            public ResGuests ResGuests { get; set; }

            public HotelReservation() {
                CreateDateTime = "";
                LastModifyDateTime = "";
                ResStatus = "";
                UniqueID = new UniqueID();
                RoomStays = new RoomStays();
                ResGuests = new ResGuests();
            }
        }

        public class UniqueID {
            public String ID { get; set; }

            public UniqueID() {
                ID = "";
            }
        }

        public class RoomStays {
            public RoomStay RoomStay { get; set; }

            public RoomStays() {
                RoomStay = new RoomStay();
            }
        }

        public class RoomStay {
            public TimeSpan TimeSpan { get; set; }
            public BasicPropertyInfo BasicPropertyInfo { get; set; }

            public RoomStay() {
                TimeSpan = new TimeSpan();
                BasicPropertyInfo = new BasicPropertyInfo();
            }
        }

        public class TimeSpan {
            public String Start { get; set; }
            public String End { get; set; }

            public TimeSpan() {
                Start = "";
                End = "";
            }
        }

        public class BasicPropertyInfo {
            public String HotelCode { get; set; }

            public BasicPropertyInfo() {
                HotelCode = "";
            }
        }

        public class ResGuests {
            public ResGuest ResGuest { get; set; }

            public ResGuests() {
                ResGuest = new ResGuest();
            }
        }

        public class ResGuest {
            public Profiles Profiles { get; set; }

            public ResGuest() {
                Profiles = new Profiles();
            }
        }

        public class Profiles {
            public ProfileInfo ProfileInfo { get; set; }

            public Profiles() {
                ProfileInfo = new ProfileInfo();
            }
        }

        public class ProfileInfo {
            public Profile Profile { get; set; }
            public UniqueID UniqueID { get; set; }

            public ProfileInfo() {
                Profile = new Profile();
                UniqueID = new UniqueID();
            }
        }

        public class Profile {
            public Customer Customer { get; set; }
            public String ProfileType { get; set; }

            public Profile() {
                Customer = new Customer();
                ProfileType = "";
            }
        }

        public class Customer {
            public String Language { get; set; }
            public PersonName PersonName { get; set; }
            public Telephone Telephone { get; set; }
            public String Email { get; set; }

            public Customer() {
                Language = "";
                PersonName = new PersonName();
                Telephone = new Telephone();
                Email = "";
            }
        }

        public class PersonName {
            public String Surname { get; set; }
            public String GivenName { get; set; }

            public PersonName() {
                Surname = "";
                GivenName = "";
            }
        }

        public class Telephone {
            public String PhoneNumber { get; set; }
            public String FormattedInd { get; set; }

            public Telephone() {
                PhoneNumber = "";
                FormattedInd = "";
            }
        }
        #endregion

    }

}