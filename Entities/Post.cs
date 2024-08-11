using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPersonal.Entities
{
    public class Post
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPost { get; set; }
        public string ImagePreview { get; set; }
        public string Title { get; set; }
        public string? ShortDescription { get; set; }
        public string Body { get; set; }
        public DateTime PublishDate { get; set; }
        public int IdCategory { get; set; }
        [ForeignKey(nameof(IdCategory))]
        public Category Category { get; set; }
    }


}

