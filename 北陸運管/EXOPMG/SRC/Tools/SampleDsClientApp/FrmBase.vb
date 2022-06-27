' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇  新規作成
'   0.1      2013/03/01  (NES)小林  操作ログ機能を追加
'   0.2      2013/05/13  (NES)小林  デ集クライアント試供アプリ化
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports System.Deployment.Application
Imports System.Reflection
Imports System.Text

Public Class FrmBase
    Inherits System.Windows.Forms.Form

    ''' <summary>操作ログファイルの部分名称</summary>
    Private Const sOpLogName As String = "Operation"

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

    End Sub

    ' Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    ' メモ : 以下のプロシージャは、Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使って変更してください。  
    ' コード エディタを使って変更しないでください。
    Friend WithEvents timTimer As System.Windows.Forms.Timer
    Protected WithEvents lblTitle As System.Windows.Forms.Label
    Public WithEvents pnlBodyBase As System.Windows.Forms.Panel
    Public WithEvents lblToday As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblToday = New System.Windows.Forms.Label
        Me.pnlBodyBase = New System.Windows.Forms.Panel
        Me.timTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 32)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(1014, 56)
        Me.lblTitle.TabIndex = 1
        Me.lblTitle.Text = "Title"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblToday.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblToday.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblToday.Location = New System.Drawing.Point(0, 0)
        Me.lblToday.Name = "lblToday"
        Me.lblToday.Size = New System.Drawing.Size(1014, 32)
        Me.lblToday.TabIndex = 0
        Me.lblToday.Text = "YYYY/MM/DD(Ｎ)　hh:mm"
        Me.lblToday.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlBodyBase.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlBodyBase.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.pnlBodyBase.Location = New System.Drawing.Point(0, 88)
        Me.pnlBodyBase.Name = "pnlBodyBase"
        Me.pnlBodyBase.Size = New System.Drawing.Size(1014, 656)
        Me.pnlBodyBase.TabIndex = 2
        '
        'timTimer
        '
        '
        'FrmBase
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Controls.Add(Me.lblToday)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.pnlBodyBase)
        Me.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmBase"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.ResumeLayout(False)

    End Sub

#End Region

    ' <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< イベント

    ''' <summary>
    ''' [フォームロード]
    ''' </summary>
    Private Sub FrmBase_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'システム日時を表示する
        timTimer.Interval = 100
        timTimer.Enabled = True

        '装置名＋バージョン情報を表示する
        
        Dim sVersion As String = ""
        If ApplicationDeployment.IsNetworkDeployed Then
            Dim deploy As ApplicationDeployment
            deploy = ApplicationDeployment.CurrentDeployment

            'バージョンの、[メジャー]、[マイナ]、[ビルド]を表示する。[リビジョン] は表示しない。
            sVersion = "Ver" & deploy.CurrentVersion.ToString()
            Dim intFindPos As Integer = sVersion.LastIndexOf(".")
            sVersion = Mid(sVersion, 1, intFindPos)

        End If

        Me.Text = String.Format("{0} {1}", Lexis.FormTitle.Gen(), sVersion)

        '画面背景色（BackColor）を設定する。
        '尚、マスタバージョン画面、プログラムバージョン画面については、
        '駅名ボタンに処理に応じて色をつける必要があるため、
        '各画面にて背景色を設定する。
        If Me.Name <> "FrmMstDispVersion" And Me.Name <> "FrmPrgDispVersion" Then
            LfSetBackColor(Me)
        End If
    End Sub

    ''' <summary>
    ''' [Timer.Tickイベント]
    ''' </summary>
    Private Sub timTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles timTimer.Tick

        timTimer.Interval = 1000

        'システム日時を表示する
        Dim dNow As DateTime
        dNow = Now
        lblToday.Text = dNow.ToString("yyyy/MM/dd(ddd)  HH:mm")
    End Sub

    ' <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< メソッド

    ''' <summary>
    ''' [指定コントロール内全コントロール取得]
    ''' </summary>
    ''' <param name="top">対象コントロール</param>
    ''' <returns>配置されているコントロール配列</returns>
    Public Shared Function BaseGetAllControls(ByVal top As Control) As Control()
        Dim buf As ArrayList = New ArrayList
        For Each c As Control In top.Controls
            buf.Add(c)
            buf.AddRange(BaseGetAllControls(c))
        Next
        Return CType(buf.ToArray(GetType(Control)), Control())
    End Function

    ''' <summary>
    ''' [指定コントロール内全コントロールEnable=False]
    ''' </summary>
    ''' <param name="ctl">設定対象画面コントロール</param>
    ''' <param name="bLabel">ラベルが含まれている場合、ラベルも対象とする場合、True。対象としない場合False(ﾃﾞﾌｫﾙﾄ)。</param>
    Public Shared Sub BaseCtlDisabled(ByVal ctl As Control, Optional ByVal bLabel As Boolean = False)
        Dim all As Control() = BaseGetAllControls(ctl)
        For Each c As Control In all
            Try
                If TypeOf c Is Label Then
                    If bLabel Then
                        c.Enabled = False
                    End If
                ElseIf TypeOf c Is Panel Then
                ElseIf TypeOf c Is GroupBox Then
                Else
                    c.Enabled = False
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub

    ''' <summary>
    ''' [指定コントロール内全コントロールEnable=True]
    ''' </summary>
    ''' <param name="ctl">設定対象画面コントロール</param>
    Public Shared Sub BaseCtlEnabled(ByVal ctl As Control)
        Dim all As Control() = BaseGetAllControls(ctl)
        For Each c As Control In all
            Try
                c.Enabled = True
            Catch ex As Exception
            End Try
        Next
    End Sub

    ''' <summary>
    ''' カーソル待ち
    ''' </summary>
    ''' <param name="bWait">true:待ち開始　false:待ち終了</param>
    ''' <remarks>カーソルが砂時計になる</remarks>
    Public Sub LfWaitCursor(Optional ByVal bWait As Boolean = True)
        If bWait = True Then
            Me.Cursor = Cursors.WaitCursor
        Else
            Me.Cursor = Cursors.Default
        End If
    End Sub

    ''' <summary>
    ''' [ベース画面背景色設定]
    ''' 継承先の画面内にあるコントロール（制限あり）の背景色を設定する。
    ''' </summary>
    ''' <param name="ctl">設定対象画面コントロール</param>
    Private Shared Sub LfSetBackColor(ByVal ctl As Control)
        LfSetBackColorCore(ctl)
        Dim all As Control() = BaseGetAllControls(ctl)
        For Each c As Control In all
            LfSetBackColorCore(c)
        Next
    End Sub

    ''' <summary>
    ''' [指定コントロール背景色設定]
    ''' 対象コントロールの背景色を設定する。
    ''' 但し、対象コントロールの種類制限あり（コード内参照）。
    ''' 別途、共通以外で設定する場合は各画面にて処理すること。
    ''' </summary>
    ''' <param name="ctl">対象コントロール</param>
    Private Shared Sub LfSetBackColorCore(ByVal ctl As Control)
        Dim bFlg As Boolean = False
        If TypeOf ctl Is Button Then
            ctl.BackColor = Config.ButtonColor
        Else
            '背景色を設定するコントロール
            If TypeOf ctl Is Form Then bFlg = True
            If TypeOf ctl Is Panel Then bFlg = True
            If TypeOf ctl Is GroupBox Then bFlg = True
            If TypeOf ctl Is Label Then bFlg = True
            If TypeOf ctl Is RadioButton Then bFlg = True
            If TypeOf ctl Is TabPage Then bFlg = True
            If bFlg Then
                Try
                    ctl.BackColor = Config.BackgroundColor
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                End Try
            End If
        End If
    End Sub

    ''' <summary>
    ''' カーソル待ち
    ''' </summary>
    ''' <param name="bWait">true:待ち開始　false:待ち終了</param>
    ''' <remarks>カーソルが砂時計になる</remarks>
    Protected Sub waitCursor(Optional ByVal bWait As Boolean = True)

        If bWait = True Then
            Me.Cursor = Cursors.WaitCursor
            Me.Enabled = False
        Else
            Me.Cursor = Cursors.Default
            Me.Enabled = True
        End If

    End Sub

    ''' <summary>
    ''' 任意文言で操作履歴を記録する。
    ''' </summary>
    ''' <param name="oSentence">記録文言</param>
    ''' <param name="args">0個以上の書式設定対象オブジェクトを含んだ Object配列</param>
    Public Shared Sub LogOperation(ByVal oSentence As Sentence, ByVal ParamArray args As Object())
        Log.Extra(sOpLogName, New StackTrace(0, True).GetFrame(1).GetMethod(), oSentence.Gen(args))
    End Sub

    ''' <summary>
    ''' 子画面の操作履歴を記録する。
    ''' </summary>
    ''' <param name="oSender">イベント送信元のオブジェクト</param>
    ''' <param name="oEventArgs">イベントの付属データ</param>
    ''' <param name="sFormTitle">子画面のタイトル</param>
    Public Shared Sub LogOperation(ByVal oSender As Object, ByVal oEventArgs As System.EventArgs, ByVal sFormTitle As String)
        LogOperationCore(New StackTrace(0, True).GetFrame(1).GetMethod(), oSender, oEventArgs, sFormTitle & Lexis.DialogSuffix.Gen())
    End Sub

    ''' <summary>
    ''' 操作履歴を記録する。
    ''' </summary>
    ''' <param name="oSender">イベント送信元のオブジェクト</param>
    ''' <param name="oEventArgs">イベントの付属データ</param>
    Protected Sub LogOperation(ByVal oSender As Object, ByVal oEventArgs As System.EventArgs)
        LogOperationCore(New StackTrace(0, True).GetFrame(1).GetMethod(), oSender, oEventArgs, lblTitle.Text & Lexis.WindowSuffix.Gen())
    End Sub

    ''' <summary>
    ''' 操作履歴を記録する。
    ''' </summary>
    ''' <param name="oSender">イベント送信元のオブジェクト</param>
    ''' <param name="oEventArgs">イベントの付属データ</param>
    ''' <param name="sFormTitle">画面タイトル</param>
    Private Shared Sub LogOperationCore(ByVal oCaller As MethodBase, ByVal oSender As Object, ByVal oEventArgs As System.EventArgs, ByVal sFormTitle As String)
        If TypeOf oSender Is Control Then
            'TODO: StackTraceから呼び元のMethodNameを取得し、それが
            'oSender.GetType().GetEvent("Foo").GetRaiseMethod()と
            '一致するかチェックする。そして、一致する場合のみ、
            '専用文言（Lexis.SenderTypeNameFoo）を使った
            '記録を行うようにする。
            Dim oControl As Control = CType(oSender, Control)
            Select Case True
                Case TypeOf oSender Is DateTimePicker
                    Dim oDateTimePicker As DateTimePicker = CType(oSender, DateTimePicker)
                    Dim oValue As DateTime = oDateTimePicker.Value
                    Log.Extra(sOpLogName, oCaller, Lexis.DateTimePickerValueChanged.Gen(sFormTitle, oControl.Name, oValue.ToString("yyyy/MM/dd HH:mm:ss")))
                Case TypeOf oSender Is ComboBox
                    Dim oComboBox As ComboBox = CType(oSender, ComboBox)
                    Dim oSelection As Object = oComboBox.SelectedItem
                    If oSelection IsNot Nothing Then
                        Log.Extra(sOpLogName, oCaller, Lexis.ComboBoxSelectionChanged.Gen(sFormTitle, oControl.Name,  oSelection.ToString()))
                    Else
                        Log.Extra(sOpLogName, oCaller, Lexis.ComboBoxSelectionChangedToNothing.Gen(sFormTitle, oControl.Name))
                    End If
                Case TypeOf oSender Is Button
                    Log.Extra(sOpLogName, oCaller, Lexis.ButtonClicked.Gen(sFormTitle, oControl.Name))
                Case Else
                    Log.Extra(sOpLogName, oCaller, Lexis.SomeControlInvoked.Gen(sFormTitle, oControl.Name, oControl.GetType().ToString()))
            End Select
        Else
            Log.Fatal("The method called with invalid argument.")
        End If
    End Sub

End Class
