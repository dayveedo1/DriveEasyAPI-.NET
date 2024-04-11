using DriveEasy.API.DriveEasy.Dto;
using DriveEasy.API.DriveEasy.Interface;
using DriveEasy.API.DriveEasy.Models;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//using BC = BCrypt.Net.BCrypt;

namespace DriveEasy.API.DriveEasy.Repo
{
    public class AuthImpl : IAuth
    {

        /* DbContext DI */
        private readonly DriveEasyApiDbContext context;

        /* IdentityCore DI */
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public IConfiguration config;

        private User? user;

        public AuthImpl(DriveEasyApiDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.config = config;
        }



        #region JWT

        /* Func to create JWT Token */
        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            try
            {
                var jwtSettings = config.GetSection("JWT");
                var key = jwtSettings.GetSection("SKEY").Value;

                var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = config.GetSection("JWT");
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("lifetime").Value));

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("Issuer").Value,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials);

            return token;
        }

        #endregion


        #region Authentication

        public async Task<ViewApiResponse> CreateUserAccount(UserDto userDto)
        {
            if (userDto is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            var isUserExist = await context.Users.AnyAsync(x => x.Email.Equals(userDto.Email));
            if (isUserExist)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request, User already exist",
                    ResponseData = null
                };

            //var hashedPassword = HashPassword(userDto.Password).ToUpper();

            User user = new()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Sex = userDto.Sex,
                DateOfBirth = userDto.DateOfBirth,
                Address = userDto.Address,
                City = userDto.City,
                State = userDto.State,
                ZipCode = userDto.ZipCode,
                Country = userDto.Country,
                DriverLicence = userDto.DriverLicence,
                ProfilePicture = userDto.ProfilePicture,
                PhoneNumber = userDto.PhoneNumber,
                Email = userDto.Email,
                PasswordHash = userDto.Password,     //hashedPassword,
                NormalizedEmail = userDto.Email.ToUpper(),
                UserName = userDto.Email,
                NormalizedUserName = userDto.Email.ToUpper(),
                LockoutEnabled = false
            };

            var response = await userManager.CreateAsync(user, user.PasswordHash);
            if (response.Succeeded)
            {
                var role = await roleManager.FindByNameAsync("ROLE_USER");
                await userManager.AddToRoleAsync(user, role.Name);

                await context.SaveChangesAsync();

                var getUser = await context.Users.Where(x => x.Email.Equals(userDto.Email)).FirstOrDefaultAsync();

                return new ViewApiResponse
                {
                    ResponseStatus = 200,
                    ResponseMessage = $"Success",
                    ResponseData = getUser
                };
            }
            else
            {
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };
            }


        }

        public async Task<ViewApiResponse> Login(LoginDto loginDto)
        {
            if (String.IsNullOrEmpty(loginDto.Email) || String.IsNullOrEmpty(loginDto.Password))
            {
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Credentials cannot be null or empty",
                    ResponseData = null
                };
            }

            var validateLoginDetails = await ValidateUser(loginDto);
            if (!validateLoginDetails)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Invalid Credentials",
                    ResponseData = null
                };

            user = await context.Users.Where(x => x.Email.Equals(loginDto.Email)).FirstOrDefaultAsync();

            var token = await CreateToken();

            var response = new
            {
                token
            };

            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Login Successful",
                ResponseData = response
            };

        }

        private async Task<bool> ValidateUser(LoginDto loginDto)
        {
            if (loginDto is null || String.IsNullOrEmpty(loginDto.Email) || String.IsNullOrEmpty(loginDto.Password))
                return false;

            try
            {
                user = await userManager.FindByNameAsync(loginDto.Email);
                return user != null && await userManager.CheckPasswordAsync(user, loginDto.Password);
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        /*
        private static string HashPassword(string password)
        {
            //return BC.HashPassword(password);
            Microsoft.AspNet.Identity.PasswordHasher hasher = new();


            return hasher.HashPassword(password);
        }

        */

        #endregion


        #region IdentityCore
        public async Task<ViewApiResponse> RemoveUserFromRole(string email, string rolename)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(rolename))
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            var getUser = await context.Users.Where(x => x.Email.Equals(email)).FirstOrDefaultAsync();
            var getRole = await context.Roles.Where(x => x.Name.Equals(rolename)).Select(x => x.Name).FirstOrDefaultAsync();


            if (getUser is null || getRole is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            if (!await userManager.IsInRoleAsync(getUser, getRole))
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request, User not assigned to role",
                    ResponseData = null
                };

            var removeUserFromRole = await userManager.RemoveFromRoleAsync(getUser, getRole);

            await context.SaveChangesAsync();
            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = "Success, User removed from role",
                ResponseData = { }
            };
        }

        public async Task<ViewApiResponse> AddRole(AddRoleDto addRoleDto)
        {
            if (String.IsNullOrEmpty(addRoleDto.RoleName))
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            if (await roleManager.RoleExistsAsync(addRoleDto.RoleName))
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request, Role Exists",
                    ResponseData = null
                };

            IdentityRole role = new()
            {
                Name = addRoleDto.RoleName,
                NormalizedName = addRoleDto.RoleName.ToUpper(),
            };

            var response = await roleManager.CreateAsync(role);
            await context.SaveChangesAsync();

            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Success",
                ResponseData = response
            };
        }

        public async Task<ViewApiResponse> GetRoles()
        {
            //var response = await context.Roles.ToListAsync();

            var response = await roleManager.Roles.ToListAsync();

            if (response is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            else if (response.Count < 0)
                return new ViewApiResponse
                {
                    ResponseStatus = 200,
                    ResponseMessage = $"Success",
                    ResponseData = response
                };

            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Success",
                ResponseData = response
            };
        }

        public async Task<ViewApiResponse> GetRoleByRoleName(string roleName)
        {
            if (String.IsNullOrEmpty(roleName))
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            //var response = await context.Roles.Where(x => x.Name.Equals(roleName)).FirstOrDefaultAsync();

            var response = await roleManager.FindByNameAsync(roleName);

            if (response is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 404,
                    ResponseMessage = $"Not Found",
                    ResponseData = null
                };

            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Success",
                ResponseData = response
            };
        }

        public async Task<ViewApiResponse> AddUserToRole(AddUserToRoleDto addUserToRoleDto)
        {
            var getUser = await context.Users.Where(x => x.Email.Equals(addUserToRoleDto.Email)).FirstOrDefaultAsync();
            var getRole = await context.Roles.Where(x => x.Name.Equals(addUserToRoleDto.RoleName)).FirstOrDefaultAsync();

            if (getUser is null || getRole is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            if (await userManager.IsInRoleAsync(getUser, getRole.Name))
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request, User already assigned to role",
                    ResponseData = null
                };

            var response = await userManager.AddToRoleAsync(getUser, getRole.Name);

            await context.SaveChangesAsync();
            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Success",
                ResponseData = { }
            };
        }

        public async Task<ViewApiResponse> DeleteRole(string rolename)
        {
            if (String.IsNullOrEmpty(rolename))
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = null
                };

            var findRole = await roleManager.FindByNameAsync(rolename);
            if (findRole is null)
                return new ViewApiResponse
                {
                    ResponseStatus = 404,
                    ResponseMessage = $"Not Found",
                    ResponseData = null
                };

            var findUserAssignedToRole = await context.UserRoles.Where(x => x.RoleId.Equals(findRole.Id)).ToListAsync(); ;
            if (findUserAssignedToRole.Count > 0)
                return new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request, Role is assigned to user(s)",
                    ResponseData = { }
                };

            var response = await roleManager.DeleteAsync(findRole);
            await context.SaveChangesAsync();

            return new ViewApiResponse
            {
                ResponseStatus = 200,
                ResponseMessage = $"Success, Item Deleted",
                ResponseData = { }
            };
        }

        #endregion
    }
}
