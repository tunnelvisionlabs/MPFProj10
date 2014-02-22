$Version = "1.0.0-alpha001"

.\nuget\NuGet.exe push .\nuget\release\Microsoft.VisualStudio.Project.10.0.nupkg

## For now, the Visual Studio 2012 project doesn't include any additional features,
## so there's no reason for users to choose it. Uploading it at this point would
## simply be misleading.
# .\nuget\NuGet.exe push .\nuget\release\Microsoft.VisualStudio.Project.11.0.nupkg
