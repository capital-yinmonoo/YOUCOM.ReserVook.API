using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    //[EnableCors("AllowSpecificOrigin")]
    [ApiController]
    [Route("[controller]")]
    public class CompanyGroupController : ControllerBase
    {

        private readonly ILogger<CompanyGroupController> _logger;
        private ICompanyGroupService _companyGroupService;

        public CompanyGroupController(ILogger<CompanyGroupController> logger, ICompanyGroupService companyGroupService)
        {
            _logger = logger;

            _companyGroupService = companyGroupService;
        }

        /// <summary>
        /// 会社グループリスト 取得
        /// </summary>
        /// <returns></returns>
        [HttpGet("getCompanyGroupList")]
        public async Task<List<MstCompanyGroupInfo>> GetCompanyGroupList()
        {
            return await _companyGroupService.GetCompanyGroupList();
        }

        /// <summary>
        /// 会社グループ　取得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost("getCompanyGroupByPK")]
        public async Task<MstCompanyGroupInfo> GetCompanyGroupByPK(MstCompanyGroupInfo info)
        {
            return await _companyGroupService.GetCompanyGroupByPK(info.CompanyGroupId);
        }

        /// <summary>
        /// 会社グループ 追加
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("addCompanyGroup")]
        public async Task<CommonEnum.DBUpdateResult> AddCompanyGroup(MstCompanyGroupInfo info)
        {
            return await _companyGroupService.AddCompanyGroup(info);
        }

        /// <summary>
        /// 会社グループ 更新
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("updateCompanyGroup")]
        public async Task<CommonEnum.DBUpdateResult> UpdateCompanyGroup(MstCompanyGroupInfo info)
        {
            try
            {
                return await _companyGroupService.UpdateCompanyGroup(info);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }

        }

        /// <summary>
        /// 会社グループ 削除
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("deleteCompanyGroup")]
        public async Task<CommonEnum.DBUpdateResult> DeleteCompanyGroup(MstCompanyGroupInfo info)
        {

            try
            {
                return await _companyGroupService.DeleteCompanyGroup(info);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// 会社グループ 削除前チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("checkBeforeDelete")]
        public async Task<CommonEnum.DBUpdateResult> CheckBeforeDelete(MstCompanyGroupInfo info)
        {
            return await _companyGroupService.CheckBeforeDelete(info);
        }

    }
}