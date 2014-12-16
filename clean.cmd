@echo off
echo Cleaning, a moment please...

rem attrib *.suo -s -h -r

rem del /f /s /q *.suo
del /f /s /q *.user
del /f /s /q *.ncb

rd /s /q datatree_dotnet\bin
rd /s /q datatree_dotnet\obj
rd /s /q test_dotnet\bin
rd /s /q test_dotnet\obj
rd /s /q temp
rd /s /q output\debug
rd /s /q editor\obj

del /f /s /q output\*.res
del /f /s /q output\*.dep
del /f /s /q output\*.htm
del /f /s /q output\*.ilk
del /f /s /q output\*.manifest
del /f /s /q output\*.obj
del /f /s /q output\*.pdb
del /f /s /q output\*.idb
del /f /s /q output\*.vshost.exe
del /f /s /q output\Debug\lastfile
del /f /s /q output\Release\lastfile

echo Cleaning done!
