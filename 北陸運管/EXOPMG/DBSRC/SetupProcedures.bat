rem 車改サーバのインスタンス名、データベース名、ユーザ名、パスワードをセット
set inst_mei=.\EXOPMGDB
set db_mei=EXOPMG
set db_user=exopmg
set db_pass=exopmg

call CreateProcedures.bat %inst_mei% %db_mei% %db_user% %db_pass% > .\Log\CreateProcedures.log
echo off
echo Logフォルダを確認してください。
pause
