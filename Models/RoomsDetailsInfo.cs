using System.Collections.Generic;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    /// <summary>部屋割詳細 更新用</summary>
    public partial class UpdateRoomDetails
    {
        
        /// <summary>アサイン情報</summary>
        public List<TrnReserveAssignInfo> assignList { get; set; }

        /// <summary>氏名ファイル情報</summary>
        public List<TrnNameFileInfo> nameFileList { get; set; }

    }

}
