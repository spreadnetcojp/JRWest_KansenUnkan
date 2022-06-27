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

''' <summary>
''' Type5～8のシーケンスでServerTelegrapherやClientTelegrapherが
''' 想定する仮想REQ電文。
''' </summary>
Public Interface IXllReqTelegram
    Inherits IReqTelegram, IXllTelegram

    '転送対象ファイルパス一覧のローカル側ベースディレクトリ
    'NOTE: 電文そのものの要素ではない。
    ReadOnly Property TransferListBase() As String

    '転送対象ファイルパス一覧
    ReadOnly Property TransferList() As List(Of String)

    'ファイル転送の期限
    'NOTE: プロトコルによって、実際の電文内にこれに相当する項目は存在しないはずである。
    'その場合は、アプリ側においてインスタンス化する際の引数で設定する。
    ReadOnly Property TransferLimitTicks() As Integer

    'HashValue部が設定済みか否か
    'NOTE: サーバからの要求で開始するDLLシーケンスのREQ電文または
    'クライアントからの要求で開始するULLシーケンスのREQ電文である場合のみ、
    'Telegrapherはこのプロパティを参照する。よって、それ以外の電文の場合は、
    '適当な固定値を返すように実装してもよい。
    'NOTE: HashValue部が文字列のように冗長な書式のプロトコルの場合は、
    'HashValue未指定で生成した時点で、ヌル文字のようにハッシュ値と
    'みなせない値を格納しておき、それを判定するとよい。
    'HashValue部の書式に冗長性がないプロトコルの場合は、電文バイト列とは
    '別にBoolean型メンバ（_HasHashValue）を用意し、HashValue未指定で生成
    'した時点ではそれをFalseとし、ImportFileDependentValueFromFoo()や
    'UpdateHashValue()でTrueにするとよい。
    ReadOnly Property IsHashValueReady() As Boolean

    'HashValue部の値とファイルの内容が整合しているか
    'NOTE: サーバからの要求で開始するULLシーケンスのREQ電文である場合、
    '実際の電文にHashValue部はないはずである。
    'ハッシュ値のチェックを行うプロトコルであるならば、その場合も、
    'ImportFileDependentValueFromFoo()やUpdateHashValue()で変更される
    'メンバ変数をファイルから算出したハッシュ値と比較すること。
    'ハッシュ値のチェックを行わないプロトコルであるならば、無条件で
    'Trueを返してよい。
    ReadOnly Property IsHashValueIndicatingOkay() As Boolean

    'ACK電文を生成するメソッド
    'NOTE: ACK電文にHashValue部が存在する場合即ち、
    'サーバからの要求で開始するULLシーケンスの場合または、
    'クライアントからの要求で開始するDLLシーケンスの場合または、
    'クライアントからの要求で開始するULLシーケンスの場合は、
    'Meに格納されているハッシュ値を生成するACK電文にコピーする。
    'なお、サーバからの要求で開始するULLシーケンスの場合、
    '実際のREQ電文にHashValue部はないはずであるが、
    'ハッシュ値のチェックを行うプロトコルであるならば、
    'その場合も、ImportFileDependentValueFromAck()などで
    '変更されるメンバ変数の値をコピーすること。
    Function CreateAckTelegram() As IXllTelegram

    '渡された電文の型をACK電文の型に変換するメソッド
    'NOTE: 電文書式が同一であることは、呼び元が保証する。
    'NOTE: 呼び元は、Meだけでなく、引数で渡す電文についても、
    'ヘッダ部に書式違反が無いことを確認済みでなければならない。
    'NOTE: 変換後のオブジェクトに対するGetBodyFormatViolation()の実行も、呼び元の責務である。
    Shadows Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As IXllTelegram

    '渡された電文の型を同一型に変換するメソッド
    'NOTE: 電文書式が同一であることは、呼び元が保証する。
    'NOTE: 呼び元は、Meだけでなく、引数で渡す電文についても、
    'ヘッダ部に書式違反が無いことを確認済みでなければならない。
    'NOTE: 変換後のオブジェクトに対するGetBodyFormatViolation()の実行も、呼び元の責務である。
    'NOTE: プロトコル上で（送受信バイト列内に）ReplyLimitTicks相当の情報が存在しない
    '電文フォーマットの場合、ReplyLimitTicksはREQ電文を送信する側のみが参照するもので
    'ある故、適当な値（0など）が自動で設定されればよい。
    'NOTE: プロトコル上で（送受信バイト列内に）TransferLimitTicks相当の情報が存在しない
    '電文フォーマットの場合、TransferLimitTicksにはMeと同じ値が設定されるものとする。
    Function ParseAsSameKind(ByVal oNextTeleg As ITelegram) As IXllReqTelegram

    '渡された同一型電文のObjDetail部が同一のファイル転送を示しているか判定するメソッド
    'NOTE: このメソッドは、2つのXllReq電文が単一シーケンスのものであるか
    '判定するために（サーバ側で）使用される。
    'NOTE: 電文書式が同一であることは、呼び元が保証する。
    'NOTE: 電文インスタンスの型が同一であることも、呼び元が保証する。
    'NOTE: 呼び元は、Meだけでなく、引数で渡す電文についても、
    'ヘッダ部及びボディ部に書式違反が無いことを確認済みでなければならない。
    Function IsContinuousWith(ByVal oXllReqTeleg As IXllReqTelegram) As Boolean

    'ACK電文からハッシュ値やファイル転送期限を取り込むメソッド
    'NOTE: 電文書式が同一であることは、呼び元が保証する。
    'NOTE: 電文インスタンスの型がMeのACK電文の型であることも、呼び元が保証する。
    'NOTE: 呼び元は、Meだけでなく、引数で渡す電文についても、
    'ヘッダ部に書式違反が無いことを確認済みでなければならない。
    'NOTE: Meがサーバからの要求で開始するULLシーケンスのREQ電文の場合または
    'クライアントからの要求で開始するDLLシーケンスのREQ電文の場合のみ
    '呼び出される。前者のような電文において、ハッシュ値を格納できる項目は
    'ないはずであるが、その場合は、実際の電文内容を保持する変数とは
    '別のメンバ変数にハッシュ値を格納する。
    '上記に当てはまらない電文においては、ACK電文にハッシュ値が存在しない
    '場合もあると思われるが、このメソッド自体、呼び出されることがないため、
    '何もしないメソッドにしてよい。
    'NOTE: 電文上にファイル転送期限に相当する項目がないプロトコルの場合、
    'Meの構築時にファイル転送期限を設定済みであるため、このメソッドにおける
    'ファイル転送期限の取り込みは不要である。
    Sub ImportFileDependentValueFromAck(ByVal oReplyTeleg As IXllTelegram)

    '同一型電文からハッシュ値やファイル転送期限を取り込むメソッド
    'NOTE: 電文書式が同一であることは、呼び元が保証する。
    'NOTE: 電文インスタンスの型が同一であることも、呼び元が保証する。
    'NOTE: 呼び元は、Meだけでなく、引数で渡す電文についても、
    'ヘッダ部に書式違反が無いことを確認済みでなければならない。
    'NOTE: Meがサーバからの要求で開始するULLシーケンスのREQ電文の場合または
    'クライアントからの要求で開始するULLシーケンスのREQ電文の場合のみ
    '呼び出される。前者のような電文において、ハッシュ値を格納できる項目は
    'ないはずであるが、その場合は、実際の電文内容を保持する変数とは
    '別のメンバ変数にハッシュ値を格納できるようにしておき、それをコピーする。
    Sub ImportFileDependentValueFromSameKind(ByVal oPreviousTeleg As IXllReqTelegram)

    'HashValue部に値を設定するメソッド
    'NOTE: サーバからの要求で開始するULLシーケンスのREQ電文である場合、
    '実際の電文にHashValue部はないはずである。
    'そのようなシーケンスの電文である場合も、ハッシュ値のチェックを行う
    'プロトコルであるならば、実際の電文内容を保持する変数とは別の
    'メンバ変数を用意しておき、そこにハッシュ値を格納しなければならない。
    Sub UpdateHashValue()
End Interface
