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
	using Microsoft.VisualStudio;
	using Microsoft.VisualStudio.OLE.Interop;
	using CONNECTDATA = Microsoft.VisualStudio.OLE.Interop.CONNECTDATA;

	public class ConnectionPoint<TSink> : IConnectionPoint
		where TSink : class
	{
		private readonly Dictionary<uint, TSink> sinks;
		private readonly ConnectionPointContainer container;
		private readonly IEventSource<TSink> source;
		private uint nextCookie;

		internal ConnectionPoint(ConnectionPointContainer container, IEventSource<TSink> source)
		{
			if(null == container)
			{
				throw new ArgumentNullException("container");
			}
			if(null == source)
			{
				throw new ArgumentNullException("source");
			}

			this.sinks = new Dictionary<uint, TSink>();
			this.container = container;
			this.source = source;
			this.nextCookie = 1;
		}

		#region IConnectionPoint Members
		public void Advise(object pUnkSink, out uint pdwCookie)
		{
			TSink sink = pUnkSink as TSink;
			if (sink == null)
				Marshal.ThrowExceptionForHR(VSConstants.E_NOINTERFACE);

			sinks.Add(nextCookie, sink);
			pdwCookie = nextCookie;
			source.OnSinkAdded(sink);
			nextCookie += 1;
		}

		public void EnumConnections(out IEnumConnections ppEnum)
		{
			ppEnum = new ConnectionEnumerator(sinks);
		}

		public void GetConnectionInterface(out Guid pIID)
		{
			pIID = typeof(TSink).GUID;
		}

		public void GetConnectionPointContainer(out IConnectionPointContainer ppCPC)
		{
			ppCPC = this.container;
		}

		public void Unadvise(uint dwCookie)
		{
			// This will throw if the cookie is not in the list.
			TSink sink = sinks[dwCookie];
			sinks.Remove(dwCookie);
			source.OnSinkRemoved(sink);
		}
		#endregion

		[ComVisible(true)]
		private class ConnectionEnumerator : IEnumConnections
		{
			private readonly ReadOnlyCollection<KeyValuePair<uint, TSink>> _connections;
			private int _currentIndex;

			internal ConnectionEnumerator(IEnumerable<KeyValuePair<uint, TSink>> connections)
			{
				if (connections == null)
					throw new ArgumentNullException("connections");

				_connections = new List<KeyValuePair<uint, TSink>>(connections).AsReadOnly();
			}

			private ConnectionEnumerator(ReadOnlyCollection<KeyValuePair<uint, TSink>> connections, int currentIndex)
			{
				if (connections == null)
					throw new ArgumentNullException("connections");

				_connections = connections;
				_currentIndex = currentIndex;
			}

			#region IEnumConnections Members

			public void Clone(out IEnumConnections ppEnum)
			{
				ppEnum = new ConnectionEnumerator(_connections, _currentIndex);
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
}
