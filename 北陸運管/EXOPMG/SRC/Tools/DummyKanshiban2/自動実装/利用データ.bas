Public Sub 自動実装()
    Const cnsFilter = "VB.NET実装ファイル (*.vb),*.vb"
    Dim xlAPP As Application        ' Applicationオブジェクト
    Dim FSO As Object
    Dim title As String
    Dim outFileName As String
    Dim outFile As Integer
    Dim vntFileName As Variant      ' ファイル名受取り用
    Dim row As Long
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

    row = 1
    Do
        metaName = Cells(row, 1).Value
        If metaName = "" Then Exit Do
        xlAPP.StatusBar = "出力中です．．．．(" & row & "レコード目)"

        metaType = Cells(row, 7).Value

        elemCount = Cells(row, 5).Value
        If elemCount = "" Then
            elemCount = "1"
        End If

        dataSizeA = CInt(Cells(row, 2).Value)
        dataSizeB = CInt(Cells(row, 3).Value) \ CInt(elemCount)
        If dataSizeB * CInt(elemCount) <> CInt(Cells(row, 3).Value) Then
            dataSize = "算出できません"
        Else
            dataSize = CStr(dataSizeA) & "*" & CStr(dataSizeB)
        End If

        elemFormat = Cells(row, 4).Value
        If elemFormat = "" Then
            elemFormat = "X" & CStr((dataSizeA * dataSizeB + 3) \ 4)
        End If

        sep = Cells(row, 6).Value
        If sep = "" Then
            sep = " "
        End If

        outArgs = dataSize & ", """ & elemFormat & """, " & elemCount & ", """ & sep & """c, """ & metaName & """"
        If metaType <> "" Then
            outArgs = outArgs & ", """ & metaType & """"
        End If
        Print #outFile, Spc(8); "New XlsField(" & outArgs & "), _"

        row = row + 1
    Loop

    Close #outFile
    xlAPP.StatusBar = False
    Set FSO = Nothing

    MsgBox "実装が完了しました。", _
           vbOKOnly, "報告"
End Sub
