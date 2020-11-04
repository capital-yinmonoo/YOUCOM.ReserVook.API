using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.Commons.Extensions;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Services
{
    public class BillService : IBillService
    {
        private DBContext _context;

        public BillService(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 予約に存在するビル分割Noのリストを取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<IList<string>> GetSeparateBillNoList(ReserveModel cond)
        {

            var billNoList = new List<string>();

            try
            {

                // 売上明細(精算済)
                var salesDetailsInfo = _context.SalesDetailsInfo
                                            .Where(sI => sI.CompanyNo == cond.CompanyNo
                                                      && sI.ReserveNo == cond.ReserveNo
                                                      && sI.AdjustmentFlag == CommonConst.ADJUSTMENTED)
                                            .AsNoTracking()
                                            .OrderBy(o => o.BillSeparateSeq)
                                            .Select(s => s.BillSeparateSeq.ToString())
                                            .ToList();

                // 入金明細
                var depositInfo = _context.DepositDetailsInfo
                                            .Where(d => d.CompanyNo == cond.CompanyNo
                                                      && d.ReserveNo == cond.ReserveNo)
                                            .AsNoTracking()
                                            .OrderBy(o => o.BillSeparateSeq)
                                            .Select(s => s.BillSeparateSeq.ToString())
                                            .ToList();

                billNoList.AddRange(salesDetailsInfo);
                billNoList.AddRange(depositInfo);
                billNoList = billNoList.GroupBy(p => p).Select(group => group.First()).ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return billNoList;
        }

        /// <summary>
        /// ビルNoをチェック
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public async Task<bool> CheckBillNo(ReserveModel cond)
        {
            try
            {
                var billSepList = await GetSeparateBillNoList(cond);

                foreach (string billSepNo in billSepList)
                {
                    // 売上明細(精算済)
                    var salesDetailsInfo = _context.SalesDetailsInfo
                                                .Where(sI => sI.CompanyNo == cond.CompanyNo
                                                          && sI.ReserveNo == cond.ReserveNo
                                                          && sI.AdjustmentFlag == CommonConst.ADJUSTMENTED
                                                          && sI.BillSeparateSeq == int.Parse(billSepNo))
                                                .ToList();

                    // 入金明細
                    var depositInfo = _context.DepositDetailsInfo
                                                .Where(d => d.CompanyNo == cond.CompanyNo
                                                          && d.ReserveNo == cond.ReserveNo
                                                          && d.BillSeparateSeq == int.Parse(billSepNo))
                                                .ToList();

                    string wkBillNo = salesDetailsInfo[0].BillNo.NullToEmpty();
                    string nextBillNo = "";
                    bool isFirst = true;
                    bool updateFlg = false;

                    // 売上明細
                    foreach (var sale in salesDetailsInfo)
                    {
                        // ビル分割毎のビルNoが一致しているか整合性をチェック
                        if (wkBillNo == sale.BillNo.NullToEmpty())
                        {
                            // ビルNoが登録されていない場合、新規採番を行う
                            if (sale.BillNo.IsNullOrEmpty())
                            {
                                if (isFirst)
                                {
                                    // 会社マスタからビルNoを新規採番
                                    nextBillNo = Numbering(cond.CompanyNo);
                                }

                                sale.BillNo = nextBillNo;
                                sale.Version++;
                                sale.Udt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                                sale.Updator = cond.Updator;
                                updateFlg = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("売上明細のビル分割No" + billSepNo + "で整合性エラーがあります。");
                            return false;
                        }

                        isFirst = false;
                    }

                    // 入金明細
                    foreach (var deposit in depositInfo)
                    {
                        // ビル分割毎のビルNoが一致しているか整合性をチェック
                        if (wkBillNo == deposit.BillNo.NullToEmpty())
                        {
                            deposit.BillNo = nextBillNo;
                            deposit.Version++;
                            deposit.Udt = DateTime.Now.ToString(CommonConst.DATETIME_FORMAT);
                            deposit.Updator = cond.Updator;
                        }
                        else
                        {
                            Console.WriteLine("入金明細のビル分割No" + billSepNo + "で整合性エラーがあります。");
                            return false;
                        }

                    }

                    // 採番なしの為,更新しない
                    if (!updateFlg) { return true; }

                    // トランザクション作成
                    using (var tran = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var sale in salesDetailsInfo)
                            {
                                _context.SalesDetailsInfo.Update(sale);
                                _context.SaveChanges();
                            }
                            foreach (var deposit in depositInfo)
                            {
                                _context.DepositDetailsInfo.Update(deposit);
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
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 採番
        /// </summary>
        /// <param name="companyNo">会社番号</param>
        public string Numbering(string companyNo)
        {
            var info = new MstCompanyInfo();

            // 採番
            info = _context.CompanyInfo.Single(x => x.CompanyNo == companyNo);

            var ret = info.LastBillNo.ToString("0000000000");

            // +1して更新
            info.LastBillNo = info.LastBillNo + 1;

            _context.CompanyInfo.Update(info);
            _context.SaveChanges();

            return ret;
        }

        /// <summary>
        /// 会社マスタからロゴデータを取得
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public byte[] GetCompanyLogo(BaseInfo cond)
        {

            try
            {

                // ロゴ取得
                byte[] logo = _context.CompanyInfo.Where(c => c.CompanyNo == cond.CompanyNo)
                                            .Select(s => s.LogoData)
                                            .SingleOrDefault();
                return logo;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

    }
}
