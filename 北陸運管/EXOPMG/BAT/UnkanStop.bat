taskkill /fi "imagename eq ExOpmgServerAppManager.exe"
timeout /T 60
taskkill /F /T /fi "imagename eq ExOpmgServerAppManager.exe"
