using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    public partial class RoomTypeInfo : BaseInfo
    {
        public string UseDate { get; set; }
        public string RoomType { get; set; }
        public int Rooms { get; set; }
        public int RouteSEQ { get; set; }
        public int RoomtypeSeq { get; set; }


        // 更新用フラグ

        /// <summary>追加フラグ</summary>
        public bool AddFlag { get; set; }

        /// <summary>更新フラグ</summary>
        public bool UpdateFlag { get; set; }

        /// <summary>削除フラグ</summary>
        public bool DeleteFlag { get; set; }
    }
}
