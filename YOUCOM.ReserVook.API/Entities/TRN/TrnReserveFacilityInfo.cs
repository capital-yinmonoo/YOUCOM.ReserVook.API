using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>予約会場情報</summary>
    public partial class TrnReserveFacilityInfo : BaseInfo
    {

        ///<Summry>会場コード</Summry>
        public string FacilityCode { get; set; }
        ///<Summry>利用日</Summry>
        public string UseDate { get; set; }
        ///<Summry>会場SEQ</Summry>
        public int FacilitySeq { get; set; }
        ///<Summry>開始時刻</Summry>
        public string StartTime { get; set; }
        ///<Summry>終了時刻</Summry>
        public string EndTime { get; set; }
        ///<Summry>会場人数</Summry>
        public int FacilityMember { get; set; }
        ///<Summry>会場備考</Summry>
        public string FacilityRemarks { get; set; }
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }

        // 表示用
        ///<Summry>会場名</Summry>
        [NotMapped]
        public string FacilityName { get; set; }

        // 表示用
        ///<Summry>予約者名</Summry>
        [NotMapped]
        public string GuestName { get; set; }

        // For Verison Check
        ///<Summry>元会場コード</Summry>
        [NotMapped]
        public string OrgFacilityCode { get; set; }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public TrnReserveFacilityInfo Clone()
        {
            return (TrnReserveFacilityInfo)MemberwiseClone();
        }

    }

}
