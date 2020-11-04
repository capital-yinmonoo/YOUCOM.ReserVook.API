namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>税率マスタ情報</summary>
    public partial class MstTaxRateInfo : BaseInfo
    {
        /// <summary>税率</summary>
        public decimal TaxRate { get; set; }
        /// <summary>開始日</summary>
        public string BeginDate { get; set; }
        /// <summary>終了日</summary>
        public string EndDate { get; set; }
        /// <summary>税率区分</summary>
        public string TaxrateDivision { get; set; }
    }
}