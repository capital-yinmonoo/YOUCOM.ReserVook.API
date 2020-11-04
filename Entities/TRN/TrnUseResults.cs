using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities {

    /// <summary>利用実績情報</summary>
    public partial class TrnUseResultsInfo : BaseInfo {

        /// <summary>顧客番号</summary>
        public string CustomerNo { get; set; }
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>到着日</summary>
        public string ArrivalDate { get; set; }
        /// <summary>出発日</summary>
        public string DepartureDate { get; set; }
        /// <summary>利用金額</summary>
        public decimal UseAmount { get; set; }

        /// <summary>(表示用)到着日</summary>
        [NotMapped]
        public string DisplayArrivalDate { get; set; }

        /// <summary>(表示用)出発日</summary>
        [NotMapped]
        public string DisplayDepartureDate { get; set; }
    }
}
