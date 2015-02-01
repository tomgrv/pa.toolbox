@ECHO OFF

ECHO Running %0 in %CD%
ECHO Listing project files

FOR /F "tokens=*" %%D IN ('DIR /S /B "PA.*.nupkg"') DO (

	copy /Y "%%D" "./publish/" 
	
)


