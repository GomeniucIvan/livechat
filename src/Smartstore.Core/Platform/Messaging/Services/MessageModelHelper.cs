using Microsoft.AspNetCore.Mvc;
using Smartstore.Core.Common.Services;
using Smartstore.Core.Identity;
using Smartstore.Core.Localization;
using Smartstore.Core.Messaging.Events;
using Smartstore.Events;
using Smartstore.Utilities;

namespace Smartstore.Core.Messaging
{
    public partial class MessageModelHelper
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly Lazy<IUrlHelper> _urlHelper;

        public MessageModelHelper(
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            Lazy<IUrlHelper> urlHelper)
        {
            _dateTimeHelper = dateTimeHelper;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _urlHelper = urlHelper;
        }

        public void ApplyCustomerContentPart(IDictionary<string, object> model, CustomerContent content, MessageContext ctx)
        {
            model["CustomerId"] = content.CustomerId;
            model["IpAddress"] = content.IpAddress;
            model["CreatedOn"] = ToUserDate(content.CreatedOnUtc, ctx);
            model["UpdatedOn"] = ToUserDate(content.UpdatedOnUtc, ctx);
        }

        public string BuildUrl(string url, MessageContext ctx)
        {
            return ctx.BaseUri.GetLeftPart(UriPartial.Authority) + url.EnsureStartsWith('/');
        }

        public string BuildRouteUrl(object routeValues, MessageContext ctx)
        {
            return ctx.BaseUri.GetLeftPart(UriPartial.Authority) + _urlHelper.Value?.RouteUrl(routeValues);
        }

        public string BuildRouteUrl(string routeName, object routeValues, MessageContext ctx)
        {
            return ctx.BaseUri.GetLeftPart(UriPartial.Authority) + _urlHelper.Value?.RouteUrl(routeName, routeValues);
        }

        public string BuildActionUrl(string action, string controller, object routeValues, MessageContext ctx)
        {
            return ctx.BaseUri.GetLeftPart(UriPartial.Authority) + _urlHelper.Value?.Action(action, controller, routeValues);
        }

        public async Task PublishModelPartCreatedEventAsync<T>(T source, dynamic part) where T : class
        {
            await _eventPublisher.PublishAsync(new MessageModelPartCreatedEvent<T>(source, part));
        }

        public string GetBoolResource(bool value, MessageContext ctx)
        {
            return _localizationService.GetResource(value ? "Common.Yes" : "Common.No", ctx.Language.Id);
        }

        public DateTime? ToUserDate(DateTime? utcDate, MessageContext messageContext)
        {
            if (utcDate == null)
                return null;

            return _dateTimeHelper.ConvertToUserTime(
                utcDate.Value,
                TimeZoneInfo.Utc,
                _dateTimeHelper.GetCustomerTimeZone(messageContext.Customer));
        }

        public static string GetDisplayNameForCustomer(Customer customer)
        {
            return customer.GetFullName().NullEmpty() ?? customer.FindEmail();
        }

        // INFO: parameters must be of type 'string'. Type 'object' outputs nothing.
        public static string[] GetValidValues(params string[] values)
        {
            return values.Where(x => CommonHelper.IsTruthy(x)).ToArray();
        }
    }
}
