' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/02/16  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Windows.Forms

'NOTE: ���̃N���X���g���ꍇ�ADataGridView�ƈقȂ�A�e�L�X�g�{�b�N�X�ɓ��͂����l��
'�`�F�b�N��CellParsing�C�x���g�n���h����ValueChecking�C�x���g�n���h���Ɏ������܂��B
Public Class XlsDataGridView
    Inherits DataGridView

    Public Event ValueChecking(ByVal sender As Object, ByVal e As XlsDataGridViewValueCheckingEventArgs)

    Private Structure Dirt
        Public ColumnIndex As Integer
        Public RowIndex As Integer
        Public Value As Object
        Public Sub New(ByVal columnIndex As Integer, ByVal rowIndex As Integer, ByVal value As Object)
            Me.ColumnIndex = columnIndex
            Me.RowIndex = rowIndex
            Me.Value = value
        End Sub
    End Structure

    Private alreadyChecked As Boolean = False

    Public Sub New()
        MyBase.New()
        Me.AllowUserToAddRows = False
        Me.AllowUserToDeleteRows = False
        Me.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
    End Sub

    Protected Overrides Sub OnEditingControlShowing(ByVal e As DataGridViewEditingControlShowingEventArgs)
        MyBase.OnEditingControlShowing(e)

        If CurrentCell.ErrorText <> "" Then
            Dim col As Integer = CurrentCell.ColumnIndex
            If col <> 0 AndAlso _
               Me.Columns(col - 1).DataPropertyName.Equals(Me.Columns(col).DataPropertyName) Then
                Dim cbo As ComboBox = TryCast(e.Control, ComboBox)
                If cbo IsNot Nothing Then
                   'NOTE: �����̃e�L�X�g�{�b�N�X�ɒ�`�O�̒l�����͂����
                   '�R���{�{�b�N�X��Value�������I�ɋ�ɂȂ��Ă���󋵂ŁA
                   '�R���{�{�b�N�X�̐擪�A�C�e����I�������ꍇ��
                   '�R���{�{�b�X�̒l���߂��Ă��܂��iSelectedIndexChanged���������Ȃ��H�j
                   '�����������邽�߂ɁA���L���s�����Ƃɂ��Ă���B
                   cbo.SelectedIndex = -1
                End If
            End If
        End If
    End Sub

    Private Function ParseFormattedValue(ByVal formattedValue As Object, ByVal oCell As DataGridViewCell) As Object
        Dim e As New DataGridViewCellParsingEventArgs(oCell.RowIndex, oCell.ColumnIndex, formattedValue, oCell.ValueType, oCell.InheritedStyle)
        OnCellParsing(e)
        If Not e.ParsingApplied AndAlso formattedValue.GetType() IsNot oCell.ValueType Then
            Throw New FormatException("")
        End If
        Return e.Value
    End Function

    Protected Overrides Sub OnCellParsing(ByVal e As DataGridViewCellParsingEventArgs)
        If Not e.ParsingApplied Then
            Dim oTag As Object = Me.Rows(e.RowIndex).Cells(e.ColumnIndex).Tag
            If oTag IsNot Nothing AndAlso TypeOf oTag Is XlsField Then
                e.Value = DirectCast(oTag, XlsField).NormalizeValue(DirectCast(e.Value, String))
                e.ParsingApplied = True
            End If
        End If
        MyBase.OnCellParsing(e)
    End Sub

    Protected Overridable Function CheckValue(ByVal v As Object, ByVal columnIndex As Integer, ByVal rowIndex As Integer) As Boolean
        Dim e As New XlsDataGridViewValueCheckingEventArgs(v, columnIndex, rowIndex)
        RaiseEvent ValueChecking(Me, e)
        If e.Cancel Then Return False
        Return True
    End Function

    Protected Overrides Sub OnCellValidating(ByVal e As DataGridViewCellValidatingEventArgs)
        Me.Rows(e.RowIndex).Cells(e.ColumnIndex).ErrorText = ""

        If e.ColumnIndex <> Me.Columns.Count - 1 AndAlso _
           Me.Columns(e.ColumnIndex).DataPropertyName.Equals(Me.Columns(e.ColumnIndex + 1).DataPropertyName) Then
            Me.Rows(e.RowIndex).Cells(e.ColumnIndex + 1).ErrorText = ""
        End If

        If alreadyChecked Then Return

        If TypeOf Me.Columns(e.ColumnIndex) Is DataGridViewTextBoxColumn Then
            If Not Me.IsCurrentCellDirty Then Return

            Dim v As Object
            Try
                v = ParseFormattedValue(e.FormattedValue, Me.Rows(e.RowIndex).Cells(e.ColumnIndex))
            Catch ex As Exception
                MessageBox.Show("���͂����������s���ł��B" & If(ex.Message.Length <> 0, vbCrLf & ex.Message, ""), "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                e.Cancel = True
                Return
            End Try

            If CheckValue(v, e.ColumnIndex, e.RowIndex) = False Then
                MessageBox.Show("���͒l���s���ł��B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                e.Cancel = True
                Return
            End If
        Else
            MyBase.OnCellValidating(e)
        End If
    End Sub

    Protected Overrides Sub OnDataError(ByVal displayErrorDialogIfNoHandler As Boolean, ByVal e As DataGridViewDataErrorEventArgs)
        If TypeOf Me.Columns(e.ColumnIndex) Is DataGridViewComboBoxColumn AndAlso _
           e.ColumnIndex <> 0 AndAlso _
           Me.Columns(e.ColumnIndex - 1).DataPropertyName.Equals(Me.Columns(e.ColumnIndex).DataPropertyName) Then
            If DirectCast(Me.Rows(e.RowIndex).Cells(e.ColumnIndex), DataGridViewComboBoxCell).Items.Count <> 0 Then
                Me.Rows(e.RowIndex).Cells(e.ColumnIndex).ErrorText = "���͒l����`�O�ł��B"
            End If
        Else
            MyBase.OnDataError(displayErrorDialogIfNoHandler, e)
        End If
    End Sub

    Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
        If e.KeyCode = Keys.Delete Or e.KeyCode = Keys.Back Then
            If Me.CurrentCell IsNot Nothing AndAlso _
               Not Me.CurrentCell.ReadOnly AndAlso _
               TypeOf Me.CurrentCell Is DataGridViewTextBoxCell Then
                Me.BeginEdit(True)
                Dim oTextBox As DataGridViewTextBoxEditingControl = _
                   DirectCast(Me.EditingControl, DataGridViewTextBoxEditingControl)
                oTextBox.Text = ""
            End If
        ElseIf (e.Modifiers And Keys.Control) = Keys.Control AndAlso e.KeyCode = Keys.V Then
            '�N���b�v�{�[�h�̓��e���擾����B
            Dim sClipText As String = Clipboard.GetText()

            '�N���b�v�{�[�h�̓��e�����s�����ŕ�������B
            sClipText = sClipText.Replace(vbCrLf, vbLf).Replace(ControlChars.Cr, ControlChars.Lf)
            Dim sClipRowsBeforeSplit() As String = sClipText.Split(ControlChars.Lf)
            Dim clipRowCount As Integer = sClipRowsBeforeSplit.Length

            '�N���b�v�{�[�h�e�s���^�u�����ŕ�������B
            Dim sClipRows(clipRowCount - 1)() As String
            For r As Integer = 0 To clipRowCount - 1
                sClipRows(r) = sClipRowsBeforeSplit(r).Split(ControlChars.Tab)
            Next r

            Dim oGridColumns As DataGridViewColumnCollection = Me.Columns
            Dim gridColumnCount As Integer = oGridColumns.Count
            Dim gridRowCount As Integer = Me.Rows.Count

            Dim oGridDispColumns(gridColumnCount - 1) As DataGridViewColumn
            For Each oGridColumn As DataGridViewColumn In oGridColumns
                If oGridColumn.Visible Then
                    oGridDispColumns(oGridColumn.DisplayIndex) = oGridColumn
                End If
            Next oGridColumn

            Dim oDirtList As New List(Of Dirt)
            Try
                For Each oGridSelection As DataGridViewCell In Me.SelectedCells
                    Dim gridRowBase As Integer = oGridSelection.RowIndex
                    Dim gridColumnBase As Integer = oGridSelection.ColumnIndex
                    Dim gridColumnDispBase As Integer = oGridColumns(gridColumnBase).DisplayIndex

                    Dim gridRowIndex As Integer = gridRowBase
                    For clipRowIndex As Integer = 0 To clipRowCount - 1
                        Dim oGridRow As DataGridViewRow = Me.Rows(gridRowIndex)
                        If clipRowIndex <> 0 AndAlso oGridRow.Cells(gridColumnBase).Selected Then Exit For

                        Dim sClipCells As String() = sClipRows(clipRowIndex)
                        Dim gridColumnDispIndex As Integer = gridColumnDispBase
                        For clipColumnIndex As Integer = 0 To sClipCells.Length - 1
                            Dim oGridColumn As DataGridViewColumn = oGridDispColumns(gridColumnDispIndex)
                            Dim gridColumnIndex As Integer = oGridColumn.Index

                            Dim oGridCell As DataGridViewCell = oGridRow.Cells(gridColumnIndex)
                            If clipColumnIndex <> 0 AndAlso oGridCell.Selected Then Exit For

                            If oGridCell.ReadOnly Then
                                Throw New InvalidOperationException("�\��t����̃Z��(R" & gridRowIndex.ToString() & "C" & gridColumnDispIndex.ToString() & ")���ǂݎ���p�ł��B" & _
                                                                    If(Me.ColumnHeadersVisible, vbCrLf & "�\��t����̗񖼁F" & oGridColumn.HeaderText, ""))
                            End If

                            If Not (TypeOf oGridCell Is DataGridViewTextBoxCell) Then
                                Throw New InvalidOperationException("�\��t����̃Z��(R" & gridRowIndex.ToString() & "C" & gridColumnDispIndex.ToString() & ")���e�L�X�g�̒��ړ��͂ɑΉ����Ă��܂���B" & _
                                                                    If(Me.ColumnHeadersVisible, vbCrLf & "�\��t����̗񖼁F" & oGridColumn.HeaderText, ""))
                            End If

                            Dim v As Object
                            Try
                                v = ParseFormattedValue(sClipCells(clipColumnIndex), oGridCell)
                            Catch ex As Exception
                                Throw New InvalidOperationException("�\��t����̃Z��(R" & gridRowIndex.ToString() & "C" & gridColumnDispIndex.ToString() & ")�ɂƂ��Ċi�[�s�\�Ȓl�ł��B" & _
                                                                    If(ex.Message.Length <> 0, vbCrLf & ex.Message, "") & _
                                                                    If(Me.ColumnHeadersVisible, vbCrLf & "�\��t����̗񖼁F" & oGridColumn.HeaderText, "") & _
                                                                    vbCrLf & "�l�F" & sClipCells(clipColumnIndex))
                            End Try

                            If CheckValue(v, gridColumnIndex, gridRowIndex) = False Then
                                Throw New InvalidOperationException("�\��t����̃Z��(R" & gridRowIndex.ToString() & "C" & gridColumnDispIndex.ToString() & ")�ɂƂ��ĕs���Ȓl�ł��B" & _
                                                                    If(Me.ColumnHeadersVisible, vbCrLf & "�\��t����̗񖼁F" & oGridColumn.HeaderText, "") & _
                                                                    vbCrLf & "�l�F" & sClipCells(clipColumnIndex))
                            End If

                            oDirtList.Add(New Dirt(gridColumnIndex, gridRowIndex, v))

                            Do
                                gridColumnDispIndex += 1
                                If gridColumnDispIndex >= gridColumnCount Then Exit For
                            Loop While oGridDispColumns(gridColumnDispIndex) Is Nothing
                        Next clipColumnIndex

                        gridRowIndex += 1
                        If gridRowIndex >= gridRowCount Then Exit For
                    Next clipRowIndex
                Next oGridSelection
            Catch ex As Exception
                MessageBox.Show(ex.Message, "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try

            alreadyChecked = True
            For Each d As Dirt In oDirtList
                Me.Rows(d.RowIndex).Cells(d.ColumnIndex).Value = d.Value
                Me.Rows(d.RowIndex).Cells(d.ColumnIndex).Selected = True
            Next d
            Me.InvalidateRow(Me.CurrentCellAddress.Y)
            alreadyChecked = False
        Else
            MyBase.OnKeyDown(e)
        End If
    End Sub

End Class

Public Class XlsDataGridViewValueCheckingEventArgs
    Inherits CancelEventArgs

    Private _Value As Object
    Private _ColumnIndex As Integer
    Private _RowIndex As Integer

    Public ReadOnly Property Value As Object
        Get
            Return _Value
        End Get
    End Property

    Public ReadOnly Property RowIndex As Integer
        Get
            Return _RowIndex
        End Get
    End Property

    Public ReadOnly Property ColumnIndex As Integer
        Get
            Return _ColumnIndex
        End Get
    End Property

    Friend Sub New(ByVal value As Object, ByVal columnIndex As Integer, ByVal rowIndex As Integer)
        MyBase.New()
        Me._Value = value
        Me._ColumnIndex = columnIndex
        Me._RowIndex = rowIndex
    End Sub

End Class
