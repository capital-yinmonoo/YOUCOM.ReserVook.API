using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Controllers {

    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class DataExportController : ControllerBase {

        private IDataExportService _dataExportService;

        public DataExportController(IDataExportService dataExportService) {
            _dataExportService = dataExportService;
        }

        // 予約情報取得
        [HttpPost("getReserveDataList")]
        public async Task<List<ExportReserveInfo>> GetReserveDataList(ExportReserveCondition cond) {
            var result = await _dataExportService.GetReserveDataList(cond);
            return result;
        }


        // 顧客情報取得
        [HttpPost("getCustomerDataList")]
        public async Task<List<ExportCustomerInfo>> GetCustomerDataList(ExportCustomerCondition cond) {
            var result = await _dataExportService.GetCustomerDataList(cond);
            return result;
        }
    }

}