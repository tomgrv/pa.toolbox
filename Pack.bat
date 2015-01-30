@echo off
echo Running %0 in %CD%

rem 1=outdir 2=subdir 3=isscomponent


echo Listing project files
 FOR /F %%D IN ('dir /B "."') DO (
   if exist "%%D\%%D.nuspec" (
   echo Pack %%D
   .\.nuget\nuget.exe pack "%%D\%%D.csproj" -Symbols -BasePath "%%D" -IncludeReferencedProjects -Build -Properties "Configuration=Debug;Platform=AnyCPU" -OutputDirectory .
   )
)

