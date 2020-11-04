using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>ＳＣ部屋タイプ変換マスタ</summary>
    public partial class FrMScRmtypeConvertInfo : WebBaseInfo
    {
        /// <summary>SC部屋タイプコード</summary>
        public string ScRmtypeCd { get; set; }
        /// <summary>部屋タイプコード</summary>
        public string RmtypeCd { get; set; }

        // 表示用
        /// <summary>部屋タイプ</summary>
        [NotMapped]
        public string Rmtype { get; set; }
    }
}
