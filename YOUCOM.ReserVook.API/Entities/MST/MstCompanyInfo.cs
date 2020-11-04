using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>会社マスタ情報</summary>
    public partial class MstCompanyInfo : BaseInfo
    {
        /// <summary>会社名</summary>
        public string CompanyName { get; set; }
        /// <summary>郵便番号</summary>
        public string ZipCode { get; set; }
        /// <summary>住所</summary>
        public string Address { get; set; }
        /// <summary>電話番号</summary>
        public string PhoneNo { get; set; }
        /// <summary>請求先</summary>
        public string BillingAddress { get; set; }
        /// <summary>ロゴデータ(バイナリデータ)</summary>
        public byte[] LogoData { get; set; }
        /// <summary>ファイル種別</summary>
        public string ContentType { get; set; }
        /// <summary>サービス料率</summary>
        public decimal ServiceRate { get; set; }
        /// <summary>最終予約番号</summary>
        public string LastReserveNo { get; set; }
        /// <summary>最終顧客番号</summary>
        public string LastCustomerNo { get; set; }
        /// <summary>最終ビル番号</summary>
        public uint LastBillNo { get; set; }


        /// <summary>忘れ物管理・清掃管理使用区分</summary>
        public string LostFlg { get; set; }

        /// <summary>保存期間(年)</summary>
        public uint SavePeriod { get; set; }

        /// <summary>TrustYou連携区分(0:未使用、1:使用)</summary>
        public string TrustyouConnectDiv { get; set; }
        
        /// <summary>保存画像最大容量</summary>
        public int MaxCapacity { get; set; }


        /// <summary>会社グループID</summary>
        public string CompanyGroupId { get; set; }

        // 表示用
        /// <summary>清掃/忘れ物管理使用許可フラグ</summary>
        [NotMapped]
        public string LostFlgName { get; set; }

        /// <summary>TrustYou連携区分(0:未使用、1:使用)</summary>
        [NotMapped]
        public string TrustyouConnectDivName { get; set; }

        /// <summary>会社グループ名</summary>
        [NotMapped]
        public string CompanyGroupName { get; set; }
    }
}
