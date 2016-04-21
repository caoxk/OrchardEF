using System.ComponentModel.DataAnnotations;

namespace Orchard.Core.Navigation.Models {
    public class AdminMenuPartRecord {
        public virtual  int Id { get; set; }
        public const ushort DefaultMenuTextLength = 255;

        [StringLength(DefaultMenuTextLength)]
        public virtual string AdminMenuText { get; set; }
        public virtual string AdminMenuPosition { get; set; }
        public virtual bool OnAdminMenu { get; set; }
    }
}