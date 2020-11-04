using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TaxServiceController : ControllerBase
    {
        private readonly ILogger<TaxServiceController> _logger;
        private ITaxServiceService _taxServiceService;

        public TaxServiceController(ILogger<TaxServiceController> logger, ITaxServiceService taxServiceService)
        {
            _logger = logger;

            _taxServiceService = taxServiceService;
        }

        [HttpPost("gettaxservicebyPK")]
        public async Task<MstTaxServiceInfo> GetTaxServiceByPK(MstTaxServiceInfo taxServiceInfo)
        {
            return await _taxServiceService.GetTaxServiceByPK(taxServiceInfo);
        }

        [HttpPost("gettaxservicelist")]
        public async Task<List<MstTaxServiceInfo>> GetList(MstTaxServiceInfo taxServiceInfo)
        {
            var result = await _taxServiceService.GetList(taxServiceInfo);
            return result;
        }

        [HttpPost("gettaxservicelistview")]
        public async Task<List<TaxServiceDivisionView>> GetListView(MstTaxServiceInfo taxServiceInfo)
        {
            var result = await _taxServiceService.GetListView(taxServiceInfo);
            return result;
        }
    }
}