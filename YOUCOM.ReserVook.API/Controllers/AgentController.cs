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
    public class AgentController : ControllerBase
    {


        private IAgentService _agentService;

        public AgentController(IAgentService agentService)
        {
            _agentService = agentService;

        }

        // エージェントの情報取得(画面表示,削除用)
        [HttpPost("getlist")]
        public async Task<List<MstAgentInfo>> GetAgentList(MstAgentInfo agentInfo)
        {
            var result = await _agentService.GetAgentList(agentInfo);
            return result;
        }

        // エージェントの情報取得(編集用)
        [HttpPost("getlistbyid")]
        public async Task<MstAgentInfo> GetAgentById(MstAgentInfo agentInfo)
        {
            return await _agentService.GetAgentById(agentInfo);
        }

        // エージェントの情報追加
        [HttpPost("addagent")]
        public async Task<int> AddAgent(MstAgentInfo agentInfo)
        {
            int addFlg = 0;
            try
            {
                var result = await _agentService.AddAgent(agentInfo);
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

        // エージェント情報更新
        [HttpPut("updateagent")]
        public async Task<CommonEnum.DBUpdateResult> UpdateAgent(MstAgentInfo agentInfo)
        {
            try
            {
                bool addFlag = false;
                int result = await _agentService.UpdateAgent(agentInfo, addFlag);
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

        // エージェント情報削除
        [HttpPut("delagent")]
        public async Task<CommonEnum.DBUpdateResult> DelAgent(MstAgentInfo agentInfo)
        {
            try
            {
                int ret = await _agentService.DelAgent(agentInfo);
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

        // エージェント情報削除用画面取得
        [HttpPost("getagentinfo")]
        public async Task<MstAgentInfo> GetAgentInfo(MstAgentInfo agentInfo)
        {
            return await _agentService.GetAgentInfo(agentInfo);
        }

        // 削除チェック
        [HttpPut("deleteagentcheck")]
        public async Task<int> DeleteAgentCheck(MstAgentInfo agentInfo)
        {
            int result = await _agentService.DeleteAgentCheck(agentInfo);
            return result;
        }

        // エージェント取得(サイト変換マスタ用)
        [HttpGet("getagentslist")]
        public async Task<List<MstAgentInfo>> GetAgentList(string companyNo)
        {
            var model = await _agentService.GetAgentDataDid(companyNo);
            return model;

        }
    }
}