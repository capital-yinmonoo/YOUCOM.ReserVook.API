using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IFacilityService
    {
        Task<int> MasterDelete(MstFacilityInfo info);

        Task<int> MasterUpdate(MstFacilityInfo info, bool addFlag);

        Task<int> MasterCheckDelete(MstFacilityInfo info);

        Task<MstFacilityInfo> MasterGetByPK(MstFacilityInfo info);

        Task<List<MstFacilityInfo>> MasterGetList(MstFacilityInfo info);

        Task<int> MasterAdd(MstFacilityInfo info);

        Task<TrnReserveFacilityInfo> GetByPK(TrnReserveFacilityInfo info);

        Task<List<TrnReserveFacilityInfo>> GetReserveFacilityList(FacilityCondition cond);

        Task<int> Update(TrnReserveFacilityInfo info);

        Task<int> Add(TrnReserveFacilityInfo info);

        Task<int> Delete(TrnReserveFacilityInfo info);

    }
}
