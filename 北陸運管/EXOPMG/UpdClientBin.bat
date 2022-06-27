@ECHO OFF

SET TARGET_EXE=ExOpmgClientApp.exe

SET TARGET_DLL=ExOpmgCommon.dll ExOpmgDBCommon.dll ExOpmgEkCommon.dll ExOpmgEkTelegrams.dll ExOpmgInternalMessages.dll ExOpmgITelegrams.dll ExOpmgClientDao.dll ExOpmgClientAppInternalMessages.dll ExOpmgClientTelegrapher.dll

ECHO BIN ディレクトリを初期化し、OBJ\Release に存在する実行ファイルをコピーします.
ECHO よろしいですか? (Y/N)
SET /p c=
if "%c%"=="Y" GOTO CONTINUE
if "%c%"=="y" GOTO CONTINUE
EXIT

:CONTINUE
ECHO ON
del .\BIN\ExOpmgClient*.exe
del .\BIN\ExOpmgClient*.dll
@FOR %%a IN (%TARGET_EXE%) DO copy .\OBJ\Release\%%a .\BIN
@FOR %%a IN (%TARGET_DLL%) DO copy .\OBJ\Release\%%a .\BIN
@ECHO fin
@pause