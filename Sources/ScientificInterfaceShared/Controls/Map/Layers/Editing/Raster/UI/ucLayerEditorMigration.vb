' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace Controls.Map.Layers

    Public Class ucLayerEditorMigration

        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of ucLayerEditorMigration)()

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_btnNext.Image = My.Resources.PlayStepHS
            Me.m_btnNext.Text = ""

            Me.UpdateContent(Me.Editor)
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso Me.components IsNot Nothing Then
                    Me.components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try

        End Sub

        Public Overrides Sub UpdateContent(editor As cLayerEditorRaster)
            MyBase.UpdateContent(editor)

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_cmbMonth Is Nothing) Then Return

            If (editor IsNot Nothing) Then
                Me.m_bInUpdate = True
                Try
                    Dim layer As cEcospaceLayer = Me.Editor.Layer.Data
                    Dim grp As cEcospaceGroupInput = Me.UIContext.Core.EcospaceGroupInputs(layer.Index)

                    Me.m_cmbMonth.SelectedIndex = Math.Max(0, Me.Editor.Month - 1)
                Catch ex As Exception

                End Try
                Me.m_bInUpdate = False
            End If

        End Sub

        Protected Overloads Property Editor() As cLayerEditorMigration
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorMigration)
            End Get
            Set(editor As cLayerEditorMigration)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorMigration, "ucLayerEditorMigration connected to wrong editor class")
                ' Set
                MyBase.Editor = editor
            End Set
        End Property

#Region " Event handlers "

        Private Sub OnMonthChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbMonth.SelectedIndexChanged
            Try
                Me.Editor.Month = Me.m_cmbMonth.SelectedIndex + 1
            Catch ex As Exception
                m_logger.LogError(ex, "ucLayerMigration.OnMonthChanged()")
            End Try
        End Sub

        Private Sub OnNextMonth(sender As System.Object, e As System.EventArgs) _
            Handles m_btnNext.Click
            Try
                Me.Editor.Next()
                Me.UpdateContent(Me.Editor)
            Catch ex As Exception
                m_logger.LogError(ex, "ucLayerMigration.OnNextMonth()")
            End Try
        End Sub

#End Region ' Event handlers

    End Class

End Namespace

