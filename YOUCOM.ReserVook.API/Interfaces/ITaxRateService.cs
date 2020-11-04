using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ITaxRateService
    {

        //Task<MstTaxRateInfo> GetTaxRateByPK(MstTaxRateInfo taxServiceInfo);

        Task<List<MstTaxRateInfo>> GetList(BaseInfo cond);


    }
}
