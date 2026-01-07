using ESSPMemberService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;


namespace ESSPMemberService.Attributes
{
   

    public class HasPermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permissionCode;

        public HasPermissionAttribute(string permissionCode)
        {
            _permissionCode = permissionCode;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            // Read user id from session
            var userIdStr = httpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userIdStr))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            int userId = int.Parse(userIdStr);

            // Resolve service from DI
            var permissionService = httpContext
                .RequestServices
                .GetRequiredService<PermissionService>();

            if (!permissionService.HasPermission(userId, _permissionCode))
            {
                context.Result = new ForbidResult();
            }
        }
    }

}
