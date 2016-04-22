using System;
using System.IO;

namespace Orchard.Data.Migration.Announcers
{
    public class TextWriterWithGoAnnouncer : TextWriterAnnouncer
    {
        public TextWriterWithGoAnnouncer(TextWriter writer)
            : base(writer)
        { }

        public  TextWriterWithGoAnnouncer(Action<string> write) 
            : base(write)
        { }

        public override void Sql(string sql)
        {
            if (!ShowSql) return;

            base.Sql(sql);

            if (!string.IsNullOrEmpty(sql))
                Write("GO", false);
        } 
    }
}
