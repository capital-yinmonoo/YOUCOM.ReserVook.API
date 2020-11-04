using System.Collections.Generic;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Models
{
    /// <summary>セット商品情報</summary>
    public class SetItemInfo : MstItemInfo
    {
        /// <summary>セット商品(子)リスト</summary>
        public List<MstSetItemInfo> ChildItems { get; set; }
    }
}
