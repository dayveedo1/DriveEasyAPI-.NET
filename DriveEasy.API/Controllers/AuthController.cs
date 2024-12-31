using DriveEasy.API.DriveEasy.Dto;
using DriveEasy.API.DriveEasy.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DriveEasy.API.Controllers
{
    [ApiController]
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuth auth;

        public AuthController(IAuth auth)
        {
            this.auth = auth;
        }


        #region Authentication

        [SwaggerOperation(Summary = "Authentication Endpoint that accept Username & Password to generate a JWT Token(s)")]
        [HttpPost("Login")]
        public async Task<ActionResult<ViewApiResponse>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = ModelState
                });

            var response = await auth.Login(loginDto);
            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [SwaggerOperation(Summary = "Endpoint to create user account")]
        [HttpPost("CreateUserAccount")]
        public async Task<ActionResult<ViewApiResponse>> CreateUserAccount([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = $"Bad Request",
                    ResponseData = ModelState
                });

            var response = await auth.CreateUserAccount(userDto);
            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        #endregion


        #region IdentityCore

        //[Authorize(Roles = "ROLE_USER")]
        [SwaggerOperation(Summary = "Endpoint to add user role")]
        [HttpPost("AddRole")]
        public async Task<ActionResult<ViewApiResponse>> AddRole(AddRoleDto addRoleDto)
        {
            var response = await auth.AddRole(addRoleDto);

            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);

        }

        //[Authorize(Roles = "ROLE_USER")]
        [SwaggerOperation(Summary = "Endpoint to return all user roles")]
        [HttpGet("GetRoles")]
        public async Task<ActionResult<ViewApiResponse>> GetRoles()
        {
            var response = await auth.GetRoles();
            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [SwaggerOperation(Summary = "Endpoint to return a user role by rolename")]
        [HttpGet("GetRoleByRoleName/{roleName}")]
        public async Task<ActionResult<ViewApiResponse>> GetRoleByRoleName(string roleName)
        {
            var response = await auth.GetRoleByRoleName(roleName);

            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            else if (response.ResponseStatus.Equals(404))
                return StatusCode(StatusCodes.Status404NotFound, response);

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [SwaggerOperation(Summary = "Endpoint to add a user to a role")]
        [HttpPost("AddUserToRole")]
        public async Task<ActionResult<ViewApiResponse>> AddUserToRole(AddUserToRoleDto addUserToRoleDto)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, new ViewApiResponse
                {
                    ResponseStatus = 400,
                    ResponseMessage = "Bad Request",
                    ResponseData = ModelState
                });

            var response = await auth.AddUserToRole(addUserToRoleDto);

            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [SwaggerOperation(Summary = "Endpoint to remove a role")]
        [HttpDelete("DeleteRole/{rolename}")]
        public async Task<ActionResult<ViewApiResponse>> DeleteRole(string rolename)
        {
            var response = await auth.DeleteRole(rolename);

            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(404))
                return StatusCode(StatusCodes.Status404NotFound, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [SwaggerOperation(Summary = "Endpoint to remove a user from a role")]
        [HttpDelete("RemoveUserFromRole/{email}/{rolename}")]
        public async Task<ActionResult<ViewApiResponse>> RemoveUserFromRole(string email, string rolename)
        {
            var response = await auth.RemoveUserFromRole(email, rolename);

            if (response.ResponseStatus.Equals(500))
                return StatusCode(StatusCodes.Status500InternalServerError, response);

            else if (response.ResponseStatus.Equals(400))
                return StatusCode(StatusCodes.Status400BadRequest, response);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        #endregion


        #region 

        #endregion
    }
}
