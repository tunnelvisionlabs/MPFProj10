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
    using System.Collections.Generic;
    using RuntimeHelpers = System.Runtime.CompilerServices.RuntimeHelpers;

    public class ObjectReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        private static readonly ObjectReferenceEqualityComparer<T> _default = new ObjectReferenceEqualityComparer<T>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static ObjectReferenceEqualityComparer<T> Default
        {
            get
            {
                return _default;
            }
        }

        public bool Equals(T x, T y)
        {
            return object.ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
