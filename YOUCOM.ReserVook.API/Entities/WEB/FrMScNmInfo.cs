using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>ＳＣ名称マスタ</summary>
    public partial class FrMScNmInfo : WebBaseInfo
    {
        /// <summary>SC種別コード</summary>
        public string ScCategoryCd { get; set; }
        /// <summary>SC区分コード</summary>
        public string ScSegCd { get; set; }
        /// <summary>内容1</summary>
        public string Content1 { get; set; }
        /// <summary>内容2</summary>
        public string Content2 { get; set; }
        /// <summary>表示順</summary>
        public int DisplayOdr { get; set; }

        // 表示用
        /// <summary>部屋タイプ</summary>
        [NotMapped]
        public string Rmtype { get; set; }
    }
}
