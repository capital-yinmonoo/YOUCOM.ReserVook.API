using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>ＳＣ支払変換マスタ</summary>
    public partial class FrMScPaymentConvertInfo : WebBaseInfo
    {
        /// <summary>SCサイトコード</summary>
        public string ScSiteCd { get; set; }
        /// <summary>SC決済方法</summary>
        public string ScPaymentOpts { get; set; }
        /// <summary>金種コード</summary>
        public string DenominationCode { get; set; }

        // 表示用
        /// <summary>サイトコード</summary>
        [NotMapped]
        public string SiteCdName { get; set; }
        /// <summary>決済方法</summary>
        [NotMapped]
        public string ScPaymentOptsName { get; set; }
        /// <summary>金種コード</summary>
        [NotMapped]
        public string Denomination { get; set; }
    }
}
