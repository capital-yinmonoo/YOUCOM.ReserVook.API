using System.Collections.Generic;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>忘れ物一覧</summary>
    public partial class TrnLostItemsPictureInfo : BaseInfo
    {
        /// <summary>管理番号</summary>
        public string ManagementNo { get; set; }
        /// <summary>ファイルシーケンス</summary>
        public int FileSeq { get; set; }
        /// <summary>ファイル種別</summary>
        public string ContentType { get; set; }
        /// <summary>ファイル名</summary>
        public string FileName { get; set; }
        /// <summary>写真バイナリ</summary>
        public byte[] BinaryData { get; set; }

    }
}
