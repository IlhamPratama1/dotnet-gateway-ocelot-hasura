namespace Dotnet.Auth.API.Dtos
{
    public class Role
    {
        public string? role;
        public Role() { }
    }

    public class PrivilegeDto
    {
        public Role[] roles = new Role[0];
    }
}
