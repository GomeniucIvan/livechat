using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Smartstore.Core.Localization;
using Smartstore.Core.Logging;

namespace Smartstore.Core.Stores
{
    public sealed class AuthorizeAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="AuthorizeAttribute"/>.
        /// </summary>
        /// <param name="check">Set to <c>false</c> to override any controller-level <see cref="AuthorizeAttribute"/>.</param>
        public AuthorizeAttribute(bool check = true)
            : base(typeof(AuthorizeFilter))
        {
            Check = check;
            Arguments = new object[] { check };
        }

        public bool Check { get; }

        class AuthorizeFilter : IAuthorizationFilter
        {
            private readonly IWorkContext _workContext;
            private readonly INotifier _notifier;
            private readonly bool _check;

            public AuthorizeFilter(
                IWorkContext workContext,
                INotifier notifier,
                Localizer localizer,
                bool check)
            {
                _workContext = workContext;
                _notifier = notifier;
                T = localizer;

                _check = check;
            }

            public Localizer T { get; set; } = NullLocalizer.Instance;

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (!_check)
                {
                    return;
                }

                // Prevent lockout
                //if (context.HttpContext.Connection.IsLocal())
                //{
                //    return;
                //}

                var overrideFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(x => x.Scope == FilterScope.Action)
                    .Select(x => x.Filter)
                    .OfType<AuthorizeAttribute>()
                    .FirstOrDefault();

                if (overrideFilter?.Check == false)
                {
                    return;
                }

                var customer = _workContext.CurrentCustomer;

                if (customer.IsRegistered() && customer.IsAdmin())
                {
                    // Allow admin access
                }
                else
                {
                    if (!customer.IsRegistered())
                    {
                        if (context.HttpContext.Request.IsAjax())
                        {
                            var storeClosedMessage = "{0} {1}".FormatCurrentUI(
                                T("Closed"),
                                T("Closed.Hint"));

                            _notifier.Error(storeClosedMessage);
                        }
                        else
                        {
                            context.Result = new RedirectToRouteResult("Login", null);
                        }
                    }
                }
            }
        }
    }
}
