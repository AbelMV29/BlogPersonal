namespace BlogPersonal.Models.View
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string PreviewImage { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string PublishDate { get; set; }
        public string Body { get; set; }
        public string CategoryName { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}
