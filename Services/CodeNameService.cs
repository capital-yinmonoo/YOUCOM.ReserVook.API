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
    public class CodeNameService : ICodeNameService
    {

        private DBContext _context;

        public CodeNameService(DBContext context)
        {
            _context = context;
        }

        public async Task<List<MstCodeNameInfo>> GetListDictionaryDataDid(string companyNo, string codeDivision)
        {
            return _context.CodeNameInfo
                    .Where(d => d.CompanyNo == companyNo)
                    .Where(d => d.DivisionCode == codeDivision)
                    .Where(d => d.Status == CommonConst.STATUS_USED)
                    .OrderBy(d => d.DisplayOrder).ToList();

        }

        // 情報取得(画面表示,削除用)
        public async Task<List<MstCodeNameInfo>> GetCodeNameList(MstCodeNameInfo codenameInfo)
        {
            var lists = new List<MstCodeNameInfo>();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT codename.*";
            sql += " FROM mst_code_name codename";
            sql += " WHERE codename.company_no = '" + codenameInfo.CompanyNo + "'";
            sql += " AND codename.division_code = '" + codenameInfo.DivisionCode + "'";
            sql += " AND codename.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY codename.display_order ASC ,codename.code ASC";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var list = new MstCodeNameInfo();
                        list.CompanyNo = reader["company_no"].ToString();
                        list.DivisionCode = reader["division_code"].ToString();
                        list.Code = reader["code"].ToString();
                        list.CodeName = reader["code_name"].ToString();
                        list.CodeValue = reader["code_value"].ToString();
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

        // 情報取得(編集用)
        public async Task<MstCodeNameInfo> GetCodeNameById(MstCodeNameInfo codenameInfo)
        {
            var list = new MstCodeNameInfo();
            var command = _context.Database.GetDbConnection().CreateCommand();
            string sql = "SELECT codename.*";
            sql += " FROM mst_code_name codename";
            sql += " WHERE codename.company_no = '" + codenameInfo.CompanyNo + "'";
            sql += " AND codename.division_code = '" + codenameInfo.DivisionCode + "'";
            sql += " AND codename.code = '" + codenameInfo.Code + "'";
            sql += " AND codename.status <> '" + CommonConst.STATUS_UNUSED + "'";

            command.CommandText = sql;
            _context.Database.OpenConnection();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.CompanyNo = reader["company_no"].ToString();
                        list.DivisionCode = codenameInfo.DivisionCode;
                        list.Code = reader["code"].ToString();
                        list.CodeName = reader["code_name"].ToString();
                        if(list.DivisionCode == ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT))
                        {
                            list.CodeValue = reader["code_value"].ToString();
                        }
                        else
                        {
                            list.CodeValue = " ";
                        }
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

        // 追加
        public async Task<int> AddCodeName(MstCodeNameInfo codenameInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;
            // 会社番号,分類コード,コードの一致するデータを取得
            var info = _context.CodeNameInfo.Where(w => w.CompanyNo == codenameInfo.CompanyNo && w.DivisionCode == codenameInfo.DivisionCode && w.Code == codenameInfo.Code).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                codenameInfo.Version = 0;
                codenameInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                codenameInfo.Udt = codenameInfo.Cdt;
                codenameInfo.Status = Context.CommonConst.STATUS_USED;

                _context.CodeNameInfo.Add(codenameInfo);
                _context.SaveChanges();
            }
            else if (info.Status == CommonConst.STATUS_UNUSED)
            {
                // データが存在し,Statusが「9」の場合 → 更新
                bool addFlag = true;
                codenameInfo.Version = info.Version;
                codenameInfo.Creator = info.Creator;
                codenameInfo.Cdt = info.Cdt;
                codenameInfo.Status = Context.CommonConst.STATUS_USED;
                var updateInfo = await UpdateCodeName(codenameInfo, addFlag);
            }
            else
            {
                // データが存在し,Statusが「1」の場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        // 更新
        public async Task<int> UpdateCodeName(MstCodeNameInfo codenameInfo, bool addFlag)
        {
            try
            {
                // versionチェック
                if (!addFlag)
                {
                    if (!await CodeNameCheckVer(codenameInfo)) { return -1; }
                }

                codenameInfo.Version++;
                codenameInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                _context.CodeNameInfo.Update(codenameInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // 削除
        public async Task<int> DelCodeName(MstCodeNameInfo codenameInfo)
        {
            try
            {
                // versionチェック
                if (!await CodeNameCheckVer(codenameInfo)) { return -1; }

                codenameInfo.Version++;
                codenameInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                codenameInfo.Status = Context.CommonConst.STATUS_UNUSED;

                _context.CodeNameInfo.Update(codenameInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // 削除チェック
        public async Task<int> DeleteCodeNameCheck(MstCodeNameInfo codenameInfo)
        {
            try
            {
                int count = 0;

                if (codenameInfo.DivisionCode == ((int)CommonEnum.CodeDivision.RoomType).ToString(CommonConst.CODE_DIVISION_FORMAT))
                {
                    // 部屋タイプ
                    count = _context.RoomsInfo.Count(w => w.CompanyNo == codenameInfo.CompanyNo && w.RoomTypeCode == codenameInfo.Code && w.Status == CommonConst.STATUS_USED);
                    if (count > 0)
                    {
                        // 削除不可 部屋マスタで使用中
                        return 1;
                    }
                    
                    var countRoomTypeConv = _context.RoomTypeConvertInfo.AsNoTracking().Count(w => w.CompanyNo == codenameInfo.CompanyNo && w.RmtypeCd == codenameInfo.Code && w.Status == CommonConst.STATUS_USED);
                    if (countRoomTypeConv > 0)
                    {
                        // 削除不可 部屋タイプ変換マスタで使用中
                        return 3;
                    }
                }
                else if (codenameInfo.DivisionCode == ((int)CommonEnum.CodeDivision.Floor).ToString(CommonConst.CODE_DIVISION_FORMAT))
                {
                    // フロア
                    count = _context.RoomsInfo.Count(w => w.CompanyNo == codenameInfo.CompanyNo && w.Floor == codenameInfo.Code && w.Status == CommonConst.STATUS_USED);
                    if (count > 0)
                    {
                        return 1;
                    }
                }
                else if (codenameInfo.DivisionCode == ((int)CommonEnum.CodeDivision.LostPlace).ToString(CommonConst.CODE_DIVISION_FORMAT))
                {
                    // 忘れ物発見場所分類
                    count = _context.LostItemsBaseInfo.Count(w => w.CompanyNo == codenameInfo.CompanyNo && w.FoundPlaceCode == codenameInfo.Code && w.Status == CommonConst.STATUS_USED);
                    if (count > 0)
                    {
                        return 2;
                    }
                }
                else if (codenameInfo.DivisionCode == ((int)CommonEnum.CodeDivision.LostStrage).ToString(CommonConst.CODE_DIVISION_FORMAT))
                {
                    // 忘れ物保管場所分類
                    count = _context.LostItemsBaseInfo.Count(w => w.CompanyNo == codenameInfo.CompanyNo && w.StorageCode == codenameInfo.Code && w.Status == CommonConst.STATUS_USED);
                    if (count > 0)
                    {
                        return 2;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return 0;
        }

        // バージョンチェック
        private async Task<bool> CodeNameCheckVer(MstCodeNameInfo codenameInfo)
        {
            try
            {
                // キーセット
                MstCodeNameInfo keyInfo = new MstCodeNameInfo() { CompanyNo = codenameInfo.CompanyNo, DivisionCode = codenameInfo.DivisionCode, Code = codenameInfo.Code };

                // データ取得
                var info = await GetCodeNameById(keyInfo);

                // バージョン差異チェック
                if (codenameInfo.Version != info.Version)
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