rem �ԉ��T�[�o�̃C���X�^���X���A�f�[�^�x�[�X���A���[�U���A�p�X���[�h���Z�b�g
set inst_mei=.\EXOPMGDB
set db_mei=EXOPMG
set db_user=exopmg
set db_pass=exopmg

call CreateProcedures.bat %inst_mei% %db_mei% %db_user% %db_pass% > .\Log\CreateProcedures.log
echo off
echo Log�t�H���_���m�F���Ă��������B
pause
