using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ISetItemService
    {
        Task<List<MstItemInfo>> GetSetItemParentList(BaseInfo cond);

        Task<SetItemInfo> GetSetItemByPK(MstItemInfo cond);

        Task<CommonEnum.DBUpdateResult> AddSetItem(SetItemInfo cond);

        Task<CommonEnum.DBUpdateResult> UpdateSetItem(SetItemInfo cond);

        Task<CommonEnum.DBUpdateResult> DeleteSetItem(SetItemInfo cond);

    }
}
