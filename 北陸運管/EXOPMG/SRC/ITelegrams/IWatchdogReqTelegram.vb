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
''' ServerTelegrapher��ClientTelegrapher���z�肷�鉼�z�E�H�b�`�h�b�OREQ�d���B
''' </summary>
Public Interface IWatchdogReqTelegram
    Inherits IReqTelegram

    'ACK�d���𐶐����郁�\�b�h
    Function CreateAckTelegram() As ITelegram
End Interface
