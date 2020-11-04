using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;
using YOUCOM.ReserVook.API.Models;

namespace YOUCOM.ReserVook.API.Services
{
    public class SetItemService : ISetItemService
    {
        private DBContext _context;

        public SetItemService(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// セット商品(親)リスト 取得
        /// </summary>
        /// <param name="cond">条件</param>
        /// <returns></returns>
        public async Task<List<MstItemInfo>> GetSetItemParentList(BaseInfo cond)
        {
            var lists = _context.ItemInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                     w.ItemDivision == ((int)CommonEnum.ItemDivision.SetItem).ToString() &&
                                                     w.Status != CommonConst.STATUS_UNUSED)
                                         .OrderBy(o => o.DisplayOrder).ThenBy(n => n.ItemKana)
                                         .ToList();
            return lists;
        }

        /// <summary>
        /// セット商品(親+子リスト) 取得
        /// </summary>
        /// <param name="cond">条件</param>
        /// <returns></returns>
        public async Task<SetItemInfo> GetSetItemByPK(MstItemInfo cond)
        {

            var SetItemInfo = new SetItemInfo();

            // セット商品 親情報取得
            var ParentItemInfo = _context.ItemInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                    w.ItemCode == cond.ItemCode &&
                                                    w.Status != CommonConst.STATUS_UNUSED)
                                          .SingleOrDefault();

            // セット商品 子情報取得
            var ChildItemList = _context.SetItemInfo.Where(w => w.CompanyNo == cond.CompanyNo &&
                                                                w.SetItemCode == cond.ItemCode &&
                                                                w.Status != CommonConst.STATUS_UNUSED)
                                                   .ToList();

            // セット商品 親情報
            SetItemInfo.CompanyNo = ParentItemInfo.CompanyNo;
            SetItemInfo.ItemCode = ParentItemInfo.ItemCode;
            SetItemInfo.ItemDivision = ParentItemInfo.ItemDivision;
            SetItemInfo.MealDivision = ParentItemInfo.MealDivision;
            SetItemInfo.ItemName = ParentItemInfo.ItemName;
            SetItemInfo.ItemKana = ParentItemInfo.ItemKana;
            SetItemInfo.PrintName = ParentItemInfo.PrintName;
            SetItemInfo.UnitPrice = ParentItemInfo.UnitPrice;
            SetItemInfo.TaxDivision = ParentItemInfo.TaxDivision;
            SetItemInfo.TaxrateDivision = ParentItemInfo.TaxrateDivision;
            SetItemInfo.ServiceDivision = ParentItemInfo.ServiceDivision;
            SetItemInfo.DisplayOrder = ParentItemInfo.DisplayOrder;
            SetItemInfo.Status = ParentItemInfo.Status;
            SetItemInfo.Version = ParentItemInfo.Version;
            SetItemInfo.Creator = ParentItemInfo.Creator;
            SetItemInfo.Updator = ParentItemInfo.Updator;
            SetItemInfo.Cdt = ParentItemInfo.Cdt;
            SetItemInfo.Udt = ParentItemInfo.Udt;

            // セット商品 子情報
            if(ChildItemList != null && ChildItemList.Count > 0)
            {
                // 子情報に商品マスタから付加情報を取得
                foreach (var item in ChildItemList)
                {
                    var itemInfo = _context.ItemInfo.Where(w => w.CompanyNo == item.CompanyNo &&
                                                                w.ItemCode == item.ItemCode &&
                                                                w.Status != CommonConst.STATUS_UNUSED)
                                                    .AsNoTracking()
                                                    .SingleOrDefault();

                    item.BaseItemInfo = itemInfo; 
                }

                SetItemInfo.ChildItems = ChildItemList;
            }
            else
            {
                SetItemInfo.ChildItems = new List<MstSetItemInfo>();
            }

            return SetItemInfo;

        }

        /// <summary>
        /// セット商品(親+子リスト) 追加
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> AddSetItem(SetItemInfo info)
        {
            try
            {

                var wkinfo = _context.ItemInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                          w.ItemCode == info.ItemCode)
                                              .AsNoTracking()
                                              .SingleOrDefault();

                if(wkinfo == null)
                {
                    // データが存在しなかった場合 → 追加
                    info.Version = 0;
                    info.Cdt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                    info.Udt = info.Cdt;
                    info.Updator = info.Creator;
                    info.Status = Context.CommonConst.STATUS_USED;

                    // トランザクション作成
                    using (var tran = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            // 商品マスタ(親) 追加
                            _context.ItemInfo.Add(info);
                            _context.SaveChanges();

                            // セット商品マスタ(子) 追加
                            int seq = 1;
                            foreach (var item in info.ChildItems)
                            {
                                item.Version = 0;
                                item.Seq = seq;
                                item.Cdt = info.Cdt;
                                item.Udt = item.Cdt;
                                item.Updator = info.Creator;
                                item.Status = Context.CommonConst.STATUS_USED;

                                _context.SetItemInfo.Add(item);
                                _context.SaveChanges();
                                seq++;
                            }

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }

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
                    return await UpdateSetItem(info, true);
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
        /// セット商品(親+子リスト) 更新
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> UpdateSetItem(SetItemInfo info)
        {
            return await UpdateSetItem(info, false);
        }

        /// <summary>
        /// セット商品(親+子リスト) 更新
        /// </summary>
        /// <param name="info"></param>
        /// <param name="addFlag">True:UnUsedからの復活新規作成時</param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> UpdateSetItem(SetItemInfo info, bool addFlag)
        {
            try
            {
                // versionチェック
                if (!addFlag && !await CheckVersion(info)) { return CommonEnum.DBUpdateResult.VersionError; }

                // セット商品マスタ(子) 取得
                var DeleteChildItemList = _context.SetItemInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                   w.SetItemCode == info.ItemCode)
                                                        .AsNoTracking()
                                                        .ToList();

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // 商品マスタ(親) 更新
                        info.Version++;
                        info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);

                        _context.ItemInfo.Update(info);
                        _context.SaveChanges();

                        // セット商品マスタ(子) 削除
                        foreach (var item in DeleteChildItemList)
                        {
                            _context.SetItemInfo.Remove(item);
                            _context.SaveChanges();
                        }

                        // セット商品マスタ(子) 追加
                        int seq = 1;
                        foreach (var item in info.ChildItems)
                        {
                            item.Version = 0;
                            item.Seq = seq;
                            item.Cdt = info.Udt;
                            item.Udt = item.Cdt;
                            item.Creator = info.Updator;
                            item.Updator = info.Updator;
                            item.Status = Context.CommonConst.STATUS_USED;

                            _context.SetItemInfo.Add(item);
                            _context.SaveChanges();
                            seq++;
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }

                return CommonEnum.DBUpdateResult.Success;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return CommonEnum.DBUpdateResult.Error;
            }
        }

        /// <summary>
        /// セット商品(親+子リスト) 削除
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<CommonEnum.DBUpdateResult> DeleteSetItem(SetItemInfo info)
        {
            try
            {
                // versionチェック
                if (!await CheckVersion(info)) { return CommonEnum.DBUpdateResult.VersionError; }

                // セット商品マスタ(子) 取得
                var DeleteChildItemList = _context.SetItemInfo.Where(w => w.CompanyNo == info.CompanyNo &&
                                                                   w.SetItemCode == info.ItemCode)
                                                        .ToList();

                // トランザクション作成
                using (var tran = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // 商品マスタ(親) 削除
                        info.Version++;
                        info.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                        info.Status = CommonConst.STATUS_UNUSED;

                        _context.ItemInfo.Update(info);
                        _context.SaveChanges();

                        // セット商品マスタ(子) 論理削除
                        // ↓Updateだとduplicate key valueエラーが解消されないのでDelete->Insertで対応
                        _context.SetItemInfo.RemoveRange(DeleteChildItemList);
                        _context.SaveChanges();

                        foreach (var item in DeleteChildItemList)
                        {
                            item.Version++;
                            item.Udt = System.DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            item.Updator = info.Updator;
                            item.Status = CommonConst.STATUS_UNUSED;

                            _context.SetItemInfo.Add(item);
                            _context.SaveChanges();
                        }

                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }

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
        /// <param name="itemInfo">商品情報(Versionはチェック後でカウントアップする)</param>
        /// <returns>True:Ok, False:NG</returns>
        private async Task<bool> CheckVersion(SetItemInfo info)
        {
            try
            {
                // データ取得
                var oldInfo = _context.ItemInfo.Where(w => w.CompanyNo == info.CompanyNo && w.ItemCode == info.ItemCode)
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

    }

}
