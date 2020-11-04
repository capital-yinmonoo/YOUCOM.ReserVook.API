namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>予約基本情報</summary>
    public class TrnReserveBasicInfo : BaseInfo
    {
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>予約受付日</summary>
        public string ReserveDate { get; set; }
        /// <summary>予約状態区分</summary>
        public string ReserveStateDivision { get; set; }
        /// <summary>到着日</summary>
        public string ArrivalDate { get; set; }
        /// <summary>出発日</summary>
        public string DepartureDate { get; set; }
        /// <summary>泊数</summary>
        public int StayDays { get; set; }
        /// <summary>男人数</summary>
        public int MemberMale { get; set; }
        /// <summary>女人数</summary>
        public int MemberFemale { get; set; }
        /// <summary>子供A人数</summary>
        public int MemberChildA { get; set; }
        /// <summary>子供B人数</summary>
        public int MemberChildB { get; set; }
        /// <summary>子供C人数</summary>
        public int MemberChildC { get; set; }
        /// <summary>精算フラグ</summary>
        public string AdjustmentFlag { get; set; }
        /// <summary>顧客番号</summary>
        public string CustomerNo { get; set; }
        /// <summary>エージェントコード</summary>
        public string AgentCode { get; set; }
        /// <summary>エージェント備考</summary>
        public string AgentRemarks { get; set; }
        /// <summary>サイト予約番号</summary>
        public string XTravelAgncBkngNum { get; set; }
    }
}
