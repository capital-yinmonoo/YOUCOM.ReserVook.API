using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IAgentService
    {


        // エージェント情報の取得(画面表示,削除用)
        Task<List<MstAgentInfo>> GetAgentList(MstAgentInfo agentInfo);

        // エージェント情報の取得(編集用)
        Task<MstAgentInfo> GetAgentById(MstAgentInfo agentInfo);

        // エージェント情報の追加
        Task<int> AddAgent(MstAgentInfo agentInfo);

        // エージェント情報の更新
        Task<int> UpdateAgent(MstAgentInfo agentInfo, bool addFlag);

        // エージェント情報の取得(削除用)
        Task<MstAgentInfo> GetAgentInfo(MstAgentInfo agentInfo);

        // エージェント情報の削除
        Task<int> DelAgent(MstAgentInfo agentInfo);

        // エージェント情報チェック
        Task<int> DeleteAgentCheck(MstAgentInfo agentInfo);

        // エージェント取得(サイト変換マスタ用)
        Task<List<MstAgentInfo>> GetAgentDataDid(string companyNo);
    }
}
