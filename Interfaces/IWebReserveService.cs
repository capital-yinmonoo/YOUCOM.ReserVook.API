using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IWebReserveService
    {
        Task<List<WebReserveBaseInfo>> GetWebReserveBaseList(WebReserveBaseInfo cond);

        Task<WebReserveInfo> GetWebReserveById(WebReserveBaseInfo cond);

    }
}
