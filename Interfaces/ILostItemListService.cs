using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ILostItemListService
    {
        // 情報取得(画面表示,削除用)
        Task<List<TrnLostItemsBaseInfo>> GetLostItemList(TrnLostItemsBaseInfo lostItemListInfo);

        // 情報取得(編集用)
        Task<TrnLostItemsBaseInfo> GetLostItemListById(TrnLostItemsBaseInfo lostItemListInfo);

        // イメージ画像取得(編集用)
        Task<List<TrnLostItemsPictureInfo>> GetLostItemImage(TrnLostItemsBaseInfo lostItemListInfo);

        // 現在使用容量取得
        Task<long> GetUsingCapacity(TrnLostItemsBaseInfo lostItemListInfo);

        // 情報追加
        Task<int> AddLostItem(TrnLostItemsBaseInfo lostItemListInfo);

        // 情報更新
        Task<int> UpdateLostItem(TrnLostItemsBaseInfo lostItemListInfo, bool addFlag);

        // イメージ画像登録
        Task<int> AddLostItemPicture(List<TrnLostItemsPictureInfo> lostItemListInfo);

        // イメージ画像最大容量チェック
        Task<int> IsOverMaxCapacity(List<TrnLostItemsPictureInfo> lostItemListInfo);

        // 情報削除
        Task<int> DelLostItem(TrnLostItemsBaseInfo lostItemListInfo);

        // 一括削除
        Task<int> LumpDelLostItem(TrnLostItemsBaseInfo lostItemListInfo);
    }
}
