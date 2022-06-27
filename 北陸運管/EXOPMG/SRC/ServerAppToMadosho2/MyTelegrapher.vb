' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net.Sockets

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

'-------Ver0.1 ������ԕ�Ή� MOD START-----------
''' <summary>
''' �����Ɨ��p�f�[�^�p�R�l�N�V�����œd���̑���M���s���N���X�B
''' </summary>
Public Class MyTelegrapher
    Inherits TelServerAppTelegrapher

#Region "�R���X�g���N�^"
    '-------Ver0.1 ������ԕ�Ή� MOD START-----------
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal oTelegImporter As EkTelegramImporter, _
       ByVal oTelegGene As EkTelegramGene, _
       ByVal clientCode As EkCode, _
       ByVal sClientModel As String, _
       ByVal sPortPurpose As String, _
       ByVal sCdtClientModelName As String, _
       ByVal sCdtPortName As String, _
       ByVal sClientStationName As String, _
       ByVal sClientCornerName As String)

        MyBase.New( _
          sThreadName, _
          oParentMessageSock, _
          oTelegImporter, _
          oTelegGene, _
          clientCode, _
          sClientModel, _
          sPortPurpose, _
          sCdtClientModelName, _
          sCdtPortName, _
          sClientStationName, _
          sClientCornerName, _
          Lexis.Madosho2LineErrorAlertMailSubject, _
          Lexis.Madosho2LineErrorAlertMailBody)
        Me.formalObjCodeOfWatchdog = EkWatchdogReqTelegram.FormalObjCodeInMadosho
        Me.oRiyoDataUllSpecOfObjCodes = Config.RiyoDataUllSpecOfObjCodes

        '�A�N�Z�X����\��̃f�B���N�g���ɂ��āA������΍쐬���Ă����B
        'NOTE: ���N���X���쐬������̂�A�K���T�u�f�B���N�g���̍쐬����
        '�s�����ƂɂȂ���̂ɂ��ẮA�ΏۊO�Ƃ���B
        Directory.CreateDirectory(sRiyoDataInputDirPath)
        Directory.CreateDirectory(sRiyoDataRejectDirPath)
    End Sub
    '-------Ver0.1 ������ԕ�Ή� MOD END-------------
#End Region

End Class
'-------Ver0.1 ������ԕ�Ή� MOD END-------------
