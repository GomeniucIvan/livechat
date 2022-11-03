using System.Collections;
using System.Xml;
using Smartstore.Engine.Modularity;
using Smartstore.IO;
using Smartstore.Utilities;

namespace Smartstore.Core.Localization
{
    public partial class XmlResourceManager : IXmlResourceManager
    {
        #region Resource Manager

        public virtual XmlDocument FlattenResourceFile(XmlDocument source)
        {
            Guard.NotNull(source, nameof(source));

            if (source.SelectNodes("//Children").Count == 0)
            {
                // the document contains absolutely NO nesting,
                // so don't bother parsing.
                return source;
            }

            var resources = new List<LocaleStringResourceParent>();

            foreach (XmlNode resNode in source.SelectNodes(@"//Language/LocaleResource"))
            {
                resources.Add(new LocaleStringResourceParent(resNode));
            }

            resources.Sort((x1, x2) => x1.ResourceName.CompareTo(x2.ResourceName));

            foreach (var resource in resources)
            {
                RecursivelySortChildrenResource(resource);
            }

            using var sbp = StringBuilderPool.Instance.Get(out var sb);
            using (var writer = XmlWriter.Create(sb))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Language", "");

                writer.WriteStartAttribute("Name", "");
                writer.WriteString(source.SelectSingleNode(@"//Language").Attributes["Name"].InnerText.Trim());
                writer.WriteEndAttribute();

                foreach (var resource in resources)
                {
                    RecursivelyWriteResource(resource, writer, null);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }

            var result = new XmlDocument();
            result.LoadXml(sb.ToString());

            return result;
        }

        #endregion

        #region Resource Manager Utils

        private static void RecursivelyWriteResource(LocaleStringResourceParent resource, XmlWriter writer, bool? parentAppendRootKey)
        {
            // The value isn't actually used, but the name is used to create a namespace.
            if (resource.IsPersistable)
            {
                writer.WriteStartElement("LocaleResource", "");

                writer.WriteStartAttribute("Name", "");
                writer.WriteString(resource.NameWithNamespace);
                writer.WriteEndAttribute();

                if (resource.AppendRootKey.HasValue)
                {
                    writer.WriteStartAttribute("AppendRootKey", "");
                    writer.WriteString(resource.AppendRootKey.Value ? "true" : "false");
                    writer.WriteEndAttribute();
                    parentAppendRootKey = resource.AppendRootKey;
                }
                else if (parentAppendRootKey.HasValue)
                {
                    writer.WriteStartAttribute("AppendRootKey", "");
                    writer.WriteString(parentAppendRootKey.Value ? "true" : "false");
                    writer.WriteEndAttribute();
                }

                writer.WriteStartElement("Value", "");
                writer.WriteString(resource.ResourceValue);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            foreach (var child in resource.ChildLocaleStringResources)
            {
                RecursivelyWriteResource(child, writer, resource.AppendRootKey ?? parentAppendRootKey);
            }
        }

        private static void RecursivelySortChildrenResource(LocaleStringResourceParent resource)
        {
            ArrayList.Adapter((IList)resource.ChildLocaleStringResources).Sort(new ComparisonComparer<LocaleStringResourceParent>((x1, x2) => x1.ResourceName.CompareTo(x2.ResourceName)));

            foreach (var child in resource.ChildLocaleStringResources)
            {
                RecursivelySortChildrenResource(child);
            }
        }

        private class LocaleStringResourceParent : LocaleStringResource
        {
            public LocaleStringResourceParent(XmlNode localStringResource, string nameSpace = "")
            {
                Namespace = nameSpace;
                var resNameAttribute = localStringResource.Attributes["Name"];
                var resValueNode = localStringResource.SelectSingleNode("Value");

                if (resNameAttribute == null)
                {
                    throw new SmartException("All language resources must have an attribute Name=\"Value\".");
                }

                var resName = resNameAttribute.Value.Trim();
                if (string.IsNullOrEmpty(resName))
                {
                    throw new SmartException("All languages resource attributes 'Name' must have a value.'");
                }

                ResourceName = resName;

                var appendRootKeyAttribute = localStringResource.Attributes["AppendRootKey"];
                if (appendRootKeyAttribute != null)
                {
                    AppendRootKey = appendRootKeyAttribute.Value.ToBool(true);
                }

                if (resValueNode == null || string.IsNullOrEmpty(resValueNode.InnerText.Trim()))
                {
                    IsPersistable = false;
                }
                else
                {
                    IsPersistable = true;
                    ResourceValue = resValueNode.InnerText.Trim();
                }

                foreach (XmlNode childResource in localStringResource.SelectNodes("Children/LocaleResource"))
                {
                    ChildLocaleStringResources.Add(new LocaleStringResourceParent(childResource, NameWithNamespace));
                }
            }

            public string Namespace { get; set; }

            public IList<LocaleStringResourceParent> ChildLocaleStringResources = new List<LocaleStringResourceParent>();

            public bool IsPersistable { get; set; }

            public bool? AppendRootKey { get; set; }

            public string NameWithNamespace
            {
                get
                {
                    var newNamespace = Namespace;
                    if (!string.IsNullOrEmpty(newNamespace))
                    {
                        newNamespace += ".";
                    }
                    return newNamespace + ResourceName;
                }
            }
        }

        private class ComparisonComparer<T> : IComparer<T>, IComparer
        {
            private readonly Comparison<T> _comparison;

            public ComparisonComparer(Comparison<T> comparison)
            {
                _comparison = comparison;
            }

            public int Compare(T x, T y)
            {
                return _comparison(x, y);
            }

            public int Compare(object o1, object o2)
            {
                return _comparison((T)o1, (T)o2);
            }
        }

        #endregion
    }
}
