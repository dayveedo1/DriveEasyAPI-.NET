using DriveEasy.API.DriveEasy.Dto;

namespace DriveEasy.API.DriveEasy.Interface
{
    public interface IAuth
    {
        #region Authentication

        Task<ViewApiResponse> CreateUserAccount(UserDto userDto);
        Task<ViewApiResponse> Login(LoginDto loginDto);

        #endregion


        #region JWT
        Task<string> CreateToken();

        #endregion


        #region IdentityCore
        Task<ViewApiResponse> AddRole(AddRoleDto addRoleDto);
        Task<ViewApiResponse> GetRoles();
        Task<ViewApiResponse> GetRoleByRoleName(string roleName);
        Task<ViewApiResponse> DeleteRole(string rolename);
        Task<ViewApiResponse> AddUserToRole(AddUserToRoleDto addUserToRoleDto);
        Task<ViewApiResponse> RemoveUserFromRole(string email, string rolename);
        

        #endregion
    }
}
