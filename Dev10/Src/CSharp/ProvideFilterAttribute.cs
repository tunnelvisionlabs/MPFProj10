namespace Microsoft.VisualStudio.Project
{
    using System;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This registration attribute provides the ability to specify file filters associated with a project system, which
    /// may appear in the <strong>Find in Files</strong>, <strong>Open File</strong>, and/or
    /// <strong>Add Existing Item</strong> dialogs.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-US/library/bb165750.aspx">Registering Project and Item Templates (Microsoft Developer Network)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ProvideFilterAttribute : RegistrationAttribute
    {
        /// <summary>
        /// This is the backing field for the <see cref="ProjectFactoryType"/> and <see cref="ProjectFactoryGuid"/>
        /// properties.
        /// </summary>
        private readonly Type _projectFactoryType;

        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// This is the backing field for the <see cref="Filter"/> property.
        /// </summary>
        private readonly string _filter;

        /// <summary>
        /// This is the backing field for the <see cref="SortPriority"/> property.
        /// </summary>
        private int _sortPriority;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProvideFilterAttribute"/> class with the specified project
        /// factory type and name.
        /// </summary>
        /// <param name="projectFactoryType">The project factory to associate this file filter with.</param>
        /// <param name="name">The name of the filter. This name must be a unique key for the filters included with the
        /// specified project factory, but is otherwise ignored.</param>
        /// <param name="filter">The filter value. This may be specified as a constant, such as
        /// <c>"MyLanguage Files (*.myl,*.myl2);*.myl,*.myl2"</c>, or as a resource, such as <c>"#300"</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="projectFactoryType"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="name"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="filter"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>If <paramref name="name"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="filter"/> is empty.</para>
        /// </exception>
        public ProvideFilterAttribute(Type projectFactoryType, string name, string filter)
        {
            if (projectFactoryType == null)
                throw new ArgumentNullException("projectFactoryType");
            if (name == null)
                throw new ArgumentNullException("name");
            if (filter == null)
                throw new ArgumentNullException("filter");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty", "name");
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException("filter cannot be empty", "filter");

            _projectFactoryType = projectFactoryType;
            _name = name;
            _filter = filter;
            FindInFiles = FilterBrowsableState.None;
            OpenFile = FilterBrowsableState.Standard;
            AddExistingItem = FilterBrowsableState.Standard;
        }

        /// <summary>
        /// Gets the type of the project factory for the project system the file filter is associated with.
        /// </summary>
        /// <value>
        /// The type of the project factory for the project system the file filter is associated with.
        /// </value>
        public Type ProjectFactoryType
        {
            get
            {
                return _projectFactoryType;
            }
        }

        /// <summary>
        /// Gets the GUID of the project factory for the project system the file filter is associated with.
        /// </summary>
        /// <value>
        /// The GUID of the project factory for the project system the file filter is associated with.
        /// </value>
        public Guid ProjectFactoryGuid
        {
            get
            {
                return _projectFactoryType.GUID;
            }
        }

        /// <summary>
        /// Gets the name of the file filter. This value must be unique across all file filters associated with a
        /// specific project system, but is otherwise ignored.
        /// </summary>
        /// <value>
        /// The unique name of the file filter within this project system.
        /// </value>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the file filter value.
        /// </summary>
        /// <value>
        /// <para>A file filter specification, such as <c>"MyLanguage Files (*.myl,*.myl2);*.myl,*.myl2"</c>.</para>
        /// <para>-or-</para>
        /// <para>A reference to an embedded resource string containing the filter, such as <c>"#300"</c>.</para>
        /// </value>
        public string Filter
        {
            get
            {
                return _filter;
            }
        }

        /// <summary>
        /// Specifies the behavior of this file filter
        /// </summary>
        /// <value>
        /// A <see cref="FilterBrowsableState"/> value specifying the behavior of this file filter in the
        /// <strong>Find in Files</strong> dialog.
        /// </value>
        public FilterBrowsableState FindInFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the behavior of this file filter in the <strong>Open File</strong> dialog.
        /// </summary>
        /// <value>
        /// A <see cref="FilterBrowsableState"/> value specifying the behavior of this file filter in the
        /// <strong>Open File</strong> dialog.
        /// </value>
        public FilterBrowsableState OpenFile
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the behavior of this file filter in the <strong>Add Existing Item</strong> dialog.
        /// </summary>
        /// <remarks>
        /// <para>In the current implementation, the <see cref="FilterBrowsableState.Standard"/> and
        /// <see cref="FilterBrowsableState.Common"/> values are treated equivalently for this context.</para>
        /// </remarks>
        /// <value>
        /// A <see cref="FilterBrowsableState"/> value specifying the behavior of this file filter in the
        /// <strong>Add Existing Item</strong> dialog.
        /// </value>
        public FilterBrowsableState AddExistingItem
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the sort priority of the file filter, with higher values appearing earlier in the list of filters.
        /// Note that the <see cref="FilterBrowsableState"/> value takes precedence over this value.
        /// </summary>
        /// <value>
        /// The sort priority of the file filter.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is less than 0.</exception>
        public int SortPriority
        {
            get
            {
                return _sortPriority;
            }

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                _sortPriority = value;
            }
        }

        private static string GetPathToKey(Guid projectGuid, string name)
        {
            return string.Format(@"Projects\{0:B}\Filters\{1}", projectGuid, name);
        }

        /// <inheritdoc/>
        public override void Register(RegistrationContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            using (Key childKey = context.CreateKey(GetPathToKey(ProjectFactoryGuid, _name)))
            {
                childKey.SetValue(string.Empty, _filter);

                if (FindInFiles == FilterBrowsableState.Common)
                    childKey.SetValue("CommonFindFilesFilter", 1);
                if (OpenFile == FilterBrowsableState.Common)
                    childKey.SetValue("CommonOpenFilesFilter", 1);
                if (FindInFiles == FilterBrowsableState.Standard)
                    childKey.SetValue("FindInFilesFilter", 1);
                if (OpenFile == FilterBrowsableState.None)
                    childKey.SetValue("NotOpenFileFilter", 1);
                if (AddExistingItem == FilterBrowsableState.None)
                    childKey.SetValue("NotAddExistingItemFilter", 1);

                childKey.SetValue("SortPriority", SortPriority);
            }
        }

        /// <inheritdoc/>
        public override void Unregister(RegistrationContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.RemoveKey(GetPathToKey(_projectFactoryType.GUID, _name));
        }
    }
}
