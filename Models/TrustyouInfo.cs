using System.Collections.Generic;

namespace YOUCOM.ReserVook.API.Models
{

    public partial class TrustyouCondition {

        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }

        /// <summary>利用日(開始)</summary>
        public string UseDateFrom { get; set; }

        /// <summary>利用日(終了)</summary>
        public string UseDateTo { get; set; }
    }

    public partial class TrustyouInfo {

        /// <summary>SEQ</summary>
        public int Seq { get; set; }

        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }

        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }

        /// <summary>部屋番号</summary>
        public string RoomNo { get; set; }

        /// <summary>到着日</summary>
        public string ArrivalDate { get; set; }

        /// <summary>出発日</summary>
        public string DepartureDate { get; set; }

        /// <summary>宿泊者名</summary>
        public string OutGuestName { get; set; }

        /// <summary>E-Mail</summary>
        public string OutEmail { get; set; }

        /// <summary>宿泊者名(送信用)</summary>
        public string SendGuestName { get; set; }

        /// <summary>E-Mail(送信用)</summary>
        public string SendEmail { get; set; }

        /// <summary>言語コード(現状、'ja'固定)</summary>
        public string LanguageCode { get; set; }

        /// <summary>送信日付</summary>
        public string SendDate { get; set; }

        /// <summary>送信時刻</summary>
        public string SendTime { get; set; }

        /// <summary>送信結果</summary>
        public string SendResult { get; set; }

        /// <summary>状態</summary>
        public string Status { get; set; }

        /// <summary>バージョン</summary>
        public int? Version { get; set; }

        /// <summary>作成者</summary>
        public string Creator { get; set; }

        /// <summary>更新者</summary>
        public string Updator { get; set; }

        /// <summary>作成日時</summary>
        public string Cdt { get; set; }

        /// <summary>更新日時</summary>
        public string Udt { get; set; }

        /// <summary>(表示用)状態</summary>
        public string DisplayStatus { get; set; }

        /// <summary>(表示用)到着日</summary>
        public string DisplayArrivalDate { get; set; }

        /// <summary>(表示用)出発日</summary>
        public string DisplayDepartureDate { get; set; }

        /// <summary>(表示用)送信日時</summary>
        public string SendDateTime { get; set; }

        /// <summary>(表示用)送信結果</summary>
        public string DisplaySendResult { get; set; }

        /// <summary>送信担当者</summary>
        public string ProcessUser { get; set; }

        /// <summary>Trustyou連携データ.会社番号</summary>
        public string TCompanyNo { get; set; }

        /// <summary>紐づくTrustyou連携データが存在する場合True</summary>
        public bool HasTempData { get; set; }
    }


    public partial class TrustyouSendRcvCondition {

        /// <summary>送信データリスト</summary>
        public List<TrustyouInfo> SendDataList { get; set; }

        /// <summary>キャンセル送信フラグ</summary>
        public bool IsCanceled { get; set; }
    }

    public partial class TrustyouLogCondition {

        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }

        /// <summary>処理日</summary>
        public string ProcessDate { get; set; }
    }

}
