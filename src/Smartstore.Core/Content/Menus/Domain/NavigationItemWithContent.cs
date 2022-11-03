using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Smartstore.Core.Widgets;

namespace Smartstore.Core.Content.Menus
{
    public abstract class NavigationItemWithContent : NavigationItem, IHideObjectMembers
    {
        private IHtmlContent _content;

        public bool Ajax { get; set; }

        public AttributeDictionary ContentHtmlAttributes { get; set; } = new();

        public IHtmlContent Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                }
            }
        }

        public bool HasContent
        {
            get => _content != null;
        }

        public Task<IHtmlContent> GetContentAsync(ViewContext viewContext)
        {
            Guard.NotNull(viewContext, nameof(viewContext));

            if (_content != null)
            {
                return Task.FromResult(_content);
            }

            return Task.FromResult<IHtmlContent>(null);
        }
    }
}
