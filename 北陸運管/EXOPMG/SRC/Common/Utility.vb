' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴：
'   Ver      日付        担当       コメント
'   0.0      2006/07/07             新規作成
'   0.1      2006/11/15  muneyuki   CHARtoBIN、CHARtoBIN 変更 BigEndian→LittleEndian
'                                   CHARtoBINwithBigEndian、BINtoCHARwithBigEndian 追加
'   0.2      2011/10/20  NES(河脇)  INIからのＤＢ接続文字列取得を追加
'   0.3      2013/04/01  (NES)小林  運管端末と運管サーバで分岐する類の製品依存処理を除去、
'                                   CopyIntToBcdBytes～DeleteTemporalDirectoryを追加
' **********************************************************************
Option Strict On
Option Explicit On

''' <summary>
''' ユーティリティ
''' </summary>
Public Class Utility

    ' <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< メソッド

#Region "コード変換（文字→BIN）(モトローラ型（Big Endian））"
    ''' <summary>
    ''' [コード変換（文字→BIN）(モトローラ型（Big Endian））]
    ''' 変換元の文字は数値のみ有効。
    ''' </summary>
    ''' <param name="BaseChar">変換前文字列</param>
    ''' <param name="ByteLength">変換後のByte数</param>
    ''' <returns>変換後Byte配列</returns>
    Public Shared Function CHARtoBINwithBigEndian(ByVal BaseChar As String, ByVal ByteLength As Integer) As Byte()
        Dim bRtn() As Byte
        Dim nConv As UInt64
        Try
            If String.IsNullOrEmpty(BaseChar) Then BaseChar = "0"
            nConv = System.UInt64.Parse(BaseChar)
            bRtn = System.BitConverter.GetBytes(nConv)
            Array.Resize(bRtn, ByteLength)  'サイズ変更
            Array.Reverse(bRtn)             '要素順反転
            Return bRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "コード変換（BIN→文字）(モトローラ型（Big Endian））"
    ''' <summary>
    ''' [コード変換（BIN→文字）(モトローラ型（Big Endian））]
    ''' 変換後の文字は数値のみ有効。
    ''' </summary>
    ''' <param name="BaseByte">変換前Byte配列</param>
    ''' <returns>変換後文字列</returns>
    Public Shared Function BINtoCHARwithBigEndian(ByVal BaseByte() As Byte) As String
        Dim sRtn As String = ""
        Dim i As Integer
        Dim n As UInt64
        Dim nRtn As UInt64
        Try
            Array.Reverse(BaseByte)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
        Try
            For i = 0 To BaseByte.Length - 1
                n = CType(BaseByte(i).ToString, UInt64)
                If i <> 0 Then
                    If n <> 0 Then
                        n = CType((256 ^ i) * n, UInt64)
                    End If
                End If
                nRtn = nRtn + n
            Next
            sRtn = CType(nRtn, String)
            Return sRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        Finally
            Array.Reverse(BaseByte)
        End Try
    End Function
#End Region

#Region "コード変換（文字→BIN）(インテル型（Little Endian））"
    ''' <summary>
    ''' [コード変換（文字→BIN）(インテル型（Little Endian））]
    ''' 変換元の文字は数値のみ有効。
    ''' </summary>
    ''' <param name="BaseChar">変換前文字列</param>
    ''' <param name="ByteLength">変換後のByte数</param>
    ''' <returns>変換後Byte配列</returns>
    Public Shared Function CHARtoBIN(ByVal BaseChar As String, ByVal ByteLength As Integer) As Byte()
        Dim bRtn() As Byte
        Dim nConv As UInt64
        Try
            If String.IsNullOrEmpty(BaseChar) Then BaseChar = "0"
            nConv = System.UInt64.Parse(BaseChar)
            bRtn = System.BitConverter.GetBytes(nConv)
            Array.Resize(bRtn, ByteLength)  'サイズ変更

            Return bRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "コード変換（BIN→文字）(インテル型（Little Endian））"
    ''' <summary>
    ''' [コード変換（BIN→文字）(インテル型（Little Endian））]
    ''' 変換後の文字は数値のみ有効。
    ''' </summary>
    ''' <param name="BaseByte">変換前Byte配列</param>
    ''' <returns>変換後文字列</returns>
    Public Shared Function BINtoCHAR(ByVal BaseByte() As Byte) As String
        Dim sRtn As String = ""
        Dim i As Integer
        Dim n As UInt64
        Dim nRtn As UInt64

        Try
            For i = 0 To BaseByte.Length - 1
                n = CType(BaseByte(i).ToString, UInt64)
                If i <> 0 Then
                    If n <> 0 Then
                        n = CType((256 ^ i) * n, UInt64)
                    End If
                End If
                nRtn = nRtn + n
            Next
            sRtn = CType(nRtn, String)
            Return sRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        Finally

        End Try
    End Function
#End Region

#Region "コード変換（文字→DEC）"
    ''' <summary>
    ''' [コード変換（文字→DEC）]
    ''' 変換前の文字は数値のみ有効。
    ''' 指定Byte数分ない場合は、左ゼロ埋めを実施。
    ''' </summary>
    ''' <param name="BaseChar">変換前文字列</param>
    ''' <param name="ByteLength">変換後のByte数</param>
    ''' <returns>変換後Byte配列</returns>
    Public Shared Function CHARtoDEC(ByVal BaseChar As String, ByVal ByteLength As Integer) As Byte()
        Dim bRtn() As Byte
        Dim i As Integer
        Dim sInf As String
        Dim sSet As String
        Dim sErr As String = ""
        Try
            If BaseChar.Length > ByteLength Then
                sErr = "変換前の文字が指定返却Byte指定数を超えています。" & vbCrLf & _
                       "CHARtoDEC[BaseChar=" & BaseChar & "][ByteLength=" & ByteLength.ToString & "]"
                Throw New System.ArgumentException(sErr)
            End If
            ReDim bRtn(ByteLength - 1)
            sInf = BaseChar.PadLeft(ByteLength, CType("0", Char))   '左ゼロ埋め
            For i = 0 To ByteLength - 1
                sSet = sInf.Substring(0, 1)
                'bRtn(i) = CType("&H00" + sSet, Byte)
                bRtn(i) = System.Convert.ToByte(sSet, 16)
                sInf = sInf.Substring(1)
            Next
            Return bRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "コード変換（DEC→文字）"
    ''' <summary>
    ''' [コード変換（DEC→文字）]
    ''' 変換後の文字は数値のみ有効。
    ''' </summary>
    ''' <param name="BaseByte">変換前Byte配列</param>
    ''' <returns>変換後文字列</returns>
    Public Shared Function DECtoCHAR(ByVal BaseByte() As Byte) As String
        Dim sRtn As String = ""
        Dim i As Integer
        Try
            For i = 0 To BaseByte.Length - 1
                'sRtn = sRtn + Hex(BaseByte(i)).PadLeft(2, CType("0", Char))
                sRtn = sRtn + Hex(BaseByte(i)).PadLeft(2, CType("0", Char)).Substring(1, 1)
            Next
            Return sRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "コード変換（文字→BCD）"
    ''' <summary>
    ''' [コード変換（文字→BCD）]
    ''' 変換前の文字は数値のみ有効。
    ''' 指定Byte数分ない場合は、左ゼロ埋めを実施。
    ''' </summary>
    ''' <param name="BaseChar">変換前文字列</param>
    ''' <param name="ByteLength">変換後のByte数</param>
    ''' <returns>変換後Byte配列</returns>
    Public Shared Function CHARtoBCD(ByVal BaseChar As String, ByVal ByteLength As Integer) As Byte()
        Dim bRtn() As Byte
        Dim i As Integer
        Dim sInf As String
        Dim sSet As String
        Dim sErr As String = ""
        Try
            If BaseChar.Length > ByteLength * 2 Then
                sErr = "変換前の文字が指定返却Byte指定数を超えています。" & vbCrLf & _
                       "CHARtoBCD[BaseChar=" & BaseChar & "][ByteLength=" & ByteLength.ToString & "]"
                Throw New System.ArgumentException(sErr)
            End If
            ReDim bRtn(ByteLength - 1)
            sInf = BaseChar.PadLeft(ByteLength * 2, CType("0", Char))   '左ゼロ埋め
            For i = 0 To ByteLength - 1
                sSet = sInf.Substring(0, 2)
                'bRtn(i) = CType("&H00" + sSet, Byte)
                bRtn(i) = System.Convert.ToByte(sSet, 16)
                sInf = sInf.Substring(2)
            Next
            Return bRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "コード変換（BCD→文字）"
    ''' <summary>
    ''' [コード変換（BCD→文字）]
    ''' 変換後の文字は数値のみ有効。
    ''' </summary>
    ''' <param name="BaseByte">変換前Byte配列</param>
    ''' <returns>変換後文字列</returns>
    Public Shared Function BCDtoCHAR(ByVal BaseByte() As Byte) As String
        Dim sRtn As String = ""
        Dim i As Integer
        Try
            For i = 0 To BaseByte.Length - 1
                sRtn = sRtn + Hex(BaseByte(i)).PadLeft(2, CType("0", Char))
            Next
            Return sRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "コード変換（BIN→BCD）[BINtoCHAR→CHARtoBCD]"
    ''' <summary>
    ''' [コード変換（BIN→BCD）]
    ''' 処理として、他コード変換のBINtoCHARとCHARtoBCDを実施。
    ''' [内部使用メソッド：BINtoCHAR,CHARtoBCD]
    ''' </summary>
    ''' <param name="BaseByte">変換前Byte配列</param>
    ''' <param name="ByteLength">変換後のByte数</param>
    ''' <returns>変換後Byte配列</returns>
    Public Shared Function BINtoBCD(ByVal BaseByte() As Byte, ByVal ByteLength As Integer) As Byte()
        Try
            Return CHARtoBCD(BINtoCHAR(BaseByte), ByteLength)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "コード変換（BCD→BIN）[BCDtoCHAR→CHARtoBIN]"
    ''' <summary>
    ''' [コード変換（BCD→BIN）]
    ''' 処理として、他コード変換のBCDtoCHARとCHARtoBINを実施。
    ''' [内部使用メソッド：BCDtoCHAR,CHARtoBIN]
    ''' </summary>
    ''' <param name="BaseByte">変換前Byte配列</param>
    ''' <param name="ByteLength">変換後のByte数</param>
    ''' <returns>変換後Byte配列</returns>
    Public Shared Function BCDtoBIN(ByVal BaseByte() As Byte, ByVal ByteLength As Integer) As Byte()
        Try
            Return CHARtoBIN(BCDtoCHAR(BaseByte), ByteLength)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "コード変換（S-JIS→JIS）"
    ''' <summary>
    ''' [コード変換（S-JIS→JIS）]
    ''' </summary>
    ''' <param name="BaseChar">変換前文字列</param>
    ''' <returns>変換後Byte配列</returns>
    Public Shared Function SJtoJIS(ByVal BaseChar As String) As Byte()
        Dim bRtn() As Byte
        Dim bInf() As Byte
        Dim nStartPos As Integer
        Dim nEndPos As Integer
        Dim i As Integer
        Dim n As Integer = 0
        Try
            bInf = System.Text.Encoding.GetEncoding(50220).GetBytes(BaseChar)
            If bInf.Length >= 8 Then
                If ((Hex(bInf(0)) = "1B") And _
                    (Hex(bInf(1)) = "24") And _
                    (Hex(bInf(2)) = "42")) Then
                    nStartPos = 3
                Else
                    nStartPos = 0
                End If
                If ((Hex(bInf(bInf.Length - 3)) = "1B") And _
                    (Hex(bInf(bInf.Length - 2)) = "28") And _
                    (Hex(bInf(bInf.Length - 1)) = "42")) Then
                    nEndPos = bInf.Length - 4
                Else
                    nEndPos = bInf.Length - 1
                End If
                ReDim bRtn(nEndPos - nStartPos)
                For i = nStartPos To nEndPos
                    bRtn(n) = bInf(i)
                    n += 1
                Next i
                Return bRtn
            Else
                bRtn = bInf
            End If
            Return bRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "コード変換（JIS→S-JIS）"
    ''' <summary>
    ''' [コード変換（JIS→S-JIS）]
    ''' </summary>
    ''' <param name="BaseChar">変換前文字列</param>
    ''' <returns>変換後文字列</returns>
    Public Shared Function JIStoSJ(ByVal BaseChar As String) As String
        Dim sRtn As String = BaseChar
        sRtn = BaseChar
        Try
            Dim bytesData As Byte() = System.Text.Encoding.GetEncoding(932).GetBytes(sRtn)
            sRtn = System.Text.Encoding.GetEncoding(50220).GetString(bytesData)
            Return sRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "ＤＢ接続文字列取得"
    ''' <summary>
    ''' [ＤＢ接続文字列取得]
    ''' 設定情報よりＤＢ接続情報を取得する。
    ''' </summary>
    ''' <returns>ＤＢ接続文字列取得</returns>
    Public Shared Function GetDbConnectString() As String
        Try
            Return String.Format("Server={0};Database={1};UID={2};PWD={3}", _
                                 BaseConfig.DatabaseServerName, _
                                 BaseConfig.DatabaseName, _
                                 BaseConfig.DatabaseUserName, _
                                 BaseConfig.DatabasePassword)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "イベントログ出力"
    ''' <summary>
    ''' [イベントログ出力]
    ''' ローカル コンピュータのアプリケーションログに出力する。
    ''' 出力内容の先頭に呼出元情報を出力する。
    ''' 改行マークと改行コードは半角スペース１桁に変換する。
    ''' </summary>
    ''' <param name="EntType">種類</param>
    ''' <param name="Detail">付加情報</param>
    ''' <param name="Souce_Name">呼出元クラス名</param>
    ''' <param name="Method_Name">呼出元メソッド名</param>
    Public Shared Sub WriteLogToEvent(ByVal EntType As EventLogEntryType, ByVal Detail As String, ByVal Souce_Name As String, ByVal Method_Name As String)
        Try
            WriteLogToEventCore(EntType, Detail, Souce_Name, Method_Name)
        Catch ex As Exception
            '何もしない
        End Try
    End Sub
    ''' <summary>
    ''' [イベントログ出力]
    ''' ローカル コンピュータのアプリケーションログに出力する。
    ''' 出力内容の先頭に呼出元情報を出力する。
    ''' 改行マークと改行コードは半角スペース１桁に変換する。
    ''' </summary>
    ''' <param name="EntType">種類</param>
    ''' <param name="Detail">付加情報</param>
    ''' <param name="Souce_Name">呼出元クラス名</param>
    ''' <param name="Method_Name">呼出元メソッド名</param>
    Private Shared Sub WriteLogToEventCore(ByVal EntType As EventLogEntryType, ByVal Detail As String, ByVal Souce_Name As String, ByVal Method_Name As String)
        Try
            Detail = Detail.Replace(vbCrLf, Space(1))
            Detail = String.Format("{0}[EVENT={1}]", Detail, Method_Name)

            'ローカル コンピュータのアプリケーションログに出力
            System.Diagnostics.EventLog.WriteEntry(Souce_Name, Detail, EntType)
        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try
    End Sub
#End Region

    ' <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< メソッド（ツール系）

#Region "フォルダ生成"
    ''' <summary>
    ''' [フォルダ生成]
    ''' </summary>
    ''' <param name="sFolderPath">対象パス</param>
    ''' <returns>True:成功,False:失敗(作成できないパス指定等)</returns>
    Public Shared Function MakeFolder(ByVal sFolderPath As String) As Boolean
        Try
            System.IO.Directory.CreateDirectory(sFolderPath)
            Return True
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Return False
        End Try
    End Function
#End Region

#Region "Null文字置換"
    ''' <summary>
    ''' [Null文字置換]
    ''' </summary>
    ''' <param name="oValue">チェックする値（DBのSelect結果の値が格納されるフィールド情報等）</param>
    ''' <param name="sConvStr">Null時、置換する文字列</param>
    ''' <returns>置換した文字列</returns>
    Public Shared Function CNull(ByVal oValue As Object, ByVal sConvStr As String) As String
        Try
            Return CNullCore(oValue, sConvStr)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
    ''' <summary>
    ''' [Null文字置換]
    ''' </summary>
    ''' <param name="oValue">チェックする値（DBのSelect結果の値が格納されるフィールド情報等）</param>
    ''' <param name="sConvStr">Null時、置換する文字列</param>
    ''' <returns>置換した文字列</returns>
    Private Shared Function CNullCore(ByVal oValue As Object, ByVal sConvStr As String) As String
        Dim sStr As String = ""
        Try
            If IsDBNull(oValue) Then
                sStr = sConvStr
            Else
                sStr = CStr(oValue)
            End If
            Return sStr
        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "シングルクォテーション付加"
    ''' <summary>
    ''' [シングルクォテーション付加]
    ''' 例：SetSglQuot("a'b''cd",",") → "'a''b''''cd',"
    ''' </summary>
    ''' <param name="sValue">チェックする値</param>
    ''' <param name="sLstStr">最終位置に付加する値</param>
    ''' <returns>付加した文字列</returns>
    Public Shared Function SetSglQuot(ByVal sValue As String, ByVal sLstStr As String) As String
        Try
            Return SetSglQuotCore(sValue, sLstStr)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
    ''' <summary>
    ''' [シングルクォテーション付加]
    ''' 例：SetSglQuot("a'b''cd") → "'a''b''''cd'"
    ''' </summary>
    ''' <param name="sValue">チェックする値</param>
    ''' <returns>付加した文字列</returns>
    Public Shared Function SetSglQuot(ByVal sValue As String) As String
        Try
            Return SetSglQuotCore(sValue, "")
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
    ''' <summary>
    ''' [シングルクォテーション付加]
    ''' 例：SetSglQuot("a'b''cd",",") → "'a''b''''cd',"
    ''' </summary>
    ''' <param name="sValue">チェックする値</param>
    ''' <param name="sLstStr">最終位置に付加する値</param>
    ''' <returns>付加した文字列</returns>
    Private Shared Function SetSglQuotCore(ByVal sValue As String, ByVal sLstStr As String) As String
        Dim sStr As String = ""
        Try
            sStr = sValue.Replace("'", "''")
            sStr = "'" & sStr & "'" & sLstStr
            Return sStr
        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "メソッド名取得"
    ''' <summary>
    ''' [メソッド名取得]
    ''' 呼び出し元メソッド等の名前を"メソッド名"の書式で返却します。
    ''' </summary>
    Public Shared Function MethodName() As String
        Return New StackTrace(0, True).GetFrame(1).GetMethod().Name
    End Function
#End Region

#Region "クラス名取得"
    ''' <summary>
    ''' [クラス名取得]
    ''' 呼び出し元メソッド等が所属するクラスの名前を"名前空間.クラス名"で返却します。
    ''' </summary>
    Public Shared Function ClsName() As String
        Return New StackTrace(0, True).GetFrame(1).GetMethod().DeclaringType.ToString
    End Function
#End Region

#Region "少数点以下丸め処理関数"
    ''' <summary>
    ''' 少数点以下丸め処理関数
    ''' </summary>
    ''' <param name="nValue">丸め対象値</param>
    ''' <param name="nRoundKbn">丸め区分(0:切捨て,1:四捨五入,2:切上げ,else:切捨て)</param>
    ''' <param name="nRoundPos">少数以下丸め位置(max5 5以上は5)</param>
    ''' <returns>丸め結果値</returns>
    ''' <remarks></remarks>
    Public Shared Function RoundValue(ByVal nValue As Double, ByVal nRoundKbn As Integer, ByVal nRoundPos As Integer) As Double
        Dim bMinus As Boolean
        Dim nVal As Double
        Dim nRetVal As Double

        If nValue = 0 Then
            '値0は処理なし
            Return nValue
        ElseIf nValue < 0 Then
            'マイナス退避
            nVal = nValue * -1
            bMinus = True
        Else
            nVal = nValue
            bMinus = False
        End If

        If nVal = System.Math.Floor(nVal) Then
            '整数なら丸めなし
            nRetVal = nVal
        Else
            '■以下丸め
            '丸め位置の最大は少数第５位
            If nRoundPos > 5 Then
                nRoundPos = 5
            End If

            '丸め位置で整数になるようにする（丸め位置0はそのまま）
            If (10 ^ nRoundPos) > 0 Then
                nVal = nVal * (10 ^ nRoundPos)
            End If

            '--丸め区分による丸め処理
            If nRoundKbn = 0 Then
                '切り捨て
                nRetVal = System.Math.Floor(nVal)
            ElseIf nRoundKbn = 1 Then
                '四捨五入
                nRetVal = System.Math.Floor(nVal + 0.5)
            ElseIf nRoundKbn = 2 Then
                '切り上げ
                If nVal <> System.Math.Floor(nVal) Then
                    nRetVal = System.Math.Floor(nVal) + 1
                Else
                    nRetVal = System.Math.Floor(nVal)
                End If
            Else
                '切り捨て
                nRetVal = System.Math.Floor(nVal)
            End If

            '丸め位置をもとにもどす（丸め位置0はそのまま）
            If (10 ^ nRoundPos) > 0 Then
                nRetVal = nRetVal / (10 ^ nRoundPos)
            End If
        End If


        'マイナス復帰
        If bMinus Then
            nRetVal = nRetVal * -1
        End If

        Return nRetVal

    End Function

#End Region

#Region "指定バイト位置から指定バイト数分のByte配列を取り出す"
    ''' <summary>指定バイト位置から指定バイト数分のByte配列を取り出す</summary>
    ''' <remarks>
    ''' 指定バイト位置から指定バイト数分のByte配列を取り出す
    ''' </remarks>
    ''' <param name="fromBytes">Byte配列</param>
    ''' <param name="startIndex">指定バイト位置</param>
    ''' <param name="resultLen">指定バイト数</param>
    ''' <returns>変換後Byte配列</returns>
    Public Shared Function GetBytesFromBytes(ByVal fromBytes As Byte(), ByVal startIndex As Integer, ByVal resultLen As Integer) As Byte()
        Dim bRtn(resultLen - 1) As Byte

        For i As Integer = 0 To resultLen - 1
            bRtn.SetValue((fromBytes.GetValue(startIndex + i)), i)
        Next

        Return bRtn
    End Function
#End Region

#Region "コード変換（正のInteger→BCD正数）"
    ''' <summary>
    ''' [コード変換（正のInteger→BCD正数）]
    ''' </summary>
    ''' <param name="src">変換元Integer値</param>
    ''' <param name="dst">書き込み先Byte配列</param>
    ''' <param name="pos">書き込み先Byte配列内の書き込み位置</param>
    ''' <param name="len">書き込みByte数</param>
    Public Shared Sub CopyIntToBcdBytes(ByVal src As Integer, ByVal dst As Byte(), ByVal pos As Integer, ByVal len As Integer)
        pos = pos + len
        For i As Integer = 1 To len
            Dim nextSrc As Integer = src \ 10
            Dim bin As Integer = src - nextSrc * 10
            src = nextSrc

            nextSrc = src \ 10
            bin = bin Or (src - nextSrc * 10) << 4
            src = nextSrc

            dst(pos - i) = CByte(bin)
        Next
    End Sub
#End Region

#Region "コード変換（BCD正数→正のInteger）"
    ''' <summary>
    ''' [コード変換（BCD正数→正のInteger）]
    ''' </summary>
    ''' <param name="src">変換元Byte配列</param>
    ''' <param name="pos">変換元Byte配列内の取得位置</param>
    ''' <param name="len">変換元Byte数</param>
    ''' <returns>変換後Integer</returns>
    Public Shared Function GetIntFromBcdBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Integer
        Dim ret As Integer = 0
        For i As Integer = pos To (pos + len - 1)
            Dim bcd As Integer = src(i)
            ret = ret * 10 + (bcd >> 4)
            ret = ret * 10 + (bcd And &H0f)
        Next
        Return ret
    End Function
#End Region

#Region "コード判定（BCD正数）"
    ''' <summary>
    ''' [コード判定（BCD正数）]
    ''' </summary>
    ''' <param name="src">判定Byte配列</param>
    ''' <param name="pos">判定Byte配列内の判定開始位置</param>
    ''' <param name="len">判定Byte数</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsBcdBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim bcd As Integer = src(i)
            If (bcd >> 4) > 9 OrElse (bcd And &H0f) > 9 Then Return False
        Next
        Return True
    End Function
#End Region

#Region "コード変換（正のInteger→アンパック型BCD正数）"
    ''' <summary>
    ''' [コード変換（正のInteger→アンパック型BCD正数）]
    ''' </summary>
    ''' <param name="src">変換元Integer値</param>
    ''' <param name="dst">書き込み先Byte配列</param>
    ''' <param name="pos">書き込み先Byte配列内の書き込み位置</param>
    ''' <param name="len">書き込みByte数</param>
    Public Shared Sub CopyIntToUnpackedBcdBytes(ByVal src As Integer, ByVal dst As Byte(), ByVal pos As Integer, ByVal len As Integer)
        pos = pos + len
        For i As Integer = 1 To len
            Dim nextSrc As Integer = src \ 10
            Dim bin As Integer = src - nextSrc * 10
            src = nextSrc
            dst(pos - i) = CByte(bin)
        Next
    End Sub
#End Region

#Region "コード変換（アンパック型BCD正数→正のInteger）"
    ''' <summary>
    ''' [コード変換（アンパック型BCD正数→正のInteger）]
    ''' </summary>
    ''' <param name="src">変換元Byte配列</param>
    ''' <param name="pos">変換元Byte配列内の取得位置</param>
    ''' <param name="len">変換元Byte数</param>
    ''' <returns>変換後Integer</returns>
    Public Shared Function GetIntFromUnpackedBcdBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Integer
        Dim ret As Integer = 0
        For i As Integer = pos To (pos + len - 1)
            Dim bcd As Integer = src(i)
            ret = ret * 10 + bcd
        Next
        Return ret
    End Function
#End Region

#Region "コード判定（アンパック型BCD正数）"
    ''' <summary>
    ''' [コード判定（アンパック型BCD正数）]
    ''' </summary>
    ''' <param name="src">判定Byte配列</param>
    ''' <param name="pos">判定Byte配列内の判定開始位置</param>
    ''' <param name="len">判定Byte数</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsUnpackedBcdBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim bcd As Integer = src(i)
            If bcd > 9 Then Return False
        Next
        Return True
    End Function
#End Region

#Region "コード変換（正のInteger→ASCII形式10進正数）"
    ''' <summary>
    ''' [コード変換（正のInteger→ASCII形式10進正数）]
    ''' </summary>
    ''' <param name="src">変換元Integer値</param>
    ''' <param name="dst">書き込み先Byte配列</param>
    ''' <param name="pos">書き込み先Byte配列内の書き込み位置</param>
    ''' <param name="len">書き込みByte数</param>
    Public Shared Sub CopyIntToDecimalAsciiBytes(ByVal src As Integer, ByVal dst As Byte(), ByVal pos As Integer, ByVal len As Integer)
        pos = pos + len
        For i As Integer = 1 To len
            Dim nextSrc As Integer = src \ 10
            dst(pos - i) = CByte(&H30 + (src - nextSrc * 10))
            src = nextSrc
        Next
    End Sub
#End Region

#Region "コード変換（ASCII形式10進正数→正のInteger）"
    ''' <summary>
    ''' [コード変換（ASCII形式10進正数→正のInteger）]
    ''' </summary>
    ''' <param name="src">変換元Byte配列</param>
    ''' <param name="pos">変換元Byte配列内の取得位置</param>
    ''' <param name="len">変換元Byte数</param>
    ''' <returns>変換後Integer</returns>
    Public Shared Function GetIntFromDecimalAsciiBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Integer
        Dim ret As Integer = 0
        For i As Integer = pos To (pos + len - 1)
            ret = ret * 10 + (src(i) - &H30)
        Next
        Return ret
    End Function
#End Region

#Region "コード判定（ASCII形式10進正数 後半ヌル文字許容）"
    ''' <summary>
    ''' [コード判定（ASCII形式10進正数 後半ヌル文字許容）]
    ''' </summary>
    ''' <param name="src">判定Byte配列</param>
    ''' <param name="pos">判定Byte配列内の判定開始位置</param>
    ''' <param name="len">判定Byte数</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsDecimalAsciiBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c >= &H30 AndAlso c <= &H39 Then Continue For
            If c <> &H00 Then Return False  '数字でもヌル文字でもない場合
            If i = pos Then Return False  '先頭の文字がヌル文字の場合

            'ヌル文字が出現して以降は、下記の処理で判定する。
            For j As Integer = i + 1 To (pos + len - 1)
                If src(j) <> &H00 Then Return False
            Next
            Return True
        Next
        Return True
    End Function
#End Region

#Region "コード判定（ASCII形式10進正数 ヌル文字不許可）"
    ''' <summary>
    ''' [コード判定（ASCII形式10進正数 ヌル文字不許可）]
    ''' </summary>
    ''' <param name="src">判定Byte配列</param>
    ''' <param name="pos">判定Byte配列内の判定開始位置</param>
    ''' <param name="len">判定Byte数</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsDecimalAsciiBytesFixed(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c < &H30 OrElse c > &H39 Then Return False
        Next
        Return True
    End Function
#End Region

#Region "コード判定（ASCII形式16進正数 後半ヌル文字許容）"
    ''' <summary>
    ''' [コード判定（ASCII形式16進正数 後半ヌル文字許容）]
    ''' </summary>
    ''' <param name="src">判定Byte配列</param>
    ''' <param name="pos">判定Byte配列内の判定開始位置</param>
    ''' <param name="len">判定Byte数</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsHexadecimalAsciiBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c >= &H30 AndAlso c <= &H39 Then Continue For
            If c >= &H41 AndAlso c <= &H46 Then Continue For
            If c >= &H61 AndAlso c <= &H66 Then Continue For
            If c <> &H00 Then Return False  '16進数字でもヌル文字でもない場合
            If i = pos Then Return False  '先頭の文字がヌル文字の場合

            'ヌル文字が出現して以降は、下記の処理で判定する。
            For j As Integer = i + 1 To (pos + len - 1)
                If src(j) <> &H00 Then Return False
            Next
            Return True
        Next
        Return True
    End Function
#End Region

#Region "コード判定（ASCII形式16進正数 ヌル文字不許可）"
    ''' <summary>
    ''' [コード判定（ASCII形式16進正数 ヌル文字不許可）]
    ''' </summary>
    ''' <param name="src">判定Byte配列</param>
    ''' <param name="pos">判定Byte配列内の判定開始位置</param>
    ''' <param name="len">判定Byte数</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsHexadecimalAsciiBytesFixed(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c >= &H30 AndAlso c <= &H39 Then Continue For
            If c >= &H41 AndAlso c <= &H46 Then Continue For
            If c >= &H61 AndAlso c <= &H66 Then Continue For
            Return False
        Next
        Return True
    End Function
#End Region

#Region "コード判定（ASCII形式可視文字列 後半ヌル文字許容）"
    ''' <summary>
    ''' [コード判定（ASCII形式可視文字列 後半ヌル文字許容）]
    ''' </summary>
    ''' <param name="src">判定Byte配列</param>
    ''' <param name="pos">判定Byte配列内の判定開始位置</param>
    ''' <param name="len">判定Byte数</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsVisibleAsciiBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c >= &H20 AndAlso c <= &H7E Then Continue For
            If c <> &H00 Then Return False  '可視文字でもヌル文字でもない場合

            'ヌル文字が出現して以降は、下記の処理で判定する。
            For j As Integer = i + 1 To (pos + len - 1)
                If src(j) <> &H00 Then Return False
            Next
            Return True
        Next
        Return True
    End Function
#End Region

#Region "コード判定（ASCII形式可視文字列 ヌル文字不許可）"
    ''' <summary>
    ''' [コード判定（ASCII形式可視文字列 ヌル文字不許可）]
    ''' </summary>
    ''' <param name="src">判定Byte配列</param>
    ''' <param name="pos">判定Byte配列内の判定開始位置</param>
    ''' <param name="len">判定Byte数</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsVisibleAsciiBytesFixed(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c < &H20 OrElse c > &H7E Then Return False
        Next
        Return True
    End Function
#End Region

#Region "コード変換（String型10進正数→正のInteger）"
    'NOTE: String.Substring()がヒープの操作等を行わない（高速）ならいらない。
    ''' <summary>
    ''' [コード変換（String型10進正数→正のInteger）]
    ''' </summary>
    ''' <param name="src">変換元String</param>
    ''' <param name="pos">変換元String内の取得位置</param>
    ''' <param name="len">変換元文字数</param>
    ''' <returns>変換後Integer</returns>
    Public Shared Function GetIntFromDecimalString(ByVal src As String, ByVal pos As Integer, ByVal len As Integer) As Integer
        Dim ret As Integer = 0
        For i As Integer = pos To (pos + len - 1)
            ret = ret * 10 + Val(src.Chars(i))
        Next
        Return ret
    End Function
#End Region

#Region "コード判定（String型10進正数 後半ヌル文字許容）"
    ''' <summary>
    ''' [コード判定（String型10進正数 後半ヌル文字許容）]
    ''' </summary>
    ''' <param name="src">判定String</param>
    ''' <param name="pos">判定String内の判定開始位置</param>
    ''' <param name="len">判定文字数</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsDecimalString(ByVal src As String, ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Integer = AscW(src.Chars(i))
            If c >= &H30 AndAlso c <= &H39 Then Continue For
            If c <> &H00 Then Return False '数字でもヌル文字でもない場合
            If i = pos Then Return False '先頭の文字がヌル文字の場合

            'ヌル文字が出現して以降は、下記の処理で判定する。
            For j As Integer = i + 1 To (pos + len - 1)
                If AscW(src.Chars(j)) <> &H00 Then Return False
            Next
            Return True
        Next
        Return True
    End Function
#End Region

#Region "コード判定（String型10進正数 ヌル文字不許可）"
    ''' <summary>
    ''' [コード判定（String型10進正数 ヌル文字不許可）]
    ''' </summary>
    ''' <param name="src">判定String</param>
    ''' <param name="pos">判定String列内の判定開始位置</param>
    ''' <param name="len">判定文字数</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsDecimalStringFixed(ByVal src As String, ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Integer = AscW(src.Chars(i))
            If c < &H30 OrElse c > &H39 Then Return False
        Next
        Return True
    End Function
#End Region

#Region "フィル"
    ''' <summary>
    ''' [フィル]
    ''' </summary>
    ''' <param name="val">フィル値</param>
    ''' <param name="dst">フィル対象Byte配列</param>
    ''' <param name="pos">フィル開始位置</param>
    ''' <param name="len">フィルByte数</param>
    Public Shared Sub FillBytes(ByVal val As Byte, ByVal dst As Byte(), ByVal pos As Integer, ByVal len As Integer)
        For i As Integer = pos To (pos + len - 1)
            dst(i) = val
        Next
    End Sub
#End Region

#Region "バイトオーダ保証BIN変換"
    Public Shared Function GetUInt32FromLeBytes4(ByVal aBytes As Byte(), ByVal offset As Integer) As UInteger
        Return CUInt(aBytes(offset)) Or CUInt(aBytes(offset + 1)) << 8 Or CUInt(aBytes(offset + 2)) << 16 Or CUInt(aBytes(offset + 3)) << 24
    End Function

    Public Shared Function GetUInt32FromLeBytes3(ByVal aBytes As Byte(), ByVal offset As Integer) As UInteger
        Return CUInt(aBytes(offset)) Or CUInt(aBytes(offset + 1)) << 8 Or CUInt(aBytes(offset + 2)) << 16
    End Function

    Public Shared Function GetUInt16FromLeBytes2(ByVal aBytes As Byte(), ByVal offset As Integer) As UShort
        Return CUShort(aBytes(offset)) Or CUShort(aBytes(offset + 1)) << 8
    End Function

    Public Shared Sub CopyUInt32ToLeBytes4(ByVal src As UInteger, ByVal aBytes As Byte(), ByVal offset As Integer)
        aBytes(offset) = CByte(src And &HFF)
        aBytes(offset + 1) = CByte(src >> 8 And &HFF)
        aBytes(offset + 2) = CByte(src >> 16 And &HFF)
        aBytes(offset + 3) = CByte(src >> 24 And &HFF)
    End Sub

    Public Shared Sub CopyUInt16ToLeBytes2(ByVal src As UShort, ByVal aBytes As Byte(), ByVal offset As Integer)
        aBytes(offset) = CByte(src And &Hff)
        aBytes(offset + 1) = CByte(src >> 8 And &Hff)
    End Sub
#End Region

#Region "CRC-16算出"
    Public Shared Function CalculateCRC16(ByVal buf As Byte(), ByVal pos As Integer, ByVal len As Integer) As UShort
        Const polynomial As UInteger = &H1800500
        Dim sum As UInteger = 0

        For i As Integer = pos To (pos + len - 1)
            sum = sum Or buf(i)
            For j As Integer = 0 To 7
                sum = sum << 1
                If (sum And &H1000000) <> 0 Then
                    sum = sum Xor polynomial
                End If
            Next
        Next

        For i As Integer = 0 To 1  'last 2 byte zero
            sum = CUInt(sum Or &H00)
            For j As Integer = 0 To 7
                sum = sum << 1
                If (sum And &H1000000) <> 0 Then
                    sum = sum Xor polynomial
                End If
            Next
        Next

        Return CUShort((sum >> 8) And &H0000ffff)
    End Function
#End Region

#Region "MD5算出"
    'NOTE: ファイルを開けないまたは読めない場合は、何らかの例外をスローします。
    Public Shared Function CalculateMD5(ByVal sFilePath As String) As String
        Dim aHashValue As Byte()
        Using oStream As New System.IO.FileStream(sFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)
            Dim oHasher As New System.Security.Cryptography.MD5CryptoServiceProvider()
            aHashValue = oHasher.ComputeHash(oStream)
        End Using
        Return System.BitConverter.ToString(aHashValue).Replace("-", "")
    End Function
#End Region

#Region "C言語リテラル形式文字列の翻訳（改行コード任意）"
    ''' <summary>
    ''' [C言語リテラル形式文字列の翻訳（改行コード任意）]
    ''' </summary>
    ''' <param name="sLiteral">C言語リテラル形式文字列</param>
    ''' <returns>翻訳後の文字列</returns>
    Public Shared Function TranslateClangLiteral(ByVal sLiteral As String) As String
        Dim lastIndex As Integer = sLiteral.Length - 1
        Dim i As Integer = 0
        Dim aDst(lastIndex) As Char
        Dim dstIndex As Integer = 0
        While i <= lastIndex
            Dim c As Char = sLiteral.Chars(i)
            If c.Equals("\"c) Then
                i += 1
                If i > lastIndex Then
                    Throw New ArgumentException("The argument contains bad escape sequence (solitary back slash).")
                End If
                Dim c2 As Char = sLiteral.Chars(i)
                If c2.Equals("\"c) Then
                    aDst(dstIndex) = "\"c
                ElseIf c2.Equals("n"c) Then
                    aDst(dstIndex) = ControlChars.Lf
                ElseIf c2.Equals("r"c) Then
                    aDst(dstIndex) = ControlChars.Cr
                Else
                    Throw New ArgumentException("The argument contains indecipherable escape sequence.")
                End If
            Else
                aDst(dstIndex) = c
            End If
            dstIndex += 1
            i += 1
        End While
        Return New String(aDst, 0, dstIndex)
    End Function
#End Region

#Region "C言語リテラル形式文字列の翻訳（改行コードCRLF固定）"
    ''' <summary>
    ''' [C言語リテラル形式文字列の翻訳（改行コードCRLF固定）]
    ''' </summary>
    ''' <param name="sLiteral">C言語リテラル形式文字列</param>
    ''' <returns>翻訳後の文字列</returns>
    Public Shared Function TranslateClangLiteralToDosText(ByVal sLiteral As String) As String
        Dim lastIndex As Integer = sLiteral.Length - 1
        Dim i As Integer = 0
        Dim aDst(lastIndex) As Char
        Dim dstIndex As Integer = 0
        While i <= lastIndex
            Dim c As Char = sLiteral.Chars(i)
            If c.Equals("\"c) Then
                i += 1
                If i > lastIndex Then
                    Throw New ArgumentException("The argument contains bad escape sequence (solitary back slash).")
                End If
                Dim c2 As Char = sLiteral.Chars(i)
                If c2.Equals("\"c) Then
                    aDst(dstIndex) = "\"c
                ElseIf c2.Equals("n"c) Then
                    aDst(dstIndex) = ControlChars.Cr
                    dstIndex += 1
                    aDst(dstIndex) = ControlChars.Lf
                Else
                    Throw New ArgumentException("The argument contains indecipherable escape sequence.")
                End If
            Else
                aDst(dstIndex) = c
            End If
            dstIndex += 1
            i += 1
        End While
        Return New String(aDst, 0, dstIndex)
    End Function
#End Region

#Region "ファイルパス関連"
    ''' <summary>
    ''' [仮想パスの有効性を判定]
    ''' </summary>
    ''' <param name="sVirtualPath">仮想パス</param>
    ''' <returns>判定結果</returns>
    ''' <remarks>
    ''' 仮想パスとは、仮想的なファイルシステムにおける絶対または相対パスのことである。
    ''' </remarks>
    Public Shared Function IsValidVirtualPath(ByVal sVirtualPath As String) As Boolean
        'ドライブ指定があるパスは、ローカルファイルシステムのパスに
        '結合できないので、仮想パスとして無効とみなす。
        If sVirtualPath.Contains(":") Then
            Return False
        End If

        '「\\」や「//」ではじまるパスは、ローカルファイルシステムのパスに
        '結合できない（結合するときに、最初の「\」をとっても、絶対パスに
        'みえてしまい、厄介である）ので、仮想パスとして無効とみなす。
        If sVirtualPath.Length >= 2 Then
            If sVirtualPath.Chars(1) = System.IO.Path.DirectorySeparatorChar OrElse _
               sVirtualPath.Chars(1) = System.IO.Path.AltDirectorySeparatorChar Then
                Return False
            End If
        End If

        'パス一般として無効なパスは、仮想パスとしても無効とみなす。
        Try
            System.IO.Path.GetDirectoryName(sVirtualPath)
        Catch ex As Exception
            Return False
        End Try

        '「\..」を含むパスは、親ディレクトリ（仮想ファイルシステムの外部）を
        '指しかねないという意味で不正な可能性はあるが、結合は可能なので、
        '仮想パスとしても無効とはみなさない。
        'NOTE: アクセスされたくないディレクトリを指していないかは、
        'IsAncestPath(アクセス許可パス, 結合後のパス)でチェックすること。

        Return True
    End Function

    ''' <summary>
    ''' [仮想パスをパスに結合]
    ''' </summary>
    ''' <param name="sPath">パス</param>
    ''' <param name="sVirtualPath">仮想パス</param>
    ''' <returns>結合したパス</returns>
    ''' <remarks>
    ''' 仮想パスとは、仮想的なファイルシステムにおける絶対または相対パスのことである。
    ''' 仮想的なファイルシステムにおけるものであっても、相対パスであることが確実な場合は、
    ''' このメソッドを使う必要はなく、System.IO.Path.Combine()を使えばよい。
    ''' 戻り値の用途によっては、sVirtualPathが有効な仮想パスであることを
    ''' IsValidVirtualPath()でチェックしておくことを推奨する。
    ''' sVirtualPathが有効な仮想パスでない場合、このメソッドの戻り値は
    ''' sVirtualPathになる危険性がある。
    ''' </remarks>
    Public Shared Function CombinePathWithVirtualPath(ByVal sPath As String, ByVal sVirtualPath As String) As String
        If sVirtualPath.Chars(0) = System.IO.Path.DirectorySeparatorChar OrElse _
           sVirtualPath.Chars(0) = System.IO.Path.AltDirectorySeparatorChar Then
            sVirtualPath = sVirtualPath.Remove(0, 1)
        End If
        Return System.IO.Path.Combine(sPath, sVirtualPath)
    End Function

    ''' <summary>
    ''' [親または先祖ディレクトリか判定]
    ''' </summary>
    ''' <param name="sSuperPath">判定対象パス（正規化済みフルパス）</param>
    ''' <param name="sSubPath">比較対象パス（正規化済みフルパス）</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsAncestPath(ByVal sSuperPath As String, ByVal sSubPath As String) As Boolean
        Dim i As Integer = 0
        Dim max As Integer = (sSubPath.Length + 1) \ 2
        sSuperPath = sSuperPath.ToUpperInvariant()
        sSubPath = sSubPath.ToUpperInvariant()
        Try
            sSubPath = System.IO.Path.GetDirectoryName(sSubPath)
            While sSubPath IsNot Nothing AndAlso i < max
                If sSubPath.Equals(sSuperPath) Then Return True
                sSubPath = System.IO.Path.GetDirectoryName(sSubPath)
            End While
        Catch ex As System.IO.IOException
            Return False
        Catch ex As ArgumentException
            Return False
        End Try
        Return False
    End Function
#End Region

#Region "指定ディレクトリ内の全サブディレクトリ・全ファイルに指定属性を追加"
    Public Shared Sub AddAttributesToDirectoryDescendants(ByVal sDirPath As String, ByVal attrs As System.IO.FileAttributes)
        Dim aSubDirs As String() = System.IO.Directory.GetDirectories(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sSubDir As String In aSubDirs
            Dim curAttrs As System.IO.FileAttributes = System.IO.File.GetAttributes(sSubDir)
            Dim newAttrs As System.IO.FileAttributes = curAttrs Or attrs
            If newAttrs <> curAttrs Then
                System.IO.File.SetAttributes(sSubDir, newAttrs)
            End If
        Next sSubDir

        Dim aFiles As String() = System.IO.Directory.GetFiles(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sFile As String In aFiles
            Dim curAttrs As System.IO.FileAttributes = System.IO.File.GetAttributes(sFile)
            Dim newAttrs As System.IO.FileAttributes = curAttrs Or attrs
            If newAttrs <> curAttrs Then
                System.IO.File.SetAttributes(sFile, newAttrs)
            End If
        Next sFile
    End Sub
#End Region

#Region "指定ディレクトリ内の全サブディレクトリ・全ファイルから指定属性を解除"
    Public Shared Sub RemoveAttributesFromDirectoryDescendants(ByVal sDirPath As String, ByVal attrs As System.IO.FileAttributes)
        Dim aDirs As String() = System.IO.Directory.GetDirectories(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sDir As String In aDirs
            Dim curAttrs As System.IO.FileAttributes = System.IO.File.GetAttributes(sDir)
            Dim newAttrs As System.IO.FileAttributes = curAttrs And (Not attrs)
            If newAttrs <> curAttrs Then
                System.IO.File.SetAttributes(sDir, newAttrs)
            End If
        Next sDir

        Dim aFiles As String() = System.IO.Directory.GetFiles(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sFile As String In aFiles
            Dim curAttrs As System.IO.FileAttributes = System.IO.File.GetAttributes(sFile)
            Dim newAttrs As System.IO.FileAttributes = curAttrs And (Not attrs)
            If newAttrs <> curAttrs Then
                System.IO.File.SetAttributes(sFile, newAttrs)
            End If
        Next sFile
    End Sub
#End Region

#Region "一時作業用ディレクトリの削除"
    ''' <summary>
    ''' [一時作業用ディレクトリの削除]
    ''' </summary>
    ''' <param name="sDirPath">ディレクトリのパス</param>
    ''' <remarks>
    ''' 指定されたディレクトリが存在しない場合は何もしない。
    ''' ディレクトリ内のアイテムに読み取り専用属性が付与されている場合も
    ''' まとめて削除する。
    ''' ディレクトリやディレクトリ内のアイテムを作成・削除・変更するのは、
    ''' 現在のプロセスの呼び元のスレッド（またはそれとシーケンシャルに
    ''' 動作するスレッド）のみであることが前提である。
    ''' それが守られない場合や、ディレクトリと同名のファイルが存在している
    ''' 場合や、指定のディレクトリ自体に読み取り専用属性が付与されている
    ''' 場合は、何らかの例外スローし得る。
    ''' </remarks>
    Public Shared Sub DeleteTemporalDirectory(ByVal sDirPath As String)
        If System.IO.Directory.Exists(sDirPath) Then
            RemoveAttributesFromDirectoryDescendants(sDirPath, System.IO.FileAttributes.ReadOnly)
            System.IO.Directory.Delete(sDirPath, True)
        End If
    End Sub
#End Region

#Region "ディレクトリの初期化"
    ''' <summary>
    ''' [ディレクトリの初期化]
    ''' </summary>
    ''' <param name="sDirPath">ディレクトリのパス</param>
    ''' <remarks>
    ''' 指定されたディレクトリをできる限りで空にする。
    ''' 権限等の関係で削除できないものがあっても、その削除のみを諦めて、
    ''' 他のサブディレクトリやファイルの削除は試行する。
    ''' 削除できないものがある場合はLogクラスを使って記録するため、
    ''' Logクラスのメソッドは、本メソッドを使用してはならない。
    ''' </remarks>
    Public Shared Sub CleanUpDirectory(ByVal sDirPath As String)
        'OPT: 最悪の場合にディレクトリ数の階乗オーダの処理を行うことになるため、
        '効率に問題があるかもしれない。現実的に問題があるなら、再帰呼び出しを
        '自前で行ってでも処理量を最小化するべきである。

        Dim aSubDirs As String() = System.IO.Directory.GetDirectories(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sSubDir As String In aSubDirs
            If System.IO.Directory.Exists(sSubDir) Then
                Try
                    System.IO.Directory.Delete(sSubDir, True)
                Catch ex As Exception
                    'NOTE: 削除できないファイルや削除できないサブディレクトリが１つでもあれば、
                    'sSubDir内部に未処理の（削除を試みていない）ファイルやサブディレクトリを
                    '残したまま次のsSubDirの処理に移行することになる。
                    'ただし、未処理のサブディレクトリで削除できるものは、このループの次以降の
                    'sSubDirの処理で削除することになる。また、未処理のファイルで削除できるもの
                    'は、この後のsFileのループで削除することになる。
                    '結果として、残るのは、それ自体が削除できないようになっているファイルや
                    'ディレクトリと、それらを格納する上で必要な最小限のディレクトリだけである。
                End Try
            End If
        Next sSubDir

        Dim aFiles As String() = System.IO.Directory.GetFiles(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sFile As String In aFiles
            Try
                System.IO.File.Delete(sFile)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        Next sFile
    End Sub
#End Region

End Class
