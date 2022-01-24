using InforumBackend.Authentication;
using InforumBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InforumBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        // Logs user in and returns a JWT token
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>{
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddDays(7),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                    return Ok(new
                    {
                        id = user.Id,
                        role = userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
                return NotFound(new
                {
                    Status = StatusCodes.Status404NotFound,
                    message = "Email or password is incorrect."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // Register a new user
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var userExists = await userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                {
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User already exists!"
                    });
                }

                ApplicationUser user = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User creation failed! Please check user data and try again."
                    });
                }

                // if User Role does not exist
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                {
                    // first create the User role
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                    // then assign user the User role
                    await userManager.AddToRoleAsync(user, UserRoles.User);
                }

                // if User role exist
                if (await roleManager.RoleExistsAsync(UserRoles.User))
                {
                    // assign user the User role
                    await userManager.AddToRoleAsync(user, UserRoles.User);
                }

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "User created successfully!"
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // Register a New Admin
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            try
            {
                var userExists = await userManager.FindByEmailAsync(model.Email);

                if (userExists != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User already exists!"
                    });
                }

                ApplicationUser user = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User creation failed! Please check user details and try again."
                    });
                }

                // if Admin role does not exist
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    // first create Admin role
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                    // then assign user the Admin role
                    await userManager.AddToRoleAsync(user, UserRoles.Admin);
                }

                // if Admin role exist
                if (await roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    // assogn user the Admin role
                    await userManager.AddToRoleAsync(user, UserRoles.Admin);
                }

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Admin created successfully!"
                });
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }


        // Register a New Editor
        [HttpPost]
        [Route("register-editor")]
        public async Task<IActionResult> RegisterEditor([FromBody] RegisterModel model)
        {
            try
            {
                var userExists = await userManager.FindByEmailAsync(model.Email);

                if (userExists != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User already exists!"
                    });
                }

                ApplicationUser user = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User creation failed! Please check user details and try again."
                    });
                }

                // if Editor role does not exist
                if (!await roleManager.RoleExistsAsync(UserRoles.Editor))
                {
                    // firs create Editor role
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Editor));

                    // then assign user the Editor role
                    await userManager.AddToRoleAsync(user, UserRoles.Editor);
                }

                // if Editor role exist
                if (await roleManager.RoleExistsAsync(UserRoles.Editor))
                {
                    // assign user the Editor role
                    await userManager.AddToRoleAsync(user, UserRoles.Editor);
                }

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Editor created successfully!"
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // List all Registered Users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetUsers([FromQuery] PageParameter pageParameter)
        {
            try
            {
                var usersList = userManager.Users.OrderByDescending(ul => ul.DateJoined);

                var paginationMetadata = new PaginationMetadata(usersList.Count(), pageParameter.PageNumber, pageParameter.PageSize);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                var users = await usersList.Skip((pageParameter.PageNumber - 1) * pageParameter.PageSize).Take(pageParameter.PageSize).ToListAsync();


                return Ok(new
                {
                    users = users,
                    pagination = paginationMetadata
                });
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }

        // List Single User as per the id
        [Authorize]
        [HttpGet]
        [Route("single/{id}")]
        public async Task<IActionResult> GetSingleUser(string id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);
                var userRole = await userManager.GetRolesAsync(user);

                if (user == null)
                {
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "User not found."
                    });
                }

                return Ok(new
                {
                    user = user,
                    userRole = userRole
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // Update User Data
        [Authorize]
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUser model)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);

                var requestUser = await userManager.FindByNameAsync(User.Identity.Name);

                var requestUserRole = await userManager.GetRolesAsync(requestUser);

                if (requestUser.UserName == user.UserName || requestUserRole.Contains(UserRoles.Admin))
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Gender = model.Gender;
                    user.Email = model.Email;
                    user.UserName = model.Email;

                    var result = await userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "Failed to Update User! Please check user details and try again."
                        });
                    }

                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "User Updated Successfully!"
                    });
                }

                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "You are not authorized to update this user!"
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // Change Password
        [Authorize]
        [HttpPost("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] UpdatePassword model)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);

                var requestUser = await userManager.FindByNameAsync(User.Identity.Name);

                var requestUserRole = await userManager.GetRolesAsync(requestUser);

                if (requestUser.UserName == user.UserName || requestUserRole.Contains(UserRoles.Admin))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var result = await userManager.ResetPasswordAsync(user, token, model.Password);

                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "Failed to Change Password! Please try again."
                        });
                    }

                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Password Changed Successfully!"
                    });
                }
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "You are not authorized to perform this action!"
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // Returns currently logged in user Info as per Token
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            try
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var userRole = await userManager.GetRolesAsync(user);

                if (user == null)
                {
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "User not found"
                    });
                }

                return Ok(new
                {
                    user = user,
                    userRole = userRole
                });
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }
    }

}
