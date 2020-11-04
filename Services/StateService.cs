using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOUCOM.ReserVook.API.Context;
using YOUCOM.ReserVook.API.Entities;
using YOUCOM.ReserVook.API.Interfaces;

namespace YOUCOM.ReserVook.API.Services
{
    public class StateService : IStateService
    {

        private DBContext _context;

        public StateService(DBContext context)
        {
            _context = context;
        }

        // 情報取得(画面表示用)
        public async Task<List<MstStateInfo>> GetStateList(MstStateInfo stateInfo)
        {
            string sql = "SELECT state.*";
            sql += " FROM mst_state state";
            sql += " WHERE state.company_no = '" + stateInfo.CompanyNo + "'";
            sql += " AND state.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY state.order_no ASC ,state.item_state_code ASC";

            var lists = new List<MstStateInfo>();
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
                            var list = new MstStateInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ItemStateCode = reader["item_state_code"].ToString();
                            list.ItemStateName = reader["item_state_name"].ToString();
                            list.Color = reader["color"].ToString();
                            list.DefaultFlagSearch = reader["default_flag_search"].ToString();
                            list.DefaultFlagEntry = reader["default_flag_entry"].ToString();
                            list.OrderNo = int.Parse(reader["order_no"].ToString());
                            list.Status = reader["status"].ToString();
                            list.Version = int.Parse(reader["version"].ToString());
                            list.Creator = reader["creator"].ToString();
                            list.Updator = reader["updator"].ToString();
                            list.Cdt = reader["cdt"].ToString();
                            list.Udt = reader["udt"].ToString();

                            lists.Add(list);
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

            return lists;
        }

        // 情報取得(編集,削除用)
        public async Task<MstStateInfo> GetStateById(MstStateInfo stateInfo)
        {
            string sql = "SELECT state.*";
            sql += " FROM mst_state state";
            sql += " WHERE state.company_no = '" + stateInfo.CompanyNo + "'";
            sql += " AND state.item_state_code = '" + stateInfo.ItemStateCode + "'";
            sql += " AND state.status <> '" + CommonConst.STATUS_UNUSED + "'";


            var list = new MstStateInfo();
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
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ItemStateCode = reader["item_state_code"].ToString();
                            list.ItemStateName = reader["item_state_name"].ToString();
                            list.Color = reader["color"].ToString();
                            list.DefaultFlagSearch = reader["default_flag_search"].ToString();
                            list.DefaultFlagEntry = reader["default_flag_entry"].ToString();
                            list.OrderNo = int.Parse(reader["order_no"].ToString());
                            list.Status = reader["status"].ToString();
                            list.Version = int.Parse(reader["version"].ToString());
                            list.Creator = reader["creator"].ToString();
                            list.Updator = reader["updator"].ToString();
                            list.Cdt = reader["cdt"].ToString();
                            list.Udt = reader["udt"].ToString();
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

        // 追加
        public async Task<int> AddState(MstStateInfo stateInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;
            try
            {
              
                // 会社番号,コード一致するデータを取得
                var info = _context.StateInfo.Where(w => w.CompanyNo == stateInfo.CompanyNo && w.ItemStateCode == stateInfo.ItemStateCode).AsNoTracking().SingleOrDefault();

                if (info == null)
                {
                    // データが存在しなかった場合 → 追加
                    stateInfo.Version = 0;
                    stateInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                    stateInfo.Udt = stateInfo.Cdt;
                    stateInfo.Status = Context.CommonConst.STATUS_USED;

                    _context.StateInfo.Add(stateInfo);
                    _context.SaveChanges();
                }
                else if (info.Status == CommonConst.STATUS_UNUSED)
                {
                    // データが存在し,Statusが「9」場合 → 更新
                    bool addFlag = true;
                    stateInfo.Version = info.Version;
                    stateInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                    stateInfo.Status = Context.CommonConst.STATUS_USED;
                    var updateInfo = await UpdateState(stateInfo, addFlag);
                }
                else
                {
                    // データが存在し,Statusが「1」場合 → エラー
                    errFlg = 1;
                }
            }catch(Exception e)
            {
                throw e;
            }
            return errFlg;
        }

        // 更新
        public async Task<int> UpdateState(MstStateInfo stateInfo, bool addFlag)
        {
            try
            {
                // versionチェック
                if (!addFlag)
                {
                    if (!await StateCheckVer(stateInfo)) { return -1; }
                }

                stateInfo.Version++;
                stateInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                _context.StateInfo.Update(stateInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // 削除
        public async Task<int> DelState(MstStateInfo stateInfo)
        {
            try
            {
                // versionチェック
                if (!await StateCheckVer(stateInfo)) { return -1; }

                stateInfo.Version++;
                stateInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                stateInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.StateInfo.Update(stateInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        // バージョンチェック
        private async Task<bool> StateCheckVer(MstStateInfo stateInfo)
        {
            try
            {
                // キーセット
                MstStateInfo keyInfo = new MstStateInfo() { CompanyNo = stateInfo.CompanyNo, ItemStateCode = stateInfo.ItemStateCode };

                // データ取得
                var info = await GetStateById(keyInfo);

                // バージョン差異チェック
                if (stateInfo.Version != info.Version)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        // 削除チェック
        public async Task<int> DeleteStateCheck(MstStateInfo stateInfo)
        {
            try
            {
                int count = _context.LostItemsBaseInfo.Count(w => w.CompanyNo == stateInfo.CompanyNo && w.ItemState == stateInfo.ItemStateCode && w.Status == CommonConst.STATUS_USED);
                if (count > 0)
                {
                    return 1;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return 0;
        }

        // 忘れ物状態取得(他マスタ用)
        public async Task<List<MstStateInfo>> GetState(string companyNo)
        {
            return _context.StateInfo
                    .Where(d => d.CompanyNo == companyNo && d.Status == CommonConst.STATUS_USED)
                    .OrderBy(d => d.OrderNo).ToList();

        }

    }
}