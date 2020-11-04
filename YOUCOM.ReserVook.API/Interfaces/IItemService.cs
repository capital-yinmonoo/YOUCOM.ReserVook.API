using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IItemService
    {

        Task<int> DeleteItem(MstItemInfo itemInfo);

        Task<int> UpdateItem(MstItemInfo itemInfo);

        Task<int> CheckDelete(MstItemInfo itemInfo);

        Task<int> CheckDelete(MstItemInfo itemInfo, bool skipSetItemDivCheck);

        Task<MstItemInfo> GetItemByPK(MstItemInfo itemInfo);

        Task<ItemInfoView> GetItemByPKView(MstItemInfo itemInfo);

        Task<List<MstItemInfo>> GetList(MstItemInfo itemInfo);

        Task<List<ItemInfoView>> GetListView(MstItemInfo itemInfo);

        Task<int> AddItem(MstItemInfo itemInfo);

    }
}
