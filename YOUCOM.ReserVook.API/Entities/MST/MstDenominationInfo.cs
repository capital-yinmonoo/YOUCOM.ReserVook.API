namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>金種マスタ情報</summary>
    public partial class MstDenominationInfo : BaseInfo
    {
        /// <summary>金種コード</summary>
        public string DenominationCode { get; set; }
        /// <summary>名称(正式名称:30文字)</summary>
        public string DenominationName { get; set; }
        /// <summary>名称(印刷用:20文字)</summary>
        public string PrintName { get; set; }
        /// <summary>領収区分(0:領収金額に含める, 1:領収金額に含めない)/summary>
        public string ReceiptDiv { get; set; }
        /// <summary>表示順</summary>
        public int DisplayOrder { get; set; }

    }

}
