using System.ComponentModel.DataAnnotations;

namespace Orchard.Tests.Settings.Records {
    public class MegaRecord {
        public virtual int Id { get; set; }

        [MaxLength]
        public virtual string BigStuff { get; set; }
        public virtual string SmallStuff { get; set; }

    }
}