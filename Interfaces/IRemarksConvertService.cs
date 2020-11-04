using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IRemarksConvertService
    {

        // 情報の取得(画面表示用)
        Task<List<FrMScRemarksConvertInfo>> GetRemarksConvertList(FrMScRemarksConvertInfo remarksConvertInfo);

        // 情報取得(編集,削除用)
        Task<FrMScRemarksConvertInfo> GetRemarksConvertById(FrMScRemarksConvertInfo remarksConvertInfo);

        // 情報更新
        Task<int> UpdateRemarksConvert(FrMScRemarksConvertInfo remarksConvertInfo);
    }
}
