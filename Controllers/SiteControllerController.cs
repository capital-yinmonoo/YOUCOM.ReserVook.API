using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Route("[controller]")]
    [ApiController]
    public class SiteControllerController : ControllerBase
    {


        private ISiteControllerService _siteControllerService;

        public SiteControllerController(ISiteControllerService siteControllerService)
        {
            _siteControllerService = siteControllerService;

        }

        // 情報取得(編集用)
        [HttpPost("getSiteControllerList")]
        public async Task<List<FrMScSiteControllerInfo>> GetSiteControllerList(FrMScSiteControllerInfo siteControllerInfo) {
            return await _siteControllerService.GetSiteControllerList(siteControllerInfo);
        }


        // 情報取得(編集用)
        [HttpPost("getlistbyid")]
        public async Task<FrMScSiteControllerInfo> GetSiteControllerById(FrMScSiteControllerInfo siteControllerInfo)
        {
            return await _siteControllerService.GetSiteControllerById(siteControllerInfo);
        }

        // 情報更新
        [HttpPut("updatesitecontroller")]
        public async Task<CommonEnum.DBUpdateResult> UpdateSiteController(FrMScSiteControllerInfo siteControllerInfo)
        {
            try
            {
                int result = await _siteControllerService.UpdateSiteController(siteControllerInfo);
                if (result == 1)
                {
                    return CommonEnum.DBUpdateResult.Success;
                }
                else if (result == -1)
                {
                    return CommonEnum.DBUpdateResult.VersionError;
                }
                else
                {
                    return CommonEnum.DBUpdateResult.Error;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }
    }
}