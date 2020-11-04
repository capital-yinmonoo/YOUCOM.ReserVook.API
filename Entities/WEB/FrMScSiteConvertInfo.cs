using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>ＳＣサイト変換マスタ</summary>
    public partial class FrMScSiteConvertInfo : WebBaseInfo
    {
        /// <summary>SCサイトコード</summary>
        public string ScSiteCd { get; set; }
        /// <summary>SCサイト名称</summary>
        public string ScSiteNm { get; set; }
        /// <summary>エージェントコード</summary>
        public string TravelAgncCd { get; set; }
        /// <summary>SC大人男性位置</summary>
        public string ScPositionMan { get; set; }
        /// <summary>SC大人女性位置</summary>
        public string ScPositionWoman { get; set; }
        /// <summary>SC子供A位置</summary>
        public string ScPositionChildA { get; set; }
        /// <summary>SC子供B位置</summary>
        public string ScPositionChildB { get; set; }
        /// <summary>SC子供C位置</summary>
        public string ScPositionChildC { get; set; }
        /// <summary>SC子供D位置</summary>
        public string ScPositionChildD { get; set; }
        /// <summary>SC子供E位置</summary>
        public string ScPositionChildE { get; set; }
        /// <summary>SC子供F位置</summary>
        public string ScPositionChildF { get; set; }
        /// <summary>SC子供その他位置</summary>
        public string ScPositionChildOther { get; set; }
        /// <summary>SC人数計算区分(0:部屋数で割らない, 1:部屋数で割る)</summary>
        public string ScPersonCalcSeg { get; set; }

        // 表示用
        /// <summary>エージェント名</summary>
        [NotMapped]
        public string TravelAgncName { get; set; }
    }
}
