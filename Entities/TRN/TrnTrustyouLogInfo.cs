using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YOUCOM.ReserVook.API.Entities {

    /// <summary>TrustYouログ情報</summary>
    public partial class TrnTrustyouLogInfo : BaseInfo {

        /// <summary>ログSEQ</summary>
        public int LogSeq { get; set; }

        /// <summary>処理日付</summary>
        public string ProcessDate { get; set; }

        /// <summary>処理時刻</summary>
        public string ProcessTime { get; set; }

        /// <summary>実行者名</summary>
        public string ProcessUser { get; set; }

        /// <summary>ログメッセージ</summary>
        public string LogMessage { get; set; }

        /// <summary>エラーコード</summary>
        public string ErrorCode { get; set; }

        /// <summary>(表示用)処理日付</summary>
        [NotMapped]
        public string DisplayProcessDate { get; set; }

        /// <summary>(表示用)処理時刻</summary>
        [NotMapped]
        public string DisplayProcessTime { get; set; }

    }
}
