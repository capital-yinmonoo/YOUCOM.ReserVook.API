using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Controllers;
using YOUCOM.ReserVook.API.Entities;

namespace YOUCOM.ReserVook.API.Interfaces {
    public interface ICustomerService {

        // 顧客情報の取得(画面表示,削除用)
        Task<List<MstCustomerInfo>> GetCustomerList(MstCustomerInfo customerInfo);

        // 顧客情報の取得(編集用)
        Task<MstCustomerInfo> GetCustomerById(MstCustomerInfo customerInfo);

        // 顧客情報の追加
        Task<int> AddCustomer(MstCustomerInfo customerInfo);

        // 顧客の情報追加(顧客番号を返す)
        Task<CustomerRegInfo> AddCustomerForReserve(MstCustomerInfo customerInfo);

        // 顧客情報の更新
        Task<int> UpdateCustomer(MstCustomerInfo customerInfo, bool addFlag);

        // 顧客情報の削除
        Task<int> DelCustomer(MstCustomerInfo customerInfo);
    }
}
