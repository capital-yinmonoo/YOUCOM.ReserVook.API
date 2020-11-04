using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TaxRateController : ControllerBase
    {
        private readonly ILogger<TaxRateController> _logger;
        private ITaxRateService _taxRateService;

        public TaxRateController(ILogger<TaxRateController> logger, ITaxRateService taxRateService)
        {
            _logger = logger;

            _taxRateService = taxRateService;
        }

        //[HttpPost("getTaxRateByPK")]
        //public async Task<MstTaxRateInfo> GetTaxRateByPK(MstTaxRateInfo taxRateInfo)
        //{
        //    return await _taxRateService.GetTaxRateByPK(taxRateInfo);
        //}

        [HttpPost("getTaxRateList")]
        public async Task<List<MstTaxRateInfo>> GetList(BaseInfo cond)
        {
            var result = await _taxRateService.GetList(cond);
            return result;
        }

    }
}