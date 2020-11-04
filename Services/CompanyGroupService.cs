using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Services
{
    public class CompanyGroupService : ICompanyGroupService
    {
        private DBContext _context;

        public CompanyGroupService(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 会社グループリスト 取得
        /// </summary>
        /// <returns></returns>
        public async Task<List<MstCompanyGroupInfo>> GetCompanyGroupList()
        {
            var lists = _context.CompanyGroupInfo.Where(w => w.Status != CommonConst.STATUS_UNUSED)
                                                 .OrderBy(o => o.CompanyGroupId)
                                                 .ToList();
            return lists;
        }

        /// <summary>
        /// 会社グループ 取得
        /// </summary>
        /// <param name="key">条件</param>
        /// <returns></returns>
        public async Task<MstCompanyGroupInfo> GetCompanyGroupByPK(string key)
        {
            var info = _context.CompanyGroupInfo.Where(w => w.CompanyGroupId == key)
                                                .SingleOrDefault();
            return info;
        }

        /// <summary>
        /// 会社グループ 追加
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> AddCompanyGroup(MstCompanyGroupInfo info)
        {
            try
            {
                var wkinfo = _context.CompanyGroupInfo.Where(w => w.CompanyGroupId == info.CompanyGroupId)
                                                      .AsNoTracking()
                                                      .SingleOrDefault();

                if (wkinfo == null)
                {
                    // データが存在しなかった場合 → 追加
                    info.Version = 0;
                    info.Cdt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                    info.Udt = info.Cdt;
                    info.Updator = info.Creator;
                    info.Status = Context.CommonConst.STATUS_USED;

                    _context.CompanyGroupInfo.Add(info);
                    _context.SaveChanges();

                    return CommonEnum.DBUpdateResult.Success;

                }
                else if (wkinfo.Status == CommonConst.STATUS_UNUSED)
                {
                    // データが存在し,Statusが「9」場合 → 更新
                    info.Version = info.Version;
                    info.Cdt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                    info.Udt = info.Cdt;
                    info.Updator = info.Creator;
                    info.Status = Context.CommonConst.STATUS_USED;
                    return await UpdateCompanyGroup(info, true);
                }
                else
                {
                    // 重複エラー
                    return CommonEnum.DBUpdateResult.OverlapError;
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// 会社グループ 更新
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> UpdateCompanyGroup(MstCompanyGroupInfo info)
        {
            return await UpdateCompanyGroup(info, false);
        }

        /// <summary>
        /// 会社グループ 更新
        /// </summary>
        /// <param name="info"></param>
        /// <param name="addFlag">True:UnUsedからの復活新規作成時</param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> UpdateCompanyGroup(MstCompanyGroupInfo info, bool addFlag)
        {
            try
            {
                // versionチェック
                if (!addFlag && !await CheckVersion(info)) { return CommonEnum.DBUpdateResult.VersionError; }

                // 更新
                info.Version++;
                info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

                _context.CompanyGroupInfo.Update(info);
                _context.SaveChanges();


                return CommonEnum.DBUpdateResult.Success;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// 会社グループ 削除
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> DeleteCompanyGroup(MstCompanyGroupInfo info)
        {
            try
            {
                // versionチェック
                if (!await CheckVersion(info)) { return CommonEnum.DBUpdateResult.VersionError; }

                // 削除
                info.Version++;
                info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                info.Status = CommonConst.STATUS_UNUSED;

                _context.CompanyGroupInfo.Update(info);
                _context.SaveChanges();

                return (int)CommonEnum.DBUpdateResult.Success;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// バージョンチェック
        /// </summary>
        /// <param name="info">会社グループ情報(Versionはチェック後でカウントアップする)</param>
        /// <returns>True:Ok, False:NG</returns>
        private async Task<bool> CheckVersion(MstCompanyGroupInfo info)
        {
            try
            {
                // データ取得
                var oldInfo = _context.CompanyGroupInfo.Where(w => w.CompanyGroupId == info.CompanyGroupId)
                                                       .AsNoTracking()
                                                       .SingleOrDefault();

                // バージョン差異チェック
                if (info.Version != oldInfo.Version)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

        }

        /// <summary>
        /// 削除前チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> CheckBeforeDelete(MstCompanyGroupInfo info)
        {
            int count = _context.CompanyInfo.Where(w => w.CompanyGroupId == info.CompanyGroupId
                                                     && w.Status == CommonConst.STATUS_USED)
                                            .AsNoTracking()
                                            .Count();

            if(count > 0)
            {
                return CommonEnum.DBUpdateResult.UsedError;
            }
            else
            {
                return CommonEnum.DBUpdateResult.Success;
            }

        }
    }

}
