using Microsoft.Extensions.FileProviders;
using Smartstore.Engine.Modularity;

namespace Smartstore.Web.Razor
{
    /// <summary>
    /// A file provider that is aware of module and theme paths. If an incoming path
    /// points to an extension resource, this provider will truncate the path and pass it
    /// to the extension's file provider (see <see cref="IExtensionLocation.ContentRoot"/>).
    /// </summary>
    public class RazorRuntimeFileProvider : ModularFileProvider
    {
        private readonly IApplicationContext _appContext;

        public RazorRuntimeFileProvider(IApplicationContext appContext)
        {
            _appContext = Guard.NotNull(appContext, nameof(appContext));
        }

        protected override IFileProvider ResolveFileProvider(ref string path)
        {
            return _appContext.ContentRoot;
        }
    }
}
