using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Orchard.DocumentManagement.FieldStorage.InfosetStorage;

namespace Orchard.DocumentManagement.Records {
    public class DocumentItemRecord {
        public DocumentItemRecord() {
            Infoset = new Infoset();
        }

        public virtual int Id { get; set; }
        public virtual string ContentType { get; set; }

        [MaxLength]
        public virtual string Data { get { return Infoset.Data; } set { Infoset.Data = value; } }
        [NotMapped]
        public virtual Infoset Infoset { get; protected set; }
    }
}
