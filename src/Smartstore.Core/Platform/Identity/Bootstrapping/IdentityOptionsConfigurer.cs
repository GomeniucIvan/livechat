using Autofac;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Smartstore.Core.Identity;

namespace Smartstore.Core.Bootstrapping
{
    internal sealed class IdentityOptionsConfigurer : IConfigureOptions<IdentityOptions>
    {
        private readonly IApplicationContext _appContext;

        public IdentityOptionsConfigurer(IApplicationContext appContext)
        {
            _appContext = appContext;
        }

        public void Configure(IdentityOptions options)
        {
            var customerSettings = _appContext.Services.Resolve<CustomerSettings>();

            var usr = options.User;
            usr.RequireUniqueEmail = true;
            // INFO: Add space to default list of allowed chars.
            usr.AllowedUserNameCharacters += ' ';

            var pwd = options.Password;
            pwd.RequiredLength = 6;
            pwd.RequireDigit = true;
            pwd.RequireUppercase = false;
            pwd.RequiredUniqueChars = 0;
            pwd.RequireLowercase = false;
            pwd.RequireNonAlphanumeric = customerSettings.PasswordRequireNonAlphanumeric;

            var signIn = options.SignIn;
            signIn.RequireConfirmedAccount = false;
            signIn.RequireConfirmedPhoneNumber = false;
            signIn.RequireConfirmedEmail = false;
        }
    }
}
