using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IPointConvertService
    {

        // 情報の取得(画面表示用)
        Task<List<FrMScPointConvertInfo>> GetPointConvertList(FrMScPointConvertInfo pointConvertInfo);

        // 情報取得(編集,削除用)
        Task<FrMScPointConvertInfo> GetPointConvertById(FrMScPointConvertInfo pointConvertInfo);

        // 情報追加
        Task<int> AddPointConvert(FrMScPointConvertInfo pointConvertInfo);

        // 情報更新
        Task<int> UpdatePointConvert(FrMScPointConvertInfo pointConvertInfo, bool addFlag);

        // 情報削除
        Task<int> DelPointConvert(FrMScPointConvertInfo pointConvertInfo);
    }
}
