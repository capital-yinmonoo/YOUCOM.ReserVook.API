using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>ユーザマスタ情報</summary>
    public partial class MstUserInfo : BaseInfo
    {
        /// <summary>メールアドレス</summary>
        public string UserEmail { get; set; }

        [JsonIgnore]
        /// <summary>パスワード</summary>
        public string Password { get; set; }
        /// <summary>ユーザー名</summary>
        public string UserName { get; set; }
        /// <summary>権限区分</summary>
        public string RoleDivision { get; set; }
        /// <summary>清掃/忘れ物管理使用許可フラグ</summary>
        public string LostFlg { get; set; }

        // 表示用
        /// <summary>権限</summary>
        [NotMapped]
        public string RoleName { get; set; }

        /// <summary>会社名</summary>
        [NotMapped]
        public string CompanyName { get; set; }

        /// <summary>メールアドレス(未変更)</summary>
        [NotMapped]
        public string UserEmailOrigin { get; set; }

        /// <summary>清掃/忘れ物管理使用許可フラグ</summary>
        [NotMapped]
        public string LostFlgName { get; set; }

        /// <summary>会社マスタ情報</summary>
        [NotMapped]
        public MstCompanyInfo CompanyInfo { get; set; }

        /// <summary>画面会社番号</summary>
        [NotMapped]
        public string DisplayCompanyNo { get; set; }
        
    }
}
