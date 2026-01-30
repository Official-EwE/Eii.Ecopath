' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' InputBox alternative for Mono compliance.
    ''' </summary>
    ''' =======================================================================
    Public Class frmInputBox

#Region " Private vars "

        ''' <summary>Value maintained in the box.</summary>
        Private m_strValue As String = ""

#End Region ' Private vars

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Public Shadows Function Show(owner As IWin32Window,
                                     strPrompt As String,
                                     Optional strCaption As String = "",
                                     Optional strInitialValue As String = "") As DialogResult
            Return Me.ShowDialog(owner, strPrompt, strCaption, strInitialValue)
        End Function

        Public Shadows Function Show(strPrompt As String,
                                     Optional strCaption As String = "",
                                     Optional strInitialValue As String = "") As DialogResult
            Return Me.ShowDialog(Nothing, strPrompt, strCaption, strInitialValue)
        End Function

        Public Shadows Function ShowDialog(owner As IWin32Window,
                              strPrompt As String,
                              Optional strCaption As String = "",
                              Optional strInitialValue As String = "") As DialogResult
            Me.Text = strCaption
            Me.m_lblPrompt.Text = strPrompt
            Me.m_tbxValue.Text = strInitialValue
            Return MyBase.ShowDialog(owner)
        End Function

#Region " Events "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            Me.CenterToParent()
            MyBase.OnLoad(e)
        End Sub

        Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
            Handles m_btnOk.Click
            Me.m_strValue = Me.m_tbxValue.Text
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCancel.Click
            Me.m_strValue = ""
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnInputTextChanged(sender As Object, e As System.EventArgs) _
            Handles m_tbxValue.TextChanged
            Me.UpdateControls()
        End Sub

#End Region ' Events

#Region " Public properties "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the value entered in the input box.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Value As String
            Get
                Return Me.m_strValue
            End Get
        End Property

#End Region ' Public properties

#Region " Internals "

        Private Sub UpdateControls()
            Me.m_btnOk.Enabled = (Not String.IsNullOrWhiteSpace(Me.m_tbxValue.Text))
        End Sub

#End Region ' Internals

    End Class

End Namespace
