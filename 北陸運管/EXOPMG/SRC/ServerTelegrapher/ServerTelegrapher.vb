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

Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' サーバとして電文の送受信を行うクラス。
''' </summary>
Public Class ServerTelegrapher
    Inherits Looper

#Region "内部クラス等"
    '能動的電文交換に関する状態の定義
    Protected Enum State As Integer
        NoConnection
        Idling
        WaitingForReply
    End Enum

    '能動的単発シーケンスの定義
    Protected Class ActiveOne
        '当該シーケンスのREQ電文
        Public ReqTeleg As IReqTelegram

        '軽度NAK電文受信からREQ電文再送信までのインターバルを作るためのタイマ
        Public RetryTimer As TickTimer

        '現在の試行回数
        'NOTE: 一度でも試行したか否か（シーケンスを実行中か否か）を知るため
        'だけに存在している。
        Public CurTryCount As Integer

        'NakRequirement.ForgetOnRetryOverなNAK電文の最大受信回数
        'NOTE: REQ電文に対するNakRequirement.ForgetOnRetryOverなNAK電文の
        '受信がこの回数あった場合、RetryOverToForgetで終了する。
        Public LimitNakCountToForget As Integer

        'NakRequirement.CareOnRetryOverなNAK電文の受信回数
        Public CurNakCountToForget As Integer

        'NakRequirement.CareOnRetryOverなNAK電文の最大受信回数
        'NOTE: REQ電文に対するNakRequirement.CareOnRetryOverなNAK電文の
        '受信がこの回数継続した場合、RetryOverToCareで終了する。
        Public LimitNakCountToCare As Integer

        'NakRequirement.CareOnRetryOverなNAK電文の受信回数
        Public CurNakCountToCare As Integer

        'シーケンス名（ログ出力のみに使用）
        Public SeqName As String

        'コンストラクタ
        Public Sub New( _
           ByVal oReqTeleg As IReqTelegram, _
           ByVal retryIntervalTicks As Integer, _
           ByVal limitNakCountToForget As Integer, _
           ByVal limitNakCountToCare As Integer, _
           ByVal sSeqName As String)
            Me.ReqTeleg = oReqTeleg
            Me.RetryTimer = New TickTimer(retryIntervalTicks)
            Me.CurTryCount = 0
            Me.LimitNakCountToForget = limitNakCountToForget
            Me.CurNakCountToForget = 0
            Me.LimitNakCountToCare = limitNakCountToCare
            Me.CurNakCountToCare = 0
            Me.SeqName = sSeqName
        End Sub
    End Class

    'ファイル転送シーケンスの転送方向
    'NOTE: このクラス内では、ActiveとPassiveの違いは重要であるが、
    'それが同じなら、DllとUllが違っても、制御は殆ど同一である。
    'ただし、DllとUllとでは、転送元ファイルからのハッシュ値生成と
    '転送先ファイルからのハッシュ値生成（及び通知されたハッシュ値
    'との比較）に関して、サーバ側とクライアント側のどちらがどちら
    'を行うかが異なるので注意すること。シーケンス完了時に呼び出す
    'フックメソッドについても、目的が異なる（Dllでは配信状態の
    '認識を更新するためのメソッドであり、Ullでは受信ファイルの
    '保存を行うためのメソッドである）ため、呼び出すべきタイミング
    'が微妙に異なるので、注意しなければならない。
    Protected Enum XllDirection As Integer
        Dll
        Ull
    End Enum

    '能動的ファイル転送シーケンスの定義
    Protected Class ActiveXll
        '転送方向
        Public Direction As XllDirection

        '当該ファイル転送シーケンスの最新のREQ電文
        'NOTE: 作成時点から破棄時点まで転送開始REQ電文である。
        'なお、ULLにおいては、REQ電文のバイト列にHashValue部が存在しないが、転送開始のACK電文を
        '受信した時点で、そこに格納されたハッシュ値をこのオブジェクトの専用メンバに格納する。
        '理由は、転送終了のREQ電文を受信した際に、このオブジェクト内で、ファイルから算出される
        'ハッシュ値と比較するためである。ハッシュ値の生成をREQ電文のオブジェクト内で行うのは、
        'プロトコル仕様に依存したハッシュ値の書式を隠蔽しなければならず、ファイル名がACK電文
        'ではなくREQ電文に格納されている以上、規定路線であると言える。
        '同様の必然性は、ClientTelegrapherにおいても存在する。
        Public ReqTeleg As IXllReqTelegram

        '軽度NAK電文受信から転送開始REQ電文再送信までのインターバル
        Public RetryIntervalTicks As Integer

        '現在の転送開始試行回数
        'NOTE: 一度でも試行したか否か（シーケンスを実行中か否か）を知るため
        'だけに存在している。
        Public CurTryCount As Integer

        'NakRequirement.ForgetOnRetryOverなNAK電文の最大受信回数
        'NOTE: REQ電文に対するNakRequirement.ForgetOnRetryOverなNAK電文の
        '受信がこの回数あった場合、RetryOverToForgetで終了する。
        Public LimitNakCountToForget As Integer

        'NakRequirement.CareOnRetryOverなNAK電文の受信回数
        Public CurNakCountToForget As Integer

        'NakRequirement.CareOnRetryOverなNAK電文の最大受信回数
        'NOTE: REQ電文に対するNakRequirement.CareOnRetryOverなNAK電文の
        '受信がこの回数継続した場合、RetryOverToCareで終了する。
        Public LimitNakCountToCare As Integer

        'NakRequirement.CareOnRetryOverなNAK電文の受信回数
        Public CurNakCountToCare As Integer

        'コンストラクタ
        Public Sub New( _
           ByVal direction As XllDirection, _
           ByVal oXllReqTeleg As IXllReqTelegram, _
           ByVal retryIntervalTicks As Integer, _
           ByVal limitNakCountToForget As Integer, _
           ByVal limitNakCountToCare As Integer)
            Me.Direction = direction
            Me.ReqTeleg = oXllReqTeleg
            Me.RetryIntervalTicks = retryIntervalTicks
            Me.CurTryCount = 0
            Me.LimitNakCountToForget = limitNakCountToForget
            Me.CurNakCountToForget = 0
            Me.LimitNakCountToCare = limitNakCountToCare
            Me.CurNakCountToCare = 0
        End Sub
    End Class

    '受動的ファイル転送シーケンスの定義
    Protected Class PassiveXll
        '転送方向
        Public Direction As XllDirection

        '当該ファイル転送シーケンスの最新REQ電文
        'NOTE: 作成時点から転送終了のREQ電文である。
        'ただし、HashValue部については、上記のタイミングとは別に、DLLで
        'クライアントからハッシュ値を受信した際にも上書きする。
        Public ReqTeleg As IXllReqTelegram

        'コンストラクタ
        Public Sub New( _
           ByVal direction As XllDirection, _
           ByVal oXllReqTeleg As IXllReqTelegram)
            Me.Direction = direction
            Me.ReqTeleg = oXllReqTeleg
        End Sub
    End Class

    '能動的ファイル転送シーケンスに関する状態の定義
    Protected Enum ActiveXllState As Integer
        None       '実行前・実行中の能動的ファイル転送シーケンスなし
        BeforeFtp  '転送開始のACK電文受信前
        Ftp        '転送開始のACK電文受信後
    End Enum

    '受動的ファイル転送シーケンスに関する状態の定義
    Protected Enum PassiveXllState As Integer
        None       '実行前・実行中の受動的ファイル転送シーケンスなし（転送開始のACK電文送信前）
        Ftp        '転送開始のACK電文送信後
    End Enum

    'NAK電文の要件
    Protected Enum NakRequirement As Integer
        ForgetOnRetryOver       '問題とみなすべきでない
        CareOnRetryOver         '継続する場合は問題とみなすべき
        DisconnectImmediately   'コネクションを切断すべき
    End Enum
#End Region

#Region "定数や変数"
    'ソケットから電文を取り込むためのインタフェース
    'NOTE: 本クラスにとってのこれは、Telegramファクトリであるが、これ自身は、
    'Telegramの生成を一手に担うことを目的として存在しているわけではない。
    'TelegramImporterは「ソケットやファイルなどの外部媒体から」電文を取り込む
    'ことを専門とするクラスである。Telegramインスタンス生成のための実装は
    '各Telegramクラスで行うことになっており、TelegramImporterもそれを利用して
    'いる。実際に、本クラスにTelegramインスタンスを供給するのは
    'oTelegImporter.GetTelegramFromSocket()だけではない。本クラスの
    'サブクラスのProcOnHogeRequestReceive()などもNewで直接生成した各種Telegram
    'インスタンスを供給する。また、Telegramインスタンス自体にも、それ自身に
    '対する否定応答Telegramインスタンス等を生成する機能がある。
    Protected oTelegImporter As ITelegramImporter

    '電文送受信用ソケット
    'NOTE: クローズ実施時点でNothingに戻すことになっている。
    Protected oTelegSock As Socket

    'NOTE: このTelegrapherからのREQ電文送信で始まるシーケンスを能動的シーケンス
    'と呼ぶ。一度でもREQ電文を送信したら、リトライインターバル中も含めて１つの
    '能動的シーケンスを実行しているものとみなす。REQ電文の送信後、それに対する
    '応答電文を受信するまで、次のREQ電文は送信しないが、それが守られる範囲で、
    '複数の能動的シーケンスを並行して実行する。REQ電文の送信には下記の優先順位
    'を設ける（上位に記載したREQ電文を優先して送信する）。
    ' (1)ウォッチドッグシーケンスのREQ電文
    ' (2)ウォッチドッグシーケンス以外の能動的単発シーケンスのREQ電文
    ' (3)能動的ファイル転送シーケンスのREQ電文（転送開始のREQ電文）
    'これにより、シーケンス全体の実行順序には、以下のような規則ができる。
    ' (a)上位記載シーケンスの実行が控えている（開始していない）場合、
    '    下位記載シーケンスは開始しない。
    ' (b)上位記載シーケンスが実行中になれば、送信済みのREQ電文に対する
    '    応答電文受信待ちでない限りは（即ち、上位記載シーケンスがリトライ
    '    インターバル中ならば）、下位記載シーケンスも開始する。
    ' (c)下位記載シーケンスの実行状態がどうであろうと、送信済みのREQ電文に
    '    対する応答電文受信待ちでない限り、上位記載シーケンスは開始する。
    'なお、設定次第では、(2)記載のシーケンス同士や、(3)記載のシーケンス同士
    'は、同時に実行しない。即ち、先に開始したシーケンス全体が終了する（完了
    'またはリトライオーバーする）まで、同じ種類のシーケンスは開始しない。
    'よって、設定次第では、(b)記載の「上位記載シーケンス」とは「上位に記載
    'されている各シーケンスの中で最初に控えていた（開始した）もの」という
    '意味である。

    '能動的電文交換に関する状態
    'NOTE: REQ/ACKレベルの順序を維持するための（REQ電文送信～応答電文受信を
    '排他的に行うための）状態とする。
    'NOTE: curState相当の情報は、oTelegSockとoLastSentReqTelegからも取得できる
    'が、更新時のフックが有用になる可能性があるので、用意している。
    'NOTE: たとえisPendingFooBarRetry等によって先送りにしているREQ電文の再送信
    'であっても、優先順位が高いREQ電文の通常送信（先送りにしていない場合の送信
    'や再送信）より後回しにする。優先順位の高いREQ電文が常に控えている場合、
    'それが何かの要因でリトライインターバルに入らない限りは、優先度の低い
    'シーケンスは、永遠に開始しないだけでなく、既に開始していれば永遠に終了
    'しない（リトライオーバー等にならない）ことになるが、それは設計思想に合致
    'している。要送信のREQ電文が残り続ける（発生速度の方が送信速度よりも平均
    'して高い）ということは、そもそもあってはならないことであるし、滞留する
    '時間の猶予は、優先順位の高いREQ電文の方が短いわけである。サーバにおいて、
    'まず、ウォッチドッグシーケンスは、キューイングされない（優先順位が低い
    'シーケンスを開始する隙が必ず生じる）上、そのREQ電文を期限内に送信しな
    'ければ通信異常に直結するので、最優先である。また、能動的単発シーケンス
    'と能動的ファイル転送シーケンスでは、用途上、前者の方が即時性が高いため、
    '前者が優先である。
    Protected curState As State
    Protected isPendingWatchdog As Boolean
    Protected oActiveOneRetryPendingQueue As Queue(Of ActiveOne)
    Protected isPendingActiveXllRetry As Boolean

    '最後に送信したREQ電文
    'NOTE: 応答電文受信時点でNothingに戻すことになっている。
    Protected oLastSentReqTeleg As IReqTelegram

    'ウォッチドッグシーケンスのREQ電文
    Protected oWatchdogReqTeleg As IReqTelegram

    '能動的単発シーケンスのキュー
    'NOTE: このキューの要素は、ウォッチドッグシーケンス以外の能動的な単発
    'シーケンスに相当する。先頭の要素は、現在実行中であるか、さもなくば、
    '能動的電文交換の状態がIdlingに戻った際や、ペンディングされている
    'ウォッチドッグシーケンスが無くなった際に開始するはずのものである。
    '能動的単発シーケンス順序強制モードでは、先頭要素のシーケンスが終了
    '（完了またはリトライオーバー）しない限り、次以降の要素は実行しない。
    'NOTE: 能動的単発シーケンスのリトライタイムアウト発生時点を除けば、
    'たとえこのキューに要素が存在していても、全要素のCurTryCountが1以上
    '（全要素が初回送信済み）でありかつ、oActiveOneRetryPendingQueue.Countが0
    'である（先送りにされている再送信が無い）場合は、能動的単発シーケンスは
    '全てリトライインターバル中であるといえる。その場合は、能動的単発シーケンス
    'より優先度の低い能動的シーケンスであっても、実施可能である。
    Protected oActiveOneQueue As LinkedList(Of ActiveOne)

    '最後にREQ電文送信を実施した能動的単発シーケンス
    'NOTE: 応答電文受信時点でNothingに戻すことになっている。
    Protected oLastSentActiveOne As ActiveOne

    '能動的ファイル転送シーケンスのキュー
    'NOTE: このキューの要素は、能動的ファイル転送シーケンスに相当する。先頭の
    '要素は、現在実行中であるか、さもなくば、能動的電文交換の状態がIdlingに
    '戻った際や、ペンディングされているウォッチドッグシーケンスが無くなった際
    'や、全ての能動的単発シーケンスが無くなった際や、全ての能動的単発シーケンス
    'がリトライインターバル中になった際に開始するはずのものである。
    '現状の実装では、先頭要素のシーケンスが終了（完了またはリトライオーバー）
    'しない限り、次以降の要素は実行しない。
    Protected oActiveXllQueue As LinkedList(Of ActiveXll)

    '能動的ファイル転送シーケンスの状態
    'NOTE: 相手から受信したREQ電文のObjCodeが正当か否か判定するために必要で
    'ある。これがないと、oActiveXllQueueの先頭要素に転送開始のREQ電文が格納
    'されている状況において、それと同じObjCodeの転送終了のREQ電文を受信した
    '場合に、正当なシーケンスが実施されているか否かを容易には判定できない。
    '転送開始のACK電文受信まで済んでいるか否かを判定できればよいわけであるが、
    'かなり面倒な判定が必要になってしまうはずである。
    'NOTE: たとえ転送開始REQ電文の送信を一度も実施していなくても、
    'oActiveXllQueueの先頭にあるシーケンスの情報を（次の電文受信までに
    'oActiveXllQueueがクリアされることが確実でない限り必ず）セットしておく。
    'oActiveXllQueueが空であれば、ActiveXllState.Noneをセットしておく。
    Protected curActiveXllState As ActiveXllState

    '受動的ファイル転送シーケンスのキュー
    'NOTE: このキューの要素は、受動的ファイル転送シーケンスに相当する。
    '先頭の要素は、現在実行中のシーケンスである。
    '現状の設計では、このキューに１件でも要素が存在していれば、新たな
    '受動的ファイル転送シーケンスの転送開始REQ電文は受け付けない。
    'ある意味、キューである必要はないが、能動的ファイル転送との
    '一貫性確保や、複数の受動的ファイル転送シーケンスを並行実施する
    '可能性を考慮して、キューで管理する。
    Protected oPassiveXllQueue As LinkedList(Of PassiveXll)

    '受動的ファイル転送シーケンスの状態
    'NOTE: 殆ど無意味だが、能動的ファイル転送との一貫性確保のため存在する。
    'oPassiveXllQueueの先頭にあるシーケンスの情報を（次の電文受信までに
    'oPassiveXllQueueがクリアされることが確実でない限り必ず）セットしておく。
    'oPassiveXllQueueが空であれば、PassiveXllState.Noneをセットしておく。
    Protected curPassiveXllState As PassiveXllState

    '各種タイマ
    Protected oWatchdogTimer As TickTimer
    Protected oReplyLimitTimer As TickTimer
    Protected oActiveXllRetryTimer As TickTimer
    Protected oActiveXllLimitTimer As TickTimer  '動作させたくない場合は0や-1を設定。
    Protected oPassiveXllLimitTimer As TickTimer  '動作させたくない場合は0や-1を設定。

    '１電文読み書きの期限
    Protected telegReadingLimitBaseTicks As Integer  '0や-1は指定禁止。
    Protected telegReadingLimitExtraTicksPerMiB As Integer
    Protected telegWritingLimitBaseTicks As Integer  '0や-1は指定禁止。
    Protected telegWritingLimitExtraTicksPerMiB As Integer

    '１電文あたりのログ保存最大長
    Protected telegLoggingMaxLengthOnRead As Integer
    Protected telegLoggingMaxLengthOnWrite As Integer

    'ファイル転送シーケンス排他増強モード設定
    'NOTE: 能動的ファイル転送と受動的ファイル転送を並行して実施した場合に
    'クライアントが（同時に実施できないように制御しているどころか）誤動作する
    'ようなら、これをTrueとするべきである。
    'NOTE: これをTrueに設定している場合、能動的ファイル転送シーケンスの実施中
    'は、受動的ファイル転送シーケンスの転送開始REQ電文に対し、NAK（ビジー）を
    '返信する。逆に、能動的ファイル転送シーケンスの転送開始REQ電文を送信すべき
    '時点で受動的ファイル転送シーケンスが実行中であれば、能動的ファイル転送の
    '転送開始REQ電文は送信せず、無条件で試行回数を増進させる。なお、ここで言う
    '「能動的ファイル転送シーケンスの実施中」は、能動的ファイル転送シーケンスの
    '転送開始REQ電文に対する応答電文受信待ち（curActiveXllState = BeforeFtp
    'AndAlso oActiveXllQueue.First.Value.ReqTeleg = oLastSentReqTeleg）の場合と、
    '転送開始ACK電文の受信後（curActiveXllState = Ftp）に限定する。
    'これは、サーバとクライアントの両方に能動的ファイル転送シーケンスが控えて
    'いる場合のお見合い（キューイングされているものがある間、双方が必ずビジー
    'を返すことで、全てのファイル転送シーケンスが双方で必ずリトライオーバーと
    'なる事態のことであり、最悪の場合、リトライオーバーとなるまでの間に、
    'クライアント側に次の能動的ファイル転送シーケンスがキューイングされてゆく
    'と思われる）を回避するためである。
    Protected enableXllStrongExclusion As Boolean

    '能動的シーケンス排他増強モード設定
    'NOTE: 能動的ファイル転送シーケンスの実施中に能動的単発シーケンスのREQ電文
    'を送信するとクライアントが（ビジーを返すどころか）誤動作するようなら、
    'これをTrueとするべきである。
    'NOTE: これをTrueに設定している場合、能動的単発シーケンスを実施する際に
    '能動的ファイル転送シーケンスの転送を実行中である（電文交換において転送
    '開始が成立している）ならば、能動的単発シーケンスのREQ電文は送信せず、
    '無条件で試行回数を増進させる。
    Protected enableActiveSeqStrongExclusion As Boolean

    '能動的単発シーケンス順序強制モード設定
    Protected enableActiveOneOrdering As Boolean

    '所定時間よりも短い間隔でSystemTickを書き込む（0～0xFFFFFFFF）
    Private _LastPulseTick As Long
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket, ByVal oTelegImporter As ITelegramImporter)
        'NOTE: このメソッドは親スレッドで実行されることになる。そして、
        'ここで（親スレッドで）初期化した変数は、MyBase.Startメソッドを実行して
        '以降、子スレッドで参照されることになる。しかし、MyBase.Startメソッドが
        'メモリバリアとなるため、初期化は単純代入等で済まして問題ない。

        MyBase.New(sThreadName, oParentMessageSock)

        Me.oTelegImporter = oTelegImporter
        Me.oTelegSock = Nothing

        Me.curState = State.NoConnection
        Me.isPendingWatchdog = False
        Me.oActiveOneRetryPendingQueue = New Queue(Of ActiveOne)
        Me.isPendingActiveXllRetry = False
        Me.oLastSentReqTeleg = Nothing

        Me.oWatchdogReqTeleg = Nothing
        Me.oActiveOneQueue = New LinkedList(Of ActiveOne)
        Me.oLastSentActiveOne = Nothing
        Me.oActiveXllQueue = New LinkedList(Of ActiveXll)
        Me.curActiveXllState = ActiveXllState.None
        Me.oPassiveXllQueue = New LinkedList(Of PassiveXll)
        Me.curPassiveXllState = PassiveXllState.None

        'NOTE: 現状のoWatchdogTimerの設定時間は、相手装置に対する
        'ウォッチドッグREQ電文の送信周期と、親スレッドに公開する
        'LastPulseTickの更新周期を兼ねているので注意。
        Me.oWatchdogTimer = New TickTimer(60 * 1000)  'NOTE: MayOverride
        Me.oReplyLimitTimer = New TickTimer(0)
        Me.oActiveXllRetryTimer = New TickTimer(0)
        Me.oActiveXllLimitTimer = New TickTimer(0)
        Me.oPassiveXllLimitTimer = New TickTimer(0)

        'NOTE: MayOverride
        Me.telegReadingLimitBaseTicks = 10 * 1000
        Me.telegReadingLimitExtraTicksPerMiB = 0
        Me.telegWritingLimitBaseTicks = 5 * 1000
        Me.telegWritingLimitExtraTicksPerMiB = 0

        'NOTE: MayOverride
        Me.telegLoggingMaxLengthOnRead = 0
        Me.telegLoggingMaxLengthOnWrite = 0

        'NOTE: MayOverride
        Me.enableXllStrongExclusion = False
        Me.enableActiveSeqStrongExclusion = False
        Me.enableActiveOneOrdering = False

        Me.LastPulseTick = 0
    End Sub
#End Region

#Region "親スレッド用メソッド"
    Public Overrides Sub Start()
        Dim systemTick As Long = TickTimer.GetSystemTick()
        LastPulseTick = systemTick
        RegisterTimer(oWatchdogTimer, systemTick)

        MyBase.Start()
    End Sub
#End Region

#Region "プロパティ"
    'NOTE: 子スレッドを開始して以降の_LastPulseTickは、カーネルを介した排他制御
    'なしに、子スレッドで書き込み、親スレッドで読み出すことにしている。
    'なお、_LastPulseTickは、実際的には、x86-64プロセッサにおける通常の
    '転送命令１つで（即ち、少なくとも割込による分断は無しに）全体を読む（書く）
    'ことが可能なサイズであり、複数コアによるバスオペレーションレベルでも
    '読み書きが分割されることのない位置に配置されていると思われる。また、
    '書き込みを行うスレッドが１つであるため、書き込みの競合についてのケアも
    '不要である。しかしながら、ThreadクラスのVolatileReadやVolatileWriteは
    '使用しない方針とする。これらのメソッドは不可分な動作を意図している
    'わけではない（たとえば、VolatileWriteは、VolatileReadを使用する別の
    'スレッドからの可視性を保証していても、不可分に見える書き換えを保証している
    'わけではない）と思われるのに対し、これらの変数に格納する値は、一応全バイト
    'で意味を成すものであるためである。_LastPulseTickは、死活監視に使うため
    'の重要な変数であるから、パフォーマンス上のよほどの必要性がない限り
    '（LOCK信号によるバスの性能低下すら問題となるような状況にならない限り）
    'VolatileReadやVolatileWriteに変更してはならない。
    Public Property LastPulseTick() As Long
        Get
            Return Interlocked.Read(_LastPulseTick)
        End Get

        Protected Set(ByVal tick As Long)
            Interlocked.Exchange(_LastPulseTick, tick)
        End Set
    End Property
#End Region

#Region "イベント処理メソッド"
    Protected Overrides Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        If oTimer Is oWatchdogTimer Then
            Return ProcOnWatchdogTime()
        End If

        If oTimer Is oReplyLimitTimer Then
            Return ProcOnReplyLimitTime()
        End If

        For Each oOne As ActiveOne In oActiveOneQueue
            If oTimer Is oOne.RetryTimer Then
                Return ProcOnActiveOneRetryTime(oOne)
            End If
        Next oOne

        If oTimer Is oActiveXllRetryTimer Then
            Return ProcOnActiveXllRetryTime()
        End If

        If oTimer Is oActiveXllLimitTimer Then
            Return ProcOnActiveXllLimitTime()
        End If

        If oTimer Is oPassiveXllLimitTimer Then
            Return ProcOnPassiveXllLimitTime()
        End If

        Debug.Fail("This case is impermissible.")
        Return MyBase.ProcOnTimeout(oTimer)
    End Function

    Protected Overridable Function ProcOnWatchdogTime() As Boolean
        Dim systemTick As Long = TickTimer.GetSystemTick()
        LastPulseTick = systemTick
        RegisterTimer(oWatchdogTimer, systemTick)

        If curState = State.NoConnection Then
            Return True
        End If

        If curState = State.WaitingForReply Then
            isPendingWatchdog = True
            Return True
        End If

        Debug.Assert(curState = State.Idling)
        Debug.Assert(Not isPendingWatchdog)

        'NOTE: 状況に応じてウォッチドッグの有無や電文内容を変更する類の
        'プロトコルを想定し、勿体ないが毎回作成しなおすことにしている。
        oWatchdogReqTeleg = CreateWatchdogReqTelegram()
        If oWatchdogReqTeleg IsNot Nothing Then
            Log.Info("Sending Watchdog REQ...")
            If SendReqTelegram(oWatchdogReqTeleg) = False Then
                Disconnect()
                Return True
            End If

            TransitState(State.WaitingForReply)
            oLastSentReqTeleg = oWatchdogReqTeleg
            oReplyLimitTimer.Renew(oWatchdogReqTeleg.ReplyLimitTicks)
            RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnReplyLimitTime() As Boolean
        Log.Error("Reply limit time comes.")
        Disconnect()
        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneRetryTime(ByVal oOne As ActiveOne) As Boolean
        Log.Info("ActiveOne retry time comes.")

        'NOTE: 優先順位の一貫性を考慮すると、isPendingWatchdogがTrueの場合も
        'ActiveOneRetryは先送りにするべきであるが、その場合は下記の条件が必ず
        '成立するため、その判定は省略する。
        If curState = State.WaitingForReply Then
            oActiveOneRetryPendingQueue.Enqueue(oOne)
            Return True
        End If

        Debug.Assert(curState = State.Idling)
        Debug.Assert(Not oActiveOneRetryPendingQueue.Contains(oOne))

        If enableActiveSeqStrongExclusion Then
            'NOTE: 下記の場合だけでなく、curActiveXllStateがBeforeFtpの
            '場合も、能動的ファイル転送シーケンスの転送開始REQ電文が送信済み
            'という可能性はある（相手がACK電文を送信している可能性を考慮する
            'と、能動的単発シーケンスと排他的な状態とみなせる）。
            'しかし、そのような状態であるならば、上記curStateの判定でReturnして
            'いるはずである。なお、Returnした後、REQ/ACKレベルのペンディングが
            '解除される時（応答電文を受信した後）には、能動的単発シーケンスが
            '可能になっているかもしれない（NAK（ビジー）電文の受信によって、
            '能動的ファイル転送シーケンスがリトライインターバルに入っている
            '等が期待できる）。
            If curActiveXllState = ActiveXllState.Ftp Then
                Log.Info(oOne.SeqName & " is regulated by ActiveXll.")
                'NOTE: REQ電文を送信してNAK（ビジー）電文を受信した場合と
                '同等の処理を行う。
                oOne.CurTryCount += 1
                oOne.CurNakCountToCare += 1
                If oOne.CurNakCountToCare >= oOne.LimitNakCountToCare Then
                    Log.Warn(oOne.SeqName & " retry over.")
                    ProcOnActiveOneRetryOverToCare(oOne.ReqTeleg, Nothing)
                    oActiveOneQueue.Remove(oOne)
                    DoNextActiveSeq()
                Else
                    RegisterTimer(oOne.RetryTimer, TickTimer.GetSystemTick())
                    'NOTE: このケースでは、oActiveOneQueueの要素のCurTryCountに
                    '変化があるが、元々1以上だったのがインクリメントされるだけ
                    'であり、それによって何かが送信可能になるわけではないため、
                    'DoNextActiveSeq()は省略する。
                End If
                Return True
            End If
        End If

        Log.Info("Sending " & oOne.SeqName & " REQ...")
        oOne.CurTryCount += 1
        If SendReqTelegram(oOne.ReqTeleg) = False Then
            Disconnect()
            Return True
        End If

        TransitState(State.WaitingForReply)
        oLastSentReqTeleg = oOne.ReqTeleg
        oLastSentActiveOne = oOne
        oReplyLimitTimer.Renew(oOne.ReqTeleg.ReplyLimitTicks)
        RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
        Return True
    End Function

    Protected Overridable Function ProcOnActiveXllRetryTime() As Boolean
        Log.Info("ActiveXll retry time comes.")

        'NOTE: 優先順位の一貫性を考慮すると、isPendingWatchdogがTrueの場合や、
        'oActiveOneRetryPendingQueue.Countが0以外の場合や、開始前のActiveOne
        'シーケンスが控えている場合(*1)も、ActiveXllRetryは先送りにするべきで
        'あるが、それらの場合は下記の条件が必ず成立するため、それらの判定は
        '省略する。
        '*1 ActiveOneのREQ電文よりも優先順位の高いREQ電文送信がペンディング
        'している場合であり、結局のところisPendingWatchdogがTrueの場合である。
        If curState = State.WaitingForReply Then
            isPendingActiveXllRetry = True
            Return True
        End If

        Debug.Assert(curState = State.Idling)
        Debug.Assert(Not isPendingActiveXllRetry)

        Dim oXll As ActiveXll = oActiveXllQueue.First.Value

        'NOTE: ここはIdling状態でのみ実行されるため、
        'たとえ能動的シーケンス排他増強モードであっても、
        '能動的単発シーケンスの実行状態を気にする必要はない。
        If enableXllStrongExclusion Then
            If curPassiveXllState = PassiveXllState.Ftp Then
                'NOTE: リトライインターバルの間にクライアントからPassiveXllの
                '転送開始REQ電文を受信し、それを受け付けていた場合である。
                Log.Info("ActiveXll is regulated by PassiveXll.")
                'NOTE: REQ電文を送信してNAK（ビジー）電文を受信した場合と
                '同等の処理を行う。
                oXll.CurTryCount += 1
                oXll.CurNakCountToCare += 1
                If oXll.CurNakCountToCare >= oXll.LimitNakCountToCare Then
                    If oXll.Direction = XllDirection.Dll Then
                        Log.Warn("ActiveDll retry over.")
                        ProcOnActiveDllRetryOverToCare(oXll.ReqTeleg, Nothing)
                    Else
                        Log.Warn("ActiveUll retry over.")
                        ProcOnActiveUllRetryOverToCare(oXll.ReqTeleg, Nothing)
                    End If
                    oActiveXllQueue.RemoveFirst()
                    UpdateActiveXllStateAfterDequeue()
                    DoNextActiveSeq()
                Else
                    oActiveXllRetryTimer.Renew(oXll.RetryIntervalTicks)
                    RegisterTimer(oActiveXllRetryTimer, TickTimer.GetSystemTick())
                    'NOTE: このケースでは、oActiveXllQueueトップのCurTryCountに
                    '変化があるが、元々1以上だったのがインクリメントされるだけ
                    'であり、それによって何かが送信可能になるわけではないため、
                    'DoNextActiveSeq()は省略する。
                End If
                Return True
            End If
        End If

        If oXll.Direction = XllDirection.Dll Then
            Log.Info("Sending ActiveDllStart REQ...")
        Else
            Log.Info("Sending ActiveUllStart REQ...")
        End If
        oXll.CurTryCount += 1
        If SendReqTelegram(oXll.ReqTeleg) = False Then
            Disconnect()
            Return True
        End If

        TransitState(State.WaitingForReply)
        oLastSentReqTeleg = oXll.ReqTeleg
        oReplyLimitTimer.Renew(oXll.ReqTeleg.ReplyLimitTicks)
        RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
        Return True
    End Function

    Protected Overridable Function ProcOnActiveXllLimitTime() As Boolean
        Log.Error("ActiveXll limit time comes.")

        Dim oXll As ActiveXll = oActiveXllQueue.First.Value

        If oXll.Direction = XllDirection.Dll Then
            ProcOnActiveDllTimeout(oXll.ReqTeleg)
        Else
            ProcOnActiveUllTimeout(oXll.ReqTeleg)
        End If
        oActiveXllQueue.RemoveFirst()
        UpdateActiveXllStateAfterDequeue()

        Disconnect()
        Return True
    End Function

    Protected Overridable Function ProcOnPassiveXllLimitTime() As Boolean
        Log.Error("PassiveXll limit time comes.")

        Dim oXll As PassiveXll = oPassiveXllQueue.First.Value

        If oXll.Direction = XllDirection.Dll Then
            ProcOnPassiveDllTimeout(oXll.ReqTeleg)
        Else
            ProcOnPassiveUllTimeout(oXll.ReqTeleg)
        End If
        oPassiveXllQueue.RemoveFirst()
        UpdatePassiveXllStateAfterDequeue()

        Disconnect()
        Return True
    End Function

    Protected Overrides Function ProcOnSockReadable(ByVal oSock As Socket) As Boolean
        If oSock Is oParentMessageSock Then
            Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oSock)
            Return ProcOnParentMessageReceive(oRcvMsg)
        End If

        If oSock Is oTelegSock Then
            Dim oRcvTeleg As ITelegram _
               = oTelegImporter.GetTelegramFromSocket(oSock, telegReadingLimitBaseTicks, telegReadingLimitExtraTicksPerMiB, telegLoggingMaxLengthOnRead)
            If oRcvTeleg Is Nothing Then
                Disconnect()
                Return True
            End If
            Return ProcOnTelegramReceive(oRcvTeleg)
        End If

        Debug.Fail("This case is impermissible.")
        Return MyBase.ProcOnSockReadable(oSock)
    End Function

    'NOTE: MayOverride
    Protected Overridable Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Select Case oRcvMsg.Kind
            Case InternalMessageKind.QuitRequest
                Return ProcOnQuitRequestReceive(oRcvMsg)

            Case InternalMessageKind.ConnectNotice
                Return ProcOnConnectNoticeReceive(oRcvMsg)

            Case InternalMessageKind.DisconnectRequest
                Return ProcOnDisconnectRequestReceive(oRcvMsg)

            Case Else
                Debug.Fail("This case is impermissible.")
                Return True
        End Select
    End Function

    Protected Overridable Function ProcOnQuitRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("Quit requested by manager.")

        'NOTE: このスレッドが正常終了する際はここを必ず通り、
        '異常終了する際はProcOnUnhandledException()が必ず実行される
        'はずであるため、ファイナライザが実行される時点では、
        '必要なClose()やDispose()は既に実行している想定である。
        '現状、GCのパフォーマンスを考慮し、ファイナライザは実装して
        'いないが、心配であれば、ファイナライザを用意し、そこで
        'Debug.Assert(oTelegSock Is Nothing)
        'Debug.Assert(oParentMessageSock Is Nothing)
        'のようなチェックを実装するとよい。

        If curState <> State.NoConnection Then
            Disconnect()
        End If

        UnregisterSocket(oParentMessageSock)
        oParentMessageSock.Close()
        oParentMessageSock = Nothing

        Return False
    End Function

    Protected Overridable Function ProcOnConnectNoticeReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("New socket comes from manager.")

        If curState <> State.NoConnection Then
            Disconnect()
        End If

        Connect(ConnectNotice.Parse(oRcvMsg).GetSocket())

        '従来機は、新たなコネクションにおけるウォッチドッグREQ電文の
        '送信をコネクション確立の60秒後に行うようになっている。
        '本プログラムでもその仕様を継承する。
        Dim systemTick As Long = TickTimer.GetSystemTick()
        LastPulseTick = systemTick
        RegisterTimer(oWatchdogTimer, systemTick)
        Return True
    End Function

    Protected Overridable Function ProcOnDisconnectRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("Disconnect requested by manager.")

        If curState <> State.NoConnection Then
            Disconnect()
        End If
        Return True
    End Function

    Protected Overridable Function ProcOnTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim violation As NakCauseCode = oRcvTeleg.GetHeaderFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("Telegram with invalid HeadPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Select Case oRcvTeleg.CmdKind
            Case CmdKind.Req
                Return ProcOnReqTelegramReceive(oRcvTeleg)
            Case CmdKind.Ack
                Return ProcOnAckTelegramReceive(oRcvTeleg)
            Case CmdKind.Nak
                Return ProcOnNakTelegramReceive(oRcvTeleg)
            Case Else
                Log.Error("Telegram with invalid CmdKind received.")
                SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oRcvTeleg)
                Return True
        End Select
    End Function

    Protected Overridable Function ProcOnReqTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        If curActiveXllState = ActiveXllState.Ftp Then
            Dim oCurXll As ActiveXll = oActiveXllQueue.First.Value
            If oRcvTeleg.IsSameKindWith(oCurXll.ReqTeleg) Then
                Dim sSeqName As String = If(oCurXll.Direction = XllDirection.Dll, "ActiveDll", "ActiveUll")
                Dim oXllReqTeleg As IXllReqTelegram = oCurXll.ReqTeleg.ParseAsSameKind(oRcvTeleg)
                Dim violation As NakCauseCode = oXllReqTeleg.GetBodyFormatViolation()
                If violation <> NakCauseCode.None Then
                    Log.Error(sSeqName & " REQ with invalid BodyPart received.")
                    SendNakTelegramThenDisconnect(violation, oRcvTeleg)
                    Return True
                End If
                If oXllReqTeleg.IsContinuousWith(oCurXll.ReqTeleg) Then
                    Return ProcOnContinuousActiveXllReqTelegramReceive(oCurXll.Direction, oXllReqTeleg)
                End If
            End If
        End If

        If curPassiveXllState = PassiveXllState.Ftp Then
            Dim oCurXll As PassiveXll = oPassiveXllQueue.First.Value
            If oRcvTeleg.IsSameKindWith(oCurXll.ReqTeleg) Then
                Dim sSeqName As String = If(oCurXll.Direction = XllDirection.Dll, "PassiveDll", "PassiveUll")
                Dim oXllReqTeleg As IXllReqTelegram = oCurXll.ReqTeleg.ParseAsSameKind(oRcvTeleg)
                Dim violation As NakCauseCode = oXllReqTeleg.GetBodyFormatViolation()
                If violation <> NakCauseCode.None Then
                    Log.Error(sSeqName & " REQ with invalid BodyPart received.")
                    SendNakTelegramThenDisconnect(violation, oRcvTeleg)
                    Return True
                End If
                If oXllReqTeleg.IsContinuousWith(oCurXll.ReqTeleg) Then
                    Return ProcOnContinuousPassiveXllReqTelegramReceive(oCurXll.Direction, oXllReqTeleg)
                End If
            End If
        End If

        If IsPassiveDllReq(oRcvTeleg) Then
            Return ProcOnPassiveXllReqTelegramReceive(XllDirection.Dll, oRcvTeleg)
        End If

        If IsPassiveUllReq(oRcvTeleg) Then
            Return ProcOnPassiveXllReqTelegramReceive(XllDirection.Ull, oRcvTeleg)
        End If

        Return ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnContinuousActiveXllReqTelegramReceive(ByVal direction As XllDirection, ByVal oXllReqTeleg As IXllReqTelegram) As Boolean
        Dim sSeqName As String = If(direction = XllDirection.Dll, "ActiveDll", "ActiveUll")
        Select Case oXllReqTeleg.ContinueCode
            Case ContinueCode.Finish
                Log.Info(sSeqName & "Finish REQ received.")
                UnregisterTimer(oActiveXllLimitTimer)

                Dim nakCause As NakCauseCode = NakCauseCode.None
                If direction = XllDirection.Dll Then
                    'NOTE: 下記メソッドは、当該クライアントに対するファイル配信状態の
                    '認識を更新するためのメソッドである。クライアントは既にファイルを
                    '保存しているはずであるため、可能な限りこちらの認識がクライアント
                    'の状態と合致するように、この時点で呼び出すことにする。
                    'なお、本シーケンスを実施するクライアントにおいて、受信したファイル
                    'を保存するか否かをサーバからの応答に従って決めてはならないことは、
                    '絶対的なルールである。仮に、クライアントが、サーバからの応答が
                    'あるまで保存を行わないとなると、サーバは、このシーケンスに関する
                    '処理を終えた時点でも（ソケットへのACK電文の書き込みに成功したと
                    'しても）クライアントに対する配信状態の認識を更新することが
                    'できなくなってしまう。即ち、正常系であるにもかかわらず、配信状態
                    'を「不明」としなければならないため、その後、新たなコネクションで
                    '同一データの配信操作が行われたときの運用に、著しい弊害が生じる。
                    ProcOnActiveDllComplete(oXllReqTeleg)
                Else
                    oXllReqTeleg.ImportFileDependentValueFromSameKind(oActiveXllQueue.First.Value.ReqTeleg)
                    If Not oXllReqTeleg.IsHashValueIndicatingOkay Then
                        Log.Error("The hash values differ from one another.")
                        nakCause = ProcOnActiveUllHashValueError(oXllReqTeleg)
                    Else
                        'NOTE: 下記のメソッドでは、受信したファイルの保存を行うはずである。
                        '問題はそれを呼び出すタイミングであるが、たとえソケットへのACK電文
                        '書き込み成功後に呼び出したとしても、クライアントがこちらのファイル保存
                        'を認識している（認識する）か否かはわからないため、どのみちクライアント
                        'との認識が合わなくなる可能性は排除できない。
                        'そもそも、たとえソケットへのACK電文書き込みでエラーが返ってきたとしても、
                        'どの時点のエラーかは区別していないため、クライアントにはACK電文が
                        '届いてしまっている恐れがある。その際にこちらがファイルを保存して
                        'いないというのは最悪であり、そのような事態を避けるために
                        '保存できるものは先に保存しておく。
                        nakCause = ProcOnActiveUllComplete(oXllReqTeleg)
                    End If
                End If
                oActiveXllQueue.RemoveFirst()
                UpdateActiveXllStateAfterDequeue()

                If nakCause = NakCauseCode.None Then
                    Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                    Log.Info("Sending " & sSeqName & "Finish ACK...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                Else
                    Dim oReplyTeleg As INakTelegram = oXllReqTeleg.CreateNakTelegram(nakCause)
                    If oReplyTeleg Is Nothing Then
                        Disconnect()
                        Return True
                    End If

                    Log.Info("Sending " & sSeqName & "Finish NAK (" & nakCause.ToString() & ")...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                End If

                If curState = State.Idling Then
                    DoNextActiveSeq()
                End If
                Return True

            Case ContinueCode.FinishWithoutStoring
                Log.Info(sSeqName & "FinishWithoutStoring REQ received.")
                UnregisterTimer(oActiveXllLimitTimer)

                If direction = XllDirection.Dll Then
                    'NOTE: 下記メソッドは、当該クライアントに対するファイル配信状態の
                    '認識を更新するためのメソッドである。
                    ProcOnActiveDllCompleteWithoutStoring(oXllReqTeleg)
                Else
                    'NOTE: Ull用電文のクラスにおいて、ContinueCodeに
                    'ContinueCode.FinishWithoutStoringをマッピング
                    'するのは禁止である。
                    Debug.Fail("This case is impermissible.")
                    Abort()
                End If
                oActiveXllQueue.RemoveFirst()
                UpdateActiveXllStateAfterDequeue()

                Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                Log.Info("Sending " & sSeqName & "FinishWithoutStoring ACK...")
                If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                    Disconnect()
                    Return True
                End If

                If curState = State.Idling Then
                    DoNextActiveSeq()
                End If
                Return True

            Case ContinueCode.Abort
                'NOTE: このケースのように、ファイル転送失敗の場合は、
                '必ずコネクションを切断することになっている。
                'なお、ファイル転送失敗後の電文コネクションの存続の
                '決定を完全に相手に委ねてしまえば、幾分柔軟である。
                'が、相手装置は、電文コネクションを維持したい場合
                '（転送や転送したファイルの内容だけが異常の場合）、
                'ContinueCode.FinishWithoutStoringを送信することで、
                '望み通りに維持できる。すなわち、ContinueCode.Abort
                'の場合にこちらか切断する現状の仕様であっても、
                '相手装置次第でコネクションを維持する方法がある
                'ということに違いはない。

                Log.Error(sSeqName & "Abort REQ received.")
                UnregisterTimer(oActiveXllLimitTimer)

                If direction = XllDirection.Dll Then
                    ProcOnActiveDllAbort(oXllReqTeleg)
                Else
                    ProcOnActiveUllAbort(oXllReqTeleg)
                End If
                oActiveXllQueue.RemoveFirst()
                UpdateActiveXllStateAfterDequeue()

                'NOTE: 一部のプロトコル仕様書に曖昧な記述があり、このケースでは
                'NAK電文を返信するべきであるようにも読み取れる。
                'ただし、全体的にみて、REQ電文（転送失敗を示す）自体に異常が
                'なければACK電文を返すべきであるし、ACK電文を返すシーケンス図も
                '存在している。よって、上記の曖昧な記述は、REQ電文自体に異常が
                'あるケースについて（こんなケースもあるということを示すために）
                '記載されていると考えることにする。
                Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                Log.Info("Sending " & sSeqName & "Abort ACK...")
                SendReplyTelegram(oReplyTeleg, oXllReqTeleg)
                '上記呼び出しの戻り値は無視する（その後の処理に差異がないため）。
                Disconnect()
                Return True

            Case Else
                Log.Error(sSeqName & " REQ with invalid ContinueCode received.")
                SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oXllReqTeleg)
                Return True
        End Select
    End Function

    Protected Overridable Function ProcOnContinuousPassiveXllReqTelegramReceive(ByVal direction As XllDirection, ByVal oXllReqTeleg As IXllReqTelegram) As Boolean
        Dim sSeqName As String = If(direction = XllDirection.Dll, "PassiveDll", "PassiveUll")
        Select Case oXllReqTeleg.ContinueCode
            Case ContinueCode.Finish
                Log.Info(sSeqName & "Finish REQ received.")
                UnregisterTimer(oPassiveXllLimitTimer)

                Dim nakCause As NakCauseCode = NakCauseCode.None
                If direction = XllDirection.Dll Then
                    'NOTE: 既にクライアントは受信ファイルの保存を完了しているはずである。
                    ProcOnPassiveDllComplete(oXllReqTeleg)
                Else
                    oXllReqTeleg.ImportFileDependentValueFromSameKind(oPassiveXllQueue.First.Value.ReqTeleg)
                    If Not oXllReqTeleg.IsHashValueIndicatingOkay Then
                        Log.Error("The hash values differ from one another.")
                        nakCause = ProcOnPassiveUllHashValueError(oXllReqTeleg)
                    Else
                        'NOTE: 下記のメソッドでは、受信したファイルの保存を行うはずである。
                        '問題はそれを呼び出すタイミングであるが、たとえソケットへのACK電文
                        '書き込み成功後に呼び出したとしても、クライアントがこちらのファイル保存
                        'を認識している（認識する）か否かはわからないため、どのみちクライアント
                        'との認識が合わなくなる可能性は排除できない。
                        'そもそも、たとえソケットへのACK電文書き込みでエラーが返ってきたとしても、
                        'どの時点のエラーかは区別していないため、クライアントにはACK電文が
                        '届いてしまっている恐れがある。その際にこちらがファイルを保存して
                        'いないというのは最悪であり、そのような事態を避けるために
                        '保存できるものは先に保存しておく。
                        nakCause = ProcOnPassiveUllComplete(oXllReqTeleg)
                    End If
                End If
                oPassiveXllQueue.RemoveFirst()
                UpdatePassiveXllStateAfterDequeue()

                If nakCause = NakCauseCode.None Then
                    Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                    Log.Info("Sending " & sSeqName & "Finish ACK...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                Else
                    Dim oReplyTeleg As INakTelegram = oXllReqTeleg.CreateNakTelegram(nakCause)
                    If oReplyTeleg Is Nothing Then
                        Disconnect()
                        Return True
                    End If

                    Log.Info("Sending " & sSeqName & "Finish NAK (" & nakCause.ToString() & ")...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                End If

                'NOTE: あらゆるモードを考慮しても、受動的ファイル転送シーケンスの完了時に
                '実施するべき能動的シーケンスはない（ファイル転送シーケンス排他増強モード
                'であっても、受動的ファイル転送シーケンスは能動的ファイル転送シーケンスを
                'ブロックしているわけではない）。よって、DoNextActiveSeq()は省略する。
                Return True

            Case ContinueCode.FinishWithoutStoring
                Log.Info(sSeqName & "FinishWithoutStoring REQ received.")
                UnregisterTimer(oPassiveXllLimitTimer)

                If direction = XllDirection.Dll Then
                    'NOTE: 既にクライアントは受信ファイルの保存中止を決定している。
                    ProcOnPassiveDllCompleteWithoutStoring(oXllReqTeleg)
                Else
                    'NOTE: Ull用電文のクラスにおいて、ContinueCodeに
                    'ContinueCode.FinishWithoutStoringをマッピング
                    'するのは禁止である。
                    Debug.Fail("This case is impermissible.")
                    Abort()
                End If
                oPassiveXllQueue.RemoveFirst()
                UpdatePassiveXllStateAfterDequeue()

                Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                Log.Info("Sending " & sSeqName & "FinishWithoutStoring ACK...")
                If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                    Disconnect()
                    Return True
                End If

                'NOTE: あらゆるモードを考慮しても、受動的ファイル転送シーケンスの完了時に
                '実施するべき能動的シーケンスはない（ファイル転送シーケンス排他増強モード
                'であっても、受動的ファイル転送シーケンスは能動的ファイル転送シーケンスを
                'ブロックしているわけではない）。よって、DoNextActiveSeq()は省略する。
                Return True

            Case ContinueCode.Abort
                Log.Error(sSeqName & "Abort REQ received.")
                UnregisterTimer(oPassiveXllLimitTimer)

                If direction = XllDirection.Dll Then
                    ProcOnPassiveDllAbort(oXllReqTeleg)
                Else
                    ProcOnPassiveUllAbort(oXllReqTeleg)
                End If
                oPassiveXllQueue.RemoveFirst()
                UpdatePassiveXllStateAfterDequeue()

                'NOTE: 一部のプロトコル仕様書に曖昧な記述があり、このケースでは
                'NAK電文を返信するべきであるようにも読み取れる。
                'ただし、全体的にみて、REQ電文（転送失敗を示す）自体に異常が
                'なければACK電文を返すべきであるし、ACK電文を返すシーケンス図も
                '存在している。よって、上記の曖昧な記述は、REQ電文自体に異常が
                'あるケースについて（こんなケースもあるということを示すために）
                '記載されていると考えることにする。
                Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                Log.Info("Sending " & sSeqName & "Abort ACK...")
                SendReplyTelegram(oReplyTeleg, oXllReqTeleg)
                '上記呼び出しの戻り値は無視する（その後の処理に差異がないため）。
                Disconnect()
                Return True

            Case Else
                Log.Error(sSeqName & " REQ with invalid ContinueCode received.")
                SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oXllReqTeleg)
                Return True
        End Select
    End Function

    Protected Overridable Function ProcOnPassiveXllReqTelegramReceive(ByVal direction As XllDirection, ByVal oRcvTeleg As ITelegram) As Boolean
        Dim sSeqName As String = If(direction = XllDirection.Dll, "PassiveDll", "PassiveUll")
        Dim oXllReqTeleg As IXllReqTelegram = If(direction = XllDirection.Dll, ParseAsPassiveDllReq(oRcvTeleg), ParseAsPassiveUllReq(oRcvTeleg))
        Dim violation As NakCauseCode = oXllReqTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error(sSeqName & " REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Select Case oXllReqTeleg.ContinueCode
            Case ContinueCode.Start
                If enableXllStrongExclusion Then
                    If curActiveXllState = ActiveXllState.BeforeFtp Then
                        Dim oActiveXll As ActiveXll = oActiveXllQueue.First.Value
                        If oLastSentReqTeleg Is oActiveXll.ReqTeleg Then
                            'NOTE: ActiveXllの転送開始REQ電文を送信して応答受信
                            '待ちをしている場合である。ここでPassiveXllの転送
                            '開始にACK電文を返してしまえば、ActiveXllの転送開始
                            'REQ電文に対してACK電文が返ってきた場合に、もう
                            'PassiveXllをビジーとすることもできない（排他したい
                            'シーケンスの同時実行を避けるには、コネクションを
                            '切断するくらいしか手段がなくなってしまう）。
                            'また、ActiveXllに関する相手からの応答電文を待って
                            'から、PassiveXllに関するこちらの応答を決めるのは
                            'ご法度である（相手も同様のことをすれば、双方で
                            '応答受信タイムアウトとなる）。
                            'よって、双方がビジーを返すことになる可能性は生じる
                            'が、この時点でビジーとする。
                            Log.Info(sSeqName & "Start REQ received in ActiveXll engaged state.")

                            Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateNakTelegram(NakCauseCode.Busy)
                            If oReplyTeleg Is Nothing Then
                                Disconnect()
                                Return True
                            End If

                            Log.Info("Sending " & sSeqName & "Start NAK (" & NakCauseCode.Busy.ToString() & ")...")
                            If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                                Disconnect()
                                Return True
                            End If
                            Return True
                        End If
                    ElseIf curActiveXllState = ActiveXllState.Ftp Then
                        Log.Info(sSeqName & "Start REQ received while waiting for ActiveXllFinish REQ.")

                        Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateNakTelegram(NakCauseCode.Busy)
                        If oReplyTeleg Is Nothing Then
                            Disconnect()
                            Return True
                        End If

                        Log.Info("Sending " & sSeqName & "Start NAK (" & NakCauseCode.Busy.ToString() & ")...")
                        If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                            Disconnect()
                            Return True
                        End If
                        Return True
                    End If
                End If

                If curPassiveXllState = PassiveXllState.Ftp Then
                    'NOTE: 理論上は、ファイル転送を実施中に新たなファイル転送
                    'の開始を要求されたとしても、SubCmdCodeやObjCodeもしくは
                    'ObjDetailのSubObjCodeやファイル名等の違いで区別がつくなら、
                    '誤りであるとは言い切れない。よって、下記のようにコネクション
                    '終了に持ち込むのではなく、ビジーを返却する。
                    'Log.Error(sSeqName & "Start REQ received while waiting for PassiveXllFinish REQ.")
                    'SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oRcvTeleg)
                    'Return True

                    Log.Warn(sSeqName & "Start REQ received while waiting for PassiveXllFinish REQ.")

                    Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateNakTelegram(NakCauseCode.Busy)
                    If oReplyTeleg Is Nothing Then
                        Disconnect()
                        Return True
                    End If

                    Log.Info("Sending " & sSeqName & "Start NAK (" & NakCauseCode.Busy.ToString() & ")...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                    Return True
                End If

                Log.Info(sSeqName & "Start REQ received.")

                Dim nakCause As NakCauseCode
                If direction = XllDirection.Dll Then
                    nakCause = PrepareToStartPassiveDll(oXllReqTeleg)
                Else
                    nakCause = PrepareToStartPassiveUll(oXllReqTeleg)
                End If

                If nakCause = NakCauseCode.None Then
                    If direction = XllDirection.Dll Then
                        oXllReqTeleg.UpdateHashValue()
                    End If
                    Dim oReplyTeleg As ITelegram = oXllReqTeleg.CreateAckTelegram()
                    Log.Info("Sending " & sSeqName & "Start ACK...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If

                    oPassiveXllQueue.AddLast(New PassiveXll(direction, oXllReqTeleg))
                    TransitPassiveXllState(PassiveXllState.Ftp)
                    If oXllReqTeleg.TransferLimitTicks > 0 Then
                        oPassiveXllLimitTimer.Renew(oXllReqTeleg.TransferLimitTicks)
                        RegisterTimer(oPassiveXllLimitTimer, TickTimer.GetSystemTick())
                    End If
                Else
                    Dim oReplyTeleg As INakTelegram = oXllReqTeleg.CreateNakTelegram(nakCause)
                    If oReplyTeleg Is Nothing Then
                        Disconnect()
                        Return True
                    End If

                    Log.Info("Sending " & sSeqName & "Start NAK (" & nakCause.ToString() & ")...")
                    If SendReplyTelegram(oReplyTeleg, oXllReqTeleg) = False Then
                        Disconnect()
                        Return True
                    End If
                End If
                Return True

            Case Else
                Log.Error(sSeqName & " REQ with invalid ContinueCode received.")
                SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oRcvTeleg)
                Return True
        End Select
    End Function

    'NOTE: MayOverride
    Protected Overridable Function ProcOnPassiveOneReqTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Log.Error("REQ telegram with invalid Kind received.")
        SendNakTelegramThenDisconnect(NakCauseCode.TelegramError, oRcvTeleg)
        Return True
    End Function

    Protected Overridable Function ProcOnAckTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        If curState <> State.WaitingForReply Then
            Log.Error("ACK telegram received in disproportionate state.")
            Disconnect()
            Return True
        End If

        If Not oLastSentReqTeleg.IsValidAck(oRcvTeleg) Then
            Log.Error("ACK telegram with disproportionate HeadPart received.")
            Disconnect()
            Return True
        End If

        UnregisterTimer(oReplyLimitTimer)

        Dim toBeContinued As Boolean = True
        If oLastSentReqTeleg Is oWatchdogReqTeleg Then
            toBeContinued = ProcOnWatchdogAckTelegramReceive(oRcvTeleg)
        ElseIf oLastSentActiveOne IsNot Nothing Then
            Debug.Assert(oLastSentReqTeleg Is oLastSentActiveOne.ReqTeleg)
            toBeContinued = ProcOnActiveOneAckTelegramReceive(oRcvTeleg)
        ElseIf curActiveXllState = ActiveXllState.BeforeFtp AndAlso _
           oLastSentReqTeleg Is oActiveXllQueue.First.Value.ReqTeleg Then
            toBeContinued = ProcOnActiveXllAckTelegramReceive(oRcvTeleg)
        Else
            Debug.Fail("This case is impermissible.")
            Disconnect()
            Return True
        End If

        If curState = State.WaitingForReply Then
            If ProcOnReqTelegramSendCompleteByReceiveAck(oLastSentReqTeleg, oRcvTeleg) = False Then
                Disconnect()
                Return True
            End If
            TransitState(State.Idling)
            oLastSentReqTeleg = Nothing
        End If

        'サブメソッドがTelegrapherを終了すべきと判断している場合は、
        '以降の処理は行わない。
        If Not toBeContinued Then
            Return False
        End If

        'NOTE: Disconnect()などでcurStateがState.NoConnectionに変更された
        '場合は、DoNextActiveSeqを呼び出さないようにしている。そのような
        '場合は、isPendingFooBarや各Queueもクリアされているため、たとえ
        '呼び出したとしても、DoNextActiveSeqは何も行わないはずであるが、
        '定型的に条件をつけている。
        If curState = State.Idling Then
            DoNextActiveSeq()
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnWatchdogAckTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim violation As NakCauseCode = oLastSentReqTeleg.ParseAsAck(oRcvTeleg).GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("Watchdog ACK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Log.Info("Watchdog ACK received.")
        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneAckTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim oAckTeleg As ITelegram = oLastSentActiveOne.ReqTeleg.ParseAsAck(oRcvTeleg)
        Dim violation As NakCauseCode = oAckTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error(oLastSentActiveOne.SeqName & " ACK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Log.Info(oLastSentActiveOne.SeqName & " ACK received.")

        'NOTE: 能動的通知シーケンスにおいて、下記メソッドは、当該クライアント
        'に対するデータ通知状況の認識を更新するためのメソッドである。
        'NOTE: 能動的要求シーケンスにおいて、下記メソッドは、受信したデータの
        '保存を行うためのメソッドである。なお、クライアントは、たとえACK電文の
        'ソケットへの書き込みが成功したからといって、こちらがそのデータを保存
        'したと判断するわけにはいかない（正確な判断は、次のREQ電文を受信する
        'まで不可能である）。よって、このシーケンスで受け取るデータに関しては、
        'クライアント側で送信済みか否かを管理するとは考えられないため、
        'こちらにおいて受信済みか否かを管理するべきである。
        ProcOnActiveOneComplete(oLastSentActiveOne.ReqTeleg, oAckTeleg)
        oActiveOneQueue.Remove(oLastSentActiveOne)
        oLastSentActiveOne = Nothing
        Return True
    End Function

    Protected Overridable Function ProcOnActiveXllAckTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim oCurXll As ActiveXll = oActiveXllQueue.First.Value
        Dim sSeqName As String = If(oCurXll.Direction = XllDirection.Dll, "ActiveDll", "ActiveUll")
        Dim oAckTeleg As IXllTelegram = oCurXll.ReqTeleg.ParseAsAck(oRcvTeleg)
        Dim violation As NakCauseCode = oAckTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error(sSeqName & " ACK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        If oAckTeleg.ContinueCode <> ContinueCode.Start Then
            Log.Error(sSeqName & " ACK with disproportionate ContinueCode received.")
            Disconnect()
            Return True
        End If

        Log.Info(sSeqName & "Start ACK received.")

        If oCurXll.Direction = XllDirection.Ull Then
            oCurXll.ReqTeleg.ImportFileDependentValueFromAck(oAckTeleg)
        End If

        TransitActiveXllState(ActiveXllState.Ftp)
        If oCurXll.ReqTeleg.TransferLimitTicks > 0 Then
            oActiveXllLimitTimer.Renew(oCurXll.ReqTeleg.TransferLimitTicks)
            RegisterTimer(oActiveXllLimitTimer, TickTimer.GetSystemTick())
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnNakTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        If curState <> State.WaitingForReply Then
            Log.Error("NAK telegram received in disproportionate state.")
            Disconnect()
            Return True
        End If

        If Not oLastSentReqTeleg.IsValidNak(oRcvTeleg) Then
            Log.Error("NAK telegram with disproportionate HeadPart received.")
            Disconnect()
            Return True
        End If

        UnregisterTimer(oReplyLimitTimer)

        Dim toBeContinued As Boolean = True
        If oLastSentReqTeleg Is oWatchdogReqTeleg Then
            toBeContinued = ProcOnWatchdogNakTelegramReceive(oRcvTeleg)
        ElseIf oLastSentActiveOne IsNot Nothing Then
            Debug.Assert(oLastSentReqTeleg Is oLastSentActiveOne.ReqTeleg)
            toBeContinued = ProcOnActiveOneNakTelegramReceive(oRcvTeleg)
        ElseIf curActiveXllState = ActiveXllState.BeforeFtp AndAlso _
           oLastSentReqTeleg Is oActiveXllQueue.First.Value.ReqTeleg Then
            toBeContinued = ProcOnActiveXllNakTelegramReceive(oRcvTeleg)
        Else
            Debug.Fail("This case is impermissible.")
            Disconnect()
            Return True
        End If

        If curState = State.WaitingForReply Then
            If ProcOnReqTelegramSendCompleteByReceiveNak(oLastSentReqTeleg, oRcvTeleg) = False Then
                Disconnect()
                Return True
            End If
            TransitState(State.Idling)
            oLastSentReqTeleg = Nothing
        End If

        'サブメソッドがTelegrapherを終了すべきと判断している場合は、
        '以降の処理は行わない。
        If Not toBeContinued Then
            Return False
        End If

        'NOTE: Disconnect()などでcurStateがState.NoConnectionに変更された
        '場合は、DoNextActiveSeqを呼び出さないようにしている。そのような
        '場合は、isPendingFooBarや各Queueもクリアされているため、たとえ
        '呼び出したとしても、DoNextActiveSeqは何も行わないはずであるが、
        '定型的に条件をつけている。
        If curState = State.Idling Then
            DoNextActiveSeq()
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnWatchdogNakTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim oNakTeleg As INakTelegram = oLastSentReqTeleg.ParseAsNak(oRcvTeleg)
        Dim violation As NakCauseCode = oNakTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("Watchdog NAK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Dim nakCause As NakCauseCode = oNakTeleg.CauseCode
        If GetRequirement(oNakTeleg) = NakRequirement.DisconnectImmediately Then
            Log.Error("Watchdog NAK (" & nakCause.ToString() & ") received.")
            Disconnect()
        Else
            Log.Warn("Watchdog NAK (" & nakCause.ToString() & ") received.")
            'NOTE: 再送タイマは開始せず、次の通常送信に委ねる。
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneNakTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim oNakTeleg As INakTelegram = oLastSentReqTeleg.ParseAsNak(oRcvTeleg)
        Dim violation As NakCauseCode = oNakTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error(oLastSentActiveOne.SeqName & " NAK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Dim nakCause As NakCauseCode = oNakTeleg.CauseCode
        Dim requirement As NakRequirement = GetRequirement(oNakTeleg)
        If requirement = NakRequirement.DisconnectImmediately Then
            Log.Error(oLastSentActiveOne.SeqName & " NAK (" & nakCause.ToString() & ") received.")
            Disconnect()
        Else
            Log.Info(oLastSentActiveOne.SeqName & " NAK (" & nakCause.ToString() & ") received.")

            If requirement = NakRequirement.CareOnRetryOver Then
                oLastSentActiveOne.CurNakCountToCare += 1
                If oLastSentActiveOne.CurNakCountToCare >= oLastSentActiveOne.LimitNakCountToCare Then
                    Log.Warn(oLastSentActiveOne.SeqName & " retry over.")
                    ProcOnActiveOneRetryOverToCare(oLastSentActiveOne.ReqTeleg, oNakTeleg)
                    oActiveOneQueue.Remove(oLastSentActiveOne)
                Else
                    RegisterTimer(oLastSentActiveOne.RetryTimer, TickTimer.GetSystemTick())
                End If
            Else
                oLastSentActiveOne.CurNakCountToForget += 1
                oLastSentActiveOne.CurNakCountToCare = 0
                If oLastSentActiveOne.CurNakCountToForget >= oLastSentActiveOne.LimitNakCountToForget Then
                    Log.Info(oLastSentActiveOne.SeqName & " retry over.")
                    ProcOnActiveOneRetryOverToForget(oLastSentActiveOne.ReqTeleg, oNakTeleg)
                    oActiveOneQueue.Remove(oLastSentActiveOne)
                Else
                    RegisterTimer(oLastSentActiveOne.RetryTimer, TickTimer.GetSystemTick())
                End If
            End If

            oLastSentActiveOne = Nothing
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnActiveXllNakTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim oCurXll As ActiveXll = oActiveXllQueue.First.Value
        Dim sSeqName As String = If(oCurXll.Direction = XllDirection.Dll, "ActiveDll", "ActiveUll")

        Dim oNakTeleg As INakTelegram = oLastSentReqTeleg.ParseAsNak(oRcvTeleg)
        Dim violation As NakCauseCode = oNakTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error(sSeqName & " NAK with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Debug.Assert(oCurXll.ReqTeleg.ContinueCode = ContinueCode.Start)

        Dim nakCause As NakCauseCode = oNakTeleg.CauseCode
        Dim requirement As NakRequirement = GetRequirement(oNakTeleg)
        If requirement = NakRequirement.DisconnectImmediately Then
            Log.Error(sSeqName & " NAK (" & nakCause.ToString() & ") received.")
            Disconnect()
        Else
            Log.Info(sSeqName & " NAK (" & nakCause.ToString() & ") received.")

            If requirement = NakRequirement.CareOnRetryOver Then
                oCurXll.CurNakCountToCare += 1
                If oCurXll.CurNakCountToCare >= oCurXll.LimitNakCountToCare Then
                    Log.Warn(sSeqName & " retry over.")
                    If oCurXll.Direction = XllDirection.Dll Then
                        ProcOnActiveDllRetryOverToCare(oCurXll.ReqTeleg, oNakTeleg)
                    Else
                        ProcOnActiveUllRetryOverToCare(oCurXll.ReqTeleg, oNakTeleg)
                    End If
                    oActiveXllQueue.RemoveFirst()
                    UpdateActiveXllStateAfterDequeue()
                Else
                    oActiveXllRetryTimer.Renew(oCurXll.RetryIntervalTicks)
                    RegisterTimer(oActiveXllRetryTimer, TickTimer.GetSystemTick())
                End If
            Else
                oCurXll.CurNakCountToForget += 1
                oCurXll.CurNakCountToCare = 0
                If oCurXll.CurNakCountToForget >= oCurXll.LimitNakCountToForget Then
                    Log.Info(sSeqName & " retry over.")
                    If oCurXll.Direction = XllDirection.Dll Then
                        ProcOnActiveDllRetryOverToForget(oCurXll.ReqTeleg, oNakTeleg)
                    Else
                        ProcOnActiveUllRetryOverToForget(oCurXll.ReqTeleg, oNakTeleg)
                    End If
                    oActiveXllQueue.RemoveFirst()
                    UpdateActiveXllStateAfterDequeue()
                Else
                    oActiveXllRetryTimer.Renew(oCurXll.RetryIntervalTicks)
                    RegisterTimer(oActiveXllRetryTimer, TickTimer.GetSystemTick())
                End If
            End If
        End If

        Return True
    End Function

    Protected Overrides Sub ProcOnUnhandledException(ByVal ex As Exception)
        'NOTE: リソース解放のために、このスレッドを強制終了する際は、必ず
        '下記を呼び出しておきたい。

        'TODO: Abort()が呼び出された際にも、必ずここが実行されることを確認。

        If oTelegSock IsNot Nothing Then
            UnregisterSocket(oTelegSock)
            oTelegSock.Close()
            oTelegSock = Nothing
        End If

        'NOTE: 親スレッド側で対端のソケットを読み書きしようとした際に
        'エラーが発生するはずである。親スレッドは、そのことを前提にして、
        '実装しなければならない。
        If oParentMessageSock IsNot Nothing Then
            UnregisterSocket(oParentMessageSock)
            oParentMessageSock.Close()
            oParentMessageSock = Nothing
        End If

        'このまま呼び元に戻って、スレッドは終了状態になる。
    End Sub
#End Region

#Region "イベント処理実装用メソッド"
    Protected Sub RegisterActiveOne( _
       ByVal oReqTeleg As IReqTelegram, _
       ByVal retryIntervalTicks As Integer, _
       ByVal limitNakCountToForget As Integer, _
       ByVal limitNakCountToCare As Integer, _
       ByVal sSeqName As String)

        If curState = State.NoConnection Then
            ProcOnActiveOneAnonyError(oReqTeleg)
            Return
        End If

        oActiveOneQueue.AddLast(New ActiveOne(oReqTeleg, retryIntervalTicks, limitNakCountToForget, limitNakCountToCare, sSeqName))
        If curState = State.Idling Then
            DoNextActiveSeq()
        End If
    End Sub

    Protected Sub RegisterActiveDll( _
       ByVal oXllReqTeleg As IXllReqTelegram, _
       ByVal retryIntervalTicks As Integer, _
       ByVal limitNakCountToForget As Integer, _
       ByVal limitNakCountToCare As Integer)

        If curState = State.NoConnection Then
            ProcOnActiveDllAnonyError(oXllReqTeleg)
            Return
        End If

        If Not oXllReqTeleg.IsHashValueReady Then
            oXllReqTeleg.UpdateHashValue()
        End if

        oActiveXllQueue.AddLast(New ActiveXll(XllDirection.Dll, oXllReqTeleg, retryIntervalTicks, limitNakCountToForget, limitNakCountToCare))
        If curActiveXllState = ActiveXllState.None Then
            TransitActiveXllState(ActiveXllState.BeforeFtp)
            If curState = State.Idling Then
                DoNextActiveSeq()
            End If
        End If
    End Sub

    Protected Sub RegisterActiveUll( _
       ByVal oXllReqTeleg As IXllReqTelegram, _
       ByVal retryIntervalTicks As Integer, _
       ByVal limitNakCountToForget As Integer, _
       ByVal limitNakCountToCare As Integer)

        If curState = State.NoConnection Then
            ProcOnActiveUllAnonyError(oXllReqTeleg)
            Return
        End If

        oActiveXllQueue.AddLast(New ActiveXll(XllDirection.Ull, oXllReqTeleg, retryIntervalTicks, limitNakCountToForget, limitNakCountToCare))
        If curActiveXllState = ActiveXllState.None Then
            TransitActiveXllState(ActiveXllState.BeforeFtp)
            If curState = State.Idling Then
                DoNextActiveSeq()
            End If
        End If
    End Sub

    'NOTE: curStateがIdlingになった際に呼ぶべきメソッド。
    Protected Sub DoNextActiveSeq()
        If isPendingWatchdog Then
            isPendingWatchdog = False

            'NOTE: 状況に応じてウォッチドッグの有無や電文内容を変更する類の
            'プロトコルを想定し、勿体ないが毎回作成しなおすことにしている。
            oWatchdogReqTeleg = CreateWatchdogReqTelegram()
            If oWatchdogReqTeleg IsNot Nothing Then
                Log.Info("Sending Watchdog REQ...")
                If SendReqTelegram(oWatchdogReqTeleg) = False Then
                    Disconnect()
                    Return
                End If

                TransitState(State.WaitingForReply)
                oLastSentReqTeleg = oWatchdogReqTeleg
                oReplyLimitTimer.Renew(oWatchdogReqTeleg.ReplyLimitTicks)
                RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
                Return
            End If
        End If

        If enableActiveSeqStrongExclusion AndAlso _
           curActiveXllState = ActiveXllState.Ftp Then
            '実行中のActiveXllによってActiveOneのための通信が禁止されている
            '状態である。この状態では、処理を待っていたActiveOneは基本的に
            '全て処理可能である（試行回数を増進させるだけであるため）。

            'NOTE: curActiveXllStateが上記の場合だけでなく、BeforeFtpの場合も、
            '能動的ファイル転送シーケンスの転送開始REQ電文が送信済みという
            '可能性はある（相手がACK電文を送信している可能性を考慮すると、
            '能動的単発シーケンスと排他的な状態とみなせる）。
            'しかし、そのような状態であるならば、このメソッドは呼び出されない
            'はずである（このメソッドは、Idling状態でのみ呼び出される）。

            If enableActiveOneOrdering Then
                '能動的単発シーケンス順序強制モードの場合である。このモード
                'では、処理を待っていたActiveOneのうち、まだ開始していない
                'ものについては、実行中のActiveOneがない場合にのみ開始する
                'よう、制限する必要がある。

                '実行中のActiveOneの件数を調べる。
                Dim executingActiveOneCount As Integer = 0
                For Each oOne As ActiveOne In oActiveOneQueue
                    If oOne.CurTryCount <> 0 Then
                        executingActiveOneCount += 1
                    End If
                Next oOne

                '再送信ペンディング状態になっている全てのActiveOneを処理する。
                'NOTE: 能動的単発シーケンス順序強制モードであっても、そのように
                '過去に（能動的単発シーケンス順序強制モードでなかったときに）
                '一度でも送信したActiveOneについては、可能であれば（Idling状態
                'が続く限りは）何件でも処理する方針である。
                'NOTE: 処理の結果、実行中でなくなったものは、調べた件数から
                '差し引く。
                While oActiveOneRetryPendingQueue.Count <> 0
                    Dim oOne As ActiveOne = oActiveOneRetryPendingQueue.Dequeue()
                    Log.Info(oOne.SeqName & " is regulated by ActiveXll.")
                    'NOTE: REQ電文を送信してNAK（ビジー）電文を受信した場合と
                    '同等の処理を行う。
                    'NOTE: oActiveOneRetryPendingQueueに登録してあるActiveOneは
                    '全て実行中である（oOne.CurTryCountは既に1以上である）
                    'ため、ここでは、executingActiveOneCountのインクリメントを
                    '考慮する必要性はない。
                    oOne.CurTryCount += 1
                    oOne.CurNakCountToCare += 1
                    If oOne.CurNakCountToCare >= oOne.LimitNakCountToCare Then
                        Log.Warn(oOne.SeqName & " retry over.")
                        ProcOnActiveOneRetryOverToCare(oOne.ReqTeleg, Nothing)
                        oActiveOneQueue.Remove(oOne)
                        executingActiveOneCount -= 1
                    Else
                        RegisterTimer(oOne.RetryTimer, TickTimer.GetSystemTick())
                    End If
                End While

                '実行中のActiveOneがない場合のみ、
                '新たに登録されたActiveOneを開始する。
                While executingActiveOneCount = 0 AndAlso oActiveOneQueue.Count <> 0
                    Dim oOne As ActiveOne = oActiveOneQueue.First.Value
                    Log.Info(oOne.SeqName & " is regulated by ActiveXll.")
                    'NOTE: REQ電文を送信してNAK（ビジー）電文を受信した場合と
                    '同等の処理を行う。
                    'NOTE: 実行中のActiveOneが増えるわけであるが、直後に
                    '実行中でなくなる（リトライオーバーする）可能性も高い
                    'ため、executingActiveOneCountのインクリメントは、
                    '条件分岐後に必要に応じて行う。
                    Debug.Assert(oOne.CurTryCount = 0)
                    oOne.CurTryCount += 1
                    oOne.CurNakCountToCare += 1
                    If oOne.CurNakCountToCare >= oOne.LimitNakCountToCare Then
                        Log.Warn(oOne.SeqName & " retry over.")
                        ProcOnActiveOneRetryOverToCare(oOne.ReqTeleg, Nothing)
                        oActiveOneQueue.Remove(oOne)
                    Else
                        RegisterTimer(oOne.RetryTimer, TickTimer.GetSystemTick())
                        executingActiveOneCount += 1
                        'NOTE: Whileから抜けることになるはずである。
                    End If
                End While
            Else
                '能動的単発シーケンス順序強制モードでない場合である。

                '再送信ペンディング状態になっている全てのActiveOneを処理する。
                While oActiveOneRetryPendingQueue.Count <> 0
                    Dim oOne As ActiveOne = oActiveOneRetryPendingQueue.Dequeue()
                    Log.Info(oOne.SeqName & " is regulated by ActiveXll.")
                    'NOTE: REQ電文を送信してNAK（ビジー）電文を受信した場合と
                    '同等の処理を行う。
                    oOne.CurTryCount += 1
                    oOne.CurNakCountToCare += 1
                    If oOne.CurNakCountToCare >= oOne.LimitNakCountToCare Then
                        Log.Warn(oOne.SeqName & " retry over.")
                        ProcOnActiveOneRetryOverToCare(oOne.ReqTeleg, Nothing)
                        oActiveOneQueue.Remove(oOne)
                    Else
                        RegisterTimer(oOne.RetryTimer, TickTimer.GetSystemTick())
                    End If
                End While

                '新たに登録されたActiveOneがあれば全て開始する。
                While oActiveOneQueue.Count <> 0
                    Dim oOne As ActiveOne = oActiveOneQueue.First.Value
                    If oOne.CurTryCount = 0 Then
                        Log.Info(oOne.SeqName & " is regulated by ActiveXll.")
                        'NOTE: REQ電文を送信してNAK（ビジー）電文を受信した場合と
                        '同等の処理を行う。
                        oOne.CurTryCount += 1
                        oOne.CurNakCountToCare += 1
                        If oOne.CurNakCountToCare >= oOne.LimitNakCountToCare Then
                            Log.Warn(oOne.SeqName & " retry over.")
                            ProcOnActiveOneRetryOverToCare(oOne.ReqTeleg, Nothing)
                            oActiveOneQueue.Remove(oOne)
                        Else
                            RegisterTimer(oOne.RetryTimer, TickTimer.GetSystemTick())
                        End If
                    End If
                End While
            End If
        Else
            'ActiveOneのための通信が禁止されていない状態である。この状態では、
            '処理を待っていたActiveOneのうち、最も優先すべきもののみ処理可能
            'である（REQ電文を送信することで、Idling状態でなくなるため）。

            If enableActiveOneOrdering Then
                '能動的単発シーケンス順序強制モードの場合である。このモード
                'では、まだ開始していないActiveOneについては、たとえ、それが
                '最も優先すべきものであるとしても、実行中のActiveOneがあれば
                '開始（送信）することはできない。

                '再送信ペンディング状態のActiveOneがあれば、最も過去に再送信
                'ペンディング状態になったものを送信対象とする。
                'なければ、新たに登録されたActiveOneのうち、最も過去に登録された
                'ものを送信対象とする。
                Dim oOne As ActiveOne = Nothing
                If oActiveOneRetryPendingQueue.Count <> 0 Then
                    'NOTE: 能動的単発シーケンス順序強制モードであっても、
                    'oActiveOneRetryPendingQueueの要素のように過去に（能動的
                    '単発シーケンス順序強制モードでなかったときに）一度でも
                    '送信したActiveOneについては、可能であれば（Idling状態が
                    '続く限りは）何件でも処理する方針である。よって、下記で
                    '取得するActiveOneについては、実行中のActiveOneの有無に
                    '関係なく、処理する必要がある。
                    oOne = oActiveOneRetryPendingQueue.Dequeue()
                Else
                    '実行中のActiveOneの有無を調べる。
                    Dim isThereExecutingActiveOne As Boolean = False
                    For Each oQueuingOne As ActiveOne In oActiveOneQueue
                        If oQueuingOne.CurTryCount <> 0 Then
                            isThereExecutingActiveOne = True
                            Exit For
                        End If
                    Next oQueuingOne

                    '実行中のActiveOneがない場合のみ、
                    '新たに登録されたActiveOneを送信対象とする。
                    If Not isThereExecutingActiveOne Then
                        For Each oQueuingOne As ActiveOne In oActiveOneQueue
                            If oQueuingOne.CurTryCount = 0 Then
                                oOne = oQueuingOne
                                Exit For
                            End If
                        Next oQueuingOne
                    End If
                End If

                '送信対象に選んだActiveOneを送信する。
                If oOne IsNot Nothing Then
                    Log.Info("Sending " & oOne.SeqName & " REQ...")
                    oOne.CurTryCount += 1
                    If SendReqTelegram(oOne.ReqTeleg) = False Then
                        Disconnect()
                        Return
                    End If

                    TransitState(State.WaitingForReply)
                    oLastSentReqTeleg = oOne.ReqTeleg
                    oLastSentActiveOne = oOne
                    oReplyLimitTimer.Renew(oOne.ReqTeleg.ReplyLimitTicks)
                    RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
                    Return
                End If
            Else
                '能動的単発シーケンス順序強制モードでない場合である。

                '再送信ペンディング状態のActiveOneがあれば、最も過去に再送信
                'ペンディング状態になったものを送信対象とする。
                'なければ、新たに登録されたActiveOneのうち、最も過去に登録された
                'ものを送信対象とする。
                Dim oOne As ActiveOne = Nothing
                If oActiveOneRetryPendingQueue.Count <> 0 Then
                    oOne = oActiveOneRetryPendingQueue.Dequeue()
                Else
                    For Each oQueuingOne As ActiveOne In oActiveOneQueue
                        If oQueuingOne.CurTryCount = 0 Then
                            oOne = oQueuingOne
                            Exit For
                        End If
                    Next oQueuingOne
                End If

                '送信対象に選んだActiveOneを送信する。
                If oOne IsNot Nothing Then
                    Log.Info("Sending " & oOne.SeqName & " REQ...")
                    oOne.CurTryCount += 1
                    If SendReqTelegram(oOne.ReqTeleg) = False Then
                        Disconnect()
                        Return
                    End If

                    TransitState(State.WaitingForReply)
                    oLastSentReqTeleg = oOne.ReqTeleg
                    oLastSentActiveOne = oOne
                    oReplyLimitTimer.Renew(oOne.ReqTeleg.ReplyLimitTicks)
                    RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
                    Return
                End If
            End If
        End If

        If oActiveXllQueue.Count <> 0 Then
            Dim oXll As ActiveXll = oActiveXllQueue.First.Value
            If oXll.CurTryCount = 0 OrElse isPendingActiveXllRetry Then
                isPendingActiveXllRetry = False
                'NOTE: このメソッドはIdling状態でのみ呼ばれるため、
                'たとえ能動的シーケンス排他増強モードであっても、
                '能動的単発シーケンスの実行状態を気にする必要はない。
                If enableXllStrongExclusion AndAlso _
                   curPassiveXllState = PassiveXllState.Ftp Then
                    'NOTE: シーケンス開始前の能動的電文交換待ちの間や
                    'リトライインターバル＋リトライの能動的電文交換待ちの間に
                    'クライアントからPassiveXllの転送開始REQ電文を受信し、
                    'それを受け付けていた場合である。
                    Log.Info("ActiveXll is regulated by PassiveXll.")
                    'NOTE: REQ電文を送信してNAK（ビジー）電文を受信した場合と
                    '同等の処理を行う。
                    oXll.CurTryCount += 1
                    oXll.CurNakCountToCare += 1
                    If oXll.CurNakCountToCare >= oXll.LimitNakCountToCare Then
                        If oXll.Direction = XllDirection.Dll Then
                            Log.Warn("ActiveDll retry over.")
                            ProcOnActiveDllRetryOverToCare(oXll.ReqTeleg, Nothing)
                        Else
                            Log.Warn("ActiveUll retry over.")
                            ProcOnActiveUllRetryOverToCare(oXll.ReqTeleg, Nothing)
                        End If
                        oActiveXllQueue.RemoveFirst()
                        UpdateActiveXllStateAfterDequeue()
                    Else
                        oActiveXllRetryTimer.Renew(oXll.RetryIntervalTicks)
                        RegisterTimer(oActiveXllRetryTimer, TickTimer.GetSystemTick())
                    End If
                Else
                    If oXll.Direction = XllDirection.Dll Then
                        Log.Info("Sending ActiveDllStart REQ...")
                    Else
                        Log.Info("Sending ActiveUllStart REQ...")
                    End If
                    oXll.CurTryCount += 1
                    If SendReqTelegram(oXll.ReqTeleg) = False Then
                        Disconnect()
                        Return
                    End If

                    TransitState(State.WaitingForReply)
                    oLastSentReqTeleg = oXll.ReqTeleg
                    oReplyLimitTimer.Renew(oXll.ReqTeleg.ReplyLimitTicks)
                    RegisterTimer(oReplyLimitTimer, TickTimer.GetSystemTick())
                    Return
                End If
            End If
        End If
    End Sub

    'NOTE: 受信電文に異常がある場合は、その電文が他のシーケンスの終わりを告げる
    '電文である可能性を想定するべきである。すなわち、コネクションを終了させて、
    'キューイングしているシーケンス等、コネクションに従属するリソースを解放する
    'べきである。よって、NAK事由に動作を左右されないよう、このメソッドで、
    'NAK電文の返信とコネクション終了を行う方針とする。
    '逆に、正しい電文を受信した後、自らの内部都合等に従ってNAK電文を返信する
    '場合は、NAK事由によってコネクション存続の有無が決まるよう、このメソッド
    'の使用は控える方針とする。
    Protected Sub SendNakTelegramThenDisconnect(ByVal cause As NakCauseCode, ByVal oSourceTeleg As ITelegram)
        Dim oReplyTeleg As ITelegram = oSourceTeleg.CreateNakTelegram(cause)
        If oReplyTeleg IsNot Nothing Then
            Log.Info("Sending NAK (" & cause.ToString() & ") telegram...")
            SendReplyTelegram(oReplyTeleg, oSourceTeleg)
            '上記呼び出しの戻り値は無視する（その後の処理に差異がないため）。
        End If
        Disconnect()
    End Sub

    Protected Sub Connect(ByVal oNewTelegSock As Socket)
        Debug.Assert(curState = State.NoConnection)

        oTelegSock = oNewTelegSock
        RegisterSocket(oTelegSock)
        TransitState(State.Idling)
        oLastSentReqTeleg = Nothing

        ProcOnConnectionAppear()
    End Sub

    'NOTE: ProcOnActiveDllXxxx等のメソッド（各シーケンスが終了する際の業務依存処理を実装するためのメソッド）
    'からは呼び出し禁止である。それらのメソッドは通信制御の中で呼ばれるフックである。言い方を変えると、
    'Disconnectメソッドは、それらから呼ぶためのメソッドではなく、それらを呼び出す側のメソッドである。
    'それらのメソッドの中で行った自律的な判定に基づいてコネクションを終了させるには、
    'ProcOnReqTelegramReceiveCompleteBySendXxx等がFalseを返すように、それらのメソッドの中で
    'インスタンスの内部状態を変更するのが、理想的である。
    Protected Sub Disconnect()
        'NOTE: このメソッドにおいては、oActiveXllQueueの先頭要素が変化する
        'たびにTransitActiveXllState()を呼び出すわけではない。
        'つまり、TransitActiveXllState()等で行う処理は、あくまで、
        'このLooperが１つのイベントを処理する上での「処理前後」の状態変化に
        '応じて実施すべき処理（わかりやすく言うと、イベント待機方法に関する
        '設定変更）のみに限定するべきである。

        UnregisterSocket(oTelegSock)
        Log.Info("Closing current socket...")
        Try
            'NOTE: 現在想定しているプロトコルでは致命的ではないが、
            'せっかくソケットに書き込んだ応答データが送信されない
            'のはイマイチと思われるため、これを実施している。
            'また、この時点で届いていたデータがあったり、この後に
            '届いたデータがあれば、それを読まないことを知らせるために、
            '相手にRSTの送信を試みる。ただし、そのようなことに頼る
            'プロトコルは、このクラスの守備範囲外である。
            oTelegSock.Shutdown(SocketShutdown.Both)
        Catch ex As SocketException
            Log.Error("SocketException caught.", ex)
        End Try
        oTelegSock.Close()
        oTelegSock = Nothing
        UnregisterConnectionDependentTimers()
        ProcOnConnectionDisappear()

        'NOTE: キューに残っているのは、全く実行していなかった（開始前の）
        'シーケンスであるか、実行中のシーケンスであっても、当該シーケンスで
        '電文書式や応答受信タイムアウト等の一般的な異常が発生して中止した
        '場合であるか、ウォッチドッグ等の別のシーケンスで認識した異常により
        '中止された場合だけである。当該シーケンスのリトライオーバが発生した
        '場合などは、その場でDequeueして、適切なフックを呼び出しているはず
        'である。

        'NOTE: あるシーケンスで異常が発生した際、待機していた能動的な
        '配信や収集のシーケンスに関しても、失敗とする。運用上、AnonyError
        'とみなすのは微妙かもしれないが、理には適っているはず。

        For Each oOne As ActiveOne In oActiveOneQueue
            ProcOnActiveOneAnonyError(oOne.ReqTeleg)
        Next oOne
        oActiveOneQueue.Clear()
        oLastSentActiveOne = Nothing

        For Each oXll As ActiveXll In oActiveXllQueue
            If oXll.Direction = XllDirection.Dll Then
                ProcOnActiveDllAnonyError(oXll.ReqTeleg)
            Else
                ProcOnActiveUllAnonyError(oXll.ReqTeleg)
            End If
        Next oXll
        oActiveXllQueue.Clear()
        TransitActiveXllState(ActiveXllState.None)

        For Each oXll As PassiveXll In oPassiveXllQueue
            If oXll.Direction = XllDirection.Dll Then
                ProcOnPassiveDllAnonyError(oXll.ReqTeleg)
            Else
                ProcOnPassiveUllAnonyError(oXll.ReqTeleg)
            End If
        Next oXll
        oPassiveXllQueue.Clear()
        TransitPassiveXllState(PassiveXllState.None)

        TransitState(State.NoConnection)
        isPendingWatchdog = False
        oActiveOneRetryPendingQueue.Clear()
        isPendingActiveXllRetry = False
        oLastSentReqTeleg = Nothing
    End Sub

    Protected Sub UpdateActiveXllStateAfterDequeue()
        If oActiveXllQueue.Count = 0 Then
            TransitActiveXllState(ActiveXllState.None)
        Else
            TransitActiveXllState(ActiveXllState.BeforeFtp)
        End If
    End Sub

    Protected Sub UpdatePassiveXllStateAfterDequeue()
        If oPassiveXllQueue.Count = 0 Then
            TransitPassiveXllState(PassiveXllState.None)
        Else
            Debug.Fail("This case is impermissible.")
        End If
    End Sub

    Protected Overridable Sub TransitState(ByVal nextState As State)
        If IsParentMessageReceptibleState(curActiveXllState) AndAlso _
           IsParentMessageReceptibleState(curPassiveXllState) Then
            If IsParentMessageReceptibleState(curState) Then
                If Not IsParentMessageReceptibleState(nextState) Then
                    UnregisterSocket(oParentMessageSock)
                End If
            Else
                If IsParentMessageReceptibleState(nextState) Then
                    RegisterSocket(oParentMessageSock)
                End If
            End If
        Else
            'この場合、oParentMessageSockは登録されていないはずであり、
            'nextStateが何であるかに関係なく、登録するべきではない。
        End If
        curState = nextState
    End Sub

    Protected Overridable Sub TransitActiveXllState(ByVal nextState As ActiveXllState)
        If IsParentMessageReceptibleState(curState) AndAlso _
           IsParentMessageReceptibleState(curPassiveXllState) Then
            If IsParentMessageReceptibleState(curActiveXllState) Then
                If Not IsParentMessageReceptibleState(nextState) Then
                    UnregisterSocket(oParentMessageSock)
                End If
            Else
                If IsParentMessageReceptibleState(nextState) Then
                    RegisterSocket(oParentMessageSock)
                End If
            End If
        Else
            'この場合、oParentMessageSockは登録されていないはずであり、
            'nextStateが何であるかに関係なく、登録するべきではない。
        End If
        curActiveXllState = nextState
    End Sub


    Protected Overridable Sub TransitPassiveXllState(ByVal nextState As PassiveXllState)
        If IsParentMessageReceptibleState(curState) AndAlso _
           IsParentMessageReceptibleState(curActiveXllState) Then
            If IsParentMessageReceptibleState(curPassiveXllState) Then
                If Not IsParentMessageReceptibleState(nextState) Then
                    UnregisterSocket(oParentMessageSock)
                End If
            Else
                If IsParentMessageReceptibleState(nextState) Then
                    RegisterSocket(oParentMessageSock)
                End If
            End If
        Else
            'この場合、oParentMessageSockは登録されていないはずであり、
            'nextStateが何であるかに関係なく、登録するべきではない。
        End If
        curPassiveXllState = nextState
    End Sub

    'NOTE: Disconnectを行うべき状況になった場合はFalseを返却することになっている。
    'NOTE: このメソッドがProtectedなのは、派生クラスで「オーバーライドする」ことを想定しているためである。
    'このメソッドを呼んだ際は、TransitState、oLastSentReqTeleg更新、oReplyLimitTimerの登録などを
    '行う必要があるため、派生クラスで無暗に「呼び出す」べきではない。REQ電文の送信を行いたい場合は、
    'RegisterActiveOne()、RegisterActiveDll()、RegisterActiveUll()を実行するのが妥当である。
    Protected Overridable Function SendReqTelegram(ByVal oReqTeleg As IReqTelegram) As Boolean
        Return oReqTeleg.WriteToSocket(oTelegSock, telegWritingLimitBaseTicks, telegWritingLimitExtraTicksPerMiB, telegLoggingMaxLengthOnWrite)
    End Function

    'NOTE: Disconnectを行うべき状況になった場合はFalseを返却することになっている。
    'NOTE: オーバライドする場合、oSourceTelegのヘッダ部に書式違反がある可能性に
    '注意してください。バイト数など、TelegramImporter.GetTelegramFromSocket()が
    '保証することは保証されます。
    Protected Overridable Function SendReplyTelegram(ByVal oReplyTeleg As ITelegram, ByVal oSourceTeleg As ITelegram) As Boolean
        If oReplyTeleg.WriteToSocket(oTelegSock, telegWritingLimitBaseTicks, telegWritingLimitExtraTicksPerMiB, telegLoggingMaxLengthOnWrite) = False Then
            Return False
        End If

        Dim cmdKind As CmdKind = oReplyTeleg.CmdKind
        If cmdKind = CmdKind.Ack Then
            Return ProcOnReqTelegramReceiveCompleteBySendAck(oSourceTeleg, oReplyTeleg)
        ElseIf cmdKind = CmdKind.Nak Then
            If GetRequirement(DirectCast(oReplyTeleg, INakTelegram)) = NakRequirement.DisconnectImmediately Then
                Return False
            End If
            Return ProcOnReqTelegramReceiveCompleteBySendNak(oSourceTeleg, oReplyTeleg)
        Else
            Debug.Fail("This case is impermissible.")
            Return False
        End If
    End Function

    Protected Overridable Sub UnregisterConnectionDependentTimers()
        UnregisterTimer(oReplyLimitTimer)

        For Each oOne As ActiveOne In oActiveOneQueue
            UnregisterTimer(oOne.RetryTimer)
        Next oOne

        UnregisterTimer(oActiveXllRetryTimer)

        UnregisterTimer(oActiveXllLimitTimer)

        UnregisterTimer(oPassiveXllLimitTimer)
    End Sub
#End Region

#Region "表層機能カスタマイズ用仮想メソッド"
    'ウォッチドッグシーケンスのREQ電文を生成するメソッド
    'NOTE: ウォッチドッグシーケンスが不要な場合は、Nothingを返却すること。
    Protected Overridable Function CreateWatchdogReqTelegram() As IReqTelegram
        Return Nothing
    End Function

    '能動的単発シーケンスが成功した場合
    'NOTE: oReqTelegは、RegisterActiveOneに渡したものである。
    'NOTE: oAckTelegは、oReqTeleg.ParseAsAckで生成したものである。
    Protected Overridable Sub ProcOnActiveOneComplete(ByVal oReqTeleg As IReqTelegram, ByVal oAckTeleg As ITelegram)
    End Sub

    '能動的単発シーケンスで異常とみなすべきでないリトライオーバーが発生した場合
    'NOTE: oReqTelegは、RegisterActiveOneに渡したものである。
    'NOTE: oNakTelegは、oReqTeleg.ParseAsNak(oRcvTeleg)で生成したオブジェクトである。
    'oRcvTelegは、リトライオーバーの判定に至った際に受信した電文である。
    Protected Overridable Sub ProcOnActiveOneRetryOverToForget(ByVal oReqTeleg As IReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '能動的単発シーケンスで異常とみなすべきリトライオーバーが発生した場合
    'NOTE: oReqTelegは、RegisterActiveOneに渡したものである。
    'NOTE: oNakTelegは、oReqTeleg.ParseAsNak(oRcvTeleg)で生成したオブジェクトまたはNothingである。
    'oRcvTelegは、リトライオーバーの判定に至った際に受信した電文である。
    'oNakTelegがNothingになるのは、REQ電文を送信するまでもなく諦めた場合であり、
    'EnableActiveSeqStrongExclusionがTrueの場合にのみあり得る。
    Protected Overridable Sub ProcOnActiveOneRetryOverToCare(ByVal oReqTeleg As IReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '能動的単発シーケンスの最中やキューイングされた能動的単発シーケンスの実施前に通信異常を検出した場合
    'NOTE: oReqTelegは、RegisterActiveOneに渡したものである。
    Protected Overridable Sub ProcOnActiveOneAnonyError(ByVal oReqTeleg As IReqTelegram)
    End Sub

    '能動的DLLが成功した（ContinueCode.Finishの転送終了REQ電文を受信した）場合
    'NOTE: oXllReqTelegは、RegisterActiveDllに渡したものからParseAsSameKindで生成したものである。
    Protected Overridable Sub ProcOnActiveDllComplete(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '能動的DLLが成功した（ContinueCode.FinishWithoutStoringの転送終了REQ電文を受信した）場合
    'NOTE: oXllReqTelegは、RegisterActiveDllに渡したものからParseAsSameKindで生成したものである。
    Protected Overridable Sub ProcOnActiveDllCompleteWithoutStoring(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '能動的DLLにてクライアントから転送失敗または転送化けを通知された（ContinueCode.Abortの転送終了REQ電文を受信した）場合
    'NOTE: oXllReqTelegは、RegisterActiveDllに渡したものからParseAsSameKindで生成したものである。
    Protected Overridable Sub ProcOnActiveDllAbort(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '能動的DLLにて転送終了REQ電文待ちのタイムアウトが発生した場合
    'NOTE: oXllReqTelegは、RegisterActiveDllに渡したものである。
    'NOTE: プロトコル上、この時点でクライアントがファイルの取得を継続していることは正当な
    'ことであり、クライアントは、ハッシュ値のチェックで異常が検出できなければ、取得した
    'ファイルをそのまま保存する（配信する）はずである。よって、下記のメソッドではDLL対象となる
    'FTPサーバ上のファイルを削除するべきではない。削除するのであれば、当該クライアントから
    '新たな接続要求があったときに削除するのが、望ましい。
    Protected Overridable Sub ProcOnActiveDllTimeout(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '能動的DLLの開始で異常とみなすべきでないリトライオーバーが発生した場合
    'NOTE: oXllReqTelegは、RegisterActiveDllに渡したものである。
    'NOTE: oNakTelegは、oXllReqTeleg.ParseAsNak(oRcvTeleg)で生成したオブジェクトである。
    'oRcvTelegは、リトライオーバーの判定に至った際に受信した電文である。
    Protected Overridable Sub ProcOnActiveDllRetryOverToForget(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '能動的DLLの開始で異常とみなすべきリトライオーバーが発生した場合
    'NOTE: oXllReqTelegは、RegisterActiveDllに渡したものである。
    'NOTE: oNakTelegは、oXllReqTeleg.ParseAsNak(oRcvTeleg)で生成したオブジェクトまたはNothingである。
    'oRcvTelegは、リトライオーバーの判定に至った際に受信した電文である。
    'oNakTelegがNothingになるのは、REQ電文を送信するまでもなく諦めた場合であり、
    'EnableActiveSeqStrongExclusionまたはEnableXllStrongExclusionがTrueの場合にのみあり得る。
    Protected Overridable Sub ProcOnActiveDllRetryOverToCare(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '能動的DLLの最中やキューイングされた能動的DLLの開始前に通信異常を検出した場合
    'NOTE: oXllReqTelegは、RegisterActiveDllに渡したものである。
    'NOTE: プロトコル上、この時点でクライアントがファイルの取得を継続していることは正当な
    'ことであり、クライアントは、ハッシュ値のチェックで異常が検出できなければ、取得した
    'ファイルをそのまま保存する（配信する）はずである。よって、下記のメソッドではDLL対象となる
    'FTPサーバ上のファイルを削除するべきではない。削除するのであれば、当該クライアントから
    '新たな接続要求があったときに削除するのが、望ましい。
    Protected Overridable Sub ProcOnActiveDllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '能動的ULLが成功した（受信済みのハッシュ値と受信完了したファイルの内容が整合していることを確認した）
    '（転送終了REQ電文に対しACK電文を返信することになる）場合
    'NOTE: oXllReqTelegは、RegisterActiveUllに渡したものからParseAsSameKindで生成したものである。
    'NOTE: 受け入れ不可能な内容であればNakCauseCode.InvalidContentなどを返却すること。
    Protected Overridable Function ProcOnActiveUllComplete(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '能動的ULLにて転送化けを検出した（受信済みのハッシュ値と受信完了したファイルの内容に不整合を検出した）
    '（転送終了REQ電文に対しハッシュ値の不一致を示すNAK電文を返信することになる）場合
    'NOTE: oXllReqTelegは、RegisterActiveUllに渡したものからParseAsSameKindで生成したものである。
    'NOTE: プロトコルに応じて、NakCauseCode.TelegramErrorや専用のNAKを生み出すためのNakCauseCodeを返却すること。
    Protected Overridable Function ProcOnActiveUllHashValueError(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '能動的ULLにてクライアントから転送失敗を通知された（ContinueCode.Abortの転送終了REQ電文を受信した）場合
    'NOTE: oXllReqTelegは、RegisterActiveUllに渡したものからParseAsSameKindで生成したものである。
    Protected Overridable Sub ProcOnActiveUllAbort(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '能動的ULLの開始で異常とみなすべきでないリトライオーバーが発生した場合
    'NOTE: oXllReqTelegは、RegisterActiveUllに渡したものである。
    'NOTE: oNakTelegは、oXllReqTeleg.ParseAsNak(oRcvTeleg)で生成したオブジェクトである。
    'oRcvTelegは、リトライオーバーの判定に至った際に受信した電文である。
    Protected Overridable Sub ProcOnActiveUllRetryOverToForget(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '能動的ULLの開始で異常とみなすべきリトライオーバーが発生した場合
    'NOTE: oXllReqTelegは、RegisterActiveUllに渡したものである。
    'NOTE: oNakTelegは、oXllReqTeleg.ParseAsNak(oRcvTeleg)で生成したオブジェクトまたはNothingである。
    'oRcvTelegは、リトライオーバーの判定に至った際に受信した電文である。
    'oNakTelegがNothingになるのは、REQ電文を送信するまでもなく諦めた場合であり、
    'EnableActiveSeqStrongExclusionまたはEnableXllStrongExclusionがTrueの場合にのみあり得る。
    Protected Overridable Sub ProcOnActiveUllRetryOverToCare(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
    End Sub

    '能動的ULLにて転送終了REQ電文待ちのタイムアウトが発生した場合
    'NOTE: oXllReqTelegは、RegisterActiveUllに渡したものである。
    Protected Overridable Sub ProcOnActiveUllTimeout(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '能動的ULLの最中やキューイングされた能動的ULLの開始前に通信異常を検出した場合
    'NOTE: oXllReqTelegは、RegisterActiveUllに渡したものである。
    Protected Overridable Sub ProcOnActiveUllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    'ヘッダ部の内容が受動的DLLのREQ電文のものであるか判定するメソッド
    'NOTE: oTelegは、このクラスのNewに渡したTelegramImporterが生成したものである。
    'NOTE: コマンド種別がREQであることは確定している。
    Protected Overridable Function IsPassiveDllReq(ByVal oTeleg As ITelegram) As Boolean
        Return False
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    'NOTE: oTelegは、このクラスのNewに渡したTelegramImporterが生成したものである。
    'NOTE: ヘッダ部の内容が受動的DLLのREQ電文のものであることは確定している。
    'NOTE: 以降の受動的DLL用メソッドに渡される電文インスタンスは、このメソッドで生成したもの
    'または、そのインスタンスのParseAsSameKindメソッドで生成したものである。
    'NOTE: GetBodyFormatViolation()の実行は、呼び出し後に行うので不要である。
    Protected Overridable Function ParseAsPassiveDllReq(ByVal oTeleg As ITelegram) As IXllReqTelegram
        Return Nothing
    End Function

    '受動的DLLの準備（指定されたファイルの用意）を行うメソッド
    'NOTE: 用意ができなければNakCauseCode.NoDataを返却すること。
    Protected Overridable Function PrepareToStartPassiveDll(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '受動的DLLが成功した（ContinueCode.Finishの転送終了REQ電文を受信した）場合
    Protected Overridable Sub ProcOnPassiveDllComplete(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '受動的DLLが成功した（ContinueCode.FinishWithoutStoringの転送終了REQ電文を受信した）場合
    Protected Overridable Sub ProcOnPassiveDllCompleteWithoutStoring(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '受動的DLLにてクライアントから転送失敗または転送化けを通知された（ContinueCode.Abortの転送終了REQ電文を受信した）場合
    Protected Overridable Sub ProcOnPassiveDllAbort(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '受動的DLLにて転送終了REQ電文待ちのタイムアウトが発生した場合
    Protected Overridable Sub ProcOnPassiveDllTimeout(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '受動的DLLの最中やキューイングされた受動的DLLの開始前に通信異常を検出した場合
    Protected Overridable Sub ProcOnPassiveDllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    'ヘッダ部の内容が受動的ULLのREQ電文のものであるか判定するメソッド
    'NOTE: oTelegは、このクラスのNewに渡したTelegramImporterが生成したものである。
    'NOTE: コマンド種別がREQであることは確定している。
    Protected Overridable Function IsPassiveUllReq(ByVal oTeleg As ITelegram) As Boolean
        Return False
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    'NOTE: oTelegは、このクラスのNewに渡したTelegramImporterが生成したものである。
    'NOTE: ヘッダ部の内容が受動的ULLのREQ電文のものであることは確定している。
    'NOTE: 以降の受動的ULL用メソッドに渡される電文インスタンスは、このメソッドで生成したもの
    'または、そのインスタンスのParseAsSameKindメソッドで生成したものである。
    'NOTE: GetBodyFormatViolation()の実行は、呼び出し後に行うので不要である。
    Protected Overridable Function ParseAsPassiveUllReq(ByVal oTeleg As ITelegram) As IXllReqTelegram
        Return Nothing
    End Function

    '受動的ULLの準備（予告されたファイルの受け入れ確認）を行うメソッド
    'NOTE: 受け入れ不可能であればNakCauseCode.BusyやNakCauseCode.InvalidContentを返却すること。
    Protected Overridable Function PrepareToStartPassiveUll(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '受動的ULLが成功した（受信済みのハッシュ値と受信完了したファイルの内容が整合していることを確認した）
    '（転送終了REQ電文に対しACK電文を返信することになる）場合
    'NOTE: 受け入れ不可能な内容であればNakCauseCode.InvalidContentなどを返却すること。
    Protected Overridable Function ProcOnPassiveUllComplete(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '受動的ULLにて転送化けを検出した（受信済みのハッシュ値と受信完了したファイルの内容に不整合を検出した）
    '（転送終了REQ電文に対しハッシュ値の不一致を示すNAK電文を返信することになる）場合
    'NOTE: プロトコルに応じて、NakCauseCode.TelegramErrorや専用のNAKを生み出すためのNakCauseCodeを返却すること。
    Protected Overridable Function ProcOnPassiveUllHashValueError(ByVal oXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Return NakCauseCode.None
    End Function

    '受動的ULLにてクライアントから転送失敗を通知された（ContinueCode.Abortの転送終了REQ電文を受信した）場合
    Protected Overridable Sub ProcOnPassiveUllAbort(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '受動的ULLにて転送終了REQ電文待ちのタイムアウトが発生した場合
    Protected Overridable Sub ProcOnPassiveUllTimeout(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '受動的ULLの最中やキューイングされた受動的ULLの開始前に通信異常を検出した場合
    Protected Overridable Sub ProcOnPassiveUllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
    End Sub

    '親スレッドからコネクションを受け取った場合（通信状態の変化をフックするためのメソッド）
    Protected Overridable Sub ProcOnConnectionAppear()
    End Sub

    'REQ電文受信及びそれに対するACK電文送信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    'NOTE: Falseを返すようにすれば、コネクションが切断される。
    Protected Overridable Function ProcOnReqTelegramReceiveCompleteBySendAck(ByVal oRcvTeleg As ITelegram, ByVal oSndTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ電文受信及びそれに対する軽度NAK電文（BUSY等）送信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    'NOTE: Falseを返すようにすれば、コネクションが切断される。
    Protected Overridable Function ProcOnReqTelegramReceiveCompleteBySendNak(ByVal oRcvTeleg As ITelegram, ByVal oSndTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ電文送信及びそれに対するACK電文受信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    'NOTE: Falseを返すようにすれば、コネクションが切断される。
    Protected Overridable Function ProcOnReqTelegramSendCompleteByReceiveAck(ByVal oSndTeleg As ITelegram, ByVal oRcvTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ電文送信及びそれに対する軽度NAK電文（BUSY等）受信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    'NOTE: Falseを返すようにすれば、コネクションが切断される。
    Protected Overridable Function ProcOnReqTelegramSendCompleteByReceiveNak(ByVal oSndTeleg As ITelegram, ByVal oRcvTeleg As ITelegram) As Boolean
        Return True
    End Function

    'コネクションを切断した場合（通信状態の変化をフックするためのメソッド）
    Protected Overridable Sub ProcOnConnectionDisappear()
    End Sub

    Protected Overridable Function IsParentMessageReceptibleState(ByVal state As State) As Boolean
        Return True
    End Function

    Protected Overridable Function IsParentMessageReceptibleState(ByVal activeXllState As ActiveXllState) As Boolean
        Return True
    End Function

    Protected Overridable Function IsParentMessageReceptibleState(ByVal passiveXllState As PassiveXllState) As Boolean
        Return True
    End Function

    'NAK電文を送信する場合や受信した場合のその後の挙動を決めるためのメソッド
    'NOTE: NAK電文のデータ種別やNAK電文の事由によって決めることを想定している。
    Protected Overridable Function GetRequirement(ByVal oNakTeleg As INakTelegram) As NakRequirement
        If oNakTeleg.CauseCode = NakCauseCode.Busy Then
            Return NakRequirement.CareOnRetryOver
        Else
            Return NakRequirement.DisconnectImmediately
        End If
    End Function
#End Region

End Class
