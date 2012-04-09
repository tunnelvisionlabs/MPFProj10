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
	using System.Runtime.InteropServices;
	using CONNECTDATA = Microsoft.VisualStudio.OLE.Interop.CONNECTDATA;
	using IEnumConnections = Microsoft.VisualStudio.OLE.Interop.IEnumConnections;

	[ComVisible(true)]
	public class EnumConnections<TSink> : IEnumConnections
		where TSink : class
	{
		private readonly ReadOnlyCollection<KeyValuePair<uint, TSink>> _connections;
		private int _currentIndex;

		public EnumConnections(IEnumerable<KeyValuePair<uint, TSink>> connections)
		{
			if (connections == null)
				throw new ArgumentNullException("connections");

			_connections = new List<KeyValuePair<uint, TSink>>(connections).AsReadOnly();
		}

		private EnumConnections(ReadOnlyCollection<KeyValuePair<uint, TSink>> connections, int currentIndex)
		{
			if (connections == null)
				throw new ArgumentNullException("connections");

			_connections = connections;
			_currentIndex = currentIndex;
		}

		#region IEnumConnections Members

		public void Clone(out IEnumConnections ppEnum)
		{
			ppEnum = new EnumConnections<TSink>(_connections, _currentIndex);
		}

		public int Next(uint cConnections, CONNECTDATA[] rgcd, out uint pcFetched)
		{
			pcFetched = 0;

			if (rgcd == null || rgcd.Length < cConnections)
				return VSConstants.E_INVALIDARG;

			int remaining = _connections.Count - _currentIndex;
			pcFetched = checked((uint)Math.Min(cConnections, remaining));
			for (int i = 0; i < pcFetched; i++)
			{
				rgcd[i].dwCookie = _connections[_currentIndex + i].Key;
				rgcd[i].punk = _connections[_currentIndex + i].Value;
			}

			_currentIndex += (int)pcFetched;
			return pcFetched == cConnections ? VSConstants.S_OK : VSConstants.S_FALSE;
		}

		public int Reset()
		{
			_currentIndex = 0;
			return VSConstants.S_OK;
		}

		public int Skip(uint cConnections)
		{
			int remaining = _connections.Count - _currentIndex;
			if (remaining < cConnections)
			{
				_currentIndex = _connections.Count;
				return VSConstants.S_FALSE;
			}

			_currentIndex += (int)cConnections;
			return VSConstants.S_OK;
		}

		#endregion
	}
}
