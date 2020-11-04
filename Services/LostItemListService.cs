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
    public class LostItemListService : ILostItemListService
    {

        private DBContext _context;

        public LostItemListService(DBContext context)
        {
            _context = context;
        }

        // 情報取得(画面表示用)
        public async Task<List<TrnLostItemsBaseInfo>> GetLostItemList(TrnLostItemsBaseInfo lostItemListInfo)
        {
            string check;
            string sql = "SELECT lostitem.*,state.item_state_name,state.default_flag_search,lostplace.code_name AS lost_place_name,storage.code_name AS storage_name,pic.binary_data,pic.content_type";
            sql += " FROM trn_lost_items_base lostitem";

            sql += " LEFT JOIN(SELECT pic1.*";
            sql += " FROM trn_lost_items_picture pic1";
            sql += " INNER JOIN(SELECT company_no, management_no, min(file_seq) file_seq";
            sql += " FROM trn_lost_items_picture";
            sql += " GROUP BY company_no, management_no";
            sql += " )pic2";
            sql += " ON pic1.company_no = pic2.company_no";
            sql += " AND pic1.management_no = pic2.management_no";
            sql += " AND pic1.file_seq = pic2.file_seq";
            sql += " )pic";
            sql += " ON lostitem.company_no = pic.company_no";
            sql += " AND lostitem.management_no = pic.management_no";

            sql += " LEFT JOIN mst_state state";
            sql += " ON state.company_no = lostitem.company_no";
            sql += " AND state.item_state_code = lostitem.item_state";

            sql += " LEFT JOIN mst_code_name lostplace";
            sql += " ON lostplace.company_no = lostitem.company_no";
            sql += " AND lostplace.division_code ='" + ((int)CommonEnum.CodeDivision.LostPlace).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += " AND lostplace.code = lostitem.found_place_code";

            sql += " LEFT JOIN mst_code_name storage";
            sql += " ON storage.company_no = lostitem.company_no";
            sql += " AND storage.division_code ='" + ((int)CommonEnum.CodeDivision.LostStrage).ToString(CommonConst.CODE_DIVISION_FORMAT) + "'";
            sql += " AND storage.code = lostitem.storage_code";

            sql += " WHERE lostitem.company_no = '" + lostItemListInfo.CompanyNo + "'";
            sql += " AND lostitem.status <> '" + CommonConst.STATUS_UNUSED + "'";

            // 画面表示時の情報取得
            if (lostItemListInfo.SearchWord == null && lostItemListInfo.FoundPlaceCode == null && lostItemListInfo.StorageCode == null) {
                sql += " AND state.default_flag_search = '" + ((int)CommonEnum.DefaultFlagDivision.ON).ToString() + "'";
            }

            // 簡易検索時の情報取得
            if (lostItemListInfo.SearchWord != null && lostItemListInfo.SearchWord != "")
            {
                string[] searchWords = lostItemListInfo.SearchWord.Trim().Replace("　", " ").Split(' ');
                searchWords = searchWords.Where(s => s != "").ToArray();
                for (int i = 0; i < searchWords.Count(); i++) {
                    searchWords[i] = SqlUtils.GetStringContainsPattern(SqlUtils.GetStringWithSqlWildcardsEscaped(SqlUtils.GetStringWithSqlEscaped(searchWords[i])));
                }

                foreach (var words in searchWords) {
                    var wkSql = " AND lostitem.search_word LIKE '{0}'".FillFormat(words);
                    sql += wkSql;
                }
            }
            // 詳細検索時の情報取得(忘れ物発見場所分類が選択された場合)
            if (lostItemListInfo.FoundPlaceCode != null && lostItemListInfo.FoundPlaceCode != "")
            {
                sql += " AND lostitem.found_place_code = '" + lostItemListInfo.FoundPlaceCode + "'";
            }
            // 詳細検索時の情報取得(忘れ物保管分類が選択された場合)
            if (lostItemListInfo.StorageCode != null && lostItemListInfo.StorageCode != "")
            {
                sql += " AND lostitem.storage_code = '" + lostItemListInfo.StorageCode + "'";
            }
            sql += " ORDER BY lostitem.cdt DESC, lostitem.management_no ASC";

            var lists = new List<TrnLostItemsBaseInfo>();
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
                            var list = new TrnLostItemsBaseInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ManagementNo = reader["management_no"].ToString();
                            list.ItemState = reader["item_state"].ToString();
                            list.ItemCategory = reader["item_category"].ToString();
                            list.ItemName = reader["item_name"].ToString();
                            list.FoundDate = reader["found_date"].ToString();
                            list.FoundTime = reader["found_time"].ToString();
                            list.FoundPlace = reader["found_place"].ToString();
                            list.Comment = reader["comment"].ToString();
                            list.SearchWord = reader["search_word"].ToString();
                            list.FoundPlaceCode = reader["found_place_code"].ToString();
                            list.StorageCode = reader["storage_code"].ToString();
                            list.ReserveNo = reader["reserve_no"].ToString();
                            list.RoomNo = reader["room_no"].ToString();

                            list.Status = reader["status"].ToString();
                            list.Version = int.Parse(reader["version"].ToString());
                            list.Creator = reader["creator"].ToString();
                            list.Updator = reader["updator"].ToString();
                            list.Cdt = reader["cdt"].ToString();
                            list.Udt = reader["udt"].ToString();

                            list.StateName = reader["item_state_name"].ToString();
                            list.FoundPlaceName = reader["lost_place_name"].ToString();
                            list.StorageName = reader["storage_name"].ToString();

                            check = reader["binary_data"].ToString();
                            if(check != "")
                            {
                                list.ImageData = (byte[])reader["binary_data"];
                                list.ImageContentType = reader["content_type"].ToString();
                            }
                            else
                            {
                                list.ImageData = null;
                                list.ImageContentType = null;
                            }
                            
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
        public async Task<TrnLostItemsBaseInfo> GetLostItemListById(TrnLostItemsBaseInfo lostItemListInfo)
        {
            string sql = "SELECT lostitem.*";
            sql += " FROM trn_lost_items_base lostitem";
            sql += " WHERE lostitem.company_no = '" + lostItemListInfo.CompanyNo + "'";
            sql += " AND lostitem.management_no = '" + lostItemListInfo.ManagementNo + "'";
            sql += " AND lostitem.status <> '" + CommonConst.STATUS_UNUSED + "'";


            var list = new TrnLostItemsBaseInfo();
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
                            list.ManagementNo = reader["management_no"].ToString();
                            list.ItemState = reader["item_state"].ToString();
                            list.ItemCategory = reader["item_category"].ToString();
                            list.ItemName = reader["item_name"].ToString();
                            list.FoundDate = reader["found_date"].ToString();
                            list.FoundTime = reader["found_time"].ToString();
                            list.FoundPlace = reader["found_place"].ToString();
                            list.Comment = reader["comment"].ToString();
                            list.SearchWord = reader["search_word"].ToString();
                            list.FoundPlaceCode = reader["found_place_code"].ToString();
                            list.StorageCode = reader["storage_code"].ToString();
                            list.ReserveNo = reader["reserve_no"].ToString();
                            list.RoomNo = reader["room_no"].ToString();

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

        // 情報取得(編集,削除用)
        public async Task<List<TrnLostItemsPictureInfo>> GetLostItemImage(TrnLostItemsBaseInfo lostItemListInfo)
        {
            string check;
            string sql = "SELECT lostitempicture.*";
            sql += " FROM trn_lost_items_picture lostitempicture";
            sql += " WHERE lostitempicture.company_no = '" + lostItemListInfo.CompanyNo + "'";
            sql += " AND lostitempicture.management_no = '" + lostItemListInfo.ManagementNo + "'";
            sql += " AND lostitempicture.status <> '" + CommonConst.STATUS_UNUSED + "'";
            sql += " ORDER BY lostitempicture.file_seq";


            var lists = new List<TrnLostItemsPictureInfo>();
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
                            var list = new TrnLostItemsPictureInfo();
                            list.CompanyNo = reader["company_no"].ToString();
                            list.ManagementNo = reader["management_no"].ToString();
                            list.FileSeq = int.Parse(reader["file_seq"].ToString());
                            list.ContentType = reader["content_type"].ToString();
                            list.FileName = reader["file_name"].ToString();
                            check = reader["binary_data"].ToString();
                            if (check != "")
                            {
                                list.BinaryData = (byte[])reader["binary_data"];
                            }
                            else
                            {
                                list.BinaryData = null;
                            }
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

        // 現在使用容量取得
        public async Task<long> GetUsingCapacity(TrnLostItemsBaseInfo lostItemListInfo)
        {
            try
            {
                long sum = 0;
                var defaultSearch = _context.LostItemsPictureInfo.Where(w => w.CompanyNo == lostItemListInfo.CompanyNo && w.BinaryData != null).ToList();

                sum = defaultSearch.Sum(c => c.BinaryData.Length);
                return sum;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 管理番号 採番
        /// </summary>
        private string Numbering(string companyNo)
        {
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

        // 情報追加
        public async Task<int> AddLostItem(TrnLostItemsBaseInfo lostItemListInfo)
        {
            // エラーフラグ → 「0」:正常 「1」:エラー
            int errFlg = 0;

            // 管理番号 採番
            lostItemListInfo.ManagementNo = Numbering(lostItemListInfo.CompanyNo);

            // 会社番号,コード一致するデータを取得
            var info = _context.LostItemsBaseInfo.Where(w => w.CompanyNo == lostItemListInfo.CompanyNo && w.ManagementNo == lostItemListInfo.ManagementNo).AsNoTracking().SingleOrDefault();

            if (info == null)
            {
                // データが存在しなかった場合 → 追加
                lostItemListInfo.Version = 0;
                lostItemListInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                lostItemListInfo.Udt = lostItemListInfo.Cdt;
                lostItemListInfo.Status = Context.CommonConst.STATUS_USED;

                _context.LostItemsBaseInfo.Add(lostItemListInfo);
                _context.SaveChanges();
            }
            else 
            {
                // データが存在し,Statusが「1」場合 → エラー
                errFlg = 1;
            }
            return errFlg;
        }

        // 情報更新
        public async Task<int> UpdateLostItem(TrnLostItemsBaseInfo lostItemListInfo, bool addFlag)
        {
            try
            {
                // versionチェック
                if (!addFlag)
                {
                    if (!await LostItemCheckVer(lostItemListInfo)) { return -1; }
                }

                lostItemListInfo.Version++;
                lostItemListInfo.Udt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");

                _context.LostItemsBaseInfo.Update(lostItemListInfo);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -99;
            }
        }

        // イメージ画像登録
        public async Task<int> AddLostItemPicture(List<TrnLostItemsPictureInfo> lostItemPictureList) {

            int retVal = 0;
            using (var tran = _context.Database.BeginTransaction()) {

                try {
                    // データ削除
                    string managementNo;
                    if (lostItemPictureList.First().ManagementNo == "")
                    {
                        managementNo = Numbering(lostItemPictureList.First().CompanyNo);
                        var delPictureList = _context.LostItemsPictureInfo.Where(d => d.CompanyNo == lostItemPictureList.First().CompanyNo && d.ManagementNo == managementNo).ToList();
                        foreach (var delPicture in delPictureList)
                        {
                            _context.LostItemsPictureInfo.Remove(delPicture);
                            retVal += _context.SaveChanges();
                        }
                    }
                    else
                    {
                        var delPictureList = _context.LostItemsPictureInfo.Where(d => d.CompanyNo == lostItemPictureList.First().CompanyNo && d.ManagementNo == lostItemPictureList.First().ManagementNo).ToList();
                        foreach (var delPicture in delPictureList)
                        {
                            _context.LostItemsPictureInfo.Remove(delPicture);
                            retVal += _context.SaveChanges();
                        }
                    }

                    // データ登録
                    foreach (var picInfo in lostItemPictureList) {
                        // バイナリデータがある場合のみ保存
                        if (picInfo.BinaryData != null) {
                            if (picInfo.ManagementNo == "") {
                                picInfo.ManagementNo = (int.Parse(Numbering(picInfo.CompanyNo)) - 1).ToString();
                            }
                            picInfo.Version = 0;
                            picInfo.Cdt = System.DateTime.Now.ToString("yyyyMMdd HHmmss");
                            picInfo.Udt = picInfo.Cdt;
                            picInfo.Status = Context.CommonConst.STATUS_USED;

                            _context.LostItemsPictureInfo.Add(picInfo);
                            retVal += _context.SaveChanges();
                        }
                    }

                    tran.Commit();

                    return retVal;

                } catch (Exception e) {
                    tran.Rollback();
                    throw e;
                }
            }
        }

        // イメージ画像最大容量チェック
        public async Task<int> IsOverMaxCapacity(List<TrnLostItemsPictureInfo> lostItemPictureList) {

            try {
                if (lostItemPictureList != null && lostItemPictureList.Count() > 0) {

                    int allPictureSize;

                    var companyNo = lostItemPictureList.First().CompanyNo;
                    var managementNo = lostItemPictureList.First().ManagementNo;

                    if (managementNo.IsNotBlanks()) {
                        // 自身以外の画像イメージを取得
                        var picList = _context.LostItemsPictureInfo.Where(w => w.CompanyNo == companyNo && w.ManagementNo != managementNo && w.BinaryData != null).ToList();

                        allPictureSize = picList.Sum(c => c.BinaryData.Length);

                    } else {
                        var picList = _context.LostItemsPictureInfo.Where(w => w.CompanyNo == companyNo && w.BinaryData != null).ToList();

                        allPictureSize = picList.Sum(c => c.BinaryData.Length);
                    }

                    // 自身以外の画像サイズに自身の画像サイズを加算
                    foreach (var picInfo in lostItemPictureList) {
                        if (picInfo.BinaryData != null ) {
                            allPictureSize += picInfo.BinaryData.Length;
                        }
                    }

                    int maxCapacity = _context.CompanyInfo.Where(w => w.CompanyNo == companyNo).Select(x => x.MaxCapacity).SingleOrDefault();

                    if (allPictureSize > maxCapacity) {
                        return 0;
                    } else if(allPictureSize > (maxCapacity * 0.8)) {
                        return 1;
                    } else {
                        return 2;
                    }
                } else {
                    return 0;
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return 0;
            }
        }


        // 削除
        public async Task<int> DelLostItem(TrnLostItemsBaseInfo lostItemListInfo)
        {
            try
            {
                int retVal = 0;
                // データ取得
                var delBase = _context.LostItemsBaseInfo.SingleOrDefault(d => d.CompanyNo == lostItemListInfo.CompanyNo && d.ManagementNo == lostItemListInfo.ManagementNo);
                if (delBase != null) { 
                    _context.LostItemsBaseInfo.Remove(delBase);

                    string sql = "DELETE FROM trn_lost_items_picture AS picture";
                    sql += " WHERE picture.company_no = '" + lostItemListInfo.CompanyNo + "'";
                    sql += " AND picture.management_no = '" + lostItemListInfo.ManagementNo + "'";
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = sql;
                        _context.Database.OpenConnection();
                        try
                        {
                            var count = command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        finally
                        {
                            _context.Database.CloseConnection();
                            retVal += _context.SaveChanges();
                        }
                    }
                }

                return retVal;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        // バージョンチェック
        private async Task<bool> LostItemCheckVer(TrnLostItemsBaseInfo lostItemListInfo)
        {
            try
            {
                // キーセット
                TrnLostItemsBaseInfo keyInfo = new TrnLostItemsBaseInfo() { CompanyNo = lostItemListInfo.CompanyNo, ManagementNo = lostItemListInfo.ManagementNo };

                // データ取得
                var info = await GetLostItemListById(keyInfo);

                // バージョン差異チェック
                if (lostItemListInfo.Version != info.Version)
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

        // 一括削除
        public async Task<int> LumpDelLostItem(TrnLostItemsBaseInfo lostItemListInfo)
        {
            try
            {
                #region trn_lost_items_pictureデータ削除

                string sql = "DELETE FROM trn_lost_items_picture";
                sql += " WHERE management_no IN(";
                sql += " SELECT management_no";
                sql += " FROM trn_lost_items_base AS lostitem";
                sql += " WHERE lostitem.company_no = '" + lostItemListInfo.CompanyNo + "'";
                // 忘れ物状態が選択された場合
                if (lostItemListInfo.ItemState != null) { 
                    sql += " AND lostitem.item_state = '" + lostItemListInfo.ItemState + "'";
                }
                // 更新日前が選択された場合
                if (lostItemListInfo.UdtBefore != null) { 
                sql += " AND lostitem.udt >= '" + lostItemListInfo.UdtBefore + " 000000'";
                }
                sql += " AND lostitem.udt <= '" + lostItemListInfo.UdtAfter + " 240000')";

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    _context.Database.OpenConnection();
                    try
                    {
                        var count = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        _context.Database.CloseConnection();
                        _context.SaveChanges();
                    }
                }

                #endregion

                #region trn_lost_items_baseデータ削除

                sql = "DELETE FROM trn_lost_items_base lostitem";
                sql += " WHERE lostitem.company_no = '" + lostItemListInfo.CompanyNo + "'";
                // 忘れ物状態が選択された場合
                if (lostItemListInfo.ItemState != null)
                {
                    sql += " AND lostitem.item_state = '" + lostItemListInfo.ItemState + "'";
                }
                // 更新日前が選択された場合
                if (lostItemListInfo.UdtBefore != null)
                {
                    sql += " AND lostitem.udt >= '" + lostItemListInfo.UdtBefore + " 000000'";
                }
                sql += " AND lostitem.udt <= '" + lostItemListInfo.UdtAfter + " 240000'";
                

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    _context.Database.OpenConnection();
                    try
                    {
                        var count = command.ExecuteNonQuery();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        _context.Database.CloseConnection();
                    }
                }                
                return _context.SaveChanges();

                #endregion
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}