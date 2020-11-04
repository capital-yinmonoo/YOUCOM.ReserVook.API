using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IReserveService
    {


        Task<ReserveModel> GetReserveInfoByPK(ReserveModel cond);

        Task<ResultInfo> InsertReserveInfo(ReserveModel reserveInfo);

        Task<List<MstCodeNameInfo>> GetMasterRoomTypeList(BaseInfo cond);

        Task<List<MstAgentInfo>> GetMasterAgentList(BaseInfo cond);

        Task<List<MstItemInfo>> GetMasterItemList_StayItem(BaseInfo cond);

        Task<List<MstItemInfo>> GetMasterItemList_OtherItem(BaseInfo cond);

        Task<List<MstItemInfo>> GetMasterItemList_SetItem(BaseInfo cond);

        Task<List<MstSetItemInfo>> GetMasterSetItemList(BaseInfo cond);

        Task<List<MstDenominationInfo>> GetMasterDenominationCodeList(BaseInfo cond);

        Task<ResultInfo> UpdateReserveInfo(ReserveModel reserveInfo);

        Task<int> UpdatetReserveInfo_ReserveCancel(StayInfo cond);

        Task<int> UpdatetSalesDetailsInfo_AdjustmentFlag(SalesDetailsInfo cond);


    }
}
