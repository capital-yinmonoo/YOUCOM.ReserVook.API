using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.Commons.Extensions;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Services
{
    public class FacilityService : IFacilityService
    {

        private DBContext _context;

        public FacilityService(DBContext context)
        {
            _context = context;
        }

        #region マスタ系
        /// <summary>
        /// 会場マスタ 追加
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> MasterAdd(MstFacilityInfo info)
        {
            var facilityInfo = _context.FacilityInfo.Where(w => w.CompanyNo == info.CompanyNo && w.FacilityCode == info.FacilityCode).AsNoTracking().SingleOrDefault();

            if (facilityInfo == null)
            {
                // データが存在しなかった場合 → 追加
                info.Version = 0;
                info.Cdt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                info.Udt = info.Cdt;
                info.Status = Context.CommonConst.STATUS_USED;

                _context.FacilityInfo.Add(info);
                return _context.SaveChanges();
            }
            else if (facilityInfo.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」場合 → 更新
                info.Version = facilityInfo.Version;
                info.Cdt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                info.Status = Context.CommonConst.STATUS_USED;
                return await MasterUpdate(info, true);
            }
            else
            {
                // 重複エラー
                return (int)CommonEnum.DBUpdateResult.OverlapError;
            }

        }

        /// <summary>
        /// 削除時チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns>
        /// 0:削除可
        /// 1:削除確認
        /// -1:削除不可(本予約かつ未精算)
        /// </returns>
        public async Task<int> MasterCheckDelete(MstFacilityInfo info)
        {
            try
            {
                // データ取得
                var trnReserveFacilities = _context.ReserveFacilityInfo.Where(w => w.CompanyNo == info.CompanyNo && w.FacilityCode == info.FacilityCode).AsNoTracking().ToList();

                if (trnReserveFacilities != null && trnReserveFacilities.Count > 0)
                {
                    // 削除不可(本予約かつ未精算)
                    return -1;
                }
                else
                {
                    // 削除可
                    return 0;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -99;
            }
        }

        /// <summary>
        /// 会場マスタ 削除
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> MasterDelete(MstFacilityInfo info)
        {
            try
            {
                // versionチェック
                if (!await MasterCheckVersion(info)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                info.Version++;
                info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                info.Status = Context.CommonConst.STATUS_UNUSED;

                _context.FacilityInfo.Update(info);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 会場マスタ 取得
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<MstFacilityInfo> MasterGetByPK(MstFacilityInfo info)
        {
            try
            {
                return _context.FacilityInfo.Where(w => w.CompanyNo == info.CompanyNo && w.FacilityCode == info.FacilityCode && w.Status != CommonConst.STATUS_UNUSED)
                            .AsNoTracking().SingleOrDefault();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 会場マスタ 一覧取得
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<List<MstFacilityInfo>> MasterGetList(MstFacilityInfo info)
        {
            try
            {
                return _context.FacilityInfo.Where(w => w.CompanyNo == info.CompanyNo && w.Status != CommonConst.STATUS_UNUSED)
                            .OrderBy(o => o.DisplayOrder).ThenBy(n => n.FacilityCode).ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 会場マスタ 更新
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> MasterUpdate(MstFacilityInfo info)
        {
            return await MasterUpdate(info, false);
        }

        /// <summary>
        /// 会場マスタ 更新
        /// </summary>
        /// <param name="info"></param>
        /// <param name="addFlag">上書き追加</param>
        /// <returns></returns>
        public async Task<int> MasterUpdate(MstFacilityInfo info, bool addFlag)
        {
            try
            {
                // versionチェック
                if (!addFlag && !await MasterCheckVersion(info)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                info.Version++;
                info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

                _context.FacilityInfo.Update(info);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (int)CommonEnum.DBUpdateResult.Error;
            }

        }

        /// <summary>
        /// バージョンチェック
        /// </summary>
        /// <param name="info">会場情報(Versionはチェック後でカウントアップする)</param>
        /// <returns>True:Ok, False:NG</returns>
        private async Task<bool> MasterCheckVersion(MstFacilityInfo info)
        {
            try
            {
                // キーセット
                MstFacilityInfo keyinfo = new MstFacilityInfo() { CompanyNo = info.CompanyNo, FacilityCode = info.FacilityCode };

                // データ取得
                var facilityInfo = await MasterGetByPK(keyinfo);

                // バージョン差異チェック
                if (facilityInfo.Version != info.Version)
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
        #endregion

        #region 会場状況
        /// <summary>
        /// 予約会場 取得
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<TrnReserveFacilityInfo> GetByPK(TrnReserveFacilityInfo info)
        {
            try
            {
                return _context.ReserveFacilityInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                               w.FacilityCode == info.FacilityCode &&
                                               w.UseDate == info.UseDate &&
                                               w.FacilitySeq == info.FacilitySeq)
                                   .AsNoTracking().SingleOrDefault();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 予約会場 一覧取得
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<List<TrnReserveFacilityInfo>> GetReserveFacilityList(FacilityCondition cond)
        {
            string sql = "SELECT facility.*, name.guest_name";
            sql += " FROM trn_reserve_facility facility";
            sql += " LEFT JOIN trn_reserve_basic basic";
            sql += " ON basic.company_no =  facility.company_no";
            sql += " AND basic.reserve_no = facility.reserve_no";
            sql += " LEFT JOIN trn_name_file name";
            sql += " ON basic.company_no =  name.company_no";
            sql += " AND basic.reserve_no = name.reserve_no";
            sql += " AND name.use_date = '" + CommonConst.USE_DATE_EMPTY + "'";
            sql += " AND name.route_seq = '" + CommonConst.DEFAULT_ROUTE_SEQ + "'";
            sql += " WHERE facility.company_no = '" + cond.CompanyNo + "'";
            sql += " AND facility.use_date = '" + cond.UseDate + "'";

            var list = new List<TrnReserveFacilityInfo>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _context.Database.OpenConnection();

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var facilityInfo = new TrnReserveFacilityInfo();
                            facilityInfo.CompanyNo = reader["company_no"].ToString();
                            facilityInfo.FacilityCode = reader["facility_code"].ToString();
                            facilityInfo.UseDate = reader["use_date"].ToString();
                            facilityInfo.ReserveNo = reader["reserve_no"].ToString();
                            facilityInfo.FacilitySeq = reader["facility_seq"].ToString().ToInt_Or_Zero();
                            facilityInfo.StartTime = reader["start_time"].ToString();
                            facilityInfo.EndTime = reader["end_time"].ToString();
                            facilityInfo.FacilityMember = reader["facility_member"].ToString().ToInt_Or_Zero();
                            facilityInfo.FacilityRemarks = reader["facility_remarks"].ToString();

                            facilityInfo.Status = reader["status"].ToString();
                            facilityInfo.Version = int.Parse(reader["version"].ToString());
                            facilityInfo.Creator = reader["creator"].ToString();
                            facilityInfo.Updator = reader["updator"].ToString();
                            facilityInfo.Cdt = reader["cdt"].ToString();
                            facilityInfo.Udt = reader["udt"].ToString();

                            facilityInfo.GuestName = reader["guest_name"].ToString();
                            facilityInfo.OrgFacilityCode = facilityInfo.FacilityCode;

                            list.Add(facilityInfo);
                        }
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    _context.Database.CloseConnection();
                }
            }

            return list;
        }

        /// <summary>
        /// 予約会場 更新
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> Update(TrnReserveFacilityInfo info)
        {
            try
            {
                // versionチェック
                if (!await CheckVersion(info)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                // 時間帯重複のチェック
                if (await CheckTimeOverLap(info)) { return (int)CommonEnum.DBUpdateResult.OverlapError; }

                info.Version++;
                info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

                //トランザクション作成
                using (var tran = _context.Database.BeginTransaction())
                {
                    try
                    {
                        if (info.FacilityCode != info.OrgFacilityCode)
                        {
                            // 会場の変更あり、Key更新なのでDelete->Insert
                            var workInfo = info.Clone();
                            workInfo.FacilityCode = info.OrgFacilityCode;

                            // seq採番
                            int seq = 1;
                            var list = _context.ReserveFacilityInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                               w.FacilityCode == info.FacilityCode &&
                                                                               w.UseDate == info.UseDate)
                                                                   .AsNoTracking()
                                                                   .ToList();

                            if (list != null && list.Count != 0) { 
                                seq = list.Max(m => m.FacilitySeq) + 1;
                            }
                            info.FacilitySeq = seq;

                            _context.ReserveFacilityInfo.Remove(workInfo);
                            _context.ReserveFacilityInfo.Add(info);

                            if (_context.SaveChanges() == 2)
                            {
                                tran.Commit();
                                return 1;
                            }
                            else
                            {
                                throw new Exception("Version Error");
                            }
                        }
                        else
                        {
                            // 会場の変更なし、Update
                            _context.ReserveFacilityInfo.Update(info);
                            if (_context.SaveChanges() == 1)
                            {
                                tran.Commit();
                                return 1;
                            }
                            else
                            {
                                throw new Exception("Version Error");
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (int)CommonEnum.DBUpdateResult.Error;
            }

        }

        /// <summary>
        /// 予約会場 追加
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> Add(TrnReserveFacilityInfo info)
        {
            try
            {
                // 時間帯重複のチェック
                if (await CheckTimeOverLap(info)){ return (int)CommonEnum.DBUpdateResult.OverlapError; }

                // seq採番
                var list = _context.ReserveFacilityInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                  w.FacilityCode == info.FacilityCode &&
                                                                  w.UseDate == info.UseDate)
                                                       .AsNoTracking()
                                                       .ToList();
                
                int seq = 1;
                if (list != null && list.Count != 0) { seq = list.Max(m => m.FacilitySeq) + 1; }

                info.FacilitySeq = seq;
                info.Version = 0;
                info.Cdt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                info.Udt = info.Cdt;
                info.Updator = info.Creator;
                info.Status = Context.CommonConst.STATUS_USED;

                _context.ReserveFacilityInfo.Add(info);
                return _context.SaveChanges();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (int)CommonEnum.DBUpdateResult.Error;
            }

        }

        /// <summary>
        /// 予約会場 削除
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> Delete(TrnReserveFacilityInfo info)
        {
            try
            {
                // versionチェック
                if (!await CheckVersion(info)) { return (int)CommonEnum.DBUpdateResult.VersionError; }

                _context.ReserveFacilityInfo.Remove(info);
                return _context.SaveChanges();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (int)CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// バージョンチェック
        /// </summary>
        /// <param name="info">予約会場(Versionはチェック後でカウントアップする)</param>
        /// <returns>True:Ok, False:NG</returns>
        private async Task<bool> CheckVersion(TrnReserveFacilityInfo info)
        {
            try
            {
                // キーセット
                TrnReserveFacilityInfo keyinfo = new TrnReserveFacilityInfo() {
                    CompanyNo = info.CompanyNo,
                    FacilityCode = info.OrgFacilityCode,
                    UseDate = info.UseDate,
                    FacilitySeq = info.FacilitySeq
                };

                // データ取得
                var reserveFacilityInfo = await GetByPK(keyinfo);

                // バージョン差異チェック
                if (reserveFacilityInfo.Version != info.Version)
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
        /// 時間帯重複チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns>True:重複有 False:重複なし</returns>
        private async Task<bool> CheckTimeOverLap(TrnReserveFacilityInfo info)
        {
            try
            {
                // 利用時間帯に別予約が入っているか取得
                var list = _context.ReserveFacilityInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                   w.FacilityCode == info.FacilityCode &&
                                                                   w.UseDate == info.UseDate &&
                                                                   ((w.StartTime.CompareTo(info.StartTime) > 0 && w.StartTime.CompareTo(info.EndTime) < 0) ||
                                                                   (w.EndTime.CompareTo(info.StartTime) > 0 && w.EndTime.CompareTo(info.EndTime) < 0) ||
                                                                   (w.StartTime.CompareTo(info.StartTime) < 0 && w.EndTime.CompareTo(info.EndTime) > 0)) && 
                                                                   w.FacilitySeq != info.FacilitySeq)
                                                       .AsNoTracking()
                                                       .ToList();

                if (list != null && list.Count > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return true;
            }
        }

        #endregion

    }
}