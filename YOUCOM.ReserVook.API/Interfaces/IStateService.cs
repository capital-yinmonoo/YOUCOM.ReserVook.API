using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IStateService
    {
        // 情報取得(画面表示,削除用)
        Task<List<MstStateInfo>> GetStateList(MstStateInfo stateInfo);

        // 情報取得(編集用)
        Task<MstStateInfo> GetStateById(MstStateInfo stateInfo);

        // 情報追加
        Task<int> AddState(MstStateInfo stateInfo);

        // 情報更新
        Task<int> UpdateState(MstStateInfo stateInfo, bool addFlag);

        // 情報削除
        Task<int> DelState(MstStateInfo stateInfo);

        // 削除チェック
        Task<int> DeleteStateCheck(MstStateInfo stateInfo);

        // 忘れ物状態取得(他マスタ用)
        Task<List<MstStateInfo>> GetState(string companyNo);
    }
}
