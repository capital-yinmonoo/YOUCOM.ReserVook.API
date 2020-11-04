using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    //[EnableCors("AllowSpecificOrigin")]
    [Route("[controller]")]
    [ApiController]
    public class WebReserveController : ControllerBase
    {
        private readonly ILogger<WebReserveController> _logger;

        private IWebReserveService _webReserveService;

        public WebReserveController(ILogger<WebReserveController> logger, IWebReserveService getWebReserveBaseService)
        {
            _logger = logger;
            _webReserveService = getWebReserveBaseService;
        }

        [HttpPost("getwebreservelist")]
        public async Task<List<WebReserveBaseInfo>> getWebReserveBaseList(WebReserveBaseInfo cond)
        {
            var model = await _webReserveService.GetWebReserveBaseList(cond);
            return model;
        }

        [HttpPost("getwebreserveinfo")]
        public async Task<WebReserveInfo> getWebreserveInfo(WebReserveBaseInfo cond)
        {
            var model = await _webReserveService.GetWebReserveById(cond);
            return model;
        }
    }
}
