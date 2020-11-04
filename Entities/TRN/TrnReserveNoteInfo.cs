using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>予約備考情報</summary>
    public partial class TrnReserveNoteInfo : BaseInfo
    {
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>SEQ</summary>
        public int NoteSeq { get; set; }
        /// <summary>備考</summary>
        public string Remarks { get; set; }

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
    }

}
