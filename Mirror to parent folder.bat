REM Copy newer files to another folder ready to copy
REM Excludes all OBJ and BIN folders
robocopy.exe . ..\TakeExtractor /MIR /XO /XD obj bin .svn *.user *.suo *.cachefile
