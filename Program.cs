using BlogPersonal.Data;
using BlogPersonal.DataAccess;
using BlogPersonal.Entities;
using BlogPersonal.Extras;
using BlogPersonal.Interfaces;
using BlogPersonal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.OutputCaching;

namespace BlogPersonal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options=>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnectionString"));
            });
            builder.Services.AddIdentity<AppUser, IdentityRole<int>>(optionsAppUser=>
            {
                optionsAppUser.Password.RequireUppercase = false;
                optionsAppUser.Password.RequireLowercase = false;
                optionsAppUser.Password.RequiredLength = 8;
                optionsAppUser.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<UserManager<AppUser>>();
            builder.Services.AddScoped<RoleManager<IdentityRole<int>>>();
            builder.Services.AddScoped<SignInManager<AppUser>>();
            builder.Services.AddScoped<ICloudService, CloudinaryPhotoService>();
            builder.Services.AddScoped<IRepository<AppUser>,Repository<AppUser>>();
            builder.Services.AddScoped<IRepository<Post>,Repository<Post>>();
            builder.Services.AddScoped<IRepository<Category>,Repository<Category>>();
            builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();

            builder.Services.AddAuthorization(configure =>
            {
                configure.AddPolicy(Roles.Admin, policyBulder => policyBulder.RequireRole(Roles.Admin));
                configure.AddPolicy(Roles.User, policyBulder => policyBulder.RequireRole(Roles.User));
            });
            //builder.Services.AddOutputCache(config=>
            //{
            //    config.addb
            //});

            builder.Services.AddMemoryCache();

            // Configurar JWT settings.
            var cloudSettings = new CloudinarySettings();
            //Obtiene la configuracion almacenada en appSettings.json de la key "JwtSettings":
            builder.Configuration.Bind("CloudinarySettings", cloudSettings);
            builder.Services.AddSingleton(cloudSettings);

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            await AddRoles(app);
            await InitAdminUser(app);
            await AddCategories(app);
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(); 
            //app.UseOutputCache();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapFallbackToController("Notfound", "Home");
            app.Run();
        }


        private static async Task AddRoles(WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
                string[] roles = { Roles.Admin, Roles.User};
                IdentityResult roleResult;

                foreach (var rol in roles)
                {
                    var roleExist = await roleManager.RoleExistsAsync(rol);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole<int>(rol));
                    }
                }

            }
        }

        private static async Task AddCategories(WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
            {
                var categoryRep = scope.ServiceProvider.GetRequiredService<IRepository<Category>>();

                var count = categoryRep.GetAll().Count();
                if(count == 0)
                {
                    await categoryRep.AddAsync(new Category
                    {
                        Name = "Tecnologia"
                    });
                    await categoryRep.SaveChangesAsync();

                    await categoryRep.AddAsync(new Category
                    {
                        Name = "Entrenamiento"
                    });
                    await categoryRep.SaveChangesAsync();
                }
                
            }
        }
        private static async Task InitAdminUser(WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var section = webApp.Configuration.GetSection("AdminUser");
                string emailAdmin = section.GetValue<string>("Email");
                if ((await service.FindByEmailAsync(emailAdmin)) is null)
                {
                    var appAdmin = new AppUser
                    {
                        Email = emailAdmin,
                        BirthDate = section.GetValue<DateTime>("BirthDate"),
                        FirstName = section.GetValue<string>("FirstName"),
                        LastName = section.GetValue<string>("LastName"),
                        UserName = section.GetValue<string>("UserName")
                    };
                    var resultCreate = await service.CreateAsync(appAdmin, section.GetValue<string>("Password"));
                    if (resultCreate.Succeeded)
                    {
                        var result = await service.AddToRoleAsync(appAdmin, Roles.Admin);
                    }

                }
            }

        }
    }

    public class OutputCachePolicy : IOutputCachePolicy
    {

        public static readonly OutputCachePolicy Instance = new();

        private OutputCachePolicy()
        {
        }

        ValueTask IOutputCachePolicy.CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
        {
            var attemptOutputCaching = AttemptOutputCaching(context);
            context.EnableOutputCaching = true;
            context.AllowCacheLookup = attemptOutputCaching;
            context.AllowCacheStorage = attemptOutputCaching;
            context.AllowLocking = true;

            // Vary by any query by default
            context.CacheVaryByRules.QueryKeys = "*";
            return ValueTask.CompletedTask;
        }
        // this never gets hit when Authorization is present
        ValueTask IOutputCachePolicy.ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
        {
            return ValueTask.CompletedTask;
        }

        ValueTask IOutputCachePolicy.ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
        {
            var response = context.HttpContext.Response;
            context.AllowCacheStorage = true;

            return ValueTask.CompletedTask;
        }

        private static bool AttemptOutputCaching(OutputCacheContext context)
        {
            // Check if the current request fulfills the requirements to be cached

            var request = context.HttpContext.Request;

            // Verify the method
            if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
            {
                return false;
            }

            // Verify existence of authorization headers
            //if (!StringValues.IsNullOrEmpty(request.Headers.Authorization) || request.HttpContext.User?.Identity?.IsAuthenticated == true)
            //{
            //    return false;
            //}
            return true;
        }
    }
}
