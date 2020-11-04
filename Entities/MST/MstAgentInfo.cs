namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>エージェントマスタ情報</summary>
    public class MstAgentInfo : BaseInfo
    {
        /// <summary>エージェントコード</summary>
        public string AgentCode { get; set; }
        /// <summary>名称</summary>
        public string AgentName { get; set; }
        /// <summary>備考</summary>
        public string Remarks { get; set; }
        /// <summary>表示順</summary>
        public int DisplayOrder { get; set; }

    }

}
