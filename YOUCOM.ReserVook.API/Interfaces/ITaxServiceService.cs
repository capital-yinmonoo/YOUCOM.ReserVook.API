using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ITaxServiceService
    {

        Task<MstTaxServiceInfo> GetTaxServiceByPK(MstTaxServiceInfo taxServiceInfo);

        Task<List<MstTaxServiceInfo>> GetList(MstTaxServiceInfo taxServiceInfo);

        Task<List<TaxServiceDivisionView>> GetListView(MstTaxServiceInfo taxServiceInfo);

    }
}
