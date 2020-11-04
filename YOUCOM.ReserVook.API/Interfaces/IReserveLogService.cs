using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IReserveLogService
    {
        Task<List<TrnReserveLogInfo>> MakeReserveLog(CommonEnum.ReserveLogProcessDivision processDivision, ReserveInfo afterInfo);

        Task<List<TrnReserveLogInfo>> MakeReserveLog(CommonEnum.ReserveLogProcessDivision processDivision);

        Task<List<TrnReserveLogInfo>> GetReserveLogList(TrnReserveLogInfo cond);
        
        Task<int> NumberingLogSeq(string companyNo, string reserveNo);

        Task<int> NumberingSeqGroup(string companyNo, string reserveNo);
    }
}
