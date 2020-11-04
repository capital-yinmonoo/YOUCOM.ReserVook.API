using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>売上明細情報</summary>
    public partial class TrnSalesDetailsInfo : BaseInfo
    {
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>SEQ</summary>
        public int DetailsSeq { get; set; }
        /// <summary>商品区分</summary>
        public string ItemDivision { get; set; }
        /// <summary>料理区分</summary>
        public string MealDivision { get; set; }
        /// <summary>利用日</summary>
        public string UseDate { get; set; }
        /// <summary>商品コード</summary>
        public string ItemCode { get; set; }
        /// <summary>印字用名称</summary>
        public string PrintName { get; set; }
        /// <summary>単価</summary>
        public decimal UnitPrice { get; set; }
        /// <summary>人数(男)</summary>
        public int ItemNumberM { get; set; }
        /// <summary>人数(女)</summary>
        public int ItemNumberF { get; set; }
        /// <summary>人数(子)</summary>
        public int ItemNumberC { get; set; }
        /// <summary>金額</summary>
        public decimal AmountPrice { get; set; }
        /// <summary>内消費税額</summary>
        public decimal InsideTaxPrice { get; set; }
        /// <summary>内サービス料額</summary>
        public decimal InsideServicePrice { get; set; }
        /// <summary>外サービス料額</summary>
        public decimal OutsideServicePrice { get; set; }
        /// <summary>消費税率</summary>
        public decimal TaxRate { get; set; }
        /// <summary>ビル分割SEQ</summary>
        public int BillSeparateSeq { get; set; }
        /// <summary>ビルNo</summary>
        public string BillNo { get; set; }
        /// <summary>税区分(0:税無, 1:税込)</summary>
        public string TaxDivision { get; set; }
        /// <summary>税率区分(1:標準税率, 2:軽減税率)</summary>
        public string TaxRateDivision { get; set; }
        /// <summary>サービス料区分(0:サ無, 1:サ込, 2:サ別)</summary>
        public string ServiceDivision { get; set; }
        /// <summary>セット商品区分(0:商品, 1:セット商品)</summary>
        public string SetItemDivision { get; set; }
        /// <summary>セット商品SEQ(セット商品の親と子紐付け用)</summary>
        public int SetItemSeq { get; set; }
        /// <summary>精算フラグ</summary>
        public string AdjustmentFlag { get; set; }

        // 更新対象チェック用フラグ

        /// <summary>追加フラグ</summary>
        [NotMapped]
        public bool AddFlag { get; set; }

        /// <summary>更新フラグ</summary>
        [NotMapped]
        public bool UpdateFlag { get; set; }

        /// <summary>削除フラグ</summary>
        [NotMapped]
        public bool DeleteFlag { get; set; }

        // 削除チェック用
        /// <summary>予約基本 予約状態</summary>
        [NotMapped]
        public string ReserveStateDivision { get; set; }
    }
}
