namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>コード分類マスタ情報</summary>
    public partial class MstCodeDivisionInfo : BaseInfo
    {
        /// <summary>分類コード</summary>
        public int DivisionCode { get; set; }
        /// <summary>分類名</summary>
        public string DivisionName { get; set; }
    }
}
