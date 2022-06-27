' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/06/27  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Runtime.Serialization

<DataContract> Public Class UiStateClass
    'NOTE: �@��̏�Ԃ͂����ɕۑ����Ă��悢���A�V�~�����[�^�{�̂��w�肵�Ă���
    '�p�X�̋@��ʃf�B���N�g���ɕۑ����Ă��悢�B�^�p�����G�ɂȂ�̂ŁA
    '�ǂ��炩�ɓ��ꂵ�������悢�B�����ɕۑ����Ă������������ɎQ�Ƃł���B
    <DataMember> Public Machines As Dictionary(Of String, Machine)

    '���O�\���t�B���^�̗���
    <DataMember> Public LogDispFilterHistory As List(Of String)

    Public Sub New()
        Me.Machines = New Dictionary(Of String, Machine)
        Me.LogDispFilterHistory = New List(Of String)
    End Sub
End Class

<DataContract> Public Class Machine
    '�@��\���t�@�C���̍ŏI�m�F����
    <DataMember> Public LastConfirmed As DateTime

    '�@��\���t�@�C���̃^�C���X�^���v
    <DataMember> Public ProfileTimestamp As DateTime
    <DataMember> Public TermMachinesProfileTimestamp As DateTime

    '�@��\���t�@�C���̃L���b�V��
    <DataMember> Public Profile As Object()

    '�e����
    <DataMember> Public TermMachines As Dictionary(Of String, TermMachine)

    Public Sub New()
        Me.TermMachines = New Dictionary(Of String, TermMachine)
    End Sub
End Class

<DataContract> Public Class TermMachine
    '�@��\���t�@�C���̃L���b�V��
    <DataMember> Public Profile As Object()

    '�e����
    <DataMember> Public LatchConf As Byte
    <DataMember> Public SeqNumber As UInteger
    <DataMember> Public PassDate As DateTime

    Public Sub New()
    End Sub
End Class
