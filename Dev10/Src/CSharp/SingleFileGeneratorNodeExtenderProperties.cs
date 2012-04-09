﻿namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class SingleFileGeneratorNodeExtenderProperties : LocalizableProperties
    {
        private readonly HierarchyNode _node;

        public SingleFileGeneratorNodeExtenderProperties(HierarchyNode node)
            : base(GetProjectManager(node))
        {
            _node = node;
        }

        private static ProjectNode GetProjectManager(HierarchyNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            return node.ProjectManager;
        }

        public event EventHandler<HierarchyNodeEventArgs> CustomToolChanged;
        public event EventHandler<HierarchyNodeEventArgs> CustomToolNamespaceChanged;

        [Browsable(false)]
        [AutomationBrowsable(false)]
        public HierarchyNode Node
        {
            get
            {
                return _node;
            }
        }

        [SRCategoryAttribute(SR.Advanced)]
        [LocDisplayName(SR.CustomTool)]
        [SRDescriptionAttribute(SR.CustomToolDescription)]
        public virtual string CustomTool
        {
            get
            {
                return this.Node.ItemNode.GetMetadata(ProjectFileConstants.Generator);
            }

            set
            {
                if (CustomTool != value)
                {
                    this.Node.ItemNode.SetMetadata(ProjectFileConstants.Generator, !string.IsNullOrEmpty(value) ? value : null);
                    HierarchyNodeEventArgs args = new HierarchyNodeEventArgs(this.Node);
                    OnCustomToolChanged(args);
                }
            }
        }

        [SRCategoryAttribute(VisualStudio.Project.SR.Advanced)]
        [LocDisplayName(SR.CustomToolNamespace)]
        [SRDescriptionAttribute(SR.CustomToolNamespaceDescription)]
        public virtual string CustomToolNamespace
        {
            get
            {
                return this.Node.ItemNode.GetMetadata(ProjectFileConstants.CustomToolNamespace);
            }

            set
            {
                if (CustomToolNamespace != value)
                {
                    this.Node.ItemNode.SetMetadata(ProjectFileConstants.CustomToolNamespace, !string.IsNullOrEmpty(value) ? value : null);
                    HierarchyNodeEventArgs args = new HierarchyNodeEventArgs(this.Node);
                    OnCustomToolNamespaceChanged(args);
                }
            }
        }

        protected virtual void OnCustomToolChanged(HierarchyNodeEventArgs e)
        {
            var t = CustomToolChanged;
            if (t != null)
                t(this, e);
        }

        protected virtual void OnCustomToolNamespaceChanged(HierarchyNodeEventArgs e)
        {
            var t = CustomToolNamespaceChanged;
            if (t != null)
                t(this, e);
        }
    }
}
