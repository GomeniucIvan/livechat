namespace Smartstore.Core.Configuration
{
    /// <summary>
    /// Responsible for activating and populating setting class instances that implement <see cref="ISettings"/>.
    /// Instances are cached as singleton objects: CacheKey is composed of class name.
    /// </summary>
    public interface ISettingFactory
    {
        /// <summary>
        /// Loads settings.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        T LoadSettings<T>() where T : ISettings, new();

        /// <summary>
        /// Loads settings.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        Task<T> LoadSettingsAsync<T>() where T : ISettings, new();

        /// <summary>
        /// Loads settings.
        /// </summary>
        /// <param name="settingType">Setting class type</param>
        ISettings LoadSettings(Type settingType);

        /// <summary>
        /// Loads settings.
        /// </summary>
        /// <param name="settingType">Setting class type</param>
        Task<ISettings> LoadSettingsAsync(Type settingType);

        /// <summary>
        /// Save settings object. This methods commits changes to database.
        /// </summary>
        /// <typeparam name="T">Settings type</typeparam>
        /// <param name="settings">Setting instance</param>
        /// <returns>The number of setting entities committed to database.</returns>
        Task<int> SaveSettingsAsync<T>(T settings) where T : ISettings, new();
    }
}