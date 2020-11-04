using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ICompanyService
    {
        Task<int> DeleteCompany(MstCompanyInfo companyInfo);

        Task<int> UpdateCompany(MstCompanyInfo companyInfo);

        Task<int> UpdateImage(IFormFileCollection files, string companyNo);

        Task<int> AddCompany(MstCompanyInfo companyInfo);

        Task<MstCompanyInfo> GetCompanyByPK(string companyNo);

        Task<List<MstCompanyInfo>> GetList();

        Task<List<MstCompanyInfo>> GetCompanyListByCompanyGroupId(string companyGroupId);

        Task<decimal> GetServiceRate(BaseInfo cond);

    }
}
