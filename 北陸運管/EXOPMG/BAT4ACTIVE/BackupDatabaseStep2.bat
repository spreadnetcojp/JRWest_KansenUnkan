@echo off
set dd=%date:~8,2%
set logfile=D:\EXOPMG\LOG\XXXXXX%dd%-BackupDatabaseStep2.log
@echo on
echo %date% %time% ˆ³kŠJŽn> %logfile%
echo "a" | C:\EXOPMG\BIN\tsbcab -a X:\EXOPMG\BAT4ACTIVE\archives\DBBackup.cab X:\EXOPMG\BAT4ACTIVE\DATA\*.dat >> %logfile%
echo %date% %time% ˆ³kI—¹>> %logfile%
