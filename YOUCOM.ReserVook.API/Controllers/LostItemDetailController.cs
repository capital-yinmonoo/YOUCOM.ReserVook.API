using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Controllers {

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LostItemDetailController : ControllerBase {

        private readonly ILogger<LostItemDetailController> _logger;
        private ILostItemDetailService _lostItemDetailService;

        public LostItemDetailController(ILogger<LostItemDetailController> logger, ILostItemDetailService lostItemDetailService)
        {
            _logger = logger;
            _lostItemDetailService = lostItemDetailService;

        }

        // データ取得
        [HttpPost("getlostitemByPK")]
        public async Task<LostItemDetailInfo> GetlostitemByPK(LostItemDetailInfo lostInfo)
        {
            // 忘れ物基本情報取得
            var result = await _lostItemDetailService.GetlostItemByPK(lostInfo);
            return result;
        }

        // データ取得(Picture)
        [HttpPost("getlostitemImage")]
        public async Task<IActionResult> GetlostitemImage(LostItemsPictureInfo lostInfo)
        {
            // 忘れ物写真情報取得
            var result = await _lostItemDetailService.GetlostItemImage(lostInfo);
            return File(result.BinaryData, result.ContentType);
        }

        // 登録
        [HttpPost("addlostitem")]
        public async Task<string> InsertInfo(LostItemDetailInfo lostInfo)
        {
            // 忘れ物情報登録
            var result = await _lostItemDetailService.AddLostItem(lostInfo);
            return result;
        }

        // 更新
        [HttpPost("updatelostitem")]
        public async Task<CommonEnum.DBUpdateResult> UpdateInfo(LostItemDetailInfo lostInfo)
        {
            // 忘れ物情報登録
            var result = await _lostItemDetailService.UpdateLostItem(lostInfo);
            return result;
        }

        // イメージ更新
        [HttpPost("updateImage")]
        public async Task<CommonEnum.DBUpdateResult> UpdateImage()
        {

            // Key情報取得
            string companyNo;
            companyNo = Request.Form["company"];

            string managementNo;
            managementNo = Request.Form["managementNo"];

            // ファイルを取得
            var files = Request.Form.Files;

            // 忘れ物情報登録
            var result = await _lostItemDetailService.UpdateImage(companyNo, managementNo, files);
            return result;
        }
    }
}
