using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface ILedgerService
    {
        Task<List<LedgerInfo>> GetList(LedgerInfo cond);
    }
}
