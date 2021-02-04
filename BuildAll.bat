pushd NCDK
call :BuildProject NCDK
if errorlevel 1 goto :ERROREND
popd
pushd NCDK.Math
dotnet add package MathNet.Numerics.Signed
dotnet restore
call :BuildProject NCDK.Math
if errorlevel 1 goto :ERROREND
popd
call :BuildNeGet NCDK
if errorlevel 1 goto :ERROREND
pushd NCDKTests
call :BuildProject NCDKTests
if errorlevel 1 goto :ERROREND
popd

pushd NCDK.RDF
dotnet add package dotNetRDF
dotnet restore
call :BuildProject NCDK.RDF
if errorlevel 1 goto :ERROREND
popd
pushd NCDK.RDFTests
call :BuildProject NCDK.RDFTests
if errorlevel 1 goto :ERROREND
popd

pushd NCDK.Legacy
call :BuildProject NCDK.Legacy
if errorlevel 1 goto :ERROREND
popd
pushd NCDK.LegacyTests
call :BuildProject NCDK.LegacyTests
if errorlevel 1 goto :ERROREND
popd

pushd NCDK.Display
call :BuildProject NCDK.Display
if errorlevel 1 goto :ERROREND
popd
call :BuildNeGet NCDK.Display
if errorlevel 1 goto :ERROREND
pushd NCDK.DisplayTests
call :BuildProject NCDK.DisplayTests
if errorlevel 1 goto :ERROREND
popd

goto :END

:BuildProject
set ProjectName=%1
if "%ProjectName%" == "" exit 1
MSBuild "%ProjectName%.csproj" /t:Build /p:Configuration=Release
if errorlevel 1 exit /b 1
set ProjectName=
exit /b

:BuildNeGet
set NuGetOption=%1
if "%NuGetOption%" == "" goto SkipNuGet
nuget pack "%NuGetOption%.nuspec" -Prop Configuration=Release -IncludeReferencedProjects
if errorlevel 1 exit /b 1
if "%MyNuGetDir%" neq "" (
	xcopy /D /Y "*.nupkg" "%MyNuGetDir%"
	del /Q "*.nupkg"
)
set ProjectName=
exit /b

:ERROREND

exit /b 1

:END
