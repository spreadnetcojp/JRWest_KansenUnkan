' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

''' <summary>
''' Type5�`8�̃V�[�P���X��ServerTelegrapher��ClientTelegrapher��
''' �z�肷�鉼�z�d���B
''' </summary>
Public Interface IXllTelegram
    Inherits ITelegram

    'NOTE: Ull�p�d���̃N���X�ɂ�����ContinueCode�v���p�e�B��
    'ContinueCode.FinishWithoutStorin��ԋp����̂͋֎~�Ƃ���B
    '���������A���ۂ�Ull�p�d���̎d�l�ŁA���̂悤�Ȓl��
    '��`����Ă��邱�Ƃ͂Ȃ��͂��ł���B
    '��M�����d����ContinueCode�������ڂɁuDll�p�d���̎d�l��
    'ContinueCode.FinishWithoutStoring�����̒l�v�����R�i�[�����
    '����ꍇ�AContinueCode�v���p�e�B�́AContinueCode.None��
    '�ԋp����ׂ��ł���B
    ReadOnly Property ContinueCode() As ContinueCode
End Interface

'���z�u�J�n�E�I���v�l
Public Enum ContinueCode As Integer
    None
    Start                   '�]���J�n
    Finish                  '�]������I��
    FinishWithoutStoring    '�]������I�����ۑ�����
    Abort                   '�]���ُ�I��
End Enum
