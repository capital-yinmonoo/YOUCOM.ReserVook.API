namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>コード名称マスタ情報</summary>
    public partial class MstCodeNameInfo : BaseInfo
    {
        /// <summary>分類コード</summary>
        public string DivisionCode { get; set; }
        /// <summary>コード</summary>
        public string Code { get; set; }
        /// <summary>名称</summary>
        public string CodeName { get; set; }
        /// <summary>値</summary>
        public string CodeValue { get; set; }
        /// <summary>表示順</summary>
        public int DisplayOrder { get; set; }
    }
}
