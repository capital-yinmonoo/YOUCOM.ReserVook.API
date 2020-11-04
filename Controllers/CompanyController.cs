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
    public class CompanyController : ControllerBase
    {

        private readonly ILogger<CompanyController> _logger;
        private ICompanyService _companyservice;

        public CompanyController(ILogger<CompanyController> logger, ICompanyService companyService)
        {
            _logger = logger;

            _companyservice = companyService;
        }

        /// <summary>
        /// 会社マスタ 削除
        /// </summary>
        /// <param name="conpanyInfo">会社情報</param>
        /// <returns></returns>
        [HttpPut("deleteCompany")]
        public async Task<CommonEnum.DBUpdateResult> DeleteCompany(MstCompanyInfo conpanyInfo)
        {

            try
            {
                int ret = await _companyservice.DeleteCompany(conpanyInfo);
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

        /// <summary>
        ///会社マスタ 更新
        /// </summary>
        /// <param name="conpanyInfo">会社情報</param>
        /// <returns></returns>
        [HttpPost("updateCompany")]
        public async Task<CommonEnum.DBUpdateResult> UpdateCompany(MstCompanyInfo conpanyInfo)
        {
            try
            {
                int ret = await _companyservice.UpdateCompany(conpanyInfo);
                if (ret == 1)
                {
                    return CommonEnum.DBUpdateResult.Success;
                }
                else if (ret == -1)
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

        /// <summary>
        /// 会計書ロゴ 更新
        /// </summary>
        /// <param name="conpanyInfo">会社情報</param>
        /// <returns></returns>
        [HttpPost("updateImage")]
        public async Task<CommonEnum.DBUpdateResult> UpdateImage()
        {

            try
            {
                // Key情報取得
                string companyNo;
                companyNo = Request.Form["company"];

                // ファイルを取得
                var files = Request.Form.Files;

                // 更新
                int ret = await _companyservice.UpdateImage(files, companyNo);
                if (ret == 1)
                {
                    return CommonEnum.DBUpdateResult.Success;
                }
                else if (ret == -1)
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

        /// <summary>
        /// 会社マスタ 追加
        /// </summary>
        /// <param name="companyInfo">会社情報</param>
        /// <returns></returns>
        [HttpPut("addCompany")]
        public async Task<bool> AddCompany(MstCompanyInfo companyInfo)
        {
            try
            {
                int var = await _companyservice.AddCompany(companyInfo);
                if (var > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 会社マスタ 取得
        /// </summary>
        /// <param name="id">会社番号</param>
        /// <returns></returns>
        [HttpGet("getCompanyByPK/{id}")]
        public async Task<MstCompanyInfo> GetCompanyByPK(string id)
        {
            return await _companyservice.GetCompanyByPK(id);
        }

        /// <summary>
        /// 会社マスタ 一覧取得
        /// </summary>
        /// <returns></returns>
        [HttpGet("getCompanyList")]
        public async Task<List<MstCompanyInfo>> GetList()
        {
            return await _companyservice.GetList();
        }

        /// <summary>
        /// 会社マスタ 一覧取得
        /// </summary>
        /// <returns></returns>
        [HttpGet("getCompanyListByCompanyGroupId/{companyGroupId}")]
        public async Task<List<MstCompanyInfo>> GetCompanyListByCompanyGroupId(string companyGroupId)
        {
            return await _companyservice.GetCompanyListByCompanyGroupId(companyGroupId);
        }

        /// <summary>
        /// サービス料率取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("getServiceRate")]
        public async Task<decimal> GetServiceRate(BaseInfo cond)
        {
            return await _companyservice.GetServiceRate(cond);
        }

        /// <summary>
        /// 会社ロゴ 取得
        /// </summary>
        /// <param name="id">会社番号</param>
        /// <returns></returns>
        [HttpGet("getCompanyImage/{id}")]
        public async Task<IActionResult> GetCompanyImage(string id)
        {
            var companyInfo = await _companyservice.GetCompanyByPK(id);
            if (companyInfo.LogoData != null && companyInfo.ContentType != null)
            {
                return File(companyInfo.LogoData, companyInfo.ContentType);
            }
            else
            {
                return Ok();
            }

        }
    }
}