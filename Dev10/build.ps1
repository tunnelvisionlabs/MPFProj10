# build the solutions
$SolutionPath = ".\Microsoft.VisualStudio.Project.sln"

# make sure the script was run from the expected path
if (!(Test-Path $SolutionPath)) {
	echo "The script was run from an invalid working directory."
	exit 1
}

$BuildConfig = "Release"
$DebugBuild = false
$Version = "1.0.0-alpha001"

# build the main project
$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"

&$msbuild /nologo /m /nr:false /t:rebuild /p:Configuration=$BuildConfig $SolutionPath
if ($LASTEXITCODE -ne 0) {
	echo "Build failed, aborting!"
	exit $p.ExitCode
}

if (-not (Test-Path 'nuget\release')) {
	mkdir 'nuget\release'
}

.\nuget\NuGet.exe pack .\Src\Microsoft.VisualStudio.Project.10.0.nuspec -NoDefaultExcludes -OutputDirectory nuget\release -Prop Configuration=$BuildConfig -Version $Version -Symbols
.\nuget\NuGet.exe pack .\Src\Microsoft.VisualStudio.Project.11.0.nuspec -NoDefaultExcludes -OutputDirectory nuget\release -Prop Configuration=$BuildConfig -Version $Version -Symbols
