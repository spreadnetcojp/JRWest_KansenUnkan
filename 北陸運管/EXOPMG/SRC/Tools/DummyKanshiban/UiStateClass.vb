' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/02/16  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Runtime.Serialization

<DataContract> Public Class UiStateClass
    'NOTE: 機器の状態はここに保存してもよいし、シミュレータ本体が指定してくる
    'パスの機器別ディレクトリに保存してもよい。運用が複雑になるので、
    'どちらかに統一した方がよい。ここに保存しておく方が高速に参照できる。
    <DataMember> Public Machines As Dictionary(Of String, Machine)

    'ログ表示フィルタの履歴
    <DataMember> Public LogDispFilterHistory As List(Of String)

    Public Sub New()
        Me.Machines = New Dictionary(Of String, Machine)
        Me.LogDispFilterHistory = New List(Of String)
    End Sub
End Class

<DataContract> Public Class Machine
    '機器構成ファイルの最終確認日時
    <DataMember> Public LastConfirmed As DateTime

    '機器構成ファイルのタイムスタンプ
    <DataMember> Public ProfileTimestamp As DateTime
    <DataMember> Public TermMachinesProfileTimestamp As DateTime

    '機器構成ファイルのキャッシュ
    <DataMember> Public Profile As Object()

    '各種状態
    <DataMember> Public TermMachines As Dictionary(Of String, TermMachine)

    'NOTE: HoldingMastersのValueは、要素数2の配列である。
    'バージョン（パターン番号）がどちらの要素とも違うものを受信した際は、
    '受け入れ可能な側（空いている側、改札機に適用されているものと違う側）
    'の要素を調べ、そちらに受け入れを行う。ただし、要素1に何か保持して
    'いながら、要素0が受け入れ可能な場合は、要素1にあるものを要素0に移動
    'し、要素1に受け入れを行う。つまり、要素0には要素1より古い世代の情報を
    '格納するように努力する。両方空いている場合は、いきなり要素0に格納する。
    'この順序は、そのまま、ContinueCodeがFinishWithoutStoringの
    'DLL終了REQ電文における「監視盤保持バージョン」の順序になる。
    '運管が制御に使わないのでどうでもよいが、この順序は、本物の
    '監視盤とは違うかもしれない。
    'NOTE: HoldingMastersがNothingになることはあり得ないが、任意の
    'マスタ種別XXXについて、HoldingMasters("XXX")が登録されているとは限らない。
    'HoldingMasters("XXX")が登録されている場合でも、
    'HoldingMasters("XXX")(0)やHoldingMasters("XXX")(1)がNothingということも
    'あり得る。
    <DataMember> Public HoldingMasters As Dictionary(Of String, HoldingMaster())

    'NOTE: HoldingProgramsも、要素数2の配列である。
    '受信したものは、基本的に、この配列の要素1に格納する。
    'その際に、要素1に格納されていたものを要素0に移動することはない。
    '要素1に格納されているプログラムを、配下の全改札機に適用した
    '時点で、それを要素0に移動し、要素1を空（全メンバ0）にする。
    'ただし、受信したものが配下の全改札機に適用済みである（この
    '配列の要素0に格納されている）場合は、要素0に上書きする。
    'この動作は本物の監視盤と違うかもしれないが、全改札機同一
    'バージョンが基本であることや、改札機が保持するプログラムを
    '監視盤も保持するという前提を掲げると、AcceptGatePro()における
    'プログラム受け入れの条件と合わせて、このようにするしかない
    'と思われる。この方式では、要素0には要素1より古い世代の情報
    'が格納されるが、この順序は、そのまま、ContinueCodeが
    'FinishWithoutStoringのDLL終了REQ電文の「監視盤保持バージョン」の
    '順序になる。運管が制御に使わないのでどうでもよいが、この順序は、
    '本物の監視盤と違うかもしれない。
    'NOTE: HoldingProgramsがNothingになることはあり得ないが、
    'HoldingPrograms(0)やHoldingPrograms(1)がNothingというのはあり得る。
    <DataMember> Public HoldingPrograms As HoldingProgram()

    'NOTE: HoldingKsbProgramsも、要素数2の配列である。この配列では、
    '適用中のものを要素0に格納し、適用待ちのものを要素1に格納する。
    'NOTE: HoldingKsbProgramsやHoldingKsbPrograms(0)がNothingになることは
    'あり得ないが、HoldingKsbPrograms(1)がNothingというのはあり得る。
    <DataMember> Public HoldingKsbPrograms As HoldingKsbProgram()
    <DataMember> Public PendingKsbPrograms As LinkedList(Of PendingKsbProgram)
    <DataMember> Public LatchConf As Byte
    <DataMember> Public FaultSeqNumber As UInteger
    <DataMember> Public FaultDate As DateTime

    Public Sub New()
        Me.TermMachines = New Dictionary(Of String, TermMachine)
        Me.HoldingMasters = New Dictionary(Of String, HoldingMaster())
        Me.HoldingPrograms = New HoldingProgram(1) {}
        Me.HoldingKsbPrograms = New HoldingKsbProgram(1) {}
        Me.PendingKsbPrograms = New LinkedList(Of PendingKsbProgram)
    End Sub
End Class

<DataContract> Public Class TermMachine
    '機器構成ファイルのキャッシュ
    <DataMember> Public Profile As Object()

    '各種状態
    'NOTE: HoldingProgramsは、要素数2の配列である。この配列では、
    '適用中のものを要素0に格納し、適用待ちのものを要素1に格納する。
    'NOTE: HoldingProgramsやHoldingPrograms(0)がNothingになることは
    'あり得ないが、HoldingPrograms(1)がNothingというのはあり得る。
    <DataMember> Public PwrStatusFromKsb As Byte
    <DataMember> Public McpStatusFromKsb As Byte
    <DataMember> Public IcmStatusFromMcp As Byte
    <DataMember> Public DlsStatusFromMcp As Byte
    <DataMember> Public DlsStatusFromIcm As Byte
    <DataMember> Public ExsStatusFromIcm As Byte
    <DataMember> Public HoldingMasters As Dictionary(Of String, HoldingMaster)
    <DataMember> Public PendingMasters As Dictionary(Of String, LinkedList(Of PendingMaster))
    <DataMember> Public HoldingPrograms As HoldingProgram()
    <DataMember> Public PendingPrograms As LinkedList(Of PendingProgram)
    <DataMember> Public LatchConf As Byte
    <DataMember> Public FaultSeqNumber As UInteger
    <DataMember> Public FaultDate As DateTime
    <DataMember> Public KadoSlot(1) As Integer
    <DataMember> Public KadoSeqNumber(1) As UInteger
    <DataMember> Public KadoDate(1) As DateTime

    Public Sub New()
        Me.PwrStatusFromKsb = &H1
        Me.McpStatusFromKsb = &H0
        Me.IcmStatusFromMcp = &H0
        Me.DlsStatusFromMcp = &H0
        Me.DlsStatusFromIcm = &H0
        Me.ExsStatusFromIcm = &H0
        Me.HoldingMasters = New Dictionary(Of String, HoldingMaster)
        Me.PendingMasters = New Dictionary(Of String, LinkedList(Of PendingMaster))
        Me.HoldingPrograms = New HoldingProgram(1) {}
        Me.PendingPrograms = New LinkedList(Of PendingProgram)
    End Sub
End Class

<DataContract> Public Class HoldingMaster
    'NOTE: このインスタンスを所有するのがTermMachineの場合も、ListVersionには
    '値を格納する。改札機は適用リストを保持しないが、どの適用リストの
    '指示によって当該改札機にマスタ本体の配信が行われたかが分かるように
    'するためである。よって、あくまで表示専用であり、制御には用いない。
    'ListContentやListHashValueについても同様である。

    'NOTE: このインスタンスを所有するのがMachineの場合も、ListVersionは制御に
    '使用しない（監視盤が最後に受け入れた適用リストのバージョンを
    'マスタバージョン別に画面表示するためだけに使用する）。
    'そのかわりに、配信先端末ごとに、配信処理未実施の適用リストを
    '任意の件数キューイング可能にしている。
    '理由については、HoldingProgramクラスのコメントを参照。

    <DataMember> Public DataSubKind As Integer
    <DataMember> Public DataVersion As Integer
    <DataMember> Public ListVersion As Integer
    <DataMember> Public DataAcceptDate As DateTime
    <DataMember> Public ListAcceptDate As DateTime
    <DataMember> Public DataDeliverDate As DateTime

    'NOTE: これはDataGridView2の行をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public DataFooter As Byte()

    'NOTE: これはDataGridView2の行の適用リストバージョン列をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public ListContent As String

    'NOTE: このアプリでは、マスタの内容が同一であるか否かを比較する上で、
    'これを用いる。この方法は本物の監視盤と違うかもしれない。
    'NOTE: そもそも、マスタの内容が同一であることをチェックすること自体、
    '厳密すぎるようにみえるが、それをチェックしないと、このアプリの
    '実装が逆に複雑化してしまうので、このアプリ自身のためである。
    'また、このアプリを実運用のリハーサル用に使う場合は、本物の
    '監視盤でも許容されない可能性のある（あるいは、本質的に危険な）
    '運用に対し、それが分かるようにしておくにこしたことはない。
    <DataMember> Public DataHashValue As String

    'NOTE: 本質的に保持しておく必然性がない情報であるが、
    '適用リスト閲覧ウィンドウのキーの一部として使用する
    'ことにしている（プログラム本体の閲覧ウィンドウと
    '一貫した実装にするため）。
    <DataMember> Public ListHashValue As String
End Class

<DataContract> Public Class PendingMaster
    'NOTE: マスタの場合、適用リストのDL完了が無いので、ListVersionは不要であるが、
    'どの適用リストによる配信が保留になっているのか、UIに表示される方が
    '使いやすいと思われるため、保存することにしている。
    <DataMember> Public DataSubKind As Integer
    <DataMember> Public DataVersion As Integer
    <DataMember> Public ListVersion As Integer
    <DataMember> Public DataAcceptDate As DateTime
    <DataMember> Public ListAcceptDate As DateTime

    'NOTE: これはDataGridView2の行をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public DataFooter As Byte()

    'NOTE: これはDataGridView2の行の適用リストバージョン列をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public ListContent As String

    'NOTE: 本質的に、ここにこれは不要であるが、単純化のために
    '格納しておく。
    <DataMember> Public DataHashValue As String
    <DataMember> Public ListHashValue As String
End Class

<DataContract> Public Class HoldingProgram
    'NOTE: プログラム配信では、DL完了通知にエリア番号を設定する必要がない。
    '加えて、監視機器は、適用リストのみを受信した場合も、それに紐づく
    'プログラム本体を探す上で、保持しているプログラム本体のエリア番号と
    '比較する必要がない（受信したもののエリア番号は、適用リストに記載
    'された自らの配下にある端末のエリア番号と一致するかをチェックする
    'ため、受け入れたもの同士のエリア番号が食い違うことがない）ように
    '思える。以上のことから、制御上は、ここにエリア番号を保存する必要
    'がないように思えるかもしれない。しかし、明収のような監視機器は、
    'エリア番号のみが異なるプログラムを（設置エリアの異なる複数の窓処の
    'ために）同時に保持する必要があるかもしれず、そうであるなら、
    'ここにエリア番号を保存することは必須である。そもそも、Profileの
    'エリア番号と比較するだけだと、それが変わった場合に、前後に受信した
    'プログラム本体と適用リストの関係を誤って解釈することになる。
    'また、検査に使う上で、受信したもののエリア番号をUI上で確認できる
    '方がよいはずである。以上のことから、ここに保存するようにしておく。

    'NOTE: このインスタンスを所有するのがTermMachineではなく、Machineである
    '場合は、ListVersionやApplicableDateやListHashValueは制御に使用
    'しない（監視盤が最後に受け入れた適用リストのバージョンを
    '代表バージョン別に画面表示するためだけに使用する）。そのかわりに、
    '配信先端末ごとに、配信処理未実施の適用リストを任意の件数
    'キューイング可能にしている。直観的には、監視機器がプログラム
    '本体と同じ件数の（つまり最大２つの）適用リストを保持するのが
    '自然にも思えるが、同じ代表バージョンでも、適用リストはいくつも
    '用意することが許されており、その１つ１つに意味があるため、
    'それでは、まともな機能を実現することが不可能になるはずである。仮に、
    '処理の済んでいない適用リストがある状況で、同一代表バージョンの
    'プログラムに関する次のDLL要求があった場合にBUSY等のNAKを返すに
    'しても、休止させている改札機が適用リストに記載されているだけで
    'リトライオーバーとなる（他の改札機にも配信が行えない）わけであり、
    'それが合理的とは考えにくい。また、（結果を待たずに配信可能な）
    '同一代表バージョンの適用リストの件数が99件に限定されるのも、
    '監視盤でのキューイングの容易さを考慮しての仕様と推測できる。
    'なお、本物の監視盤が以上のような仕様になっているかは不明であるが、
    '回線断となっている改札機が存在している状況でも、代表バージョン
    'が同じである限り（代表バージョンの変更で、プログラム本体の
    'バージョンが改札機間で不一致になるなどということがなければ）
    '新しい適用リストを受け付けることは間違いない。また、適用リスト
    'を受け付けた（DLLシーケンスを正常終了させた）以上、そこで
    '指定されている全ての改札機に配信を行う（諦める場合は、
    '監視盤自身が「配信異常」の改札機DL完了通知を生成する？）
    'ことも監視盤の責務として宣言されているため、キューイング
    '相当のことをすると考えるのが自然である。

    <DataMember> Public DataSubKind As Integer
    <DataMember> Public DataVersion As Integer
    <DataMember> Public ListVersion As Integer
    <DataMember> Public DataAcceptDate As DateTime
    <DataMember> Public ListAcceptDate As DateTime
    <DataMember> Public DataDeliverDate As DateTime
    <DataMember> Public ListDeliverDate As DateTime
    <DataMember> Public RunnableDate As String
    <DataMember> Public ApplicableDate As String
    <DataMember> Public ApplyDate As DateTime

    'NOTE: これはDataGridView2の行をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public ModuleInfos As ProgramModuleInfo()
    <DataMember> Public ArchiveCatalog As String
    <DataMember> Public VersionListData As Byte()

    'NOTE: これはDataGridView2の行の適用リストバージョン列をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public ListContent As String

    'NOTE: このアプリでは、CABの内容が同一であるか否かを比較する上で、
    'これを用いる。この方法は本物の監視盤と違うかもしれない。
    'NOTE: そもそも、CABの内容が同一であることをチェックすること自体、
    '厳密すぎるようにみえるが、それをチェックしないと、このアプリの
    '実装が逆に複雑化してしまうので、このアプリ自身のためである。
    'また、このアプリを実運用のリハーサル用に使う場合は、本物の
    '監視盤でも許容されない可能性のある（あるいは、本質的に危険な）
    '運用に対し、それが分かるようにしておくにこしたことはない。
    <DataMember> Public DataHashValue As String

    'NOTE: このアプリでは、適用リストの内容が同一であるか否かを比較する上で、
    'これを用いる。この方法は本物の監視盤と違うかもしれない。
    <DataMember> Public ListHashValue As String
End Class

<DataContract> Public Class PendingProgram
    <DataMember> Public DataSubKind As Integer
    <DataMember> Public DataVersion As Integer
    <DataMember> Public ListVersion As Integer
    <DataMember> Public DataAcceptDate As DateTime
    <DataMember> Public ListAcceptDate As DateTime
    <DataMember> Public RunnableDate As String
    <DataMember> Public ApplicableDate As String

    'NOTE: これはDataGridView2の行をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    'NOTE: 監視盤における運管からの改札機向けプログラム受け入れ条件による
    '縛りから、PendingProgramに格納されているものは、Machineの
    'HoldingProgramsの何れかの要素に必ず格納されているはずであり、
    '本質的に、ここにこれは不要であるが、単純化のために
    '格納しておく。
    <DataMember> Public ModuleInfos As ProgramModuleInfo()
    <DataMember> Public ArchiveCatalog As String
    <DataMember> Public VersionListData As Byte()

    'NOTE: これはDataGridView2の行の適用リストバージョン列をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public ListContent As String

    'NOTE: 本質的に、ここにこれは不要であるが、単純化のために
    '格納しておく。
    <DataMember> Public DataHashValue As String
    <DataMember> Public ListHashValue As String
End Class

<DataContract> Public Structure ProgramModuleInfo
    <DataMember> Public Elements As ProgramElementInfo()
End Structure

<DataContract> Public Structure ProgramElementInfo
    <DataMember> Public FileName As String
    <DataMember> Public DispData As Byte()
End Structure

<DataContract> Public Class HoldingKsbProgram
    <DataMember> Public DataSubKind As Integer
    <DataMember> Public DataVersion As Integer
    <DataMember> Public ListVersion As Integer
    <DataMember> Public DataAcceptDate As DateTime
    <DataMember> Public ListAcceptDate As DateTime
    <DataMember> Public DataDeliverDate As DateTime
    <DataMember> Public ListDeliverDate As DateTime
    <DataMember> Public RunnableDate As String
    <DataMember> Public ApplicableDate As String
    <DataMember> Public ApplyDate As DateTime

    'NOTE: これはDataGridView2の行をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public ArchiveCatalog As String
    <DataMember> Public VersionListData As Byte()

    'NOTE: これはDataGridView2の行の適用リストバージョン列をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public ListContent As String

    'NOTE: このアプリでは、CABの内容が同一であるか否かを比較する上で、
    'これを用いる。この方法は本物の監視盤と違うかもしれない。
    'NOTE: そもそも、CABの内容が同一であることをチェックすること自体、
    '厳密すぎるようにみえるが、それをチェックしないと、このアプリの
    '実装が逆に複雑化してしまうので、このアプリ自身のためである。
    'また、このアプリを実運用のリハーサル用に使う場合は、本物の
    '監視盤でも許容されない可能性のある（あるいは、本質的に危険な）
    '運用に対し、それが分かるようにしておくにこしたことはない。
    <DataMember> Public DataHashValue As String

    'NOTE: このアプリでは、適用リストの内容が同一であるか否かを比較する上で、
    'これを用いる。この方法は本物の監視盤と違うかもしれない。
    <DataMember> Public ListHashValue As String
End Class

<DataContract> Public Class PendingKsbProgram
    <DataMember> Public DataSubKind As Integer
    <DataMember> Public DataVersion As Integer
    <DataMember> Public ListVersion As Integer
    <DataMember> Public DataAcceptDate As DateTime
    <DataMember> Public ListAcceptDate As DateTime
    <DataMember> Public RunnableDate As String
    <DataMember> Public ApplicableDate As String

    'NOTE: これはDataGridView2の行をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public ArchiveCatalog As String
    <DataMember> Public VersionListData As Byte()

    'NOTE: これはDataGridView2の行の適用リストバージョン列をダブルクリックすると開く
    '独立したモードレスダイアログに表示する想定である。
    <DataMember> Public ListContent As String

    'NOTE: 本質的に、ここにこれは不要であるが、単純化のために
    '格納しておく。
    <DataMember> Public DataHashValue As String
    <DataMember> Public ListHashValue As String
End Class
