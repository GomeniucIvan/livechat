namespace Smartstore.Core.Stores
{
    /// <summary>
    /// Store context
    /// </summary>
    public interface IStoreContext
    {
        /// <summary>
        /// Gets or sets the current store
        /// </summary>
        /// <remarks>Setter is for virtualization and testing purposes</remarks>
        Store CurrentStore { get; set; }

        /// <summary>
        /// Gets a cache wrapper for all untracked store entities.
        /// </summary>
        StoreEntityCache GetCachedStores();

        /// <summary>
        /// Sets a store override to be used for the current request
        /// </summary>
        void SetRequestStore();

        /// <summary>
        /// Gets the store override for the current request
        /// </summary>
        /// <returns>The store override or <c>null</c></returns>
        int? GetRequestStore();
    }
}
