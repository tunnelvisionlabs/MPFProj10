namespace Microsoft.VisualStudio.Project
{
    using System;

    /// <summary>
    /// Specifies the priority for a file filter within a particular context.
    /// </summary>
    /// <seealso cref="ProvideFilterAttribute"/>
    public enum FilterBrowsableState
    {
        /// <summary>
        /// The file filter is not visible.
        /// </summary>
        None,

        /// <summary>
        /// The file filter is visible, but after any <see cref="Common"/> items.
        /// </summary>
        Standard,

        /// <summary>
        /// The file filter is visible, and placed before any <see cref="Standard"/> filters.
        /// </summary>
        Common,
    }
}
