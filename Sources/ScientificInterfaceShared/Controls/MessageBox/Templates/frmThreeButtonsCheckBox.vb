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

    Friend Class frmThreeButtonsCheckBox
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
                ' NOP
            End Try

            Select Case Me.m_options.Buttons
                Case MessageBoxButtons.AbortRetryIgnore
                    btnOne.Text = "&Ignore"
                    btnTwo.Text = "&Retry"
                    btnThree.Text = "&Abort"
                Case MessageBoxButtons.YesNoCancel
                    btnOne.Text = "&Cancel"
                    btnTwo.Text = "&No"
                    btnThree.Text = "&Yes"
            End Select

            Me.SetUp()

        End Sub

        Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)

            e.Cancel = (m_bUnload = False)
            MyBase.OnFormClosing(e)

        End Sub

        Private Sub btnOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles btnOne.Click

            Me.Hide()

            Select Case Me.m_options.Buttons
                Case MessageBoxButtons.AbortRetryIgnore
                    Me.m_options.Result = Forms.DialogResult.Ignore
                Case MessageBoxButtons.YesNoCancel
                    Me.m_options.Result = Forms.DialogResult.Cancel
            End Select

            Me.m_options.CanSuppress = False
            Me.m_bUnload = True
            Me.Close()

        End Sub

        Private Sub btnTwo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTwo.Click

            Me.Hide()

            Select Case Me.m_options.Buttons
                Case MessageBoxButtons.AbortRetryIgnore
                    Me.m_options.Result = Forms.DialogResult.Retry
                Case MessageBoxButtons.YesNoCancel
                    Me.m_options.Result = Forms.DialogResult.No
            End Select

            Me.m_bUnload = True
            Me.Close()

        End Sub

        Private Sub btnThree_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnThree.Click

            Me.Hide()

            Select Case Me.m_options.Buttons
                Case MessageBoxButtons.AbortRetryIgnore
                    Me.m_options.Result = Forms.DialogResult.Ignore
                Case MessageBoxButtons.YesNoCancel
                    Me.m_options.Result = Forms.DialogResult.Yes
            End Select

            Me.m_options.CanSuppress = Me.chkRemember.Checked
            Me.m_bUnload = True
            Me.Close()

        End Sub

        Protected Overrides Sub OnActivated(ByVal e As System.EventArgs)

            Static pblnFirst As Boolean
            If Not pblnFirst Then
                cSoundUtilities.PlaySound(Me.m_options.MessageBoxIcon, Me.m_options.Sound)
                pblnFirst = True
            End If

            MyBase.OnActivated(e)

        End Sub

        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            Const WM_NCLBUTTONDBLCLK As Int32 = &HA3
            If m.Msg = WM_NCLBUTTONDBLCLK Then
                Exit Sub
            End If
            MyBase.WndProc(m)
        End Sub


        Private Sub SetUp()
            Dim ppntLoc As Point

            Me.AlignPicLabel()

            'MAKE FORM SIZE
            If picIcon.Image Is Nothing Then
                With Me
                    .Width = (BLANKSPACE * 2) + lblMessage.Width
                    .Height = (BLANKSPACE * 5) + lblMessage.Height + btnOne.Height + chkRemember.Height
                End With
                picIcon.Visible = False
            Else
                With Me
                    .Width = (BLANKSPACE * 3) + lblMessage.Width + picIcon.Width
                    If lblMessage.Height > picIcon.Height Then
                        .Height = (BLANKSPACE * 6) + lblMessage.Height + btnOne.Height + chkRemember.Height
                    Else
                        .Height = (BLANKSPACE * 6) + picIcon.Height + btnOne.Height + chkRemember.Height
                    End If
                End With
            End If

            'MAKE SURE FORM IS BIG ENOUGH
            Dim pszForm As New Size(262, 113)
            If Me.Size.Width < pszForm.Width Then
                pszForm = New Size(262, Me.Height)
                Me.Size = pszForm
            End If
            If Me.Size.Height < 140 Then
                pszForm = New Size(Me.Width, 140)
                Me.Size = pszForm
            End If

            'ALIGN BUTTONS
            Dim pintx As Integer = CInt((Me.Width / 2) - ((BLANKSPACE) + (btnTwo.Width * 1.5)))
            ppntLoc = New Point(pintx, CInt(Me.Height - ((BLANKSPACE * 2) + (btnTwo.Height * 1.5))))
            btnThree.Location = ppntLoc
            ppntLoc = New Point(CInt(btnThree.Left + btnThree.Width + (BLANKSPACE / 2)), btnThree.Top)
            btnTwo.Location = ppntLoc
            ppntLoc = New Point(CInt(btnTwo.Left + btnTwo.Width + (BLANKSPACE / 2)), btnTwo.Top)
            btnOne.Location = ppntLoc

            'ALIGN CHECKBOX
            Dim pintY As Integer = (btnTwo.Top - (BLANKSPACE + chkRemember.Height))
            ppntLoc = New Point(16, pintY)
            chkRemember.Location = ppntLoc
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

#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This Me.is required by the Windows Form Designer.
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
        Friend WithEvents btnThree As System.Windows.Forms.Button
        Friend WithEvents btnTwo As System.Windows.Forms.Button
        Friend WithEvents btnOne As System.Windows.Forms.Button
        Friend WithEvents lblMessage As System.Windows.Forms.Label
        Friend WithEvents picIcon As System.Windows.Forms.PictureBox
        Friend WithEvents chkRemember As System.Windows.Forms.CheckBox
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmThreeButtonsCheckBox))
            Me.btnThree = New System.Windows.Forms.Button
            Me.btnTwo = New System.Windows.Forms.Button
            Me.btnOne = New System.Windows.Forms.Button
            Me.lblMessage = New System.Windows.Forms.Label
            Me.picIcon = New System.Windows.Forms.PictureBox
            Me.chkRemember = New System.Windows.Forms.CheckBox
            CType(Me.picIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'btnThree
            '
            resources.ApplyResources(Me.btnThree, "btnThree")
            Me.btnThree.Name = "btnThree"
            '
            'btnTwo
            '
            resources.ApplyResources(Me.btnTwo, "btnTwo")
            Me.btnTwo.Name = "btnTwo"
            '
            'btnOne
            '
            resources.ApplyResources(Me.btnOne, "btnOne")
            Me.btnOne.Name = "btnOne"
            '
            'lblMessage
            '
            resources.ApplyResources(Me.lblMessage, "lblMessage")
            Me.lblMessage.Name = "lblMessage"
            '
            'picIcon
            '
            resources.ApplyResources(Me.picIcon, "picIcon")
            Me.picIcon.Name = "picIcon"
            Me.picIcon.TabStop = False
            '
            'chkRemember
            '
            resources.ApplyResources(Me.chkRemember, "chkRemember")
            Me.chkRemember.Name = "chkRemember"
            '
            'frmThreeButtonsCheckBox
            '
            Me.AcceptButton = Me.btnThree
            resources.ApplyResources(Me, "$this")
            Me.ControlBox = False
            Me.Controls.Add(Me.chkRemember)
            Me.Controls.Add(Me.picIcon)
            Me.Controls.Add(Me.lblMessage)
            Me.Controls.Add(Me.btnOne)
            Me.Controls.Add(Me.btnTwo)
            Me.Controls.Add(Me.btnThree)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmThreeButtonsCheckBox"
            Me.ShowInTaskbar = False
            Me.TopMost = True
            CType(Me.picIcon, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

    End Class

End Namespace ' Controls
