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

Imports System.IO
Imports System.Net.Sockets

''' <summary>
''' ServerTelegrapherやClientTelegrapherが想定する仮想電文。
''' </summary>
Public Interface ITelegram
    'コマンド種別（仮想）
    ReadOnly Property CmdKind() As CmdKind

    'NAK電文を生成するメソッド
    'NOTE: TelegrapherはこのメソッドがNothingを返却する可能性も想定する。
    'Nothingが返却された場合は、causeCodeが何であろうと、
    'コネクションを切断することになる。
    Function CreateNakTelegram(ByVal causeCode As NakCauseCode) As INakTelegram

    'ヘッダ部の書式違反をチェックするメソッド
    'NOTE: 内容不明電文にアクセスする際は、ヘッダ部のプロパティを
    '読み書きするだけであっても、このメソッドを事前を実施し、
    '書式違反が無いことを確認しておかなければならない。
    '書式違反がある場合に呼び出し可能なメソッドやプロパティは、
    'CmdKindとCreateNakTelegramのみである。
    'NOTE: プロパティの取得に必須でないチェックもこれに実装するが、
    '電文単体の仕様において認められている値は全て許容する。
    'つまり、状況に依存した値のチェックは呼び元の責務である。
    Function GetHeaderFormatViolation() As NakCauseCode

    'ボディ部の書式違反をチェックするメソッド
    'NOTE: 内容不明電文をもとに生成した各種電文インスタンスに対し、
    '固有のプロパティを読み書きしたり、固有のメソッドを呼び出したり
    'する際（ボディ部にアクセスする際）は、このメソッドを事前に実施し、
    '書式違反が無いことを確認しておかなければならない。
    'NOTE: プロパティの取得に必須でないチェックもこれに実装するが、
    '電文単体の仕様において認められている値は全て許容する。
    'つまり、状況に依存した値のチェックは呼び元の責務である。
    Function GetBodyFormatViolation() As NakCauseCode

    '渡された電文の種類が同じであるか判定するメソッド
    'NOTE: 電文書式が同一であることは、呼び元が保証する。
    'NOTE: 呼び元は、Meだけでなく、引数で渡す電文についても、
    'ヘッダ部に書式違反が無いことを確認済みでなければならない。
    Function IsSameKindWith(ByVal oTeleg As ITelegram) As Boolean

    'ソケットへの出力メソッド
    'NOTE: timeoutBaseTicksに0または-1を指定すると無期限待機となる。
    'NOTE: 外部要因の可能性があるSocketExceptionが発生した場合など、
    'コネクション終了に持ち込むべきである（プログラムの異常と扱うべきでない）
    'ケースでは、発生事象を内部で記録し、Falseで戻る。
    Function WriteToSocket( _
       ByVal oSocket As Socket, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As Boolean
End Interface

'コマンド種別（仮想）
Public Enum CmdKind As Integer
    None
    Req
    Ack
    Nak
End Enum
