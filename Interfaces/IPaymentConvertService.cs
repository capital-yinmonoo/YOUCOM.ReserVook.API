using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface IPaymentConvertService
    {

        // 情報の取得(画面表示用)
        Task<List<FrMScPaymentConvertInfo>> GetPaymentConvertList(FrMScPaymentConvertInfo paymentConvertInfo);

        // 情報取得(編集,削除用)
        Task<FrMScPaymentConvertInfo> GetPaymentConvertById(FrMScPaymentConvertInfo paymentConvertInfo);

        // 情報追加
        Task<int> AddPaymentConvert(FrMScPaymentConvertInfo paymentConvertInfo);

        // 情報更新
        Task<int> UpdatePaymentConvert(FrMScPaymentConvertInfo paymentConvertInfo, bool addFlag);

        // 情報削除
        Task<int> DelPaymentConvert(FrMScPaymentConvertInfo paymentConvertInfo);
    }
}
