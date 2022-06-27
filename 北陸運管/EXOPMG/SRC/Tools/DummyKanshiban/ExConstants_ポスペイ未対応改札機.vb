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

''' <summary>
''' インタフェース仕様で定められた値を格納するクラス。
''' </summary>
Public Class ExConstants

    '改札機用マスタ種別に対応する電文サブ種別
    'NOTE: この辞書はマスタ種別と電文サブ種別の対応関係を表すだけでなく、
    '中継機が受け付けるべきマスタ種別も表している。
    'TODO: 現状、いずれかのエリアの端末にDLLできる必要があるマスタは、
    '全てこの辞書に定義することにしているが、弊害として、DLL可能な端末と
    '無関係な中継機においても、そのマスタ種別を受け付けることになって
    'しまう。たとえば、「マスタデータ仕様書25H」のマスタ定義一覧によると、
    'Suicaエリアの中継機はIJCを受け付けないと推測されるため、これは
    'いただけない（推測の根拠は当該セルの値が「△」ではなく「－」になって
    'いることである。「－」の意味が記載されていないため定かではないが、
    '消去法により「中継機自身が受け付けない」ことを表しているのではないか
    'と推測される）。もし、この仕様書が信頼できるものであり、この推測も
    '合っているなら、この辞書もエリア別に用意するべきである。
    'ただし、たとえば統括管理サーバが複数のエリアの窓処にDLLを行えるように
    'なっていることからわかるように、本質的にひとつの中継機がひとつの辞書
    'だけをみればよいわけではないことに注意が必要である。
    'ここで言うエリアは、中継機向け部材のエリアのことではなく、あくまでも
    '端末向け部材のエリアのことである。
    Public Shared ReadOnly GateMastersSubObjCodes As New Dictionary(Of String, Byte) From { _
       {"DSH", &H47}, _
       {"LOS", &H48}, _
       {"DSC", &H49}, _
       {"HLD", &H4A}, _
       {"EXP", &H4B}, _
       {"FRX", &H4C}, _
       {"LST", &H4D}, _
       {"FJW", &H3E}, _
       {"IJW", &H43}, _
       {"FJC", &H4E}, _
       {"IJC", &H4F}, _
       {"FJR", &H50}, _
       {"IJE", &H56}, _
       {"KEN", &H59}, _
       {"DLY", &H41}, _
       {"ICH", &H44}, _
       {"PAY", &H42}, _
       {"CYC", &H64}, _
       {"STP", &H63}, _
       {"PNO", &H62}, _
       {"FRC", &H61}, _
       {"DUS", &H66}, _
       {"NSI", &H70}, _
       {"NTO", &H71}, _
       {"NIC", &H72}, _
       {"NJW", &H73}, _
       {"IUK", &H86}, _
       {"IUZ", &H84}, _
       {"KSZ", &H85}, _
       {"SWK", &H87}, _
       {"FSK", &H80}, _
       {"HIR", &H8A}, _
       {"PPA", &H89}}

    '東海道Suicaエリアの改札機が受信できるマスタ
    'TODO: 受信できるか否かよくわかっていないもの（DLY, KEN, CYCなど）は、とりあえず
    '入れてあるが、本物の改札機システムに合わせるべきである。適用済み対応前の立合試験などにおいて、
    'Suicaエリアの改札機にはCYCがDLLされなかった（DL完了通知が返ってこなかった）ような記憶もある。
    Private Shared ReadOnly GateReadyGateMastersInTokaidoSuicaArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJR", _
       "IJE", _
       "KEN", _
       "DLY", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NSI"}

    '東海道TOICAエリアの改札機が受信できるマスタ
    'TODO: とりあえず「東海道山陽向け 新幹線自動改札システム システム仕様書」に
    '合わせてあるが、立会試験をしたとき、このエリアには東か西のマスタも
    '配信していたかもしれない。
    'TODO: 受信できるか否かよくわかっていないもの（DLY, KENなど）は、とりあえず
    '入れてあるが、本物の改札機システムに合わせるべきである。
    Private Shared ReadOnly GateReadyGateMastersInTokaidoToicaArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJC", _
       "IJC", _
       "KEN", _
       "DLY", _
       "ICH", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NTO"}

    '東海道ICOCAエリアの改札機が受信できるマスタ
    'TODO: 受信できるか否かよくわかっていないもの（DLYなど）は、とりあえず
    '入れてあるが、本物の改札機システムに合わせるべきである。
    Private Shared ReadOnly GateReadyGateMastersInTokaidoIcocaArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "KEN", _
       "DLY", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NIC"}

    '山陽ICOCAエリアの改札機が受信できるマスタ
    'TODO: 受信できるか否かよくわかっていないもの（PAYなど）は、とりあえず
    '入れてあるが、本物の改札機システムに合わせるべきである。
    Private Shared ReadOnly GateReadyGateMastersInSanyoIcocaArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "KEN", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NJW"}

    '山陽SUGOCAエリアの改札機が受信できるマスタ
    'TODO: よく分からないが、とりあえず山陽ICOCAエリアと同じにしてある。
    '専用のマスタ種別があるなら、GateMastersSubObjCodesとともに修正しなければならない。
    Private Shared ReadOnly GateReadyGateMastersInSanyoSugocaArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "KEN", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NJW"}

    '東京駅幹幹口の改札機が受信できるマスタ
    'TODO: よく分からない。とりあえず東海道Suicaエリアと同じにしてあるが、
    '本物の改札機システムに合わせるべきである。
    Private Shared ReadOnly GateReadyGateMastersInTokyoKanKanArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJR", _
       "IJE", _
       "KEN", _
       "DLY", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NSI"}

    '北陸エリアの改札機が受信できるマスタ
    'TODO: とりあえず山陽ICOCAエリアの種別+北陸専用種別-ポイントポスペ関連種別にしてあるが、
    '本物の改札機システムに合わせるべきである。
    Private Shared ReadOnly GateReadyGateMastersInHokurikuArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "KEN", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NJW", _
       "IUK", _
       "IUZ", _
       "KSZ", _
       "SWK", _
       "FSK"}

    '東海道山陽線区共通の監視盤が受け付ける改札機マスタ
    'NOTE: GateMastersSubObjCodes に存在していて、ここに存在しない
    '改札機マスタのDLLシーケンスについて、東海道山陽線区共通の監視盤は、
    'NAKを返信したりせずに最後まで実行するが、ファイル転送終了後に
    '送信するREQ電文の開始・終了コードを0x03（FinishWithoutStoring）とする。
    'TODO: とりあえず、北陸専用以外の全マスタを入れているが、よくわからない。
    '北陸専用のマスタについては、運管側で東海道山陽線区共通の監視盤が、未だに
    'それを知らないのであれば、NAKを返信する可能性もある。
    '逆に、監視盤ではノーガードであり、DLLシーケンスの完了後、
    '改札機が監視盤から受信しないことで、監視盤が「適用済み」のDL完了通知を
    '作成する可能性もある。
    Private Shared ReadOnly KsbReadyGateMastersInTokaidoSanyo As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "FJC", _
       "IJC", _
       "FJR", _
       "IJE", _
       "KEN", _
       "DLY", _
       "ICH", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NSI", _
       "NTO", _
       "NIC", _
       "NJW"}

    '北陸向けの監視盤が受け付ける改札機マスタ
    'NOTE: GateMastersSubObjCodes に存在していて、ここに存在しない
    '改札機マスタのDLLシーケンスについて、北陸向けの監視盤は、
    'NAKを返信したりせずに最後まで実行するが、ファイル転送終了後に
    '送信するREQ電文の開始・終了コードを0x03（FinishWithoutStoring）とする。
    'TODO: とりあえず、北陸エリアの改札機が受信するマスタと（接続試験時に
    'なぜか受け付けるようになっていた）ポイントポストペイ用マスタを
    '入れているが、このまま現地リリースするのかはよくわからない。
    Private Shared ReadOnly KsbReadyGateMastersInHokuriku As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "KEN", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NJW", _
       "IUK", _
       "IUZ", _
       "KSZ", _
       "SWK", _
       "FSK"}

    Public Shared ReadOnly GateAreasSpecs As New Dictionary(Of Integer, ExAreaSpec) From { _
       {1, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInTokaidoSuicaArea)}, _
       {3, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInTokaidoToicaArea)}, _
       {2, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInTokaidoIcocaArea)}, _
       {4, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInSanyoIcocaArea)}, _
       {6, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInSanyoSugocaArea)}, _
       {7, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInTokyoKanKanArea)}, _
       {8, New ExAreaSpec(KsbReadyGateMastersInHokuriku, GateReadyGateMastersInHokurikuArea)}}

    Public Const GateProgramVersionListPathInCab As String = "\KANSI\N_GATE\JPROWRK\Gversion.dat"
    Public Const GateProgramModuleBasePathInCab As String = "\KANSI\N_GATE"
    Public Const GateProgramModuleCatalogFileNameInCab As String = "FILELIST.TXT"
    Public Shared ReadOnly GateProgramModuleNamesInCab As String() = {"JHANWRK", "JPROWRK", "JSCPUWRK", "JOSWRK", "JICUWRK"}
    Public Shared ReadOnly GateProgramModuleNamesInVersionInfo As String() = {"JHANNOW", "JPRONOW", "JSCPUNOW", "JOSNOW", "JICUNOW"}

    Public Const KsbProgramVersionListPathInCab As String = "\KANSI_PROG\WRK\Kversion.dat"

End Class

Public Class ExAreaSpec

    Public KsbReadyGateMasters As HashSet(Of String)
    Public GateReadyGateMasters As HashSet(Of String)
    Public Sub New(ByVal oKsbReadyGateMasters As HashSet(Of String), ByVal oGateReadyGateMasters As HashSet(Of String))
        KsbReadyGateMasters = oKsbReadyGateMasters
        GateReadyGateMasters = oGateReadyGateMasters
    End Sub

End Class
