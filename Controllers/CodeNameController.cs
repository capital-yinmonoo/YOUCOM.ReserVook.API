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
    [Route("[controller]")]
    [ApiController]
    public class CodeNameController : ControllerBase
    {

        private readonly ILogger<CodeNameController> _logger;

        private ICodeNameService _dictionaryDataService;

        public CodeNameController(ILogger<CodeNameController> logger, ICodeNameService dictionaryDataService)
        {

            _logger = logger;

            _dictionaryDataService = dictionaryDataService;

        }

        // マスタの情報取得(画面表示,削除用)
        [HttpPost("getlist")]
        public async Task<List<MstCodeNameInfo>> GetCodeNameList(MstCodeNameInfo codenameInfo)
        {
            var result = await _dictionaryDataService.GetCodeNameList(codenameInfo);
            return result;
        }

        // マスタの情報取得(編集用)
        [HttpPost("getlistbyid")]
        public async Task<MstCodeNameInfo> GetCodeNameById(MstCodeNameInfo codenameInfo)
        {
            return await _dictionaryDataService.GetCodeNameById(codenameInfo);
        }

        // マスタの情報追加
        [HttpPost("addcodename")]
        public async Task<int> AddCodeName(MstCodeNameInfo codenameInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _dictionaryDataService.AddCodeName(codenameInfo);
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

        // マスタ情報更新
        [HttpPut("updatecodename")]
        public async Task<CommonEnum.DBUpdateResult> UpdateCodeName(MstCodeNameInfo codenameInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _dictionaryDataService.UpdateCodeName(codenameInfo, addFlag);
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

        // マスタ情報削除
        [HttpPut("delcodename")]
        public async Task<CommonEnum.DBUpdateResult> DelCodeName(MstCodeNameInfo codenameInfo)
        {
            try
            {
                int ret = await _dictionaryDataService.DelCodeName(codenameInfo);
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

        // 削除チェック
        [HttpPut("deletecodenamecheck")]
        public async Task<int> DeleteCodeNameCheck(MstCodeNameInfo codenameInfo)
        {
            int result = await _dictionaryDataService.DeleteCodeNameCheck(codenameInfo);
            return result;
        }

        // フロアの取得
        [HttpGet("getFloorList")]
        public async Task<List<MstCodeNameInfo>> GetFloorList(string companyNo)
        {
            string codeDivision = ((int)CommonEnum.CodeDivision.Floor).ToString(CommonConst.CODE_DIVISION_FORMAT);
            var model = await _dictionaryDataService.GetListDictionaryDataDid(companyNo, codeDivision);
            return model;

        }

        // 部屋タイプの取得
        [HttpGet("getTypeList")]
        public async Task<List<MstCodeNameInfo>> GetTypeList(string companyNo)
        {
            string codeDivision = ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT);
            var model = await _dictionaryDataService.GetListDictionaryDataDid(companyNo, codeDivision);
            return model;

        }

        // 禁煙/喫煙の取得
        [HttpGet("getIsForbidList")]
        public async Task<List<MstCodeNameInfo>> GetIsForbidList(string companyNo)
        {
            string codeDivision = ((int)CommonEnum.CodeDivision.IsForbid).ToString(CommonConst.CODE_DIVISION_FORMAT);
            var model = await _dictionaryDataService.GetListDictionaryDataDid(companyNo, codeDivision);
            return model;

        }

        // TLリンカーン用決済方法の取得
        [HttpGet("getSettlementList")]
        public async Task<List<MstCodeNameInfo>> GetSettlementList(string companyNo)
        {
            string codeDivision = ((int)CommonEnum.CodeDivision.TLPayment).ToString(CommonConst.CODE_DIVISION_FORMAT);
            var model = await _dictionaryDataService.GetListDictionaryDataDid(companyNo, codeDivision);
            return model;

        }

        // ユーザー権限の取得
        [HttpGet("getRoleList")]
        public async Task<List<MstCodeNameInfo>> GetRoleList(string companyNo)
        {
            string codeDivision = ((int)CommonEnum.CodeDivision.Role).ToString(CommonConst.CODE_DIVISION_FORMAT);
            var model = await _dictionaryDataService.GetListDictionaryDataDid(companyNo, codeDivision);
            return model;

        }

        // 忘れ物・清掃管理使用区分の取得
        [HttpGet("getLostFlgList")]
        public async Task<List<MstCodeNameInfo>> GetLostFlgList(string companyNo)
        {
            string codeDivision = ((int)CommonEnum.CodeDivision.CleanRoomsManager).ToString(CommonConst.CODE_DIVISION_FORMAT);
            var model = await _dictionaryDataService.GetListDictionaryDataDid(companyNo, codeDivision);
            return model;
        }

        // 部屋タイプ区分の取得
        [HttpGet("getRoomTypeDivisionList")]
        public async Task<List<MstCodeNameInfo>> GetRoomTypeDivisionList(string companyNo)
        {
            string codeDivision = ((int)CommonEnum.CodeDivision.RoomTypeDivision).ToString(CommonConst.CODE_DIVISION_FORMAT);
            var model = await _dictionaryDataService.GetListDictionaryDataDid(companyNo, codeDivision);
            return model;
        }

        // 忘れ物発見場所分類の取得
        [HttpGet("getFoundPlaceList")]
        public async Task<List<MstCodeNameInfo>> GetFoundPlaceList(string companyNo)
        {
            string codeDivision = ((int)CommonEnum.CodeDivision.LostPlace).ToString(CommonConst.CODE_DIVISION_FORMAT);
            var model = await _dictionaryDataService.GetListDictionaryDataDid(companyNo, codeDivision);
            return model;
        }

        // 忘れ物保管分類の取得
        [HttpGet("getStorageList")]
        public async Task<List<MstCodeNameInfo>> GetStorageList(string companyNo)
        {
            string codeDivision = ((int)CommonEnum.CodeDivision.LostStrage).ToString(CommonConst.CODE_DIVISION_FORMAT);
            var model = await _dictionaryDataService.GetListDictionaryDataDid(companyNo, codeDivision);
            return model;
        }

    }
}