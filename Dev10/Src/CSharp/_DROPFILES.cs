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
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DROPFILES")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
    public struct _DROPFILES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public int pFiles;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
        public int X;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
        public int Y;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [MarshalAs(UnmanagedType.Bool)]
        public bool fNC;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [MarshalAs(UnmanagedType.Bool)]
        public bool fWide;

        public static bool operator ==(_DROPFILES left, _DROPFILES right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(_DROPFILES left, _DROPFILES right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is _DROPFILES))
                return false;

            return this.Equals((_DROPFILES)obj);
        }

        public bool Equals(_DROPFILES other)
        {
            return this.pFiles == other.pFiles
                && this.X == other.X
                && this.Y == other.Y
                && this.fNC == other.fNC
                && this.fWide == other.fWide;
        }

        public override int GetHashCode()
        {
            int hashCode = 5;
            hashCode = 31 * hashCode ^ pFiles;
            hashCode = 31 * hashCode ^ X;
            hashCode = 31 * hashCode ^ Y;
            hashCode = 31 * hashCode ^ (fNC ? 1 : 0);
            hashCode = 31 * hashCode ^ (fWide ? 1 : 0);
            return hashCode;
        }
    }
}
