﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace EmployeeManagement.Security
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler : 
        AuthorizationHandler<ManageAdminRolesClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesClaimsRequirement requirement)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;

            if(authFilterContext == null)
            {
                return Task.CompletedTask;
            }

            string loggedInAdminId = context.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //NameIdentifier has the loggedin ID 

            string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];

            if (context.User.IsInRole("Admin") &&
                context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
                adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
  }

