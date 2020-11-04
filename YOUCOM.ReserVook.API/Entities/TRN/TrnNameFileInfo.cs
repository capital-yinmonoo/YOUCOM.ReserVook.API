using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>氏名ファイル情報</summary>
    public partial class TrnNameFileInfo : BaseInfo
    {
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>SEQ</summary>
        public int NameSeq { get; set; }
        /// <summary>利用者名</summary>
        public string GuestName { get; set; }
        /// <summary>利用者カナ</summary>
        public string GuestKana { get; set; }
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
        /// <summary>顧客番号</summary>
        public string CustomerNo { get; set; }

        /// <summary>利用日</summary>
        public string UseDate { get; set; }
        /// <summary>ルートSEQ</summary>
        public int RouteSEQ { get; set; }

        #region -- 部屋割詳細 変換用 --
        /// <summary>利用者カナ</summary>
        [NotMapped]
        public string GuestNameKana { get; set; }
        /// <summary>電話番号</summary>
        [NotMapped]
        public string Phone { get; set; }

        /// <summary>携帯番号</summary>
        [NotMapped]
        public string Cellphone { get; set; }
        #endregion

    }

}
