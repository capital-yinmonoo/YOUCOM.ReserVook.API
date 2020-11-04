using System.ComponentModel.DataAnnotations.Schema;

namespace YOUCOM.ReserVook.API.Entities
{
    /// <summary>忘れ物管理基本</summary>
    public partial class TrnLostItemsBaseInfo : BaseInfo
    {
        /// <summary>管理番号</summary>
        public string ManagementNo { get; set; }

        /// <summary>忘れ物状態</summary>
        public string ItemState { get; set; }

        /// <summary>カテゴリー</summary>
        public string ItemCategory { get; set; }

        /// <summary>物品名</summary>
        public string ItemName { get; set; }

        /// <summary>発見日付</summary>
        public string FoundDate { get; set; }

        /// <summary>発見時刻</summary>
        public string FoundTime { get; set; }

        /// <summary>発見場所</summary>
        public string FoundPlace { get; set; }

        /// <summary>コメント</summary>
        public string Comment { get; set; }

        /// <summary>検索用ワード</summary>
        public string SearchWord { get; set; }

        /// <summary>忘れ物発見場所分類</summary>
        public string FoundPlaceCode { get; set; }

        /// <summary>忘れ物保管分類</summary>
        public string StorageCode { get; set; }
        
        /// <summary>部屋番号</summary>
        public string RoomNo { get; set; }
        /// <summary>予約番号</summary>
        public string ReserveNo { get; set; }

        // 表示用
        /// <summary>状態名</summary>
        [NotMapped]
        public string StateName { get; set; }
        /// <summary>発見場所分類名</summary>
        [NotMapped]
        public string FoundPlaceName { get; set; }
        /// <summary>保管分類名</summary>
        [NotMapped]
        public string StorageName { get; set; }
        /// <summary>イメージ画像</summary>
        [NotMapped]
        public byte[] ImageData { get; set; }
        /// <summary>画像ファイル型</summary>
        [NotMapped]
        public string ImageContentType { get; set; }

        /// <summary>更新日前</summary>
        [NotMapped]
        public string UdtBefore { get; set; }
        /// <summary>更新日後</summary>
        [NotMapped]
        public string UdtAfter { get; set; }

    }
}
