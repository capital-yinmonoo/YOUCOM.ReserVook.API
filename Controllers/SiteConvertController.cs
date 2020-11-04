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
    public class SiteConvertController : ControllerBase
    {


        private ISiteConvertService _siteConvertService;


        public SiteConvertController(ISiteConvertService siteConvertService)
        {
            _siteConvertService = siteConvertService;

        }

        // 情報取得(画面表示用)
        [HttpPost("getlist")]
        public async Task<List<FrMScSiteConvertInfo>> GetSiteConvertList(FrMScSiteConvertInfo siteConvertInfo)
        {
            var result = await _siteConvertService.GetSiteConvertList(siteConvertInfo);
            return result;
        }

        // 情報取得(編集用)
        [HttpPost("getlistbyid")]
        public async Task<FrMScSiteConvertInfo> GetSiteConvertById(FrMScSiteConvertInfo siteConvertInfo)
        {
            return await _siteConvertService.GetSiteConvertById(siteConvertInfo);
        }

        // 情報追加
        [HttpPost("addsiteconvert")]
        public async Task<int> AddSiteConvert(FrMScSiteConvertInfo siteConvertInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _siteConvertService.AddSiteConvert(siteConvertInfo);
                if (result == 0)
                {
                    return addFlg;
                }
                else
                {
                    addFlg = 1;
                    return addFlg;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                addFlg = -1;
                return addFlg;
            }

        }

        // 情報更新
        [HttpPut("updatesiteconvert")]
        public async Task<CommonEnum.DBUpdateResult> UpdateSiteConvert(FrMScSiteConvertInfo siteConvertInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _siteConvertService.UpdateSiteConvert(siteConvertInfo, addFlag);
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

        // 情報削除
        [HttpPut("delsiteconvert")]
        public async Task<CommonEnum.DBUpdateResult> DelSiteConvert(FrMScSiteConvertInfo siteConvertInfo)
        {
            try
            {
                int ret = await _siteConvertService.DelSiteConvert(siteConvertInfo);
                if (ret == 1)
                {
                    return CommonEnum.DBUpdateResult.Success;
                }
                else if (ret == -1)
                {
                    return CommonEnum.DBUpdateResult.VersionError;
                }
                else if (ret == -2)
                {
                    return CommonEnum.DBUpdateResult.UsedError;
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

        // サイト名取得(別ページ用)
        [HttpGet("getSiteCodeList")]
        public async Task<List<FrMScSiteConvertInfo>> GetSiteCodeList(string companyNo, string scCd)
        {
            var result = await _siteConvertService.GetSiteCodeList(companyNo, scCd);
            return result;
        }

        // 削除チェック
        [HttpPut("deleteSiteCdCheck")]
        public async Task<int> DeleteSiteCdCheck(FrMScSiteConvertInfo siteConvertInfo)
        {
            int result = await _siteConvertService.DeleteSiteCdCheck(siteConvertInfo);
            return result;
        }
    }
}