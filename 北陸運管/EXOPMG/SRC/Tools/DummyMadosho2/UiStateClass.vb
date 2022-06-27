' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/06/27  (NES)小林  新規作成
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

    Public Sub New()
        Me.TermMachines = New Dictionary(Of String, TermMachine)
    End Sub
End Class

<DataContract> Public Class TermMachine
    '機器構成ファイルのキャッシュ
    <DataMember> Public Profile As Object()

    '各種状態
    <DataMember> Public LatchConf As Byte
    <DataMember> Public SeqNumber As UInteger
    <DataMember> Public PassDate As DateTime

    Public Sub New()
    End Sub
End Class
