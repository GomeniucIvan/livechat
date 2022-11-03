using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Smartstore.Data;

namespace Smartstore.Core.Security
{
    /// <summary>
    /// Checks whether the current user has the permission to access the shop.
    /// </summary>
    public sealed class AuthorizeAccessAttribute : TypeFilterAttribute
    {
        public AuthorizeAccessAttribute()
            : base(typeof(AuthorizeAccessFilter))
        {
        }

        class AuthorizeAccessFilter : IAsyncAuthorizationFilter
        {
            private readonly IPermissionService _permissionService;

            public AuthorizeAccessFilter(IPermissionService permissionService)
            {
                _permissionService = permissionService;
            }

            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                Guard.NotNull(context, nameof(context));

                if (!DataSettings.DatabaseIsInstalled())
                {
                    return;
                }

                var overrideFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(x => x.Scope == FilterScope.Action)
                    .Select(x => x.Filter)
                    .OfType<NeverAuthorizeAttribute>()
                    .FirstOrDefault();

                if (overrideFilter != null)
                {
                    return;
                }

                if (!await HasWebsiteAccess())
                {
                    context.Result = new ChallengeResult();
                }
            }

            private async Task<bool> HasWebsiteAccess()
            {
                if (await _permissionService.AuthorizeAsync(Permissions.System.AccessWebsite))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
