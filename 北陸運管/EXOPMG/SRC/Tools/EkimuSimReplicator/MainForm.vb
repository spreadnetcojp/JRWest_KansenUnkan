Option Explicit On
Option Strict On

Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

Imports IWshRuntimeLibrary

Imports JR.ExOpmg.Common

Public Class MainForm

    Private Sub ExecButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExecButton.Click
        Using oReader As StreamReader _
           = New StreamReader(ReplicantsListPathTextBox.Text, Encoding.GetEncoding(932))

            Dim sLine As String = oReader.ReadLine()
            While sLine IsNot Nothing
                If Not sLine.Length = 0 Then
                    MakeReplicant(sLine)
                End If
                sLine = oReader.ReadLine()
            End While

        End Using
    End Sub

    Private Sub MakeReplicant(ByVal sReplicantName As String)
        Dim replicantCode As EkCode = EkCode.Parse(sReplicantName.Substring(1), "%3R%3S_%C_%U")
        Dim sExeFilePath As String = ExePathTextBox.Text
        Dim sExeFileName As String = Path.GetFileName(sExeFilePath)
        Dim sOriginPath As String = OriginPathTextBox.Text
        Dim sReplicantPath As String = Path.Combine(Path.GetDirectoryName(sOriginPath), sReplicantName)

        Directory.CreateDirectory(sReplicantPath)

        Dim aFiles As String() = Directory.GetFiles(sOriginPath)
        For Each sFilePath As String In aFiles
            Dim sFileName As String = Path.GetFileName(sFilePath)
            Dim sDstPath As String = Path.Combine(sReplicantPath, sFileName)

            If Not sFileName.Equals(sExeFileName & ".lnk", StringComparison.OrdinalIgnoreCase) Then
                System.IO.File.Copy(sFilePath, sDstPath)
            End If

            If sFileName.Equals(Path.ChangeExtension(sExeFileName, "ini"), StringComparison.OrdinalIgnoreCase) Then
                Dim originCode As EkCode = EkCode.Parse(Constant.GetIni("Credential", "SelfEkCode", sDstPath), "%M-%R-%S-%C-%U")
                replicantCode.Model = originCode.Model
                Constant.SetIni("Credential", "SelfEkCode", sDstPath, replicantCode.ToString())
            End If
        Next sFilePath

        Dim WshShell As WshShellClass = New WshShellClass()
        Dim sShortcutPath As String = Path.Combine(sReplicantPath, sExeFileName) & ".lnk"
        Dim oShortcut As IWshRuntimeLibrary.IWshShortcut = CType(WshShell.CreateShortcut(sShortcutPath), IWshRuntimeLibrary.IWshShortcut)
        oShortcut.TargetPath = sExeFilePath
        oShortcut.WorkingDirectory = sReplicantPath
        oShortcut.Save()
    End Sub

End Class
