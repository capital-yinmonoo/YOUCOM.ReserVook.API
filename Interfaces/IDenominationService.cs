using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IDenominationService
    {
        // 情報取得(画面表示,削除用)
        Task<List<MstDenominationInfo>> GetDenominationList(MstDenominationInfo denominationInfo);

        // 情報取得(編集用)
        Task<MstDenominationInfo> GetDenominationById(MstDenominationInfo denominationInfo);

        // 情報追加
        Task<int> AddDenomination(MstDenominationInfo denominationInfo);

        // 情報更新
        Task<int> UpdateDenomination(MstDenominationInfo denominationInfo, bool addFlag);

        // 情報削除
        Task<int> DelDenomination(MstDenominationInfo denominationInfo);

        // 金種取得(別ページ用)
        Task<List<MstDenominationInfo>> GetDenominationList(string companyNo);

        // 削除チェック
        Task<int> DeleteDenominationCheck(MstDenominationInfo denominationInfo);

    }
}
