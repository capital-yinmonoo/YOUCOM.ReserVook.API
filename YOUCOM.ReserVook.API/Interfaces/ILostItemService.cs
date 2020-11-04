using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Interfaces {
    public interface ILostItemDetailService {

        Task<LostItemDetailInfo> GetlostItemByPK(LostItemDetailInfo lostItemModel);

        Task<LostItemsPictureInfo> GetlostItemImage(LostItemsPictureInfo lostItemModel);

        Task<string> AddLostItem(LostItemDetailInfo lostItemModel);

        Task<CommonEnum.DBUpdateResult> UpdateLostItem(LostItemDetailInfo lostItemModel);

        Task<CommonEnum.DBUpdateResult> UpdateImage(string companyNo, string managementNo, IFormFileCollection files);

    }
}
