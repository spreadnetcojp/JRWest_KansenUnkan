Public Sub 自動実装()
    Const cnsFilter = "VB.NET実装ファイル (*.vb),*.vb"
    Dim xlAPP As Application        ' Applicationオブジェクト
    Dim FSO As Object
    Dim title As String
    Dim outFileName As String
    Dim outFile As Integer
    Dim vntFileName As Variant      ' ファイル名受取り用
    Dim row As Long
    Dim itemNumber As Integer
    Dim metaNames(500) As String
    Dim metaName As String
    Dim dataSizeA As Integer
    Dim dataSizeB As Integer
    Dim dataSize As String
    Dim elemFormat As String
    Dim elemCount As String
    Dim sep As String
    Dim metaType As String
    Dim outArgs As String

    Set xlAPP = Application
    Set FSO = CreateObject("Scripting.FileSystemObject")

    title = "出力するファイル名を指定して下さい"
    xlAPP.StatusBar = title
    vntFileName = xlAPP.GetSaveAsFilename(InitialFileName:="result.vb", _
                                          FileFilter:=cnsFilter, _
                                          title:=title)
    If VarType(vntFileName) = vbBoolean Then Exit Sub
    outFileName = vntFileName

    outFile = FreeFile
    Open outFileName For Output As #outFile

    For row = 4 To 65535
        If Cells(row, 2).Value <> "" Then
            itemNumber = Cells(row, 2).Value
            If itemNumber = "999" Then Exit For

            metaName = Cells(row, 3).Value
            If metaName <> "" Then
                metaName = "集計" & Format(itemNumber, "000") & " " & metaName
            Else
                metaName = "集計" & Format(itemNumber, "000") & " （不明）"
            End If
            metaNames(itemNumber) = metaName
        End If
    Next

    For itemNumber = 1 To 500
        xlAPP.StatusBar = "出力中です．．．．(" & itemNumber & "レコード目)"

        metaName = metaNames(itemNumber)
        If StrPtr(metaName) = 0 Then
            metaName = "集計" & Format(itemNumber, "000") & " （空き）"
        End If

        metaType = ""
        elemCount = "1"

        dataSizeA = CInt("8")
        dataSizeB = CInt("4") \ CInt(elemCount)
        If dataSizeB * CInt(elemCount) <> CInt("4") Then
            dataSize = "算出できません"
        Else
            dataSize = CStr(dataSizeA) & "*" & CStr(dataSizeB)
        End If

        elemFormat = "D"
        If elemFormat = "" Then
            elemFormat = "X" & CStr((dataSizeA * dataSizeB + 3) \ 4)
        End If

        sep = " "
        If sep = "" Then
            sep = " "
        End If

        outArgs = dataSize & ", """ & elemFormat & """, " & elemCount & ", """ & sep & """c, """ & metaName & """"
        If metaType <> "" Then
            outArgs = outArgs & ", """ & metaType & """"
        End If
        Print #outFile, Spc(8); "New XlsField(" & outArgs & ", Nothing, XlsByteOrder.LittleEndian), _"
    Next

    Close #outFile
    xlAPP.StatusBar = False
    Set FSO = Nothing

    MsgBox "実装が完了しました。", _
           vbOKOnly, "報告"
End Sub
