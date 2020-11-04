using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>予約変更履歴情報</summary>
    public partial class TrnReserveLogInfo : BaseInfo
    {
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>ログSEQ</summary>
        public int ReserveLogSeq { get; set; }
        /// <summary>SEQグループ(同時刻に作成のもので採番)</summary>
        public int SeqGroup { get; set; }
        /// <summary>処理区分(1:新規作成,2:更新,3:削除)</summary>
        public string ProcessDivision { get; set; }
        /// <summary>変更項目</summary>
        public string ChangeItem { get; set; }
        /// <summary>変更前値</summary>
        public string BeforeValue { get; set; }
        /// <summary>変更後値</summary>
        public string AfterValue { get; set; }

    }

}
