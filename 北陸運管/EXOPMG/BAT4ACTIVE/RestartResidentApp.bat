taskkill /fi "imagename eq ExOpmgServerAppManager.exe"
timeout /T 45 /NOBREAK
taskkill /F /T /fi "imagename eq ExOpmgServerAppManager.exe"
timeout /T 15 /NOBREAK
start C:\EXOPMG\BIN\ExOpmgServerAppManager.exe
