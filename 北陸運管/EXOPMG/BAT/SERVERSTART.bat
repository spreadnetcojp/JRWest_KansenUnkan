SCHTASKS /DELETE /TN SERVERSTART /F
rem ----- �^�X�N�X�P�W���[���ēo�^�B
rem �P�j�W�������X�V
SCHTASKS /CREATE /RU exopmg /RP exopmg /TN SERVERSTART /XML "C:\EXOPMG\BAT\SERVERSTART.xml" /IT 
