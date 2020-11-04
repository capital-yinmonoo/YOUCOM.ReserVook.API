using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>予約アサイン情報</summary>
    public partial class TrnReserveAssignInfo : BaseInfo
    {
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
        /// <summary>利用日</summary>
        public string UseDate { get; set; }
        /// <summary>ルートSEQ</summary>
        public int RouteSEQ { get; set; }
        /// <summary>部屋番号</summary>
        public string RoomNo { get; set; }
        /// <summary>部屋タイプコード</summary>
        public string RoomtypeCode { get; set; }
        /// <summary>元部屋タイプコード</summary>
        public string OrgRoomtypeCode { get; set; }
        /// <summary>部屋タイプSEQ</summary>
        public int RoomtypeSeq { get; set; }
        /// <summary>部屋状態</summary>
        public string RoomStateClass { get; set; }
        /// <summary>利用者名</summary>
        public string GuestName { get; set; }
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

        /// <summary>E-mail</summary>
        public string Email { get; set; }

        /// <summary>清掃指示</summary>
        public string CleaningInstruction { get; set; }

        /// <summary>清掃備考</summary>
        public string CleaningRemarks { get; set; }

        /// <summary>中抜け状態</summary>
        public string HollowStateClass { get; set; }

        /// <summary>予約番号(中抜け)</summary>
        [NotMapped]
        public string HideReserveNo { get; set; }

        /// <summary>中抜け状態(中抜け)</summary>
        [NotMapped]
        public string HideHollowStateClass { get; set; }

        /// <summary>客室状態更新フラグ</summary>
        [NotMapped]
        public bool IsStatusUpdateData { get; set; }

        /// <summary>更新前部屋状態</summary>
        [NotMapped]
        public int RoomStatusValue { get; set; }

        // 更新チェック用
        /// <summary>追加フラグ</summary>
        [NotMapped]
        public bool AddFlag { get; set; }

        /// <summary>更新フラグ</summary>
        [NotMapped]
        public bool UpdateFlag { get; set; }

        /// <summary>削除フラグ</summary>
        [NotMapped]
        public bool DeleteFlag { get; set; }

        // 表示用
        /// <summary>部屋タイプ名</summary>
        [NotMapped]
        public string RoomtypeName { get; set; }

        /// <summary>合計人数</summary>
        [NotMapped]
        public int MemberTotal { get; set; }

        /// <summary>氏名ファイル</summary>
        [NotMapped]
        public GuestInfo nameFileInfo { get; set; }


        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public TrnReserveAssignInfo Clone()
        {
            return (TrnReserveAssignInfo)MemberwiseClone();
        }
    }
}
