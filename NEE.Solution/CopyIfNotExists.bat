@echo off
echo Copying file if not exist: %1 to %2

if not exist %1 goto File1NotFound
if not exist %2 goto File2NotFound

:NoCopy
echo Target file exists.  Did nothing
goto END

:File1NotFound
echo %1 not found.
goto END

:File2NotFound
copy %1 %2 /y
goto END

:END
echo Done.