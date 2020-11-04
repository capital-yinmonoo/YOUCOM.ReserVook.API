using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    //[EnableCors("AllowSpecificOrigin")]
    [Route("[controller]")]
    [ApiController]
    public class NameSearchController : ControllerBase
    {
        private readonly ILogger<NameSearchController> _logger;

        private INameSearchService _nameSearchService;

        public NameSearchController(ILogger<NameSearchController> logger, INameSearchService getNameSearchService)
        {
            _logger = logger;
            _nameSearchService = getNameSearchService;
        }

        [HttpPost("getnamesearchlist")]
        public async Task<List<NameSearchInfo>> getNameSearchList(NameSearchCondition cond)
        {
            var model = await _nameSearchService.GetNameSearchList(cond);
            return model;
        }
    }
}
