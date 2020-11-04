using System.Text.RegularExpressions;
using YOUCOM.Commons.Extensions;

namespace YOUCOM.ReserVook.API.Context
{

    /// <summary>
    /// SQL補助
    /// </summary>
    public class SqlUtils
    {
        /// <summary>
        /// Assymbly情報
        /// </summary>
        private static System.Reflection.Assembly _assy = System.Reflection.Assembly.GetExecutingAssembly();

        /// <summary>SQLインジェクション対策で"'"を"'"でエスケープする</summary>
        /// <param name="field">The field.</param>
        /// <returns>escaped string</returns>
        public static string GetStringWithSqlEscaped(string field)
        {
            var result = Regex.Replace(field, @"[']", @"''");
            return result;
        }

        /// <summary>SQLのLIKE文で特別な意味を持つ"%"、"_"を"\"でエスケープする</summary>
        /// <param name="field">The field.</param>
        /// <returns>escaped string</returns>
        public static string GetStringWithSqlWildcardsEscaped(string field)
        {
            return Regex.Replace(field, @"[%_]", @"\$&");
        }

        /// <summary>前方一致</summary>
        /// <param name="field">The field.</param>
        /// <returns>field + "%"</returns>
        public static string GetStringStartWithPattern(string field)
        {
            return GetStringWithSqlWildcardsEscaped(field) + "%";
        }

        /// <summary>部分一致</summary>
        /// <param name="field">The field.</param>
        /// <returns>"%" + field + "%"</returns>
        public static string GetStringContainsPattern(string field)
        {
            if (field.IsNullOrEmpty()) return "%";
            return "%" + GetStringWithSqlWildcardsEscaped(field) + "%";
        }

        /// <summary>Matches the empty.</summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public static string GetStringEmptyPattern(string field)
        {
            return (string.IsNullOrEmpty(field)) ? "%" : GetStringWithSqlWildcardsEscaped(field);
        }

    }
}
