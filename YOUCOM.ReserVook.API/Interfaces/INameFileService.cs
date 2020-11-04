using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces
{
    public interface INameFileService
    {
        Task<List<GuestInfo>> GetGuestInfoList(GuestInfo cond);

    }
}
