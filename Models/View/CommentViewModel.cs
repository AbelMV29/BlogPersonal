namespace BlogPersonal.Models.View
{
    public class CommentViewModel
    {
        public int IdComment { get; set; }
        public string FullName { get; set; }
        public string Body { get; set; }
        public string? MediaUrl { get; set; }
        public string PublishDate { get; set; }
        public int IdAppUser { get; set; }
    }
}
