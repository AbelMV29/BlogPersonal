using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPersonal.Entities
{
    public class PostTag
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPostTag { get; set; }
        public int IdPost { get; set; }
        [ForeignKey(nameof(IdPost))]
        public Post Post { get; set; }
        public int IdTag { get; set; }
        [ForeignKey(nameof(IdTag))]
        public Tag Tag { get; set; }
    }


}

