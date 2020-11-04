using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    /// <summary>連泊状況　検索条件</summary>
    public class NameSearchCondition
    {
        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }
        /// <summary>利用者名</summary>
        public string Name { get; set; }
        /// <summary>電話番号</summary>
        public string Phone { get; set; }
        /// <summary>利用開始日</summary>
        public string UseDateFrom { get; set; }
        /// <summary>利用終了日</summary>
        public string UseDateTo { get; set; }
        /// <summary>キーワード（スペース区切り）</summary>
        public string Keyword { get; set; }

        /// <summary>予約 利用者情報のみ</summary>
        public bool  ReserveOnly { get; set; }

    }

    /// <summary>連泊状況　検索結果</summary>
    public class NameSearchInfo
    {
        /// <summary>部屋番号</summary>
        public string RoomNo { get; set; }
        /// <summary>利用者名漢字</summary>
        public string NameKanji{ get; set; }
        /// <summary>利用者名カナ</summary>
        public string NameKana { get; set; }
        /// <summary>電話番号</summary>
        public string Phone { get; set; }
        /// <summary>到着日</summary>
        public string ArrivalDate { get; set; }
        /// <summary>出発日</summary>
        public string DepartureDate { get; set; }
        /// <summary>泊数</summary>
        public int StayDays { get; set; }
        /// <summary>部屋数</summary>
        public int Rooms { get; set; }
        /// <summary>人数</summary>
        public int Persons { get; set; }
        /// <summary>状態</summary>
        public string Status { get; set; }
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }
    }
}
