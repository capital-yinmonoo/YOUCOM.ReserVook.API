using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    /// <summary>売上情報</summary>
    public partial class SalesReportInfo : BaseKey
    {
        /// <summary>利用日</summary>
        public string UseDate { get; set; }
        /// <summary>商品区分</summary>
        public string ItemDivisionName { get; set; }
        /// <summary>商品コード</summary>
        public string ItemCode { get; set; }
        /// <summary>印字用名称</summary>
        public string PrintName { get; set; }
        /// <summary>マスタ基本単価</summary>
        public decimal UnitPrice { get; set; }
        /// <summary>数量</summary>
        public int ItemNumber { get; set; }
        /// <summary>ネット金額</summary>
        public decimal NetAmount { get; set; }
        /// <summary>内消費税</summary>
        public decimal InsideTaxPrice { get; set; }
        /// <summary>内サービス料</summary>
        public decimal InsideServicePrice { get; set; }
        /// <summary>外サービス料</summary>
        public decimal OutsideServicePrice { get; set; }
        /// <summary>金額</summary>
        public decimal AmountPrice { get; set; }
        /// <summary>入金科目フラグ</summary>
        public bool DepositFlag { get; set; }
    }

    /// <summary>売上情報 条件</summary>
    public partial class SalesReportCondition : BaseKey
    {
        /// <summary>利用日</summary>
        public string UseDate { get; set; }
        /// <summary>月報</summary>
        public bool MonthlyFlag { get; set; }
    }
}
