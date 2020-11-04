using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.Commons.Extensions;

namespace YOUCOM.ReserVook.API.Services
{
    public class CompanyService : ICompanyService
    {
        private DBContext _context;

        public CompanyService(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 会社マスタ 論理削除
        /// </summary>
        /// <param name="companyInfo">会社情報</param>
        /// <returns></returns>
        public async Task<int> DeleteCompany(MstCompanyInfo companyInfo)
        {
            try
            {
                // versionチェック
                if (!await CheckVersion(companyInfo)) { return -1; }

                companyInfo.Version++;
                companyInfo.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                companyInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.CompanyInfo.Update(companyInfo);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 会社マスタ 更新
        /// </summary>
        /// <param name="companyInfo">会社情報</param>
        /// <returns></returns>
        public async Task<int> UpdateCompany(MstCompanyInfo companyInfo)
        {
            try
            {
                // versionチェック
                if (!await CheckVersion(companyInfo)) { return -1; }

                // データ取得
                var info = await GetCompanyByPK(companyInfo.CompanyNo, true);

                companyInfo.Version++;
                companyInfo.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                companyInfo.LogoData = info.LogoData;
                companyInfo.ContentType = info.ContentType;
                companyInfo.MaxCapacity = info.MaxCapacity;
                companyInfo.CompanyGroupId.NullToEmpty();

                _context.CompanyInfo.Update(companyInfo);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -99;
            }
        }

        /// <summary>
        /// 会計書ロゴ 更新
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<int> UpdateImage(IFormFileCollection files, string companyNo)
        {
            try
            {
                // キーチェック
                if (companyNo == null || companyNo.Length == 0) { return -2; }

                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {

                        // ファイル種別チェック
                        if (!file.ContentType.StartsWith("image")) { return -3; }

                        // image => byte[] 変換
                        byte[] fileBytes;
                        using (var ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            fileBytes = ms.ToArray();

                        }

                        // 更新
                        var companyInfo = await GetCompanyByPK(companyNo, true);
                        if (companyInfo == null) { return -4; }
                        companyInfo.LogoData = fileBytes;
                        companyInfo.ContentType = file.ContentType;

                        _context.CompanyInfo.Update(companyInfo);
                        return _context.SaveChanges();

                    }

                }

                // ファイル無し
                return -5;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 会社マスタ 追加
        /// </summary>
        /// <param name="companyInfo">会社情報</param>
        /// <returns></returns>
        public async Task<int> AddCompany(MstCompanyInfo companyInfo)
        {
            companyInfo.LostFlg = companyInfo.LostFlg.IsBlanks() ? "0" : companyInfo.LostFlg;
            companyInfo.TrustyouConnectDiv = companyInfo.TrustyouConnectDiv.IsBlanks() ? "0" : companyInfo.TrustyouConnectDiv;
            companyInfo.CompanyGroupId.NullToEmpty();
            companyInfo.Version = 0;
            companyInfo.Cdt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
            companyInfo.Udt = companyInfo.Cdt;
            companyInfo.Status = Context.CommonConst.STATUS_USED;

            _context.CompanyInfo.Add(companyInfo);
            return _context.SaveChanges();
        }

        /// <summary>
        /// 会社マスタ 取得
        /// </summary>
        /// <param name="companyNo">会社番号</param>
        /// <returns></returns>
        public async Task<MstCompanyInfo> GetCompanyByPK(string companyNo)
        {
            return await GetCompanyByPK(companyNo, false);
        }

        /// <summary>
        /// 会社マスタ 取得
        /// </summary>
        /// <param name="companyNo">会社番号</param>
        /// <param name="notracking">True: AsNoTrackingMode False:TackingMode </param>
        /// <returns></returns>
        public async Task<MstCompanyInfo> GetCompanyByPK(string companyNo, bool notracking)
        {
            MstCompanyInfo company;
            if (notracking)
            {
                company = _context.CompanyInfo.AsNoTracking().SingleOrDefault(c => c.CompanyNo == companyNo);
            }
            else
            {
                company = _context.CompanyInfo.SingleOrDefault(c => c.CompanyNo == companyNo);
            }

            // 名称を補完
            if (company.CompanyGroupId.IsNotBlanks())
            {
                company.CompanyGroupName = _context.CompanyGroupInfo.Where(w => w.CompanyGroupId == company.CompanyGroupId).Select(s => s.CompanyGroupName).SingleOrDefault();
            }

            return company;
        }

        /// <summary>
        /// 会社マスタ 一覧取得
        /// </summary>
        /// <returns></returns>
        public async Task<List<MstCompanyInfo>> GetList() {
            /// 一覧を取得
            var lstCompanyInfo = _context.CompanyInfo.Where(w => w.Status != CommonConst.STATUS_UNUSED).OrderBy(c => c.CompanyNo).ToList();

            /// 名称を補完
            foreach (var info in lstCompanyInfo) {
                try {
                    MstCodeNameInfo codeNameInfo = _context.CodeNameInfo.Where(x => x.CompanyNo == info.CompanyNo
                                                                && x.DivisionCode == ((int)CommonEnum.CodeDivision.CleanRoomsManager).ToString(CommonConst.CODE_DIVISION_FORMAT)
                                                                && x.Code == info.LostFlg).AsNoTracking().SingleOrDefault();
                    if (codeNameInfo is null) {
                    } else {
                        info.LostFlgName = codeNameInfo.CodeName;
                    }

                    codeNameInfo = _context.CodeNameInfo.Where(x => x.CompanyNo == info.CompanyNo
                                                                && x.DivisionCode == ((int)CommonEnum.CodeDivision.TrustyouConnectDiv).ToString(CommonConst.CODE_DIVISION_FORMAT)
                                                                && x.Code == info.TrustyouConnectDiv).AsNoTracking().SingleOrDefault();

                    if (codeNameInfo is null) {
                    } else {
                        info.TrustyouConnectDivName = codeNameInfo.CodeName;
                    }

                    if (info.CompanyGroupId.IsNotBlanks())
                    {
                        var compnayGroupInfo = _context.CompanyGroupInfo.Where(x => x.CompanyGroupId == info.CompanyGroupId).AsNoTracking().SingleOrDefault();
                        if (compnayGroupInfo != null)
                        {
                            info.CompanyGroupName = compnayGroupInfo.CompanyGroupName;
                        }
                    }

                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            return lstCompanyInfo;
        }

        /// <summary>
        /// 会社マスタ 一覧取得
        /// </summary>
        /// <returns></returns>
        public async Task<List<MstCompanyInfo>> GetCompanyListByCompanyGroupId(string companyGroupId)
        {
            try
            {
                // 一覧を取得
                var lstCompanyInfo = _context.CompanyInfo.Where(w => w.Status != CommonConst.STATUS_UNUSED
                                                                  && w.CompanyGroupId == companyGroupId)
                                                         .OrderBy(c => c.CompanyNo)
                                                         .ToList();

                return lstCompanyInfo;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// サービス料率取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<decimal> GetServiceRate(BaseInfo cond)
        {
            return _context.CompanyInfo.Where(c => c.CompanyNo == cond.CompanyNo)
                                       .Select(s => s.ServiceRate)
                                       .SingleOrDefault();
        }

        /// <summary>
        /// バージョンチェック
        /// </summary>
        /// <param name="companyInfo">会社情報(Versionはチェック後でカウントアップする)</param>
        /// <returns>True:Ok, False:NG</returns>
        private async Task<bool> CheckVersion(MstCompanyInfo companyInfo)
        {
            try
            {
                // データ取得
                var info = await GetCompanyByPK(companyInfo.CompanyNo, true);

                // バージョン差異チェック
                if (companyInfo.Version != info.Version)
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
    }
}
