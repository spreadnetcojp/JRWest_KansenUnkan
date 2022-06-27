' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2013/06/18  (NES)����  �b���ɔ�������ʓ��e�ُ̈�ɑΉ��A
'                                   ���[�X�R���f�B�V��������
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp.RecDataStructure

''' <summary>
''' ���W�f�[�^�Ɍ��o�����ُ��o�^���邽�߂̃N���X�B
''' </summary>
Public Class CollectedDataTypoRecorder

#Region "�萔��ϐ�"
    Private Const UserId As String = "System"
    Private Const MachineId As String = "Server"
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' ���W�f�[�^��茟�o�����ُ��DB�ɓo�^����B
    ''' </summary>
    ''' <param name="infoObj">�w���@�킩���M����f�[�^�̊�{���</param>
    ''' <param name="dataKind">�f�[�^���</param> 
    ''' <param name="errInfo">�ُ���e</param> 
    ''' <returns>True:�����AFalse:���s</returns>
    Public Shared Function Record(ByVal infoObj As BaseInfo, ByVal dataKind As String, ByVal errInfo As String) As Boolean
        'NOTE: infoObj.STATION_CODE.RAIL_SECTION_CODE��
        'infoObj.STATION_CODE.STATION_ORDER_CODE�̌����ɂ��ẮA
        '�����̑Ó������`�F�b�N�����O�ɁA���̍��ڂ̕s���ɂ����
        '�{���\�b�h���Ăяo����邱�Ƃ�z�肵�A�����Ń[���p�f�B���O��
        '�s���i�^�ǒ[������݂��Ȃ��Ȃ�\�����ɗ͌��炷�j�B
        '�R�[�i�[�R�[�h���������łȂ��ꍇ�́A�{���\�b�h�͌Ăяo����Ȃ�
        '�͂��ł��邪�A�Ăяo���ꂽ�Ƃ��Ă����O���o�͂��邱�ƂőΉ�����B
        Dim sSQL As String = _
           "MERGE INTO D_COLLECTED_DATA_TYPO AS Target" _
           & " USING (SELECT '" & infoObj.STATION_CODE.RAIL_SECTION_CODE.PadLeft(3, "0"c) & "' RAIL_SECTION_CODE," _
                         & " '" & infoObj.STATION_CODE.STATION_ORDER_CODE.PadLeft(3, "0"c) & "' STATION_ORDER_CODE," _
                         & " " & infoObj.CORNER_CODE & " CORNER_CODE," _
                         & " '" & infoObj.MODEL_CODE & "' MODEL_CODE," _
                         & " " & infoObj.UNIT_NO & " UNIT_NO," _
                         & " '" & dataKind & "' DATA_KIND," _
                         & " '" & infoObj.PROCESSING_TIME & "' PROCESSING_TIME," _
                         & " '" & errInfo & "' ERROR_INFO) AS Source" _
           & " ON (Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
            & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
            & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
            & " AND Target.MODEL_CODE = Source.MODEL_CODE" _
            & " AND Target.UNIT_NO = Source.UNIT_NO" _
            & " AND Target.DATA_KIND = Source.DATA_KIND" _
            & " AND Target.PROCESSING_TIME = Source.PROCESSING_TIME" _
            & " AND Target.ERROR_INFO = Source.ERROR_INFO)" _
           & " WHEN MATCHED THEN" _
            & " UPDATE" _
             & " SET Target.UPDATE_DATE = GETDATE()," _
                 & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                 & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'" _
           & " WHEN NOT MATCHED THEN" _
            & " INSERT (INSERT_DATE," _
                    & " INSERT_USER_ID," _
                    & " INSERT_MACHINE_ID," _
                    & " UPDATE_DATE," _
                    & " UPDATE_USER_ID," _
                    & " UPDATE_MACHINE_ID," _
                    & " RAIL_SECTION_CODE," _
                    & " STATION_ORDER_CODE," _
                    & " CORNER_CODE," _
                    & " MODEL_CODE," _
                    & " UNIT_NO," _
                    & " DATA_KIND," _
                    & " PROCESSING_TIME," _
                    & " ERROR_INFO)" _
            & " VALUES (GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " Source.RAIL_SECTION_CODE," _
                    & " Source.STATION_ORDER_CODE," _
                    & " Source.CORNER_CODE," _
                    & " Source.MODEL_CODE," _
                    & " Source.UNIT_NO," _
                    & " Source.DATA_KIND," _
                    & " Source.PROCESSING_TIME," _
                    & " Source.ERROR_INFO);"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()
            Return True

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            dbCtl.TransactionRollBack()
            Return False

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function
#End Region

End Class
