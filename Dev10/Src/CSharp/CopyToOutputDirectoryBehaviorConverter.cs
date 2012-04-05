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
    using System.ComponentModel;
    using CultureInfo = System.Globalization.CultureInfo;

    public class CopyToOutputDirectoryBehaviorConverter : EnumConverter
    {
        public CopyToOutputDirectoryBehaviorConverter()
            : base(typeof(CopyToOutputDirectoryBehavior))
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;

            if (str != null)
            {
                if (string.Equals(str, SR.GetString(CopyToOutputDirectoryBehavior.DoNotCopy.ToString()), StringComparison.OrdinalIgnoreCase))
                    return CopyToOutputDirectoryBehavior.DoNotCopy;
                else if (string.Equals(str, SR.GetString(CopyToOutputDirectoryBehavior.Always.ToString()), StringComparison.OrdinalIgnoreCase))
                    return CopyToOutputDirectoryBehavior.Always;
                else if (string.Equals(str, SR.GetString(CopyToOutputDirectoryBehavior.PreserveNewest.ToString()), StringComparison.OrdinalIgnoreCase))
                    return CopyToOutputDirectoryBehavior.PreserveNewest;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((value is CopyToOutputDirectoryBehavior || value == null) && destinationType == typeof(string))
            {
                string result = null;

                if (value == null)
                    result = SR.GetString(CopyToOutputDirectoryBehavior.DoNotCopy.ToString(), culture);
                else
                    result = SR.GetString(((CopyToOutputDirectoryBehavior)value).ToString(), culture);

                if (result != null)
                    return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new CopyToOutputDirectoryBehavior[] { CopyToOutputDirectoryBehavior.DoNotCopy, CopyToOutputDirectoryBehavior.Always, CopyToOutputDirectoryBehavior.PreserveNewest });
        }
    }
}
