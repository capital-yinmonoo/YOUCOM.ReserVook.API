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
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private IItemService _itemService;

        public ItemController(ILogger<ItemController> logger, IItemService itemService)
        {
            _logger = logger;

            _itemService = itemService;
        }

        /// <summary>
        /// 商品マスタ 削除
        /// </summary>
        /// <param name="itemInfo">商品情報</param>
        /// <returns></returns>
        [HttpPut("deleteitem")]
        public async Task<CommonEnum.DBUpdateResult> DeleteItem(MstItemInfo itemInfo)
        {

            try
            {
                int ret = await _itemService.DeleteItem(itemInfo);
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
        /// 商品マスタ 更新
        /// </summary>
        /// <param name="itemInfo">商品情報</param>
        /// <returns></returns>
        [HttpPut("updateitem")]
        public async Task<CommonEnum.DBUpdateResult> UpdateItem(MstItemInfo itemInfo)
        {
            try
            {
                int ret = await _itemService.UpdateItem(itemInfo);
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
        /// 商品マスタ 更新
        /// </summary>
        /// <param name="itemInfo">商品情報</param>
        /// <returns></returns>
        [HttpPut("checkDelete")]
        public async Task<int> CheckDelete(MstItemInfo itemInfo)
        {
            return await _itemService.CheckDelete(itemInfo);
        }

        [HttpPost("getitembyPK")]
        public async Task<MstItemInfo> GetItemByPK(MstItemInfo itemInfo)
        {
            return await _itemService.GetItemByPK(itemInfo);
        }

        [HttpPost("getitembyPKview")]
        public async Task<ItemInfoView> GetItemByPKView(MstItemInfo itemInfo)
        {
            return await _itemService.GetItemByPKView(itemInfo);
        }

        [HttpPost("getitemlist")]
        public async Task<List<MstItemInfo>> GetList(MstItemInfo itemInfo)
        {
            var result = await _itemService.GetList(itemInfo);
            return result;
        }

        [HttpPost("getItemListView")]
        public async Task<List<ItemInfoView>> GetListView(MstItemInfo itemInfo)
        {
            var result = await _itemService.GetListView(itemInfo);
            return result;
        }

        [HttpPost("additem")]
        public async Task<bool> AddItem(MstItemInfo itemInfo)
        {
            try
            {
                int var = await _itemService.AddItem(itemInfo);
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
    }
}