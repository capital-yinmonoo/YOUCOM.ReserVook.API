using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>ＳＣ備考変換マスタ</summary>
    public partial class FrMScRemarksConvertInfo : WebBaseInfo
    {
        /// <summary>SCXML項目名</summary>
        public string ScXClmn { get; set; }
        /// <summary>SCXML項目名漢字</summary>
        public string ScXClmnKanji { get; set; }
        /// <summary>SC備考設定位置</summary>
        public int ScRemarksSetLocation { get; set; }
        /// <summary>SC備考優先順</summary>
        public int ScRemarksPriorityOdr { get; set; }

        // 表示用
        /// <summary>サイトコード</summary>
        [NotMapped]
        public string SiteCdName { get; set; }
    }
}
