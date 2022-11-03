using Microsoft.OData.ModelBuilder;
using Smartstore.IO;

namespace Smartstore.Web.Api
{
    /// <summary>
    /// Base class for OData model providers.
    /// </summary>
    public abstract class ODataModelProviderBase : IODataModelProvider
    {
        /// <inheritdoc/>
        public abstract void Build(ODataModelBuilder builder, int version);

        /// <inheritdoc/>
        public virtual Stream GetXmlCommentsStream(IApplicationContext appContext)
            => null;
    }
}
