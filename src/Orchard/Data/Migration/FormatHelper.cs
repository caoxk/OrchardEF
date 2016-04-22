using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.Migration
{
    public class FormatHelper
    {
        public static string FormatSqlEscape(string sql)
        {
            return sql.Replace("'", "''");
        }
    }
}
