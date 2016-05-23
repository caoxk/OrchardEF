using System.ComponentModel.DataAnnotations;
using Orchard.Settings.FieldStorage.InfosetStorage;

namespace Orchard.Settings.Records {
    public class ContentItemRecord {
        public ContentItemRecord() {
            Infoset = new Infoset();
        }

        public virtual int Id { get; set; }
        public virtual string ContentType { get; set; }

        [MaxLength]
        public virtual string Data { get { return Infoset.Data; } set { Infoset.Data = value; } }
        public virtual Infoset Infoset { get; protected set; }
    }
}
