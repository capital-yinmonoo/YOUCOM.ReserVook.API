using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IBillService
    {
        Task<IList<string>> GetSeparateBillNoList(ReserveModel cond);

        Task<bool> CheckBillNo(ReserveModel cond);

        byte[] GetCompanyLogo(BaseInfo cond);
    }
}
