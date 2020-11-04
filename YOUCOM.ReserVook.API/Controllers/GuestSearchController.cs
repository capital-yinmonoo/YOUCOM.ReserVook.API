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
    [Route("[controller]")]
    [ApiController]
    public class GuestSearchController : ControllerBase
    {

        private INameFileService _nameFileService;

        public GuestSearchController(INameFileService nameFileService)
        {
            _nameFileService = nameFileService;

        }

        // エージェントの情報取得(画面表示,削除用)
        [HttpPost("getGuestInfoList")]
        public async Task<List<GuestInfo>> GetGuestInfoList(GuestInfo cond)
        {
            var result = await _nameFileService.GetGuestInfoList(cond);
            return result;
        }
    }
}