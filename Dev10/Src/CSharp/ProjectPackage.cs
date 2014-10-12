/********************************************************************************************

Copyright (c) Microsoft Corporation 
All rights reserved. 

Microsoft Public License: 

This license governs use of the accompanying software. If you use the software, you 
accept this license. If you do not accept the license, do not use the software. 

1. Definitions 
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the 
same meaning here as under U.S. copyright law. 
A "contribution" is the original software, or any additions or changes to the software. 
A "contributor" is any person that distributes its contribution under this license. 
"Licensed patents" are a contributor's patent claims that read directly on its contribution. 

2. Grant of Rights 
(A) Copyright Grant- Subject to the terms of this license, including the license conditions 
and limitations in section 3, each contributor grants you a non-exclusive, worldwide, 
royalty-free copyright license to reproduce its contribution, prepare derivative works of 
its contribution, and distribute its contribution or any derivative works that you create. 
(B) Patent Grant- Subject to the terms of this license, including the license conditions 
and limitations in section 3, each contributor grants you a non-exclusive, worldwide, 
royalty-free license under its licensed patents to make, have made, use, sell, offer for 
sale, import, and/or otherwise dispose of its contribution in the software or derivative 
works of the contribution in the software. 

3. Conditions and Limitations 
(A) No Trademark License- This license does not grant you rights to use any contributors' 
name, logo, or trademarks. 
(B) If you bring a patent claim against any contributor over patents that you claim are 
infringed by the software, your patent license from such contributor to the software ends 
automatically. 
(C) If you distribute any portion of the software, you must retain all copyright, patent, 
trademark, and attribution notices that are present in the software. 
(D) If you distribute any portion of the software in source code form, you may do so only 
under this license by including a complete copy of this license with your distribution. 
If you distribute any portion of the software in compiled or object code form, you may only 
do so under a license that complies with this license. 
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give 
no express warranties, guarantees or conditions. You may have additional consumer rights 
under your local laws which this license cannot change. To the extent permitted under your 
local laws, the contributors exclude the implied warranties of merchantability, fitness for 
a particular purpose and non-infringement.

********************************************************************************************/

namespace Microsoft.VisualStudio.Project
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Microsoft.VisualStudio.Project.PropertyPages;
	using Interlocked = System.Threading.Interlocked;
	using IVsComponentSelectorProvider = Microsoft.VisualStudio.Shell.Interop.IVsComponentSelectorProvider;
	using ObjectExtenders = EnvDTE.ObjectExtenders;
	using VSPROPSHEETPAGE = Microsoft.VisualStudio.Shell.Interop.VSPROPSHEETPAGE;

	/// <summary>
	/// Defines abstract package.
	/// </summary>
	[ComVisible(true)]
	[CLSCompliant(false)]
	public abstract class ProjectPackage : Microsoft.VisualStudio.Shell.Package, IVsComponentSelectorProvider
	{
		#region fields
		/// <summary>
		/// This is the place to register all the solution listeners.
		/// </summary>
		private List<SolutionListener> solutionListeners = new List<SolutionListener>();

		/// <summary>
		/// This field is set to <see langword="true"/> when <see cref="Initialize"/> is called,
		/// indicating that the reference count in <see cref="_singleFileGeneratorNodeExtenderReferenceCount"/>
		/// has been incremented and needs to be decremented when <see cref="Dispose"/> is called.
		/// </summary>
		/// <remarks>
		/// Once set to <see langword="true"/>, this field is never altered. The <see cref="_disposed"/>
		/// field tracks whether a call to <see cref="Dispose(bool)"/> has occurred.
		/// </remarks>
		private bool _initialized;

		/// <summary>
		/// This field is set to <see langword="true"/> when <see cref="Dispose(bool)"/> is called,
		/// indicating that the reference count in <see cref="_singleFileGeneratorNodeExtenderReferenceCount"/>
		/// has been decremented (if required), and should not be decremented again if <see cref="Dispose(bool)"/>
		/// is called multiple times.
		/// </summary>
		private bool _disposed;

		/// <summary>
		/// This field tracks the reference count for the number of <see cref="ProjectPackage"/> instances
		/// which are initialized and need the <see cref="_singleFileGeneratorNodeExtenderProvider"/>
		/// object to be registered with the <see cref="ObjectExtenders"/> service.
		/// </summary>
		private static int _singleFileGeneratorNodeExtenderReferenceCount;
		private static SingleFileGeneratorNodeExtenderProvider _singleFileGeneratorNodeExtenderProvider;
		private static int _singleFileGeneratorNodeExtenderCookie;

		#endregion

		#region properties
		/// <summary>
		/// Add your listener to this list. They should be added in the overridden Initialize befaore calling the base.
		/// </summary>
		public IList<SolutionListener> SolutionListeners
		{
			get
			{
				return this.solutionListeners;
			}
		}

		public abstract string ProductUserContext { get; }

		#endregion

		#region methods
		protected override void Initialize()
		{
			base.Initialize();

			// Subscribe to the solution events
			this.solutionListeners.Add(new SolutionListenerForProjectReferenceUpdate(this));
			this.solutionListeners.Add(new SolutionListenerForProjectOpen(this));
			this.solutionListeners.Add(new SolutionListenerForBuildDependencyUpdate(this));
			this.solutionListeners.Add(new SolutionListenerForProjectEvents(this));

			foreach(SolutionListener solutionListener in this.solutionListeners)
			{
				solutionListener.Init();
			}

			try
			{
				// this block assumes that the ProjectPackage instances will all be initialized on the same thread,
				// but doesn't assume that only one ProjectPackage instance exists at a time
				if (Interlocked.Increment(ref _singleFileGeneratorNodeExtenderReferenceCount) == 1)
				{
					ObjectExtenders objectExtenders = (ObjectExtenders)GetService(typeof(ObjectExtenders));
					_singleFileGeneratorNodeExtenderProvider = new SingleFileGeneratorNodeExtenderProvider();
					string extenderCatId = typeof(FileNodeProperties).GUID.ToString("B");
					string extenderName = SingleFileGeneratorNodeExtenderProvider.Name;
					string localizedName = extenderName;
					_singleFileGeneratorNodeExtenderCookie = objectExtenders.RegisterExtenderProvider(extenderCatId, extenderName, _singleFileGeneratorNodeExtenderProvider, localizedName);
				}
			}
			finally
			{
				_initialized = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			// Unadvise solution listeners.
			try
			{
				if(disposing)
				{
					// only decrement the reference count once, regardless of the number of times Dispose is called.
					// Ignore if Initialize was never called.
					if (_initialized && !_disposed && Interlocked.Decrement(ref _singleFileGeneratorNodeExtenderReferenceCount) == 0)
					{
						ObjectExtenders objectExtenders = (ObjectExtenders)GetService(typeof(ObjectExtenders));
						objectExtenders.UnregisterExtenderProvider(_singleFileGeneratorNodeExtenderCookie);
					}

					foreach(SolutionListener solutionListener in this.solutionListeners)
					{
						solutionListener.Dispose();
					}
				}
			}
			finally
			{
				_disposed = true;
				base.Dispose(disposing);
			}
		}
		#endregion

		#region IVsComponentSelectorProvider Members

		/// <inheritdoc/>
		int IVsComponentSelectorProvider.GetComponentSelectorPage(ref Guid rguidPage, VSPROPSHEETPAGE[] ppage)
		{
			if (ppage == null || ppage.Length == 0)
				return VSConstants.E_INVALIDARG;

			ComponentSelectorControl page;
			PropertySheetPageFlags flags;
			int result = GetComponentSelectorPage(rguidPage, out page, out flags);
			if (ErrorHandler.Failed(result))
				return result;

			const PropertySheetPageFlags supportedFlags = PropertySheetPageFlags.PSP_HASHELP | PropertySheetPageFlags.PSP_HIDEHEADER;
			if (page == null)
				throw new NotSupportedException();
			if ((flags & ~supportedFlags) != 0)
				throw new NotSupportedException();

			ppage[0] = new VSPROPSHEETPAGE()
			{
				dwFlags = (uint)flags,
				dwReserved = 0,
				dwSize = (uint)Marshal.SizeOf(typeof(VSPROPSHEETPAGE)),
				dwTemplateSize = 0,
				HINSTANCE = 0,
				hwndDlg = page.Handle,
				lParam = IntPtr.Zero,
				pcRefParent = IntPtr.Zero,
				pfnCallback = IntPtr.Zero,
				pfnDlgProc = IntPtr.Zero,
				pTemplate = IntPtr.Zero,
				wTemplateId = 0,
			};

			return result;
		}

		/// <summary>
		/// Create a component selector page.
		/// </summary>
		/// <param name="page">The ID of the component selector page. For custom pages, this will be the GUID of the type
		/// specified in the <see cref="ProvideComponentSelectorTabAttribute"/> attribute.</param>
		/// <param name="control">The component selector control to display.</param>
		/// <param name="flags">A combination of flags from the <see cref="PropertySheetPageFlags"/> enumeration specifying
		/// the behavior of the component selector.</param>
		/// <returns>
		/// <para><see cref="VSConstants.S_OK"/> if a control is provided for the specified <paramref name="page"/>.</para>
		/// <para>-or-</para>
		/// <para>An error code if the requested control could not be created.</para>
		/// </returns>
		protected virtual int GetComponentSelectorPage(Guid page, out ComponentSelectorControl control, out PropertySheetPageFlags flags)
		{
			control = null;
			flags = default(PropertySheetPageFlags);
			return VSConstants.E_INVALIDARG;
		}

		#endregion
	}
}
