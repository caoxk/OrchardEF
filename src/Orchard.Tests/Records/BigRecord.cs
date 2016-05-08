using System.ComponentModel.DataAnnotations;

namespace Orchard.Tests.Records {
    public class BigRecord {
        public virtual int Id { get; set; }
        [MaxLength]
        public virtual string Body { get; set; }

        [MaxLength]
        public virtual byte[] Banner { get; set; }
    }
}