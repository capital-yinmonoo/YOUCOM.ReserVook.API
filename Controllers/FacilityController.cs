using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Route("[controller]")]
    [ApiController]
    public class FacilityController : ControllerBase
    {


        private IFacilityService _facilityService;

        public FacilityController(IFacilityService facilityService)
        {
            _facilityService = facilityService;

        }

        #region マスタ系
        [HttpPost("getFacility")]
        public async Task<MstFacilityInfo> GetFacility(MstFacilityInfo FacilityInfo)
        {
            var result = await _facilityService.MasterGetByPK(FacilityInfo);
            return result;
        }

        [HttpPost("getFacilityList")]
        public async Task<List<MstFacilityInfo>> GetFacilityList(MstFacilityInfo FacilityInfo)
        {
            return await _facilityService.MasterGetList(FacilityInfo);
        }

        [HttpPost("addFacility")]
        public async Task<int> AddFacility(MstFacilityInfo FacilityInfo)
        {

            try
            {
                var result = await _facilityService.MasterAdd(FacilityInfo);
                switch (result)
                {
                    case 1: /*single add*/
                        return (int)CommonEnum.DBUpdateResult.Success;

                    case (int)CommonEnum.DBUpdateResult.OverlapError:
                        return (int)CommonEnum.DBUpdateResult.OverlapError;

                    default:
                        return (int)CommonEnum.DBUpdateResult.Error;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return (int)CommonEnum.DBUpdateResult.Error;
            }

        }

        [HttpPost("updateFacility")]
        public async Task<CommonEnum.DBUpdateResult> UpdateFacility(MstFacilityInfo FacilityInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _facilityService.MasterUpdate(FacilityInfo, addFlag);

                switch (result)
                {
                    case 1: /*single update*/
                        return (int)CommonEnum.DBUpdateResult.Success;

                    case (int)CommonEnum.DBUpdateResult.VersionError:
                        return CommonEnum.DBUpdateResult.VersionError;

                    default:
                        return CommonEnum.DBUpdateResult.Error;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        [HttpPost("deleteFacility")]
        public async Task<CommonEnum.DBUpdateResult> DeleteFacility(MstFacilityInfo FacilityInfo)
        {
            try
            {
                int result = await _facilityService.MasterDelete(FacilityInfo);
                switch (result)
                {
                    case 1: /*single delete*/
                        return (int)CommonEnum.DBUpdateResult.Success;

                    case (int)CommonEnum.DBUpdateResult.VersionError:
                        return CommonEnum.DBUpdateResult.VersionError;

                    case (int)CommonEnum.DBUpdateResult.UsedError:
                        return CommonEnum.DBUpdateResult.UsedError;

                    default:
                        return CommonEnum.DBUpdateResult.Error;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        [HttpPost("checkDelete")]
        public async Task<int> CheckDelete(MstFacilityInfo FacilityInfo)
        {
            int result = await _facilityService.MasterCheckDelete(FacilityInfo);
            return result;
        }
        #endregion

        #region 予約会場状況
        [HttpPost("getReserveFacilityList")]
        public async Task<List<TrnReserveFacilityInfo>> GetReserveFacilityList(FacilityCondition cond)
        {
            return await _facilityService.GetReserveFacilityList(cond);
        }

        [HttpPost("addReserveFacility")]
        public async Task<int> AddReserveFacility(TrnReserveFacilityInfo info)
        {

            try
            {
                var result = await _facilityService.Add(info);
                switch (result)
                {
                    case 1: /*single add*/
                        return (int)CommonEnum.DBUpdateResult.Success;

                    case (int)CommonEnum.DBUpdateResult.OverlapError:
                        return (int)CommonEnum.DBUpdateResult.OverlapError;

                    default:
                        return (int)CommonEnum.DBUpdateResult.Error;
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return (int)CommonEnum.DBUpdateResult.Error;
            }

        }

        [HttpPost("updateReserveFacility")]
        public async Task<CommonEnum.DBUpdateResult> UpdateReserveFacility(TrnReserveFacilityInfo info)
        {
            try
            {
                int result = await _facilityService.Update(info);
                switch (result)
                {
                    case 1: /*single update*/
                        return CommonEnum.DBUpdateResult.Success;
                        
                    case (int)CommonEnum.DBUpdateResult.VersionError:
                        return CommonEnum.DBUpdateResult.VersionError;

                    case (int)CommonEnum.DBUpdateResult.OverlapError:
                        return CommonEnum.DBUpdateResult.OverlapError;

                    default:
                        return CommonEnum.DBUpdateResult.Error;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        [HttpPost("deleteReserveFacility")]
        public async Task<CommonEnum.DBUpdateResult> DeleteReserveFacility(TrnReserveFacilityInfo info)
        {
            try
            {
                int result = await _facilityService.Delete(info);

                switch (result)
                {
                    case 1: /*single delete*/
                        return (int)CommonEnum.DBUpdateResult.Success;

                    case (int)CommonEnum.DBUpdateResult.VersionError:
                        return CommonEnum.DBUpdateResult.VersionError;

                    case (int)CommonEnum.DBUpdateResult.UsedError:
                        return CommonEnum.DBUpdateResult.UsedError;

                    default:
                        return CommonEnum.DBUpdateResult.Error;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }
        #endregion

    }
}