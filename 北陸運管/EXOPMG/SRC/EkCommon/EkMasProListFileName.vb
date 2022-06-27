' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text.RegularExpressions

''' <summary>
''' 適用リストのファイル名を取り扱うためのクラス。
''' </summary>
Public Class EkMasProListFileName

#Region "定数"
    Private Shared ReadOnly oFileNameRegx As New Regex("^[A-Z]{3}_[A-Z]{3}[0-9]{2}_[A-Z]_[0-9]{1,8}_[0-9]{2}\.csv$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Const fileNameLenForMasterPurpose As Integer = 22
    Private Const fileNameLenForProgramPurpose As Integer = 27
#End Region

#Region "メソッド"
    ''' <summary>
    ''' ファイル名が適用リストのものであるか否かを判定する。
    ''' </summary>
    ''' <remarks>
    ''' GetXxxxメソッドは、このメソッドの戻り値がTrueになるファイル名を
    ''' 引数に呼び出すことを前提とする。
    ''' </remarks>
    ''' <param name="sFileName">ファイル名</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsValid(ByVal sFileName As String) As Boolean
        If Not oFileNameRegx.IsMatch(sFileName) Then Return False

        Dim sListKind As String = GetListKind(sFileName)
        Dim sDataApplicableModel As String = GetDataApplicableModel(sFileName)
        Dim dataVersion As Integer = GetDataVersionAsInt(sFileName)

        If sListKind.Equals("TGL") Then
            If sFileName.Length <> fileNameLenForMasterPurpose Then Return False
            If GetDataSubKind(sFileName).Equals("00") Then Return False

            If sDataApplicableModel.Equals("G") OrElse sDataApplicableModel.Equals("Y") Then
                'NOTE: このメソッドではDataKindの値はチェックしない。
                'このメソッドを呼び出した後、GetDataKindの戻り値が
                '「DataPurposeごとに用意されたDB上のテーブル」に
                '登録されているか別途チェックすることを前提としている。
                'また、DataKindが対応するエリア番号をDBから取得し、
                '適用リストに記載された駅のエリアと比較するのも、
                'このメソッドの役割ではない。

                If dataVersion < 1 OrElse dataVersion > 255 Then Return False
            Else
                Return False
            End If
        ElseIf sListKind.Equals("TDL") Then
            If sFileName.Length <> fileNameLenForProgramPurpose Then Return False

            If sDataApplicableModel.Equals("G") Then
                If Not GetDataKind(sFileName).Equals("GPG") Then Return False
                If dataVersion > 9999 Then Return False
            ElseIf sDataApplicableModel.Equals("Y") Then
                If Not GetDataKind(sFileName).Equals("YPG") Then Return False
                If dataVersion > 9999 Then Return False
            ElseIf sDataApplicableModel.Equals("W") Then
                If Not GetDataKind(sFileName).Equals("WPG") Then Return False
            Else
                Return False
            End If
        Else
            Return False
        End If

        If GetListVersionAsInt(sFileName) = 0 Then Return False

        Return True
    End Function

    ''' <summary>
    ''' 適用リストファイル名から適用リスト自身の種別を取得する。
    ''' </summary>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>適用リスト自身の種別（"TGL"または"TDL"）</returns>
    Public Shared Function GetListKind(ByVal sFileName As String) As String
        Return sFileName.Substring(0, 3).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' 適用リストファイル名からデータの用途を取得する。
    ''' </summary>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>データ用途（"MST"または"PRG"）</returns>
    Public Shared Function GetDataPurpose(ByVal sFileName As String) As String
        Select Case GetListKind(sFileName)
            Case "TGL"
                Return "MST"
            Case "TDL"
                Return "PRG"
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' 適用リストファイル名からデータの種別を取得する。
    ''' </summary>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>データの種別（用途がマスタの場合は"DSH"等、用途がプログラムの場合は"WPG"等）</returns>
    Public Shared Function GetDataKind(ByVal sFileName As String) As String
        Return sFileName.Substring(4, 3).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' 適用リストファイル名からデータのサブ種別（パターンNoまたはエリアNo）を取得する。
    ''' </summary>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>データのサブ種別（用途がマスタの場合はパターンNo、用途がプログラムの場合はエリアNo）</returns>
    Public Shared Function GetDataSubKindAsInt(ByVal sFileName As String) As Integer
        Return Integer.Parse(GetDataSubKind(sFileName))
    End Function

    ''' <summary>
    ''' 適用リストファイル名からデータのサブ種別（パターンNoまたはエリアNo）を取得する。
    ''' </summary>
    ''' <remarks>
    ''' ２桁の文字列として取得する。
    ''' </remarks>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>データのサブ種別（用途がマスタの場合はパターンNo、用途がプログラムの場合はエリアNo）</returns>
    Public Shared Function GetDataSubKind(ByVal sFileName As String) As String
        Return sFileName.Substring(7, 2)
    End Function

    ''' <summary>
    ''' 適用リストファイル名からデータの適用対象機種を取得する。
    ''' </summary>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>データの適用対象機種（用途がマスタの場合は"G"または"Y"、用途がプログラムの場合は"W"または"G"または"Y"）</returns>
    Public Shared Function GetDataApplicableModel(ByVal sFileName As String) As String
        Return sFileName.Substring(10, 1).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' 適用リストファイル名からデータのバージョン（マスタバージョンまたは代表バージョン）を取得する。
    ''' </summary>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>データのバージョン（用途がマスタの場合はマスタバージョン、用途がプログラムの場合は代表バージョン）</returns>
    Public Shared Function GetDataVersionAsInt(ByVal sFileName As String) As Integer
        Dim nextSepPos As Integer = sFileName.IndexOf("_"c, 13)
        Return Integer.Parse(sFileName.Substring(12, nextSepPos - 12))
    End Function

    ''' <summary>
    ''' 適用リストファイル名からデータのバージョン（マスタバージョンまたは代表バージョン）を取得する。
    ''' </summary>
    ''' <remarks>
    ''' マスタバージョンならば３桁、代表バージョンならば８桁または４桁（適用対象機種による）の文字列として取得する。
    ''' </remarks>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>データのバージョン（用途がマスタの場合はマスタバージョン、用途がプログラムの場合は代表バージョン）</returns>
    Public Shared Function GetDataVersion(ByVal sFileName As String) As String
        Dim intValue As Integer = GetDataVersionAsInt(sFileName)
        If GetListKind(sFileName).Equals("TGL") Then
            Return intValue.ToString("D3")
        Else
            Return intValue.ToString(EkConstants.ProgramDataVersionFormatOfModels(GetDataApplicableModel(sFileName)))
        End If
    End Function

    ''' <summary>
    ''' 適用リストファイル名から適用リスト自身のバージョンを取得する。
    ''' </summary>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>適用リスト自身のバージョン</returns>
    Public Shared Function GetListVersionAsInt(ByVal sFileName As String) As Integer
        Return Integer.Parse(GetListVersion(sFileName))
    End Function

    ''' <summary>
    ''' 適用リストファイル名から適用リスト自身のバージョンを取得する。
    ''' </summary>
    ''' <remarks>
    ''' ２桁の文字列として取得する。
    ''' </remarks>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>適用リスト自身のバージョン</returns>
    Public Shared Function GetListVersion(ByVal sFileName As String) As String
        Dim startPos As Integer = sFileName.Length - 6
        Return sFileName.Substring(startPos, 2)
    End Function

    ''' <summary>
    ''' 厳密なファイル名を生成する。
    ''' </summary>
    ''' <param name="sFileName">適用リストファイル名</param>
    ''' <returns>厳密な適用リストファイル名</returns>
    Public Shared Function Normalize(ByVal sFileName As String) As String
        Return Gen(GetDataPurpose(sFileName), GetDataKind(sFileName), GetDataSubKind(sFileName), GetDataApplicableModel(sFileName), GetDataVersion(sFileName), GetListVersion(sFileName))
    End Function

    ''' <summary>
    ''' 適用リストのファイル名を生成する。
    ''' </summary>
    ''' <remarks>
    ''' 生成するファイル名の形式は本クラスの内部にカプセル化してある。
    ''' </remarks>
    ''' <param name="sDataPurpose">データの用途（"MST"または"PRG"）</param>
    ''' <param name="sDataKind">データの種別（用途がマスタの場合は"DSH"等、用途がプログラムの場合は"WPG"等）</param>
    ''' <param name="sDataSubKind">データのサブ種別（用途がマスタの場合はパターンNo、用途がプログラムの場合はエリアNo）</param>
    ''' <param name="sDataApplicableModel">データの適用対象機種（用途がマスタの場合は"G"または"Y"、用途がプログラムの場合は"W"または"G"または"Y"）</param>
    ''' <param name="sDataVersion">データのバージョン（用途がマスタの場合はマスタバージョン、用途がプログラムの場合は代表バージョン）</param>
    ''' <param name="sListVersion">適用リスト自身のバージョン</param>
    ''' <returns>適用リストファイル名</returns>
    Public Shared Function Gen( _
       ByVal sDataPurpose As String, _
       ByVal sDataKind As String, _
       ByVal sDataSubKind As String, _
       ByVal sDataApplicableModel As String, _
       ByVal sDataVersion As String, _
       ByVal sListVersion As String) As String

        Select Case sDataPurpose
            Case "MST"
                Return "TGL_" & sDataKind & sDataSubKind & "_" & sDataApplicableModel & "_" & sDataVersion & "_" & sListVersion & ".csv"
            Case "PRG"
                Return "TDL_" & sDataKind & sDataSubKind & "_" & sDataApplicableModel & "_" & Integer.Parse(sDataVersion).ToString("D8") & "_" & sListVersion & ".csv"
            Case Else
                Return Nothing
        End Select
    End Function
#End Region

End Class
