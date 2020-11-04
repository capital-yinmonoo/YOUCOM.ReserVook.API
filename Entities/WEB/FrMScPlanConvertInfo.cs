using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>ＳＣプラン変換マスタ</summary>
    public partial class FrMScPlanConvertInfo : WebBaseInfo
    {
        /// <summary>ＳＣ企画パッケージコード</summary>
        public string ScPackagePlanCd { get; set; }
        /// <summary>ＳＣ食事条件</summary>
        public string ScMealCond { get; set; }
        /// <summary>ＳＣ食事有無情報</summary>
        public string ScSpecMealCond { get; set; }

        /// <summary>商品コード</summary>
        public string ItemCode { get; set; }

        // 表示用
        /// <summary>商品名</summary>
        [NotMapped]
        public string ItemName { get; set; }
    }
}
