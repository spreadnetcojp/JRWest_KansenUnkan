' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2013/06/18  (NES)����  BaseInfo�ɃR���X�g���N�^��ǉ�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' �e���ʏ����̍\����
''' 
''' </summary>
''' <remarks></remarks>
Public Class RecDataStructure

#Region "�錾�̈�iPublic�j"

    ''' <summary>
    ''' ��`���
    ''' </summary>
    Public Structure DefineInfo
        Dim KOMOKU_NAME As String                   '���ږ���
        Dim COMMENT As String                       '�R�}���h
        Dim BYTE_OFFSET As Integer                  '�o�C�g�I�t�Z�b�g
        Dim BYTE_LEN As Integer                     '�o�C�g�����O�X
        Dim BIT_OFFSET As Integer                   '�r�b�g�I�t�Z�b�g
        Dim BIT_LEN As Integer                      '�r�b�g�����O�X
        Dim DATA_FORMAT As String                   '�f�[�^�`��
        Dim FIELD_NAME As String                    '�t�B�[���h��
        Dim FIELD_FORMAT As String                  '�t�B�[���h�`��
        Dim PARA1 As Boolean                        '��L�[���ۂ�
        Dim PARA2 As Boolean                        'NULL���e���ۂ�
        Dim PARA3 As String                         '�p�����[�^�[
        Dim PARA4 As String                         '�p�����[�^�[
        Dim PARA5 As String                         '�p�����[�^�[
        Dim PARA6 As String                         '�p�����[�^�[
    End Structure

    ''' <summary>
    ''' ��{�w�b�_�����
    ''' </summary>
    Public Structure BaseInfo
        Dim DATA_KIND As String                 '�f�[�^��ʁi1�`2����16�i�����j
        Dim STATION_CODE As Station             '�w�R�[�h
        Dim PROCESSING_TIME As String           '���������i�����ɂ��yyyyMMddHHmmss�`���܂���DateTime���J���`���ˑ���ToString�����`���jTODO: �v���t�@�N�^�����O
        Dim CORNER_CODE As String               '�R�[�i�[�i4���̐����j
        Dim UNIT_NO As Integer                  '���@�i1�`2���̐����j
        Dim MODEL_CODE As String                '�@��

        Public Sub New(ByVal causeModel As String, ByVal causeUnit As EkCode, ByVal time As DateTime)
            Me.MODEL_CODE = causeModel
            Me.STATION_CODE.RAIL_SECTION_CODE = causeUnit.RailSection.ToString("D3")
            Me.STATION_CODE.STATION_ORDER_CODE = causeUnit.StationOrder.ToString("D3")
            Me.CORNER_CODE = causeUnit.Corner.ToString("D4")
            Me.UNIT_NO = causeUnit.Unit
            Me.PROCESSING_TIME = time.ToString("yyyyMMddHHmmss")
        End Sub

        Public Sub New(ByVal causeModel As String, ByVal causeUnit As EkCode)
            Me.New(causeModel, causeUnit, DateTime.Now)
        End Sub
    End Structure

    ''' <summary>
    ''' �w�R�[�h
    ''' </summary>
    Public Structure Station
        Dim RAIL_SECTION_CODE As String        '����i3���̐����j
        Dim STATION_ORDER_CODE As String       '�w���i3���̐����j
    End Structure

#End Region

End Class
