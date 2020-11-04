namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>税サ区分マスタ情報</summary>
    public partial class MstTaxServiceInfo : BaseInfo
    {
        /// <summary>名称</summary>
        public string DisplayName { get; set; }
        /// <summary>税区分</summary>
        public string TaxDivision { get; set; }
        /// <summary>サービス料区分</summary>
        public string ServiceDivision { get; set; }
    }
}