' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/01/14  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' �e�X���b�h����MyTelegrapher�ւ̃V�i���I�J�n�v�����b�Z�[�W�B
''' </summary>
Public Structure ScenarioStartRequest
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property ExtendPart() As ScenarioStartRequestExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, ScenarioStartRequestExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal extend As ScenarioStartRequestExtendPart) As InternalMessage
        Return New InternalMessage(MyInternalMessageKind.ScenarioStartRequest, extend)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As ScenarioStartRequest
        Debug.Assert(msg.Kind = MyInternalMessageKind.ScenarioStartRequest)

        Dim ret As ScenarioStartRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class ScenarioStartRequestExtendPart
    '�J�n�����w��L��
    Public StartTimeSpecified As Boolean

    '�J�n����
    Public StartTime As DateTime

    '�V�i���I�t�@�C���̃p�X
    Public ScenarioFilePath As String
End Class
