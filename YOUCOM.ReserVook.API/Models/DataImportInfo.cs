using System.Collections.Generic;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models {

    public enum ImportResult {
        /// <summary>成功</summary>
        Success,
        /// <summary>必須項目が空</summary>
        Empty,
        /// <summary>不正な形式の入力</summary>
        InvalidInput,
        /// <summary>桁数オーバー</summary>
        DigitOver,
        /// <summary>最大値以上の値</summary>
        MaxValueOver,
        /// <summary>最小値未満の値</summary>
        MinValueUnder,
        /// <summary>マスタ未登録</summary>
        MasterNotExists,
        /// <summary>想定外のエラー</summary>
        UnexpectedError = -1
    }


    /// <summary>
    /// 予約情報(CSVの生データのため全てString)
    /// </summary>
    public partial class ImportReserveInfo : BaseInfo {

        // 宿泊者情報
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>到着日</summary>
        public string ArrivalDate { get; set; }
        /// <summary>泊数</summary>
        public string StayDays { get; set; }
        /// <summary>出発日</summary>
        public string DepartureDate { get; set; }
        /// <summary>予約受付日</summary>
        public string ReserveDate { get; set; }
        /// <summary>男人数</summary>
        public string MemberMale { get; set; }
        /// <summary>女人数</summary>
        public string MemberFemale { get; set; }
        /// <summary>子供A人数</summary>
        public string MemberChildA { get; set; }
        /// <summary>子供B人数</summary>
        public string MemberChildB { get; set; }
        /// <summary>子供C人数</summary>
        public string MemberChildC { get; set; }

        // エージェント情報
        /// <summary>エージェントコード</summary>
        public string AgentCode { get; set; }
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
    }


    /// <summary>顧客情報</summary>
    public partial class ImportCustomerInfo : BaseInfo {

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

    /// <summary>インポート結果情報</summary>
    public partial class ImportResultInfo {

        /// <summary>インポート結果コード</summary>
        public int ResultCode { get; set; }
        /// <summary>エラー行番号</summary>
        public int RowNo { get; set; }
        /// <summary>エラー項目名</summary>
        public string ItemName { get; set; }
    }
}
