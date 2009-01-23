'
' Copyright 2007 Twin Rose Software
'
' You are free to use this code and form in any way that you like, 
' personally or professionally.  If you use it in a published application,
' we ask that you link to us from your website to:
' http:'www.twinrose.net/
'
' Any redistrubition of this code must contain this entire comment text.
'
' We would also appreciate a thank you!
'  Chris Johanson
'  twinrose@twinrose.net
' 

imports System
imports System.Collections.Generic
imports System.ComponentModel
Imports System.Drawing
imports System.Text
imports System.Windows.Forms

Namespace Controls

    ''' <summary>
    ''' 
    ''' </summary>
    Public Enum eMemoryBoxResultTypes
        Yes
        YesToAll
        No
        NoToAll
        Cancel
    End Enum

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class frmMemoryBox
        Inherits Form

        ''' <summary></summary>
        Private m_mbrLastResult As eMemoryBoxResultTypes = eMemoryBoxResultTypes.Cancel
        ''' <summary></summary>
        Private m_mbrResult As eMemoryBoxResultTypes = eMemoryBoxResultTypes.Cancel

        ''' <summary>
        ''' The default constructor for MemoryBox.
        ''' </summary>
        Public Sub New()
            InitializeComponent()
        End Sub

#Region " Properties "

        Public Property LabelText() As String
            Get
                Return Me.m_lbPrompt.Text
            End Get
            Set(ByVal value As String)
                Me.m_lbPrompt.Text = value
                UpdateSize()
            End Set
        End Property

        Public Property Result() As eMemoryBoxResultTypes
            Get
                Return Me.m_mbrResult
            End Get
            Set(ByVal value As eMemoryBoxResultTypes)
                Me.m_mbrResult = value
            End Set
        End Property

#End Region ' Properties

#Region " Public Methods "

        ''' <summary>
        ''' Call this function instead of ShowDialog, to check for remembered
        ''' result.
        ''' </summary>
        Public Function ShowMemoryDialog() As eMemoryBoxResultTypes
            Me.m_mbrResult = eMemoryBoxResultTypes.Cancel
            If (m_mbrLastResult = eMemoryBoxResultTypes.NoToAll) Then
                Me.m_mbrResult = eMemoryBoxResultTypes.No
            ElseIf (m_mbrLastResult = eMemoryBoxResultTypes.YesToAll) Then
                Me.m_mbrResult = eMemoryBoxResultTypes.Yes
            Else
                MyBase.ShowDialog()
            End If
            Return Me.m_mbrResult
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strLabel"></param>
        ''' <param name="strTitle"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ShowMemoryDialog(ByVal strLabel As String, ByVal strTitle As String) As eMemoryBoxResultTypes
            Me.Text = strTitle
            LabelText = strLabel
            Return ShowMemoryDialog()
        End Function

#End Region ' Public methods

#Region " Private Methods "

        ''' <summary>
        ''' This call updates the size of the window based on certain factors,
        ''' such as if an icon is present, and the size of label.
        ''' </summary>
        Private Sub UpdateSize()

            ' ToDo: smarten up this code; resize relative to start size and -positions
            Dim newWidth As Integer = m_lbPrompt.Size.Width + 40

            ' Add the width of the icon, and some padding.
            If (m_pbIcon.Image IsNot Nothing) Then
                newWidth += m_pbIcon.Width + 20
                m_lbPrompt.Location = New Point(118, m_lbPrompt.Location.Y)
            Else
                m_lbPrompt.Location = New Point(12, m_lbPrompt.Location.Y)
                If (newWidth >= 440) Then
                    Me.Width = newWidth
                Else
                    Me.Width = 440
                    Dim newHeight As Integer = m_lbPrompt.Size.Height + 100
                    If (newHeight >= 200) Then
                        Me.Height = newHeight
                    Else
                        Me.Height = 200
                    End If
                End If
            End If
        End Sub

#End Region ' Private methods

#Region " Button events "

        Private Sub buttonYes_Click(ByVal sender As Object, ByVal e As EventArgs) Handles m_btnYes.Click
            Me.m_mbrResult = eMemoryBoxResultTypes.Yes
            Me.m_mbrLastResult = eMemoryBoxResultTypes.Yes
            Me.DialogResult = Windows.Forms.DialogResult.Yes
        End Sub

        Private Sub buttonYestoAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles m_btnYesToAll.Click
            Me.m_mbrResult = eMemoryBoxResultTypes.Yes
            Me.m_mbrLastResult = eMemoryBoxResultTypes.YesToAll
            Me.DialogResult = Windows.Forms.DialogResult.Yes
        End Sub

        Private Sub buttonNo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles m_btnNo.Click
            Me.m_mbrResult = eMemoryBoxResultTypes.No
            Me.m_mbrLastResult = eMemoryBoxResultTypes.No
            Me.DialogResult = Windows.Forms.DialogResult.No
        End Sub

        Private Sub buttonNotoAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles m_btnNotoAll.Click
            Me.m_mbrResult = eMemoryBoxResultTypes.No
            Me.m_mbrLastResult = eMemoryBoxResultTypes.NoToAll
            Me.DialogResult = Windows.Forms.DialogResult.No
        End Sub

        Private Sub buttonCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles m_btnCancel.Click
            Me.m_mbrResult = eMemoryBoxResultTypes.Cancel
            Me.m_mbrLastResult = eMemoryBoxResultTypes.Cancel
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
        End Sub

#End Region ' Button events

#Region " Windows Form Designer generated code "

        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="bDisposing">true if managed resources should be disposed otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)
            If (bDisposing And (components IsNot Nothing)) Then
                components.Dispose()
            End If
            MyBase.Dispose(Disposing)
        End Sub

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMemoryBox))
            Me.m_pbIcon = New System.Windows.Forms.PictureBox
            Me.m_btnYes = New System.Windows.Forms.Button
            Me.m_btnYesToAll = New System.Windows.Forms.Button
            Me.m_btnNo = New System.Windows.Forms.Button
            Me.m_btnNotoAll = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_lbPrompt = New System.Windows.Forms.Label
            Me.m_tlButtons = New System.Windows.Forms.TableLayoutPanel
            CType(Me.m_pbIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlButtons.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_pbIcon
            '
            resources.ApplyResources(Me.m_pbIcon, "m_pbIcon")
            Me.m_pbIcon.Name = "m_pbIcon"
            Me.m_pbIcon.TabStop = False
            '
            'm_btnYes
            '
            resources.ApplyResources(Me.m_btnYes, "m_btnYes")
            Me.m_btnYes.Name = "m_btnYes"
            Me.m_btnYes.UseVisualStyleBackColor = True
            '
            'm_btnYesToAll
            '
            resources.ApplyResources(Me.m_btnYesToAll, "m_btnYesToAll")
            Me.m_btnYesToAll.Name = "m_btnYesToAll"
            Me.m_btnYesToAll.UseVisualStyleBackColor = True
            '
            'm_btnNo
            '
            resources.ApplyResources(Me.m_btnNo, "m_btnNo")
            Me.m_btnNo.Name = "m_btnNo"
            Me.m_btnNo.UseVisualStyleBackColor = True
            '
            'm_btnNotoAll
            '
            resources.ApplyResources(Me.m_btnNotoAll, "m_btnNotoAll")
            Me.m_btnNotoAll.Name = "m_btnNotoAll"
            Me.m_btnNotoAll.UseVisualStyleBackColor = True
            '
            'm_btnCancel
            '
            resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
            Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnCancel.Name = "m_btnCancel"
            Me.m_btnCancel.UseVisualStyleBackColor = True
            '
            'm_lbPrompt
            '
            resources.ApplyResources(Me.m_lbPrompt, "m_lbPrompt")
            Me.m_lbPrompt.Name = "m_lbPrompt"
            '
            'm_tlButtons
            '
            resources.ApplyResources(Me.m_tlButtons, "m_tlButtons")
            Me.m_tlButtons.Controls.Add(Me.m_btnNo, 2, 0)
            Me.m_tlButtons.Controls.Add(Me.m_btnNotoAll, 3, 0)
            Me.m_tlButtons.Controls.Add(Me.m_btnYes, 0, 0)
            Me.m_tlButtons.Controls.Add(Me.m_btnYesToAll, 1, 0)
            Me.m_tlButtons.Controls.Add(Me.m_btnCancel, 4, 0)
            Me.m_tlButtons.Name = "m_tlButtons"
            '
            'MemoryBox
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlButtons)
            Me.Controls.Add(Me.m_lbPrompt)
            Me.Controls.Add(Me.m_pbIcon)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "MemoryBox"
            Me.ShowIcon = False
            CType(Me.m_pbIcon, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlButtons.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_pbIcon As System.Windows.Forms.PictureBox
        Private WithEvents m_btnYes As System.Windows.Forms.Button
        Private WithEvents m_btnYesToAll As System.Windows.Forms.Button
        Private WithEvents m_btnNo As System.Windows.Forms.Button
        Private WithEvents m_btnNotoAll As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_tlButtons As System.Windows.Forms.TableLayoutPanel
        Private m_lbPrompt As System.Windows.Forms.Label

#End Region

    End Class

End Namespace