namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>商品マスタ情報</summary>
    public partial class MstItemInfo : BaseInfo
    {
        /// <summary>商品コード</summary>
        public string ItemCode { get; set; }
        /// <summary>商品区分</summary>
        public string ItemDivision { get; set; }
        /// <summary>料理区分</summary>
        public string MealDivision { get; set; }
        /// <summary>名称(正式名称:30文字)</summary>
        public string ItemName { get; set; }
        /// <summary>名称(フリガナ:30文字)</summary>
        public string ItemKana { get; set; }
        /// <summary>名称(印刷用:20文字)</summary>
        public string PrintName { get; set; }
        /// <summary>単価</summary>
        public int UnitPrice { get; set; }
        /// <summary>税区分</summary>
        public string TaxDivision { get; set; }
        /// <summary>税率区分</summary>
        public string TaxrateDivision { get; set; }
        /// <summary>サービス料区分</summary>
        public string ServiceDivision { get; set; }
        /// <summary>表示順</summary>
        public int DisplayOrder { get; set; }
    }
}
