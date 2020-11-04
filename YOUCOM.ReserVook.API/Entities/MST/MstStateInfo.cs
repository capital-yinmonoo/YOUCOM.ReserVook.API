using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>忘れ物状態設定</summary>
    public partial class MstStateInfo : BaseInfo
    {
        /// <summary>状態コード</summary>
        public string ItemStateCode { get; set; }

        /// <summary>状態名</summary>
        public string ItemStateName { get; set; }
        /// <summary>設定色</summary>
        public string Color { get; set; }
        /// <summary>検索初期設定</summary>
        public string DefaultFlagSearch { get; set; }
        /// <summary>登録初期設定</summary>
        public string DefaultFlagEntry { get; set; }
        /// <summary>表示順</summary>
        public int OrderNo { get; set; }

        
    }
}
