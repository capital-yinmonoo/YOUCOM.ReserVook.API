using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>ＳＣポイント変換マスタ</summary>
    public partial class FrMScPointConvertInfo : WebBaseInfo
    {
        /// <summary>SCサイトコード</summary>
        public string ScSiteCd { get; set; }
        /// <summary>SCポイント・補助金名称</summary>
        public string ScPntsDiscntNm { get; set; }
        /// <summary>金種コード</summary>
        public string DenominationCode { get; set; }

        // 表示用
        /// <summary>サイトコード</summary>
        [NotMapped]
        public string SiteCdName { get; set; }
        /// <summary>ポイント割引・補助金名</summary>
        [NotMapped]
        public string PntsDiscntName { get; set; }
        /// <summary>金種コード</summary>
        [NotMapped]
        public string Denomination { get; set; }
    }
}
