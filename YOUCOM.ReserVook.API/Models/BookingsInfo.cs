using System.Collections.Generic;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    /// <summary>連泊状況　検索条件</summary>
    public partial class BookingsCondition
    {

        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }

        /// <summary>開始日</summary>
        public string StartDate { get; set; }

        /// <summary>終了日</summary>
        public string EndDate { get; set; }

        /// <summary>表示日数(7/14日)</summary>
        public int DisplayDays { get; set; }

    }

    /// <summary>アサイン情報</summary>
    public partial class BookingsInfo : BaseInfo
    {
        /// <summary>部屋番号</summary>
        public string RoomNo { get; set; }
        /// <summary>部屋名称</summary>
        public string RoomName { get; set; }
        /// <summary>部屋タイプ</summary>
        public string RoomType { get; set; }
        /// <summary>部屋タイプ名</summary>
        public string RoomTypeName { get; set; }
        /// <summary>表示順</summary>
        public int DisplayOrder { get; set; }

        public List<BookingsAssignInfo> AssignList { get; set; }

    }

    /// <summary>アサイン情報</summary>
    public partial class BookingsAssignInfo : BaseInfo
    {
        /// <summary>利用日</summary>
        public string UseDate { get; set; }

        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }

        ///// <summary>部屋状態</summary>
        //public string RoomStateClass { get; set; }

        ///// <summary>利用者名</summary>
        //public string GuestName { get; set; }
    }
}
