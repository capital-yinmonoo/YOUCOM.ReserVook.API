using YOUCOM.ReserVook.API.Entities;
using static YOUCOM.ReserVook.API.Context.CommonEnum;

namespace YOUCOM.ReserVook.API.Models
{
    /// <summary>アサイン　検索条件</summary>
    public partial class RoomsAssignCondition
    {
        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }

        /// <summary>日付</summary>
        public string UseDate { get; set; }

        /// <summary>CO予定を表示</summary>
        public bool ViewCOFlg { get; set; }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public RoomsAssignCondition Clone()
        {
            return (RoomsAssignCondition)MemberwiseClone();
        }
    }

    /// <summary>アサイン情報</summary>
    public partial class RoomsAssignedInfo : TrnReserveAssignInfo
    {
        /// <summary>部屋割画面行インデックス</summary>
        public int? RowIndex { get; set; }
        /// <summary>部屋割画面列インデックス</summary>
        public int? ColumnIndex { get; set; }
        /// <summary>部屋名称</summary>
        public string RoomName { get; set; }
        /// <summary>部屋タイプ名</summary>
        public string RoomTypeName { get; set; }
        /// <summary>部屋タイプ区分</summary>
        public string RoomTypeDivision { get; set; }
        /// <summary>禁煙喫煙区分</summary>
        public SmokingDivision SmokingDivision { get; set; }
        /// <summary>到着日</summary>
        public string ArrivalDate { get; set; }
        /// <summary>出発日</summary>
        public string DepartureDate { get; set; }
        /// <summary>泊数</summary>
        public int StayDays { get; set; }
        /// <summary>大人人数</summary>
        public int MemberAdult { get; set; }
        /// <summary>子供人数</summary>
        public int MemberChild { get; set; }

        /// <summary>清掃状態(2020.07.21 Add)</summary>
        public string CleaningInstruction { get; set; }

        /// <summary>清掃備考(2020.07.21 Add)</summary>
        public string CleaningRemarks { get; set; }

        /// <summary>チェックイン日</summary>
        public bool CheckInDay { get; set; }

        /// <summary>チェックアウト日</summary>
        public bool CheckOutDay { get; set; }

        public RoomsAssignedInfo()
        {
            this.ReserveNo = string.Empty;
            this.UseDate = string.Empty;
            this.RoomNo = null;
            this.RoomtypeCode = null;
            this.OrgRoomtypeCode = null;
            this.RouteSEQ = 0;
            this.RoomStateClass = null;
            this.GuestName = string.Empty;
            this.MemberMale = 0;
            this.MemberFemale = 0;
            this.MemberChildA = 0;
            this.MemberChildB = 0;
            this.MemberChildC = 0;

            this.RoomName = string.Empty;
            this.RoomTypeName = string.Empty;
            this.RoomTypeDivision = string.Empty;
            this.SmokingDivision = SmokingDivision.NoSmoking;
            this.ArrivalDate = string.Empty;
            this.DepartureDate = string.Empty;
            this.StayDays = 0;
            this.MemberAdult = 0;
            this.MemberChild = 0;

            // 2020.07.21 Add
            this.CleaningInstruction = string.Empty;
            this.CleaningRemarks = string.Empty;

            this.CheckInDay = false;
            this.CheckOutDay = false;

        }

    }

    /// <summary>未アサイン情報</summary>
    public partial class NotAssignInfo : TrnReserveAssignInfo
    {
        /// <summary>部屋タイプ名</summary>
        public string RoomTypeName { get; set; }
        /// <summary>到着日</summary>
        public string ArrivalDate { get; set; }
        /// <summary>出発日</summary>
        public string DepartureDate { get; set; }
        /// <summary>泊数</summary>
        public int StayDays { get; set; }
        /// <summary>大人人数</summary>
        public int MemberAdult { get; set; }
        /// <summary>子供人数</summary>
        public int MemberChild { get; set; }
    }

    /// <summary>ルームチェンジ更新結果</summary>
    public partial class RoomChangeResult
    {
        /// <summary>更新結果</summary>
        public DBUpdateResult Result { get; set; }
        /// <summary>移動元部屋番号</summary>
        public string BaseRoomNo { get; set; }
        /// <summary>移動先部屋番号</summary>
        public string TargetRoomNo { get; set; }
        /// <summary>相互ルームチェンジ</summary>
        public bool IsMutualChange { get; set; }

    }

}
