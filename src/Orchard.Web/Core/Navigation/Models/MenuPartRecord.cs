using System.ComponentModel.DataAnnotations;

namespace Orchard.Core.Navigation.Models {
    public class MenuPartRecord {
        public int Id { get; set; }
        public const ushort DefaultMenuTextLength = 255;

        [StringLength(DefaultMenuTextLength)]
        public virtual string MenuText { get; set; }
        public virtual string MenuPosition { get; set; }
        public virtual int MenuId { get; set; }
    }
}