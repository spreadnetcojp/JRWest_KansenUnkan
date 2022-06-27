' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp.RecDataStructure

''' <summary>
''' �o�C�i���t�@�C���̊�{�w�b�_������͂��A�o�^�f�[�^�Ƃ��ă������ɕێ�����B
''' </summary>
Public Class BinaryHeadInfoParse

#Region "���\�b�h�iPublic�j"
    ''' <summary>
    ''' ��{�w�b�_�����̉��
    ''' </summary>
    ''' <param name="baseInfo">�o�C�i���t�@�C���̊�{�w�b�_��</param>
    ''' <param name="clientKind">�N���C�A���g���</param>
    ''' <param name="infoObj">��͂������ʂ�ۑ��p</param> 
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Public Shared Function GetBaseInfo(ByVal baseInfo As Byte(), _
                                       ByVal clientKind As String, _
                                       ByRef infoObj As BaseInfo) As Boolean
        Dim bRtn As Boolean = False

        Try

            '�f�[�^���
            infoObj.DATA_KIND = Hex(baseInfo(0))

            '�w�R�[�h
            infoObj.STATION_CODE.RAIL_SECTION_CODE = baseInfo(1).ToString("D3")
            infoObj.STATION_CODE.STATION_ORDER_CODE = baseInfo(2).ToString("D3")

            '��������
            infoObj.PROCESSING_TIME = FnHexDisp(baseInfo(3)) & FnHexDisp(baseInfo(4)) & _
                                      FnHexDisp(baseInfo(5)) & FnHexDisp(baseInfo(6)) & _
                                      FnHexDisp(baseInfo(7)) & FnHexDisp(baseInfo(8)) & _
                                      FnHexDisp(baseInfo(9))

            '�R�[�i�[
            infoObj.CORNER_CODE = baseInfo(10).ToString("D4")

            '���@
            infoObj.UNIT_NO = baseInfo(11)

            '�@��
            Select Case clientKind
                Case "02"
                    Select Case infoObj.DATA_KIND
                        Case "A1", "A2", "A3", "A4", "A5", "A7", "A8", "55", "B1"
                            'G�F�i���D�@�j
                            infoObj.MODEL_CODE = EkConstants.ModelCodeGate
                        Case "54"
                            'W�F�i�Ď��Ձj
                            infoObj.MODEL_CODE = EkConstants.ModelCodeKanshiban
                        Case "A6", "C3"
                            If infoObj.UNIT_NO = 0 Then
                                'G�F�i���D�@�j
                                infoObj.MODEL_CODE = EkConstants.ModelCodeGate
                            Else
                                'W�F�i�Ď��Ձj
                                infoObj.MODEL_CODE = EkConstants.ModelCodeKanshiban
                            End If
                    End Select

                Case "06"
                    If infoObj.DATA_KIND = "89" Then
                        'Y�F�i���������@�j
                        infoObj.MODEL_CODE = EkConstants.ModelCodeMadosho
                    Else
                        'X�F�����^EX����
                        infoObj.MODEL_CODE = EkConstants.ModelCodeTokatsu
                    End If
                Case "08"
                    '�O�W��Y�F�i���������@�j
                    infoObj.MODEL_CODE = EkConstants.ModelCodeMadosho
            End Select

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        End Try

        Return bRtn

    End Function

    ''' <summary>
    ''' �P�O�i�����O���ڽ���Ȃ��P�U�i���̌`�ɕϊ�����
    ''' </summary>
    ''' <param name="bytDat10">�P�޲��ް�</param>
    ''' <returns>fnHexDisp       �P�U�i��������</returns>
    Private Shared Function FnHexDisp(ByVal bytDat10 As Byte) As String

        '�펞�Q���ŕԂ�
        If Len(Hex(bytDat10)) <= 1 Then     '�P���Ȃ��
            fnHexDisp = "0" & Hex(bytDat10)   '�O���ڽ���Ȃ�
        Else                                '�Q���Ȃ��
            fnHexDisp = Hex(bytDat10)         '���̂܂�
        End If

    End Function

#End Region

End Class
