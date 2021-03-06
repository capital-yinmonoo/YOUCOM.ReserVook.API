﻿using Microsoft.AspNetCore.Authorization;
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
    public class PaymentConvertController : ControllerBase
    {


        private IPaymentConvertService _paymentConvertService;


        public PaymentConvertController(IPaymentConvertService paymentConvertService)
        {
            _paymentConvertService = paymentConvertService;

        }

        // 情報取得(画面表示用)
        [HttpPost("getList")]
        public async Task<List<FrMScPaymentConvertInfo>> GetPaymentConvertList(FrMScPaymentConvertInfo paymentConvertInfo)
        {
            var result = await _paymentConvertService.GetPaymentConvertList(paymentConvertInfo);
            return result;
        }

        // 情報取得(編集用)
        [HttpPost("getListById")]
        public async Task<FrMScPaymentConvertInfo> GetPaymentConvertById(FrMScPaymentConvertInfo paymentConvertInfo)
        {
            return await _paymentConvertService.GetPaymentConvertById(paymentConvertInfo);
        }

        // 情報追加
        [HttpPost("addPaymentConvert")]
        public async Task<int> AddPaymentConvert(FrMScPaymentConvertInfo paymentConvertInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _paymentConvertService.AddPaymentConvert(paymentConvertInfo);
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
        [HttpPut("updatePaymentConvert")]
        public async Task<CommonEnum.DBUpdateResult> UpdatePaymentConvert(FrMScPaymentConvertInfo paymentConvertInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _paymentConvertService.UpdatePaymentConvert(paymentConvertInfo, addFlag);
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
        [HttpPut("delPaymentConvert")]
        public async Task<CommonEnum.DBUpdateResult> DelPaymentConvert(FrMScPaymentConvertInfo paymentConvertInfo)
        {
            try
            {
                int ret = await _paymentConvertService.DelPaymentConvert(paymentConvertInfo);
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
    }
}