using System.Collections.Generic;

namespace YOUCOM.ReserVook.API.Models
{
    /// <summary>料理日報 検索条件</summary>
    public partial class DishReportCondition
    {
        /// <summary>会社番号</summary>
        public string CompanyNo { get; set; }

        /// <summary>利用日(開始)</summary>
        public string FromUseDate { get; set; }

        /// <summary>利用日(終了)</summary>
        public string ToUseDate { get; set; }
    }

    /// <summary>料理日報情報</summary>
    public partial class DishInfo
    {
        /// <summary>料理区分</summary>
        public string MealDivision { get; set; }
        /// <summary>料理区分名</summary>
        public string MealDivisionName { get; set; }
        /// <summary>商品コード</summary>
        public string ItemCode { get; set; }
        /// <summary>料理名</summary>
        public string MealName { get; set; }
        /// <summary>合計料理数</summary>
        public int SumMealNumber { get; set; }
        /// <summary>料理日報日別情報</summary>
        public List<DishDayInfo> DishDayInfo { get; set; }
        /// <summary>料理区分-並び順</summary>
        public int MealDisplayOrder { get; set; }
        /// <summary>商品-並び順</summary>
        public int ItemDisplayOrder { get; set; }

    }

    /// <summary>料理日報日別情報</summary>
    public partial class DishDayInfo
    {
        /// <summary>利用日</summary>
        public string UseDate { get; set; }
        /// <summary>料理数</summary>
        public int MealNumber { get; set; }

    }




}
