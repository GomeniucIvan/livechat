using System.Xml;

namespace Smartstore.Core.Localization
{
    /// <summary>
    /// Responsible for importing and exporting locale string resources from and to XML.
    /// </summary>
    public partial interface IXmlResourceManager
    {
        /// <summary>
        /// Flattens all nested <c>LocaleResource</c> child nodes into a new document.
        /// </summary>
        /// <param name="source">The source xml resource file</param>
        /// <returns>
        /// Either a new document with flattened resources or - if no nesting is determined - 
        /// the original document, which was passed as <paramref name="source"/>.
        /// </returns>
        XmlDocument FlattenResourceFile(XmlDocument source);
    }
}
