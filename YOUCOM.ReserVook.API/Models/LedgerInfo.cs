using System.Collections.Generic;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    /// <summary>台帳情報</summary>
    public class LedgerInfo : TrnReserveBasicInfo
    {
        /// <summary>利用日</summary>
        public string UseDate { get; set; }
        /// <summary>利用者名</summary>
        public string GuestName { get; set; }
        /// <summary>利用者名カナエ</summary>
        public string GuestKana { get; set; }
        /// <summary>金額</summary>
        public decimal AmountPrice { get; set; }
        /// <summary>内消費税額</summary>
        public decimal InsideTaxPrice { get; set; }
        /// <summary>アサイン部屋リスト</summary>
        public List<string> AssignRoomList { get; set; }

    }
}
