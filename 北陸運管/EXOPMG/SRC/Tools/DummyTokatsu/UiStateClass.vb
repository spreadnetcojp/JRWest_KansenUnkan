' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/08/08  (NES)小林  新規作成
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

    '端末の状態
    <DataMember> Public TermMachines As Dictionary(Of String, TermMachine)

    '接続状態
    <DataMember> Public NegaStatus As Byte
    <DataMember> Public MeisaiStatus As Byte
    <DataMember> Public OnlineStatus As Byte

    'NOTE: 統括は、マスタやプログラムを「窓処別に」所定世代分保持するはずである
    '（それにより「全端末の保持するものが一致するまで新たなものを受け入れない」と
    'いった監視盤的な制限をなくしている。また、それにより、配下にある窓処のエリアが
    '１種類でない状況に対応している）。よって、ここのHoldingFooBarは廃止して、
    '窓処別の（TermMachineクラスの）PendingFooを使いまわす方が合理的とも思える。
    '統括も、窓処がオフラインの場合に窓処への配信を失敗とする（統括自身が「異常」の
    'DL完了通知を生成する）のではなく、監視盤のように保留する思想に変更されたため、
    'シミュレータとして十分な状況を表示する上で、どのみちTermMachineクラスの
    'PendingFooは必須だからである。
    'しかしながら、ある１つの部材について、最初に窓処AへのDLL要求がデータ本体添付
    'で行われ、次に窓処BへのDLL要求が適用リストのみ添付で行われた場合、先に添付
    'されていたデータ本体を使用して窓処BへのDLLを行うべきであり、そのデータ本体に
    '関する情報は、窓処Aの行ではなく、統括の行（全窓処共通の行）に表示される方がよい
    'ため、ここのHoldingFooBarに保持させることにする。TermMachineクラスにおいて、
    '実際に窓処が保持しているデータを表すHoldingFooとは別に、統括側が保持している
    'データ（*1）を表すメンバを用意し、表示の際に、全TermMachineのそれを統合して
    '統括の行に表示することも不可能ではないが、同じデータを複数の箇所に重複して管理
    'することになるため、いただけない。１つの統括配下の全窓処についての所定世代分の
    'データを過不足なくここに保持させるのは少し面倒であるが、窓処への配信を試行する
    'たびに、TermMachines内に保持されているデータの名前（エリアやバージョン）をもとに、
    'ここに保持しているデータを整理する（不要なものを削除する）だけであり、
    '何とかなるはずである。
    '*1 たとえ同一名（同一エリア、同一バージョン）のデータであっても、窓処が保持している
    'ものと統括が保持しているもののデータ内容は、別物と考える必要がある。
    '過去のDLL要求で添付されていたものと同一名のデータ本体が新たなDLL要求で再度添付されて
    'きた場合、統括は自身の保持しているものを上書きする（日付が新しい別のデータとみなす）
    'が、適用リスト上で適用対象になっている窓処でない限り、過去の同一名データを保持して
    'いる窓処に対して、新しいものを勝手に配信しなおすことはない（これについては、配信指示と
    '配信の本来の関係が守られている）と思われるためである（※適用リスト上で適用対象になって
    'いる窓処については、このようにデータ本体が添付されているDLL要求の場合は、強制的に配信
    'しなおすことになり、データ本体が添付されていないDLL要求の場合は、データ本体を配信
    'しなおさずに「適用済」とするはずである）。
    '一方、初期化した窓処を再接続した場合に、運管サーバからの新たな配信指示なしで、
    '自身が保持している部材をもとに窓処への配信を行うのか否かについては、不明である。

    'NOTE: 統括に保持させている窓処プログラムが差分DLL部材のみであると意味がないので、
    '最後に受信した（というか最新n世代の）全体DLL部材と、（それぞれの世代について）
    'それ以降に受信した差分DLL部材全てを保持するべきかもしれない。
    'あるいは、もっと高機能に、統括の内部で部材のマージを行う可能性もないとは
    '言い切れない（さすがに違うと思うが）。
    'そもそも、監視盤同様、差分DLLには対応していなかったかもしれない（差分という
    '考え方は、わずかなバージョンの違いを世代の違いとみなす現状の世代管理と
    '絶対に相容れないはずである）。
    '⇒特定エリアのみに対応した部材（当該エリアに無関係なファイルは省略するが、
    '当該エリアに必要なファイルは前回配信バージョンに関係なく全て格納している部材）
    'を「差分DLL」で配信するようである。

    <DataMember> Public HoldingMasters As Dictionary(Of String, List(Of HoldingMaster))
    <DataMember> Public HoldingPrograms As List(Of HoldingProgram)

    Public Sub New()
        Me.TermMachines = New Dictionary(Of String, TermMachine)
        Me.NegaStatus = &H2
        Me.MeisaiStatus = &H2
        Me.OnlineStatus = &H2
        Me.HoldingMasters = New Dictionary(Of String, List(Of HoldingMaster))
        Me.HoldingPrograms = New List(Of HoldingProgram)
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
    <DataMember> Public DlsStatus As Byte
    <DataMember> Public KsbStatus As Byte
    <DataMember> Public Tk1Status As Byte
    <DataMember> Public Tk2Status As Byte
    <DataMember> Public HoldingMasters As Dictionary(Of String, HoldingMaster)
    <DataMember> Public PendingMasters As Dictionary(Of String, LinkedList(Of PendingMaster))
    <DataMember> Public HoldingPrograms As HoldingProgram()
    <DataMember> Public PendingPrograms As LinkedList(Of PendingProgram)

    Public Sub New()
        Me.DlsStatus = &H2
        Me.KsbStatus = &H2
        Me.Tk1Status = &H2
        Me.Tk2Status = &H2
        Me.HoldingMasters = New Dictionary(Of String, HoldingMaster)
        Me.PendingMasters = New Dictionary(Of String, LinkedList(Of PendingMaster))
        Me.HoldingPrograms = New HoldingProgram(1) {}
        Me.PendingPrograms = New LinkedList(Of PendingProgram)
    End Sub
End Class

<DataContract> Public Class HoldingMaster
    'NOTE: このクラスにおけるListVersionにも存在意義はある。
    '窓処は適用リストを保持しないが、どの適用リストの
    '指示によって当該窓処にマスタ本体の配信が行われたかが分かるように
    'するためである。よって、あくまで表示専用であり、制御には用いない。
    'ListContentやListHashValueについても同様である。

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
    'これを用いる。この方法は本物の統括と違うかもしれない。
    'NOTE: そもそも、マスタの内容が同一であることをチェックすること自体、
    '厳密すぎるようにみえるが、それをチェックしないと、このアプリの
    '実装が逆に複雑化してしまうので、このアプリ自身のためである。
    'また、このアプリを実運用のリハーサル用に使う場合は、本物の
    '統括でも許容されない可能性のある（あるいは、本質的に危険な）
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
    'これを用いる。この方法は本物の統括と違うかもしれない。
    'NOTE: そもそも、CABの内容が同一であることをチェックすること自体、
    '厳密すぎるようにみえるが、それをチェックしないと、このアプリの
    '実装が逆に複雑化してしまうので、このアプリ自身のためである。
    'また、このアプリを実運用のリハーサル用に使う場合は、本物の
    '統括でも許容されない可能性のある（あるいは、本質的に危険な）
    '運用に対し、それが分かるようにしておくにこしたことはない。
    <DataMember> Public DataHashValue As String

    'NOTE: このアプリでは、適用リストの内容が同一であるか否かを比較する上で、
    'これを用いる。この方法は本物の統括と違うかもしれない。
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
