using Smartstore.Core.Configuration;
using Smartstore.Core.Localization;
using Smartstore.Core.Stores;
using Smartstore.Scheduling;

namespace Smartstore.Core.Seo
{
    public class XmlSitemapBuildContext
    {
        private readonly ISettingFactory _settingFactory;

        public XmlSitemapBuildContext(Store store, Language[] languages, ISettingFactory settingFactory)
        {
            Guard.NotNull(store, nameof(store));
            Guard.NotEmpty(languages, nameof(languages));

            Store = store;
            Languages = languages;
            Protocol = store.SupportsHttps() && store.ForceSslForAllPages ? "https" : "http";

            _settingFactory = settingFactory;
        }

        public CancellationToken CancellationToken { get; init; }
        public ProgressCallback ProgressCallback { get; set; }
        public Store Store { get; init; }
        public Language[] Languages { get; init; }
        public int MaximumNodeCount { get; init; } = XmlSitemapGenerator.MaximumSiteMapNodeCount;

        public string Protocol { get; init; }

        public T LoadSettings<T>() where T : ISettings, new()
        {
            return _settingFactory.LoadSettings<T>();
        }

        public Task<T> LoadSettingsAsync<T>() where T : ISettings, new()
        {
            return _settingFactory.LoadSettingsAsync<T>();
        }
    }
}
