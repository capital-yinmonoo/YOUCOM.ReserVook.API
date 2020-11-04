using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>予約部屋タイプ情報</summary>
    public partial class TrnReserveRoomtypeInfo : BaseInfo
    {
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>利用日</summary>
        public string UseDate { get; set; }
        /// <summary>部屋タイプコード</summary>
        public string RoomtypeCode { get; set; }
        /// <summary>部屋数</summary>
        public int Rooms { get; set; }
        /// <summary>部屋タイプSEQ</summary>
        public int RoomtypeSeq { get; set; }

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
    }
}
