namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>会場マスタ情報</summary>
    public class MstFacilityInfo : BaseInfo
    {
        /// <summary>エージェントコード</summary>
        public string FacilityCode { get; set; }
        /// <summary>名称</summary>
        public string FacilityName { get; set; }
        /// <summary>表示順</summary>
        public int DisplayOrder { get; set; }

    }

}
