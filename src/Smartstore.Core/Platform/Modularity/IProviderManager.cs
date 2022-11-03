using Smartstore.Core.Configuration;

namespace Smartstore.Engine.Modularity
{
    /// <summary>
    /// Manages provider implementations
    /// </summary>
    public interface IProviderManager
    {
        /// <summary>
        /// Gets a provider of type <typeparamref name="TProvider"/> by system name.
        /// </summary>
        Provider<TProvider> GetProvider<TProvider>(string systemName) where TProvider : IProvider;

        /// <summary>
        /// Gets a provider by system name.
        /// </summary>
        Provider<IProvider> GetProvider(string systemName);

        /// <summary>
        /// Enumerates all providers of type <typeparamref name="TProvider"/> lazily without instantiating them.
        /// </summary>
        IEnumerable<Provider<TProvider>> GetAllProviders<TProvider>() where TProvider : IProvider;

        /// <summary>
        /// Enumerates all providers lazily without instantiating them.
        /// </summary>
        IEnumerable<Provider<IProvider>> GetAllProviders();

        /// <summary>
        /// Gets a user setting for the given provider.
        /// </summary>
        T GetUserSetting<T>(ProviderMetadata metadata, Expression<Func<ProviderMetadata, T>> propertyAccessor);

        /// <summary>
        /// Applies a user setting for the given provider. The caller is responsible for database commit.
        /// </summary>
        ApplySettingResult ApplyUserSetting<T>(ProviderMetadata metadata, Expression<Func<ProviderMetadata, T>> propertyAccessor);
    }
}