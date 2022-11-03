using Microsoft.AspNetCore.Http;
using Smartstore.Caching;
using Smartstore.Core.Common.Services;
using Smartstore.Core.Identity;
using Smartstore.Core.Localization;
using Smartstore.Core.Stores;
using Smartstore.Core.Web;
using Smartstore.Net;

namespace Smartstore.Web
{
    public partial class WebWorkContext : IWorkContext
    {
        private readonly SmartDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageResolver _languageResolver;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly PrivacySettings _privacySettings;
        private readonly ICacheManager _cache;
        private readonly IUserAgent _userAgent;
        private readonly IWebHelper _webHelper;
        private readonly IGeoCountryLookup _geoCountryLookup;

        private Language _language;
        private Customer _customer;
        private Customer _impersonator;
        private bool? _isAdminArea;

        public WebWorkContext(
            SmartDbContext db,
            IHttpContextAccessor httpContextAccessor,
            ILanguageResolver languageResolver,
            IStoreContext storeContext,
            ICustomerService customerService,
            PrivacySettings privacySettings,
            ICacheManager cache,
            IUserAgent userAgent,
            IWebHelper webHelper,
            IGeoCountryLookup geoCountryLookup)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _languageResolver = languageResolver;
            _storeContext = storeContext;
            _customerService = customerService;
            _privacySettings = privacySettings;
            _userAgent = userAgent;
            _cache = cache;
            _webHelper = webHelper;
            _geoCountryLookup = geoCountryLookup;
        }

        public Customer CurrentCustomer
        {
            get
            {
                if (_customer != null)
                {
                    return _customer;
                }

                var httpContext = _httpContextAccessor.HttpContext;

                // Is system account?
                if (TryGetSystemAccount(httpContext, out var customer))
                {
                    // Get out quickly. Bots tend to overstress the shop.
                    _customer = customer;
                    return customer;
                }

                // Registered/Authenticated customer?
                customer = _customerService.GetAuthenticatedCustomerAsync().Await();

                // impersonate user if required (currently used for 'phone order' support)
                if (customer != null)
                {
                    var impersonatedCustomerId = customer.GenericAttributes.ImpersonatedCustomerId;
                    if (impersonatedCustomerId > 0)
                    {
                        var impersonatedCustomer = _db.Customers
                            .IncludeCustomerRoles()
                            .FindById(impersonatedCustomerId.Value);

                        if (impersonatedCustomer != null && !impersonatedCustomer.Deleted && impersonatedCustomer.Active)
                        {
                            // set impersonated customer
                            _impersonator = customer;
                            customer = impersonatedCustomer;
                        }
                    }
                }

                // Load guest customer
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    customer = GetGuestCustomerAsync(httpContext).Await();
                }

                _customer = customer;

                return _customer;
            }
            set => _customer = value;
        }

        protected bool TryGetSystemAccount(HttpContext context, out Customer customer)
        {
            // Never check whether customer is deleted/inactive in this method.
            // System accounts should neither be deletable nor activatable, they are mandatory.

            customer = null;

            // check whether request is made by a background task
            // in this case return built-in customer record for background task
            if (context != null && context.Request.IsCalledByTaskScheduler())
            {
                customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.BackgroundTask);
            }

            // check whether request is made by a search engine
            // in this case return built-in customer record for search engines 
            if (customer == null && _userAgent.IsBot)
            {
                customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.SearchEngine);
            }

            // check whether request is made by the PDF converter
            // in this case return built-in customer record for the converter
            if (customer == null && _userAgent.IsPdfConverter)
            {
                customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.PdfConverter);
            }

            return customer != null;
        }

        protected virtual async Task<Customer> GetGuestCustomerAsync(HttpContext context)
        {
            Customer customer = null;

            var visitorCookie = context?.Request?.Cookies[CookieNames.Visitor];
            if (visitorCookie == null)
            {
                // No anonymous visitor cookie yet. Try to identify anyway (by IP and UserAgent)
                customer = await _customerService.FindGuestCustomerByClientIdentAsync(maxAgeSeconds: 180);
            }
            else if (Guid.TryParse(visitorCookie, out var customerGuid))
            {
                // Cookie present. Try to load guest customer by it's value.
                customer = await _db.Customers
                    .IncludeCustomerRoles()
                    .Where(c => c.CustomerGuid == customerGuid)
                    .FirstOrDefaultAsync();
            }

            if (customer == null || customer.Deleted || !customer.Active || customer.IsRegistered())
            {
                // No record yet or account deleted/deactivated.
                // Also dont' treat registered customers as guests.
                // Create new record in these cases.
                customer = await _customerService.CreateGuestCustomerAsync();
            }

            if (context != null)
            {
                var cookieExpiry = customer.CustomerGuid == Guid.Empty
                    ? DateTime.Now.AddMonths(-1)
                    : DateTime.Now.AddHours(24 * 365); // TODO make configurable

                // Set visitor cookie
                var cookieOptions = new CookieOptions
                {
                    Expires = cookieExpiry,
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = _webHelper.IsCurrentConnectionSecured(),
                    SameSite = SameSiteMode.Lax
                };

                // INFO: Global OnAppendCookie does not always run for visitor cookie.
                if (cookieOptions.Secure)
                {
                    cookieOptions.SameSite = _privacySettings.SameSiteMode;
                }

                if (context.Request.PathBase.HasValue)
                {
                    cookieOptions.Path = context.Request.PathBase;
                }

                var cookies = context.Response.Cookies;
                try
                {
                    cookies.Delete(CookieNames.Visitor, cookieOptions);
                }
                finally
                {
                    cookies.Append(CookieNames.Visitor, customer.CustomerGuid.ToString(), cookieOptions);
                }
            }

            return customer;
        }

        public Customer CurrentImpersonator => _impersonator;

        public Language WorkingLanguage
        {
            get
            {
                if (_language == null)
                {
                    var customer = CurrentCustomer;

                    // Resolve the current working language
                    _language = _languageResolver.ResolveLanguage(customer, _httpContextAccessor.HttpContext);

                    // Set language if current customer langid does not match resolved language id
                    var customerAttributes = customer.GenericAttributes;
                    if (customerAttributes.LanguageId != _language.Id)
                    {
                        SetCustomerLanguage(_language.Id);
                    }
                }

                return _language;
            }
            set
            {
                if (value?.Id != _language?.Id)
                {
                    SetCustomerLanguage(value?.Id);
                    _language = value;
                }
            }
        }

        private void SetCustomerLanguage(int? languageId)
        {
            var customer = CurrentCustomer;

            if (customer == null || customer.IsSystemAccount)
                return;

            customer.GenericAttributes.LanguageId = languageId;
            customer.GenericAttributes.SaveChanges();
        }

        public bool IsAdminArea
        {
            get
            {
                if (_isAdminArea.HasValue)
                {
                    return _isAdminArea.Value;
                }

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _isAdminArea = false;
                }

                if (httpContext.Request.IsAdminArea())
                {
                    _isAdminArea = true;
                }

                // TODO: (core) More checks for admin area?

                _isAdminArea ??= false;
                return _isAdminArea.Value;
            }
            set => _isAdminArea = value;
        }
    }
}
