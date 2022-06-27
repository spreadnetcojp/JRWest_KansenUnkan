' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2013/11/07  (NES)河脇  フェーズ２対応
'                                   ・SNMPTrap対象及びメール対象対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports System.IO
Imports JR.ExOpmg.DBCommon
Imports System.Text

Public Class BatchAppComm

#Region "メソッド（Public）"

    ''' <summary>
    ''' １０進数を０ｻﾌﾟﾚｽしない１６進数の形に変換する
    ''' </summary>
    ''' <param name="bytDat10">１ﾊﾞｲﾄﾃﾞｰﾀ</param>
    ''' <returns>１６進数文字列</returns>
    ''' <remarks>１ﾊﾞｲﾄﾃﾞｰﾀの０ｻﾌﾟﾚｽ無し１６進数表記</remarks>
    Public Shared Function fnHexDisp(ByVal bytDat10 As Byte) As String

        '常時２桁で返す
        If Len(Hex(bytDat10)) <= 1 Then     '１桁ならば
            fnHexDisp = "0" & Hex(bytDat10)   '０ｻﾌﾟﾚｽしない
        Else                                '２桁ならば
            fnHexDisp = Hex(bytDat10)         'そのまま
        End If

    End Function

    ''' <summary>
    ''' DATファイルの解析
    ''' </summary>
    ''' <param name="iniInfoAry">INIファイルの内容</param>
    ''' <param name="datFileName">datファイル名</param>
    ''' <param name="clientKind"></param>
    ''' <param name="redLen">データ部サイズ</param>
    ''' <param name="headLen">ヘッダ部サイズ</param>
    ''' <param name="lineInfoLst">解析したデータ</param>
    ''' <param name="dataKind">データ種別</param>
    ''' <param name="isFtpData">true:FTPデータ;false:電文データ</param>
    ''' <param name="isGet">true:データ種別による値取得;false:値取得しない</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Public Shared Function GetInfoFromDataFileComm(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                                   ByVal datFileName As String, _
                                                   ByVal clientKind As String, _
                                                   ByVal redLen As Integer, _
                                                   ByVal headLen As Integer, _
                                                   ByRef lineInfoLst As List(Of String()), _
                                                   ByVal dataKind As String, _
                                                   Optional ByRef isFtpData As Boolean = False, _
                                                   Optional ByVal isGet As Boolean = False) As Boolean
        'Ver0.1 ↑関数パラメータにisFtpDataを追加（SNMPTrap対象及びメール対象対応）

        '登録用の基本情報
        Dim headInfo As RecDataStructure.BaseInfo = Nothing

        '１レコード分の生データを一時的に読み込むための領域
        Dim bData(redLen + headLen) As Byte

        '解析した情報を返却するためのリストを生成または初期化
        If lineInfoLst Is Nothing Then
            lineInfoLst = New List(Of String())
        Else
            lineInfoLst.Clear()
        End If

        Dim fileStream As FileStream
        Try
            'ファイルストリームを取得
            fileStream = New FileStream(datFileName, FileMode.Open)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Try
            'ファイルサイズが１レコード分に満たない場合
            If fileStream.Length < (redLen + headLen) Then
                Log.Error(RecAppConstants.ERR_TOO_SHORT_FILE)

                CollectedDataTypoRecorder.Record( _
                   New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                   DbConstants.CdtKindServerError, _
                   Lexis.CdtReadingTotallyFailed.Gen(dataKind, Path.GetFileNameWithoutExtension(datFileName)))
                Return False
            End If

            'レコード数取得
            Dim iRecCnt As Integer = CInt(fileStream.Length \ (redLen + headLen))

            '開始レコードindex取得
            Dim iStarRecIndex As Integer
            If fileStream.Length > (redLen + headLen) Then
                'FTPで取得したファイルの場合は、先頭のレコードは読まない。
                iStarRecIndex = 1
                fileStream.Seek(redLen + headLen, SeekOrigin.Begin)
                'Ver0.1 ADD SNMPTrap対象及びメール対象対応
                isFtpData = True
            Else
                '電文として取得したファイルの場合は、先頭のレコードを読む。
                iStarRecIndex = 0
                'Ver0.1 ADD SNMPTrap対象及びメール対象対応
                isFtpData = False
            End If

            Dim skipped As Boolean = False '解析できないレコードの有無

            '１レコード単位でデータを読み取り、解析する。
            For i As Integer = iStarRecIndex To iRecCnt - 1
                fileStream.Read(bData, 0, redLen + headLen)
                headInfo = Nothing
                BinaryHeadInfoParse.GetBaseInfo(bData, clientKind, headInfo)

                If isGet = True Then
                    If (Not dataKind = "") AndAlso (Not dataKind = headInfo.DATA_KIND) Then
                        Continue For
                    End If
                End If

                '１レコード分の解析結果を格納するための領域を生成
                Dim lineInfo As String() = New String(iniInfoAry.Length - 1) {}

                'データの解析
                If GetRecDataComm(iniInfoAry, bData, headInfo, lineInfo) = False Then
                    Log.Error(String.Format(RecAppConstants.ERR_INVALID_RECORD, i.ToString()))

                    'NOTE: ここでは収集データ誤記は登録しない。
                    '通常の収集データ誤記は、その駅コード等が機器構成に存在する保証が
                    '無い以上は、登録しても、運管端末から見えない可能性がある。
                    'そして、ここで捨てたレコードについては、この後に行う機器存在チェック
                    'の対象にならないため、登録した誤記が端末で閲覧できないものである場合
                    'において、代わりに「機器が存在しません」という異常が登録されるという
                    'わけではない。よって、ここで捨てるレコードについては、通常の
                    '収集データ誤記に登録するのではなく、サーバ内異常に登録する。
                    skipped = True
                    Continue For
                End If

                lineInfoLst.Add(lineInfo)
            Next

            'ファイルの長さがレコード長の倍数でない場合
            If fileStream.Length Mod (redLen + headLen) <> 0 Then
                Log.Error(RecAppConstants.ERR_FILE_ROUNDED_OFF)
                skipped = True
            End If

            If skipped Then
                CollectedDataTypoRecorder.Record( _
                   New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                   DbConstants.CdtKindServerError, _
                   Lexis.CdtReadingPartiallyFailed.Gen(dataKind, Path.GetFileNameWithoutExtension(datFileName)))

                'TODO: 本当は、ここに該当したファイルは、Normalディレクトリではなく
                'Skippedディレクトリ等に移動させたい（RecordToDatabaseからIOError等で
                '戻りたい）。しかし、それを実現するには、これら登録系プロセスの
                '内部メソッドの戻り値がTrueかFalseの二択であるという問題から
                '解決しなければならない。
            End If

            Return True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'NOTE: この類の箇所では、上記した（GetRecDataComm()の戻り値がFalseの
            'ケースの）事情だけでなく、headInfo構造体のString参照型メンバに
            'Nothingがセットされていることがあり得るため、SetCollectionDataの
            '呼び出し（収集データ誤記の登録）は行わないこととする。
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(dataKind, Path.GetFileNameWithoutExtension(datFileName)))

            Return False
        Finally
            'ファイルストリームを解放
            fileStream.Close()
        End Try

    End Function

    ''' <summary>
    ''' DATファイルの１レコード取得
    ''' </summary>
    ''' <param name="iniInfoAry">iniファイル情報</param>
    ''' <param name="bData">データ情報</param>
    ''' <param name="headInfo">ヘッダ情報</param>
    ''' <param name="lineInfo">１レコードの内容</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Public Shared Function GetRecDataComm(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                      ByVal bData() As Byte, _
                                      ByVal headInfo As RecDataStructure.BaseInfo, _
                                      ByRef lineInfo() As String) As Boolean

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0

        Dim strData As String = ""               'HEX 又は BCDデータ
        Dim lBinData As Long = 0                 'binaryデータ
        Dim iPower As Integer = 0                '累乗


        '窓処 在来線区駅順⇒幹線線区駅順 変換対応
        Dim CHK_Count As Integer = 0            'チェック合致数

        Try
            For j = 0 To iniInfoAry.Length - 1
                'ヘッダ部のフィールドである場合
                '窓処 在来線区駅順⇒幹線線区駅順 変換対応 MOD START
                '　（線区：119の駅順：003の窓処の稼動保守か異常データか？）
                'Select Case UCase(iniInfoAry(j).FIELD_NAME)
                '    Case "DATA_KIND" 'データ種別
                '        lineInfo(j) = headInfo.DATA_KIND
                '        Continue For
                '    Case "MODEL_CODE" '機種
                '        lineInfo(j) = headInfo.MODEL_CODE
                '        Continue For
                '    Case "RAIL_SECTION_CODE"  'サイバネ線区コード
                '        lineInfo(j) = headInfo.STATION_CODE.RAIL_SECTION_CODE
                '        Continue For
                '    Case "STATION_ORDER_CODE"  'サイバネ駅順コード
                '        lineInfo(j) = headInfo.STATION_CODE.STATION_ORDER_CODE
                '        Continue For
                '    Case "CORNER_CODE"  'コーナーコード
                '        lineInfo(j) = headInfo.CORNER_CODE
                '        Continue For
                'End Select
                Select Case UCase(iniInfoAry(j).FIELD_NAME)
                    Case "DATA_KIND" 'データ種別
                        lineInfo(j) = headInfo.DATA_KIND
                        If (headInfo.DATA_KIND = "C3") Or (headInfo.DATA_KIND = "A6") Or (headInfo.DATA_KIND = "A7") Then
                            'C3:異常データ、A6:異常データ（再収集）、A7:稼動データ　ならカウント
                            CHK_Count = CHK_Count + 1
                        End If
                        Continue For
                    Case "MODEL_CODE" '機種
                        lineInfo(j) = headInfo.MODEL_CODE
                        If (headInfo.MODEL_CODE = "Y") Then
                            'Y:窓処　ならカウント
                            CHK_Count = CHK_Count + 1
                        End If
                        Continue For
                    Case "RAIL_SECTION_CODE"  'サイバネ線区コード
                        lineInfo(j) = headInfo.STATION_CODE.RAIL_SECTION_CODE
                        If (headInfo.STATION_CODE.RAIL_SECTION_CODE = "119") Then
                            '119:博多南　ならカウント
                            CHK_Count = CHK_Count + 1
                        End If
                        Continue For
                    Case "STATION_ORDER_CODE"  'サイバネ駅順コード
                        lineInfo(j) = headInfo.STATION_CODE.STATION_ORDER_CODE
                        If (headInfo.STATION_CODE.STATION_ORDER_CODE = "003") Then
                            '003:博多南　ならカウント
                            CHK_Count = CHK_Count + 1
                        End If
                        Continue For
                    Case "CORNER_CODE"  'コーナーコード
                        lineInfo(j) = headInfo.CORNER_CODE
                        Continue For
                End Select
                '窓処 在来線区駅順⇒幹線線区駅順 変換対応 MOD END

                If iniInfoAry(j).BYTE_LEN = 0 Then
                    lineInfo(j) = ""
                    Continue For
                End If

                'データ部のフィールドである場合
                Dim dataFormat As String = UCase(iniInfoAry(j).DATA_FORMAT)
                Select Case dataFormat
                    Case "HEX", "BCD"
                        strData = ""
                        If iniInfoAry(j).BIT_LEN = 0 Then
                            For k = 0 To iniInfoAry(j).BYTE_LEN - 1
                                strData = strData & fnHexDisp(bData(iniInfoAry(j).BYTE_OFFSET + k))
                            Next
                        Else
                            'bit操作が必要な場合
                            If iniInfoAry(j).BYTE_LEN = 1 Then
                                strData = GetBitValueFromByte("BCD", bData(iniInfoAry(j).BYTE_OFFSET), iniInfoAry(j).BIT_OFFSET, iniInfoAry(j).BIT_LEN)
                            End If
                        End If

                        If dataFormat = "BCD" Then
                            'NOTE: 項目の意味に依存しない（データフォーマットにのみ依存する）チェックは、
                            'CheckDataComm()ではなく、このメソッド内で行ってしまうことにする。
                            'NOTE: 文字列として保持されている値を文字列よりNarrowな別の形式に変換する
                            '際に（必要に応じて）チェックするという考え方もあるかもしれないが、
                            '問題のあるレコードのみを登録対象外とするためには、ListにレコードのAddを行うここか
                            '呼び元が新たなListを用意しているCheckDataComm()にてチェックを行うのが妥当である。
                            For Each c As Char In strData
                                '数字以外の文字が含まれているか調べる。
                                If c < "0"c OrElse "9"c < c Then
                                    Log.Error(String.Format(RecAppConstants.ERR_INVALID_FIELD_AS_BCD, iniInfoAry(j).KOMOKU_NAME))
                                    Return False
                                End If
                            Next c
                        End If

                        lineInfo(j) = strData

                    Case "BIN"
                        lBinData = 0
                        If iniInfoAry(j).BIT_LEN = 0 Then
                            'OPT: 以下の無駄は、処理内容や処理頻度にから考えると、かなり問題があると思われる。
                            If iniInfoAry(j).PARA6.Trim.Equals("1") Then
                                'インテル形式
                                iPower = iniInfoAry(j).BYTE_LEN - 1
                                For k = iniInfoAry(j).BYTE_LEN - 1 To 0 Step -1
                                    lBinData = lBinData + CLng((bData(iniInfoAry(j).BYTE_OFFSET + k)) * (256 ^ iPower))
                                    iPower = iPower - 1
                                Next
                            Else
                                iPower = iniInfoAry(j).BYTE_LEN - 1
                                For k = 0 To iniInfoAry(j).BYTE_LEN - 1
                                    lBinData = lBinData + CLng((bData(iniInfoAry(j).BYTE_OFFSET + k)) * (256 ^ iPower))
                                    iPower = iPower - 1
                                Next
                            End If
                        Else
                            'bit操作が必要な場合
                            If iniInfoAry(j).BYTE_LEN = 1 Then
                                strData = GetBitValueFromByte("BIN", bData(iniInfoAry(j).BYTE_OFFSET), iniInfoAry(j).BIT_OFFSET, iniInfoAry(j).BIT_LEN)
                                lBinData = Integer.Parse(strData)
                            End If

                        End If
                        lineInfo(j) = lBinData.ToString

                    Case "S-JIS"
                        '領域のバイト長をdataLenに取得。
                        Dim dataLen As Integer = iniInfoAry(j).BYTE_LEN

                        '「有効バイト数」の格納位置（PARA3）とレングス（PARA4）が定義されている場合は、
                        '「有効バイト数」の値を取得し、それに従ってdataLenを読み替える。
                        If (Not iniInfoAry(j).PARA3.Equals("")) AndAlso (Not iniInfoAry(j).PARA4.Equals("")) Then
                            'TODO: 定義ファイルの値が不正な場合は、起動直後にプロセス終了する方がよい。
                            Dim yukoByteNumOffset As Integer = Integer.Parse(iniInfoAry(j).PARA3)
                            Dim yukoByteNumLength As Integer = Integer.Parse(iniInfoAry(j).PARA4)
                            Dim yukoByteNum As UInteger
                            Select Case yukoByteNumLength
                                Case 4
                                    yukoByteNum = Utility.GetUInt32FromLeBytes4(bData, yukoByteNumOffset)
                                Case 3
                                    yukoByteNum = Utility.GetUInt32FromLeBytes3(bData, yukoByteNumOffset)
                                Case 2
                                    yukoByteNum = Utility.GetUInt16FromLeBytes2(bData, yukoByteNumOffset)
                                Case 1
                                    yukoByteNum = bData(yukoByteNumOffset)
                            End Select

                            'NOTE: 「yukoByteNum = dataLen」の場合はdataLenをそのまま使用する。
                            'なお、「yukoByteNum > dataLen」の場合もdataLenをそのまま使用し、
                            '不正なデータとはみなさないが、それは仕様である。
                            If yukoByteNum < dataLen Then
                                dataLen = CInt(yukoByteNum)
                            End If
                        End If

                        '領域からdataLen分のデータを取得する。
                        'NOTE: 例外が発生した場合は、レコード全体の異常とみなし、本メソッドを
                        '異常終了する（収集データ誤記テーブルに登録する）。
                        'NOTE: bDataがdataLenバイトの文字列や定義された長さの「有効バイト数」を
                        '格納している（十分な長さである）ことは、本メソッドの呼び元の責務である。
                        lineInfo(j) = OPMGUtility.getJisStringFromBytes(bData, iniInfoAry(j).BYTE_OFFSET, dataLen)
                End Select
            Next

            '窓処 在来線区駅順⇒幹線線区駅順 変換対応 ADD START
            '線区駅順コード置換え対応（線区：119、駅順：003 ⇒ 線区：070、駅順：100）
            If CHK_Count = 4 Then
                For j = 0 To iniInfoAry.Length - 1
                    Select Case UCase(iniInfoAry(j).FIELD_NAME)
                        Case "RAIL_SECTION_CODE"  'サイバネ線区コード
                            lineInfo(j) = "070"
                            Continue For
                        Case "STATION_ORDER_CODE"  'サイバネ駅順コード
                            lineInfo(j) = "100"
                            Continue For
                    End Select
                Next
            End If
            '窓処 在来線区駅順⇒幹線線区駅順 変換対応 ADD END

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' 日付チェック
    ''' </summary>
    ''' <param name="strDate">YYYYMMDDHHMMSS</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks></remarks>
    Public Shared Function CheckDate(ByVal strDate As String) As Boolean

        '処理日時フォーマートチェック
        Dim sDate As String = strDate.Substring(0, 4) & "/" & _
                        strDate.ToString.Substring(4, 2) & "/" & _
                            strDate.Substring(6, 2) & " " & _
                            strDate.Substring(8, 2) & ":" & _
                            strDate.Substring(10, 2) & ":" & _
                            strDate.Substring(12, 2)
        If Date.TryParse(sDate, New Date) = False Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' DATファイルの共通チェック:1レコードのチェック
    ''' </summary>
    ''' <param name="rowIndex">行目</param>
    ''' <param name="iniInfoAry">iniファイル情報</param>
    ''' <param name="lineInfo">レコードデータ</param>
    ''' <param name="datFileName">ファイル名</param>
    ''' <param name="isCheckMachine">機器構成マスタチェック： True：チェック　False：チェックしない</param>
    ''' <param name="isMachineCollect">機器構成マスタチェック、存在しない場合、True：収集データを登録 False：収集データを登録しない</param>
    ''' <param name="isMachineLog">機器構成マスタチェック、存在しない場合、True：ログを出力 False：収集データを登録しない</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Public Shared Function CheckDataComm(ByVal rowIndex As Integer, _
                                         ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                         ByVal lineInfo() As String, _
                                         ByVal datFileName As String, _
                                         Optional ByVal isCheckMachine As Boolean = True, _
                                         Optional ByVal isMachineCollect As Boolean = True, _
                                         Optional ByVal isMachineLog As Boolean = False) As Boolean

        Dim iFlag As Integer = 4
        Dim dataKind As String = "" 'データ種別

        Try

            For i As Integer = 0 To iniInfoAry.Length - 1
                If UCase(iniInfoAry(i).FIELD_NAME) = "DATA_KIND" Then
                    dataKind = lineInfo(i)
                    Continue For
                End If

                '駅コード、コーナーコード、号機番号が全部チェックではない場合
                If iFlag > 0 Then
                    Select Case UCase(iniInfoAry(i).FIELD_NAME)  '駅コード、コーナーコード、号機番号
                        Case "RAIL_SECTION_CODE", "STATION_ORDER_CODE", "CORNER_CODE", "UNIT_NO"
                            iFlag = iFlag - 1
                            'Nullチェック用
                            If (iniInfoAry(i).PARA2 = False) Then
                                If lineInfo(i) Is Nothing OrElse _
                                   lineInfo(i) = "" OrElse _
                                   lineInfo(i).Replace("0", "").Length <= 0 Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                                    SetCollectionData(datFileName, dataKind) 'ファイル名解析
                                    Return False
                                End If
                            End If

                            Continue For
                    End Select
                End If

                'キー 且 NULL不可
                Select Case UCase(iniInfoAry(i).FIELD_FORMAT)
                    Case "INTEGER"
                        '不正場合
                        If lineInfo(i) IsNot Nothing AndAlso _
                           (Not lineInfo(i) = "") AndAlso _
                           OPMGUtility.checkNumber(lineInfo(i)) = False Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                            '収集データの登録
                            SetCollectionData(iniInfoAry, lineInfo)
                            Return (False)
                        Else '空場合
                            'NULL不可
                            If (iniInfoAry(i).PARA2 = False) Then
                                If lineInfo(i) Is Nothing OrElse _
                                   lineInfo(i) = "" OrElse _
                                   lineInfo(i).Replace("0", "").Length <= 0 Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                                    '収集データの登録
                                    SetCollectionData(iniInfoAry, lineInfo)
                                    Return (False)
                                End If
                            End If
                        End If
                    Case "DATESTR"
                        '処理日時フォーマートチェック
                        Dim lnDate As Long = 0

                        If lineInfo(i) IsNot Nothing AndAlso _
                           (Not lineInfo(i) = "") AndAlso _
                           OPMGUtility.checkNumber(lineInfo(i)) = False Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                            '収集データの登録
                            SetCollectionData(datFileName, dataKind)
                            Return False
                        Else '全部０場合
                            'NULL不可
                            If (iniInfoAry(i).PARA2 = False) Then
                                If lineInfo(i) Is Nothing OrElse _
                                   lineInfo(i) = "" OrElse _
                                   lineInfo(i).Replace("0", "").Length <= 0 Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                                    '収集データの登録
                                    SetCollectionData(datFileName, dataKind)
                                    Return False
                                End If
                            End If
                            If lineInfo(i).Length = 14 Then
                                If CheckDate(lineInfo(i)) = False Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                                    '収集データの登録
                                    SetCollectionData(datFileName, dataKind)
                                    Return False
                                End If
                            Else
                                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                                '収集データの登録
                                SetCollectionData(datFileName, dataKind)
                                Return False
                            End If

                        End If
                End Select

            Next

            '機器構成マスタチェック
            If isCheckMachine Then
                Dim sBuilder As New StringBuilder
                Dim sRail_Code As String = ""
                Dim sStation_Code As String = ""
                Dim sCorner_Code As String = ""
                Dim sUnit_No As String = ""
                Dim sModel_Code As String = ""

                '機器構成マスタチェック用SQL文
                sBuilder.AppendLine("SELECT COUNT(1) FROM V_MACHINE_NOW WHERE 0=0 ")

                iFlag = 5
                For i As Integer = 0 To iniInfoAry.Length - 1
                    '駅コード、コーナーコード
                    Select Case UCase(iniInfoAry(i).FIELD_NAME)
                        Case "RAIL_SECTION_CODE"
                            iFlag = iFlag - 1
                            '機器構成マスタチェック用
                            If UCase(iniInfoAry(i).FIELD_FORMAT) = "INTEGER" Then
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & lineInfo(i))
                            Else
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & Utility.SetSglQuot(lineInfo(i)))
                            End If
                            sRail_Code = lineInfo(i)

                        Case "STATION_ORDER_CODE"
                            iFlag = iFlag - 1
                            '機器構成マスタチェック用
                            If UCase(iniInfoAry(i).FIELD_FORMAT) = "INTEGER" Then
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & lineInfo(i))
                            Else
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & Utility.SetSglQuot(lineInfo(i)))
                            End If
                            sStation_Code = lineInfo(i)

                        Case "CORNER_CODE"
                            iFlag = iFlag - 1
                            '機器構成マスタチェック用
                            If UCase(iniInfoAry(i).FIELD_FORMAT) = "INTEGER" Then
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & lineInfo(i))
                            Else
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & Utility.SetSglQuot(lineInfo(i)))
                            End If
                            sCorner_Code = lineInfo(i)

                        Case "UNIT_NO"
                            iFlag = iFlag - 1
                            '機器構成マスタチェック用
                            If UCase(iniInfoAry(i).FIELD_FORMAT) = "INTEGER" Then
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & lineInfo(i))
                            Else
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & Utility.SetSglQuot(lineInfo(i)))
                            End If
                            sUnit_No = lineInfo(i)

                        Case "MODEL_CODE"
                            iFlag = iFlag - 1
                            '機器構成マスタチェック用
                            If UCase(iniInfoAry(i).FIELD_FORMAT) = "INTEGER" Then
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & lineInfo(i))
                            Else
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & Utility.SetSglQuot(lineInfo(i)))
                            End If
                            sUnit_No = lineInfo(i)

                    End Select
                    If iFlag = 0 Then
                        Exit For
                    End If
                Next

                If iFlag = 0 Then
                    '機器構成マスタチェック
                    Dim dbCtl As DatabaseTalker = Nothing
                    Dim nRtn As Integer
                    Try
                        dbCtl = New DatabaseTalker
                        dbCtl.ConnectOpen()
                        nRtn = CInt(dbCtl.ExecuteSQLToReadScalar(sBuilder.ToString))
                        '  監視盤設定情報の固有処理
                        '  監視盤のIPアドレスから対象の改札機を抽出し、コーナコードを取得
                        If (nRtn = 0) And (dataKind = "54") Then
                            Dim j As Integer
                            Dim code As EkCode
                            'ファイル名を解析する
                            code = UpboundDataPath.GetEkCode(datFileName)
                            Dim sSQL As String = _
                                    "SELECT CORNER_CODE FROM V_MACHINE_NOW" _
                                    & "  WHERE RAIL_SECTION_CODE = '" & code.RailSection.ToString("D3") & "'" _
                                    & "    AND STATION_ORDER_CODE = '" & code.StationOrder.ToString("D3") & "'" _
                                    & "    AND MONITOR_ADDRESS = (" _
                                    & "    SELECT ADDRESS FROM V_MACHINE_NOW" _
                                    & "      WHERE RAIL_SECTION_CODE = '" & code.RailSection.ToString("D3") & "'" _
                                    & "        AND STATION_ORDER_CODE = '" & code.StationOrder.ToString("D3") & "'" _
                                    & "        AND CORNER_CODE = '" & code.Corner.ToString & "'" _
                                    & "        AND MODEL_CODE = 'W'" _
                                    & "        AND UNIT_NO = '" & code.Unit.ToString & "')" _
                                    & "    AND UNIT_NO = '" & sUnit_No & "'" _
                                    & "    AND MODEL_CODE = 'G'"
                            Dim oCorner As Object = dbCtl.ExecuteSQLToReadScalar(sSQL)
                            If oCorner IsNot Nothing Then
                                For j = 0 To iniInfoAry.Length - 1
                                    If iniInfoAry(j).FIELD_NAME = "CORNER_CODE" Then
                                        lineInfo(j) = Format(CInt(oCorner), "0000")
                                    End If
                                Next
                                nRtn = 1
                            End If
                        End If
                    Finally
                        'DBを閉じる
                        If dbCtl IsNot Nothing AndAlso dbCtl.IsConnect = True Then
                            dbCtl.ConnectClose()
                        End If
                        If dbCtl IsNot Nothing Then dbCtl = Nothing
                    End Try

                    If nRtn = 0 Then
                        '存在しない場合、ログを出力する
                        If isMachineLog Then
                            Log.Error(String.Format(RecAppConstants.ERR_MACHINE_NOVALUE, sRail_Code, sStation_Code, sCorner_Code, sUnit_No))
                        End If
                        '存在しない場合、収集データ誤記を登録する
                        'TODO: sRail_CodeかsStation_Codeの不正で「機器が存在しません」になる場合は、
                        '収集データのかわりにこれを登録したところで、結局、運管端末で探しても
                        '何もみえないのでは？
                        '処理日時がユーザの想定から外れている場合も、何もヒットしないわけで
                        'あるし、「機器コードが機器構成に無い場合や、処理日時と現在日時の間に
                        '設定した日数を超える乖離がある場合に、CdtReadingPartiallyFailedの
                        'サーバ内異常を登録する」等、抜本的な改善が必要では？
                        If isMachineCollect Then
                            SetCollectionData(iniInfoAry, lineInfo, _
                                              Lexis.CdtTheUnitNotFound.Gen(sRail_Code, sStation_Code, sCorner_Code, sUnit_No), _
                                              True)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            '収集データの登録
            SetCollectionData(iniInfoAry, lineInfo)
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' DATファイルのデータの登録
    ''' </summary>
    ''' <param name="iniInfoAry">INIファイル内容</param>
    ''' <param name="dlineInfoLst">データ</param>
    ''' <param name="dbName">テーブル名</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>チェック処理による加工したデータにて登録処理を行う</remarks>
    Public Shared Function PutDataToDBCommon(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                             ByVal dlineInfoLst As List(Of String()), _
                                             ByVal dbName As String) As Boolean
        Dim bRtn As Boolean = False
        Dim sCountBuilder As New StringBuilder
        Dim sAddBuilder As New StringBuilder
        Dim sbAddItem As New StringBuilder
        Dim sbAddValue As New StringBuilder
        Dim sUpdateBuilder As New StringBuilder
        Dim sbSqlWhere As New StringBuilder
        Dim sLoginID As String = "batch"
        Dim sClient As String = "OPMGServer"
        Dim dbCtl As DatabaseTalker = Nothing
        Dim i As Integer
        Dim j As Integer


        Try
            If dlineInfoLst Is Nothing OrElse dlineInfoLst.Count = 0 Then
                Return True
            End If

            dbCtl = New DatabaseTalker
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
        Catch ex As Exception
            'DBを閉じる
            If dbCtl IsNot Nothing AndAlso dbCtl.IsConnect = True Then
                dbCtl.ConnectClose()
            End If
            If dbCtl IsNot Nothing Then dbCtl = Nothing
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Try

            For i = 0 To dlineInfoLst.Count - 1

                Dim lineInfo(iniInfoAry.Length) As String
                lineInfo = dlineInfoLst.Item(i)

                sCountBuilder = New StringBuilder
                sAddBuilder = New StringBuilder
                sbAddItem = New StringBuilder
                sbAddValue = New StringBuilder
                sUpdateBuilder = New StringBuilder
                sbSqlWhere = New StringBuilder
                sCountBuilder.AppendLine("SELECT COUNT(1) FROM " & dbName)

                sbAddItem.AppendLine("INSERT INTO " & dbName & "(INSERT_DATE,INSERT_USER_ID,INSERT_MACHINE_ID")
                sbAddItem.AppendLine(",UPDATE_DATE,UPDATE_USER_ID,UPDATE_MACHINE_ID")
                sbAddValue.AppendLine("VALUES(getdate(),")
                sbAddValue.AppendLine(Utility.SetSglQuot(sLoginID) & ",")
                sbAddValue.AppendLine(Utility.SetSglQuot(sClient) & ",")
                sbAddValue.AppendLine("getdate(),")
                sbAddValue.AppendLine(Utility.SetSglQuot(sLoginID) & ",")
                sbAddValue.AppendLine(Utility.SetSglQuot(sClient))

                sUpdateBuilder.AppendLine("UPDATE " & dbName & " SET ")
                sUpdateBuilder.AppendLine("UPDATE_DATE=getdate(),")
                sUpdateBuilder.AppendLine("UPDATE_USER_ID=" & Utility.SetSglQuot(sLoginID) & ",")
                sUpdateBuilder.AppendLine("UPDATE_MACHINE_ID=" & Utility.SetSglQuot(sClient))

                sbSqlWhere.AppendLine(" WHERE 0 = 0 ")

                For j = 0 To iniInfoAry.Length - 1

                    'DB中で　有効なフィールドではない
                    If UCase(iniInfoAry(j).PARA5) = "FALSE" Then
                        Continue For
                    End If

                    sbAddItem.AppendLine("," & iniInfoAry(j).FIELD_NAME)

                    If UCase(iniInfoAry(j).FIELD_FORMAT).Equals("INTEGER") Then
                        sbAddValue.AppendLine("," & lineInfo(j).ToString)
                    Else
                        sbAddValue.AppendLine("," & Utility.SetSglQuot(lineInfo(j).ToString))
                    End If

                    If iniInfoAry(j).PARA1 Then
                        If UCase(iniInfoAry(j).FIELD_FORMAT).Equals("INTEGER") Then
                            sbSqlWhere.AppendLine(" AND " & iniInfoAry(j).FIELD_NAME & "=" & lineInfo(j).ToString)
                        Else
                            sbSqlWhere.AppendLine(" AND " & iniInfoAry(j).FIELD_NAME & "=" & Utility.SetSglQuot(lineInfo(j).ToString))
                        End If
                    Else
                        If UCase(iniInfoAry(j).FIELD_FORMAT).Equals("INTEGER") Then
                            sUpdateBuilder.AppendLine("," & iniInfoAry(j).FIELD_NAME & "=" & lineInfo(j).ToString)
                        Else
                            sUpdateBuilder.AppendLine("," & iniInfoAry(j).FIELD_NAME & "=" & Utility.SetSglQuot(lineInfo(j).ToString))
                        End If
                    End If
                Next

                sCountBuilder.Append(sbSqlWhere)

                sbAddItem.AppendLine(")")
                sbAddValue.AppendLine(")")
                sAddBuilder.Append(sbAddItem)
                sAddBuilder.Append(sbAddValue)

                sUpdateBuilder.Append(sbSqlWhere)

                Dim nRtn As Integer = CInt(dbCtl.ExecuteSQLToReadScalar(sCountBuilder.ToString))
                If nRtn = 0 Then
                    dbCtl.ExecuteSQLToWrite(sAddBuilder.ToString)
                Else
                    dbCtl.ExecuteSQLToWrite(sUpdateBuilder.ToString)
                End If
            Next
            dbCtl.TransactionCommit()
            bRtn = True
        Catch ex As Exception
            Try
                dbCtl.TransactionRollBack()
            Catch ex1 As Exception
            End Try
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            'DBを閉じる
            If dbCtl IsNot Nothing AndAlso dbCtl.IsConnect = True Then
                dbCtl.ConnectClose()
            End If
            If dbCtl IsNot Nothing Then dbCtl = Nothing

            If bRtn = False Then
                '収集データの登録
                SetCollectionData(iniInfoAry, dlineInfoLst.Item(i))
            End If
            dbCtl = Nothing
        End Try

        Return bRtn
    End Function

    ''' <summary>
    ''' バイト内容
    ''' </summary>
    ''' <param name="byteData">バイト内容</param>
    ''' <param name="bitOff">ビットオフセット</param>
    ''' <param name="bitLen">ビットレングス</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>ビットオフセット、ビットレングスによる値を取得</remarks>
    Public Shared Function GetBitValueFromByte(ByVal byteType As String, _
                                               ByVal byteData As Byte, _
                                               ByVal bitOff As Integer, _
                                               ByVal bitLen As Integer) As String
        Dim strValue As String = ""
        Dim bytData As Byte = byteData
        '左移
        If bitOff > 0 Then
            bytData = bytData << bitOff
        End If
        '右移
        bytData = bytData >> (8 - bitLen)

        Select Case UCase(byteType)
            Case "HEX", "BCD"
                strValue = fnHexDisp(bytData)
                If bitLen <= 4 Then
                    strValue = strValue.Substring(1, 1)
                End If
            Case "BIN"
                strValue = bytData.ToString
        End Select

        Return strValue

    End Function

    ''' <summary>
    ''' バイト内容
    ''' </summary>
    ''' <param name="iniInfoAry">iniファイル情報</param>
    ''' <param name="lineInfo">レコードデータ</param>
    ''' <param name="errInfo">異常内容</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>ビットオフセット、ビットレングスによる値を取得</remarks>
    Public Shared Function SetCollectionData(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                         ByVal lineInfo() As String, _
                                         Optional ByVal errInfo As String = "", _
                                         Optional ByVal isMachine As Boolean = False) As Boolean


        Dim iFlag As Integer = 7
        Dim baseInfo As RecDataStructure.BaseInfo = Nothing
        Dim sDataKindText As String = ""
        Try

            For i As Integer = 0 To iniInfoAry.Length - 1

                Select Case UCase(iniInfoAry(i).FIELD_NAME)
                    Case "DATA_KIND"
                        iFlag = iFlag - 1
                        baseInfo.DATA_KIND = lineInfo(i)
                    Case "RAIL_SECTION_CODE" '駅コード
                        iFlag = iFlag - 1
                        baseInfo.STATION_CODE.RAIL_SECTION_CODE = lineInfo(i)
                    Case "STATION_ORDER_CODE"
                        iFlag = iFlag - 1
                        baseInfo.STATION_CODE.STATION_ORDER_CODE = lineInfo(i)
                    Case "CORNER_CODE" 'コーナーコード
                        iFlag = iFlag - 1
                        baseInfo.CORNER_CODE = lineInfo(i)
                    Case "UNIT_NO" '号機番号
                        iFlag = iFlag - 1
                        baseInfo.UNIT_NO = Integer.Parse(lineInfo(i))
                    Case "PROCESSING_TIME", "OCCUR_DATE", "SYUSYU_DATE"
                        iFlag = iFlag - 1
                        If lineInfo(i).Length > 14 Then
                            baseInfo.PROCESSING_TIME = GetDateTimeString(lineInfo(i))
                        Else
                            baseInfo.PROCESSING_TIME = lineInfo(i)
                        End If
                    Case "MODEL_CODE"
                        iFlag = iFlag - 1
                        baseInfo.MODEL_CODE = lineInfo(i)
                End Select

                If iFlag = 0 Then Exit For
            Next
            If errInfo = "" Then
                errInfo = Lexis.CdtRecordingFailed.Gen()
            End If
            If isMachine Then
                sDataKindText = DbConstants.CdtKindServerError
            Else
                sDataKindText = GetDataKindText(baseInfo.DATA_KIND)
            End If
            CollectedDataTypoRecorder.Record(baseInfo, sDataKindText, errInfo)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' バイト内容
    ''' </summary>
    ''' <param name="baseInfo">ヘッド情報</param>
    ''' <param name="errInfo">異常内容</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>ビットオフセット、ビットレングスによる値を取得</remarks>
    Public Shared Function SetCollectionData(ByVal baseInfo As RecDataStructure.BaseInfo, _
                                             ByVal dataKind As String, _
                                             Optional ByVal errInfo As String = "", _
                                             Optional ByVal isMachine As Boolean = False) As Boolean

        Dim sDataKindText As String = ""
        Try
            If errInfo = "" Then
                errInfo = Lexis.CdtRecordingFailed.Gen()
            End If

            baseInfo.DATA_KIND = dataKind

            If isMachine Then
                sDataKindText = DbConstants.CdtKindServerError
            Else
                sDataKindText = GetDataKindText(baseInfo.DATA_KIND)
            End If

            CollectedDataTypoRecorder.Record(baseInfo, sDataKindText, errInfo)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' バイト内容
    ''' </summary>
    ''' <param name="filePath">datファイル名称</param>
    ''' <param name="errInfo">異常内容</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>ビットオフセット、ビットレングスによる値を取得</remarks>
    Public Shared Function SetCollectionData(ByVal filePath As String, _
                                             ByVal dataKind As String, _
                                             Optional ByVal errInfo As String = "", _
                                             Optional ByVal isMachine As Boolean = False) As Boolean
        'ヘッド部
        Dim headInfo As RecDataStructure.BaseInfo = Nothing
        Dim sDataKindText As String = ""
        Try
            Dim clientKind As String
            Dim codeInfo As EkCode = UpboundDataPath.GetEkCode(filePath)
            headInfo.DATA_KIND = dataKind
            headInfo.STATION_CODE.RAIL_SECTION_CODE = codeInfo.RailSection.ToString("D3")
            headInfo.STATION_CODE.STATION_ORDER_CODE = codeInfo.StationOrder.ToString("D3")
            headInfo.CORNER_CODE = codeInfo.Corner.ToString("D4")
            headInfo.UNIT_NO = codeInfo.Unit
            clientKind = codeInfo.Model.ToString("D2")
            headInfo.MODEL_CODE = GetModelCode(clientKind, dataKind, codeInfo.Unit)
            headInfo.PROCESSING_TIME = UpboundDataPath.GetTimestampString(filePath)

            If errInfo = "" Then
                errInfo = Lexis.CdtRecordingFailed.Gen()
            End If

            If isMachine Then
                sDataKindText = DbConstants.CdtKindServerError
            Else
                sDataKindText = GetDataKindText(headInfo.DATA_KIND)
            End If

            CollectedDataTypoRecorder.Record(headInfo, sDataKindText, errInfo)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try
        Return True

    End Function

    ''' <summary>
    ''' ポート番号取得
    ''' </summary>
    ''' <param name="clientKind">クライアントID</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    Public Shared Function GetPortNumber(ByVal clientKind As String) As Integer
        Select Case clientKind
            Case "02"
                Return RecServerAppBaseConfig.InputIpPortFromKanshiban
            Case "06"
                Return RecServerAppBaseConfig.InputIpPortFromTokatsu
            Case "08"
                Return RecServerAppBaseConfig.InputIpPortFromMadosho
            Case Else
                Return 0
        End Select
    End Function

    ''' <summary>
    ''' 機種取得
    ''' </summary>
    ''' <param name="clientKind">クライアントID</param>
    ''' <param name="dataKind">データ種別</param>
    ''' <param name="unitNo">号機番号</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>ビットオフセット、ビットレングスによる値を取得</remarks>
    Public Shared Function GetModelCode(ByVal clientKind As String, _
                                        ByVal dataKind As String, _
                                        Optional ByVal unitNo As Integer = 0) As String
        Dim modelCode As String = ""

        Select Case clientKind
            Case "02"
                Select Case dataKind
                    Case "A1", "A2", "A3", "A4", "A5", "A7", "A8", "55", "B1"
                        'G：（改札機）
                        modelCode = EkConstants.ModelCodeGate
                    Case "54"
                        'W：（監視盤）
                        modelCode = EkConstants.ModelCodeKanshiban
                    Case "A6", "C3"
                        If unitNo = 0 Then
                            'G：（改札機）
                            modelCode = EkConstants.ModelCodeGate
                        Else
                            'W：（監視盤）
                            modelCode = EkConstants.ModelCodeKanshiban
                        End If
                End Select

            Case "06"
                If dataKind = "89" Then
                    'Y：（窓口処理機）
                    modelCode = EkConstants.ModelCodeMadosho
                Else
                    'X：明収／EX統括
                    modelCode = EkConstants.ModelCodeTokatsu
                End If
            Case "08"
                '０８＝Y：（窓口処理機）
                modelCode = EkConstants.ModelCodeMadosho
        End Select

        Return modelCode
    End Function

    ''' <summary>
    ''' データ種別によって、名称を取得する
    ''' </summary>
    ''' <param name="sDataKind">データ種別</param>
    ''' <returns>データ種別名称</returns>
    Public Shared Function GetDataKindText(ByVal sDataKind As String) As String
        Dim sDataKindText As String = ""

        Select Case sDataKind
            Case "A1"
                sDataKindText = DbConstants.CdtKindBesshuData

            Case "A2"
                sDataKindText = DbConstants.CdtKindFuseiJoshaData

            Case "A3"
                sDataKindText = DbConstants.CdtKindKyokoToppaData

            Case "A4", "A5"
                sDataKindText = DbConstants.CdtKindFunshitsuData

            Case "A6", "C3"
                sDataKindText = DbConstants.CdtKindFaultData

            Case "A7", "A8"
                sDataKindText = DbConstants.CdtKindKadoData

            Case "B1"
                sDataKindText = DbConstants.CdtKindTrafficData

            Case "54"
                sDataKindText = DbConstants.CdtKindKsbConfig

            Case "55"
                sDataKindText = DbConstants.CdtKindConStatus

            Case "89"
                sDataKindText = DbConstants.CdtKindConStatus

            Case Else
                sDataKindText = sDataKind

        End Select

        Return sDataKindText
    End Function

    ''' <summary>
    ''' yyyy/MM/dd HH:mm:ss->yyyyMMddHHmmss
    ''' </summary>
    ''' <param name="sDataTime">日時</param>
    ''' <returns>yyyyMMddHHmmss</returns>
    Public Shared Function GetDateTimeString(ByVal sDataTime As String) As String
        Dim sRtnDateTime As String
        Dim sDate() As String = sDataTime.Split(CChar(" "))

        'TODO: Nowから得た値を返すのは、このメソッドの仕様なのか？

        If sDate.Length >= 2 Then
            sRtnDateTime = sDate(0).Replace("/", "")

            Dim sTime() As String = sDate(1).Split(CChar(":"))
            If sTime.Length >= 3 Then
                For n As Integer = 0 To 2
                    sRtnDateTime += Format(CInt(sTime(n)), "00")
                Next
            Else
                sRtnDateTime = Now.ToString("yyyyMMddHHmmss")
            End If
        Else
            sRtnDateTime = Now.ToString("yyyyMMddHHmmss")
        End If

        Return sRtnDateTime
    End Function
#End Region

End Class
