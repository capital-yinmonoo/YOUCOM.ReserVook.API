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
    public class AgentService : IAgentService
    {

        private DBContext _context;

        public AgentService(DBContext context)
        {
            _context = context;
        }

        // エージェントの情報取得(画面表示,削除用)
        public async Task<List<MstAgentInfo>> GetAgentList(MstAgentInfo agentInfo)
        {
            var lists = new List<MstAgentInfo>();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT agent.*";
            sql += " FROM mst_agent agent";
            sql += " WHERE agent.company_no = '" + agentInfo.CompanyNo + "'";
            sql += " AND agent.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY agent.display_order ASC ,agent.agent_code ASC";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var list = new MstAgentInfo();
                        list.CompanyNo = reader["company_no"].ToString();
                        list.AgentCode = reader["agent_code"].ToString();
                        list.AgentName = reader["agent_name"].ToString();
                        list.Remarks = reader["remarks"].ToString();
                        list.DisplayOrder = int.Parse(reader["display_order"].ToString());
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

            return lists;
        }

        // エージェントの情報取得(編集用)
        public async Task<MstAgentInfo> GetAgentById(MstAgentInfo agentInfo)
        {
            var list = new MstAgentInfo();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT agent.*";
            sql += " FROM mst_agent agent";
            sql += " WHERE agent.company_no = '" + agentInfo.CompanyNo + "'";
            sql += " AND agent.agent_code = '" + agentInfo.AgentCode + "'";
            sql += " AND agent.status <> '" + CommonConst.STATUS_UNUSED + "'";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.CompanyNo = reader["company_no"].ToString();
                        list.AgentCode = reader["agent_code"].ToString();
                        list.AgentName = reader["agent_name"].ToString();
                        list.Remarks = reader["remarks"].ToString();
                        list.DisplayOrder = int.Parse(reader["display_order"].ToString());
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

            return list;
        }

        // エージェントの情報取得(バージョンチェック用)
        private async Task<MstAgentInfo> GetAgentVer(MstAgentInfo agentInfo)
        {
            MstAgentInfo info = _context.AgentInfo.AsNoTracking().Where(w => w.CompanyNo == agentInfo.CompanyNo && w.AgentCode == agentInfo.AgentCode && w.Status != CommonConst.STATUS_UNUSED).SingleOrDefault();
            return info;
        }

        // エージェント情報追加
        public async Task<int> AddAgent(MstAgentInfo agentInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;
            // 会社番号,エージェントコードの一致するデータを取得
            var info = _context.AgentInfo.Where(w => w.CompanyNo == agentInfo.CompanyNo && w.AgentCode == agentInfo.AgentCode).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                agentInfo.Version = 0;
                agentInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                agentInfo.Udt = agentInfo.Cdt;
                agentInfo.Status = Context.CommonConst.STATUS_USED;

                _context.AgentInfo.Add(agentInfo);
                _context.SaveChanges();
            }
            else if (info.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」の場合 → 更新
                bool addFlag = true;
                agentInfo.Version = info.Version;
                agentInfo.Creator = info.Creator;
                agentInfo.Cdt = info.Cdt;
                agentInfo.Status = Context.CommonConst.STATUS_USED;
                var updateInfo = await UpdateAgent(agentInfo, addFlag);
            }
            else
            {
                // データが存在し,Statusが「1」の場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        // エージェント情報更新
        public async Task<int> UpdateAgent(MstAgentInfo agentInfo, bool addFlag)
        {
            try
            {
                // versionチェック
                if (!addFlag)
                {
                    if (!await AgentCheckVer(agentInfo)) { return -1; }
                }

                agentInfo.Version++;
                agentInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                _context.AgentInfo.Update(agentInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // エージェント情報削除用画面取得
        public async Task<MstAgentInfo> GetAgentInfo(MstAgentInfo agentInfo)
        {
            // CompanyNo,AgentCodeが一致し,Statusが「9」ではないデータを取得
            MstAgentInfo info = _context.AgentInfo.Where(w => w.CompanyNo == agentInfo.CompanyNo && w.AgentCode == agentInfo.AgentCode && w.Status != CommonConst.STATUS_UNUSED).SingleOrDefault();
            return info;
        }

        // エージェント情報削除
        public async Task<int> DelAgent(MstAgentInfo agentInfo)
        {
            try
            {
                // versionチェック
                if (!await AgentCheckVer(agentInfo)) { return -1; }

                agentInfo.Version++;
                agentInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                agentInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.AgentInfo.Update(agentInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        // バージョンチェック
        private async Task<bool> AgentCheckVer(MstAgentInfo agentInfo)
        {
            try
            {
                // キーセット
                MstAgentInfo keyInfo = new MstAgentInfo() { CompanyNo = agentInfo.CompanyNo, AgentCode = agentInfo.AgentCode };

                // データ取得
                var info = await GetAgentById(keyInfo);

                // バージョン差異チェック
                if (agentInfo.Version != info.Version)
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

        /// <summary>
        /// 削除チェック
        /// </summary>
        /// <param name="agentInfo"></param>
        /// <returns>0:OK -1以下:NG 1以上:フロントエンドで確認 </returns>
        public async Task<int> DeleteAgentCheck(MstAgentInfo agentInfo)
        {
            // ---- Do not Delete ----
            // 本予約、未精算の予約での使用チェック
            // CompanyNo,AgentCodeが一致し,ReserveStateDivisionが「1」でAdjustmentFlugが「0」,Statusが「1」のデータを取得
            int count = _context.ReserveBasicInfo.AsNoTracking()
                                                 .Count(w => w.CompanyNo == agentInfo.CompanyNo
                                                          && w.AgentCode == agentInfo.AgentCode
                                                          && w.AdjustmentFlag == CommonConst.NOT_ADJUSTMENTED
                                                          && w.ReserveStateDivision == ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString()
                                                          && w.Status == CommonConst.STATUS_USED);
            if (count > 0) { return -1; }

            // サイト変換での使用チェック
            count = _context.SiteConvertInfo.AsNoTracking()
                                            .Count(w => w.CompanyNo == agentInfo.CompanyNo
                                                     && w.TravelAgncCd == agentInfo.AgentCode
                                                     && w.Status == CommonConst.STATUS_USED);
            if (count > 0) { return -2; }



            // ---- Confirm Delete ----
            // 本予約、精算済の予約での使用チェック
            // CompanyNo,AgentCodeが一致し,ReserveStateDivisionが「1」でAdjustmentFlugが「1」,Statusが「1」のデータを取得
            count = _context.ReserveBasicInfo.AsNoTracking()
                                             .Count(w => w.CompanyNo == agentInfo.CompanyNo
                                                      && w.AgentCode == agentInfo.AgentCode
                                                      && w.AdjustmentFlag == CommonConst.ADJUSTMENTED
                                                      && w.ReserveStateDivision == ((int)CommonEnum.ReserveStateDivision.MainReserve).ToString()
                                                      && w.Status == CommonConst.STATUS_USED);
            if (count > 0) { return 1; } 

            // ---- OK! ----
            return 0;
        }

        // エージェント取得(サイト変換マスタ用)
        public async Task<List<MstAgentInfo>> GetAgentDataDid(string companyNo)
        {
            return _context.AgentInfo
                    .Where(d => d.CompanyNo == companyNo && d.Status == CommonConst.STATUS_USED)
                    .OrderBy(d => d.DisplayOrder).ToList();

        }
    }
}