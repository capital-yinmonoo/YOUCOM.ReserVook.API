using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YOUCOM.ReserVook.API.Entities {

    /// <summary>顧客マスタ情報</summary>
    public partial class MstCustomerInfo : BaseInfo {
    
        /// <summary>顧客番号</summary>
        public string CustomerNo { get; set; }

        /// <summary>顧客名</summary>
        public string CustomerName { get; set; }

        /// <summary>顧客名カナ</summary>
        public string CustomerKana { get; set; }

        /// <summary>郵便番号</summary>
        public string ZipCode { get; set; }

        /// <summary>住所</summary>
        public string Address { get; set; }

        /// <summary>電話番号</summary>
        public string PhoneNo { get; set; }

        /// <summary>携帯電話番号</summary>
        public string MobilePhoneNo { get; set; }

        /// <summary>メールアドレス</summary>
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
