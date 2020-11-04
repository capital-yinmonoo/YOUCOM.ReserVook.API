using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Controllers;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces {
    public interface IUseResultsService {

        // 利用実績の情報取得
        Task<List<TrnUseResultsInfo>> GetUseResultsList(TrnUseResultsInfo useResultsInfo);

        // 利用実績情報追加
        Task<int> AddUseResults(TrnUseResultsInfo useResultsInfo);

        // 利用実績情報の削除
        Task<int> DelUseResults(TrnUseResultsInfo useResultsInfo);
    }
}
