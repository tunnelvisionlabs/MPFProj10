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
	using System.Diagnostics;
    using System.Runtime.InteropServices;
	using Microsoft.VisualStudio.OLE.Interop;

    internal static class NativeMethods
	{
		public const int
		WM_KEYFIRST = 0x0100,
		WM_KEYLAST = 0x0108,
		WM_MOUSEFIRST = 0x0200,
		WM_MOUSELAST = 0x020A;

		/// <devdoc>
		/// Please use this "approved" method to compare file names.
		/// </devdoc>
		public static bool IsSamePath(string file1, string file2)
		{
			if(file1 == null || file1.Length == 0)
			{
				return (file2 == null || file2.Length == 0);
			}

			Uri uri1 = null;
			Uri uri2 = null;

			try
			{
				if(!Uri.TryCreate(file1, UriKind.Absolute, out uri1) || !Uri.TryCreate(file2, UriKind.Absolute, out uri2))
				{
					return false;
				}

				if(uri1 != null && uri1.IsFile && uri2 != null && uri2.IsFile)
				{
					return String.Equals(uri1.LocalPath, uri2.LocalPath, StringComparison.OrdinalIgnoreCase);
				}

				return file1 == file2;
			}
			catch(UriFormatException e)
			{
				Trace.WriteLine("Exception " + e.Message);
			}

			return false;
		}

		public const ushort CF_HDROP = 15; // winuser.h
		public const uint MK_CONTROL = 0x0008; //winuser.h
		public const uint MK_SHIFT = 0x0004;
		public const int MAX_PATH = 260; // windef.h	

		public const int ILD_NORMAL = 0x0000,
			ILD_TRANSPARENT = 0x0001,
			ILD_MASK = 0x0010,
			ILD_ROP = 0x0040;

		/// <summary>
		/// Changes the parent window of the specified child window.
		/// </summary>
		/// <param name="hWnd">Handle to the child window.</param>
		/// <param name="hWndParent">Handle to the new parent window. If this parameter is NULL, the desktop window becomes the new parent window.</param>
		/// <returns>A handle to the previous parent window indicates success. NULL indicates failure.</returns>
		[DllImport(ExternDll.User32)]
		public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

		[DllImport(ExternDll.User32)]
		public static extern bool DestroyIcon(IntPtr handle);

		[DllImport("user32.dll", EntryPoint = "IsDialogMessageA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool IsDialogMessageA(IntPtr hDlg, ref MSG msg);

		/// <summary>
		/// Indicates whether the file type is binary or not
		/// </summary>
		/// <param name="lpApplicationName">Full path to the file to check</param>
		/// <param name="lpBinaryType">If file isbianry the bitness of the app is indicated by lpBinaryType value.</param>
		/// <returns>True if the file is binary false otherwise</returns>
		[DllImport(ExternDll.Kernel32)]
		public static extern bool GetBinaryType([MarshalAs(UnmanagedType.LPWStr)]string lpApplicationName, out uint lpBinaryType);

	}
}

