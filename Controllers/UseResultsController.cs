using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Controllers {

    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UseResultsController : ControllerBase {

        private IUseResultsService _useResultsService;

        public UseResultsController(IUseResultsService useResultsService) {
            _useResultsService = useResultsService;
        }

        // 利用実績の情報取得
        [HttpPost("getUseResultsList")]
        public async Task<List<TrnUseResultsInfo>> GetUseResultsList(TrnUseResultsInfo useResultsInfo) {
            var result = await _useResultsService.GetUseResultsList(useResultsInfo);
            return result;
        }

        // 利用実績情報追加
        [HttpPost("addUseResults")]
        public async Task<int> AddUseResults(TrnUseResultsInfo useResultsInfo) {
            int addFlg = 0;

            try {
                var result = await _useResultsService.AddUseResults(useResultsInfo);
                if (result == 0) {
                    return addFlg;
                } else {
                    addFlg = 1;
                    return addFlg;
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                addFlg = -1;
                return addFlg;
            }

        }

        // 顧客情報削除
        [HttpPut("delUseResults")]
        public async Task<CommonEnum.DBUpdateResult> DelUseResults(TrnUseResultsInfo useResultsInfo) {
            try {
                int ret = await _useResultsService.DelUseResults(useResultsInfo);
                if (ret == 1) {
                    return CommonEnum.DBUpdateResult.Success;
                } else if (ret == -1) {
                    return CommonEnum.DBUpdateResult.VersionError;
                } else if (ret == -2) {
                    return CommonEnum.DBUpdateResult.UsedError;
                } else {
                    return CommonEnum.DBUpdateResult.Error;
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }
    }

}