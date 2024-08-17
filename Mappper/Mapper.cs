using BlogPersonal.Entities;
using BlogPersonal.Models.Account;
using BlogPersonal.Models.Admin;
using BlogPersonal.Models.User;
using BlogPersonal.Models.View;

namespace BlogPersonal.Mappper
{
    public static class Mapper
    {
        public static AppUser MapRegisterViewModelToAppUser(RegisterViewModel model)
        {
            return new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                BirthDate = model.BirthDate.Value,
                UserName = string.Concat(model.FirstName, model.LastName).Replace(" ", "")
            };
        }

        public static Post MapCreatePostViewModelToPost(this CreatePostViewModel model)
        {
            return new Post
            {
                Body = model.Body,
                PublishDate = DateTime.Now,
                Title = model.Title,
                ShortDescription = model.ShortDescription,
                ImagePreview = null
            };
        }

        public static ShortPostViewModel MapPostToShortPostViewModel(this Post post)
        {
            return new ShortPostViewModel
            {
                Id = post.IdPost,
                PreviewImage = post.ImagePreview,
                PublishDate = post.PublishDate.ToString("dd MMM yyyy"),
                Title = post.Title,
                ShortDescription = post.ShortDescription
            };
        }

        public static List<ShortPostViewModel> MapListPostToListShortPostViewModel(this List<Post> posts)
        {
            return posts.Select(p => p.MapPostToShortPostViewModel()).ToList();
        }

        public static PostViewModel MapPostToPostViewModel(this Post post)
        {
            return new PostViewModel
            {
                Id = post.IdPost,
                PreviewImage = post.ImagePreview,
                PublishDate = post.PublishDate.ToString("dd MMM yyyy"),
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Body = post.Body,
                CategoryName = post.Category.Name
            };
        }

        public static Comment MapCreateCommentViewModelToComment(this CreateCommentViewModel model)
        {
            return new Comment
            {
                Body = model.Body,
                IdPost = model.IdPost,
                MediaUrl = null,
                PublishDate = DateTime.Now
            };
        }

        public static CommentViewModel MapCommentToCommentViewModel(this Comment comment)
        {
            return new CommentViewModel
            {
                IdComment = comment.IdComment,
                Body = comment.Body,
                MediaUrl = comment.MediaUrl,
                PublishDate = comment.PublishDate.ToString("dd MMM yyyy"),
                FullName = string.Concat(comment.AppUser.FirstName, " ", comment.AppUser.LastName),
                IdAppUser = comment.IdAppUser
            };
        }

        public static List<CommentViewModel> MapListCommentToCommentViewModel(this List<Comment> list)
        {
            return list.Select(c => c.MapCommentToCommentViewModel()).ToList();
        }
    }
}
