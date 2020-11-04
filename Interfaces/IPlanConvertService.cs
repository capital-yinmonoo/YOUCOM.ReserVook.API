using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IPlanConvertService
    {
        // 情報の取得(画面表示用)
        Task<List<FrMScPlanConvertInfo>> GetPlanConvertList(FrMScPlanConvertInfo planConvertInfo);

        // 情報取得(編集,削除用)
        Task<FrMScPlanConvertInfo> GetPlanConvertById(FrMScPlanConvertInfo planConvertInfo);

        // 情報追加
        Task<int> AddPlanConvert(FrMScPlanConvertInfo planConvertInfo);

        // 情報更新
        Task<int> UpdatePlanConvert(FrMScPlanConvertInfo planConvertInfo, bool addFlag);

        // 情報削除
        Task<int> DelPlanConvert(FrMScPlanConvertInfo planConvertInfo);
    }
}
