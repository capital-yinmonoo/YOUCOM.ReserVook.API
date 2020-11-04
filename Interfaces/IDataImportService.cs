using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Controllers;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces {
    public interface IDataImportService {

        // 予約情報インポート
        Task<ImportResultInfo> ImportReserveData(List<ImportReserveInfo> importReserveList);

        // 顧客情報インポート
        Task<ImportResultInfo> ImportCustomerData(List<ImportCustomerInfo> importCustomerList);
        
    }
}
