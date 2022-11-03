using Smartstore.Collections;

namespace Smartstore.Web.Models.Catalog
{
    public interface IListActions
    {
        ProductSummaryViewMode ViewMode { get; }
        bool AllowViewModeChanging { get; }

        bool AllowFiltering { get; }

        bool AllowSorting { get; }
        int? CurrentSortOrder { get; }
        string CurrentSortOrderName { get; }
        string RelevanceSortOrderName { get; }
        Dictionary<int, string> AvailableSortOptions { get; }

        IPageable PagedList { get; }
        IEnumerable<int> AvailablePageSizes { get; }
    }

    public enum ProductSummaryViewMode
    {
        Mini,
        Grid,
        List,
        Compare
    }
}
