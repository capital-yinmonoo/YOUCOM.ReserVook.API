using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    public class ItemInfoView : MstItemInfo
    {
        /// <summary>税サ区分</summary>
        public string TaxServiceDivision { get; set; }
        /// <summary>税サ区分名称</summary>
        public string TaxServiceDivisionName { get; set; }
        /// <summary>商品分類名</summary>
        public string ItemDivisionName { get; set; }
        /// <summary>料理区分名</summary>
        public string MealDivisionName { get; set; }
    }
}
