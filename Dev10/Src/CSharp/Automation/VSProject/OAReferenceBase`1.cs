/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Microsoft.VisualStudio.Project.Automation
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.InteropServices;
	using prjReferenceType = VSLangProj.prjReferenceType;
	using Reference = VSLangProj.Reference;
	using References = VSLangProj.References;

	/// <summary>
	/// Represents the automation equivalent of ReferenceNode
	/// </summary>
	/// <typeparam name="RefType"></typeparam>
	[SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
	[ComVisible(true)]
	public abstract class OAReferenceBase<TReferenceNode> : Reference
		where TReferenceNode : ReferenceNode
	{
		private readonly TReferenceNode _referenceNode;

		protected OAReferenceBase(TReferenceNode referenceNode)
		{
			this._referenceNode = referenceNode;
		}

		protected TReferenceNode BaseReferenceNode
		{
			get
			{
				return _referenceNode;
			}
		}

		#region Reference Members

		public virtual References Collection
		{
			get
			{
				return BaseReferenceNode.Parent.Object as References;
			}
		}

		public virtual EnvDTE.Project ContainingProject
		{
			get
			{
				return BaseReferenceNode.ProjectManager.GetAutomationObject() as EnvDTE.Project;
			}
		}

		public virtual bool CopyLocal
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string Culture
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual EnvDTE.DTE DTE
		{
			get
			{
				return BaseReferenceNode.ProjectManager.Site.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
			}
		}

		public virtual string Description
		{
			get
			{
				return this.Name;
			}
		}

		public virtual string ExtenderCATID
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual object ExtenderNames
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string Identity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual int MajorVersion
		{
			get
			{
				return 0;
			}
		}

		public virtual int MinorVersion
		{
			get
			{
				return 0;
			}
		}

		public virtual int BuildNumber
		{
			get
			{
				return 0;
			}
		}

		public virtual int RevisionNumber
		{
			get
			{
				return 0;
			}
		}

		public virtual string Version
		{
			get
			{
				return new Version().ToString();
			}
		}

		public virtual string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string Path
		{
			get
			{
				return BaseReferenceNode.Url;
			}
		}

		public virtual string PublicKeyToken
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual EnvDTE.Project SourceProject
		{
			get
			{
				return null;
			}
		}

		public virtual bool StrongName
		{
			get
			{
				return false;
			}
		}

		public virtual prjReferenceType Type
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual object get_Extender(string ExtenderName)
		{
			throw new NotImplementedException();
		}

		public virtual void Remove()
		{
			BaseReferenceNode.Remove(false);
		}

		#endregion
	}
}
