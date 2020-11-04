using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.Commons.Extensions;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Services {
    
    public class LostItemDetailService : ILostItemDetailService {

        private DBContext _context;

        public LostItemDetailService(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 管理番号 採番
        /// </summary>
        private string Numbering(string companyNo)
        {
            var info = new TrnLostItemsBaseInfo();

            var command = _context.Database.GetDbConnection().CreateCommand();

            // 採番
            string sql = "Select to_char(to_number(max(management_no), '99999999') + 1, 'FM99999999') as management_no";
            sql += " From trn_lost_items_base";
            sql += " Where company_no = '{0}'";
            sql += " And management_no like '{1}'";

            var yearMonth = DateTime.Now.ToString("yyMM");
            command.CommandText = sql.FillFormat(companyNo, yearMonth + "%");
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    var no = reader["management_no"].ToString();
                    if (no.IsNullOrEmpty()) no = yearMonth + "1".PadLeft(4, '0');
                    return no;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            finally
            {
                _context.Database.CloseConnection();
            }
        }

        /// <summary>
        /// 忘れ物管理写真登録状況
        /// </summary>
        /// <param name="companyNo">会社番号</param>
        /// <param name="managementNo">管理番号</param>
        /// <returns>0以上: 登録済みの件数, -1:エラー</returns>
        private int PictureResistState(string companyNo, string managementNo)
        {

            var command = _context.Database.GetDbConnection().CreateCommand();

            string sql = "Select count(*) as count";
            sql += " From trn_lost_items_picture";
            sql += " Where company_no = {0}";
            sql += "  And management_no = {1}";

            command.CommandText = sql.FillFormat(companyNo, managementNo);
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    return reader["count"].ToString().ToInt_Or_Zero();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -1;
            }
            finally
            {
                _context.Database.CloseConnection();
            }
        }

       /// <summary>
        /// 忘れ物管理基本データ取得
        /// </summary>
        /// <param name="lostItemModel"></param>
        /// <returns>取得した値</returns>
        public async Task<LostItemDetailInfo> GetlostItemByPK(LostItemDetailInfo lostItemModel)
        {

            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT *";
            sql += " FROM trn_lost_items_base";
            sql += " WHERE company_no = '" + lostItemModel.CompanyNo + "'";
            sql += " AND management_no = '" + lostItemModel.ManagementNo + "'";
            command.CommandText = sql;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    var info = new LostItemDetailInfo();
                    if (reader.Read() == true)
                    {
                        info.ItemState = reader["item_state"].ToString();
                        info.ItemCategory = reader["item_category"].ToString();
                        info.ItemName = reader["item_name"].ToString();
                        info.FoundDate = reader["found_date"].ToString();
                        info.FoundTime = reader["found_time"].ToString();
                        info.FoundPlace = reader["found_place"].ToString();
                        info.Comment = reader["comment"].ToString();
                        info.SearchWord = reader["search_word"].ToString();
                        info.FoundPlaceCode = reader["found_place_code"].ToString();
                        info.StorageCode = reader["storage_code"].ToString();
                        info.ReserveNo = reader["reserve_no"].ToString();
                        info.RoomNo = reader["room_no"].ToString();
                        info.Status = reader["status"].ToString();
                        info.Version = int.Parse(reader["version"].ToString());
                        info.Creator = reader["creator"].ToString();
                        info.Updator = reader["updator"].ToString();
                        info.Cdt = reader["cdt"].ToString();
                        info.Udt = reader["udt"].ToString();
                    }

                    return info;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            finally
            {
                _context.Database.CloseConnection();
            }

        }

        /// <summary>
        /// 忘れ物管理基本データ取得
        /// </summary>
        /// <param name="lostItemModel"></param>
        /// <returns>取得した値</returns>
        public async Task<LostItemsPictureInfo> GetlostItemImage(LostItemsPictureInfo lostItemModel)
        {

            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT *";
            sql += " FROM trn_lost_items_picture";
            sql += " WHERE company_no = '" + lostItemModel.CompanyNo + "'";
            sql += " AND management_no = '" + lostItemModel.ManagementNo + "'";
            sql += " AND file_seq = '" + lostItemModel.FileSeq + "'";
            command.CommandText = sql;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    var info = new LostItemsPictureInfo();
                    if (reader.Read() == true)
                    {
                        info.ContentType = reader["content_type"].ToString();
                        info.FileName = reader["file_name"].ToString();

                        if (reader["binary_data"] is DBNull)
                        {

                        } else
                        {
                            info.BinaryData = (byte[])reader["binary_data"];
                        }

                        info.Status = reader["status"].ToString();
                        info.Version = int.Parse(reader["version"].ToString());
                        info.Creator = reader["creator"].ToString();
                        info.Updator = reader["updator"].ToString();
                        info.Cdt = reader["cdt"].ToString();
                        info.Udt = reader["udt"].ToString();
                    }

                    return info;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            finally
            {
                _context.Database.CloseConnection();
            }
        }

        /// <summary>
        /// 新規登録
        /// </summary>
        /// <param name="reserveModel"></param>
        /// <returns>false:正常終了, true:異常終了</returns>
        public async Task<string> AddLostItem(LostItemDetailInfo lostItemModel)
        {
            // 管理番号 採番
            lostItemModel.ManagementNo = Numbering(lostItemModel.CompanyNo);
            lostItemModel.Version = 0;
            lostItemModel.Cdt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
            lostItemModel.Udt = lostItemModel.Cdt;
            lostItemModel.Status = Context.CommonConst.STATUS_USED;

            // トランザクション作成
            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                    // 忘れ物管理基本 新規登録
                    _context.LostItemsBaseInfo.Add(lostItemModel);
                    _context.SaveChanges();

                    //忘れ物管理写真
                    var pict = new TrnLostItemsPictureInfo();
                    pict.CompanyNo = lostItemModel.CompanyNo;
                    pict.ManagementNo = lostItemModel.ManagementNo;
                    pict.FileSeq = 1;
                    pict.Status = Context.CommonConst.STATUS_USED;
                    pict.Version = 0;
                    pict.Creator = lostItemModel.Creator;
                    pict.Updator = lostItemModel.Updator;
                    pict.Cdt = lostItemModel.Cdt;
                    pict.Udt = lostItemModel.Udt;
                        
                    // 忘れ物管理写真 新規登録
                    _context.LostItemsPictureInfo.Add(pict);
                    _context.SaveChanges();

                    tran.Commit();

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                    tran.Rollback();
                    return null;
                } // tran
            }

            return lostItemModel.ManagementNo;
        }

/// <summary>
/// 更新
/// </summary>
/// <param name="lostItemModel"></param>
/// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> UpdateLostItem(LostItemDetailInfo lostItemModel)
        {
            lostItemModel.Udt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

            try
            {
                lostItemModel.FoundTime = lostItemModel.FoundTime.Replace(":", "");

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction())
                {
                    var version = _context.LostItemsBaseInfo
                                            .Where(x => x.CompanyNo == lostItemModel.CompanyNo
                                                     && x.ManagementNo == lostItemModel.ManagementNo)
                                            .Select(s => s.Version)
                                            .SingleOrDefault();

                    // 排他チェック
                    if (version != lostItemModel.Version)
                    {
                        return CommonEnum.DBUpdateResult.VersionError;
                    }

                    // 忘れ物基本
                    _context.LostItemsBaseInfo.Update(lostItemModel);
                    _context.SaveChanges();

                    //忘れ物管理写真
                    var pict = new TrnLostItemsPictureInfo();
                    pict.CompanyNo = lostItemModel.CompanyNo;
                    pict.ManagementNo = lostItemModel.ManagementNo;
                    pict.FileSeq = 1;
                    pict.Status = Context.CommonConst.STATUS_USED;
                    pict.Updator = lostItemModel.Updator;
                    pict.Udt = lostItemModel.Udt;

                    // 忘れ物管理写真 新規登録
                    _context.LostItemsPictureInfo.Update(pict);
                    _context.SaveChanges();

                    tran.Commit();
                    return CommonEnum.DBUpdateResult.Success;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return CommonEnum.DBUpdateResult.Error;
            }
            finally
            {
                _context.Database.CloseConnection();
            }
        }

        /// <summary>
        /// イメージ更新
        /// </summary>
        /// <param name="companyNo"></param>
        /// <param name="ManagementNo"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> UpdateImage(string companyNo, string ManagementNo, IFormFileCollection files)
        {
            try
            {
                // 忘れ物写真
                foreach (var pict in files)
                {
                    var info = _context.LostItemsPictureInfo
                                            .Where(x => x.CompanyNo == companyNo
                                                     && x.ManagementNo == ManagementNo
                                                     && x.FileSeq == 1).Single();

                    info.ContentType = pict.ContentType;
                    info.FileName = pict.FileName;
                    info.Version += 1;

                    // image => byte[] 変換
                    byte[] fileBytes;
                    using (var ms = new MemoryStream())
                    {
                        await pict.CopyToAsync(ms);
                        fileBytes = ms.ToArray();

                    }
                    info.BinaryData = fileBytes;

                    // 忘れ物管理写真 変更
                    _context.LostItemsPictureInfo.Update(info);
                    _context.SaveChanges();

                    break;
                }

                return CommonEnum.DBUpdateResult.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
            finally
            {
                _context.Database.CloseConnection();
            }
        }

    }
}