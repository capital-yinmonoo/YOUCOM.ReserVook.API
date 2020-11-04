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
    public class RoomTypeConvertService : IRoomTypeConvertService
    {

        private DBContext _context;

        public RoomTypeConvertService(DBContext context)
        {
            _context = context;
        }

        // 情報取得(画面表示用)
        public async Task<List<FrMScRmtypeConvertInfo>> GetRoomTypeConvertList(FrMScRmtypeConvertInfo roomTypeConvertInfo)
        {
            string sql = "SELECT roomTypeConvert.*, code_name, content_1";
            sql += " FROM fr_m_sc_rmtype_convert roomTypeConvert";

            sql += " LEFT JOIN fr_m_sc_nm scName";
            sql += " ON roomTypeConvert.company_no =  scName.company_no";
            sql += " AND scName.sc_seg_cd = roomTypeConvert.sc_cd";

            sql += " LEFT JOIN mst_code_name roomtype";
            sql += " ON roomTypeConvert.company_no =  roomtype.company_no";
            sql += " AND roomtype.division_code ='" + ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += " AND roomTypeConvert.rmtype_cd =  roomtype.code";

            sql += " WHERE roomTypeConvert.company_no = '" + roomTypeConvertInfo.CompanyNo + "'";
            sql += " AND roomTypeConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY roomTypeConvert.sc_cd ASC, roomTypeConvert.sc_rmtype_cd ASC";

            var lists = new List<FrMScRmtypeConvertInfo>();
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
                            var list = new FrMScRmtypeConvertInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ScCd = reader["sc_cd"].ToString();
                            list.ScRmtypeCd = reader["sc_rmtype_cd"].ToString();
                            list.RmtypeCd = reader["rmtype_cd"].ToString();
                            list.UpdateCnt = int.Parse(reader["update_cnt"].ToString());
                            list.ProgramId = reader["program_id"].ToString();
                            list.CreateClerk = reader["create_clerk"].ToString();
                            list.CreateMachineNo = reader["create_machine_no"].ToString();
                            list.CreateMachine = reader["create_machine"].ToString();
                            list.CreateDatetime = reader["create_datetime"].ToString();
                            list.UpdateClerk = reader["update_clerk"].ToString();
                            list.UpdateMachineNo = reader["update_machine_no"].ToString();
                            list.UpdateMachine = reader["update_machine"].ToString();
                            list.UpdateDatetime = reader["update_datetime"].ToString();
                            list.Status = reader["status"].ToString();

                            list.CdName = reader["content_1"].ToString();
                            list.Rmtype = reader["code_name"].ToString();

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

        // 情報取得(編集用)
        public async Task<FrMScRmtypeConvertInfo> GetRoomTypeConvertById(FrMScRmtypeConvertInfo roomTypeConvertInfo)
        {
            var list = new FrMScRmtypeConvertInfo();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT roomTypeConvert.*";
            sql += " FROM fr_m_sc_rmtype_convert roomTypeConvert";
            sql += " WHERE roomTypeConvert.company_no = '" + roomTypeConvertInfo.CompanyNo + "'";
            sql += " AND roomTypeConvert.sc_cd = '" + roomTypeConvertInfo.ScCd + "'";
            sql += " AND roomTypeConvert.sc_rmtype_cd = '" + roomTypeConvertInfo.ScRmtypeCd + "'";
            sql += " AND roomTypeConvert.status <> '" + CommonConst.STATUS_UNUSED + "'";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.CompanyNo = reader["company_no"].ToString();
                        list.ScCd = reader["sc_cd"].ToString();
                        list.ScRmtypeCd = reader["sc_rmtype_cd"].ToString();
                        list.RmtypeCd = reader["rmtype_cd"].ToString();
                        list.UpdateCnt = int.Parse(reader["update_cnt"].ToString());
                        list.ProgramId = reader["program_id"].ToString();
                        list.CreateClerk = reader["create_clerk"].ToString();
                        list.CreateMachineNo = reader["create_machine_no"].ToString();
                        list.CreateMachine = reader["create_machine"].ToString();
                        list.CreateDatetime = reader["create_datetime"].ToString();
                        list.UpdateClerk = reader["update_clerk"].ToString();
                        list.UpdateMachineNo = reader["update_machine_no"].ToString();
                        list.UpdateMachine = reader["update_machine"].ToString();
                        list.UpdateDatetime = reader["update_datetime"].ToString();
                        list.Status = reader["status"].ToString();
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

        // 情報追加
        public async Task<int> AddRoomTypeConvert(FrMScRmtypeConvertInfo roomTypeConvertInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;
            // 会社番号,SCコード,SC部屋タイプコードの一致するデータを取得
            var info = _context.RoomTypeConvertInfo.Where(w => w.CompanyNo == roomTypeConvertInfo.CompanyNo && w.ScCd == roomTypeConvertInfo.ScCd && w.ScRmtypeCd == roomTypeConvertInfo.ScRmtypeCd).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                roomTypeConvertInfo.UpdateCnt = 0;
                roomTypeConvertInfo.CreateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                roomTypeConvertInfo.CreateMachine = Environment.MachineName;
                roomTypeConvertInfo.UpdateDatetime = roomTypeConvertInfo.CreateDatetime;
                roomTypeConvertInfo.UpdateMachine = roomTypeConvertInfo.CreateMachine;
                roomTypeConvertInfo.Status = Context.CommonConst.STATUS_USED;

                _context.RoomTypeConvertInfo.Add(roomTypeConvertInfo);
                _context.SaveChanges();
            }
            else if (info.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」の場合 → 更新
                bool addFlag = true;
                roomTypeConvertInfo.UpdateCnt = info.UpdateCnt;
                roomTypeConvertInfo.CreateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                roomTypeConvertInfo.CreateMachine = Environment.MachineName;
                var updateInfo = await UpdateRoomTypeConvert(roomTypeConvertInfo, addFlag);
            }
            else
            {
                // データが存在し,Statusが「1」の場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        // 情報更新
        public async Task<int> UpdateRoomTypeConvert(FrMScRmtypeConvertInfo roomTypeConvertInfo, bool addFlag)
        {
            try
            {
                // 会社番号,SCコード,SCサイトコードの一致するデータを取得
                var info = _context.RoomTypeConvertInfo.Where(w => w.CompanyNo == roomTypeConvertInfo.CompanyNo && w.ScCd == roomTypeConvertInfo.ScCd && w.ScRmtypeCd == roomTypeConvertInfo.ScRmtypeCd).AsNoTracking().SingleOrDefault();

                if (!addFlag)
                {
                    // バージョンチェック
                    if (!await SiteConvertCheckVer(roomTypeConvertInfo)) { return -1; }
                    roomTypeConvertInfo.CreateClerk = info.CreateClerk;
                    roomTypeConvertInfo.CreateDatetime = info.CreateDatetime;
                    roomTypeConvertInfo.CreateMachine = info.CreateMachine;
                }
                roomTypeConvertInfo.UpdateCnt++;
                roomTypeConvertInfo.ProgramId = info.ProgramId;
                roomTypeConvertInfo.CreateMachineNo = info.CreateMachineNo;
                roomTypeConvertInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                roomTypeConvertInfo.UpdateMachineNo = info.UpdateMachineNo;
                roomTypeConvertInfo.UpdateMachine = Environment.MachineName;
                roomTypeConvertInfo.Status = Context.CommonConst.STATUS_USED;

                _context.RoomTypeConvertInfo.Update(roomTypeConvertInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // 情報削除
        public async Task<int> DelRoomTypeConvert(FrMScRmtypeConvertInfo roomTypeConvertInfo)
        {
            try
            {
                // バージョンチェック
                if (!await SiteConvertCheckVer(roomTypeConvertInfo)) { return -1; }

                roomTypeConvertInfo.UpdateCnt++;
                roomTypeConvertInfo.UpdateDatetime = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                roomTypeConvertInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.RoomTypeConvertInfo.Update(roomTypeConvertInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // バージョンチェック
        private async Task<bool> SiteConvertCheckVer(FrMScRmtypeConvertInfo roomTypeConvertInfo)
        {
            try
            {
                // キーセット
                FrMScRmtypeConvertInfo keyInfo = new FrMScRmtypeConvertInfo() { CompanyNo = roomTypeConvertInfo.CompanyNo, ScCd = roomTypeConvertInfo.ScCd, ScRmtypeCd = roomTypeConvertInfo.ScRmtypeCd };

                // データ取得
                var info = await GetRoomTypeConvertById(keyInfo);

                // バージョン差異チェック
                if (roomTypeConvertInfo.UpdateCnt != info.UpdateCnt)
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
    }
}