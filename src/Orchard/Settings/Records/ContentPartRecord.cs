using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchard.Settings.Records {
    public abstract class ContentPartRecord {
        [Key,ForeignKey("ContentItemRecord")]
        public virtual int Id { get; set; }
        [Required]
        public virtual ContentItemRecord ContentItemRecord { get; set; }
    }
}
