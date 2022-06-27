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

''' <summary>
''' インタフェース仕様で定められた値を格納するクラス。
''' </summary>
Public Class ExConstants

    '窓処用マスタ種別に対応する電文サブ種別
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
    'TODO: 現状、北陸新幹線の駅にJRW窓口処理機が存在しないため、
    'FSK～SWKの定義は削除しているが、必要であれ追加すること。
    Public Shared ReadOnly MadoMastersSubObjCodes As New Dictionary(Of String, Byte) From { _
       {"DSH", &H47}, _
       {"LST", &H4D}, _
       {"FJW", &H3E}, _
       {"IJW", &H43}, _
       {"FJC", &H4E}, _
       {"IJC", &H4F}, _
       {"FJR", &H50}, _
       {"IJE", &H56}, _
       {"ICD", &H55}, _
       {"DLY", &H41}, _
       {"ICH", &H44}, _
       {"CYC", &H64}, _
       {"STP", &H63}, _
       {"PNO", &H62}, _
       {"FRC", &H61}, _
       {"DUS", &H66}, _
       {"NSI", &H70}, _
       {"NTO", &H71}, _
       {"NIC", &H72}, _
       {"NJW", &H73}}

    'Suicaエリアの窓処が受信できるマスタ
    'TODO: 受信できるか否かよくわかっていないもの（DLYやCYCなど）は、とりあえず
    '入れてあるが、本物の窓処に合わせるべきである。立合試験などにおいて、
    'Suicaエリアの改札機にはCYCがDLLされなかった（DL完了通知が返ってこなかった）
    'ような記憶もある。
    Private Shared ReadOnly MadoMastersInSuicaArea As New HashSet(Of String) From { _
       "DSH", _
       "LST", _
       "FJR", _
       "IJE", _
       "ICD", _
       "DLY", _
       "CYC", _
       "NSI"}

    '東京駅幹幹口の窓処が受信できるマスタ
    'TODO: よく分からないので、とりあえずSuicaエリアと同等にしてあるが、
    '本物の窓処に合わせるべきである。
    Private Shared ReadOnly MadoMastersInTokyoKanKanArea As New HashSet(Of String) From { _
       "DSH", _
       "LST", _
       "FJR", _
       "IJE", _
       "ICD", _
       "DLY", _
       "CYC", _
       "NSI"}

    'TOICAエリアの窓処が受信できるマスタ
    'TODO: とりあえず「東海道山陽向け 新幹線自動改札システム システム仕様書」に
    '合わせてあるが、立会試験をしたとき、このエリアには東か西のマスタも
    '配信していたかもしれない。
    'TODO: 受信できるか否かよくわかっていないもの（DLYなど）は、とりあえず
    '入れてあるが、本物の窓処に合わせるべきである。
    Private Shared ReadOnly MadoMastersInToicaArea As New HashSet(Of String) From { _
       "DSH", _
       "LST", _
       "FJC", _
       "IJC", _
       "ICD", _
       "DLY", _
       "ICH", _
       "CYC", _
       "NTO"}

    'ICOCAエリアの窓処が受信できるマスタ
    'TODO: 受信できるか否かよくわかっていないもの（DLYなど）は、とりあえず
    '入れてあるが、本物の窓処に合わせるべきである。
    '窓処のICOCAエリアはJR東海管轄ICOCAエリアとJR西日本管轄ICOCAエリアに
    '分かれていないため、ICOCAエリアであればNICもNJWもDLLする想定にしてある。
    'もし、本物の窓処がJR東海向けとJR西日本向けで異なる動作をする（一方の
    '駅名データしかDLLしないように作り込まれている）なら、
    'シミュレータも事業者別に実装しなければならない。
    Private Shared ReadOnly MadoMastersInIcocaArea As New HashSet(Of String) From { _
       "DSH", _
       "LST", _
       "FJW", _
       "IJW", _
       "ICD", _
       "DLY", _
       "CYC", _
       "NIC", _
       "NJW"}

    'SUGOCAエリアの窓処が受信できるマスタ
    'TODO: 全く未知のエリアなので、とりあえずICOCAエリアからJR東海専用と思われる
    'DLYとNICを抜いた構成にしてある。もし、JR東海管轄ICOCAエリアとJR西日本管轄ICOCAエリア
    'の窓処が共通化されているのと同様に、SUGOCAエリアの窓処もICOCAエリアと共通化
    'されているのであれば、DLYやNICも入れた方がよい（どのみちJRW運管から配信不可能に
    'なっているが）。なお、共通化されているという根拠は「マスタデータ仕様書25H」の
    'マスタ定義一覧である（JR西日本の窓処がICOCAエリアとSUGOCAエリアに分けられていない）。
    'ただし、JR西日本の窓処がJR東海の西日本エリア（ICOCAエリア？）と区別されていたり、
    'DLYがどのエリアのどの端末にも中継機からDLLされないことになっていたり、
    '何が正しいのか考えさせられる表になっている）。
    Private Shared ReadOnly MadoMastersInSugocaArea As New HashSet(Of String) From { _
       "DSH", _
       "LST", _
       "FJW", _
       "IJW", _
       "ICD", _
       "CYC", _
       "NJW"}

    Public Shared ReadOnly MadoAreasMasters As New Dictionary(Of Integer, HashSet(Of String)) From { _
       {1, MadoMastersInSuicaArea}, _
       {3, MadoMastersInToicaArea}, _
       {2, MadoMastersInIcocaArea}, _
       {6, MadoMastersInSugocaArea}, _
       {7, MadoMastersInTokyoKanKanArea}}

    Public Const MadoProgramVersionListPathInCab As String = "\Mversion.dat"

End Class
