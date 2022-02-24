using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Data;
using InforumBackend.Models;
using Microsoft.AspNetCore.Authorization;
using InforumBackend.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace InforumBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirstRunController : ControllerBase
    {
        private readonly InforumBackendContext _context;

        private readonly UserManager<ApplicationUser> userManager;

        private readonly RoleManager<IdentityRole> roleManager;

        private readonly ILogger _logger;

        public FirstRunController(InforumBackendContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILoggerFactory logger)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _logger = logger.CreateLogger("FirstRunController");
        }

        // GET: api/GetFirstRun
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FirstRun>>> FirstRunStatus()
        {
            try
            {
                var firstRun = await _context.FirstRun.FirstOrDefaultAsync();

                if (firstRun == null)
                {
                    _logger.LogInformation("First Run not Performed");
                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "First Run not Performed.",
                        IsOpen = true
                    }
                    );
                }
                else
                {
                    _logger.LogInformation("Access Denied.");

                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "First Run Finsihed",
                        IsOpen = false
                    }
                    );
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // POST: api/GetFirstRun
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FirstRun>> FirstRun([FromBody] RegisterModel firstAdmin)
        {
            try
            {
                var firstRun = await _context.FirstRun.FirstOrDefaultAsync();

                if (firstRun != null)
                {
                    _logger.LogInformation("Access Denied.");

                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Route Locked and cannot be accessed."
                    }
                    );
                }

                ApplicationUser newAdmin = new ApplicationUser
                {
                    UserName = firstAdmin.Email,
                    Email = firstAdmin.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = firstAdmin.FirstName,
                    LastName = firstAdmin.LastName,
                    Gender = firstAdmin.Gender,
                    ProfileImage = "https://res.cloudinary.com/inforum/image/upload/v1645625776/Defaults/profile_image_dummy_oawg87.png",
                    IsRestricted = false,
                    Address = "",
                    DOB = ""
                };

                var result = await userManager.CreateAsync(newAdmin, firstAdmin.Password);

                if (!result.Succeeded)
                {
                    _logger.Log(LogLevel.Error, result.Errors.ToString());

                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Admin Creation Failed."
                    });
                }

                // Check if roles Exist, if not, create them unless skip
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin) && !await roleManager.RoleExistsAsync(UserRoles.User) && !await roleManager.RoleExistsAsync(UserRoles.Editor))
                {
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Editor));

                    _logger.LogInformation("Roles Created.");
                }

                // Add newAdmin to Admin Role
                await userManager.AddToRoleAsync(newAdmin, UserRoles.Admin);

                _logger.LogInformation("Admin Created.");

                // Create a Default User
                ApplicationUser defaultUser = new ApplicationUser
                {
                    UserName = "defaultUser@mail.com",
                    Email = "defaultUser@mail.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Default",
                    LastName = "User",
                    Gender = Genders.Male,
                    ProfileImage = "https://res.cloudinary.com/inforum/image/upload/v1645625776/Defaults/profile_image_dummy_oawg87.png",
                    IsRestricted = false,
                    Address = "Void City, Near Eye, Black Hole",
                    DOB = ""
                };

                // Generate a random password
                string defaultPassword = Guid.NewGuid().ToString().Substring(0, 8);

                // Create the default user
                var createDefaultUser = await userManager.CreateAsync(defaultUser, defaultPassword);

                if (!createDefaultUser.Succeeded)
                {
                    _logger.Log(LogLevel.Error, createDefaultUser.Errors.ToString());

                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Default User Creation Failed."
                    });
                }

                // Assign role to Default User
                await userManager.AddToRoleAsync(defaultUser, UserRoles.User);
                _logger.LogInformation("Default User Created.");

                // Initialize base Category
                Category generalCat = new Category
                {
                    Name = "General",
                    Slug = ""
                };

                var genSlug = generateSlug(generalCat.Name);

                generalCat.Slug = genSlug;

                await _context.Category.AddAsync(generalCat);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Base Category Created.");

                // Initialize base Home Data
                var newHome = new Home
                {
                    SubHeading = "For Students made by Students.",
                    HeaderImage = "https://res.cloudinary.com/inforum/image/upload/v1644820069/Defaults/img-1_nvdef7.jpg"
                };

                await _context.Home.AddAsync(newHome);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Base Home Data Created.");

                // Lock Route
                var newFirstRun = new FirstRun
                {
                    IsFinished = true
                };

                await _context.FirstRun.AddAsync(newFirstRun);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Route Locked.");

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "First Run Succeeded. This Route will be locked now."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        private bool HomeExists(long id)
        {
            return _context.Home.Any(e => e.Id == id);
        }

        // Generate Slugs
        private string generateSlug(string title)
        {
            var slug = title.ToLower();

            // remove all uneeded characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            // remove multiple spaces
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            // replace spaces with dashes(-)
            slug = Regex.Replace(slug, @"\s", "-");

            return slug;
        }
    }
}
