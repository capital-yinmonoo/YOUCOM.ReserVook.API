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
    [ApiController]
    [Route("[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly ILogger<BookingsController> _logger;
        private IRoomsService _roomsService;

        public BookingsController(ILogger<BookingsController> logger, IRoomsService roomsService)
        {
            _logger = logger;

            _roomsService = roomsService;
        }

        /// <summary>
        /// 連泊状況 データ取得
        /// </summary>
        /// <param name="cond">条件</param>
        /// <returns></returns>
        [HttpPost("getbookingslist")]
        public async Task<List<BookingsInfo>> GetBookingsList(BookingsCondition cond)
        {
            return await _roomsService.GetBookingsList(cond);
        }

    }
}