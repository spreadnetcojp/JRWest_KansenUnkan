@ECHO OFF

SET TARGET_EXE=ExOpmgServerAppConStatusMailer.exe ExOpmgServerAppAlertMailer.exe ExOpmgServerAppForBesshuData.exe ExOpmgServerAppForConStatus.exe ExOpmgServerAppForFaultData.exe ExOpmgServerAppForKadoData.exe ExOpmgServerAppForKsbConfig.exe ExOpmgServerAppForMeisaiData.exe ExOpmgServerAppForTrafficData.exe ExOpmgServerAppManager.exe ExOpmgServerAppScheduler.exe ExOpmgServerAppToKanshiban.exe ExOpmgServerAppToKanshiban2.exe ExOpmgServerAppToMadosho.exe ExOpmgServerAppToMadosho2.exe ExOpmgServerAppToNkan.exe ExOpmgServerAppToOpClient.exe ExOpmgServerAppToTokatsu.exe ExOpmgServerAppSweeper.exe ExOpmgServerAppForRiyoData.exe

SET TARGET_DLL=ExOpmgCommon.dll ExOpmgDBCommon.dll ExOpmgEkCommon.dll ExOpmgEkTelegrams.dll ExOpmgInternalMessages.dll ExOpmgITelegrams.dll ExOpmgNkTelegrams.dll ExOpmgServerAppCommon.dll ExOpmgServerAppExternalMessages.dll ExOpmgServerAppForAnyUpboundData.dll ExOpmgServerAppInternalMessages.dll ExOpmgServerAppToAnyEkimuModel.dll ExOpmgServerTelegrapher.dll

ECHO BIN ディレクトリを初期化し、OBJ\Release に存在する実行ファイルをコピーします.
ECHO よろしいですか? (Y/N)
SET /p c=
if "%c%"=="Y" GOTO CONTINUE
if "%c%"=="y" GOTO CONTINUE
EXIT

:CONTINUE
ECHO ON
del .\BIN\ExOpmg*.exe
del .\BIN\ExOpmg*.dll
@FOR %%a IN (%TARGET_EXE%) DO copy .\OBJ\Release\%%a .\BIN
@FOR %%a IN (%TARGET_DLL%) DO copy .\OBJ\Release\%%a .\BIN
@ECHO fin
@pause