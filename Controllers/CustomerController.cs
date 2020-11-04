using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Controllers {

    [Authorize]
    //[EnableCors("AllowSpecificOrigin")]
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase {

        private ICustomerService _customerService;

        public CustomerController(ICustomerService customerService) {
            _customerService = customerService;
        }

        // 顧客の情報取得(画面表示,削除用)
        [HttpPost("getCustomerList")]
        public async Task<List<MstCustomerInfo>> GetCustomerList(MstCustomerInfo customerInfo) {
            var result = await _customerService.GetCustomerList(customerInfo);
            return result;
        }

        // 顧客の情報取得(編集用)
        [HttpPost("getCustomerById")]
        public async Task<MstCustomerInfo> GetCustomerById(MstCustomerInfo customerInfo) {
            return await _customerService.GetCustomerById(customerInfo);
        }

        // 顧客の情報追加
        [HttpPost("addCustomer")]
        public async Task<int> AddCustomer(MstCustomerInfo customerInfo) {
            int addFlg = 0;

            try {
                var result = await _customerService.AddCustomer(customerInfo);
                if (result == 0) {
                    return addFlg;
                } else {
                    addFlg = 1;
                    return addFlg;
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                addFlg = -1;
                return addFlg;
            }

        }

        // 顧客の情報追加(顧客番号を返す)
        [HttpPost("addCustomerForReserve")]
        public async Task<CustomerRegInfo> AddCustomerForReserve(MstCustomerInfo customerInfo) {

            try {
                return await _customerService.AddCustomerForReserve(customerInfo);

            } catch (Exception e) {
                Console.WriteLine(e.Message);

                CustomerRegInfo retInfo = new CustomerRegInfo();
                retInfo.CustomerNo = "";
                retInfo.ResultCode = -1;

                return retInfo;
            }
        }

        // 顧客情報更新
        [HttpPut("updateCustomer")]
        public async Task<CommonEnum.DBUpdateResult> UpdateCustomer(MstCustomerInfo customerInfo) {
            try {
                bool addFlag = false;
                int result = await _customerService.UpdateCustomer(customerInfo, addFlag);
                if (result == 1) {
                    return CommonEnum.DBUpdateResult.Success;
                } else if (result == -1) {
                    return CommonEnum.DBUpdateResult.VersionError;
                } else {
                    return CommonEnum.DBUpdateResult.Error;
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        // 顧客情報削除
        [HttpPut("delCustomer")]
        public async Task<CommonEnum.DBUpdateResult> DelCustomer(MstCustomerInfo customerInfo) {
            try {
                int ret = await _customerService.DelCustomer(customerInfo);
                if (ret == 1) {
                    return CommonEnum.DBUpdateResult.Success;
                } else if (ret == -1) {
                    return CommonEnum.DBUpdateResult.VersionError;
                } else if (ret == -2) {
                    return CommonEnum.DBUpdateResult.UsedError;
                } else {
                    return CommonEnum.DBUpdateResult.Error;
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }
    }
    public class CustomerRegInfo {

        public string CustomerNo;
        public int ResultCode;

        public CustomerRegInfo() {
            CustomerNo = "";
            ResultCode = 0;
        }
    }

}