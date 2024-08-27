using BlogPersonal.Entities;
using BlogPersonal.Extras;
using BlogPersonal.Interfaces;
using BlogPersonal.Mappper;
using BlogPersonal.Models.Admin;
using BlogPersonal.Models.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlogPersonal.Controllers
{
    [Authorize(Policy =Roles.Admin)]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IRepository<AppUser> _repositoryAppUser;
        private readonly IRepository<Post> _repositoryPost;
        private readonly ICloudService _cloudService;
        private readonly IRepository<Category> _repositoryCategory;
        private readonly IMemoryCache _memoryCache;

        public AdminController(IRepository<AppUser> repositoryAppUser,IRepository<Post> repositoryPost,
            ICloudService cloudService, IRepository<Category> repositoryCategory,IMemoryCache memoryCache)
        {
            _repositoryAppUser = repositoryAppUser;
            _repositoryPost = repositoryPost;
            _cloudService = cloudService;
            _repositoryCategory = repositoryCategory;
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("user")]
        public async Task<IActionResult> ListUser(CancellationToken cancellationToken = default)
        {
            await Task.Delay(15000, cancellationToken);
            string cacheKey = "listUser";
            if (!_memoryCache.TryGetValue(cacheKey, out List<AppUser> users))
            {
                users = await _repositoryAppUser.GetAll().ToListAsync(cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(10));

                _memoryCache.Set(cacheKey, users, cacheEntryOptions);
            }
            
            return View(users);
        }

        [HttpGet("post")]
        public async Task<IActionResult> ListPost()
        {
            var posts = await _repositoryPost.GetAll().OrderByDescending(p => p.PublishDate).ToListAsync();
            var postsView = posts.MapListPostToListShortPostViewModel();
            return View(postsView);
        }


        [HttpGet("create/post")]
        public async Task<IActionResult> PublishPost()
        {
            var categories = await _repositoryCategory.GetAll().ToListAsync();
            var categorieslist = new List<SelectListItem>();
            foreach(var item in categories)
            {
                categorieslist.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name
                });
            }
            ViewBag.Categories = categorieslist;
            return View();
        }

        [HttpPost("create/post")]
        public async Task<IActionResult> PublishPost(CreatePostViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var postToCreate = model.MapCreatePostViewModelToPost();
            postToCreate.IdCategory = _repositoryCategory.GetAll().First(c => c.Name == model.Category).IdCategory;
            if(model.ImagePreview is not null && model.ImagePreview.Length>0)
            {
                var stringUrl = await _cloudService.UploadPreviewPost(model.ImagePreview);
                if(!string.IsNullOrEmpty(stringUrl))
                {
                    postToCreate.ImagePreview = stringUrl;
                }
            }

            await _repositoryPost.AddAsync(postToCreate);
            await _repositoryPost.SaveChangesAsync();
            var url = Url.Action("PublishPost","Admin");
            
            TempData["success"] = "Publicacion exitosa!";
            _memoryCache.Remove("indexHome");
            return RedirectToAction("Index");
        }

        [HttpPost("uploadPhoto")]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile imagen)
        {
            if(imagen is not null && imagen.Length >0)
            {
                var result = await _cloudService.UploadMedia(imagen);
                return Ok(result);
            }
            return StatusCode(400);
        }

        //DELETE
        [HttpGet("delete/post")]
        public async Task<IActionResult> DeletePost(int idPost)
        {
            var postToDelete = await _repositoryPost.GetByIdAsync(idPost);
            if(postToDelete is null)
            {
                return RedirectToAction("NotFound","Home");
            }

            await _repositoryPost.DeleteAsync(postToDelete);
            await _repositoryPost.SaveChangesAsync();
            TempData["success"] = "Eliminación exitosa";
            _memoryCache.Remove("indexHome");
            return RedirectToAction("ListPost");
        }
    }


}
