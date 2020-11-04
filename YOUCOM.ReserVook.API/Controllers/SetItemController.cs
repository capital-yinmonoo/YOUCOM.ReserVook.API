using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    //[EnableCors("AllowSpecificOrigin")]
    [ApiController]
    [Route("[controller]")]
    public class SetItemController : ControllerBase
    {

        private readonly ILogger<SetItemController> _logger;
        private ISetItemService _setItemService;
        private IItemService _itemService;

        public SetItemController(ILogger<SetItemController> logger, ISetItemService setItemService, IItemService itemService)
        {
            _logger = logger;

            _setItemService = setItemService;

            _itemService = itemService;
        }

        /// <summary>
        /// セット商品(親)リスト 取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        [HttpPost("getSetItemParentList")]
        public async Task<List<MstItemInfo>> GetSetItemParentList(BaseInfo cond)
        {
            return await _setItemService.GetSetItemParentList(cond);
        }

        /// <summary>
        /// セット商品(親+子リスト) 取得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost("getSetItemByPK")]
        public async Task<SetItemInfo> GetSetItemByPK(MstItemInfo key)
        {
            return await _setItemService.GetSetItemByPK(key);
        }

        /// <summary>
        /// セット商品(親+子リスト) 追加
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost("addSetItem")]
        public async Task<CommonEnum.DBUpdateResult> AddSetItem(SetItemInfo info)
        {
            return await _setItemService.AddSetItem(info);
        }

        /// <summary>
        /// セット商品(親+子リスト) 更新
        /// </summary>
        /// <param name="itemInfo">商品情報</param>
        /// <returns></returns>
        [HttpPost("updateSetItem")]
        public async Task<CommonEnum.DBUpdateResult> UpdateSetItem(SetItemInfo info)
        {
            try
            {
                return await _setItemService.UpdateSetItem(info);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }

        }

        /// <summary>
        /// セット商品(親+子リスト) 削除
        /// </summary>
        /// <param name="itemInfo">商品情報</param>
        /// <returns></returns>
        [HttpPost("deleteSetItem")]
        public async Task<CommonEnum.DBUpdateResult> DeleteSetItem(SetItemInfo info)
        {

            try
            {
                return await _setItemService.DeleteSetItem(info);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// セット商品 削除前チェック
        /// </summary>
        /// <param name="itemInfo">商品情報</param>
        /// <returns></returns>
        [HttpPost("checkBeforeDelete")]
        public async Task<int> CheckBeforeDelete(SetItemInfo info)
        {
            return await _itemService.CheckDelete(info, true);
        }

    }
}