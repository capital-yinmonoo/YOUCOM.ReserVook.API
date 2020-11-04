using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ISalesReportService
    {
        Task<List<SalesReportInfo>> GetList(SalesReportCondition cond);
    }
}
