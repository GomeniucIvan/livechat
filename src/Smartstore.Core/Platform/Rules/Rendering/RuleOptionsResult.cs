namespace Smartstore.Core.Rules.Rendering
{
    public enum RuleOptionsRequestReason
    {
        /// <summary>
        /// Get options for select list.
        /// </summary>
        SelectListOptions = 0,

        /// <summary>
        /// Get display names of selected options.
        /// </summary>
        SelectedDisplayNames
    }


    public class RuleOptionsResult
    {
        /// <summary>
        /// Select list options or display names of selected values, depending on <see cref="RuleOptionsRequestReason"/>.
        /// </summary>
        public IList<RuleValueSelectListOption> Options { get; init; } = new List<RuleValueSelectListOption>();

        /// <summary>
        /// Indicates whether the provided data is paged.
        /// </summary>
        public bool IsPaged { get; set; }

        /// <summary>
        /// Indicates whether further data is available.
        /// </summary>
        public bool HasMoreData { get; set; }
    }
}
