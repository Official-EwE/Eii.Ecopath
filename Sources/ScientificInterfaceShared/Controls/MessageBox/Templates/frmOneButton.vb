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

    Friend Class frmOneButton
        Inherits Form

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
                picIcon.Image = Me.m_options.Icon.ToBitmap
            Catch ex As Exception
            End Try

            Me.SetUp()

        End Sub

#Region " Overloads "

        Protected Overrides Sub OnActivated(ByVal e As System.EventArgs)

            Static pblnFirst As Boolean
            If Not pblnFirst Then
                cSoundUtilities.PlaySound(Me.m_options.MessageBoxIcon, Me.m_options.Sound)
                pblnFirst = True
            End If

            MyBase.OnActivated(e)

        End Sub

        Protected Overrides Sub OnFormClosing(ByVal e As FormClosingEventArgs)

            e.Cancel = (m_bUnload = False)
            MyBase.OnFormClosing(e)

        End Sub

        Protected Overrides Sub WndProc(ByRef m As Message)
            'Keep form from expanding when titlebar is double clicked
            Const WM_NCLBUTTONDBLCLK As Int32 = &HA3
            If m.Msg = WM_NCLBUTTONDBLCLK Then
                Exit Sub
            End If
            MyBase.WndProc(m)
        End Sub

#End Region ' Overloads

#Region " Events "

        Private Sub btnOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles btnOne.Click

            Me.Hide()
            Me.m_options.Result = Forms.DialogResult.OK
            Me.m_bUnload = True
            Me.Close()

        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub SetUp()

            Dim ppntLoc As Point

            Me.AlignPicLabel()

            'MAKE FORM SIZE
            If picIcon.Image Is Nothing Then
                With Me
                    .Width = (BLANKSPACE * 2) + lblMessage.Width
                    .Height = (BLANKSPACE * 4) + lblMessage.Height + btnOne.Height
                End With
                If Me.Width < (BLANKSPACE * 2) + btnOne.Width Then
                    Me.Width = btnOne.Width + (BLANKSPACE * 2)
                End If
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

            'ALIGN BUTTON
            If picIcon.Image Is Nothing Then
                ppntLoc = New Point(CInt((Me.DisplayRectangle.Width / 2) - (btnOne.Width / 2)), (BLANKSPACE * 2) + lblMessage.Height)
                btnOne.Location = ppntLoc
                picIcon.Visible = False
                Me.lblMessage.Location = New Point((Me.DisplayRectangle.Width \ 2) - (Me.lblMessage.Width \ 2), Me.lblMessage.Location.Y)
                If Me.DisplayRectangle.Height < btnOne.Bottom + BLANKSPACE Then
                    Dim pintDif As Integer = Me.Height - Me.DisplayRectangle.Height
                    Me.Height = btnOne.Bottom + BLANKSPACE + pintDif
                End If
            Else 'HAS AN ICON
                If lblMessage.Height > picIcon.Height Then
                    ppntLoc = New Point(CInt((Me.Width / 2) - (btnOne.Width / 2)), (BLANKSPACE * 2) + lblMessage.Height)
                Else
                    ppntLoc = New Point(CInt((Me.Width / 2) - (btnOne.Width / 2)), (BLANKSPACE * 2) + picIcon.Height)
                End If
                btnOne.Location = ppntLoc
            End If

            Me.MaximumSize = Me.Size
        End Sub

        Private Sub AlignPicLabel()
            Dim ppntLoc As Point
            Dim ScreenSize As Size
            Dim fntFont As Font
            Dim pintLblHeight As Integer
            'Dim pintLblWidth As Integer

            fntFont = lblMessage.Font
            pintLblHeight = fntFont.Height

            ScreenSize = SystemInformation.PrimaryMonitorSize()

            lblMessage.AutoSize = True

            ' wrap lblMessage to a 1:60 ratio
            While lblMessage.Width > (lblMessage.Height * 60)
                lblMessage.AutoSize = False
                lblMessage.Width = CInt(lblMessage.Width / 2)
                lblMessage.Height = CInt(lblMessage.Height * 2)
            End While

            'If lblMessage.Text.Length > 80 Then
            '    lblMessage.Text = ResizeText(lblMessage.Text)
            '    pintLblHeight = ((CInt(Len(lblMessage.Text) / 80)) * pintLblHeight) + 1
            '    pintLblWidth = lblMessage.Width
            '    lblMessage.AutoSize = False
            '    lblMessage.Height = pintLblHeight
            '    lblMessage.Width = pintLblWidth
            'End If

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
                        lblMessage.Location = ppntLoc
                    Else
                        ppntLoc = New Point((BLANKSPACE * 2) + picIcon.Width, BLANKSPACE)
                        lblMessage.Location = ppntLoc
                    End If
                End If
            End If

        End Sub

#End Region ' Internals

#Region " Windows Form Designer generated code "

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
        Private WithEvents lblMessage As System.Windows.Forms.Label

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        Friend WithEvents picIcon As PictureBox
        Friend WithEvents btnOne As Button
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Me.btnOne = New System.Windows.Forms.Button
            Me.lblMessage = New System.Windows.Forms.Label
            Me.picIcon = New System.Windows.Forms.PictureBox
            CType(Me.picIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'btnOne
            '
            Me.btnOne.Location = New System.Drawing.Point(8, 72)
            Me.btnOne.Name = "btnOne"
            Me.btnOne.Size = New System.Drawing.Size(72, 24)
            Me.btnOne.TabIndex = 0
            Me.btnOne.Text = "&OK"
            '
            'lblMessage
            '
            Me.lblMessage.AutoSize = True
            Me.lblMessage.Location = New System.Drawing.Point(64, 16)
            Me.lblMessage.Name = "lblMessage"
            Me.lblMessage.Size = New System.Drawing.Size(33, 13)
            Me.lblMessage.TabIndex = 1
            Me.lblMessage.Text = "Label"
            '
            'picIcon
            '
            Me.picIcon.Location = New System.Drawing.Point(16, 24)
            Me.picIcon.Name = "picIcon"
            Me.picIcon.Size = New System.Drawing.Size(32, 32)
            Me.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
            Me.picIcon.TabIndex = 2
            Me.picIcon.TabStop = False
            '
            'frmOneButton
            '
            Me.AcceptButton = Me.btnOne
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(104, 106)
            Me.ControlBox = False
            Me.Controls.Add(Me.picIcon)
            Me.Controls.Add(Me.lblMessage)
            Me.Controls.Add(Me.btnOne)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmOneButton"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "frmOneButton"
            Me.TopMost = True
            CType(Me.picIcon, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

    End Class

End Namespace ' Controls
