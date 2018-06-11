pushd NCDK
call :BuildProject NCDK
popd

pushd NCDKDisplay
call :BuildProject NCDK.Display
popd

goto END

:BuildProject

set ProjectName=%1
if "%ProjectName%" == "" exit 1
MSBuild "%ProjectName%.csproj" /t:Build /p:Configuration=Release
nuget pack "%ProjectName%.csproj" -Prop Configuration=Release -IncludeReferencedProjects
if "%MyNuGetDir%" neq "" (
	xcopy /D /Y "*.nupkg" "%MyNuGetDir%"
	del /Q "*.nupkg"
)
set ProjectName=

exit /b

:END
