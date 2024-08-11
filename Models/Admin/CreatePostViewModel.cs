using BlogPersonal.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlogPersonal.Models.Admin
{
    public class CreatePostViewModel
    {
        public IFormFile? ImagePreview { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Body { get; set; }
        public string Category { get; set; }
    }
}
