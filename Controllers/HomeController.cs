using BlogPersonal.Entities;
using BlogPersonal.Interfaces;
using BlogPersonal.Mappper;
using BlogPersonal.Models;
using BlogPersonal.Models.User;
using BlogPersonal.Models.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;

namespace BlogPersonal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository<Post> _repositoryPost;
        private readonly IRepository<Comment> _repositoryComment;
        private readonly ICloudService _cloudService;
        private readonly IMemoryCache _memoryCache;

        public HomeController(ILogger<HomeController> logger,IRepository<Post> repositoryPost,
            IRepository<Comment> repositoryComment,ICloudService cloudService,IMemoryCache memoryCache)
        {
            _logger = logger;
            _repositoryPost = repositoryPost;
            _repositoryComment = repositoryComment;
            _cloudService = cloudService;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            string cacheKey = "indexHome";
            if(!_memoryCache.TryGetValue(cacheKey,out List<ShortPostViewModel> tenShortPostViewModel))
            {
                var tenPosts = _repositoryPost.GetAll().OrderByDescending(p => p.PublishDate).Take(10).ToList();
                tenShortPostViewModel = tenPosts.MapListPostToListShortPostViewModel();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(10));

                _memoryCache.Set(cacheKey, tenShortPostViewModel,cacheEntryOptions);
            }
            
            return View(tenShortPostViewModel);
        }
        [HttpGet("posts")]
        public async Task<IActionResult> Posts()
        {
            _memoryCache.Remove("indexHome");
            return View();
        }

        [HttpGet("postsQuery")]
        public async Task<Object> Posts(
           [FromQuery] string name,
           [FromQuery] string category,
           [FromQuery] int page = 1,
           [FromQuery] string order = "desc")
        {
           var postsQuery = _repositoryPost.GetAll();

           if(!name.IsNullOrEmpty())
           {
               postsQuery = postsQuery.Where(p => p.Title.Contains(name));
           }
           if(category == "Tecnologia" || category == "Entrenamiento")
           {
               postsQuery = postsQuery.Include(p => p.Category).Where(p => p.Category.Name == category);
           }
           postsQuery = (order == "desc")? postsQuery.OrderByDescending(p => p.PublishDate) : postsQuery.OrderBy(p => p.PublishDate);

            int totalPosts = await postsQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalPosts / 9);

            var result = await postsQuery.Skip((page - 1)*9).Take(9).ToListAsync();
            
            var listView = result.MapListPostToListShortPostViewModel();

            return new {list = listView,totalPages,currentPage = page };
        }

        [HttpGet("post/{id}")]
        public async Task<IActionResult> Post(int id)
        {
            var post = await _repositoryPost.GetAll().Include(p => p.Category).FirstOrDefaultAsync(p => p.IdPost == id);
            if(post is null)
                return RedirectToAction("Notfound");
            var comments = await _repositoryComment.GetAll().Include(c=>c.AppUser).Where(c => c.IdPost == id).OrderByDescending(c=>c.PublishDate).ToListAsync();
            var postToShow = post.MapPostToPostViewModel();
            postToShow.Comments = comments.MapListCommentToCommentViewModel();
            
            return View(postToShow);
        }

        [HttpGet("notfound")]
        public IActionResult Notfound()
        {
            return View();
        }

        [HttpPost("comment"),Authorize]
        public async Task<IActionResult> Comment([FromForm] CreateCommentViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var postsToAddCommment = await _repositoryPost.GetByIdAsync(model.IdPost);
            if (postsToAddCommment is null)
                return BadRequest();
            var commentToAdd = model.MapCreateCommentViewModelToComment();
            commentToAdd.IdAppUser = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(model.MediaUrl is not null && model.MediaUrl.Length > 0)
            {
                commentToAdd.MediaUrl = await _cloudService.UploadMedia(model.MediaUrl);
            }
            try
            {
                await _repositoryComment.AddAsync(commentToAdd);
                await _repositoryComment.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }

            var commentAdded = _repositoryComment.GetAll().Include(c => c.AppUser).First(c=>c.IdComment == commentToAdd.IdComment);

            var commentResult = commentAdded.MapCommentToCommentViewModel();
            
            return Ok(commentResult);
        }

        [HttpDelete("comment/{idComment}"),Authorize]
        public async Task<IActionResult> Comment(int idComment)
        {
            var comment = await _repositoryComment.GetByIdAsync(idComment);
            if(comment is null) return BadRequest();
            if(comment.IdAppUser != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return BadRequest();
            await _repositoryComment.DeleteAsync(comment);
            await _repositoryComment.SaveChangesAsync();
            return Ok();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
