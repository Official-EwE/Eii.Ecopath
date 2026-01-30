' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On


Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecosim Vulnerabilities interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmVulnerabilities

#Region " Private vars "

        Private m_cmdEstimateVs As cCommand = Nothing

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            MyBase.New(New gridVulnerabilities())
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Overloads "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If Me.UIContext Is Nothing Then Return

            Me.m_cmdEstimateVs = Me.CommandHandler.GetCommand("EstimateVs")
            If (Me.m_cmdEstimateVs IsNot Nothing) Then
                Me.m_cmdEstimateVs.AddControl(Me.m_tsbEstimateVs)
            End If

#If Not DEBUG Then
            ' Remove estimate V's from release version while under development
            Me.m_tsbEstimateVs.Visible = False
#End If

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            If (Me.m_cmdEstimateVs IsNot Nothing) Then
                Me.m_cmdEstimateVs.RemoveControl(Me.m_tsbEstimateVs)
            End If
            MyBase.OnFormClosed(e)
        End Sub

        Private Sub OnScaleVtoTL(sender As Object, e As EventArgs) Handles m_tsbnScaleVtoTL.Click
            Dim dlg As New dlgScaleVs(Me.UIContext)
            If (dlg.ShowDialog(Me.UIContext.FormMain) = DialogResult.OK) Then
                'NOP
            End If
        End Sub

#End Region ' Overloads

    End Class

End Namespace

