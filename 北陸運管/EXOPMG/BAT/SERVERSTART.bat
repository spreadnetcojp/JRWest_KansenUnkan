SCHTASKS /DELETE /TN SERVERSTART /F
rem ----- タスクスケジューラ再登録。
rem １）係員権限更新
SCHTASKS /CREATE /RU exopmg /RP exopmg /TN SERVERSTART /XML "C:\EXOPMG\BAT\SERVERSTART.xml" /IT 
