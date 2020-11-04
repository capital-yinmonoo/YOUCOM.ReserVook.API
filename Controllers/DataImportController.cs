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
    public class DataImportController : ControllerBase {

        private IDataImportService _dataImportService;

        public DataImportController(IDataImportService dataImportService) {
            _dataImportService = dataImportService;
        }

        // 予約情報取得
        [HttpPost("importReserveData")]
        public async Task<ImportResultInfo> ImportReserveData(List<ImportReserveInfo> importReserveList) {
            var result = await _dataImportService.ImportReserveData(importReserveList);
            return result;
        }


        // 顧客情報取得
        [HttpPost("importCustomerData")]
        public async Task<ImportResultInfo> ImportCustomerData(List<ImportCustomerInfo> importCustomerList) {
            var result = await _dataImportService.ImportCustomerData(importCustomerList);
            return result;
        }
    }

}