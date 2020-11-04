using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>客室情報</summary>
    public partial class TrnCleaningManagementReceiveInfo : BaseInfo
    {
        /// <summary>シーケンス</summary>
        public int Seq { get; set; }
        /// <summary></summary>
        public string MessageType { get; set; }
        /// <summary>部屋番号</summary>
        public string RoomNo { get; set; }
        /// <summary>清掃備考</summary>
        public string Contents { get; set; }
        /// <summary>清掃指示</summary>
        public string ProcessingFlag { get; set; }
    }

}
