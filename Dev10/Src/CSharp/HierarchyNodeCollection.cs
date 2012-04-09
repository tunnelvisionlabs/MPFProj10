/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using IEnumerable = System.Collections.IEnumerable;
    using IEnumerator = System.Collections.IEnumerator;
    using Interlocked = System.Threading.Interlocked;
    using LockRecursionPolicy = System.Threading.LockRecursionPolicy;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;

    public class HierarchyNodeCollection : IEnumerable<KeyValuePair<uint, HierarchyNode>>
    {
        private readonly ProjectNode _projectManager;
        private readonly IEqualityComparer<string> _canonicalNameComparer;
        private readonly ReaderWriterLockSlim _syncObject = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly Dictionary<uint, HierarchyNode> _nodes = new Dictionary<uint, HierarchyNode>();
        private readonly Dictionary<HierarchyNode, uint> _itemIds = new Dictionary<HierarchyNode, uint>(ObjectReferenceEqualityComparer<HierarchyNode>.Default);

        private readonly HashSet<HierarchyNode> _nonCacheableCanonicalNameNodes = new HashSet<HierarchyNode>(ObjectReferenceEqualityComparer<HierarchyNode>.Default);
        private readonly Dictionary<HierarchyNode, string> _nodeToCanonicalNameMap = new Dictionary<HierarchyNode, string>(ObjectReferenceEqualityComparer<HierarchyNode>.Default);
        // TODO: create a dictionary for the common case of only having a single value for a particular canonical name
        private readonly Dictionary<string, List<HierarchyNode>> _canonicalNameToNodesMap;

        private int _nextNode;

        public HierarchyNodeCollection(ProjectNode projectManager, IEqualityComparer<string> canonicalNameComparer)
        {
            new ObjectReferenceEqualityComparer<HierarchyNode>();
            if (projectManager == null)
                throw new ArgumentNullException("projectManager");

            _projectManager = projectManager;
            _canonicalNameComparer = canonicalNameComparer ?? EqualityComparer<string>.Default;
            _canonicalNameToNodesMap = new Dictionary<string, List<HierarchyNode>>(_canonicalNameComparer);
        }

        public ProjectNode ProjectManager
        {
            get
            {
                //Contract.Ensures(Contract.Result<ProjectNode>() != null);

                return _projectManager;
            }
        }

        public int Count
        {
            get
            {
                //Contract.Ensures(Contract.Result<int>() >= 0);

                return _nodes.Count;
            }
        }

        public HierarchyNode this[uint itemId]
        {
            get
            {
                _syncObject.EnterReadLock();
                try
                {
                    HierarchyNode node;
                    if (!_nodes.TryGetValue(itemId, out node))
                        return null;

                    return node;
                }
                finally
                {
                    _syncObject.ExitReadLock();
                }
            }
        }

        public uint Add(HierarchyNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            _syncObject.EnterWriteLock();
            try
            {
                uint itemId = (uint)Interlocked.Increment(ref _nextNode);
                _itemIds.Add(node, itemId);
                _nodes.Add(itemId, node);
                // always add the node as non-cacheable since the canonical name may not be initialized when this method is called.
                _nonCacheableCanonicalNameNodes.Add(node);
                return itemId;
            }
            finally
            {
                _syncObject.ExitWriteLock();
            }
        }

        public void Remove(HierarchyNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            _syncObject.EnterWriteLock();
            try
            {
                uint itemId;
                if (!_itemIds.TryGetValue(node, out itemId))
                    return;

                _itemIds.Remove(node);
                _nodes.Remove(itemId);

                // remove any existing copy of this name
                if (!_nonCacheableCanonicalNameNodes.Remove(node))
                {
                    string previousName;
                    if (_nodeToCanonicalNameMap.TryGetValue(node, out previousName))
                    {
                        List<HierarchyNode> previousList;
                        if (_canonicalNameToNodesMap.TryGetValue(previousName, out previousList))
                        {
                            previousList.Remove(node);
                            if (previousList.Count == 0)
                                _canonicalNameToNodesMap.Remove(previousName);
                        }

                        _nodeToCanonicalNameMap.Remove(node);
                    }
                }
            }
            finally
            {
                _syncObject.ExitWriteLock();
            }
        }

        public ReadOnlyCollection<HierarchyNode> GetNodesByName(string canonicalName)
        {
            List<HierarchyNode> nodes = new List<HierarchyNode>();

            _syncObject.EnterReadLock();
            try
            {
                List<HierarchyNode> cachedNodes;
                if (_canonicalNameToNodesMap.TryGetValue(canonicalName, out cachedNodes))
                    nodes.AddRange(cachedNodes);

                nodes.AddRange(_nonCacheableCanonicalNameNodes.Where(i => _canonicalNameComparer.Equals(canonicalName, i.CanonicalName)));
            }
            finally
            {
                _syncObject.ExitReadLock();
            }

            return nodes.AsReadOnly();
        }

        public void UpdateAllCanonicalNames()
        {
            KeyValuePair<HierarchyNode, string>[] itemsToCheck = _nodeToCanonicalNameMap.ToArray();
            foreach (var item in itemsToCheck)
            {
                if (!item.Key.CanCacheCanonicalName || !_canonicalNameComparer.Equals(item.Value, item.Key.CanonicalName))
                    UpdateCanonicalName(item.Key);
            }
        }

        public void UpdateCanonicalName(HierarchyNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (!node.CanCacheCanonicalName)
            {
                _syncObject.EnterWriteLock();
                try
                {
                    if (_nonCacheableCanonicalNameNodes.Add(node))
                    {
                        string previousName;
                        if (_nodeToCanonicalNameMap.TryGetValue(node, out previousName))
                        {
                            List<HierarchyNode> previousList;
                            if (_canonicalNameToNodesMap.TryGetValue(previousName, out previousList))
                            {
                                previousList.Remove(node);
                                if (previousList.Count == 0)
                                    _canonicalNameToNodesMap.Remove(previousName);
                            }

                            _nodeToCanonicalNameMap.Remove(node);
                        }
                    }
                }
                finally
                {
                    _syncObject.ExitWriteLock();
                }

                return;
            }
            else
            {
                string canonicalName = node.CanonicalName;

                _syncObject.EnterWriteLock();
                try
                {
                    // remove any existing copy of this name
                    if (!_nonCacheableCanonicalNameNodes.Remove(node))
                    {
                        string previousName;
                        if (_nodeToCanonicalNameMap.TryGetValue(node, out previousName))
                        {
                            List<HierarchyNode> previousList;
                            if (_canonicalNameToNodesMap.TryGetValue(previousName, out previousList))
                            {
                                previousList.Remove(node);
                                if (previousList.Count == 0)
                                    _canonicalNameToNodesMap.Remove(previousName);
                            }

                            _nodeToCanonicalNameMap.Remove(node);
                        }
                    }

                    _nodeToCanonicalNameMap.Add(node, canonicalName);
                    List<HierarchyNode> currentList;
                    if (!_canonicalNameToNodesMap.TryGetValue(canonicalName, out currentList))
                    {
                        currentList = new List<HierarchyNode>();
                        _canonicalNameToNodesMap.Add(canonicalName, currentList);
                    }

                    currentList.Add(node);
                }
                finally
                {
                    _syncObject.ExitWriteLock();
                }

                return;
            }
        }

        public IEnumerator<KeyValuePair<uint, HierarchyNode>> GetEnumerator()
        {
            _syncObject.EnterReadLock();
            try
            {
                return new Dictionary<uint, HierarchyNode>(_nodes).GetEnumerator();
            }
            finally
            {
                _syncObject.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
