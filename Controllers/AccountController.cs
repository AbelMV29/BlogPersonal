using BlogPersonal.Entities;
using BlogPersonal.Extras;
using BlogPersonal.Mappper;
using BlogPersonal.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BlogPersonal.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [AllowAnonymous,HttpGet("login")]
        public IActionResult Login([FromQuery] string returnUrl = "")
        {

            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            //empty or null
            if(!returnUrl.IsNullOrEmpty())
            {
                ViewData["returnUrl"] = returnUrl;
            }
            return View();
        }

        [HttpPost("login"),AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model,[FromQuery]string returnUrl = "") 
        {
            ViewData["returnUrl"] = returnUrl;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            //Validaciones lógicas
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var userByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userByEmail is null || !(await _userManager.CheckPasswordAsync(userByEmail, model.Password)))
            {
                ModelState.AddModelError("", "Correo y/o contraseña incorrectas");
                return View(model);
            }

            //Realizar el login
            var resultSignIn = await _signInManager.PasswordSignInAsync(userByEmail, model.Password,false,false);
            if (!resultSignIn.Succeeded)
            {
                ModelState.AddModelError("", "Error inesperado, intentelo más tarde");
                return View(model);
            }

            if(!returnUrl.IsNullOrEmpty())
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous,HttpGet("registro")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost("registro"),AllowAnonymous,ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            //Validaciones lógicas
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userByEmail is not null)
            {
                ModelState.AddModelError("", "Correo eléctronico ya registrado en el sistema");
                return View(model);
            }

            //Creando el usuario
            var appUser = Mapper.MapRegisterViewModelToAppUser(model);
            var resultCreate = await _userManager.CreateAsync(appUser, model.Password);

            if(!resultCreate.Succeeded)
            {
                ModelState.AddModelError("", "Error inesperado, intentelo más tarde");
                return View(model);
            }
            //Añadiendo rol
            var resultRoleAdd = await _userManager.AddToRoleAsync(appUser, Roles.User);
            if(!resultRoleAdd.Succeeded)
            {
                ModelState.AddModelError("", "Error inesperado, envie un correo al soporte mvabel23@gmail.com");
                return View(model);
            }
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
