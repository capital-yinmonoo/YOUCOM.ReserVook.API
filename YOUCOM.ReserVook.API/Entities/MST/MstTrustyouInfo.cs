using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YOUCOM.ReserVook.API.Entities {

    /// <summary>TrustYou連携マスタ情報</summary>
    public partial class MstTrustyouInfo : BaseInfo {

        /// <summary>ホテルコード</summary>
        public string HotelCode { get; set; }

        /// <summary>連携データ送信先URL</summary>
        public string SendUrl { get; set; }

        /// <summary>ログインユーザー名</summary>
        public string UserName { get; set; }

        /// <summary>ログインパスワード</summary>
        public string Password { get; set; }

    }
}
