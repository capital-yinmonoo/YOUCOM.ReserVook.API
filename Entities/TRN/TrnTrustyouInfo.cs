using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YOUCOM.ReserVook.API.Entities {

    /// <summary>TrustYou連携データ情報</summary>
    public partial class TrnTrustyouInfo : BaseInfo {

        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }

        /// <summary>到着日</summary>
        public string ArrivalDate { get; set; }

        /// <summary>出発日</summary>
        public string DepartureDate { get; set; }

        /// <summary>部屋番号</summary>
        public string RoomNo { get; set; }

        /// <summary>宿泊者名</summary>
        public string GuestName { get; set; }

        /// <summary>Email</summary>
        public string Email { get; set; }

        /// <summary>言語コード(現状'ja'固定)</summary>
        public string LanguageCode { get; set; }

        /// <summary>送信日付</summary>
        public string SendDate { get; set; }

        /// <summary>送信時間</summary>
        public string SendTime { get; set; }

        /// <summary>送信結果</summary>
        public string SendResult { get; set; }

        /// <summary>新規フラグ</summary>
        [NotMapped]
        public bool IsNewData { get; set; }

    }
}
