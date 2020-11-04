using System.Collections.Generic;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models {

    /// <summary>予約情報抽出条件</summary>
    public partial class ExportReserveCondition {

        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }
        /// <summary>到着日(開始)</summary>
        public string ArrivalDateFrom { get; set; }
        /// <summary>到着日(終了)</summary>
        public string ArrivalDateTo { get; set; }
        /// <summary>出発日(開始)</summary>
        public string DepartureDateFrom { get; set; }
        /// <summary>出発日(終了)</summary>
        public string DepartureDateTo { get; set; }
    }

    /// <summary>予約情報</summary>
    public partial class ExportReserveInfo {

        // 宿泊者情報
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>到着日</summary>
        public string ArrivalDate { get; set; }
        /// <summary>泊数</summary>
        public int StayDays { get; set; }
        /// <summary>出発日</summary>
        public string DepartureDate { get; set; }
        /// <summary>予約受付日</summary>
        public string ReserveDate { get; set; }
        /// <summary>男人数</summary>
        public int MemberMale { get; set; }
        /// <summary>女人数</summary>
        public int MemberFemale { get; set; }
        /// <summary>子供A人数</summary>
        public int MemberChildA { get; set; }
        /// <summary>子供B人数</summary>
        public int MemberChildB { get; set; }
        /// <summary>子供C人数</summary>
        public int MemberChildC { get; set; }

        // エージェント情報
        /// <summary>エージェントコード</summary>
        public string AgentCode { get; set; }
        /// <summary>エージェント名</summary>
        public string AgentName { get; set; }
        /// <summary>エージェント備考</summary>
        public string AgentRemarks { get; set; }

        // 利用者情報
        /// <summary>電話番号</summary>
        public string PhoneNo { get; set; }
        /// <summary>携帯番号</summary>
        public string MobilePhoneNo { get; set; }
        /// <summary>利用者名</summary>
        public string GuestName { get; set; }
        /// <summary>利用者カナ</summary>
        public string GuestKana { get; set; }
        /// <summary>会社名</summary>
        public string CompanyName { get; set; }
        /// <summary>郵便番号</summary>
        public string ZipCode { get; set; }
        /// <summary>E-mail</summary>
        public string Email { get; set; }
        /// <summary>住所</summary>
        public string Address { get; set; }
        /// <summary>顧客番号</summary>
        public string CustomerNo { get; set; }

        // 商品情報の合計金額
        /// <summary>金額</summary>
        public decimal UseAmountTotal { get; set; }
    }

    /// <summary>顧客情報抽出条件</summary>
    public partial class ExportCustomerCondition {

        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }
        /// <summary>フリガナ</summary>
        public string GuestKana { get; set; }
        /// <summary>電話番号</summary>
        public string PhoneNo { get; set; }
        /// <summary>利用日(開始)</summary>
        public string UseDateFrom { get; set; }
        /// <summary>利用日(終了)</summary>
        public string UseDateTo { get; set; }
        /// <summary>利用金額(下限)</summary>
        public string UseAmountMin { get; set; }
        /// <summary>利用金額(上限)</summary>
        public string UseAmountMax { get; set; }
    }

    /// <summary>顧客情報</summary>
    public partial class ExportCustomerInfo {

        // 顧客情報
        /// <summary>顧客番号</summary>
        public string CustomerNo { get; set; }
        /// <summary>利用者名</summary>
        public string CustomerName { get; set; }
        /// <summary>利用者カナ</summary>
        public string CustomerKana { get; set; }
        /// <summary>郵便番号</summary>
        public string ZipCode { get; set; }
        /// <summary>住所</summary>
        public string Address { get; set; }
        /// <summary>電話番号</summary>
        public string PhoneNo { get; set; }
        /// <summary>携帯番号</summary>
        public string MobilePhoneNo { get; set; }
        /// <summary>E-mail</summary>
        public string Email { get; set; }
        /// <summary>会社名</summary>
        public string CompanyName { get; set; }
        /// <summary>備考1</summary>
        public string Remarks1 { get; set; }
        /// <summary>備考2</summary>
        public string Remarks2 { get; set; }
        /// <summary>備考3</summary>
        public string Remarks3 { get; set; }
        /// <summary>備考4</summary>
        public string Remarks4 { get; set; }
        /// <summary>備考5</summary>
        public string Remarks5 { get; set; }
    }
}
