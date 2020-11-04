using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IDishReportService
    {
        Task<List<DishInfo>> GetList(DishReportCondition cond);
    }
}
