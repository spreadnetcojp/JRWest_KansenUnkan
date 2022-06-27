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

Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' �T�[�o�Ƃ��ēd���̑���M���s���N���X�B
''' </summary>
Public Class ServerTelegrapher
    Inherits Looper

#Region "�����N���X��"
    '�\���I�d�������Ɋւ����Ԃ̒�`
    Protected Enum State As Integer
        NoConnection
        Idling
        WaitingForReply
    End Enum

    '�\���I�P���V�[�P���X�̒�`
    Protected Class ActiveOne
        '���Y�V�[�P���X��REQ�d��
        Public ReqTeleg As IReqTelegram

        '�y�xNAK�d����M����REQ�d���đ��M�܂ł̃C���^�[�o������邽�߂̃^�C�}
        Public RetryTimer As TickTimer

        '���݂̎��s��
        'NOTE: ��x�ł����s�������ۂ��i�V�[�P���X�����s�����ۂ��j��m�邽��
        '�����ɑ��݂��Ă���B
        Public CurTryCount As Integer

        'NakRequirement.ForgetOnRetryOver��NAK�d���̍ő��M��
        'NOTE: REQ�d���ɑ΂���NakRequirement.ForgetOnRetryOver��NAK�d����
        '��M�����̉񐔂������ꍇ�ARetryOverToForget�ŏI������B
        Public LimitNakCountToForget As Integer

        'NakRequirement.CareOnRetryOver��NAK�d���̎�M��
        Public CurNakCountToForget As Integer

        'NakRequirement.CareOnRetryOver��NAK�d���̍ő��M��
        'NOTE: REQ�d���ɑ΂���NakRequirement.CareOnRetryOver��NAK�d����
        '��M�����̉񐔌p�������ꍇ�ARetryOverToCare�ŏI������B
        Public LimitNakCountToCare As Integer

        'NakRequirement.CareOnRetryOver��NAK�d���̎�M��
        Public CurNakCountToCare As Integer

        '�V�[�P���X���i���O�o�݂͂̂Ɏg�p�j
        Public SeqName As String

        '�R���X�g���N�^
        Public Sub New( _
           ByVal oReqTeleg As IReqTelegram, _
           ByVal retryIntervalTicks As Integer, _
           ByVal limitNakCountToForget As Integer, _
           ByVal limitNakCountToCare As Integer, _
           ByVal sSeqName As String)
            Me.ReqTeleg = oReqTeleg
            Me.RetryTimer = New TickTimer(retryIntervalTicks)
            Me.CurTryCount = 0
            Me.LimitNakCountToForget = limitNakCountToForget
            Me.CurNakCountToForget = 0
            Me.LimitNakCountToCare = limitNakCountToCare
            Me.CurNakCountToCare = 0
            Me.SeqName = sSeqName
        End Sub
    End Class

    '�t�@�C���]���V�[�P���X�̓]������
    'NOTE: ���̃N���X���ł́AActive��Passive�̈Ⴂ�͏d�v�ł��邪�A
    '���ꂪ�����Ȃ�ADll��Ull������Ă��A����͖w�Ǔ���ł���B
    '�������ADll��Ull�Ƃł́A�]�����t�@�C������̃n�b�V���l������
    '�]����t�@�C������̃n�b�V���l�����i�y�ђʒm���ꂽ�n�b�V���l
    '�Ƃ̔�r�j�Ɋւ��āA�T�[�o���ƃN���C�A���g���̂ǂ��炪�ǂ���
    '���s�������قȂ�̂Œ��ӂ��邱�ƁB�V�[�P���X�������ɌĂяo��
    '�t�b�N���\�b�h�ɂ��Ă��A�ړI���قȂ�iDll�ł͔z�M��Ԃ�
    '�F�����X�V���邽�߂̃��\�b�h�ł���AUll�ł͎�M�t�@�C����
    '�ۑ����s�����߂̃��\�b�h�ł���j���߁A�Ăяo���ׂ��^�C�~���O
    '�������ɈقȂ�̂ŁA���ӂ��Ȃ���΂Ȃ�Ȃ��B
    Protected Enum XllDirection As Integer
        Dll
        Ull
    End Enum

    '�\���I�t�@�C���]���V�[�P���X�̒�`
    Protected Class ActiveXll
        '�]������
        Public Direction As XllDirection

        '���Y�t�@�C���]���V�[�P���X�̍ŐV��REQ�d��
        'NOTE: �쐬���_����j�����_�܂œ]���J�nREQ�d���ł���B
        '�Ȃ��AULL�ɂ����ẮAREQ�d���̃o�C�g���HashValue�������݂��Ȃ����A�]���J�n��ACK�d����
        '��M�������_�ŁA�����Ɋi�[���ꂽ�n�b�V���l�����̃I�u�W�F�N�g�̐�p�����o�Ɋi�[����B
        '���R�́A�]���I����REQ�d������M�����ۂɁA���̃I�u�W�F�N�g���ŁA�t�@�C������Z�o�����
        '�n�b�V���l�Ɣ�r���邽�߂ł���B�n�b�V���l�̐�����REQ�d���̃I�u�W�F�N�g���ōs���̂́A
        '�v���g�R���d�l�Ɉˑ������n�b�V���l�̏������B�����Ȃ���΂Ȃ炸�A�t�@�C������ACK�d��
        '�ł͂Ȃ�REQ�d���Ɋi�[����Ă���ȏ�A�K��H���ł���ƌ�����B
        '���l�̕K�R���́AClientTelegrapher�ɂ����Ă����݂���B
        Public ReqTeleg As IXllReqTelegram

        '�y�xNAK�d����M����]���J�nREQ�d���đ��M�܂ł̃C���^�[�o��
        Public RetryIntervalTicks As Integer

        '���݂̓]���J�n���s��
        'NOTE: ��x�ł����s�������ۂ��i�V�[�P���X�����s�����ۂ��j��m�邽��
        '�����ɑ��݂��Ă���B
        Public CurTryCount As Integer

        'NakRequirement.ForgetOnRetryOver��NAK�d���̍ő��M��
        'NOTE: REQ�d���ɑ΂���NakRequirement.ForgetOnRetryOver��NAK�d����
        '��M�����̉񐔂������ꍇ�ARetryOverToForget�ŏI������B
        Public LimitNakCountToForget As Integer

        'NakRequirement.CareOnRetryOver��NAK�d���̎�M��
        Public CurNakCountToForget As Integer

        'NakRequirement.CareOnRetryOver��NAK�d���̍ő��M��
        'NOTE: REQ�d���ɑ΂���NakRequirement.CareOnRetryOver��NAK�d����
        '��M�����̉񐔌p�������ꍇ�ARetryOverToCare�ŏI������B
        Public LimitNakCountToCare As Integer

        'NakRequirement.CareOnRetryOver��NAK�d���̎�M��
        Public CurNakCountToCare As Integer

        '�R���X�g���N�^
        Public Sub New( _
           ByVal direction As XllDirection, _
           ByVal oXllReqTeleg As IXllReqTelegram, _
           ByVal retryIntervalTicks As Integer, _
           ByVal limitNakCountToForget As Integer, _
           ByVal limitNakCountToCare As Integer)
            Me.Direction = direction
            Me.ReqTeleg = oXllReqTeleg
            Me.RetryIntervalTicks = retryIntervalTicks
            Me.CurTryCount = 0
            Me.LimitNakCountToForget = limitNakCountToForget
            Me.CurNakCountToForget = 0
            Me.LimitNakCountToCare = limitNakCountToCare
            Me.CurNakCountToCare = 0
        End Sub
    End Class

    '�󓮓I�t�@�C���]���V�[�P���X�̒�`
    Protected Class PassiveXll
        '�]������
        Public Direction As XllDirection

        '���Y�t�@�C���]���V�[�P���X�̍ŐVREQ�d��
        'NOTE: �쐬���_����]���I����REQ�d���ł���B
        '�������AHashValue���ɂ��ẮA��L�̃^�C�~���O�Ƃ͕ʂɁADLL��
        '�N���C�A���g����n�b�V���l����M�����ۂɂ��㏑������B
        Public ReqTeleg As IXllReqTelegram

        '�R���X�g���N�^
        Public Sub New( _
           ByVal direction As XllDirection, _
           ByVal oXllReqTeleg As IXllReqTelegram)
            Me.Direction = direction
            Me.ReqTeleg = oXllReqTeleg
        End Sub
    End Class

    '�\���I�t�@�C���]���V�[�P���X�Ɋւ����Ԃ̒�`
    Protected Enum ActiveXllState As Integer
        None       '���s�O�E���s���̔\���I�t�@�C���]���V�[�P���X�Ȃ�
        BeforeFtp  '�]���J�n��ACK�d����M�O
        Ftp        '�]���J�n��ACK�d����M��
    End Enum

    '�󓮓I�t�@�C���]���V�[�P���X�Ɋւ����Ԃ̒�`
    Protected Enum PassiveXllState As Integer
        None       '���s�O�E���s���̎󓮓I�t�@�C���]���V�[�P���X�Ȃ��i�]���J�n��ACK�d�����M�O�j
        Ftp        '�]���J�n��ACK�d�����M��
    End Enum

    'NAK�d���̗v��
    Protected Enum NakRequirement As Integer
        ForgetOnRetryOver       '���Ƃ݂Ȃ��ׂ��łȂ�
        CareOnRetryOver         '�p������ꍇ�͖��Ƃ݂Ȃ��ׂ�
        DisconnectImmediately   '�R�l�N�V������ؒf���ׂ�
    End Enum
#End Region

#Region "�萔��ϐ�"
    '�\�P�b�g����d������荞�ނ��߂̃C���^�t�F�[�X
    'NOTE: �{�N���X�ɂƂ��Ă̂���́ATelegram�t�@�N�g���ł��邪�A���ꎩ�g�́A
    'Telegram�̐��������ɒS�����Ƃ�ړI�Ƃ��đ��݂��Ă���킯�ł͂Ȃ��B
    'TelegramImporter�́u�\�P�b�g��t�@�C���Ȃǂ̊O���}�̂���v�d������荞��
    '���Ƃ���Ƃ���N���X�ł���BTelegram�C���X�^���X�����̂��߂̎�����
    '�eTelegram�N���X�ōs�����ƂɂȂ��Ă���ATelegramImporter������𗘗p����
    '����B���ۂɁA�{�N���X��Telegram�C���X�^���X����������̂�
    'oTelegImporter.GetTelegramFromSocket()�����ł͂Ȃ��B�{�N���X��
    '�T�u�N���X��ProcOnHogeRequestReceive()�Ȃǂ�New�Œ��ڐ��������e��Telegram
    '�C���X�^���X����������B�܂��ATelegram�C���X�^���X���̂ɂ��A���ꎩ�g��
    '�΂���ے艞��Telegram�C���X�^���X���𐶐�����@�\������B
    Protected oTelegImporter As ITelegramImporter

    '�d������M�p�\�P�b�g
    'NOTE: �N���[�Y���{���_��Nothing�ɖ߂����ƂɂȂ��Ă���B
    Protected oTelegSock As Socket

    'NOTE: ����Telegrapher�����REQ�d�����M�Ŏn�܂�V�[�P���X��\���I�V�[�P���X
    '�ƌĂԁB��x�ł�REQ�d���𑗐M������A���g���C�C���^�[�o�������܂߂ĂP��
    '�\���I�V�[�P���X�����s���Ă�����̂Ƃ݂Ȃ��BREQ�d���̑��M��A����ɑ΂���
    '�����d������M����܂ŁA����REQ�d���͑��M���Ȃ����A���ꂪ�����͈͂ŁA
    '�����̔\���I�V�[�P���X����s���Ď��s����BREQ�d���̑��M�ɂ͉��L�̗D�揇��
    '��݂���i��ʂɋL�ڂ���REQ�d����D�悵�đ��M����j�B
    ' (1)�E�H�b�`�h�b�O�V�[�P���X��REQ�d��
    ' (2)�E�H�b�`�h�b�O�V�[�P���X�ȊO�̔\���I�P���V�[�P���X��REQ�d��
    ' (3)�\���I�t�@�C���]���V�[�P���X��REQ�d���i�]���J�n��REQ�d���j
    '����ɂ��A�V�[�P���X�S�̂̎��s�����ɂ́A�ȉ��̂悤�ȋK�����ł���B
    ' (a)��ʋL�ڃV�[�P���X�̎��s���T���Ă���i�J�n���Ă��Ȃ��j�ꍇ�A
    '    ���ʋL�ڃV�[�P���X�͊J�n���Ȃ��B
    ' (b)��ʋL�ڃV�[�P���X�����s���ɂȂ�΁A���M�ς݂�REQ�d���ɑ΂���
    '    �����d����M�҂��łȂ�����́i�����A��ʋL�ڃV�[�P���X�����g���C
    '    �C���^�[�o�����Ȃ�΁j�A���ʋL�ڃV�[�P���X���J�n����B
    ' (c)���ʋL�ڃV�[�P���X�̎��s��Ԃ��ǂ��ł��낤�ƁA���M�ς݂�REQ�d����
    '    �΂��鉞���d����M�҂��łȂ�����A��ʋL�ڃV�[�P���X�͊J�n����B
    '�Ȃ��A�ݒ莟��ł́A(2)�L�ڂ̃V�[�P���X���m��A(3)�L�ڂ̃V�[�P���X���m
    '�́A�����Ɏ��s���Ȃ��B�����A��ɊJ�n�����V�[�P���X�S�̂��I������i����
    '�܂��̓��g���C�I�[�o�[����j�܂ŁA������ނ̃V�[�P���X�͊J�n���Ȃ��B
    '����āA�ݒ莟��ł́A(b)�L�ڂ́u��ʋL�ڃV�[�P���X�v�Ƃ́u��ʂɋL��
    '����Ă���e�V�[�P���X�̒��ōŏ��ɍT���Ă����i�J�n�����j���́v�Ƃ���
    '�Ӗ��ł���B

    '�\���I�d�������Ɋւ�����
    'NOTE: REQ/ACK���x���̏������ێ����邽�߂́iREQ�d�����M�`�����d����M��
    '�r���I�ɍs�����߂́j��ԂƂ���B
    'NOTE: curState�����̏��́AoTelegSock��oLastSentReqTeleg������擾�ł���
    '���A�X�V���̃t�b�N���L�p�ɂȂ�\��������̂ŁA�p�ӂ��Ă���B
    'NOTE: ���Ƃ�isPendingFooBarRetry���ɂ���Đ摗��ɂ��Ă���REQ�d���̍đ��M
    '�ł����Ă��A�D�揇�ʂ�����REQ�d���̒ʏ푗�M�i�摗��ɂ��Ă��Ȃ��ꍇ�̑��M
    '��đ��M�j����񂵂ɂ���B�D�揇�ʂ̍���REQ�d������ɍT���Ă���ꍇ�A
    '���ꂪ�����̗v���Ń��g���C�C���^�[�o���ɓ���Ȃ�����́A�D��x�̒Ⴂ
    '�V�[�P���X�́A�i���ɊJ�n���Ȃ������łȂ��A���ɊJ�n���Ă���Ήi���ɏI��
    '���Ȃ��i���g���C�I�[�o�[���ɂȂ�Ȃ��j���ƂɂȂ邪�A����͐݌v�v�z�ɍ��v
    '���Ă���B�v���M��REQ�d�����c�葱����i�������x�̕������M���x��������
    '���č����j�Ƃ������Ƃ́A�������������Ă͂Ȃ�Ȃ����Ƃł��邵�A�ؗ�����
    '���Ԃ̗P�\�́A�D�揇�ʂ̍���REQ�d���̕����Z���킯�ł���B�T�[�o�ɂ����āA
    '�܂��A�E�H�b�`�h�b�O�V�[�P���X�́A�L���[�C���O����Ȃ��i�D�揇�ʂ��Ⴂ
    '�V�[�P���X���J�n���錄���K��������j��A����REQ�d�����������ɑ��M����
    '����ΒʐM�ُ�ɒ�������̂ŁA�ŗD��ł���B�܂��A�\���I�P���V�[�P���X
    '�Ɣ\���I�t�@�C���]���V�[�P���X�ł́A�p�r��A�O�҂̕������������������߁A
    '�O�҂��D��ł���B
    Protected curState As State
    Protected isPendingWatchdog As Boolean
    Protected oActiveOneRetryPendingQueue As Queue(Of ActiveOne)
    Protected isPendingActiveXllRetry As Boolean

    '�Ō�ɑ��M����REQ�d��
    'NOTE: �����d����M���_��Nothing�ɖ߂����ƂɂȂ��Ă���B
    Protected oLastSentReqTeleg As IReqTelegram

    '�E�H�b�`�h�b�O�V�[�P���X��REQ�d��
    Protected oWatchdogReqTeleg As IReqTelegram

    '�\���I�P���V�[�P���X�̃L���[
    'NOTE: ���̃L���[�̗v�f�́A�E�H�b�`�h�b�O�V�[�P���X�ȊO�̔\���I�ȒP��
    '�V�[�P���X�ɑ�������B�擪�̗v�f�́A���ݎ��s���ł��邩�A�����Ȃ��΁A
    '�\���I�d�������̏�Ԃ�Idling�ɖ߂����ۂ�A�y���f�B���O����Ă���
    '�E�H�b�`�h�b�O�V�[�P���X�������Ȃ����ۂɊJ�n����͂��̂��̂ł���B
    '�\���I�P���V�[�P���X�����������[�h�ł́A�擪�v�f�̃V�[�P���X���I��
    '�i�����܂��̓��g���C�I�[�o�[�j���Ȃ�����A���ȍ~�̗v�f�͎��s���Ȃ��B
    'NOTE: �\���I�P���V�[�P���X�̃��g���C�^�C���A�E�g�������_�������΁A
    '���Ƃ����̃L���[�ɗv�f�����݂��Ă��Ă��A�S�v�f��CurTryCount��1�ȏ�
    '�i�S�v�f�����񑗐M�ς݁j�ł��肩�AoActiveOneRetryPendingQueue.Count��0
    '�ł���i�摗��ɂ���Ă���đ��M�������j�ꍇ�́A�\���I�P���V�[�P���X��
    '�S�ă��g���C�C���^�[�o�����ł���Ƃ�����B���̏ꍇ�́A�\���I�P���V�[�P���X
    '���D��x�̒Ⴂ�\���I�V�[�P���X�ł����Ă��A���{�\�ł���B
    Protected oActiveOneQueue As LinkedList(Of ActiveOne)

    '�Ō��REQ�d�����M�����{�����\���I�P���V�[�P���X
    'NOTE: �����d����M���_��Nothing�ɖ߂����ƂɂȂ��Ă���B
    Protected oLastSentActiveOne As ActiveOne

    '�\���I�t�@�C���]���V�[�P���X�̃L���[
    'NOTE: ���̃L���[�̗v�f�́A�\���I�t�@�C���]���V�[�P���X�ɑ�������B�擪��
    '�v�f�́A���ݎ��s���ł��邩�A�����Ȃ��΁A�\���I�d�������̏�Ԃ�Idling��
    '�߂����ۂ�A�y���f�B���O����Ă���E�H�b�`�h�b�O�V�[�P���X�������Ȃ�����
    '��A�S�Ă̔\���I�P���V�[�P���X�������Ȃ����ۂ�A�S�Ă̔\���I�P���V�[�P���X
    '�����g���C�C���^�[�o�����ɂȂ����ۂɊJ�n����͂��̂��̂ł���B
    '����̎����ł́A�擪�v�f�̃V�[�P���X���I���i�����܂��̓��g���C�I�[�o�[�j
    '���Ȃ�����A���ȍ~�̗v�f�͎��s���Ȃ��B
    Protected oActiveXllQueue As LinkedList(Of ActiveXll)

    '�\���I�t�@�C���]���V�[�P���X�̏��
    'NOTE: ���肩���M����REQ�d����ObjCode���������ۂ����肷�邽�߂ɕK�v��
    '����B���ꂪ�Ȃ��ƁAoActiveXllQueue�̐擪�v�f�ɓ]���J�n��REQ�d�����i�[
    '����Ă���󋵂ɂ����āA����Ɠ���ObjCode�̓]���I����REQ�d������M����
    '�ꍇ�ɁA�����ȃV�[�P���X�����{����Ă��邩�ۂ���e�Ղɂ͔���ł��Ȃ��B
    '�]���J�n��ACK�d����M�܂ōς�ł��邩�ۂ��𔻒�ł���΂悢�킯�ł��邪�A
    '���Ȃ�ʓ|�Ȕ��肪�K�v�ɂȂ��Ă��܂��͂��ł���B
    'NOTE: ���Ƃ��]���J�nREQ�d���̑��M����x�����{���Ă��Ȃ��Ă��A
    'oActiveXllQueue�̐擪�ɂ���V�[�P���X�̏����i���̓d����M�܂ł�
    'oActiveXllQueue���N���A����邱�Ƃ��m���łȂ�����K���j�Z�b�g���Ă����B
    'oActiveXllQueue����ł���΁AActiveXllState.None���Z�b�g���Ă����B
    Protected curActiveXllState As ActiveXllState

    '�󓮓I�t�@�C���]���V�[�P���X�̃L���[
    'NOTE: ���̃L���[�̗v�f�́A�󓮓I�t�@�C���]���V�[�P���X�ɑ�������B
    '�擪�̗v�f�́A���ݎ��s���̃V�[�P���X�ł���B
    '����̐݌v�ł́A���̃L���[�ɂP���ł��v�f�����݂��Ă���΁A�V����
    '�󓮓I�t�@�C���]���V�[�P���X�̓]���J�nREQ�d���͎󂯕t���Ȃ��B
    '����Ӗ��A�L���[�ł���K�v�͂Ȃ����A�\���I�t�@�C���]���Ƃ�
    '��ѐ��m�ۂ�A�����̎󓮓I�t�@�C���]���V�[�P���X����s���{����
    '�\�����l�����āA�L���[�ŊǗ�����B
    Protected oPassiveXllQueue As LinkedList(Of PassiveXll)

    '�󓮓I�t�@�C���]���V�[�P���X�̏��
    'NOTE: �w�ǖ��Ӗ������A�\���I�t�@�C���]���Ƃ̈�ѐ��m�ۂ̂��ߑ��݂���B
    'oPassiveXllQueue�̐擪�ɂ���V�[�P���X�̏����i���̓d����M�܂ł�
    'oPassiveXllQueue���N���A����邱�Ƃ��m���łȂ�����K���j�Z�b�g���Ă����B
    'oPassiveXllQueue����ł���΁APassiveXllState.None���Z�b�g���Ă����B
    Protected curPassiveXllState As PassiveXllState

    '�e��^�C�}
    Protected oWatchdogTimer As TickTimer
    Protected oReplyLimitTimer As TickTimer
    Protected oActiveXllRetryTimer As TickTimer
    Protected oActiveXllLimitTimer As TickTimer  '���삳�������Ȃ��ꍇ��0��-1��ݒ�B
    Protected oPassiveXllLimitTimer As TickTimer  '���삳�������Ȃ��ꍇ��0��-1��ݒ�B

    '�P�d���ǂݏ����̊���
    Protected telegReadingLimitBaseTicks As Integer  '0��-1�͎w��֎~�B
    Protected telegReadingLimitExtraTicksPerMiB As Integer
    Protected telegWritingLimitBaseTicks As Integer  '0��-1�͎w��֎~�B
    Protected telegWritingLimitExtraTicksPerMiB As Integer

    '�P�d��������̃��O�ۑ��ő咷
    Protected telegLoggingMaxLengthOnRead As Integer
    Protected telegLoggingMaxLengthOnWrite As Integer

    '�t�@�C���]���V�[�P���X�r���������[�h�ݒ�
    'NOTE: �\���I�t�@�C���]���Ǝ󓮓I�t�@�C���]������s���Ď��{�����ꍇ��
    '�N���C�A���g���i�����Ɏ��{�ł��Ȃ��悤�ɐ��䂵�Ă���ǂ��납�j�듮�삷��
    '�悤�Ȃ�A�����True�Ƃ���ׂ��ł���B
    'NOTE: �����True�ɐݒ肵�Ă���ꍇ�A�\���I�t�@�C���]���V�[�P���X�̎��{��
    '�́A�󓮓I�t�@�C���]���V�[�P���X�̓]���J�nREQ�d���ɑ΂��ANAK�i�r�W�[�j��
    '�ԐM����B�t�ɁA�\���I�t�@�C���]���V�[�P���X�̓]���J�nREQ�d���𑗐M���ׂ�
    '���_�Ŏ󓮓I�t�@�C���]���V�[�P���X�����s���ł���΁A�\���I�t�@�C���]����
    '�]���J�nREQ�d���͑��M�����A�������Ŏ��s�񐔂𑝐i������B�Ȃ��A�����Ō���
    '�u�\���I�t�@�C���]���V�[�P���X�̎��{���v�́A�\���I�t�@�C���]���V�[�P���X��
    '�]���J�nREQ�d���ɑ΂��鉞���d����M�҂��icurActiveXllState = BeforeFtp
    'AndAlso oActiveXllQueue.First.Value.ReqTeleg = oLastSentReqTeleg�j�̏ꍇ�ƁA
    '�]���J�nACK�d���̎�M��icurActiveXllState = Ftp�j�Ɍ��肷��B
    '����́A�T�[�o�ƃN���C�A���g�̗����ɔ\���I�t�@�C���]���V�[�P���X���T����
    '����ꍇ�̂��������i�L���[�C���O����Ă�����̂�����ԁA�o�����K���r�W�[
    '��Ԃ����ƂŁA�S�Ẵt�@�C���]���V�[�P���X���o���ŕK�����g���C�I�[�o�[��
    '�Ȃ鎖�Ԃ̂��Ƃł���A�ň��̏ꍇ�A���g���C�I�[�o�[�ƂȂ�܂ł̊ԂɁA
    '�N���C�A���g���Ɏ��̔\���I�t�@�C���]���V�[�P���X���L���[�C���O����Ă䂭
    '�Ǝv����j��������邽�߂ł���B
    Protected enableXllStrongExclusion As Boolean

    '�\���I�V�[�P���X�r���������[�h�ݒ�
    'NOTE: �\���I�t�@�C���]���V�[�P���X�̎��{���ɔ\���I�P���V�[�P���X��REQ�d��
    '�𑗐M����ƃN���C�A���g���i�r�W�[��Ԃ��ǂ��납�j�듮�삷��悤�Ȃ�A
    '�����True�Ƃ���ׂ��ł���B
    'NOTE: �����True�ɐݒ肵�Ă���ꍇ�A�\���I�P���V�[�P���X�����{����ۂ�
    '�\���I�t�@�C���]���V�[�P���X�̓]�������s���ł���i�d�������ɂ����ē]��
    '�J�n���������Ă���j�Ȃ�΁A�\���I�P���V�[�P���X��REQ�d���͑��M�����A
    '�������Ŏ��s�񐔂𑝐i������B
    Protected enableActiveSeqStrongExclusion As Boolean

    '�\���I�P���V�[�P���X�����������[�h�ݒ�
    Protected enableActiveOneOrdering As Boolean

    '���莞�Ԃ����Z���Ԋu��SystemTick���������ށi0�`0xFFFFFFFF�j
    Private _LastPulseTick As Long
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket, ByVal oTelegImporter As ITelegramImporter)
        'NOTE: ���̃��\�b�h�͐e�X���b�h�Ŏ��s����邱�ƂɂȂ�B�����āA
        '�����Łi�e�X���b�h�Łj�����������ϐ��́AMyBase.Start���\�b�h�����s����
        '�ȍ~�A�q�X���b�h�ŎQ�Ƃ���邱�ƂɂȂ�B�������AMyBase.Start���\�b�h��
        '�������o���A�ƂȂ邽�߁A�������͒P��������ōς܂��Ė��Ȃ��B

        MyBase.New(sThreadName, oParentMessageSock)

        Me.oTelegImporter = oTelegImporter
        Me.oTelegSock = Nothing

        Me.curState = State.NoConnection
        Me.isPendingWatchdog = False
        Me.oActiveOneRetryPendingQueue = New Queue(Of ActiveOne)
        Me.isPendingActiveXllRetry = False
        Me.oLastSentReqTeleg = Nothing

        Me.oWatchdogReqTeleg = Nothing
        Me.oActiveOneQueue = New LinkedList(Of ActiveOne)
        Me.oLastSentActiveOne = Nothing
        Me.oActiveXllQueue = New LinkedList(Of ActiveXll)
        Me.curActiveXllState = ActiveXllState.None
        Me.oPassiveXllQueue = New LinkedList(Of PassiveXll)
        Me.curPassiveXllState = PassiveXllState.None

        'NOTE: �����oWatchdogTimer�̐ݒ莞�Ԃ́A���葕�u�ɑ΂���
        '�E�H�b�`�h�b�OREQ�d���̑��M�����ƁA�e�X���b�h�Ɍ��J����
        'LastPulseTick�̍X�V���������˂Ă���̂Œ��ӁB
        Me.oWatchdogTimer = New TickTimer(60 * 1000)  'NOTE: MayOverride
        Me.oReplyLimitTimer = New TickTimer(0)
        Me.oActiveXllRetryTimer = New TickTimer(0)
        Me.oActiveXllLimitTimer = New TickTimer(0)
        Me.oPassiveXllLimitTimer = New TickTimer(0)

        'NOTE: MayOverride
        Me.telegReadingLimitBaseTicks = 10 * 1000
        Me.telegReadingLimitExtraTicksPerMiB = 0
        Me.telegWritingLimitBaseTicks = 5 * 1000
        Me.telegWritingLimitExtraTicksPerMiB = 0

        'NOTE: MayOverride
        Me.telegLoggingMaxLengthOnRead = 0
        Me.telegLoggingMaxLengthOnWrite = 0

        'NOTE: MayOverride
        Me.enableXllStrongExclusion = False
        Me.enableActiveSeqStrongExclusion = False
        Me.enableActiveOneOrdering = False

        Me.LastPulseTick = 0
    End Sub
#End Region

#Region "�e�X���b�h�p���\�b�h"
    Public Overrides Sub Start()
        Dim systemTick As Long = TickTimer.GetSystemTick()
        LastPulseTick = systemTick
        RegisterTimer(oWatchdogTimer, systemTick)

        MyBase.Start()
    End Sub
#End Region

#Region "�v���p�e�B"
    'NOTE: �q�X���b�h���J�n���Ĉȍ~��_LastPulseTick�́A�J�[�l��������r������
    '�Ȃ��ɁA�q�X���b�h�ŏ������݁A�e�X���b�h�œǂݏo�����Ƃɂ��Ă���B
    '�Ȃ��A_LastPulseTick�́A���ۓI�ɂ́Ax86-64�v���Z�b�T�ɂ�����ʏ��
    '�]�����߂P�Łi�����A���Ȃ��Ƃ������ɂ�镪�f�͖����Ɂj�S�̂�ǂށi�����j
    '���Ƃ��\�ȃT�C�Y�ł���A�����R�A�ɂ��o�X�I�y���[�V�������x���ł�
    '�ǂݏ�������������邱�Ƃ̂Ȃ��ʒu�ɔz�u����Ă���Ǝv����B�܂��A
    '�������݂��s���X���b�h���P�ł��邽�߁A�������݂̋����ɂ��ẴP�A��
    '�s�v�ł���B�������Ȃ���AThread�N���X��VolatileRead��VolatileWrite��
    '�g�p���Ȃ����j�Ƃ���B�����̃��\�b�h�͕s���ȓ�����Ӑ}���Ă���
    '�킯�ł͂Ȃ��i���Ƃ��΁AVolatileWrite�́AVolatileRead���g�p����ʂ�
    '�X���b�h����̉�����ۏ؂��Ă��Ă��A�s���Ɍ����鏑��������ۏ؂��Ă���
    '�킯�ł͂Ȃ��j�Ǝv����̂ɑ΂��A�����̕ϐ��Ɋi�[����l�́A�ꉞ�S�o�C�g
    '�ňӖ��𐬂����̂ł��邽�߂ł���B_LastPulseTick�́A�����Ď��Ɏg������
    '�̏d�v�ȕϐ��ł��邩��A�p�t�H�[�}���X��̂�قǂ̕K�v�����Ȃ�����
    '�iLOCK�M���ɂ��o�X�̐��\�ቺ������ƂȂ�悤�ȏ󋵂ɂȂ�Ȃ�����j
    'VolatileRead��VolatileWrite�ɕύX���Ă͂Ȃ�Ȃ��B
    Public Property LastPulseTick() As Long
        Get
            Return Interlocked.Read(_LastPulseTick)
        End Get

        Protected Set(ByVal tick As Long)
            Interlocked.Exchange(_LastPulseTick, tick)
        End Set
    End Property
#End Region

#Region "�C�x���g�������\�b�h"
    Protected Overrides Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        If oTimer Is oWatchdogTimer Then
            Return ProcOnWatchdogTime()
        End If

        If oTimer Is oReplyLimitTimer Then
            Return ProcOnReplyLimitTime()
        End If

        For Each oOne As ActiveOne In oActiveOneQueue
            If oTimer Is oOne.RetryTimer Then
                Return ProcOnActiveOneRetryTime(oOne)
            End If
        Next oOne

        If oTimer Is oActiveXllRetryTimer Then
            Return ProcOnActiveXllRetryTime()
        End If

        If oTimer Is oActiveXllLimitTimer Then
            Return ProcOnActiveXllLimitTime()
        End If

        If oTimer Is oPassiveXllLimitTimer Then
            Return ProcOnPassiveXllLimitTime()
        End If

        Debug.Fail("This case is impermissible.")
        Return MyBase.ProcOnTimeout(oTimer)
    End Function

    Protected Overridable Function ProcOnWatchdogTime() As Boolean
        Dim systemTick As Long = TickTimer.GetSystemTick()
        LastPulseTick = systemTick
        RegisterTimer(oWatchdogTimer, systemTick)

        If curState = State.NoConnection Then
            Return True
        End If

        If curState = State.WaitingForReply Then
            isPendingWatchdog = True
            Return True
        End If

        Debug.Assert(curState = State.Idling)
        Debug.Assert(Not isPendingWatchdog)

        'NOTE: �󋵂ɉ����ăE�H�b�`�h�b�O�̗L����d�����e��ύX����ނ�
        '�v���g�R����z�肵�A�ܑ̂Ȃ�������쐬���Ȃ������Ƃɂ��Ă���B
        oWatchdogReqTeleg = CreateWatchdogReqTelegram()
        If oWatchdogReqTeleg IsNot Nothing Then
            Log.Info("Sending Watchdog REQ...")
            If SendReqTelegram(oWatchdogReqTeleg) = False Then
                Disconnect()
                Return True
            End If

            TransitState(State.WaitingForReply)
            oLastSentReqTeleg = oWatchdogReqTeleg
            oReplyLimitTimer.Renew(oWatchdogReqTeleg.ReplyLimitTicks)
            RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnReplyLimitTime() As Boolean
        Log.Error("Reply limit time comes.")
        Disconnect()
        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneRetryTime(ByVal oOne As ActiveOne) As Boolean
        Log.Info("ActiveOne retry time comes.")

        'NOTE: �D�揇�ʂ̈�ѐ����l������ƁAisPendingWatchdog��True�̏ꍇ��
        'ActiveOneRetry�͐摗��ɂ���ׂ��ł��邪�A���̏ꍇ�͉��L�̏������K��
        '�������邽�߁A���̔���͏ȗ�����B
        If curState = State.WaitingForReply Then
            oActiveOneRetryPendingQueue.Enqueue(oOne)
            Return True
        End If

        Debug.Assert(curState = State.Idling)
        Debug.Assert(Not oActiveOneRetryPendingQueue.Contains(oOne))

        If enableActiveSeqStrongExclusion Then
            'NOTE: ���L�̏ꍇ�����łȂ��AcurActiveXllState��BeforeFtp��
            '�ꍇ���A�\���I�t�@�C���]���V�[�P���X�̓]���J�nREQ�d�������M�ς�
            '�Ƃ����\���͂���i���肪ACK�d���𑗐M���Ă���\�����l������
            '�ƁA�\���I�P���V�[�P���X�Ɣr���I�ȏ�ԂƂ݂Ȃ���j�B
            '�������A���̂悤�ȏ�Ԃł���Ȃ�΁A��LcurState�̔����Return����
            '����͂��ł���B�Ȃ��AReturn������AREQ/ACK���x���̃y���f�B���O��
            '��������鎞�i�����d������M������j�ɂ́A�\���I�P���V�[�P���X��
            '�\�ɂȂ��Ă��邩������Ȃ��iNAK�i�r�W�[�j�d���̎�M�ɂ���āA
            '�\���I�t�@�C���]���V�[�P���X�����g���C�C���^�[�o���ɓ����Ă���
            '�������҂ł���j�B
            If curActiveXllState = ActiveXllState.Ftp Then
                Log.Info(oOne.SeqName & " is regulated by ActiveXll.")
                'NOTE: REQ�d���𑗐M����NAK�i�r�W�[�j�d������M�����ꍇ��
                '�����̏������s���B
                oOne.CurTryCount += 1
                oOne.CurNakCountToCare += 1
                If oOne.CurNakCountToCare >= oOne.LimitNakCountToCare Then
                    Log.Warn(oOne.SeqName & " retry over.")
                    ProcOnActiveOneRetryOverToCare(oOne.ReqTeleg, Nothing)
                    oActiveOneQueue.Remove(oOne)
                    DoNextActiveSeq()
                Else
                    RegisterTimer(oOne.RetryTimer, TickTimer.GetSystemTick())
                    'NOTE: ���̃P�[�X�ł́AoActiveOneQueue�̗v�f��CurTryCount��
                    '�ω������邪�A���X1�ȏゾ�����̂��C���N�������g����邾��
                    '�ł���A����ɂ���ĉ��������M�\�ɂȂ�킯�ł͂Ȃ����߁A
                    'DoNextActiveSeq()�͏ȗ�����B
                End If
                Return True
            End If
        End If

        Log.Info("Sending " & oOne.SeqName & " REQ...")
        oOne.CurTryCount += 1
        If SendReqTelegram(oOne.ReqTeleg) = False Then
            Disconnect()
            Return True
        End If

        TransitState(State.WaitingForReply)
        oLastSentReqTeleg = oOne.ReqTeleg
        oLastSentActiveOne = oOne
        oReplyLimitTimer.Renew(oOne.ReqTeleg.ReplyLimitTicks)
        RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
        Return True
    End Function

    Protected Overridable Function ProcOnActiveXllRetryTime() As Boolean
        Log.Info("ActiveXll retry time comes.")

        'NOTE: �D�揇�ʂ̈�ѐ����l������ƁAisPendingWatchdog��True�̏ꍇ��A
        'oActiveOneRetryPendingQueue.Count��0�ȊO�̏ꍇ��A�J�n�O��ActiveOne
        '�V�[�P���X���T���Ă���ꍇ(*1)���AActiveXllRetry�͐摗��ɂ���ׂ���
        '���邪�A�����̏ꍇ�͉��L�̏������K���������邽�߁A�����̔����
        '�ȗ�����B
        '*1 ActiveOne��REQ�d�������D�揇�ʂ̍���REQ�d�����M���y���f�B���O
        '���Ă���ꍇ�ł���A���ǂ̂Ƃ���isPendingWatchdog��True�̏ꍇ�ł���B
        If curState = State.WaitingForReply Then
            isPendingActiveXllRetry = True
            Return True
        End If

        Debug.Assert(curState = State.Idling)
        Debug.Assert(Not isPendingActiveXllRetry)

        Dim oXll As ActiveXll = oActiveXllQueue.First.Value

        'NOTE: ������Idling��Ԃł̂ݎ��s����邽�߁A
        '���Ƃ��\���I�V�[�P���X�r���������[�h�ł����Ă��A
        '�\���I�P���V�[�P���X�̎��s��Ԃ��C�ɂ���K�v�͂Ȃ��B
        If enableXllStrongExclusion Then
            If curPassiveXllState = PassiveXllState.Ftp Then
                'NOTE: ���g���C�C���^�[�o���̊ԂɃN���C�A���g����PassiveXll��
                '�]���J�nREQ�d������M���A������󂯕t���Ă����ꍇ�ł���B
                Log.Info("ActiveXll is regulated by PassiveXll.")
                'NOTE: REQ�d���𑗐M����NAK�i�r�W�[�j�d������M�����ꍇ��
                '�����̏������s���B
                oXll.CurTryCount += 1
                oXll.CurNakCountToCare += 1
                If oXll.CurNakCountToCare >= oXll.LimitNakCountToCare Then
                    If oXll.Direction = XllDirection.Dll Then
                        Log.Warn("ActiveDll retry over.")
                        ProcOnActiveDllRetryOverToCare(oXll.ReqTeleg, Nothing)
                    Else
                        Log.Warn("ActiveUll retry over.")
                        ProcOnActiveUllRetryOverToCare(oXll.ReqTeleg, Nothing)
                    End If
                    oActiveXllQueue.RemoveFirst()
                    UpdateActiveXllStateAfterDequeue()
                    DoNextActiveSeq()
                Else
                    oActiveXllRetryTimer.Renew(oXll.RetryIntervalTicks)
                    RegisterTimer(oActiveXllRetryTimer, TickTimer.GetSystemTick())
                    'NOTE: ���̃P�[�X�ł́AoActiveXllQueue�g�b�v��CurTryCount��
                    '�ω������邪�A���X1�ȏゾ�����̂��C���N�������g����邾��
                    '�ł���A����ɂ���ĉ��������M�\�ɂȂ�킯�ł͂Ȃ����߁A
                    'DoNextActiveSeq()�͏ȗ�����B
                End If
                Return True
            End If
        End If

        If oXll.Direction = XllDirection.Dll Then
            Log.Info("Sending ActiveDllStart REQ...")
        Else
            Log.Info("Sending ActiveUllStart REQ...")
        End If
        oXll.CurTryCount += 1
        If SendReqTelegram(oXll.ReqTeleg) = False Then
            Disconnect()
            Return True
        End If

        TransitState(State.WaitingForReply)
        oLastSentReqTeleg = oXll.ReqTeleg
        oReplyLimitTimer.Renew(oXll.ReqTeleg.ReplyLimitTicks)
        RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
        Return True
    End Function

    Protected Overridable Function ProcOnActiveXllLimitTime() As Boolean
        Log.Error("ActiveXll limit time comes.")

        Dim oXll As ActiveXll = oActiveXllQueue.First.Value

        If oXll.Direction = XllDirection.Dll Then
            ProcOnActiveDllTimeout(oXll.ReqTeleg)
        Else
            ProcOnActiveUllTimeout(oXll.ReqTeleg)
        End If
        oActiveXllQueue.RemoveFirst()
        UpdateActiveXllStateAfterDequeue()

        Disconnect()
        Return True
    End Function

    Protected Overridable Function ProcOnPassiveXllLimitTime() As Boolean
        Log.Error("PassiveXll limit time comes.")

        Dim oXll As PassiveXll = oPassiveXllQueue.First.Value

        If oXll.Direction = XllDirection.Dll Then
            ProcOnPassiveDllTimeout(oXll.ReqTeleg)
        Else
            ProcOnPassiveUllTimeout(oXll.ReqTeleg)
        End If
        oPassiveXllQueue.RemoveFirst()
        UpdatePassiveXllStateAfterDequeue()

        Disconnect()
        Return True
    End Function

    Protected Overrides Function ProcOnSockReadable(ByVal oSock As Socket) As Boolean
        If oSock Is oParentMessageSock Then
            Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oSock)
            Return ProcOnParentMessageReceive(oRcvMsg)
        End If

        If oSock Is oTelegSock Then
            Dim oRcvTeleg As ITelegram _
               = oTelegImporter.GetTelegramFromSocket(oSock, telegReadingLimitBaseTicks, telegReadingLimitExtraTicksPerMiB, telegLoggingMaxLengthOnRead)
            If oRcvTeleg Is Nothing Then
                Disconnect()
                Return True
            End If
            Return ProcOnTelegramReceive(oRcvTeleg)
        End If

        Debug.Fail("This case is impermissible.")
        Return MyBase.ProcOnSockReadable(oSock)
    End Function

    'NOTE: MayOverride
    Protected Overridable Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Select Case oRcvMsg.Kind
            Case InternalMessageKind.QuitRequest
                Return ProcOnQuitRequestReceive(oRcvMsg)

            Case InternalMessageKind.ConnectNotice
                Return ProcOnConnectNoticeReceive(oRcvMsg)

            Case InternalMessageKind.DisconnectRequest
                Return ProcOnDisconnectRequestReceive(oRcvMsg)

            Case Else
                Debug.Fail("This case is impermissible.")
                Return True
        End Select
    End Function

    Protected Overridable Function ProcOnQuitRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("Quit requested by manager.")

        'NOTE: ���̃X���b�h������I������ۂ͂�����K���ʂ�A
        '�ُ�I������ۂ�ProcOnUnhandledException()���K�����s�����
        '�͂��ł��邽�߁A�t�@�C�i���C�U�����s����鎞�_�ł́A
        '�K�v��Close()��Dispose()�͊��Ɏ��s���Ă���z��ł���B
        '����AGC�̃p�t�H�[�}���X���l�����A�t�@�C�i���C�U�͎�������
        '���Ȃ����A�S�z�ł���΁A�t�@�C�i���C�U��p�ӂ��A������
        'Debug.Assert(oTelegSock Is Nothing)
        'Debug.Assert(oParentMessageSock Is Nothing)
        '�̂悤�ȃ`�F�b�N����������Ƃ悢�B

        If curState <> State.NoConnection Then
            Disconnect()
        End If

        UnregisterSocket(oParentMessageSock)
        oParentMessageSock.Close()
        oParentMessageSock = Nothing

        Return False
    End Function

    Protected Overridable Function ProcOnConnectNoticeReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("New socket comes from manager.")

        If curState <> State.NoConnection Then
            Disconnect()
        End If

        Connect(ConnectNotice.Parse(oRcvMsg).GetSocket())

        '�]���@�́A�V���ȃR�l�N�V�����ɂ�����E�H�b�`�h�b�OREQ�d����
        '���M���R�l�N�V�����m����60�b��ɍs���悤�ɂȂ��Ă���B
        '�{�v���O�����ł����̎d�l���p������B
        Dim systemTick As Long = TickTimer.GetSystemTick()
        LastPulseTick = systemTick
        RegisterTimer(oWatchdogTimer, systemTick)
        Return True
    End Function

    Protected Overridable Function ProcOnDisconnectRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("Disconnect requested by manager.")

        If curState <> State.NoConnection Then
            Disconnect()
        End If
        Return True
    End Function

    Protected Overridable Function ProcOnTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim violation As NakCauseCode = oRcvTeleg.GetHeaderFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("Telegram with invalid HeadPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Select Case oRcvTeleg.CmdKind
            Case CmdKind.Req
                Return ProcOnReqTelegramReceive(oRcvTeleg)
            Case CmdKind.Ack
                Return ProcOnAckTelegramReceive(oRcvTeleg)
            Case CmdKind.Nak
                Return ProcOnNakTelegramReceive(oRcvTeleg)
            Case Else
                Log.Error("Telegram with invalid CmdKind received.")
                SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oRcvTeleg)
                Return True
        End Select
    End Function

    Protected Overridable Function ProcOnReqTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        If curActiveXllState = ActiveXllState.Ftp Then
            Dim oCurXll As ActiveXll = oActiveXllQueue.First.Value
            If oRcvTeleg.IsSameKindWith(oCurXll.ReqTeleg) Then
                Dim sSeqName As String = If(oCurXll.Direction = XllDirection.Dll, "ActiveDll", "ActiveUll")
                Dim oXllReqTeleg As IXllReqTelegram = oCurXll.ReqTeleg.ParseAsSameKind(oRcvTeleg)
                Dim violation As NakCauseCode = oXllReqTeleg.GetBodyFormatViolation()
                If violation <> NakCauseCode.None Then
                    Log.Error(sSeqName & " REQ with invalid BodyPart received.")
                    SendNakTelegramThenDisconnect(violation, oRcvTeleg)
                    Return True
                End If
                If oXllReqTeleg.IsContinuousWith(oCurXll.ReqTeleg) Then
                    Return ProcOnContinuousActiveXllReqTelegramReceive(oCurXll.Direction, oXllReqTeleg)
                End If
            End If
        End If

        If curPassiveXllState = PassiveXllState.Ftp Then
            Dim oCurXll As PassiveXll = oPassiveXllQueue.First.Value
            If oRcvTeleg.IsSameKindWith(oCurXll.ReqTeleg) Then
                Dim sSeqName As String = If(oCurXll.Direction = XllDirection.Dll, "PassiveDll", "PassiveUll")
                Dim oXllReqTeleg As IXllReqTelegram = oCurXll.ReqTeleg.ParseAsSameKind(oRcvTeleg)
                Dim violation As NakCauseCode = oXllReqTeleg.GetBodyFormatViolation()
                If violation <> NakCauseCode.None Then
                    Log.Error(sSeqName & " REQ with invalid BodyPart received.")
                    SendNakTelegramThenDisconnect(violation, oRcvTeleg)
                    Return True
                End If
                If oXllReqTeleg.IsContinuousWith(oCurXll.ReqTeleg) Then
                    Return ProcOnContinuousPassiveXllReqTelegramReceive(oCurXll.Direction, oXllReqTeleg)
                End If
            End If
        End If

        If IsPassiveDllReq(oRcvTeleg) Then
            Return ProcOnPassiveXllReqTelegramReceive(XllDirection.Dll, oRcvTeleg)
        End If

        If IsPassiveUllReq(oRcvTeleg) Then
            Return ProcOnPassiveXllReqTelegramReceive(XllDirection.Ull, oRcvTeleg)
        End If

        Return ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnContinuousActiveXllReqTelegramReceive(ByVal direction As XllDirection, ByVal oXllReqTeleg As IXllReqTelegram) As Boolean
        Dim sSeqName As String = If(direction = XllDirection.Dll, "ActiveDll", "ActiveUll")
        Select Case oXllReqTeleg.ContinueCode
            Case ContinueCode.Finish
                Log.Info(sSeqName & "Finish REQ received.")
                UnregisterTimer(oActiveXllLimitTimer)

                Dim nakCause As NakCauseCode = NakCauseCode.None
                If direction = XllDirection.Dll Then
                    'NOTE: ���L���\�b�h�́A���Y�N���C�A���g�ɑ΂���t�@�C���z�M��Ԃ�
                    '�F�����X�V���邽�߂̃��\�b�h�ł���B�N���C�A���g�͊��Ƀt�@�C����
                    '�ۑ����Ă���͂��ł��邽�߁A�\�Ȍ��肱����̔F�����N���C�A���g
                    '�̏�Ԃƍ��v����悤�ɁA���̎��_�ŌĂяo�����Ƃɂ���B
                    '�Ȃ��A�{�V�[�P���X�����{����N���C�A���g�ɂ����āA��M�����t�@�C��
                    '��ۑ����邩�ۂ����T�[�o����̉����ɏ]���Č��߂Ă͂Ȃ�Ȃ����Ƃ́A
                    '��ΓI�ȃ��[���ł���B���ɁA�N���C�A���g���A�T�[�o����̉�����
                    '����܂ŕۑ����s��Ȃ��ƂȂ�ƁA�T�[�o�́A���̃V�[�P���X�Ɋւ���
                    '�������I�������_�ł��i�\�P�b�g�ւ�ACK�d���̏������݂ɐ���������
                    '���Ă��j�N���C�A���g�ɑ΂���z�M��Ԃ̔F�����X�V���邱�Ƃ�
                    '�ł��Ȃ��Ȃ��Ă��܂��B�����A����n�ł���ɂ�������炸�A�z�M���
                    '���u�s���v�Ƃ��Ȃ���΂Ȃ�Ȃ����߁A���̌�A�V���ȃR�l�N�V������
                    '����f�[�^�̔z�M���삪�s��ꂽ�Ƃ��̉^�p�ɁA���������Q��������B
                    ProcOnActiveDllComplete(oXllReqTeleg)
                Else
                    oXllReqTeleg.ImportFileDependentValueFromSameKind(oActiveXllQueue.First.Value.ReqTeleg)
                    If Not oXllReqTeleg.IsHashValueIndicatingOkay Then
                        Log.Error("The hash values differ from one another.")
                        nakCause = ProcOnActiveUllHashValueError(oXllReqTeleg)
                    Else
                        'NOTE: ���L�̃��\�b�h�ł́A��M�����t�@�C���̕ۑ����s���͂��ł���B
                        '���͂�����Ăяo���^�C�~���O�ł��邪�A���Ƃ��\�P�b�g�ւ�ACK�d��
                        '�������ݐ�����ɌĂяo�����Ƃ��Ă��A�N���C�A���g��������̃t�@�C���ۑ�
                        '��F�����Ă���i�F������j���ۂ��͂킩��Ȃ����߁A�ǂ݂̂��N���C�A���g
                        '�Ƃ̔F��������Ȃ��Ȃ�\���͔r���ł��Ȃ��B
                        '���������A���Ƃ��\�P�b�g�ւ�ACK�d���������݂ŃG���[���Ԃ��Ă����Ƃ��Ă��A
                        '�ǂ̎��_�̃G���[���͋�ʂ��Ă��Ȃ����߁A�N���C�A���g�ɂ�ACK�d����
                        '�͂��Ă��܂��Ă��鋰�ꂪ����B���̍ۂɂ����炪�t�@�C����ۑ�����
                        '���Ȃ��Ƃ����͍̂ň��ł���A���̂悤�Ȏ��Ԃ�����邽�߂�
                        '�ۑ��ł�����̂͐�ɕۑ����Ă����B
                        nakCause = ProcOnActiveUllComplete(oXllReqTeleg)
                    End If
                End If
                oActiveXllQueue.RemoveFirst()
                UpdateActiveXllStateAfterDequeue()

                If nakCause = NakCauseCode.None Then
                    Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                    Log.Info("Sending " & sSeqName & "Finish ACK...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                Else
                    Dim oReplyTeleg As INakTelegram = oXllReqTeleg.CreateNakTelegram(nakCause)
                    If oReplyTeleg Is Nothing Then
                        Disconnect()
                        Return True
                    End If

                    Log.Info("Sending " & sSeqName & "Finish NAK (" & nakCause.ToString() & ")...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                End If

                If curState = State.Idling Then
                    DoNextActiveSeq()
                End If
                Return True

            Case ContinueCode.FinishWithoutStoring
                Log.Info(sSeqName & "FinishWithoutStoring REQ received.")
                UnregisterTimer(oActiveXllLimitTimer)

                If direction = XllDirection.Dll Then
                    'NOTE: ���L���\�b�h�́A���Y�N���C�A���g�ɑ΂���t�@�C���z�M��Ԃ�
                    '�F�����X�V���邽�߂̃��\�b�h�ł���B
                    ProcOnActiveDllCompleteWithoutStoring(oXllReqTeleg)
                Else
                    'NOTE: Ull�p�d���̃N���X�ɂ����āAContinueCode��
                    'ContinueCode.FinishWithoutStoring���}�b�s���O
                    '����̂͋֎~�ł���B
                    Debug.Fail("This case is impermissible.")
                    Abort()
                End If
                oActiveXllQueue.RemoveFirst()
                UpdateActiveXllStateAfterDequeue()

                Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                Log.Info("Sending " & sSeqName & "FinishWithoutStoring ACK...")
                If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                    Disconnect()
                    Return True
                End If

                If curState = State.Idling Then
                    DoNextActiveSeq()
                End If
                Return True

            Case ContinueCode.Abort
                'NOTE: ���̃P�[�X�̂悤�ɁA�t�@�C���]�����s�̏ꍇ�́A
                '�K���R�l�N�V������ؒf���邱�ƂɂȂ��Ă���B
                '�Ȃ��A�t�@�C���]�����s��̓d���R�l�N�V�����̑�����
                '��������S�ɑ���Ɉς˂Ă��܂��΁A�����_��ł���B
                '���A���葕�u�́A�d���R�l�N�V�������ێ��������ꍇ
                '�i�]����]�������t�@�C���̓��e�������ُ�̏ꍇ�j�A
                'ContinueCode.FinishWithoutStoring�𑗐M���邱�ƂŁA
                '�]�ݒʂ�Ɉێ��ł���B���Ȃ킿�AContinueCode.Abort
                '�̏ꍇ�ɂ����炩�ؒf���錻��̎d�l�ł����Ă��A
                '���葕�u����ŃR�l�N�V�������ێ�������@������
                '�Ƃ������ƂɈႢ�͂Ȃ��B

                Log.Error(sSeqName & "Abort REQ received.")
                UnregisterTimer(oActiveXllLimitTimer)

                If direction = XllDirection.Dll Then
                    ProcOnActiveDllAbort(oXllReqTeleg)
                Else
                    ProcOnActiveUllAbort(oXllReqTeleg)
                End If
                oActiveXllQueue.RemoveFirst()
                UpdateActiveXllStateAfterDequeue()

                'NOTE: �ꕔ�̃v���g�R���d�l���ɞB���ȋL�q������A���̃P�[�X�ł�
                'NAK�d����ԐM����ׂ��ł���悤�ɂ��ǂݎ���B
                '�������A�S�̓I�ɂ݂āAREQ�d���i�]�����s�������j���̂Ɉُ킪
                '�Ȃ����ACK�d����Ԃ��ׂ��ł��邵�AACK�d����Ԃ��V�[�P���X�}��
                '���݂��Ă���B����āA��L�̞B���ȋL�q�́AREQ�d�����̂Ɉُ킪
                '����P�[�X�ɂ��āi����ȃP�[�X������Ƃ������Ƃ��������߂Ɂj
                '�L�ڂ���Ă���ƍl���邱�Ƃɂ���B
                Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                Log.Info("Sending " & sSeqName & "Abort ACK...")
                SendReplyTelegram(oReplyTeleg, oXllReqTeleg)
                '��L�Ăяo���̖߂�l�͖�������i���̌�̏����ɍ��ق��Ȃ����߁j�B
                Disconnect()
                Return True

            Case Else
                Log.Error(sSeqName & " REQ with invalid ContinueCode received.")
                SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oXllReqTeleg)
                Return True
        End Select
    End Function

    Protected Overridable Function ProcOnContinuousPassiveXllReqTelegramReceive(ByVal direction As XllDirection, ByVal oXllReqTeleg As IXllReqTelegram) As Boolean
        Dim sSeqName As String = If(direction = XllDirection.Dll, "PassiveDll", "PassiveUll")
        Select Case oXllReqTeleg.ContinueCode
            Case ContinueCode.Finish
                Log.Info(sSeqName & "Finish REQ received.")
                UnregisterTimer(oPassiveXllLimitTimer)

                Dim nakCause As NakCauseCode = NakCauseCode.None
                If direction = XllDirection.Dll Then
                    'NOTE: ���ɃN���C�A���g�͎�M�t�@�C���̕ۑ����������Ă���͂��ł���B
                    ProcOnPassiveDllComplete(oXllReqTeleg)
                Else
                    oXllReqTeleg.ImportFileDependentValueFromSameKind(oPassiveXllQueue.First.Value.ReqTeleg)
                    If Not oXllReqTeleg.IsHashValueIndicatingOkay Then
                        Log.Error("The hash values differ from one another.")
                        nakCause = ProcOnPassiveUllHashValueError(oXllReqTeleg)
                    Else
                        'NOTE: ���L�̃��\�b�h�ł́A��M�����t�@�C���̕ۑ����s���͂��ł���B
                        '���͂�����Ăяo���^�C�~���O�ł��邪�A���Ƃ��\�P�b�g�ւ�ACK�d��
                        '�������ݐ�����ɌĂяo�����Ƃ��Ă��A�N���C�A���g��������̃t�@�C���ۑ�
                        '��F�����Ă���i�F������j���ۂ��͂킩��Ȃ����߁A�ǂ݂̂��N���C�A���g
                        '�Ƃ̔F��������Ȃ��Ȃ�\���͔r���ł��Ȃ��B
                        '���������A���Ƃ��\�P�b�g�ւ�ACK�d���������݂ŃG���[���Ԃ��Ă����Ƃ��Ă��A
                        '�ǂ̎��_�̃G���[���͋�ʂ��Ă��Ȃ����߁A�N���C�A���g�ɂ�ACK�d����
                        '�͂��Ă��܂��Ă��鋰�ꂪ����B���̍ۂɂ����炪�t�@�C����ۑ�����
                        '���Ȃ��Ƃ����͍̂ň��ł���A���̂悤�Ȏ��Ԃ�����邽�߂�
                        '�ۑ��ł�����̂͐�ɕۑ����Ă����B
                        nakCause = ProcOnPassiveUllComplete(oXllReqTeleg)
                    End If
                End If
                oPassiveXllQueue.RemoveFirst()
                UpdatePassiveXllStateAfterDequeue()

                If nakCause = NakCauseCode.None Then
                    Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                    Log.Info("Sending " & sSeqName & "Finish ACK...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                Else
                    Dim oReplyTeleg As INakTelegram = oXllReqTeleg.CreateNakTelegram(nakCause)
                    If oReplyTeleg Is Nothing Then
                        Disconnect()
                        Return True
                    End If

                    Log.Info("Sending " & sSeqName & "Finish NAK (" & nakCause.ToString() & ")...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                End If

                'NOTE: �����郂�[�h���l�����Ă��A�󓮓I�t�@�C���]���V�[�P���X�̊�������
                '���{����ׂ��\���I�V�[�P���X�͂Ȃ��i�t�@�C���]���V�[�P���X�r���������[�h
                '�ł����Ă��A�󓮓I�t�@�C���]���V�[�P���X�͔\���I�t�@�C���]���V�[�P���X��
                '�u���b�N���Ă���킯�ł͂Ȃ��j�B����āADoNextActiveSeq()�͏ȗ�����B
                Return True

            Case ContinueCode.FinishWithoutStoring
                Log.Info(sSeqName & "FinishWithoutStoring REQ received.")
                UnregisterTimer(oPassiveXllLimitTimer)

                If direction = XllDirection.Dll Then
                    'NOTE: ���ɃN���C�A���g�͎�M�t�@�C���̕ۑ����~�����肵�Ă���B
                    ProcOnPassiveDllCompleteWithoutStoring(oXllReqTeleg)
                Else
                    'NOTE: Ull�p�d���̃N���X�ɂ����āAContinueCode��
                    'ContinueCode.FinishWithoutStoring���}�b�s���O
                    '����̂͋֎~�ł���B
                    Debug.Fail("This case is impermissible.")
                    Abort()
                End If
                oPassiveXllQueue.RemoveFirst()
                UpdatePassiveXllStateAfterDequeue()

                Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                Log.Info("Sending " & sSeqName & "FinishWithoutStoring ACK...")
                If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                    Disconnect()
                    Return True
                End If

                'NOTE: �����郂�[�h���l�����Ă��A�󓮓I�t�@�C���]���V�[�P���X�̊�������
                '���{����ׂ��\���I�V�[�P���X�͂Ȃ��i�t�@�C���]���V�[�P���X�r���������[�h
                '�ł����Ă��A�󓮓I�t�@�C���]���V�[�P���X�͔\���I�t�@�C���]���V�[�P���X��
                '�u���b�N���Ă���킯�ł͂Ȃ��j�B����āADoNextActiveSeq()�͏ȗ�����B
                Return True

            Case ContinueCode.Abort
                Log.Error(sSeqName & "Abort REQ received.")
                UnregisterTimer(oPassiveXllLimitTimer)

                If direction = XllDirection.Dll Then
                    ProcOnPassiveDllAbort(oXllReqTeleg)
                Else
                    ProcOnPassiveUllAbort(oXllReqTeleg)
                End If
                oPassiveXllQueue.RemoveFirst()
                UpdatePassiveXllStateAfterDequeue()

                'NOTE: �ꕔ�̃v���g�R���d�l���ɞB���ȋL�q������A���̃P�[�X�ł�
                'NAK�d����ԐM����ׂ��ł���悤�ɂ��ǂݎ���B
                '�������A�S�̓I�ɂ݂āAREQ�d���i�]�����s�������j���̂Ɉُ킪
                '�Ȃ����ACK�d����Ԃ��ׂ��ł��邵�AACK�d����Ԃ��V�[�P���X�}��
                '���݂��Ă���B����āA��L�̞B���ȋL�q�́AREQ�d�����̂Ɉُ킪
                '����P�[�X�ɂ��āi����ȃP�[�X������Ƃ������Ƃ��������߂Ɂj
                '�L�ڂ���Ă���ƍl���邱�Ƃɂ���B
                Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                Log.Info("Sending " & sSeqName & "Abort ACK...")
                SendReplyTelegram(oReplyTeleg, oXllReqTeleg)
                '��L�Ăяo���̖߂�l�͖�������i���̌�̏����ɍ��ق��Ȃ����߁j�B
                Disconnect()
                Return True

            Case Else
                Log.Error(sSeqName & " REQ with invalid ContinueCode received.")
                SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oXllReqTeleg)
                Return True
        End Select
    End Function

    Protected Overridable Function ProcOnPassiveXllReqTelegramReceive(ByVal direction As XllDirection, ByVal oRcvTeleg As ITelegram) As Boolean
        Dim sSeqName As String = If(direction = XllDirection.Dll, "PassiveDll", "PassiveUll")
        Dim oXllReqTeleg As IXllReqTelegram = If(direction = XllDirection.Dll, ParseAsPassiveDllReq(oRcvTeleg), ParseAsPassiveUllReq(oRcvTeleg))
        Dim violation As NakCauseCode = oXllReqTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error(sSeqName & " REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Select Case oXllReqTeleg.ContinueCode
            Case ContinueCode.Start
                If enableXllStrongExclusion Then
                    If curActiveXllState = ActiveXllState.BeforeFtp Then
                        Dim oActiveXll As ActiveXll = oActiveXllQueue.First.Value
                        If oLastSentReqTeleg Is oActiveXll.ReqTeleg Then
                            'NOTE: ActiveXll�̓]���J�nREQ�d���𑗐M���ĉ�����M
                            '�҂������Ă���ꍇ�ł���B������PassiveXll�̓]��
                            '�J�n��ACK�d����Ԃ��Ă��܂��΁AActiveXll�̓]���J�n
                            'REQ�d���ɑ΂���ACK�d�����Ԃ��Ă����ꍇ�ɁA����
                            'PassiveXll���r�W�[�Ƃ��邱�Ƃ��ł��Ȃ��i�r��������
                            '�V�[�P���X�̓������s�������ɂ́A�R�l�N�V������
                            '�ؒf���邭�炢������i���Ȃ��Ȃ��Ă��܂��j�B
                            '�܂��AActiveXll�Ɋւ��鑊�肩��̉����d����҂���
                            '����APassiveXll�Ɋւ��邱����̉��������߂�̂�
                            '���@�x�ł���i��������l�̂��Ƃ�����΁A�o����
                            '������M�^�C���A�E�g�ƂȂ�j�B
                            '����āA�o�����r�W�[��Ԃ����ƂɂȂ�\���͐�����
                            '���A���̎��_�Ńr�W�[�Ƃ���B
                            Log.Info(sSeqName & "Start REQ received in ActiveXll engaged state.")

                            Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateNakTelegram(NakCauseCode.Busy)
                            If oReplyTeleg Is Nothing Then
                                Disconnect()
                                Return True
                            End If

                            Log.Info("Sending " & sSeqName & "Start NAK (" & NakCauseCode.Busy.ToString() & ")...")
                            If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                                Disconnect()
                                Return True
                            End If
                            Return True
                        End If
                    ElseIf curActiveXllState = ActiveXllState.Ftp Then
                        Log.Info(sSeqName & "Start REQ received while waiting for ActiveXllFinish REQ.")

                        Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateNakTelegram(NakCauseCode.Busy)
                        If oReplyTeleg Is Nothing Then
                            Disconnect()
                            Return True
                        End If

                        Log.Info("Sending " & sSeqName & "Start NAK (" & NakCauseCode.Busy.ToString() & ")...")
                        If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                            Disconnect()
                            Return True
                        End If
                        Return True
                    End If
                End If

                If curPassiveXllState = PassiveXllState.Ftp Then
                    'NOTE: ���_��́A�t�@�C���]�������{���ɐV���ȃt�@�C���]��
                    '�̊J�n��v�����ꂽ�Ƃ��Ă��ASubCmdCode��ObjCode��������
                    'ObjDetail��SubObjCode��t�@�C�������̈Ⴂ�ŋ�ʂ����Ȃ�A
                    '���ł���Ƃ͌����؂�Ȃ��B����āA���L�̂悤�ɃR�l�N�V����
                    '�I���Ɏ������ނ̂ł͂Ȃ��A�r�W�[��ԋp����B
                    'Log.Error(sSeqName & "Start REQ received while waiting for PassiveXllFinish REQ.")
                    'SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oRcvTeleg)
                    'Return True

                    Log.Warn(sSeqName & "Start REQ received while waiting for PassiveXllFinish REQ.")

                    Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateNakTelegram(NakCauseCode.Busy)
                    If oReplyTeleg Is Nothing Then
                        Disconnect()
                        Return True
                    End If

                    Log.Info("Sending " & sSeqName & "Start NAK (" & NakCauseCode.Busy.ToString() & ")...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                    Return True
                End If

                Log.Info(sSeqName & "Start REQ received.")

                Dim nakCause As NakCauseCode
                If direction = XllDirection.Dll Then
                    nakCause = PrepareToStartPassiveDll(oXllReqTeleg)
                Else
                    nakCause = PrepareToStartPassiveUll(oXllReqTeleg)
                End If

                If nakCause = NakCauseCode.None Then
                    If direction = XllDirection.Dll Then
                        oXllReqTeleg.UpdateHashValue()
                    End If
                    Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                    Log.Info("Sending " & sSeqName & "Start ACK...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If

                    oPassiveXllQueue.AddLast(New PassiveXll(direction, oXllReqTeleg))
                    TransitPassiveXllState(PassiveXllState.Ftp)
                    If oXllReqTeleg.TransferLimitTicks > 0 Then
                        oPassiveXllLimitTimer.Renew(oXllReqTeleg.TransferLimitTicks)
                        RegisterTimer(oPassiveXllLimitTimer, TickTimer.GetSystemTick())
                    End If
                Else
                    Dim oReplyTeleg As INakTelegram = oXllReqTeleg.CreateNakTelegram(nakCause)
                    If oReplyTeleg Is Nothing Then
                        Disconnect()
                        Return True
                    End If

                    Log.Info("Sending " & sSeqName & "Start NAK (" & nakCause.ToString() & ")...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                End If
                Return True

            Case Else
                Log.Error(sSeqName & " REQ with invalid ContinueCode received.")
                SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oRcvTeleg)
                Return True
        End Select
    End Function

    'NOTE: MayOverride
    Protected Overridable Function ProcOnPassiveOneReqTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Log.Error("REQ telegram with invalid Kind received.")
        SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oRcvTeleg)
        Return True
    End Function

    Protected Overridable Function ProcOnAckTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        If curState <> State.WaitingForReply Then
            Log.Error("ACK telegram received in disproportionate state.")
            Disconnect()
            Return True
        End If

        If Not oLastSentReqTeleg.IsValidAck(oRcvTeleg) Then
            Log.Error("ACK telegram with disproportionate HeadPart received.")
            Disconnect()
            Return True
        End If

        UnregisterTimer(oReplyLimitTimer)

        Dim toBeContinued As Boolean = True
        If oLastSentReqTeleg Is oWatchdogReqTeleg Then
            toBeContinued = ProcOnWatchdogAckTelegramReceive(oRcvTeleg)
        ElseIf oLastSentActiveOne IsNot Nothing Then
            Debug.Assert(oLastSentReqTeleg Is oLastSentActiveOne.ReqTeleg)
            toBeContinued = ProcOnActiveOneAckTelegramReceive(oRcvTeleg)
        ElseIf curActiveXllState = ActiveXllState.BeforeFtp AndAlso _
           oLastSentReqTeleg Is oActiveXllQueue.First.Value.ReqTeleg Then
            toBeContinued = ProcOnActiveXllAckTelegramReceive(oRcvTeleg)
        Else
            Debug.Fail("This case is impermissible.")
            Disconnect()
            Return True
        End If

        If curState = State.WaitingForReply Then
            If ProcOnReqTelegramSendCompleteByReceiveAck(oLastSentReqTeleg, oRcvTeleg) = False Then
                Disconnect()
                Return True
            End If
            TransitState(State.Idling)
            oLastSentReqTeleg = Nothing
        End If

        '�T�u���\�b�h��Telegrapher���I�����ׂ��Ɣ��f���Ă���ꍇ�́A
        '�ȍ~�̏����͍s��Ȃ��B
        If Not toBeContinued Then
            Return False
        End If

        'NOTE: Disconnect()�Ȃǂ�curState��State.NoConnection�ɕύX���ꂽ
        '�ꍇ�́ADoNextActiveSeq���Ăяo���Ȃ��悤�ɂ��Ă���B���̂悤��
        '�ꍇ�́AisPendingFooBar��eQueue���N���A����Ă��邽�߁A���Ƃ�
        '�Ăяo�����Ƃ��Ă��ADoNextActiveSeq�͉����s��Ȃ��͂��ł��邪�A
        '��^�I�ɏ��������Ă���B
        If curState = State.Idling Then
            DoNextActiveSeq()
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnWatchdogAckTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim violation As NakCauseCode = oLastSentReqTeleg.ParseAsAck(oRcvTeleg).GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("Watchdog ACK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Log.Info("Watchdog ACK received.")
        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneAckTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim oAckTeleg As ITelegram = oLastSentActiveOne.ReqTeleg.ParseAsAck(oRcvTeleg)
        Dim violation As NakCauseCode = oAckTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error(oLastSentActiveOne.SeqName & " ACK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Log.Info(oLastSentActiveOne.SeqName & " ACK received.")

        'NOTE: �\���I�ʒm�V�[�P���X�ɂ����āA���L���\�b�h�́A���Y�N���C�A���g
        '�ɑ΂���f�[�^�ʒm�󋵂̔F�����X�V���邽�߂̃��\�b�h�ł���B
        'NOTE: �\���I�v���V�[�P���X�ɂ����āA���L���\�b�h�́A��M�����f�[�^��
        '�ۑ����s�����߂̃��\�b�h�ł���B�Ȃ��A�N���C�A���g�́A���Ƃ�ACK�d����
        '�\�P�b�g�ւ̏������݂�������������Ƃ����āA�����炪���̃f�[�^��ۑ�
        '�����Ɣ��f����킯�ɂ͂����Ȃ��i���m�Ȕ��f�́A����REQ�d������M����
        '�܂ŕs�\�ł���j�B����āA���̃V�[�P���X�Ŏ󂯎��f�[�^�Ɋւ��ẮA
        '�N���C�A���g���ő��M�ς݂��ۂ����Ǘ�����Ƃ͍l�����Ȃ����߁A
        '������ɂ����Ď�M�ς݂��ۂ����Ǘ�����ׂ��ł���B
        ProcOnActiveOneComplete(oLastSentActiveOne.ReqTeleg, oAckTeleg)
        oActiveOneQueue.Remove(oLastSentActiveOne)
        oLastSentActiveOne = Nothing
        Return True
    End Function

    Protected Overridable Function ProcOnActiveXllAckTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim oCurXll As ActiveXll = oActiveXllQueue.First.Value
        Dim sSeqName As String = If(oCurXll.Direction = XllDirection.Dll, "ActiveDll", "ActiveUll")
        Dim oAckTeleg As IXllTelegram = oCurXll.ReqTeleg.ParseAsAck(oRcvTeleg)
        Dim violation As NakCauseCode = oAckTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error(sSeqName & " ACK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        If oAckTeleg.ContinueCode <> ContinueCode.Start Then
            Log.Error(sSeqName & " ACK with disproportionate ContinueCode received.")
            Disconnect()
            Return True
        End If

        Log.Info(sSeqName & "Start ACK received.")

        If oCurXll.Direction = XllDirection.Ull Then
            oCurXll.ReqTeleg.ImportFileDependentValueFromAck(oAckTeleg)
        End If

        TransitActiveXllState(ActiveXllState.Ftp)
        If oCurXll.ReqTeleg.TransferLimitTicks > 0 Then
            oActiveXllLimitTimer.Renew(oCurXll.ReqTeleg.TransferLimitTicks)
            RegisterTimer(oActiveXllLimitTimer, TickTimer.GetSystemTick())
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnNakTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        If curState <> State.WaitingForReply Then
            Log.Error("NAK telegram received in disproportionate state.")
            Disconnect()
            Return True
        End If

        If Not oLastSentReqTeleg.IsValidNak(oRcvTeleg) Then
            Log.Error("NAK telegram with disproportionate HeadPart received.")
            Disconnect()
            Return True
        End If

        UnregisterTimer(oReplyLimitTimer)

        Dim toBeContinued As Boolean = True
        If oLastSentReqTeleg Is oWatchdogReqTeleg Then
            toBeContinued = ProcOnWatchdogNakTelegramReceive(oRcvTeleg)
        ElseIf oLastSentActiveOne IsNot Nothing Then
            Debug.Assert(oLastSentReqTeleg Is oLastSentActiveOne.ReqTeleg)
            toBeContinued = ProcOnActiveOneNakTelegramReceive(oRcvTeleg)
        ElseIf curActiveXllState = ActiveXllState.BeforeFtp AndAlso _
           oLastSentReqTeleg Is oActiveXllQueue.First.Value.ReqTeleg Then
            toBeContinued = ProcOnActiveXllNakTelegramReceive(oRcvTeleg)
        Else
            Debug.Fail("This case is impermissible.")
            Disconnect()
            Return True
        End If

        If curState = State.WaitingForReply Then
            If ProcOnReqTelegramSendCompleteByReceiveNak(oLastSentReqTeleg, oRcvTeleg) = False Then
                Disconnect()
                Return True
            End If
            TransitState(State.Idling)
            oLastSentReqTeleg = Nothing
        End If

        '�T�u���\�b�h��Telegrapher���I�����ׂ��Ɣ��f���Ă���ꍇ�́A
        '�ȍ~�̏����͍s��Ȃ��B
        If Not toBeContinued Then
            Return False
        End If

        'NOTE: Disconnect()�Ȃǂ�curState��State.NoConnection�ɕύX���ꂽ
        '�ꍇ�́ADoNextActiveSeq���Ăяo���Ȃ��悤�ɂ��Ă���B���̂悤��
        '�ꍇ�́AisPendingFooBar��eQueue���N���A����Ă��邽�߁A���Ƃ�
        '�Ăяo�����Ƃ��Ă��ADoNextActiveSeq�͉����s��Ȃ��͂��ł��邪�A
        '��^�I�ɏ��������Ă���B
        If curState = State.Idling Then
            DoNextActiveSeq()
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnWatchdogNakTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim oNakTeleg As INakTelegram = oLastSentReqTeleg.ParseAsNak(oRcvTeleg)
        Dim violation As NakCauseCode = oNakTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("Watchdog NAK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Dim nakCause As NakCauseCode = oNakTeleg.CauseCode
        If GetRequirement(oNakTeleg) = NakRequirement.DisconnectImmediately Then
            Log.Error("Watchdog NAK (" & nakCause.ToString() & ") received.")
            Disconnect()
        Else
            Log.Warn("Watchdog NAK (" & nakCause.ToString() & ") received.")
            'NOTE: �đ��^�C�}�͊J�n�����A���̒ʏ푗�M�Ɉς˂�B
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneNakTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim oNakTeleg As INakTelegram = oLastSentReqTeleg.ParseAsNak(oRcvTeleg)
        Dim violation As NakCauseCode = oNakTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error(oLastSentActiveOne.SeqName & " NAK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Dim nakCause As NakCauseCode = oNakTeleg.CauseCode
        Dim requirement As NakRequirement = GetRequirement(oNakTeleg)
        If requirement = NakRequirement.DisconnectImmediately Then
            Log.Error(oLastSentActiveOne.SeqName & " NAK (" & nakCause.ToString() & ") received.")
            Disconnect()
        Else
            Log.Info(oLastSentActiveOne.SeqName & " NAK (" & nakCause.ToString() & ") received.")

            If requirement = NakRequirement.CareOnRetryOver Then
                oLastSentActiveOne.CurNakCountToCare += 1
                If oLastSentActiveOne.CurNakCountToCare >= oLastSentActiveOne.LimitNakCountToCare Then
                    Log.Warn(oLastSentActiveOne.SeqName & " retry over.")
                    ProcOnActiveOneRetryOverToCare(oLastSentActiveOne.ReqTeleg, oNakTeleg)
                    oActiveOneQueue.Remove(oLastSentActiveOne)
                Else
                    RegisterTimer(oLastSentActiveOne.RetryTimer, TickTimer.GetSystemTick())
                End If
            Else
                oLastSentActiveOne.CurNakCountToForget += 1
                oLastSentActiveOne.CurNakCountToCare = 0
                If oLastSentActiveOne.CurNakCountToForget >= oLastSentActiveOne.LimitNakCountToForget Then
                    Log.Info(oLastSentActiveOne.SeqName & " retry over.")
                    ProcOnActiveOneRetryOverToForget(oLastSentActiveOne.ReqTeleg, oNakTeleg)
                    oActiveOneQueue.Remove(oLastSentActiveOne)
                Else
                    RegisterTimer(oLastSentActiveOne.RetryTimer, TickTimer.GetSystemTick())
                End If
            End If

            oLastSentActiveOne = Nothing
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnActiveXllNakTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim oCurXll As ActiveXll = oActiveXllQueue.First.Value
        Dim sSeqName As String = If(oCurXll.Direction = XllDirection.Dll, "ActiveDll", "ActiveUll")

        Dim oNakTeleg As INakTelegram = oLastSentReqTeleg.ParseAsNak(oRcvTeleg)
        Dim violation As NakCauseCode = oNakTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error(sSeqName & " NAK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Debug.Assert(oCurXll.ReqTeleg.ContinueCode = ContinueCode.Start)

        Dim nakCause As NakCauseCode = oNakTeleg.CauseCode
        Dim requirement As NakRequirement = GetRequirement(oNakTeleg)
        If requirement = NakRequirement.DisconnectImmediately Then
            Log.Error(sSeqName & " NAK (" & nakCause.ToString() & ") received.")
            Disconnect()
        Else
            Log.Info(sSeqName & " NAK (" & nakCause.ToString() & ") received.")

            If requirement = NakRequirement.CareOnRetryOver Then
                oCurXll.CurNakCountToCare += 1
                If oCurXll.CurNakCountToCare >= oCurXll.LimitNakCountToCare Then
                    Log.Warn(sSeqName & " retry over.")
                    If oCurXll.Direction = XllDirection.Dll Then
                        ProcOnActiveDllRetryOverToCare(oCurXll.ReqTeleg, oNakTeleg)
                    Else
                        ProcOnActiveUllRetryOverToCare(oCurXll.ReqTeleg, oNakTeleg)
                    End If
                    oActiveXllQueue.RemoveFirst()
                    UpdateActiveXllStateAfterDequeue()
                Else
                    oActiveXllRetryTimer.Renew(oCurXll.RetryIntervalTicks)
                    RegisterTimer(oActiveXllRetryTimer, TickTimer.GetSystemTick())
                End If
            Else
                oCurXll.CurNakCountToForget += 1
                oCurXll.CurNakCountToCare = 0
                If oCurXll.CurNakCountToForget >= oCurXll.LimitNakCountToForget Then
                    Log.Info(sSeqName & " retry over.")
                    If oCurXll.Direction = XllDirection.Dll Then
                        ProcOnActiveDllRetryOverToForget(oCurXll.ReqTeleg, oNakTeleg)
                    Else
                        ProcOnActiveUllRetryOverToForget(oCurXll.ReqTeleg, oNakTeleg)
                    End If
                    oActiveXllQueue.RemoveFirst()
                    UpdateActiveXllStateAfterDequeue()
                Else
                    oActiveXllRetryTimer.Renew(oCurXll.RetryIntervalTicks)
                    RegisterTimer(oActiveXllRetryTimer, TickTimer.GetSystemTick())
                End If
            End If
        End If

        Return True
    End Function

    Protected Overrides Sub ProcOnUnhandledException(ByVal ex As Exception)
        'NOTE: ���\�[�X����̂��߂ɁA���̃X���b�h�������I������ۂ́A�K��
        '���L���Ăяo���Ă��������B

        'TODO: Abort()���Ăяo���ꂽ�ۂɂ��A�K�����������s����邱�Ƃ��m�F�B

        If oTelegSock IsNot Nothing Then
            UnregisterSocket(oTelegSock)
            oTelegSock.Close()
            oTelegSock = Nothing
        End If

        'NOTE: �e�X���b�h���őΒ[�̃\�P�b�g��ǂݏ������悤�Ƃ����ۂ�
        '�G���[����������͂��ł���B�e�X���b�h�́A���̂��Ƃ�O��ɂ��āA
        '�������Ȃ���΂Ȃ�Ȃ��B
        If oParentMessageSock IsNot Nothing Then
            UnregisterSocket(oParentMessageSock)
            oParentMessageSock.Close()
            oParentMessageSock = Nothing
        End If

        '���̂܂܌Ăь��ɖ߂��āA�X���b�h�͏I����ԂɂȂ�B
    End Sub
#End Region

#Region "�C�x���g���������p���\�b�h"
    Protected Sub RegisterActiveOne( _
       ByVal oReqTeleg As IReqTelegram, _
       ByVal retryIntervalTicks As Integer, _
       ByVal limitNakCountToForget As Integer, _
       ByVal limitNakCountToCare As Integer, _
       ByVal sSeqName As String)

        If curState = State.NoConnection Then
            ProcOnActiveOneAnonyError(oReqTeleg)
            Return
        End If

        oActiveOneQueue.AddLast(New ActiveOne(oReqTeleg, retryIntervalTicks, limitNakCountToForget, limitNakCountToCare, sSeqName))
        If curState = State.Idling Then
            DoNextActiveSeq()
        End If
    End Sub

    Protected Sub RegisterActiveDll( _
       ByVal oXllReqTeleg As IXllReqTelegram, _
       ByVal retryIntervalTicks As Integer, _
       ByVal limitNakCountToForget As Integer, _
       ByVal limitNakCountToCare As Integer)

        If curState = State.NoConnection Then
            ProcOnActiveDllAnonyError(oXllReqTeleg)
            Return
        End If

        If Not oXllReqTeleg.IsHashValueReady Then
            oXllReqTeleg.UpdateHashValue()
        End if

        oActiveXllQueue.AddLast(New ActiveXll(XllDirection.Dll, oXllReqTeleg, retryIntervalTicks, limitNakCountToForget, limitNakCountToCare))
        If curActiveXllState = ActiveXllState.None Then
            TransitActiveXllState(ActiveXllState.BeforeFtp)
            If curState = State.Idling Then
                DoNextActiveSeq()
            End If
        End If
    End Sub

    Protected Sub RegisterActiveUll( _
       ByVal oXllReqTeleg As IXllReqTelegram, _
       ByVal retryIntervalTicks As Integer, _
       ByVal limitNakCountToForget As Integer, _
       ByVal limitNakCountToCare As Integer)

        If curState = State.NoConnection Then
            ProcOnActiveUllAnonyError(oXllReqTeleg)
            Return
        End If

        oActiveXllQueue.AddLast(New ActiveXll(XllDirection.Ull, oXllReqTeleg, retryIntervalTicks, limitNakCountToForget, limitNakCountToCare))
        If curActiveXllState = ActiveXllState.None Then
            TransitActiveXllState(ActiveXllState.BeforeFtp)
            If curState = State.Idling Then
                DoNextActiveSeq()
            End If
        End If
    End Sub

    'NOTE: curState��Idling�ɂȂ����ۂɌĂԂׂ����\�b�h�B
    Protected Sub DoNextActiveSeq()
        If isPendingWatchdog Then
            isPendingWatchdog = False

            'NOTE: �󋵂ɉ����ăE�H�b�`�h�b�O�̗L����d�����e��ύX����ނ�
            '�v���g�R����z�肵�A�ܑ̂Ȃ�������쐬���Ȃ������Ƃɂ��Ă���B
            oWatchdogReqTeleg = CreateWatchdogReqTelegram()
            If oWatchdogReqTeleg IsNot Nothing Then
                Log.Info("Sending Watchdog REQ...")
                If SendReqTelegram(oWatchdogReqTeleg) = False Then
                    Disconnect()
                    Return
                End If

                TransitState(State.WaitingForReply)
                oLastSentReqTeleg = oWatchdogReqTeleg
                oReplyLimitTimer.Renew(oWatchdogReqTeleg.ReplyLimitTicks)
                RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
                Return
            End If
        End If

        If enableActiveSeqStrongExclusion AndAlso _
           curActiveXllState = ActiveXllState.Ftp Then
            '���s����ActiveXll�ɂ����ActiveOne�̂��߂̒ʐM���֎~����Ă���
            '��Ԃł���B���̏�Ԃł́A������҂��Ă���ActiveOne�͊�{�I��
            '�S�ď����\�ł���i���s�񐔂𑝐i�����邾���ł��邽�߁j�B

            'NOTE: curActiveXllState����L�̏ꍇ�����łȂ��ABeforeFtp�̏ꍇ���A
            '�\���I�t�@�C���]���V�[�P���X�̓]���J�nREQ�d�������M�ς݂Ƃ���
            '�\���͂���i���肪ACK�d���𑗐M���Ă���\�����l������ƁA
            '�\���I�P���V�[�P���X�Ɣr���I�ȏ�ԂƂ݂Ȃ���j�B
            '�������A���̂悤�ȏ�Ԃł���Ȃ�΁A���̃��\�b�h�͌Ăяo����Ȃ�
            '�͂��ł���i���̃��\�b�h�́AIdling��Ԃł̂݌Ăяo�����j�B

            If enableActiveOneOrdering Then
                '�\���I�P���V�[�P���X�����������[�h�̏ꍇ�ł���B���̃��[�h
                '�ł́A������҂��Ă���ActiveOne�̂����A�܂��J�n���Ă��Ȃ�
                '���̂ɂ��ẮA���s����ActiveOne���Ȃ��ꍇ�ɂ̂݊J�n����
                '�悤�A��������K�v������B

                '���s����ActiveOne�̌����𒲂ׂ�B
                Dim executingActiveOneCount As Integer = 0
                For Each oOne As ActiveOne In oActiveOneQueue
                    If oOne.CurTryCount <> 0 Then
                        executingActiveOneCount += 1
                    End If
                Next oOne

                '�đ��M�y���f�B���O��ԂɂȂ��Ă���S�Ă�ActiveOne����������B
                'NOTE: �\���I�P���V�[�P���X�����������[�h�ł����Ă��A���̂悤��
                '�ߋ��Ɂi�\���I�P���V�[�P���X�����������[�h�łȂ������Ƃ��Ɂj
                '��x�ł����M����ActiveOne�ɂ��ẮA�\�ł���΁iIdling���
                '����������́j�����ł�����������j�ł���B
                'NOTE: �����̌��ʁA���s���łȂ��Ȃ������̂́A���ׂ���������
                '���������B
                While oActiveOneRetryPendingQueue.Count <> 0
                    Dim oOne As ActiveOne = oActiveOneRetryPendingQueue.Dequeue()
                    Log.Info(oOne.SeqName & " is regulated by ActiveXll.")
                    'NOTE: REQ�d���𑗐M����NAK�i�r�W�[�j�d������M�����ꍇ��
                    '�����̏������s���B
                    'NOTE: oActiveOneRetryPendingQueue�ɓo�^���Ă���ActiveOne��
                    '�S�Ď��s���ł���ioOne.CurTryCount�͊���1�ȏ�ł���j
                    '���߁A�����ł́AexecutingActiveOneCount�̃C���N�������g��
                    '�l������K�v���͂Ȃ��B
                    oOne.CurTryCount += 1
                    oOne.CurNakCountToCare += 1
                    If oOne.CurNakCountToCare >= oOne.LimitNakCountToCare Then
                        Log.Warn(oOne.SeqName & " retry over.")
                        ProcOnActiveOneRetryOverToCare(oOne.ReqTeleg, Nothing)
                        oActiveOneQueue.Remove(oOne)
                        executingActiveOneCount -= 1
                    Else
                        RegisterTimer(oOne.RetryTimer, TickTimer.GetSystemTick())
                    End If
                End While

                '���s����ActiveOne���Ȃ��ꍇ�̂݁A
                '�V���ɓo�^���ꂽActiveOne���J�n����B
                While executingActiveOneCount = 0 AndAlso oActiveOneQueue.Count <> 0
                    Dim oOne As ActiveOne = oActiveOneQueue.First.Value
                    Log.Info(oOne.SeqName & " is regulated by ActiveXll.")
                    'NOTE: REQ�d���𑗐M����NAK�i�r�W�[�j�d������M�����ꍇ��
                    '�����̏������s���B
                    'NOTE: ���s����ActiveOne��������킯�ł��邪�A�����
                    '���s���łȂ��Ȃ�i���g���C�I�[�o�[����j�\��������
                    '���߁AexecutingActiveOneCount�̃C���N�������g�́A
                    '���������ɕK�v�ɉ����čs���B
                    Debug.Assert(oOne.CurTryCount = 0)
                    oOne.CurTryCount += 1
                    oOne.CurNakCountToCare += 1
                    If oOne.CurNakCountToCare >= oOne.LimitNakCountToCare Then
                        Log.Warn(oOne.SeqName & " retry over.")
                        ProcOnActiveOneRetryOverToCare(oOne.ReqTeleg, Nothing)
                        oActiveOneQueue.Remove(oOne)
                    Else
                        RegisterTimer(oOne.RetryTimer, TickTimer.GetSystemTick())
                        executingActiveOneCount += 1
                        'NOTE: While���甲���邱�ƂɂȂ�͂��ł���B
                    End If
                End While
            Else
                '�\���I�P���V�[�P���X�����������[�h�łȂ��ꍇ�ł���B

                '�đ��M�y���f�B���O��ԂɂȂ��Ă���S�Ă�ActiveOne����������B
                While oActiveOneRetryPendingQueue.Count <> 0
                    Dim oOne As ActiveOne = oActiveOneRetryPendingQueue.Dequeue()
                    Log.Info(oOne.SeqName & " is regulated by ActiveXll.")
                    'NOTE: REQ�d���𑗐M����NAK�i�r�W�[�j�d������M�����ꍇ��
                    '�����̏������s���B
                    oOne.CurTryCount += 1
                    oOne.CurNakCountToCare += 1
                    If oOne.CurNakCountToCare >= oOne.LimitNakCountToCare Then
                        Log.Warn(oOne.SeqName & " retry over.")
                        ProcOnActiveOneRetryOverToCare(oOne.ReqTeleg, Nothing)
                        oActiveOneQueue.Remove(oOne)
                    Else
                        RegisterTimer(oOne.RetryTimer, TickTimer.GetSystemTick())
                    End If
                End While

                '�V���ɓo�^���ꂽActiveOne������ΑS�ĊJ�n����B
                While oActiveOneQueue.Count <> 0
                    Dim oOne As ActiveOne = oActiveOneQueue.First.Value
                    If oOne.CurTryCount = 0 Then
                        Log.Info(oOne.SeqName & " is regulated by ActiveXll.")
                        'NOTE: REQ�d���𑗐M����NAK�i�r�W�[�j�d������M�����ꍇ��
                        '�����̏������s���B
                        oOne.CurTryCount += 1
                        oOne.CurNakCountToCare += 1
                        If oOne.CurNakCountToCare >= oOne.LimitNakCountToCare Then
                            Log.Warn(oOne.SeqName & " retry over.")
                            ProcOnActiveOneRetryOverToCare(oOne.ReqTeleg, Nothing)
                            oActiveOneQueue.Remove(oOne)
                        Else
                            RegisterTimer(oOne.RetryTimer, TickTimer.GetSystemTick())
                        End If
                    End If
                End While
            End If
        Else
            'ActiveOne�̂��߂̒ʐM���֎~����Ă��Ȃ���Ԃł���B���̏�Ԃł́A
            '������҂��Ă���ActiveOne�̂����A�ł��D�悷�ׂ����̂̂ݏ����\
            '�ł���iREQ�d���𑗐M���邱�ƂŁAIdling��ԂłȂ��Ȃ邽�߁j�B

            If enableActiveOneOrdering Then
                '�\���I�P���V�[�P���X�����������[�h�̏ꍇ�ł���B���̃��[�h
                '�ł́A�܂��J�n���Ă��Ȃ�ActiveOne�ɂ��ẮA���Ƃ��A���ꂪ
                '�ł��D�悷�ׂ����̂ł���Ƃ��Ă��A���s����ActiveOne�������
                '�J�n�i���M�j���邱�Ƃ͂ł��Ȃ��B

                '�đ��M�y���f�B���O��Ԃ�ActiveOne������΁A�ł��ߋ��ɍđ��M
                '�y���f�B���O��ԂɂȂ������̂𑗐M�ΏۂƂ���B
                '�Ȃ���΁A�V���ɓo�^���ꂽActiveOne�̂����A�ł��ߋ��ɓo�^���ꂽ
                '���̂𑗐M�ΏۂƂ���B
                Dim oOne As ActiveOne = Nothing
                If oActiveOneRetryPendingQueue.Count <> 0 Then
                    'NOTE: �\���I�P���V�[�P���X�����������[�h�ł����Ă��A
                    'oActiveOneRetryPendingQueue�̗v�f�̂悤�ɉߋ��Ɂi�\���I
                    '�P���V�[�P���X�����������[�h�łȂ������Ƃ��Ɂj��x�ł�
                    '���M����ActiveOne�ɂ��ẮA�\�ł���΁iIdling��Ԃ�
                    '��������́j�����ł�����������j�ł���B����āA���L��
                    '�擾����ActiveOne�ɂ��ẮA���s����ActiveOne�̗L����
                    '�֌W�Ȃ��A��������K�v������B
                    oOne = oActiveOneRetryPendingQueue.Dequeue()
                Else
                    '���s����ActiveOne�̗L���𒲂ׂ�B
                    Dim isThereExecutingActiveOne As Boolean = False
                    For Each oQueuingOne As ActiveOne In oActiveOneQueue
                        If oQueuingOne.CurTryCount <> 0 Then
                            isThereExecutingActiveOne = True
                            Exit For
                        End If
                    Next oQueuingOne

                    '���s����ActiveOne���Ȃ��ꍇ�̂݁A
                    '�V���ɓo�^���ꂽActiveOne�𑗐M�ΏۂƂ���B
                    If Not isThereExecutingActiveOne Then
                        For Each oQueuingOne As ActiveOne In oActiveOneQueue
                            If oQueuingOne.CurTryCount = 0 Then
                                oOne = oQueuingOne
                                Exit For
                            End If
                        Next oQueuingOne
                    End If
                End If

                '���M�ΏۂɑI��ActiveOne�𑗐M����B
                If oOne IsNot Nothing Then
                    Log.Info("Sending " & oOne.SeqName & " REQ...")
                    oOne.CurTryCount += 1
                    If SendReqTelegram(oOne.ReqTeleg) = False Then
                        Disconnect()
                        Return
                    End If

                    TransitState(State.WaitingForReply)
                    oLastSentReqTeleg = oOne.ReqTeleg
                    oLastSentActiveOne = oOne
                    oReplyLimitTimer.Renew(oOne.ReqTeleg.ReplyLimitTicks)
                    RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
                    Return
                End If
            Else
                '�\���I�P���V�[�P���X�����������[�h�łȂ��ꍇ�ł���B

                '�đ��M�y���f�B���O��Ԃ�ActiveOne������΁A�ł��ߋ��ɍđ��M
                '�y���f�B���O��ԂɂȂ������̂𑗐M�ΏۂƂ���B
                '�Ȃ���΁A�V���ɓo�^���ꂽActiveOne�̂����A�ł��ߋ��ɓo�^���ꂽ
                '���̂𑗐M�ΏۂƂ���B
                Dim oOne As ActiveOne = Nothing
                If oActiveOneRetryPendingQueue.Count <> 0 Then
                    oOne = oActiveOneRetryPendingQueue.Dequeue()
                Else
                    For Each oQueuingOne As ActiveOne In oActiveOneQueue
                        If oQueuingOne.CurTryCount = 0 Then
                            oOne = oQueuingOne
                            Exit For
                        End If
                    Next oQueuingOne
                End If

                '���M�ΏۂɑI��ActiveOne�𑗐M����B
                If oOne IsNot Nothing Then
                    Log.Info("Sending " & oOne.SeqName & " REQ...")
                    oOne.CurTryCount += 1
                    If SendReqTelegram(oOne.ReqTeleg) = False Then
                        Disconnect()
                        Return
                    End If

                    TransitState(State.WaitingForReply)
                    oLastSentReqTeleg = oOne.ReqTeleg
                    oLastSentActiveOne = oOne
                    oReplyLimitTimer.Renew(oOne.ReqTeleg.ReplyLimitTicks)
                    RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
                    Return
                End If
            End If
        End If

        If oActiveXllQueue.Count <> 0 Then
            Dim oXll As ActiveXll = oActiveXllQueue.First.Value
            If oXll.CurTryCount = 0 OrElse isPendingActiveXllRetry Then
                isPendingActiveXllRetry = False
                'NOTE: ���̃��\�b�h��Idling��Ԃł̂݌Ă΂�邽�߁A
                '���Ƃ��\���I�V�[�P���X�r���������[�h�ł����Ă��A
                '�\���I�P���V�[�P���X�̎��s��Ԃ��C�ɂ���K�v�͂Ȃ��B
                If enableXllStrongExclusion AndAlso _
                   curPassiveXllState = PassiveXllState.Ftp Then
                    'NOTE: �V�[�P���X�J�n�O�̔\���I�d�������҂��̊Ԃ�
                    '���g���C�C���^�[�o���{���g���C�̔\���I�d�������҂��̊Ԃ�
                    '�N���C�A���g����PassiveXll�̓]���J�nREQ�d������M���A
                    '������󂯕t���Ă����ꍇ�ł���B
                    Log.Info("ActiveXll is regulated by PassiveXll.")
                    'NOTE: REQ�d���𑗐M����NAK�i�r�W�[�j�d������M�����ꍇ��
                    '�����̏������s���B
                    oXll.CurTryCount += 1
                    oXll.CurNakCountToCare += 1
                    If oXll.CurNakCountToCare >= oXll.LimitNakCountToCare Then
                        If oXll.Direction = XllDirection.Dll Then
                            Log.Warn("ActiveDll retry over.")
                            ProcOnActiveDllRetryOverToCare(oXll.ReqTeleg, Nothing)
                        Else
                            Log.Warn("ActiveUll retry over.")
                            ProcOnActiveUllRetryOverToCare(oXll.ReqTeleg, Nothing)
                        End If
                        oActiveXllQueue.RemoveFirst()
                        UpdateActiveXllStateAfterDequeue()
                    Else
                        oActiveXllRetryTimer.Renew(oXll.RetryIntervalTicks)
                        RegisterTimer(oActiveXllRetryTimer, TickTimer.GetSystemTick())
                    End If
                Else
                    If oXll.Direction = XllDirection.Dll Then
                        Log.Info("Sending ActiveDllStart REQ...")
                    Else
                        Log.Info("Sending ActiveUllStart REQ...")
                    End If
                    oXll.CurTryCount += 1
                    If SendReqTelegram(oXll.ReqTeleg) = False Then
                        Disconnect()
                        Return
                    End If

                    TransitState(State.WaitingForReply)
                    oLastSentReqTeleg = oXll.ReqTeleg
                    oReplyLimitTimer.Renew(oXll.ReqTeleg.ReplyLimitTicks)
                    RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
                    Return
                End If
            End If
        End If
    End Sub

    'NOTE: ��M�d���Ɉُ킪����ꍇ�́A���̓d�������̃V�[�P���X�̏I����������
    '�d���ł���\����z�肷��ׂ��ł���B���Ȃ킿�A�R�l�N�V�������I�������āA
    '�L���[�C���O���Ă���V�[�P���X���A�R�l�N�V�����ɏ]�����郊�\�[�X���������
    '�ׂ��ł���B����āANAK���R�ɓ�������E����Ȃ��悤�A���̃��\�b�h�ŁA
    'NAK�d���̕ԐM�ƃR�l�N�V�����I�����s�����j�Ƃ���B
    '�t�ɁA�������d������M������A����̓����s�����ɏ]����NAK�d����ԐM����
    '�ꍇ�́ANAK���R�ɂ���ăR�l�N�V���������̗L�������܂�悤�A���̃��\�b�h
    '�̎g�p�͍T������j�Ƃ���B
    Protected Sub SendNakTelegramThenDisconnect(ByVal cause As NakCauseCode, ByVal oSourceTeleg As ITelegram)
        Dim oReplyTeleg As ITelegram = oSourceTeleg.CreateNakTelegram(cause)
        If oReplyTeleg IsNot Nothing Then
            Log.Info("Sending NAK (" & cause.ToString() & ") telegram...")
            SendReplyTelegram(oReplyTeleg, oSourceTeleg)
            '��L�Ăяo���̖߂�l�͖�������i���̌�̏����ɍ��ق��Ȃ����߁j�B
        End If
        Disconnect()
    End Sub

    Protected Sub Connect(ByVal oNewTelegSock As Socket)
        Debug.Assert(curState = State.NoConnection)

        oTelegSock = oNewTelegSock
        RegisterSocket(oTelegSock)
        TransitState(State.Idling)
        oLastSentReqTeleg = Nothing

        ProcOnConnectionAppear()
    End Sub

    'NOTE: ProcOnActiveDllXxxx���̃��\�b�h�i�e�V�[�P���X���I������ۂ̋Ɩ��ˑ��������������邽�߂̃��\�b�h�j
    '����͌Ăяo���֎~�ł���B�����̃��\�b�h�͒ʐM����̒��ŌĂ΂��t�b�N�ł���B��������ς���ƁA
    'Disconnect���\�b�h�́A����炩��ĂԂ��߂̃��\�b�h�ł͂Ȃ��A�������Ăяo�����̃��\�b�h�ł���B
    '�����̃��\�b�h�̒��ōs���������I�Ȕ���Ɋ�Â��ăR�l�N�V�������I��������ɂ́A
    'ProcOnReqTelegramReceiveCompleteBySendXxx����False��Ԃ��悤�ɁA�����̃��\�b�h�̒���
    '�C���X�^���X�̓�����Ԃ�ύX����̂��A���z�I�ł���B
    Protected Sub Disconnect()
        'NOTE: ���̃��\�b�h�ɂ����ẮAoActiveXllQueue�̐擪�v�f���ω�����
        '���т�TransitActiveXllState()���Ăяo���킯�ł͂Ȃ��B
        '�܂�ATransitActiveXllState()���ōs�������́A�����܂ŁA
        '����Looper���P�̃C�x���g�����������ł́u�����O��v�̏�ԕω���
        '�����Ď��{���ׂ������i�킩��₷�������ƁA�C�x���g�ҋ@���@�Ɋւ���
        '�ݒ�ύX�j�݂̂Ɍ��肷��ׂ��ł���B

        UnregisterSocket(oTelegSock)
        Log.Info("Closing current socket...")
        Try
            'NOTE: ���ݑz�肵�Ă���v���g�R���ł͒v���I�ł͂Ȃ����A
            '���������\�P�b�g�ɏ������񂾉����f�[�^�����M����Ȃ�
            '�̂̓C�}�C�`�Ǝv���邽�߁A��������{���Ă���B
            '�܂��A���̎��_�œ͂��Ă����f�[�^����������A���̌��
            '�͂����f�[�^������΁A�����ǂ܂Ȃ����Ƃ�m�点�邽�߂ɁA
            '�����RST�̑��M�����݂�B�������A���̂悤�Ȃ��Ƃɗ���
            '�v���g�R���́A���̃N���X�̎���͈͊O�ł���B
            oTelegSock.Shutdown(SocketShutdown.Both)
        Catch ex As SocketException
            Log.Error("SocketException caught.", ex)
        End Try
        oTelegSock.Close()
        oTelegSock = Nothing
        UnregisterConnectionDependentTimers()
        ProcOnConnectionDisappear()

        'NOTE: �L���[�Ɏc���Ă���̂́A�S�����s���Ă��Ȃ������i�J�n�O�́j
        '�V�[�P���X�ł��邩�A���s���̃V�[�P���X�ł����Ă��A���Y�V�[�P���X��
        '�d�������≞����M�^�C���A�E�g���̈�ʓI�Ȉُ킪�������Ē��~����
        '�ꍇ�ł��邩�A�E�H�b�`�h�b�O���̕ʂ̃V�[�P���X�ŔF�������ُ�ɂ��
        '���~���ꂽ�ꍇ�����ł���B���Y�V�[�P���X�̃��g���C�I�[�o����������
        '�ꍇ�Ȃǂ́A���̏��Dequeue���āA�K�؂ȃt�b�N���Ăяo���Ă���͂�
        '�ł���B

        'NOTE: ����V�[�P���X�ňُ킪���������ہA�ҋ@���Ă����\���I��
        '�z�M����W�̃V�[�P���X�Ɋւ��Ă��A���s�Ƃ���B�^�p��AAnonyError
        '�Ƃ݂Ȃ��͔̂�����������Ȃ����A���ɂ͓K���Ă���͂��B

        For Each oOne As ActiveOne In oActiveOneQueue
            ProcOnActiveOneAnonyError(oOne.ReqTeleg)
        Next oOne
        oActiveOneQueue.Clear()
        oLastSentActiveOne = Nothing

        For Each oXll As ActiveXll In oActiveXllQueue
            If oXll.Direction = XllDirection.Dll Then
                ProcOnActiveDllAnonyError(oXll.ReqTeleg)
            Else
                ProcOnActiveUllAnonyError(oXll.ReqTeleg)
            End If
        Next oXll
        oActiveXllQueue.Clear()
        TransitActiveXllState(ActiveXllState.None)

        For Each oXll As PassiveXll In oPassiveXllQueue
            If oXll.Direction = XllDirection.Dll Then
                ProcOnPassiveDllAnonyError(oXll.ReqTeleg)
            Else
                ProcOnPassiveUllAnonyError(oXll.ReqTeleg)
            End If
        Next oXll
        oPassiveXllQueue.Clear()
        TransitPassiveXllState(PassiveXllState.None)

        TransitState(State.NoConnection)
        isPendingWatchdog = False
        oActiveOneRetryPendingQueue.Clear()
        isPendingActiveXllRetry = False
        oLastSentReqTeleg = Nothing
    End Sub

    Protected Sub UpdateActiveXllStateAfterDequeue()
        If oActiveXllQueue.Count = 0 Then
            TransitActiveXllState(ActiveXllState.None)
        Else
            TransitActiveXllState(ActiveXllState.BeforeFtp)
        End If
    End Sub

    Protected Sub UpdatePassiveXllStateAfterDequeue()
        If oPassiveXllQueue.Count = 0 Then
            TransitPassiveXllState(PassiveXllState.None)
        Else
            Debug.Fail("This case is impermissible.")
        End If
    End Sub

    Protected Overridable Sub TransitState(ByVal nextState As State)
        If IsParentMessageReceptibleState(curActiveXllState) AndAlso _
           IsParentMessageReceptibleState(curPassiveXllState) Then
            If IsParentMessageReceptibleState(curState) Then
                If Not IsParentMessageReceptibleState(nextState) Then
                    UnregisterSocket(oParentMessageSock)
                End If
            Else
                If IsParentMessageReceptibleState(nextState) Then
                    RegisterSocket(oParentMessageSock)
                End If
            End If
        Else
            '���̏ꍇ�AoParentMessageSock�͓o�^����Ă��Ȃ��͂��ł���A
            'nextState�����ł��邩�Ɋ֌W�Ȃ��A�o�^����ׂ��ł͂Ȃ��B
        End If
        curState = nextState
    End Sub

    Protected Overridable Sub TransitActiveXllState(ByVal nextState As ActiveXllState)
        If IsParentMessageReceptibleState(curState) AndAlso _
           IsParentMessageReceptibleState(curPassiveXllState) Then
            If IsParentMessageReceptibleState(curActiveXllState) Then
                If Not IsParentMessageReceptibleState(nextState) Then
                    UnregisterSocket(oParentMessageSock)
                End If
            Else
                If IsParentMessageReceptibleState(nextState) Then
                    RegisterSocket(oParentMessageSock)
                End If
            End If
        Else
            '���̏ꍇ�AoParentMessageSock�͓o�^����Ă��Ȃ��͂��ł���A
            'nextState�����ł��邩�Ɋ֌W�Ȃ��A�o�^����ׂ��ł͂Ȃ��B
        End If
        curActiveXllState = nextState
    End Sub


    Protected Overridable Sub TransitPassiveXllState(ByVal nextState As PassiveXllState)
        If IsParentMessageReceptibleState(curState) AndAlso _
           IsParentMessageReceptibleState(curActiveXllState) Then
            If IsParentMessageReceptibleState(curPassiveXllState) Then
                If Not IsParentMessageReceptibleState(nextState) Then
                    UnregisterSocket(oParentMessageSock)
                End If
            Else
                If IsParentMessageReceptibleState(nextState) Then
                    RegisterSocket(oParentMessageSock)
                End If
            End If
        Else
            '���̏ꍇ�AoParentMessageSock�͓o�^����Ă��Ȃ��͂��ł���A
            'nextState�����ł��邩�Ɋ֌W�Ȃ��A�o�^����ׂ��ł͂Ȃ��B
        End If
        curPassiveXllState = nextState
    End Sub

    'NOTE: Disconnect���s���ׂ��󋵂ɂȂ����ꍇ��False��ԋp���邱�ƂɂȂ��Ă���B
    'NOTE: ���̃��\�b�h��Protected�Ȃ̂́A�h���N���X�Łu�I�[�o�[���C�h����v���Ƃ�z�肵�Ă��邽�߂ł���B
    '���̃��\�b�h���Ă񂾍ۂ́ATransitState�AoLastSentReqTeleg�X�V�AoReplyLimitTimer�̓o�^�Ȃǂ�
    '�s���K�v�����邽�߁A�h���N���X�Ŗ��ÂɁu�Ăяo���v�ׂ��ł͂Ȃ��BREQ�d���̑��M���s�������ꍇ�́A
    'RegisterActiveOne()�ARegisterActiveDll()�ARegisterActiveUll()�����s����̂��Ó��ł���B
    Protected Overridable Function SendReqTelegram(ByVal oReqTeleg As IReqTelegram) As Boolean
        Return oReqTeleg.WriteToSocket(oTelegSock, telegWritingLimitBaseTicks, telegWritingLimitExtraTicksPerMiB, telegLoggingMaxLengthOnWrite)
    End Function

    'NOTE: Disconnect���s���ׂ��󋵂ɂȂ����ꍇ��False��ԋp���邱�ƂɂȂ��Ă���B
    'NOTE: �I�[�o���C�h����ꍇ�AoSourceTeleg�̃w�b�_���ɏ����ᔽ������\����
    '���ӂ��Ă��������B�o�C�g���ȂǁATelegramImporter.GetTelegramFromSocket()��
    '�ۏ؂��邱�Ƃ͕ۏ؂���܂��B
    Protected Overridable Function SendReplyTelegram(ByVal oReplyTeleg As ITelegram, ByVal oSourceTeleg As ITelegram) As Boolean
        If oReplyTeleg.WriteToSocket(oTelegSock, telegWritingLimitBaseTicks, telegWritingLimitExtraTicksPerMiB, telegLoggingMaxLengthOnWrite) = False Then
            Return False
        End If

        Dim cmdKind As CmdKind = oReplyTeleg.CmdKind
        If cmdKind = CmdKind.Ack Then
            Return ProcOnReqTelegramReceiveCompleteBySendAck(oSourceTeleg, oReplyTeleg)
        ElseIf cmdKind = CmdKind.Nak Then
            If GetRequirement(DirectCast(oReplyTeleg, INakTelegram)) = NakRequirement.DisconnectImmediately Then
                Return False
            End If
            Return ProcOnReqTelegramReceiveCompleteBySendNak(oSourceTeleg, oReplyTeleg)
        Else
            Debug.Fail("This case is impermissible.")
            Return False
        End If
    End Function

    Protected Overridable Sub UnregisterConnectionDependentTimers()
        UnregisterTimer(oReplyLimitTimer)

        For Each oOne As ActiveOne In oActiveOneQueue
            UnregisterTimer(oOne.RetryTimer)
        Next oOne

        UnregisterTimer(oActiveXllRetryTimer)

        UnregisterTimer(oActiveXllLimitTimer)

        UnregisterTimer(oPassiveXllLimitTimer)
    End Sub
#End Region

#Region "�\�w�@�\�J�X�^�}�C�Y�p���z���\�b�h"
    '�E�H�b�`�h�b�O�V�[�P���X��REQ�d���𐶐����郁�\�b�h
    'NOTE: �E�H�b�`�h�b�O�V�[�P���X���s�v�ȏꍇ�́ANothing��ԋp���邱�ƁB
    Protected Overridable Function CreateWatchdogReqTelegram() As IReqTelegram
        Return Nothing
    End Function

    '�\���I�P���V�[�P���X�����������ꍇ
    'NOTE: oReqTeleg�́ARegisterActiveOne�ɓn�������̂ł���B
    'NOTE: oAckTeleg�́AoReqTeleg.ParseAsAck�Ő����������̂ł���B
    Protected Overridable Sub ProcOnActiveOneComplete(ByVal oReqTeleg As IReqTelegram, ByVal oAckTeleg As ITelegram)
    End Sub

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    'NOTE: oReqTeleg�́ARegisterActiveOne�ɓn�������̂ł���B
    'NOTE: oNakTeleg�́AoReqTeleg.ParseAsNak(oRcvTeleg)�Ő��������I�u�W�F�N�g�ł���B
    'oRcvTeleg�́A���g���C�I�[�o�[�̔���Ɏ������ۂɎ�M�����d���ł���B
    Protected Overridable Sub ProcOnActiveOneRetryOverToForget(ByVal oReqTeleg As IReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    'NOTE: oReqTeleg�́ARegisterActiveOne�ɓn�������̂ł���B
    'NOTE: oNakTeleg�́AoReqTeleg.ParseAsNak(oRcvTeleg)�Ő��������I�u�W�F�N�g�܂���Nothing�ł���B
    'oRcvTeleg�́A���g���C�I�[�o�[�̔���Ɏ������ۂɎ�M�����d���ł���B
    'oNakTeleg��Nothing�ɂȂ�̂́AREQ�d���𑗐M����܂ł��Ȃ����߂��ꍇ�ł���A
    'EnableActiveSeqStrongExclusion��True�̏ꍇ�ɂ݂̂��蓾��B
    Protected Overridable Sub ProcOnActiveOneRetryOverToCare(ByVal oReqTeleg As IReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '�\���I�P���V�[�P���X�̍Œ���L���[�C���O���ꂽ�\���I�P���V�[�P���X�̎��{�O�ɒʐM�ُ�����o�����ꍇ
    'NOTE: oReqTeleg�́ARegisterActiveOne�ɓn�������̂ł���B
    Protected Overridable Sub ProcOnActiveOneAnonyError(ByVal oReqTeleg As IReqTelegram)
    End Sub

    '�\���IDLL�����������iContinueCode.Finish�̓]���I��REQ�d������M�����j�ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveDll�ɓn�������̂���ParseAsSameKind�Ő����������̂ł���B
    Protected Overridable Sub ProcOnActiveDllComplete(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�\���IDLL�����������iContinueCode.FinishWithoutStoring�̓]���I��REQ�d������M�����j�ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveDll�ɓn�������̂���ParseAsSameKind�Ő����������̂ł���B
    Protected Overridable Sub ProcOnActiveDllCompleteWithoutStoring(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�\���IDLL�ɂăN���C�A���g����]�����s�܂��͓]��������ʒm���ꂽ�iContinueCode.Abort�̓]���I��REQ�d������M�����j�ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveDll�ɓn�������̂���ParseAsSameKind�Ő����������̂ł���B
    Protected Overridable Sub ProcOnActiveDllAbort(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�\���IDLL�ɂē]���I��REQ�d���҂��̃^�C���A�E�g�����������ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveDll�ɓn�������̂ł���B
    'NOTE: �v���g�R����A���̎��_�ŃN���C�A���g���t�@�C���̎擾���p�����Ă��邱�Ƃ͐�����
    '���Ƃł���A�N���C�A���g�́A�n�b�V���l�̃`�F�b�N�ňُ킪���o�ł��Ȃ���΁A�擾����
    '�t�@�C�������̂܂ܕۑ�����i�z�M����j�͂��ł���B����āA���L�̃��\�b�h�ł�DLL�ΏۂƂȂ�
    'FTP�T�[�o��̃t�@�C�����폜����ׂ��ł͂Ȃ��B�폜����̂ł���΁A���Y�N���C�A���g����
    '�V���Ȑڑ��v�����������Ƃ��ɍ폜����̂��A�]�܂����B
    Protected Overridable Sub ProcOnActiveDllTimeout(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�\���IDLL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveDll�ɓn�������̂ł���B
    'NOTE: oNakTeleg�́AoXllReqTeleg.ParseAsNak(oRcvTeleg)�Ő��������I�u�W�F�N�g�ł���B
    'oRcvTeleg�́A���g���C�I�[�o�[�̔���Ɏ������ۂɎ�M�����d���ł���B
    Protected Overridable Sub ProcOnActiveDllRetryOverToForget(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '�\���IDLL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveDll�ɓn�������̂ł���B
    'NOTE: oNakTeleg�́AoXllReqTeleg.ParseAsNak(oRcvTeleg)�Ő��������I�u�W�F�N�g�܂���Nothing�ł���B
    'oRcvTeleg�́A���g���C�I�[�o�[�̔���Ɏ������ۂɎ�M�����d���ł���B
    'oNakTeleg��Nothing�ɂȂ�̂́AREQ�d���𑗐M����܂ł��Ȃ����߂��ꍇ�ł���A
    'EnableActiveSeqStrongExclusion�܂���EnableXllStrongExclusion��True�̏ꍇ�ɂ݂̂��蓾��B
    Protected Overridable Sub ProcOnActiveDllRetryOverToCare(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '�\���IDLL�̍Œ���L���[�C���O���ꂽ�\���IDLL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveDll�ɓn�������̂ł���B
    'NOTE: �v���g�R����A���̎��_�ŃN���C�A���g���t�@�C���̎擾���p�����Ă��邱�Ƃ͐�����
    '���Ƃł���A�N���C�A���g�́A�n�b�V���l�̃`�F�b�N�ňُ킪���o�ł��Ȃ���΁A�擾����
    '�t�@�C�������̂܂ܕۑ�����i�z�M����j�͂��ł���B����āA���L�̃��\�b�h�ł�DLL�ΏۂƂȂ�
    'FTP�T�[�o��̃t�@�C�����폜����ׂ��ł͂Ȃ��B�폜����̂ł���΁A���Y�N���C�A���g����
    '�V���Ȑڑ��v�����������Ƃ��ɍ폜����̂��A�]�܂����B
    Protected Overridable Sub ProcOnActiveDllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�\���IULL�����������i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e���������Ă��邱�Ƃ��m�F�����j
    '�i�]���I��REQ�d���ɑ΂�ACK�d����ԐM���邱�ƂɂȂ�j�ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveUll�ɓn�������̂���ParseAsSameKind�Ő����������̂ł���B
    'NOTE: �󂯓���s�\�ȓ��e�ł����NakCauseCode.InvalidContent�Ȃǂ�ԋp���邱�ƁB
    Protected Overridable Function ProcOnActiveUllComplete(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '�\���IULL�ɂē]�����������o�����i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e�ɕs���������o�����j
    '�i�]���I��REQ�d���ɑ΂��n�b�V���l�̕s��v������NAK�d����ԐM���邱�ƂɂȂ�j�ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveUll�ɓn�������̂���ParseAsSameKind�Ő����������̂ł���B
    'NOTE: �v���g�R���ɉ����āANakCauseCode.TelegramError���p��NAK�𐶂ݏo�����߂�NakCauseCode��ԋp���邱�ƁB
    Protected Overridable Function ProcOnActiveUllHashValueError(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '�\���IULL�ɂăN���C�A���g����]�����s��ʒm���ꂽ�iContinueCode.Abort�̓]���I��REQ�d������M�����j�ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveUll�ɓn�������̂���ParseAsSameKind�Ő����������̂ł���B
    Protected Overridable Sub ProcOnActiveUllAbort(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�\���IULL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveUll�ɓn�������̂ł���B
    'NOTE: oNakTeleg�́AoXllReqTeleg.ParseAsNak(oRcvTeleg)�Ő��������I�u�W�F�N�g�ł���B
    'oRcvTeleg�́A���g���C�I�[�o�[�̔���Ɏ������ۂɎ�M�����d���ł���B
    Protected Overridable Sub ProcOnActiveUllRetryOverToForget(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '�\���IULL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveUll�ɓn�������̂ł���B
    'NOTE: oNakTeleg�́AoXllReqTeleg.ParseAsNak(oRcvTeleg)�Ő��������I�u�W�F�N�g�܂���Nothing�ł���B
    'oRcvTeleg�́A���g���C�I�[�o�[�̔���Ɏ������ۂɎ�M�����d���ł���B
    'oNakTeleg��Nothing�ɂȂ�̂́AREQ�d���𑗐M����܂ł��Ȃ����߂��ꍇ�ł���A
    'EnableActiveSeqStrongExclusion�܂���EnableXllStrongExclusion��True�̏ꍇ�ɂ݂̂��蓾��B
    Protected Overridable Sub ProcOnActiveUllRetryOverToCare(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '�\���IULL�ɂē]���I��REQ�d���҂��̃^�C���A�E�g�����������ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveUll�ɓn�������̂ł���B
    Protected Overridable Sub ProcOnActiveUllTimeout(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�\���IULL�̍Œ���L���[�C���O���ꂽ�\���IULL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    'NOTE: oXllReqTeleg�́ARegisterActiveUll�ɓn�������̂ł���B
    Protected Overridable Sub ProcOnActiveUllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�w�b�_���̓��e���󓮓IDLL��REQ�d���̂��̂ł��邩���肷�郁�\�b�h
    'NOTE: oTeleg�́A���̃N���X��New�ɓn����TelegramImporter�������������̂ł���B
    'NOTE: �R�}���h��ʂ�REQ�ł��邱�Ƃ͊m�肵�Ă���B
    Protected Overridable Function IsPassiveDllReq(ByVal oTeleg As ITelegram) As Boolean
        Return False
    End Function

    '�n���ꂽ�d���C���X�^���X��K�؂Ȍ^�̃C���X�^���X�ɕϊ����郁�\�b�h
    'NOTE: oTeleg�́A���̃N���X��New�ɓn����TelegramImporter�������������̂ł���B
    'NOTE: �w�b�_���̓��e���󓮓IDLL��REQ�d���̂��̂ł��邱�Ƃ͊m�肵�Ă���B
    'NOTE: �ȍ~�̎󓮓IDLL�p���\�b�h�ɓn�����d���C���X�^���X�́A���̃��\�b�h�Ő�����������
    '�܂��́A���̃C���X�^���X��ParseAsSameKind���\�b�h�Ő����������̂ł���B
    'NOTE: GetBodyFormatViolation()�̎��s�́A�Ăяo����ɍs���̂ŕs�v�ł���B
    Protected Overridable Function ParseAsPassiveDllReq(ByVal oTeleg As ITelegram) As IXllReqTelegram
        Return Nothing
    End Function

    '�󓮓IDLL�̏����i�w�肳�ꂽ�t�@�C���̗p�Ӂj���s�����\�b�h
    'NOTE: �p�ӂ��ł��Ȃ����NakCauseCode.NoData��ԋp���邱�ƁB
    Protected Overridable Function PrepareToStartPassiveDll(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '�󓮓IDLL�����������iContinueCode.Finish�̓]���I��REQ�d������M�����j�ꍇ
    Protected Overridable Sub ProcOnPassiveDllComplete(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�󓮓IDLL�����������iContinueCode.FinishWithoutStoring�̓]���I��REQ�d������M�����j�ꍇ
    Protected Overridable Sub ProcOnPassiveDllCompleteWithoutStoring(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�󓮓IDLL�ɂăN���C�A���g����]�����s�܂��͓]��������ʒm���ꂽ�iContinueCode.Abort�̓]���I��REQ�d������M�����j�ꍇ
    Protected Overridable Sub ProcOnPassiveDllAbort(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�󓮓IDLL�ɂē]���I��REQ�d���҂��̃^�C���A�E�g�����������ꍇ
    Protected Overridable Sub ProcOnPassiveDllTimeout(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�󓮓IDLL�̍Œ���L���[�C���O���ꂽ�󓮓IDLL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overridable Sub ProcOnPassiveDllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�w�b�_���̓��e���󓮓IULL��REQ�d���̂��̂ł��邩���肷�郁�\�b�h
    'NOTE: oTeleg�́A���̃N���X��New�ɓn����TelegramImporter�������������̂ł���B
    'NOTE: �R�}���h��ʂ�REQ�ł��邱�Ƃ͊m�肵�Ă���B
    Protected Overridable Function IsPassiveUllReq(ByVal oTeleg As ITelegram) As Boolean
        Return False
    End Function

    '�n���ꂽ�d���C���X�^���X��K�؂Ȍ^�̃C���X�^���X�ɕϊ����郁�\�b�h
    'NOTE: oTeleg�́A���̃N���X��New�ɓn����TelegramImporter�������������̂ł���B
    'NOTE: �w�b�_���̓��e���󓮓IULL��REQ�d���̂��̂ł��邱�Ƃ͊m�肵�Ă���B
    'NOTE: �ȍ~�̎󓮓IULL�p���\�b�h�ɓn�����d���C���X�^���X�́A���̃��\�b�h�Ő�����������
    '�܂��́A���̃C���X�^���X��ParseAsSameKind���\�b�h�Ő����������̂ł���B
    'NOTE: GetBodyFormatViolation()�̎��s�́A�Ăяo����ɍs���̂ŕs�v�ł���B
    Protected Overridable Function ParseAsPassiveUllReq(ByVal oTeleg As ITelegram) As IXllReqTelegram
        Return Nothing
    End Function

    '�󓮓IULL�̏����i�\�����ꂽ�t�@�C���̎󂯓���m�F�j���s�����\�b�h
    'NOTE: �󂯓���s�\�ł����NakCauseCode.Busy��NakCauseCode.InvalidContent��ԋp���邱�ƁB
    Protected Overridable Function PrepareToStartPassiveUll(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '�󓮓IULL�����������i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e���������Ă��邱�Ƃ��m�F�����j
    '�i�]���I��REQ�d���ɑ΂�ACK�d����ԐM���邱�ƂɂȂ�j�ꍇ
    'NOTE: �󂯓���s�\�ȓ��e�ł����NakCauseCode.InvalidContent�Ȃǂ�ԋp���邱�ƁB
    Protected Overridable Function ProcOnPassiveUllComplete(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '�󓮓IULL�ɂē]�����������o�����i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e�ɕs���������o�����j
    '�i�]���I��REQ�d���ɑ΂��n�b�V���l�̕s��v������NAK�d����ԐM���邱�ƂɂȂ�j�ꍇ
    'NOTE: �v���g�R���ɉ����āANakCauseCode.TelegramError���p��NAK�𐶂ݏo�����߂�NakCauseCode��ԋp���邱�ƁB
    Protected Overridable Function ProcOnPassiveUllHashValueError(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '�󓮓IULL�ɂăN���C�A���g����]�����s��ʒm���ꂽ�iContinueCode.Abort�̓]���I��REQ�d������M�����j�ꍇ
    Protected Overridable Sub ProcOnPassiveUllAbort(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�󓮓IULL�ɂē]���I��REQ�d���҂��̃^�C���A�E�g�����������ꍇ
    Protected Overridable Sub ProcOnPassiveUllTimeout(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�󓮓IULL�̍Œ���L���[�C���O���ꂽ�󓮓IULL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overridable Sub ProcOnPassiveUllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '�e�X���b�h����R�l�N�V�������󂯎�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overridable Sub ProcOnConnectionAppear()
    End Sub

    'REQ�d����M�y�т���ɑ΂���ACK�d�����M���������ĒʐM�p�������肵���ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    'NOTE: False��Ԃ��悤�ɂ���΁A�R�l�N�V�������ؒf�����B
    Protected Overridable Function ProcOnReqTelegramReceiveCompleteBySendAck(ByVal oRcvTeleg As ITelegram, ByVal oSndTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ�d����M�y�т���ɑ΂���y�xNAK�d���iBUSY���j���M���������ĒʐM�p�������肵���ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    'NOTE: False��Ԃ��悤�ɂ���΁A�R�l�N�V�������ؒf�����B
    Protected Overridable Function ProcOnReqTelegramReceiveCompleteBySendNak(ByVal oRcvTeleg As ITelegram, ByVal oSndTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ�d�����M�y�т���ɑ΂���ACK�d����M���������ĒʐM�p�������肵���ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    'NOTE: False��Ԃ��悤�ɂ���΁A�R�l�N�V�������ؒf�����B
    Protected Overridable Function ProcOnReqTelegramSendCompleteByReceiveAck(ByVal oSndTeleg As ITelegram, ByVal oRcvTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ�d�����M�y�т���ɑ΂���y�xNAK�d���iBUSY���j��M���������ĒʐM�p�������肵���ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    'NOTE: False��Ԃ��悤�ɂ���΁A�R�l�N�V�������ؒf�����B
    Protected Overridable Function ProcOnReqTelegramSendCompleteByReceiveNak(ByVal oSndTeleg As ITelegram, ByVal oRcvTeleg As ITelegram) As Boolean
        Return True
    End Function

    '�R�l�N�V������ؒf�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overridable Sub ProcOnConnectionDisappear()
    End Sub

    Protected Overridable Function IsParentMessageReceptibleState(ByVal state As State) As Boolean
        Return True
    End Function

    Protected Overridable Function IsParentMessageReceptibleState(ByVal activeXllState As ActiveXllState) As Boolean
        Return True
    End Function

    Protected Overridable Function IsParentMessageReceptibleState(ByVal passiveXllState As PassiveXllState) As Boolean
        Return True
    End Function

    'NAK�d���𑗐M����ꍇ���M�����ꍇ�̂��̌�̋��������߂邽�߂̃��\�b�h
    'NOTE: NAK�d���̃f�[�^��ʂ�NAK�d���̎��R�ɂ���Č��߂邱�Ƃ�z�肵�Ă���B
    Protected Overridable Function GetRequirement(ByVal oNakTeleg As INakTelegram) As NakRequirement
        If oNakTeleg.CauseCode = NakCauseCode.Busy Then
            Return NakRequirement.CareOnRetryOver
        Else
            Return NakRequirement.DisconnectImmediately
        End If
    End Function
#End Region

End Class
