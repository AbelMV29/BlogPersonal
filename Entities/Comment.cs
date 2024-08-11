using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPersonal.Entities
{
    public class Comment
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdComment { get; set; }
        public string Body { get; set; }
        public string? MediaUrl { get; set; }
        public DateTime PublishDate { get; set; }
        public int IdAppUser { get; set; }
        
        public int IdPost { get; set; }
        
        [ForeignKey(nameof(IdAppUser))]
        public AppUser AppUser { get; set; }
        [ForeignKey(nameof(IdPost))]
        public Post Post { get; set; }
    }


}

