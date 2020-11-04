using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces {

    public interface ITrustyouService {

        #region TrustYouデータ一覧取得
        Task<List<TrustyouInfo>> GetTrustyouList(TrustyouCondition cond);
        #endregion

        #region TrustYou連携データ送信
        Task<List<TrnTrustyouLogInfo>> SendRecvTrustyouData(TrustyouSendRcvCondition sendRecvCond);
        #endregion

        #region TrustYouログデータ取得
        Task<List<TrnTrustyouLogInfo>> GetTrustyouLogList(TrustyouLogCondition cond);
        #endregion

        #region TrustYouデータ一時保存
        Task<int> SaveTemporarilyData(List<TrustyouInfo> tempList);
        #endregion
    }
}
