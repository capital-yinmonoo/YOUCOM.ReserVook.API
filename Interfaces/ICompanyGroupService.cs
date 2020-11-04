using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ICompanyGroupService
    {
        Task<List<MstCompanyGroupInfo>> GetCompanyGroupList();

        Task<MstCompanyGroupInfo> GetCompanyGroupByPK(string key);

        Task<CommonEnum.DBUpdateResult> AddCompanyGroup(MstCompanyGroupInfo info);

        Task<CommonEnum.DBUpdateResult> UpdateCompanyGroup(MstCompanyGroupInfo info);

        Task<CommonEnum.DBUpdateResult> DeleteCompanyGroup(MstCompanyGroupInfo info);

        Task<CommonEnum.DBUpdateResult> CheckBeforeDelete(MstCompanyGroupInfo info);
    }
}
