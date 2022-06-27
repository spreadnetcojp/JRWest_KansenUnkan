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

Imports System.Text

<FlagsAttribute()> _
Public Enum EkCodeElems As Integer
    None = 0
    Model = 1
    RailSection = 2
    StationOrder = 4
    Corner = 8
    Unit = 16
End Enum

Public Structure EkCode

    Public Shared ReadOnly Empty As New EkCode()

    Public Const SymPrefix As Char = "%"c
    Public Const ModelSymbol As Char = "M"c
    Public Const RailSectionSymbol As Char = "R"c
    Public Const StationOrderSymbol As Char = "S"c
    Public Const CornerSymbol As Char = "C"c
    Public Const UnitSymbol As Char = "U"c

    Private Const maxElems As Integer = 5

    '�@��
    Private _Model As Integer
    Public Property Model() As Integer
        Get
            Return _Model
        End Get

        Set(ByVal value As Integer)
            If value < 0 Then
                Throw New ArgumentOutOfRangeException()
            End If
            If value > 99 Then
                Throw New ArgumentOutOfRangeException()
            End If
            _Model = value
        End Set
    End Property

    '�T�C�o�l����R�[�h
    Private _RailSection As Integer
    Public Property RailSection() As Integer
        Get
            Return _RailSection
        End Get

        Set(ByVal value As Integer)
            If value < 0 Then
                Throw New ArgumentOutOfRangeException()
            End If
            If value > 999 Then
                Throw New ArgumentOutOfRangeException()
            End If
            _RailSection = value
        End Set
    End Property

    '�T�C�o�l�w���R�[�h
    Private _StationOrder As Integer
    Public Property StationOrder() As Integer
        Get
            Return _StationOrder
        End Get

        Set(ByVal value As Integer)
            If value < 0 Then
                Throw New ArgumentOutOfRangeException()
            End If
            If value > 999 Then
                Throw New ArgumentOutOfRangeException()
            End If
            _StationOrder = value
        End Set
    End Property

    '�R�[�i�[�R�[�h
    Private _Corner As Integer
    Public Property Corner() As Integer
        Get
            Return _Corner
        End Get

        Set(ByVal value As Integer)
            If value < 0 Then
                Throw New ArgumentOutOfRangeException()
            End If
            If value > 9999 Then
                Throw New ArgumentOutOfRangeException()
            End If
            _Corner = value
        End Set
    End Property

    '���@
    Private _Unit As Integer
    Public Property Unit() As Integer
        Get
            Return _Unit
        End Get

        Set(ByVal value As Integer)
            If value < 0 Then
                Throw New ArgumentOutOfRangeException()
            End If
            If value > 99 Then
                Throw New ArgumentOutOfRangeException()
            End If
            _Unit = value
        End Set
    End Property

    Public Shared Function Parse(ByVal source As String) As EkCode
        Return Parse(source, "%M-%R-%S-%C-%U")
    End Function

    'NOTE: format�����񂪈ُ�̏ꍇ��ArgumentException���X���[���܂��B
    'NOTE: source�����񂪈ُ�̏ꍇ��FormatException���X���[���܂��B
    Public Shared Function Parse(ByVal source As String, ByVal format As String) As EkCode
        Dim ret As EkCode
        Dim segArray As FormatSegment() = Compile(format)
        Dim srcLen As Integer = source.Length
        Dim srcPos As Integer = 0
        Dim nextSegPos As Integer
        Try
            For Each seg As FormatSegment In segArray
                If seg.FixedText IsNot Nothing Then
                    Dim segLen As Integer = seg.FixedText.Length
                    nextSegPos = srcPos + segLen
                    If nextSegPos > srcLen Then
                        Throw New FormatException("Too short string.")
                    End If
                    If String.CompareOrdinal(source, srcPos, seg.FixedText, 0, segLen) <> 0 Then
                        Dim endPos As Integer = nextSegPos - 1
                        Throw New FormatException("The chars from " & srcPos.ToString() & " to " & endPos.ToString() & " must agree with the format.")
                    End If
                    srcPos = nextSegPos
                ElseIf seg.ElemKind <> EkCodeElems.None Then
                    Dim segLen As Integer = seg.ElemLength
                    If segLen = 0 Then
                        If srcPos >= srcLen Then
                            Throw New FormatException("Too short string.")
                        End If

                        nextSegPos = srcPos
                        Do
                            Dim c As Integer = AscW(source.Chars(nextSegPos))
                            If c < &H30 OrElse c > &H39 Then Exit Do
                            nextSegPos += 1
                        Loop Until nextSegPos >= srcLen

                        segLen = nextSegPos - srcPos
                        If segLen < 1 Then
                            Throw New FormatException("The chars from " & srcPos.ToString() & " must be digit.")
                        End If
                    Else
                        nextSegPos = srcPos + segLen
                        If nextSegPos > srcLen Then
                            Throw New FormatException("Too short string.")
                        End If
                        If Not Utility.IsDecimalStringFixed(source, srcPos, segLen) Then
                            Dim endPos As Integer = nextSegPos - 1
                            Throw New FormatException("The chars from " & srcPos.ToString() & " to " & endPos.ToString() & " must be digit.")
                        End If
                    End If

                    Dim segValue As Integer = Utility.GetIntFromDecimalString(source, srcPos, segLen)
                    Select Case seg.ElemKind
                        Case EkCodeElems.Model
                            ret.Model = segValue
                        Case EkCodeElems.RailSection
                            ret.RailSection = segValue
                        Case EkCodeElems.StationOrder
                            ret.StationOrder = segValue
                        Case EkCodeElems.Corner
                            ret.Corner = segValue
                        Case EkCodeElems.Unit
                            ret.Unit = segValue
                    End Select
                    srcPos = nextSegPos
                Else
                    Exit For
                End If
            Next
        Catch ex As ArgumentOutOfRangeException
            Dim endPos As Integer = nextSegPos - 1
            Throw New FormatException("The chars from " & srcPos.ToString() & " to " & endPos.ToString() & " is invalid.", ex)
        End Try
        Return ret
    End Function

    Public Overrides Function ToString() As String
        Return ToString("%2M-%3R-%3S-%4C-%2U")
    End Function

    'NOTE: format�����񂪈ُ�̏ꍇ��ArgumentException���X���[���܂��B
    Public Overloads Function ToString(ByVal format As String) As String
        Dim segArray As FormatSegment() = Compile(format)
        Dim sb As New StringBuilder()
        For Each seg As FormatSegment In segArray
            If seg.FixedText IsNot Nothing Then
                sb.Append(seg.FixedText)
            ElseIf seg.ElemKind <> EkCodeElems.None Then
                If seg.ElemLength = 0 Then
                    Select Case seg.ElemKind
                        Case EkCodeElems.Model
                            sb.Append(_Model.ToString())
                        Case EkCodeElems.RailSection
                            sb.Append(_RailSection.ToString())
                        Case EkCodeElems.StationOrder
                            sb.Append(_StationOrder.ToString())
                        Case EkCodeElems.Corner
                            sb.Append(_Corner.ToString())
                        Case EkCodeElems.Unit
                            sb.Append(_Unit.ToString())
                    End Select
                Else
                    Dim fmt As String = "D" & seg.ElemLength.ToString()
                    'NOTE: �t�����l������ƁA�����ӂ���N�����ꍇ��
                    '��O���X���[��������e�؂�������Ȃ��B
                    Select Case seg.ElemKind
                        Case EkCodeElems.Model
                            sb.Append(_Model.ToString(fmt))
                        Case EkCodeElems.RailSection
                            sb.Append(_RailSection.ToString(fmt))
                        Case EkCodeElems.StationOrder
                            sb.Append(_StationOrder.ToString(fmt))
                        Case EkCodeElems.Corner
                            sb.Append(_Corner.ToString(fmt))
                        Case EkCodeElems.Unit
                            sb.Append(_Unit.ToString(fmt))
                    End Select
                End If
            Else
                Exit For
            End If
        Next
        Return sb.ToString()
    End Function

    Public Shared Operator =(ByVal a As EkCode, ByVal b As EkCode) As Boolean
        Return a.Equals(b)
    End Operator

    Public Shared Operator <>(ByVal a As EkCode, ByVal b As EkCode) As Boolean
        Return Not a.Equals(b)
    End Operator

    'Public Overloads Function Equals(ByVal obj As Object) As Boolean
    '    If TypeOf obj Is EkCode Then
    '        Dim o As EkCode = DirectCast(obj, EkCode)
    '        Return o._Model = _Model AndAlso _
    '               o._RailSection = _RailSection AndAlso _
    '               o._StationOrder = _StationOrder AndAlso _
    '               o._Corner = _Corner AndAlso _
    '               o._Unit = _Unit
    '    Else
    '        Return False
    '    End If
    'End Function

    Public Overloads Function Equals(ByVal obj As EkCode, ByVal elems As EkCodeElems) As Boolean
        If (elems And EkCodeElems.Model) <> EkCodeElems.None AndAlso _
           obj._Model <> _Model Then Return False

        If (elems And EkCodeElems.RailSection) <> EkCodeElems.None AndAlso _
           obj._RailSection <> _RailSection Then Return False

        If (elems And EkCodeElems.StationOrder) <> EkCodeElems.None AndAlso _
           obj._StationOrder <> _StationOrder Then Return False

        If (elems And EkCodeElems.Corner) <> EkCodeElems.None AndAlso _
           obj._Corner <> _Corner Then Return False

        If (elems And EkCodeElems.Unit) <> EkCodeElems.None AndAlso _
           obj._Unit <> _Unit Then Return False

        Return True
    End Function

    'Public Sub [Set](ByVal value As EkCode)
    '    Me._Model = value._Model
    '    Me._RailSection = value._RailSection
    '    Me._StationOrder = value._StationOrder
    '    Me._Corner = value._Corner
    '    Me._Unit = value._Unit
    'End Sub

    Public Sub [Set](ByVal value As EkCode, ByVal elems As EkCodeElems)
        If (elems And EkCodeElems.Model) <> EkCodeElems.None Then
            Me._Model = value._Model
        End If
        If (elems And EkCodeElems.RailSection) <> EkCodeElems.None Then
            Me._RailSection = value._RailSection
        End If
        If (elems And EkCodeElems.StationOrder) <> EkCodeElems.None Then
            Me._StationOrder = value._StationOrder
        End If
        If (elems And EkCodeElems.Corner) <> EkCodeElems.None Then
            Me._Corner = value._Corner
        End If
        If (elems And EkCodeElems.Unit) <> EkCodeElems.None Then
            Me._Unit = value._Unit
        End If
    End Sub

    Private Structure FormatSegment
        Public FixedText As String  '���̃Z�O�����g���Z�p���[�^�łȂ��Ƃ���Nothing
        Public ElemKind As EkCodeElems  '���̃Z�O�����g���Z�p���[�^�̂Ƃ���EkCodeElems.None
        Public ElemLength As Integer  '�w��Ȃ��̂Ƃ���0
    End Structure

    'NOTE �ԋp����z��́A�㔼�������v�f�ɂȂ蓾��B
    'FixedText = Nothing AndAlso ElemKind = EkCodeElems.None �̗v�f�͖����ł���A
    '����ȍ~�̗v�f���S�Ė����ł���B
    Private Shared Function Compile(ByVal format As String) As FormatSegment()
        Dim seg(maxElems * 2 + 1) As FormatSegment  '�S�v�f{Nothing, EkCodeElems.None}�ɏ�����
        Dim segCount As Integer = 0
        Dim resolvedElems As EkCodeElems = EkCodeElems.None
        Dim fmtLen As Integer = format.Length
        Dim fmtPos As Integer = 0
        Dim sb As New StringBuilder()
        While fmtPos < fmtLen
            Dim c As Char = format.Chars(fmtPos)
            fmtPos += 1
            If c = SymPrefix Then
                If fmtPos >= fmtLen Then
                    Throw New ArgumentException("The format ended in midstream of some Symbol.")
                End If
                c = format.Chars(fmtPos)
                fmtPos += 1

                'NOTE: �����d���̎����ł��邽�ߒ��ӁB
                '���L��If�̏�����AscW(c)��&H31�`&H39�̏ꍇ�ɑ�������B
                'AscW(c)��&H30�̏ꍇ�␔���łȂ��ꍇ�́AelemLen��
                '0�i�����w��Ȃ��j�Ƃ��āAc�̓V���{������
                '�iM, R, S, C, U�̂����ꂩ�ł���ׂ������j�Ƃ݂Ȃ��B
                '���Ȃ킿�u%0S�v�Ȃǂ́Aformat������Ƃ��ĕs���ł���B
                Dim elemLen As Integer = Val(c)
                If elemLen <> 0 Then
                    If fmtPos >= fmtLen Then
                        Throw New ArgumentException("The format ended in midstream of some Symbol.")
                    End If
                    c = format.Chars(fmtPos)
                    fmtPos += 1
                End If

                Select Case c
                    Case ModelSymbol
                        If sb.Length <> 0 Then
                            seg(segCount).FixedText = sb.ToString()
                            segCount += 1
                            sb.Length = 0
                        ElseIf segCount > 0 Then
                            Dim preSegCount As Integer = segCount - 1
                            If seg(preSegCount).ElemKind <> EkCodeElems.None AndAlso _
                               seg(preSegCount).ElemLength = 0 Then
                                Throw New ArgumentException("The format contains unacceptable symbol sequence.")
                            End If
                        End If
                        If (resolvedElems And EkCodeElems.Model) <> EkCodeElems.None Then
                            Throw New ArgumentException("The format contains duplicate ModelSymbol.")
                        End If
                        resolvedElems = resolvedElems Or EkCodeElems.Model
                        seg(segCount).ElemKind = EkCodeElems.Model
                        seg(segCount).ElemLength = elemLen
                        segCount += 1
                    Case RailSectionSymbol
                        If sb.Length <> 0 Then
                            seg(segCount).FixedText = sb.ToString()
                            segCount += 1
                            sb.Length = 0
                        ElseIf segCount > 0 Then
                            Dim preSegCount As Integer = segCount - 1
                            If seg(preSegCount).ElemKind <> EkCodeElems.None AndAlso _
                               seg(preSegCount).ElemLength = 0 Then
                                Throw New ArgumentException("The format contains unacceptable symbol sequence.")
                            End If
                        End If
                        If (resolvedElems And EkCodeElems.RailSection) <> EkCodeElems.None Then
                            Throw New ArgumentException("The format contains duplicate RailSectionSymbol.")
                        End If
                        resolvedElems = resolvedElems Or EkCodeElems.RailSection
                        seg(segCount).ElemKind = EkCodeElems.RailSection
                        seg(segCount).ElemLength = elemLen
                        segCount += 1
                    Case StationOrderSymbol
                        If sb.Length <> 0 Then
                            seg(segCount).FixedText = sb.ToString()
                            segCount += 1
                            sb.Length = 0
                        ElseIf segCount > 0 Then
                            Dim preSegCount As Integer = segCount - 1
                            If seg(preSegCount).ElemKind <> EkCodeElems.None AndAlso _
                               seg(preSegCount).ElemLength = 0 Then
                                Throw New ArgumentException("The format contains unacceptable symbol sequence.")
                            End If
                        End If
                        If (resolvedElems And EkCodeElems.StationOrder) <> EkCodeElems.None Then
                            Throw New ArgumentException("The format contains duplicate StationOrderSymbol.")
                        End If
                        resolvedElems = resolvedElems Or EkCodeElems.StationOrder
                        seg(segCount).ElemKind = EkCodeElems.StationOrder
                        seg(segCount).ElemLength = elemLen
                        segCount += 1
                    Case CornerSymbol
                        If sb.Length <> 0 Then
                            seg(segCount).FixedText = sb.ToString()
                            segCount += 1
                            sb.Length = 0
                        ElseIf segCount > 0 Then
                            Dim preSegCount As Integer = segCount - 1
                            If seg(preSegCount).ElemKind <> EkCodeElems.None AndAlso _
                               seg(preSegCount).ElemLength = 0 Then
                                Throw New ArgumentException("The format contains unacceptable symbol sequence.")
                            End If
                        End If
                        If (resolvedElems And EkCodeElems.Corner) <> EkCodeElems.None Then
                            Throw New ArgumentException("The format contains duplicate CornerSymbol.")
                        End If
                        resolvedElems = resolvedElems Or EkCodeElems.Corner
                        seg(segCount).ElemKind = EkCodeElems.Corner
                        seg(segCount).ElemLength = elemLen
                        segCount += 1
                    Case UnitSymbol
                        If sb.Length <> 0 Then
                            seg(segCount).FixedText = sb.ToString()
                            segCount += 1
                            sb.Length = 0
                        ElseIf segCount > 0 Then
                            Dim preSegCount As Integer = segCount - 1
                            If seg(preSegCount).ElemKind <> EkCodeElems.None AndAlso _
                               seg(preSegCount).ElemLength = 0 Then
                                Throw New ArgumentException("The format contains unacceptable symbol sequence.")
                            End If
                        End If
                        If (resolvedElems And EkCodeElems.Unit) <> EkCodeElems.None Then
                            Throw New ArgumentException("The format contains duplicate UnitSymbol.")
                        End If
                        resolvedElems = resolvedElems Or EkCodeElems.Unit
                        seg(segCount).ElemKind = EkCodeElems.Unit
                        seg(segCount).ElemLength = elemLen
                        segCount += 1
                    Case SymPrefix
                        If elemLen <> 0 Then
                            Throw New ArgumentException("The format contains invalid symbol.")
                        End If
                        sb.Append(c)
                    Case Else
                        Throw New ArgumentException("The format contains invalid symbol.")
                End Select
            Else
                sb.Append(c)
            End If
        End While

        If sb.Length <> 0 Then
            seg(segCount).FixedText = sb.ToString()
        End If

        Return seg
    End Function

End Structure
