using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchard.DocumentManagement.Records {
    public abstract class DocumentPartRecord {
        [Key,ForeignKey("ContentItemRecord")]
        public virtual int Id { get; set; }
        [Required]
        public virtual DocumentItemRecord ContentItemRecord { get; set; }
    }
}
