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
    public class LostItemListController : ControllerBase
    {


        private ILostItemListService _lostItemListService;

        public LostItemListController(ILostItemListService lostItemListService)
        {
            _lostItemListService = lostItemListService;

        }

        // 情報取得(画面表示用)
        [HttpPost("getList")]
        public async Task<List<TrnLostItemsBaseInfo>> GetLostItemList(TrnLostItemsBaseInfo lostItemListInfo)
        {
            var result = await _lostItemListService.GetLostItemList(lostItemListInfo);
            return result;
        }

        // 情報取得(編集用)
        [HttpPost("getListById")]
        public async Task<TrnLostItemsBaseInfo> GetLostItemListById(TrnLostItemsBaseInfo lostItemListInfo)
        {
            return await _lostItemListService.GetLostItemListById(lostItemListInfo);
        }

        // イメージ画像取得(編集用)
        [HttpPost("getImage")]
        public async Task<List<TrnLostItemsPictureInfo>> GetLostItemImage(TrnLostItemsBaseInfo lostItemListInfo)
        {
            return await _lostItemListService.GetLostItemImage(lostItemListInfo);
        }

        // 現在使用容量取得
        [HttpPost("getUsingCapacity")]
        public async Task<long> GetUsingCapacity(TrnLostItemsBaseInfo lostItemListInfo)
        {
            return await _lostItemListService.GetUsingCapacity(lostItemListInfo);
        }

        // 情報追加
        [HttpPut("addLostItem")]
        public async Task<int> AddLostItem(TrnLostItemsBaseInfo lostItemListInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _lostItemListService.AddLostItem(lostItemListInfo);
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
        [HttpPost("updateLostItem")]
        public async Task<CommonEnum.DBUpdateResult> UpdateLostItem(TrnLostItemsBaseInfo lostItemListInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _lostItemListService.UpdateLostItem(lostItemListInfo, addFlag);
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

        // イメージ画像登録
        [HttpPost("updateImage")]
        public async Task<int> AddLostItemPicture(List<TrnLostItemsPictureInfo> lostItemListInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _lostItemListService.AddLostItemPicture(lostItemListInfo);
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
                addFlg = -99;
                return addFlg;
            }

        }

        // イメージ画像最大容量チェック
        [HttpPost("isOverMaxCapacity")]
        public async Task<int> IsOverMaxCapacity(List<TrnLostItemsPictureInfo> lostItemListInfo) {

            return await _lostItemListService.IsOverMaxCapacity(lostItemListInfo);
        }

        // 情報削除
        [HttpPut("delLostItem")]
        public async Task<int> DelLostItem(TrnLostItemsBaseInfo lostItemListInfo)
        {
            try
            {
                int ret = await _lostItemListService.DelLostItem(lostItemListInfo);
                return ret;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // 情報一括削除
        [HttpPut("lumpDelLostItem")]
        public async Task<int> LumpDelLostItem(TrnLostItemsBaseInfo lostItemListInfo)
        {
            try
            {
                int ret = await _lostItemListService.LumpDelLostItem(lostItemListInfo);
                return ret;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}