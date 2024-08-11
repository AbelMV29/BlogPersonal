using BlogPersonal.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPersonal.Models.User
{
    public class CreateCommentViewModel
    {
        public string Body { get; set; }
        public IFormFile? MediaUrl { get; set; }
        public int IdPost { get; set; }
    }
}
