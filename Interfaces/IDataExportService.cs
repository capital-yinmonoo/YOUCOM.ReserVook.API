using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Controllers;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces {
    public interface IDataExportService {

        // 予約情報取得
        Task<List<ExportReserveInfo>> GetReserveDataList(ExportReserveCondition cond);

        // 顧客情報取得
        Task<List<ExportCustomerInfo>> GetCustomerDataList(ExportCustomerCondition cond);
    }
}
