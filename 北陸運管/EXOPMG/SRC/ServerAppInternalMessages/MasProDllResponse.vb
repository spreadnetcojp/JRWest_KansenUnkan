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

Imports JR.ExOpmg.Common

''' <summary>
''' Server�nTelegrapher����e�X���b�h�ւ̃}�X�^/�v���O����DLL�������b�Z�[�W�B
''' </summary>
''' <remarks>
''' �^�ǃT�[�o��DLL�����ɂ����āADLL���ʂ�DB�ւ̏������݂�
''' �e�X���b�h�ł͂Ȃ�Telegrapher���X���b�h�̐Ӗ��ł���B
''' ����āA�{���b�Z�[�W�̖����́ADLL�̌��ʂ�e�X���b�h�ɓ`���邱��
''' �ł͂Ȃ��A�v�����ꂽDLL�V�[�P���X�ɋN������t�@�C���]����
''' �i���炭�j����ȏ㔭�����Ȃ����Ƃ�A���̗v�����Ȉ˗���
''' �����\�ɂȂ������Ƃ�e�X���b�h�ɓ`���邱�Ƃł���B
''' </remarks>
Public Structure MasProDllResponse
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen() As InternalMessage
        Return New InternalMessage(ServerAppInternalMessageKind.MasProDllResponse)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As MasProDllResponse
        Debug.Assert(msg.Kind = ServerAppInternalMessageKind.MasProDllResponse)

        Dim ret As MasProDllResponse
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure
