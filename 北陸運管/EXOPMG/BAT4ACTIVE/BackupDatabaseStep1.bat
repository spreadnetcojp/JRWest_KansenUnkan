@echo off
set dd=%date:~8,2%
set logfile=D:\EXOPMG\LOG\XXXXXX%dd%-BackupDatabaseStep1.log
@echo on
echo %date% %time% エクスポート開始> %logfile%
set inst_mei=.\EXOPMGDB
set db_mei=EXOPMG
set db_user=exopmg
set db_pass=exopmg
X:
cd \EXOPMG\BAT4ACTIVE
call TABLE_GET.bat %inst_mei% %db_mei% %db_user% %db_pass% >> %logfile%
call CREATE_TABLEFMT.bat %inst_mei% %db_mei% %db_user% %db_pass% >> %logfile%
call DATA_Export.bat %inst_mei% %db_mei% %db_user% %db_pass% >> %logfile%
call DB_BackUp.bat >> %logfile%
echo %date% %time% エクスポート終了>> %logfile%

