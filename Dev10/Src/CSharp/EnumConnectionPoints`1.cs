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
	using IConnectionPoint = Microsoft.VisualStudio.OLE.Interop.IConnectionPoint;
	using IEnumConnectionPoints = Microsoft.VisualStudio.OLE.Interop.IEnumConnectionPoints;

	[ComVisible(true)]
	public class EnumConnectionPoints : IEnumConnectionPoints
	{
		private readonly ReadOnlyCollection<IConnectionPoint> _connectionPoints;
		private int _currentIndex;

		public EnumConnectionPoints(IEnumerable<IConnectionPoint> connectionPoints)
		{
			if (connectionPoints == null)
				throw new ArgumentNullException("connectionPoints");

			_connectionPoints = new List<IConnectionPoint>(connectionPoints).AsReadOnly();
		}

		private EnumConnectionPoints(ReadOnlyCollection<IConnectionPoint> connectionPoints, int currentIndex)
		{
			if (connectionPoints == null)
				throw new ArgumentNullException("connectionPoints");

			_connectionPoints = connectionPoints;
			_currentIndex = currentIndex;
		}

		#region IEnumConnectionPoints Members

		public void Clone(out IEnumConnectionPoints ppEnum)
		{
			ppEnum = new EnumConnectionPoints(_connectionPoints, _currentIndex);
		}

		public int Next(uint cConnections, IConnectionPoint[] ppCP, out uint pcFetched)
		{
			pcFetched = 0;

			if (ppCP == null || ppCP.Length < cConnections)
				return VSConstants.E_INVALIDARG;

			int remaining = _connectionPoints.Count - _currentIndex;
			pcFetched = checked((uint)Math.Min(cConnections, remaining));
			for (int i = 0; i < pcFetched; i++)
				ppCP[i] = _connectionPoints[_currentIndex + i];

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
			int remaining = _connectionPoints.Count - _currentIndex;
			if (remaining < cConnections)
			{
				_currentIndex = _connectionPoints.Count;
				return VSConstants.S_FALSE;
			}

			_currentIndex += (int)cConnections;
			return VSConstants.S_OK;
		}

		#endregion
	}
}
