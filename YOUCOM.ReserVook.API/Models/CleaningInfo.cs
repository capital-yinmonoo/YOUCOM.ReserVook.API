namespace YOUCOM.ReserVook.API.Models
{

    public partial class CleaningCondition
    {
        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }

        /// <summary>利用日</summary>
        public string UseDate { get; set; }

        /// <summary>フロア</summary>
        public string Floor { get; set; }
    }

    public partial class CleaningInfo
    {
        public string Floor { get; set; }

        public string RoomNo { get; set; }

        public string RoomType { get; set; }

        public string RoomStatus { get; set; }

        public string Smoking { get; set; }

        public string SmokingName { get; set; }

        public string Nights { get; set; }

        //public string DepatureTime { get; set; }

        //public string ArrivalTime { get; set; }

        public int? Man { get; set; }

        public int? Woman { get; set; }

        public int? ChildA { get; set; }

        public int? ChildB { get; set; }

        public int? ChildC { get; set; }

        public int? MemberTotal { get; set; }

        public string RoomStateDiv { get; set; }

        public string RoomChangeKey { get; set; }

        public string RoomChangeState { get; set; }

        public int RoomStatusValue { get; set; }

        public string CleaningInstruction { get; set; }
        public string CleaningRemarks { get; set; }
        public string UseDate { get; set; }
    }

    public partial class RoomChangeRoomInfo
    {
        public string BeforeRoomNo { get; set; }
        public string AfterRoomNo { get; set; }
    }


}
