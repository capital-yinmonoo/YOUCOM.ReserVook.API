using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>入金明細情報</summary>
    public partial class TrnDepositDetailsInfo : BaseInfo
    {
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>ビル分割SEQ</summary>
        public int BillSeparateSeq { get; set; }
        /// <summary>ビルNo</summary>
        public string BillNo { get; set; }
        /// <summary>SEQ</summary>
        public int DetailsSeq { get; set; }
        /// <summary>入金日</summary>
        public string DepositDate { get; set; }
        /// <summary>金種コード</summary>
        public string DenominationCode { get; set; }
        /// <summary>印字用名称</summary>
        public string PrintName { get; set; }
        /// <summary>金額</summary>
        public decimal AmountPrice { get; set; }
        /// <summary>備考</summary>
        public string Remarks { get; set; }
        /// <summary>精算フラグ</summary>
        public string AdjustmentFlag { get; set; }

        // 更新チェック用
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
