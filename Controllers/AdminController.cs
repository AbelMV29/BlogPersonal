using BlogPersonal.Entities;
using BlogPersonal.Extras;
using BlogPersonal.Interfaces;
using BlogPersonal.Mappper;
using BlogPersonal.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        public AdminController(IRepository<AppUser> repositoryAppUser,IRepository<Post> repositoryPost,
            ICloudService cloudService, IRepository<Category> repositoryCategory)
        {
            _repositoryAppUser = repositoryAppUser;
            _repositoryPost = repositoryPost;
            _cloudService = cloudService;
            _repositoryCategory = repositoryCategory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("user")]
        public async Task<IActionResult> ListUser()
        {
            var users = await _repositoryAppUser.GetAll().ToListAsync();
            return View(users);
        }

        [HttpGet("create/post")]
        public async Task<IActionResult> PublishPost()
        {
            var categories = await _repositoryCategory.GetAll().ToListAsync();
            ViewBag.Categories = new List<SelectListItem>()
            {   new SelectListItem()
                {
                    Text = categories[0].Name,
                    Value = categories[0].Name
                },
                new SelectListItem
                {
                    Text = categories[1].Name,
                    Value = categories[1].Name
                }
            } as List<SelectListItem>;
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

            return RedirectToAction("Index");
        }
    }
}
