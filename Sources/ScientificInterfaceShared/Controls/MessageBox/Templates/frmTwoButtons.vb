#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System
Imports System.Windows.Forms
Imports System.ComponentModel
Imports EwEUtils
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Controls

    Friend Class frmTwoButtons
        Inherits System.Windows.Forms.Form

        Private Const BLANKSPACE As Integer = 10
        Private m_options As cCustomMessageBox.sMessageBoxOptions = Nothing
        Private m_bUnload As Boolean = False

        Public Sub New(ByVal options As cCustomMessageBox.sMessageBoxOptions)

            MyBase.New()
            Me.InitializeComponent()

            Me.m_options = options
            Me.lblMessage.Text = Me.m_options.Prompt
            Me.Text = Me.m_options.Caption

            Try
                Me.picIcon.Image = options.Icon.ToBitmap
            Catch ex As Exception

            End Try

            Select Case options.Buttons
                Case MessageBoxButtons.OKCancel
                    Me.btnOne.Text = "&Cancel"
                    Me.btnTwo.Text = "&OK"
                Case MessageBoxButtons.RetryCancel
                    Me.btnOne.Text = "&Cancel"
                    Me.btnTwo.Text = "&Retry"
                Case MessageBoxButtons.YesNo
                    Me.btnOne.Text = "&No"
                    Me.btnTwo.Text = "&Yes"
            End Select

            Me.SetUp()

        End Sub

        Private Sub SetUp()
            Dim ppntLoc As Point

            Me.AlignPicLabel()

            'MAKE FORM SIZE
            If picIcon.Image Is Nothing Then
                With Me
                    .Width = (BLANKSPACE * 2) + lblMessage.Width
                    .Height = (BLANKSPACE * 4) + lblMessage.Height + btnOne.Height
                End With
            Else
                With Me
                    .Width = (BLANKSPACE * 3) + lblMessage.Width + picIcon.Width
                    If lblMessage.Height > picIcon.Height Then
                        .Height = (BLANKSPACE * 5) + lblMessage.Height + btnOne.Height
                    Else
                        .Height = (BLANKSPACE * 5) + picIcon.Height + btnOne.Height
                    End If
                End With
            End If

            'MAKE SURE FORM IS BIG ENOUGH
            Dim pszForm As New Size(192, 1120)
            If Me.Size.Width < pszForm.Width Then
                pszForm = New Size(192, Me.Height)
                Me.Size = pszForm
            End If
            If Me.Size.Height < 140 Then
                pszForm = New Size(Me.Width, 140)
                Me.Size = pszForm
            End If

            'MAKE BUTTON ALIGN
            Dim pintx As Integer = CInt((Me.Width / 2) - ((BLANKSPACE / 2) + (btnTwo.Width)))
            ppntLoc = New Point(pintx, CInt(Me.Height - ((BLANKSPACE * 2) + (btnTwo.Height * 1.5))))
            btnTwo.Location = ppntLoc
            ppntLoc = New Point(CInt(btnTwo.Left + btnTwo.Width + (BLANKSPACE / 2)), btnTwo.Top)
            btnOne.Location = ppntLoc

            Me.MaximumSize = Me.Size
        End Sub

        Private Sub AlignPicLabel()
            Dim ppntLoc As Point
            Dim ScreenSize As Size
            Dim fntFont As Font
            Dim pintLblHeight As Integer

            fntFont = lblMessage.Font
            pintLblHeight = fntFont.Height

            ScreenSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize()

            lblMessage.AutoSize = True

            ' wrap lblMessage to a 1:60 ratio
            While lblMessage.Width > (lblMessage.Height * 60)
                lblMessage.AutoSize = False
                lblMessage.Width = CInt(lblMessage.Width / 2)
                lblMessage.Height = CInt(lblMessage.Height * 2)
            End While

            If lblMessage.Height > ScreenSize.Height / 2 Then
                lblMessage.Height = CInt(ScreenSize.Height / 2)
            End If

            If picIcon.Image Is Nothing Then
                ppntLoc = New Point(BLANKSPACE, BLANKSPACE)
                lblMessage.Location = ppntLoc
            Else 'HAS AN ICON
                ppntLoc = New Point(BLANKSPACE, BLANKSPACE)
                picIcon.Location = ppntLoc
                If lblMessage.Text <> "" Then
                    If lblMessage.Height < picIcon.Height Then
                        ppntLoc = New Point((BLANKSPACE * 2) + picIcon.Width, CInt((picIcon.Top + (picIcon.Height / 2)) - (lblMessage.Height / 2)))
                    Else
                        ppntLoc = New Point((BLANKSPACE * 2) + picIcon.Width, BLANKSPACE)
                    End If
                    lblMessage.Location = ppntLoc
                End If
            End If

        End Sub

        Protected Overrides Sub OnFormClosing(ByVal e As FormClosingEventArgs)

            e.Cancel = (m_bUnload = False)
            MyBase.OnFormClosing(e)

        End Sub

        Protected Overrides Sub OnActivated(ByVal e As System.EventArgs)

            Static pblnFirst As Boolean
            If Not pblnFirst Then
                cSoundUtilities.PlaySound(Me.m_options.MessageBoxIcon, Me.m_options.Sound)
                pblnFirst = True
            End If

            MyBase.OnActivated(e)

        End Sub

        Private Sub btnOne_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOne.Click

            Me.Hide()

            Select Case Me.m_options.Buttons

                Case MessageBoxButtons.OKCancel, MessageBoxButtons.RetryCancel
                    Me.m_options.Result = Forms.DialogResult.Cancel

                Case MessageBoxButtons.YesNo
                    Me.m_options.Result = Forms.DialogResult.No

            End Select

            Me.m_bUnload = True
            Me.Close()

        End Sub

        Private Sub btnTwo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTwo.Click

            Me.Hide()

            Select Case Me.m_options.Buttons

                Case MessageBoxButtons.OKCancel
                    Me.m_options.Result = Forms.DialogResult.OK

                Case MessageBoxButtons.RetryCancel
                    Me.m_options.Result = Forms.DialogResult.Retry

                Case MessageBoxButtons.YesNo
                    Me.m_options.Result = Forms.DialogResult.Yes

            End Select

            Me.m_bUnload = True
            Me.Close()

        End Sub

        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            Const WM_NCLBUTTONDBLCLK As Int32 = &HA3
            If m.Msg = WM_NCLBUTTONDBLCLK Then
                Exit Sub
            End If
            MyBase.WndProc(m)
        End Sub

#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Windows Form Designer.
            InitializeComponent()

            'Add any initialization after the InitializeComponent() call

        End Sub

        'Form overrides dispose to clean up the component list.
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        Friend WithEvents btnTwo As System.Windows.Forms.Button
        Friend WithEvents btnOne As System.Windows.Forms.Button
        Friend WithEvents picIcon As System.Windows.Forms.PictureBox
        Friend WithEvents lblMessage As System.Windows.Forms.Label
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Me.btnTwo = New System.Windows.Forms.Button
            Me.btnOne = New System.Windows.Forms.Button
            Me.picIcon = New System.Windows.Forms.PictureBox
            Me.lblMessage = New System.Windows.Forms.Label
            Me.SuspendLayout()
            '
            'btnTwo
            '
            Me.btnTwo.Location = New System.Drawing.Point(8, 64)
            Me.btnTwo.Name = "btnTwo"
            Me.btnTwo.Size = New System.Drawing.Size(72, 24)
            Me.btnTwo.TabIndex = 0
            Me.btnTwo.Text = "Button2"
            '
            'btnOne
            '
            Me.btnOne.Location = New System.Drawing.Point(88, 64)
            Me.btnOne.Name = "btnOne"
            Me.btnOne.Size = New System.Drawing.Size(72, 24)
            Me.btnOne.TabIndex = 1
            Me.btnOne.Text = "Button1"
            '
            'picIcon
            '
            Me.picIcon.Location = New System.Drawing.Point(16, 16)
            Me.picIcon.Name = "picIcon"
            Me.picIcon.Size = New System.Drawing.Size(32, 32)
            Me.picIcon.TabIndex = 2
            Me.picIcon.TabStop = False
            '
            'lblMessage
            '
            Me.lblMessage.AutoSize = True
            Me.lblMessage.Location = New System.Drawing.Point(80, 16)
            Me.lblMessage.Name = "lblMessage"
            Me.lblMessage.Size = New System.Drawing.Size(38, 16)
            Me.lblMessage.TabIndex = 3
            Me.lblMessage.Text = "Label1"
            '
            'frmTwoButtons
            '
            Me.AcceptButton = Me.btnTwo
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(186, 96)
            Me.Controls.Add(Me.lblMessage)
            Me.Controls.Add(Me.picIcon)
            Me.Controls.Add(Me.btnOne)
            Me.Controls.Add(Me.btnTwo)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
            Me.Name = "frmTwoButtons"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "frmTwoButtons"
            Me.TopMost = True
            Me.ResumeLayout(False)

        End Sub

#End Region

    End Class

End Namespace ' Controls
