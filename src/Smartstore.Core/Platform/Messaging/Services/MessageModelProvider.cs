using System.Collections;
using System.Drawing;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Smartstore.Collections;
using Smartstore.ComponentModel;
using Smartstore.Core.Common;
using Smartstore.Core.Content.Media;
using Smartstore.Core.Data;
using Smartstore.Core.Identity;
using Smartstore.Core.Localization;
using Smartstore.Core.Messaging.Events;
using Smartstore.Core.Seo;
using Smartstore.Core.Stores;
using Smartstore.Engine.Modularity;
using Smartstore.Http;
using Smartstore.Imaging;
using Smartstore.Templating;
using Smartstore.Utilities.Html;

namespace Smartstore.Core.Messaging
{
    public enum ModelTreeMemberKind
    {
        Primitive,
        Complex,
        Collection,
        Root
    }

    public class ModelTreeMember
    {
        public string Name { get; set; }
        public ModelTreeMemberKind Kind { get; set; }
    }

    public partial class MessageModelProvider : IMessageModelProvider
    {
        private readonly SmartDbContext _db;
        private readonly ICommonServices _services;
        private readonly MessageModelHelper _helper;

        public MessageModelProvider(
            SmartDbContext db,
            ICommonServices services,
            MessageModelHelper helper)
        {
            _db = db;
            _services = services;
            _helper = helper;
        }

        public LocalizerEx T { get; set; } = NullLocalizer.InstanceEx;
        public ILogger Logger { get; set; } = NullLogger.Instance;

        public virtual async Task AddGlobalModelPartsAsync(MessageContext messageContext)
        {
            Guard.NotNull(messageContext, nameof(messageContext));

            var model = messageContext.Model;

            model["Context"] = new Dictionary<string, object>
            {
                { "TemplateName", messageContext.MessageTemplate.Name },
                { "LanguageId", messageContext.Language.Id },
                { "LanguageCulture", messageContext.Language.LanguageCulture },
                { "LanguageRtl", messageContext.Language.Rtl },
                { "BaseUrl", messageContext.BaseUri.ToString() }
            };

            dynamic email = new ExpandoObject();
            email.Email = messageContext.EmailAccount.Email;
            email.SenderName = messageContext.EmailAccount.DisplayName;
            email.DisplayName = messageContext.EmailAccount.DisplayName; // Alias
            model["Email"] = email;
            model["Store"] = await CreateModelPartAsync(messageContext.Store, messageContext);
        }

        public async Task<object> CreateModelPartAsync(object part, bool ignoreNullMembers, params string[] ignoreMemberNames)
        {
            Guard.NotNull(part, nameof(part));

            var store = _services.StoreContext.CurrentStore;
            var messageContext = new MessageContext
            {
                Language = _services.WorkContext.WorkingLanguage,
                Store = store,
                BaseUri = new Uri(store.GetHost()),
                Model = new TemplateModel()
            };

            if (part is Customer x)
            {
                // This case is not handled in AddModelPart core method.
                messageContext.Customer = x;
            }
            else
            {
                messageContext.Customer = _services.WorkContext.CurrentCustomer;
                await AddModelPartAsync(part, messageContext, "Part");
            }

            object result = null;

            if (messageContext.Model.Any())
            {
                result = messageContext.Model.FirstOrDefault().Value;

                if (result is IDictionary<string, object> dict)
                {
                    SanitizeModelDictionary(dict, ignoreNullMembers, ignoreMemberNames);
                }
            }

            return result;
        }

        private void SanitizeModelDictionary(IDictionary<string, object> dict, bool ignoreNullMembers, params string[] ignoreMemberNames)
        {
            if (ignoreNullMembers || ignoreMemberNames.Length > 0)
            {
                foreach (var key in dict.Keys.ToArray())
                {
                    var expando = dict as HybridExpando;
                    var value = dict[key];

                    if ((ignoreNullMembers && value == null) || ignoreMemberNames.Contains(key))
                    {
                        if (expando != null)
                            expando.Override(key, null); // INFO: we cannot remove entries from HybridExpando
                        else
                            dict.Remove(key);
                        continue;
                    }

                    if (value != null && value.GetType().IsSequenceType())
                    {
                        var ignoreMemberNames2 = ignoreMemberNames
                            .Where(x => x.StartsWith(key + ".", StringComparison.OrdinalIgnoreCase))
                            .Select(x => x[(key.Length + 1)..])
                            .ToArray();

                        if (value is IDictionary<string, object> dict2)
                        {
                            SanitizeModelDictionary(dict2, ignoreNullMembers, ignoreMemberNames2);
                        }
                        else
                        {
                            var list = ((IEnumerable)value).OfType<IDictionary<string, object>>();
                            foreach (var dict3 in list)
                            {
                                SanitizeModelDictionary(dict3, ignoreNullMembers, ignoreMemberNames2);
                            }
                        }
                    }
                }
            }
        }

        public virtual async Task AddModelPartAsync(object part, MessageContext messageContext, string name = null)
        {
            Guard.NotNull(part, nameof(part));
            Guard.NotNull(messageContext, nameof(messageContext));

            var model = messageContext.Model;

            name = name.NullEmpty() ?? ResolveModelName(part);

            object modelPart = null;

            switch (part)
            {
                case INamedModelPart x:
                    modelPart = x;
                    break;
                case IModelPart x:
                    MergeModelBag(x, model, messageContext);
                    break;
                case IEnumerable<GenericAttribute> x:
                    modelPart = await CreateModelPartAsync(x, messageContext);
                    break;
                default:
                    var partType = part.GetType();
                    modelPart = part;

                    if (partType.IsPlainObjectType() && !partType.IsAnonymousType())
                    {
                        var evt = new MessageModelPartMappingEvent(part, messageContext);
                        await _services.EventPublisher.PublishAsync(evt);

                        if (evt.Result != null && !object.ReferenceEquals(evt.Result, part))
                        {
                            _ = evt.Result;
                            name = evt.ModelPartName.NullEmpty() ?? ResolveModelName(evt.Result) ?? name;
                        }

                        modelPart = evt.Result ?? part;
                        name = evt.ModelPartName.NullEmpty() ?? name;
                    }

                    break;
            }

            if (modelPart != null)
            {
                if (name.IsEmpty())
                {
                    throw new SmartException($"Could not resolve a model key for part '{modelPart.GetType().Name}'. Use an instance of 'NamedModelPart' class to pass model with name.");
                }

                if (model.TryGetValue(name, out var existing))
                {
                    // A model part with the same name exists in model already...
                    if (existing is IDictionary<string, object> x)
                    {
                        // but it's a dictionary which we can easily merge with.
                        x.Merge(FastProperty.ObjectToDictionary(modelPart), true);
                    }
                    else
                    {
                        // Wrap in HybridExpando and merge.
                        var he = new HybridExpando(existing, true);
                        he.Merge(FastProperty.ObjectToDictionary(modelPart), true);
                        model[name] = he;
                    }
                }
                else
                {
                    // Put part to model as new property.
                    model[name] = modelPart;
                }
            }
        }

        public string ResolveModelName(object model)
        {
            Guard.NotNull(model, nameof(model));

            string name = null;
            var type = model.GetType();

            try
            {
                if (model is INamedEntity be)
                {
                    name = be.GetEntityName();
                }
                else if (model is ITestModel te)
                {
                    name = te.ModelName;
                }
                else if (model is INamedModelPart mp)
                {
                    name = mp.ModelPartName;
                }
                else if (type.IsPlainObjectType())
                {
                    name = type.Name;
                }
            }
            catch
            {
            }

            return name;
        }

        #region Generic model part handlers

        protected virtual void MergeModelBag(IModelPart part, IDictionary<string, object> model, MessageContext messageContext)
        {
            if (model.Get("Bag") is not IDictionary<string, object> bag)
            {
                model["Bag"] = bag = new Dictionary<string, object>();
            }

            var source = part as IDictionary<string, object>;
            bag.Merge(source);
        }

        #endregion

        #region Entity specific model part handlers

        protected virtual async Task<object> CreateModelPartAsync(Store part, MessageContext messageContext)
        {
            Guard.NotNull(messageContext, nameof(messageContext));
            Guard.NotNull(part, nameof(part));

            var host = messageContext.BaseUri.ToString();
            var logoFile = await _services.MediaService.GetFileByIdAsync(messageContext.Store.LogoMediaFileId, MediaLoadFlags.AsNoTracking);

            var m = new Dictionary<string, object>
            {
                { "Email", messageContext.EmailAccount.Email },
                { "EmailName", messageContext.EmailAccount.DisplayName },
                { "Name", part.Name },
                { "Url", host },
                { "Cdn", part.ContentDeliveryNetwork },
                { "Logo", await CreateModelPartAsync(logoFile, messageContext, host, null, new Size(400, 75)) },
                { "Copyright", T("Content.CopyrightNotice", messageContext.Language.Id, DateTime.Now.Year.ToString(), part.Name).ToString() }
            };

            var he = new HybridExpando(true);
            he.Merge(m, true);

            await _helper.PublishModelPartCreatedEventAsync(part, he);

            return he;
        }

        protected virtual async Task<object> CreateModelPartAsync(
            MediaFileInfo part,
            MessageContext messageContext,
            string href,
            int? targetSize = null,
            Size? clientMaxSize = null,
            string alt = null)
        {
            Guard.NotNull(messageContext, nameof(messageContext));
            Guard.NotEmpty(href, nameof(href));

            if (part == null)
            {
                return null;
            }

            var width = part.File.Width;
            var height = part.File.Height;

            if (width.HasValue && height.HasValue && (targetSize.HasValue || clientMaxSize.HasValue))
            {
                var maxSize = clientMaxSize ?? new Size(targetSize.Value, targetSize.Value);
                var size = ImagingHelper.Rescale(new Size(width.Value, height.Value), maxSize);
                width = size.Width;
                height = size.Height;
            }

            var m = new
            {
                Src = _services.MediaService.GetUrl(part, targetSize.GetValueOrDefault(), messageContext.BaseUri.ToString(), false),
                Href = href,
                Width = width,
                Height = height,
                Alt = alt
            };

            await _helper.PublishModelPartCreatedEventAsync(part, m);

            return m;
        }

        protected virtual async Task<object> CreateModelPartAsync(IEnumerable<GenericAttribute> part, MessageContext messageContext)
        {
            Guard.NotNull(messageContext, nameof(messageContext));
            Guard.NotNull(part, nameof(part));

            var m = new Dictionary<string, object>();

            foreach (var attr in part)
            {
                m[attr.Key] = attr.Value;
            }

            await _helper.PublishModelPartCreatedEventAsync(part, m);

            return m;
        }

        #endregion

        #region Model Tree

        public async Task<TreeNode<ModelTreeMember>> GetLastModelTreeAsync(string messageTemplateName)
        {
            Guard.NotEmpty(messageTemplateName, nameof(messageTemplateName));

            var template = await _db.MessageTemplates
                .AsNoTracking()
                .Where(x => x.Name == messageTemplateName)
                .FirstOrDefaultAsync();

            if (template != null)
            {
                return GetLastModelTree(template);
            }

            return null;
        }

        public TreeNode<ModelTreeMember> GetLastModelTree(MessageTemplate template)
        {
            Guard.NotNull(template, nameof(template));

            if (template.LastModelTree.IsEmpty())
            {
                return null;
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<TreeNode<ModelTreeMember>>(template.LastModelTree);
        }

        public TreeNode<ModelTreeMember> BuildModelTree(TemplateModel model)
        {
            Guard.NotNull(model, nameof(model));

            var root = new TreeNode<ModelTreeMember>(new ModelTreeMember { Name = "Model", Kind = ModelTreeMemberKind.Root });

            foreach (var kvp in model)
            {
                root.Append(BuildModelTreePart(kvp.Key, kvp.Value));
            }

            return root;
        }

        private TreeNode<ModelTreeMember> BuildModelTreePart(string modelName, object instance)
        {
            var t = instance?.GetType();
            TreeNode<ModelTreeMember> node;
            if (t == null || t.IsBasicOrNullableType())
            {
                node = new TreeNode<ModelTreeMember>(new ModelTreeMember { Name = modelName, Kind = ModelTreeMemberKind.Primitive });
            }
            else if (t.IsSequenceType() && !t.IsDictionaryType())
            {
                node = new TreeNode<ModelTreeMember>(new ModelTreeMember { Name = modelName, Kind = ModelTreeMemberKind.Collection });
            }
            else
            {
                node = new TreeNode<ModelTreeMember>(new ModelTreeMember { Name = modelName, Kind = ModelTreeMemberKind.Complex });

                if (instance is IDictionary<string, object> dict)
                {
                    foreach (var kvp in dict)
                    {
                        node.Append(BuildModelTreePart(kvp.Key, kvp.Value));
                    }
                }
                else if (instance is IDynamicMetaObjectProvider dyn)
                {
                    foreach (var name in dyn.GetMetaObject(Expression.Constant(dyn)).GetDynamicMemberNames())
                    {
                        // we don't want to go deeper in "pure" dynamic objects
                        node.Append(new TreeNode<ModelTreeMember>(new ModelTreeMember { Name = name, Kind = ModelTreeMemberKind.Primitive }));
                    }
                }
                else
                {
                    node.AppendRange(BuildModelTreePartForClass(instance));
                }
            }

            return node;
        }

        private IEnumerable<TreeNode<ModelTreeMember>> BuildModelTreePartForClass(object instance, HashSet<object> instanceLookup = null)
        {
            var type = instance?.GetType();

            if (type == null)
            {
                yield break;
            }

            foreach (var prop in FastProperty.GetProperties(type).Values)
            {
                var pi = prop.Property;

                if (pi.PropertyType.IsBasicOrNullableType())
                {
                    yield return new TreeNode<ModelTreeMember>(new ModelTreeMember { Name = prop.Name, Kind = ModelTreeMemberKind.Primitive });
                }
                else if (typeof(IDictionary<string, object>).IsAssignableFrom(pi.PropertyType))
                {
                    yield return BuildModelTreePart(prop.Name, prop.GetValue(instance));
                }
                else if (pi.PropertyType.IsSequenceType())
                {
                    yield return new TreeNode<ModelTreeMember>(new ModelTreeMember { Name = prop.Name, Kind = ModelTreeMemberKind.Collection });
                }
                else if (pi.PropertyType.IsClass)
                {
                    if (instanceLookup == null)
                    {
                        instanceLookup = new HashSet<object>(ReferenceEqualityComparer.Instance) { instance };
                    }

                    var childInstance = prop.GetValue(instance);
                    if (childInstance != null)
                    {
                        if (!instanceLookup.Contains(childInstance))
                        {
                            instanceLookup.Add(childInstance);

                            var node = new TreeNode<ModelTreeMember>(new ModelTreeMember { Name = prop.Name, Kind = ModelTreeMemberKind.Complex });
                            node.AppendRange(BuildModelTreePartForClass(childInstance, instanceLookup));
                            yield return node;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
