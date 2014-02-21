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
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using Microsoft.Build.Execution;
	using Microsoft.Build.Framework;

	partial class DesignTimeAssemblyResolution
	{
		/// <summary>
		/// Accesssor for RAR related properties in the projectInstance.
		/// See ResolveAssemblyReferennce task msdn docs for member descriptions
		/// </summary>
		private class RarInputs
		{
			#region private fields

			// RAR related property/item names etc
			private const string TargetFrameworkDirectory = "TargetFrameworkDirectory";
			private const string RegistrySearchPathFormat = "Registry:{0},{1},{2}{3}";
			private const string FrameworkRegistryBase = "FrameworkRegistryBase";
			private const string TargetFrameworkVersionName = "TargetFrameworkVersion";
			private const string AssemblyFoldersSuffix = "AssemblyFoldersSuffix";
			private const string AssemblyFoldersExConditions = "AssemblyFoldersExConditions";
			private const string AllowedReferenceAssemblyFileExtensions = "AllowedReferenceAssemblyFileExtensions";
			private const string ProcessorArchitecture = "ProcessorArchitecture";
			private const string TargetFrameworkMonikerName = "TargetFrameworkMoniker";
			private const string TargetFrameworkMonikerDisplayNameName = "TargetFrameworkMonikerDisplayName";
			private const string TargetedRuntimeVersionName = "TargetedRuntimeVersion";
			private const string FullFrameworkReferenceAssemblyPaths = "_FullFrameworkReferenceAssemblyPaths";
			private const string TargetFrameworkProfile = "TargetFrameworkProfile";

			private const string ProjectDesignTimeAssemblyResolutionSearchPaths = "ProjectDesignTimeAssemblyResolutionSearchPaths";
			private const string Content = "Content";
			private const string None = "None";
			private const string RARResolvedReferencePath = "ReferencePath";
			private const string IntermediateOutputPath = "IntermediateOutputPath";
			private const string InstalledAssemblySubsetTablesName = "InstalledAssemblySubsetTables";
			private const string IgnoreInstalledAssemblySubsetTables = "IgnoreInstalledAssemblySubsetTables";
			private const string ReferenceInstalledAssemblySubsets = "_ReferenceInstalledAssemblySubsets";
			private const string FullReferenceAssemblyNames = "FullReferenceAssemblyNames";
			private const string LatestTargetFrameworkDirectoriesName = "LatestTargetFrameworkDirectories";
			private const string FullFrameworkAssemblyTablesName = "FullFrameworkAssemblyTables";
			private const string MSBuildProjectDirectory = "MSBuildProjectDirectory";

			#endregion //private fields

			public string[] TargetFrameworkDirectories { get; private set; }
			public string[] AllowedAssemblyExtensions { get; private set; }
			public string TargetProcessorArchitecture { get; private set; }
			public string TargetFrameworkVersion { get; private set; }
			public string TargetFrameworkMoniker { get; private set; }
			public string TargetFrameworkMonikerDisplayName { get; private set; }
			public string TargetedRuntimeVersion { get; private set; }
			public string[] FullFrameworkFolders { get; private set; }
			public string ProfileName { get; private set; }
			public string[] PdtarSearchPaths { get; private set; }
			public string[] CandidateAssemblyFiles { get; private set; }
			public string StateFile { get; private set; }
			public ITaskItem[] InstalledAssemblySubsetTables { get; private set; }
			public bool IgnoreDefaultInstalledAssemblySubsetTables { get; private set; }
			public string[] TargetFrameworkSubsets { get; private set; }
			public string[] FullTargetFrameworkSubsetNames { get; private set; }
			public ITaskItem[] FullFrameworkAssemblyTables { get; private set; }
			public string[] LatestTargetFrameworkDirectories { get; private set; }

			#region constructors
			public RarInputs(ProjectInstance projectInstance)
			{
				// Run through all of the entries we want to extract from the project instance before we discard it to save memory
				TargetFrameworkDirectories = GetTargetFrameworkDirectories(projectInstance);
				AllowedAssemblyExtensions = GetAllowedAssemblyExtensions(projectInstance);
				TargetProcessorArchitecture = GetTargetProcessorArchitecture(projectInstance);
				TargetFrameworkVersion = GetTargetFrameworkVersion(projectInstance);
				TargetFrameworkMoniker = GetTargetFrameworkMoniker(projectInstance);
				TargetFrameworkMonikerDisplayName = GetTargetFrameworkMonikerDisplayName(projectInstance);
				TargetedRuntimeVersion = GetTargetedRuntimeVersion(projectInstance);
				FullFrameworkFolders = GetFullFrameworkFolders(projectInstance);
				LatestTargetFrameworkDirectories = GetLatestTargetFrameworkDirectories(projectInstance);
				FullTargetFrameworkSubsetNames = GetFullTargetFrameworkSubsetNames(projectInstance);
				FullFrameworkAssemblyTables = GetFullFrameworkAssemblyTables(projectInstance);
				IgnoreDefaultInstalledAssemblySubsetTables = GetIgnoreDefaultInstalledAssemblySubsetTables(projectInstance);
				ProfileName = GetProfileName(projectInstance);

				/*               
				 * rar.CandidateAssemblyFiles = rarInputs.CandidateAssemblyFiles;
				   rar.StateFile = rarInputs.StateFile;
				   rar.InstalledAssemblySubsetTables = rarInputs.InstalledAssemblySubsetTables;
				   rar.TargetFrameworkSubsets = rarInputs.TargetFrameworkSubsets;
				 */

				// This set needs to be kept in sync with the set of project instance data that
				// is passed into Rar
				PdtarSearchPaths = GetPdtarSearchPaths(projectInstance);

				CandidateAssemblyFiles = GetCandidateAssemblyFiles(projectInstance);
				StateFile = GetStateFile(projectInstance);
				InstalledAssemblySubsetTables = GetInstalledAssemblySubsetTables(projectInstance);
				TargetFrameworkSubsets = GetTargetFrameworkSubsets(projectInstance);
			}
			#endregion // constructors

			#region public properties

			#region common properties/items

			private string[] GetTargetFrameworkDirectories(ProjectInstance projectInstance)
			{
				if (TargetFrameworkDirectories == null)
				{
					string val = projectInstance.GetPropertyValue(TargetFrameworkDirectory).Trim();

					TargetFrameworkDirectories = val.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(s => s.Trim())
						.Where(s => s.Length > 0)
						.ToArray();
				}

				return TargetFrameworkDirectories;
			}

			private static string[] GetAllowedAssemblyExtensions(ProjectInstance projectInstance)
			{
				string[] allowedAssemblyExtensions;

				string val = projectInstance.GetPropertyValue(AllowedReferenceAssemblyFileExtensions).Trim();

				allowedAssemblyExtensions = val.Split(';').Select(s => s.Trim()).ToArray();

				return allowedAssemblyExtensions;
			}

			private static string GetTargetProcessorArchitecture(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(ProcessorArchitecture).Trim();

				return val;
			}

			private static string GetTargetFrameworkVersion(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(TargetFrameworkVersionName).Trim();

				return val;
			}

			private static string GetTargetFrameworkMoniker(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(TargetFrameworkMonikerName).Trim();

				return val;
			}

			private static string GetTargetFrameworkMonikerDisplayName(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(TargetFrameworkMonikerDisplayNameName).Trim();

				return val;
			}

			private static string GetTargetedRuntimeVersion(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(TargetedRuntimeVersionName).Trim();

				return val;
			}

			private static string[] GetFullFrameworkFolders(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(FullFrameworkReferenceAssemblyPaths).Trim();

				string[] _fullFrameworkFolders = val.Split(';').Select(s => s.Trim()).ToArray();

				return _fullFrameworkFolders;
			}

			private static string[] GetLatestTargetFrameworkDirectories(ProjectInstance projectInstance)
			{
				IEnumerable<ITaskItem> taskItems = projectInstance.GetItems(LatestTargetFrameworkDirectoriesName);

				string[] latestTargetFrameworkDirectory = (taskItems.Select((Func<ITaskItem, string>)((item) => { return item.ItemSpec.Trim(); }))).ToArray();

				return latestTargetFrameworkDirectory;
			}

			private static string GetProfileName(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(TargetFrameworkProfile).Trim();

				return val;
			}
			#endregion //common properties/items

			#region project dtar specific properties/items

			private static string[] GetPdtarSearchPaths(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(ProjectDesignTimeAssemblyResolutionSearchPaths).Trim();

				string[] _pdtarSearchPaths = val.Split(';').Select(s => s.Trim()).ToArray();

				return _pdtarSearchPaths;
			}

			private static string[] GetCandidateAssemblyFiles(ProjectInstance projectInstance)
			{
				var candidateAssemblyFilesList = new List<ProjectItemInstance>();

				candidateAssemblyFilesList.AddRange(projectInstance.GetItems(Content));
				candidateAssemblyFilesList.AddRange(projectInstance.GetItems(None));
				candidateAssemblyFilesList.AddRange(projectInstance.GetItems(RARResolvedReferencePath));

				string[] candidateAssemblyFiles = candidateAssemblyFilesList.Select((Func<ProjectItemInstance, string>)((item) => { return item.GetMetadataValue("FullPath").Trim(); })).ToArray();

				return candidateAssemblyFiles;
			}

			private static string GetStateFile(ProjectInstance projectInstance)
			{
				string intermediatePath = projectInstance.GetPropertyValue(IntermediateOutputPath).Trim();

				intermediatePath = GetFullPathInProjectContext(projectInstance, intermediatePath);

				string stateFile = Path.Combine(intermediatePath, "DesignTimeResolveAssemblyReferences.cache");

				return stateFile;
			}

			private static ITaskItem[] GetInstalledAssemblySubsetTables(ProjectInstance projectInstance)
			{
				return projectInstance.GetItems(InstalledAssemblySubsetTablesName).ToArray();
			}

			private static bool GetIgnoreDefaultInstalledAssemblySubsetTables(ProjectInstance projectInstance)
			{
				bool ignoreDefaultInstalledAssemblySubsetTables = false;

				string val = projectInstance.GetPropertyValue(IgnoreInstalledAssemblySubsetTables).Trim();

				if (!String.IsNullOrEmpty(val))
				{
					if (val == Boolean.TrueString || val == Boolean.FalseString)
					{
						ignoreDefaultInstalledAssemblySubsetTables = Convert.ToBoolean(val, CultureInfo.InvariantCulture);
					}
				}

				return ignoreDefaultInstalledAssemblySubsetTables;
			}

			private static string[] GetTargetFrameworkSubsets(ProjectInstance projectInstance)
			{
				IEnumerable<ITaskItem> taskItems = projectInstance.GetItems(ReferenceInstalledAssemblySubsets);

				string[] targetFrameworkSubsets = (taskItems.Select((Func<ITaskItem, string>)((item) => { return item.ItemSpec.Trim(); }))).ToArray();

				return targetFrameworkSubsets;
			}

			private static string[] GetFullTargetFrameworkSubsetNames(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(FullReferenceAssemblyNames).Trim();

				string[] fullTargetFrameworkSubsetNames = val.Split(';').Select(s => s.Trim()).ToArray();

				return fullTargetFrameworkSubsetNames;
			}

			private static ITaskItem[] GetFullFrameworkAssemblyTables(ProjectInstance projectInstance)
			{
				return projectInstance.GetItems(FullFrameworkAssemblyTablesName).ToArray();
			}

			#endregion //project dtar specific properties/items

			#endregion // public properties

			#region private methods
			static string GetFullPathInProjectContext(ProjectInstance projectInstance, string path)
			{
				string fullPath = path;

				if (!Path.IsPathRooted(path))
				{
					string projectDir = projectInstance.GetPropertyValue(MSBuildProjectDirectory).Trim();

					fullPath = Path.Combine(projectDir, path);

					fullPath = Path.GetFullPath(fullPath);
				}

				return fullPath;
			}
			#endregion // private methods
		}
	}
}
