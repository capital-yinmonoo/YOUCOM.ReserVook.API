using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>セット商品マスタ情報</summary>
    public partial class MstSetItemInfo : BaseInfo
    {
        /// <summary>セット商品コード</summary>
        public string SetItemCode { get; set; }
        /// <summary>SEQ</summary>
        public int Seq { get; set; }
        /// <summary>商品コード</summary>
        public string ItemCode { get; set; }
        /// <summary>単価</summary>
        public int UnitPrice { get; set; }
        /// <summary>税率区分</summary>
        public string TaxrateDivision { get; set; }
        /// <summary>サービス料区分</summary>
        public string ServiceDivision { get; set; }
        /// <summary>表示順</summary>
        public int DisplayOrder { get; set; }

        [NotMapped]
        /// <summary>商品マスタ情報</summary>
        public MstItemInfo BaseItemInfo { get; set; }
    }
}
