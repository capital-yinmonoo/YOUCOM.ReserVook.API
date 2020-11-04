using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    //[EnableCors("AllowSpecificOrigin")]
    [Route("[controller]")]
    [ApiController]
    public class ScNameController : ControllerBase
    {


        private IScNameService _scNameService;


        public ScNameController(IScNameService scNameService)
        {
            _scNameService = scNameService;

        }

        // SCサイトコード名の取得
        [HttpGet("getScNameList")]
        public async Task<List<FrMScNmInfo>> GetScNameList(string companyNo)
        {
            var model = await _scNameService.GetListScName(companyNo);
            return model;

        }

        // 情報取得
        [HttpPost("getList")]
        public async Task<List<FrMScNmInfo>> GetList(FrMScNmInfo cond)
        {
            var result = await _scNameService.GetList(cond);
            return result;
        }

    }
}