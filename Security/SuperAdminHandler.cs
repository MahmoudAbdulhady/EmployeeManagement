using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.Security
{
    public class SuperAdminHandler : 
        AuthorizationHandler<ManageAdminRolesClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ManageAdminRolesClaimsRequirement requirement)
        {
            if (context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
