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
                var firstRun = await _context.FirstRun.OrderBy(x => x.Id).FirstOrDefaultAsync();

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

        // GET: api/GetFirstRun/default_user
        [Authorize(Roles = "Admin")]
        [HttpGet("default_user")]
        public async Task<ActionResult<IEnumerable<FirstRun>>> DefaultUserStatus()
        {
            try
            {
                var defaultUser = await userManager.FindByEmailAsync("defaultUser@mail.com");

                if (defaultUser == null)
                {
                    _logger.LogInformation("Default user does not exist.");
                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Default User does not exist.",
                        exist = false
                    }
                    );
                }
                else
                {
                    _logger.LogInformation("Default user exist.");

                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Default User Exist",
                        exist = true
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

        // GET: api/GetFirstRun/roles
        [Authorize(Roles = "Admin")]
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<FirstRun>>> RolesStatus()
        {
            try
            {
                var adminRole = await roleManager.FindByNameAsync("Admin");
                var userRole = await roleManager.FindByNameAsync("User");
                var editorRole = await roleManager.FindByNameAsync("Editor");

                bool adminExist = false;
                bool userExist = false;
                bool editorExist = false;

                if (adminRole != null)
                {
                    _logger.LogInformation("Admin Role Exist.");
                    adminExist = true;
                }
                else
                {
                    _logger.LogInformation("Admin Role does not exist.");
                    adminExist = false;
                }

                if (userRole != null)
                {
                    _logger.LogInformation("User Role Exist.");
                    userExist = true;
                }
                else
                {
                    _logger.LogInformation("User Role does not exist.");
                    userExist = false;
                }

                if (editorRole != null)
                {
                    _logger.LogInformation("Editor Role Exist.");
                    editorExist = true;
                }
                else
                {
                    _logger.LogInformation("Editor Role does not exist.");
                    editorExist = false;
                }

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Default User does not exist.",
                    adminExist = adminExist,
                    userExist = userExist,
                    editorExist = editorExist
                }
                );

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // GET: api/GetFirstRun/default_category
        [Authorize(Roles = "Admin")]
        [HttpGet("default_category")]
        public async Task<ActionResult<IEnumerable<FirstRun>>> DefaultCategoryStatus()
        {
            try
            {
                var defaultCategory = await _context.Category.Where(x => x.Name == "Default").FirstOrDefaultAsync();

                if (defaultCategory == null)
                {
                    _logger.LogInformation("Default Category does not exist.");
                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Default Category does not exist.",
                        exist = false
                    }
                    );
                }
                else
                {
                    _logger.LogInformation("Default Category exist.");

                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Default Category Exist",
                        exist = true
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

        // GET: api/GetFirstRun/add/roles
        [Authorize(Roles = "Admin")]
        [HttpGet("add/roles")]
        public async Task<ActionResult<FirstRun>> AddRoles()
        {
            try
            {
                var adminRole = await roleManager.FindByNameAsync("Admin");
                var userRole = await roleManager.FindByNameAsync("User");
                var editorRole = await roleManager.FindByNameAsync("Editor");

                if (adminRole == null)
                {
                    adminRole = new IdentityRole("Admin");
                    await roleManager.CreateAsync(adminRole);

                    _logger.LogInformation("Admin Role Created.");
                }

                if (userRole == null)
                {
                    userRole = new IdentityRole("User");
                    await roleManager.CreateAsync(userRole);

                    _logger.LogInformation("User Role Created.");
                }

                if (editorRole == null)
                {
                    editorRole = new IdentityRole("Editor");
                    await roleManager.CreateAsync(editorRole);

                    _logger.LogInformation("Editor Role Created.");
                }

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Roles Initialized.",
                }
                );
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // GET: api/GetFirstRun/add/default_user
        [Authorize(Roles = "Admin")]
        [HttpGet("add/default_user")]
        public async Task<ActionResult<FirstRun>> AddDefaultUser()
        {
            try
            {
                var defaultUser = await userManager.FindByEmailAsync("defaultUser@mail.com");

                if (defaultUser == null)
                {
                    ApplicationUser user = new ApplicationUser
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

                    // Generate a random password with a minimum of 8 characters having 1 upper case, 1 lower case, 1 number and 1 special character
                    string defaultPassword = Regex.Replace(Guid.NewGuid().ToString(), @"[^a-zA-Z0-9]", m => "") + "!@#$&()AT";


                    // Create the default user
                    var createUser = await userManager.CreateAsync(user, defaultPassword);

                    if (!createUser.Succeeded)
                    {
                        _logger.Log(LogLevel.Error, createUser.Errors.ToString());

                        return BadRequest(new
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "Default User Creation Failed."
                        });
                    }

                    // Assign role to Default User
                    await userManager.AddToRoleAsync(defaultUser, UserRoles.Editor);

                    _logger.LogInformation("Default User Created.");
                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Default User Created.",
                    });

                }
                else
                {
                    _logger.LogInformation("Default User Exist.");

                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Default User Exist",
                    });
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
                var firstRun = await _context.FirstRun.OrderBy(x => x.Id).FirstOrDefaultAsync();

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

                // Check if Admin Role Exist, if not, create it unless skip
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                    _logger.LogInformation("Admin Role Created.");
                }

                // Check if User Role Exist, if not, create it unless skip
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                    _logger.LogInformation("User Role Created.");
                }

                // Check if Editor Role Exist, if not, create it unless skip
                if (!await roleManager.RoleExistsAsync(UserRoles.Editor))
                {
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Editor));

                    _logger.LogInformation("Editor Role Created.");
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

                // Generate a random password with a minimum of 8 characters having 1 upper case, 1 lower case, 1 number and 1 special character
                string defaultPassword = Regex.Replace(Guid.NewGuid().ToString(), @"[^a-zA-Z0-9]", m => "") + "!@#$&()AT";


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
                await userManager.AddToRoleAsync(defaultUser, UserRoles.Editor);
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
