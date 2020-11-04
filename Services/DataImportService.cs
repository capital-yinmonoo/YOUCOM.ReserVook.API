using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YOUCOM.Commons.Extensions;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Controllers;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Services {
    public class DataImportService : IDataImportService {

        private DBContext _context;

        public DataImportService(DBContext context) {
            _context = context;
        }

        // 予約情報取得
        public async Task<ImportResultInfo> ImportReserveData(List<ImportReserveInfo> importReserveList) {

            var companyNo = importReserveList.First().CompanyNo;

            var result = new ImportResultInfo();

            // インポートリストの各項目が正しい形式かチェック
            var existsInvalidData = FoundInvalidReserveData(importReserveList, out result);
            if (existsInvalidData) {
                return result;
            }

            // トランザクション作成
            using (var tran = _context.Database.BeginTransaction()) {

                try {
                    // 予約番号採番用サービスのインスタンス生成
                    ReserveService reserveService = new ReserveService(_context, null, null);

                    foreach (var info in importReserveList) {

                        // 予約番号 新規採番
                        var companyInfo = reserveService.Numbering(companyNo);
                        var reserveNo = companyInfo.LastReserveNo;

                        // 予約基本
                        TrnReserveBasicInfo reserveBasicInfo = SetReserveBasicInfo(info, companyNo, reserveNo);
                        _context.ReserveBasicInfo.Add(reserveBasicInfo);
                        _context.SaveChanges();

                        // 氏名ファイル
                        TrnNameFileInfo nameFileInfo = SetNameFileInfo(info, companyNo, reserveNo);
                        _context.NameFileInfo.Add(nameFileInfo);
                        _context.SaveChanges();

                        // 予約部屋タイプ
                        List<TrnReserveRoomtypeInfo> roomTypeList = SetRoomTypeList(info, companyNo, reserveNo);
                        foreach (var roomTypeInfo in roomTypeList) {
                            _context.ReserveRoomtypeInfo.Add(roomTypeInfo);
                            _context.SaveChanges();
                        }

                        // アサイン
                        List<TrnReserveAssignInfo> assignList = SetAssignList(info, companyNo, reserveNo);
                        foreach (var assignInfo in assignList) {
                            _context.ReserveAssignInfo.Add(assignInfo);
                            _context.SaveChanges();
                        }
                    }

                    tran.Commit();

                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                    tran.Rollback();
                    result.ResultCode = (int)ImportResult.UnexpectedError;
                    return result;
                }
            }

            result.ResultCode = (int)ImportResult.Success;
            return result;
        }

        public async Task<ImportResultInfo> ImportCustomerData(List<ImportCustomerInfo> importCustomerList) {

            var companyNo = importCustomerList.First().CompanyNo;

            var result = new ImportResultInfo();

            // インポートリストの各項目が正しい形式かチェック
            var existsInvalidData = FoundInvalidCustomerData(importCustomerList, out result);
            if (existsInvalidData) {
                return result;
            }

            // トランザクション作成
            using (var tran = _context.Database.BeginTransaction()) {

                try {
                    // 予約番号採番用サービスのインスタンス生成
                    CustomerService customerService = new CustomerService(_context);

                    foreach (var info in importCustomerList) {

                        // 顧客番号 新規採番
                        var customerNo = customerService.Numbering(companyNo);

                        // 顧客マスタ
                        MstCustomerInfo customerInfo = SetCustomerInfo(info, companyNo, customerNo);

                        _context.CustomerInfo.Add(customerInfo);
                        _context.SaveChanges();
                    }

                    tran.Commit();

                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                    tran.Rollback();
                    result.ResultCode = (int)ImportResult.UnexpectedError;
                    return result;
                }
            }

            result.ResultCode = (int)ImportResult.Success;
            return result;
        }


        /// <summary>
        /// インポート予約情報の中に不正なデータがないか確認する
        /// </summary>
        /// <param name="importReserveList">インポート予約情報</param>
        /// <param name="result">不正なデータがあれば</param>
        /// <returns>True:不正データあり、False:不正データなし</returns>
        private bool FoundInvalidReserveData(List<ImportReserveInfo> importReserveList, out ImportResultInfo result) {
            
            var rowNo = 1;
            result = new ImportResultInfo();

            try {
                DateTime dt;
                string itemName = "";
                // エージェント確認用サービス
                AgentService agentService = new AgentService(_context);

                foreach (var info in importReserveList) {
                    result.RowNo = rowNo++;

                    #region 各項目チェック
                    // 到着日(必須・yyyyMMdd)
                    itemName = "到着日";
                    if (info.ArrivalDate.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        if (!DateTime.TryParseExact(info.ArrivalDate, CommonConst.DATE_FORMAT, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt)) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 泊数(必須・0～100)
                    itemName = "泊数";
                    if (info.StayDays.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        switch (CheckMinMaxValue(info.StayDays, 100, 0)) {
                            case 0:
                                // 問題なし
                                break;
                            case 1:
                                result.ResultCode = (int)ImportResult.MaxValueOver;
                                result.ItemName = itemName;
                                return true;
                            case 2:
                                result.ResultCode = (int)ImportResult.MinValueUnder;
                                result.ItemName = itemName;
                                return true;
                            case 3:
                                result.ResultCode = (int)ImportResult.InvalidInput;
                                result.ItemName = itemName;
                                return true;
                        }
                    }

                    // 出発日(必須・yyyyMMdd)
                    itemName = "出発日";
                    if (info.DepartureDate.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        if (!DateTime.TryParseExact(info.DepartureDate, CommonConst.DATE_FORMAT, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt)) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 予約日(必須・yyyyMMdd)
                    itemName = "予約日";
                    if (info.ReserveDate.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        if (!DateTime.TryParseExact(info.ReserveDate, CommonConst.DATE_FORMAT, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt)) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 大人男(必須・0～999)
                    itemName = "大人男";
                    if (info.MemberMale.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        switch (CheckMinMaxValue(info.MemberMale, 999, 0)) {
                            case 0:
                                // 問題なし
                                break;
                            case 1:
                                result.ResultCode = (int)ImportResult.MaxValueOver;
                                result.ItemName = itemName;
                                return true;
                            case 2:
                                result.ResultCode = (int)ImportResult.MinValueUnder;
                                result.ItemName = itemName;
                                return true;
                            case 3:
                                result.ResultCode = (int)ImportResult.InvalidInput;
                                result.ItemName = itemName;
                                return true;
                        }
                    }

                    // 大人女(必須・0～999)
                    itemName = "大人女";
                    if (info.MemberFemale.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        switch (CheckMinMaxValue(info.MemberFemale, 999, 0)) {
                            case 0:
                                // 問題なし
                                break;
                            case 1:
                                result.ResultCode = (int)ImportResult.MaxValueOver;
                                result.ItemName = itemName;
                                return true;
                            case 2:
                                result.ResultCode = (int)ImportResult.MinValueUnder;
                                result.ItemName = itemName;
                                return true;
                            case 3:
                                result.ResultCode = (int)ImportResult.InvalidInput;
                                result.ItemName = itemName;
                                return true;
                        }
                    }

                    // 子供A(必須・0～999)
                    itemName = "子供A";
                    if (info.MemberChildA.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        switch (CheckMinMaxValue(info.MemberChildA, 999, 0)) {
                            case 0:
                                // 問題なし
                                break;
                            case 1:
                                result.ResultCode = (int)ImportResult.MaxValueOver;
                                result.ItemName = itemName;
                                return true;
                            case 2:
                                result.ResultCode = (int)ImportResult.MinValueUnder;
                                result.ItemName = itemName;
                                return true;
                            case 3:
                                result.ResultCode = (int)ImportResult.InvalidInput;
                                result.ItemName = itemName;
                                return true;
                        }
                    }

                    // 子供B(必須・0～999)
                    itemName = "子供B";
                    if (info.MemberChildB.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        switch (CheckMinMaxValue(info.MemberChildB, 999, 0)) {
                            case 0:
                                // 問題なし
                                break;
                            case 1:
                                result.ResultCode = (int)ImportResult.MaxValueOver;
                                result.ItemName = itemName;
                                return true;
                            case 2:
                                result.ResultCode = (int)ImportResult.MinValueUnder;
                                result.ItemName = itemName;
                                return true;
                            case 3:
                                result.ResultCode = (int)ImportResult.InvalidInput;
                                result.ItemName = itemName;
                                return true;
                        }
                    }

                    // 子供C(必須・0～999)
                    itemName = "子供C";
                    if (info.MemberChildC.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        switch (CheckMinMaxValue(info.MemberChildC, 999, 0)) {
                            case 0:
                                // 問題なし
                                break;
                            case 1:
                                result.ResultCode = (int)ImportResult.MaxValueOver;
                                result.ItemName = itemName;
                                return true;
                            case 2:
                                result.ResultCode = (int)ImportResult.MinValueUnder;
                                result.ItemName = itemName;
                                return true;
                            case 3:
                                result.ResultCode = (int)ImportResult.InvalidInput;
                                result.ItemName = itemName;
                                return true;
                        }
                    }

                    // 取扱先(任意・マスタ存在チェック)
                    itemName = "取扱先";
                    if (info.AgentCode.IsNotBlanks()) {
                        MstAgentInfo agentInfo = _context.AgentInfo.AsNoTracking().Where(w => w.CompanyNo == info.CompanyNo && w.AgentCode == info.AgentCode && w.Status != CommonConst.STATUS_UNUSED).SingleOrDefault();
                        if (agentInfo == null) {
                            result.ResultCode = (int)ImportResult.MasterNotExists;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 取扱先備考(任意・桁数)
                    itemName = "取扱先備考";
                    if (info.AgentRemarks.IsNotBlanks() && info.AgentRemarks.Length > 100) {
                        result.ResultCode = (int)ImportResult.DigitOver;
                        result.ItemName = itemName;
                        return true;
                    }

                    // 電話番号(必須・入力制限・桁数)
                    itemName = "電話番号";
                    if (info.PhoneNo.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        bool isMached = Regex.IsMatch(info.PhoneNo, @"^[0-9-+ ]*$");
                        if (!isMached) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }

                        if (info.PhoneNo.Length > 20) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 携帯電話(任意・入力制限・桁数)
                    itemName = "携帯電話";
                    if (info.MobilePhoneNo.IsNotBlanks()) {
                        bool isMached = Regex.IsMatch(info.MobilePhoneNo, @"^[0-9-+ ]*$");
                        if (!isMached) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }

                        if (info.MobilePhoneNo.Length > 20) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 利用者名(必須・桁数)
                    itemName = "利用者名";
                    if (info.GuestName.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        if (info.GuestName.Length > 50) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // フリガナ(必須・入力制限・桁数)
                    itemName = "フリガナ";
                    if (info.GuestKana.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        bool isMached = Regex.IsMatch(info.GuestKana, @"^[0-9０-９a-zA-Zァ-ンヴー 　]*$");
                        if (!isMached) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }

                        if (info.GuestKana.Length > 50) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 会社名(任意・桁数)
                    itemName = "会社名";
                    if (info.CompanyName.IsNotBlanks() && info.CompanyName.Length > 50) {
                        result.ResultCode = (int)ImportResult.DigitOver;
                        result.ItemName = itemName;
                        return true;
                    }

                    // 郵便番号(任意・入力制限・桁数)
                    itemName = "郵便番号";
                    if (info.ZipCode.IsNotBlanks()) {
                        bool isMached = Regex.IsMatch(info.ZipCode, @"(\d{3})[-]?(\d{4})");
                        if (!isMached) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }

                        if (info.ZipCode.Length > 8) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // メール(任意・入力制限・桁数)
                    itemName = "メール";
                    if (info.Email.IsNotBlanks()) {
                        bool isMached = Regex.IsMatch(info.Email, @"^([a-zA-Z0-9_@\-\.])*$");
                        if (!isMached) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }

                        if (info.Email.Length > 60) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 住所(任意・桁数)
                    itemName = "住所";
                    if (info.Address.IsNotBlanks() && info.Address.Length > 100) {
                        result.ResultCode = (int)ImportResult.DigitOver;
                        result.ItemName = itemName;
                        return true;
                    }

                    // 顧客番号(任意・マスタ存在チェック)
                    itemName = "顧客番号";
                    if (info.CustomerNo.IsNotBlanks()) {
                        MstCustomerInfo customerInfo = _context.CustomerInfo.AsNoTracking().Where(w => w.CompanyNo == info.CompanyNo && w.CustomerNo == info.CustomerNo && w.Status != CommonConst.STATUS_UNUSED).SingleOrDefault();
                        if (customerInfo == null) {
                            result.ResultCode = (int)ImportResult.MasterNotExists;
                            result.ItemName = itemName;
                            return true;
                        }
                    }
                    #endregion
                }

                // 最後のデータまで不正データなし
                return false;

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                result.ResultCode = (int)ImportResult.UnexpectedError;
                return true;
            }
        }

        /// <summary>
        /// インポート顧客情報の中に不正なデータがないか確認する
        /// </summary>
        /// <param name="importReserveList">インポート予約情報</param>
        /// <param name="result">不正なデータがあれば</param>
        /// <returns>True:不正データあり、False:不正データなし</returns>
        private bool FoundInvalidCustomerData(List<ImportCustomerInfo> importCustomerList, out ImportResultInfo result) {

            var rowNo = 1;
            result = new ImportResultInfo();

            try {
                string itemName = "";

                foreach (var info in importCustomerList) {
                    result.RowNo = rowNo++;

                    #region 各項目チェック
                    // 顧客名(必須・桁数)
                    itemName = "顧客名";
                    if (info.CustomerName.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        if (info.CustomerName.Length > 100) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 顧客名カナ(必須・桁数)
                    itemName = "顧客名カナ";
                    if (info.CustomerKana.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        if (info.CustomerKana.Length > 100) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 郵便番号(任意・入力制限・桁数)
                    itemName = "郵便番号";
                    if (info.ZipCode.IsNotBlanks()) {
                        bool isMached = Regex.IsMatch(info.ZipCode, @"(\d{3})[-]?(\d{4})");
                        if (!isMached) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }

                        if (info.ZipCode.Length > 8) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 住所(任意・桁数)
                    itemName = "住所";
                    if (info.Address.IsNotBlanks() && info.Address.Length > 100) {
                        result.ResultCode = (int)ImportResult.DigitOver;
                        result.ItemName = itemName;
                        return true;
                    }

                    // 電話番号(必須・入力制限・桁数)
                    itemName = "電話番号";
                    if (info.PhoneNo.IsBlanks()) {
                        result.ResultCode = (int)ImportResult.Empty;
                        result.ItemName = itemName;
                        return true;
                    } else {
                        bool isMached = Regex.IsMatch(info.PhoneNo, @"^[0-9-+ ]*$");
                        if (!isMached) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }

                        if (info.PhoneNo.Length > 20) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 携帯電話番号(任意・入力制限・桁数)
                    itemName = "携帯電話番号";
                    if (info.MobilePhoneNo.IsNotBlanks()) {
                        bool isMached = Regex.IsMatch(info.MobilePhoneNo, @"^[0-9-+ ]*$");
                        if (!isMached) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }

                        if (info.MobilePhoneNo.Length > 20) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // メールアドレス(任意・入力制限・桁数)
                    itemName = "メールアドレス";
                    if (info.Email.IsNotBlanks()) {
                        bool isMached = Regex.IsMatch(info.Email, @"^([a-zA-Z0-9_@\-\.])*$");
                        if (!isMached) {
                            result.ResultCode = (int)ImportResult.InvalidInput;
                            result.ItemName = itemName;
                            return true;
                        }

                        if (info.Email.Length > 60) {
                            result.ResultCode = (int)ImportResult.DigitOver;
                            result.ItemName = itemName;
                            return true;
                        }
                    }

                    // 会社名(任意・桁数)
                    itemName = "会社名";
                    if (info.CompanyName.IsNotBlanks() && info.CompanyName.Length > 50) {
                        result.ResultCode = (int)ImportResult.DigitOver;
                        result.ItemName = itemName;
                        return true;
                    }

                    // 備考1
                    itemName = "備考1";
                    if (info.Remarks1.IsNotBlanks() && info.Remarks1.Length > 100) {
                        result.ResultCode = (int)ImportResult.DigitOver;
                        result.ItemName = itemName;
                        return true;
                    }

                    // 備考1
                    itemName = "備考2";
                    if (info.Remarks2.IsNotBlanks() && info.Remarks2.Length > 100) {
                        result.ResultCode = (int)ImportResult.DigitOver;
                        result.ItemName = itemName;
                        return true;
                    }

                    // 備考1
                    itemName = "備考3";
                    if (info.Remarks3.IsNotBlanks() && info.Remarks3.Length > 100) {
                        result.ResultCode = (int)ImportResult.DigitOver;
                        result.ItemName = itemName;
                        return true;
                    }

                    // 備考1
                    itemName = "備考4";
                    if (info.Remarks4.IsNotBlanks() && info.Remarks4.Length > 100) {
                        result.ResultCode = (int)ImportResult.DigitOver;
                        result.ItemName = itemName;
                        return true;
                    }

                    // 備考1
                    itemName = "備考5";
                    if (info.Remarks5.IsNotBlanks() && info.Remarks5.Length > 100) {
                        result.ResultCode = (int)ImportResult.DigitOver;
                        result.ItemName = itemName;
                        return true;
                    }
                    #endregion
                }

                // 最後のデータまで不正データなし
                return false;

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                result.ResultCode = (int)ImportResult.UnexpectedError;
                return true;
            }
        }
        /// <summary>
        /// 最小値～最大値の範囲内に収まっているかを確認
        /// </summary>
        /// <param name="item">チェック項目</param>
        /// <param name="maxVal">最大値</param>
        /// <param name="minVal">最小値</param>
        /// <returns>0:問題なし、1:最大値オーバー、2:最小値オーバー、3:不正な入力</returns>
        private int CheckMinMaxValue(string item, int maxVal, int minVal) {

            try {
                var checkItem = item.Replace(",", "");

                if (checkItem.IsNumeric()) {
                    if (checkItem.ToInt_Or_Zero() > maxVal) {
                        return 1;
                    } else if (checkItem.ToInt_Or_Zero() < minVal) {
                        return 2;
                    } else {
                        return 0;
                    }
                } else {
                    // 数値に変換できない文字の入力
                    return 3;
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// 登録用予約基本情報セット
        /// </summary>
        /// <param name="info">インポート予約情報</param>
        /// <param name="companyNo">会社番号</param>
        /// <param name="reserveNo">予約番号</param>
        /// <returns></returns>
        private TrnReserveBasicInfo SetReserveBasicInfo(ImportReserveInfo info, string companyNo, string reserveNo) {

            try {

                var retInfo = new TrnReserveBasicInfo();
                retInfo.CompanyNo = companyNo;
                retInfo.ReserveNo = reserveNo;
                retInfo.ReserveDate = info.ReserveDate;
                retInfo.ReserveStateDivision = "1";
                retInfo.ArrivalDate = info.ArrivalDate;
                retInfo.DepartureDate = info.DepartureDate;
                retInfo.StayDays = info.StayDays.ToInt_Or_Zero();
                retInfo.MemberMale = info.MemberMale.ToInt_Or_Zero();
                retInfo.MemberFemale = info.MemberFemale.ToInt_Or_Zero();
                retInfo.MemberChildA = info.MemberChildA.ToInt_Or_Zero();
                retInfo.MemberChildB = info.MemberChildB.ToInt_Or_Zero();
                retInfo.MemberChildC = info.MemberChildC.ToInt_Or_Zero();
                retInfo.AdjustmentFlag = CommonConst.NOT_ADJUSTMENTED;
                retInfo.CustomerNo = info.CustomerNo;
                retInfo.AgentCode = info.AgentCode;
                retInfo.AgentRemarks = info.AgentRemarks;
                retInfo.Status = CommonConst.STATUS_USED;
                retInfo.Version = 0;
                retInfo.Creator = info.Creator;
                retInfo.Updator = info.Updator;
                retInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                retInfo.Udt = retInfo.Cdt;

                return retInfo;

            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// 登録用氏名ファイル情報セット
        /// </summary>
        /// <param name="info">インポート予約情報</param>
        /// <param name="companyNo">会社番号</param>
        /// <param name="reserveNo">予約番号</param>
        /// <returns></returns>
        private TrnNameFileInfo SetNameFileInfo(ImportReserveInfo info, string companyNo, string reserveNo) {

            try {
                var retInfo = new TrnNameFileInfo();
                retInfo.CompanyNo = companyNo;
                retInfo.ReserveNo = reserveNo;
                retInfo.UseDate = CommonConst.USE_DATE_EMPTY;
                retInfo.RouteSEQ = CommonConst.DEFAULT_ROUTE_SEQ;
                retInfo.NameSeq = 1;
                retInfo.GuestName = info.GuestName;
                retInfo.GuestKana = info.GuestKana;
                retInfo.ZipCode = info.ZipCode;
                retInfo.Address = info.Address;
                retInfo.PhoneNo = info.PhoneNo;
                retInfo.MobilePhoneNo = info.MobilePhoneNo;
                retInfo.Email = info.Email;
                retInfo.CompanyName = info.CompanyName;
                retInfo.CustomerNo = info.CustomerNo;
                retInfo.Status = CommonConst.STATUS_USED;
                retInfo.Version = 0;
                retInfo.Creator = info.Creator;
                retInfo.Updator = info.Updator;
                retInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                retInfo.Udt = retInfo.Cdt;

                return retInfo;

            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// 登録用予約部屋タイプ情報セット
        /// </summary>
        /// <param name="info">インポート予約情報</param>
        /// <param name="companyNo">会社番号</param>
        /// <param name="reserveNo">予約番号</param>
        /// <returns></returns>
        private List<TrnReserveRoomtypeInfo> SetRoomTypeList(ImportReserveInfo info, string companyNo, string reserveNo) {

            try {
                List<TrnReserveRoomtypeInfo> retList = new List<TrnReserveRoomtypeInfo>();

                var stayDays = info.StayDays == "0" ? 1 : info.StayDays.ToInt_Or_Zero();
                var currentDate = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                DateTime arrivalDate;
                if (!DateTime.TryParseExact(info.ArrivalDate, CommonConst.DATE_FORMAT, CultureInfo.CurrentCulture, DateTimeStyles.None, out arrivalDate)) {
                    return retList;
                }

                for (var i = 0; i < stayDays; i++) {
                    var roomTypeInfo = new TrnReserveRoomtypeInfo() {
                        CompanyNo = companyNo,
                        ReserveNo = reserveNo,
                        UseDate = arrivalDate.AddDays(i).ToString(CommonConst.DATE_FORMAT),
                        RoomtypeCode = CommonConst.AUTO_CONVERT_MASTER_CODE,
                        RoomtypeSeq = 1,
                        Rooms = 1,
                        Status = CommonConst.STATUS_USED,
                        Version = 0,
                        Creator = info.Creator,
                        Updator = info.Updator,
                        Cdt = currentDate,
                        Udt = currentDate
                    };
                    retList.Add(roomTypeInfo);
                }

                return retList;

            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// 登録用アサイン情報セット
        /// </summary>
        /// <param name="info">インポート予約情報</param>
        /// <param name="companyNo">会社番号</param>
        /// <param name="reserveNo">予約番号</param>
        /// <returns></returns>
        private List<TrnReserveAssignInfo> SetAssignList(ImportReserveInfo info, string companyNo, string reserveNo) {

            try {
                List<TrnReserveAssignInfo> retList = new List<TrnReserveAssignInfo>();

                var stayDays = info.StayDays == "0" ? 1 : info.StayDays.ToInt_Or_Zero();
                var currentDate = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                DateTime arrivalDate;
                if (!DateTime.TryParseExact(info.ArrivalDate, CommonConst.DATE_FORMAT, CultureInfo.CurrentCulture, DateTimeStyles.None, out arrivalDate)) {
                    return retList;
                }

                for (var i = 0; i < stayDays; i++) {
                    var roomTypeInfo = new TrnReserveAssignInfo() {
                        CompanyNo = companyNo,
                        ReserveNo = reserveNo,
                        UseDate = arrivalDate.AddDays(i).ToString(CommonConst.DATE_FORMAT),
                        RoomNo = string.Empty,
                        RoomtypeCode = CommonConst.AUTO_CONVERT_MASTER_CODE,
                        OrgRoomtypeCode = CommonConst.AUTO_CONVERT_MASTER_CODE,
                        RoomtypeSeq = 1,
                        RoomStateClass = string.Empty,
                        GuestName = info.GuestName,
                        MemberMale = info.MemberMale.ToInt_Or_Zero(),
                        MemberFemale = info.MemberFemale.ToInt_Or_Zero(),
                        MemberChildA = info.MemberChildA.ToInt_Or_Zero(),
                        MemberChildB = info.MemberChildB.ToInt_Or_Zero(),
                        MemberChildC = info.MemberChildC.ToInt_Or_Zero(),
                        CleaningInstruction = string.Empty,
                        CleaningRemarks = string.Empty,
                        Email = info.Email,
                        HollowStateClass = string.Empty,
                        Status = CommonConst.STATUS_USED,
                        Version = 0,
                        Creator = info.Creator,
                        Updator = info.Updator,
                        Cdt = currentDate,
                        Udt = currentDate
                    };
                    retList.Add(roomTypeInfo);
                }

                return retList;

            } catch (Exception ex) {
                throw ex;
            }
        }
        
        /// <summary>
        /// 登録用顧客マスタ情報セット
        /// </summary>
        /// <param name="info">インポート顧客情報</param>
        /// <param name="companyNo">会社番号</param>
        /// <param name="customerNo">顧客番号</param>
        /// <returns></returns>
        private MstCustomerInfo SetCustomerInfo(ImportCustomerInfo info, string companyNo, string customerNo) {

            try {

                var retInfo = new MstCustomerInfo();
                retInfo.CompanyNo = companyNo;
                retInfo.CustomerNo = customerNo;
                retInfo.CustomerName = info.CustomerName;
                retInfo.CustomerKana = info.CustomerKana;
                retInfo.ZipCode = info.ZipCode;
                retInfo.Address = info.Address;
                retInfo.PhoneNo = info.PhoneNo;
                retInfo.MobilePhoneNo = info.MobilePhoneNo;
                retInfo.Email = info.Email;
                retInfo.CompanyName = info.CompanyName;
                retInfo.Remarks1 = info.Remarks1;
                retInfo.Remarks2 = info.Remarks2;
                retInfo.Remarks3 = info.Remarks3;
                retInfo.Remarks4 = info.Remarks4;
                retInfo.Remarks5 = info.Remarks5;
                retInfo.Status = CommonConst.STATUS_USED;
                retInfo.Version = 0;
                retInfo.Creator = info.Creator;
                retInfo.Updator = info.Updator;
                retInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                retInfo.Udt = retInfo.Cdt;

                return retInfo;

            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}