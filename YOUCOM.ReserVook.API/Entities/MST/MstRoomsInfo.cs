using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>部屋マスタ情報</summary>
    public partial class MstRoomsInfo : BaseInfo
    {
        /// <summary>部屋番号</summary>
        public string RoomNo { get; set; }
        /// <summary>部屋名称</summary>
        public string RoomName { get; set; }
        /// <summary>部屋タイプコード</summary>
        public string RoomTypeCode { get; set; }
        /// <summary>フロア</summary>
        public string Floor { get; set; }
        /// <summary>部屋備考</summary>
        public string Remarks { get; set; }
        /// <summary>禁煙喫煙区分</summary>
        public string SmokingDivision { get; set; }
        /// <summary>部屋割画面行インデックス</summary>
        public int? RowIndex { get; set; }
        /// <summary>部屋割画面列インデックス</summary>
        public int? ColumnIndex { get; set; }


        /// 表示用
        /// <summary>予約番号</summary>
        [NotMapped]
        public string ReserveNo { get; set; }
        /// <summary>男人数</summary>
        [NotMapped]
        public int MemberMale { get; set; }
        /// <summary>女人数</summary>
        [NotMapped]
        public int MemberFemale { get; set; }
        /// <summary>子供A人数</summary>
        [NotMapped]
        public int MemberChildA { get; set; }
        /// <summary>子供B人数</summary>
        [NotMapped]
        public int MemberChildB { get; set; }
        /// <summary>子供C人数</summary>
        [NotMapped]
        public int MemberChildC { get; set; }
        /// <summary>清掃備考</summary>
        [NotMapped]
        public string CleaningRemarks { get; set; }
        /// <summary>清掃指示</summary>
        [NotMapped]
        public string CleaningInstruction { get; set; }
        /// <summary>部屋状態</summary>
        [NotMapped]
        public string RoomStateClass { get; set; }
        /// <summary>ルートSEQ</summary>
        [NotMapped]
        public int RouteSeq { get; set; }
        /// <summary>日付</summary>
        [NotMapped]
        public string Today { get; set; }
        /// <summary>変更前部屋状態</summary>
        [NotMapped]
        public string OrgRoomStateClass { get; set; }
    }
}