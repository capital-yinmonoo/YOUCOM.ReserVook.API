using System.Collections.Generic;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    /// <summary>会場状況　検索条件</summary>
    public partial class FacilityCondition
    {
        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }

        /// <summary>利用日</summary>
        public string UseDate { get; set; }

    }

    /// <summary>予約会場情報</summary>
    public partial class ReserveFacilityInfo : BaseInfo
    {
        /// <summary>会場コード</summary>
        public string FacilityCode { get; set; }
        /// <summary>会場名</summary>
        public string FacilityName { get; set; }
        /// <summary>表示順</summary>
        public int DisplayOrder { get; set; }

        public List<TrnReserveFacilityInfo> ReserveFacilityList { get; set; }

    }

}
