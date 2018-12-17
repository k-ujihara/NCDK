pushd NCDK
dotnet add package MathNet.Numerics.Signed
dotnet add package dotNetRDF
dotnet restore

call :BuildProject NCDK yes
popd

pushd NCDK.Display
call :BuildProject NCDK.Display yes
popd

pushd NCDK.Tests
call :BuildProject NCDKTests
popd

pushd NCDK.DisplayTests
call :BuildProject NCDK.DisplayTests
popd

goto END

:BuildProject

set ProjectName=%1
set NuGetOption=%2
if "%ProjectName%" == "" exit 1
if "%NuGetOption%" == "yes" set NuGetOption=%1
MSBuild "%ProjectName%.csproj" /t:Build /p:Configuration=Release

if "%NuGetOption%" == "" goto SkipNuGet
nuget pack "%NuGetOption%.nuspec" -Prop Configuration=Release -IncludeReferencedProjects
if "%MyNuGetDir%" neq "" (
	xcopy /D /Y "*.nupkg" "%MyNuGetDir%"
	del /Q "*.nupkg"
)
:SkipNuGet

set ProjectName=
set NuGetOption=

exit /b

:END
