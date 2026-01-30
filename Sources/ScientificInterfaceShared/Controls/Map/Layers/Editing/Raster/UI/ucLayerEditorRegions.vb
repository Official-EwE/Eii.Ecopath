' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    Public Class ucLayerEditorRegion

        Private m_mhSpace As cMessageHandler = Nothing

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.UpdateContent(Me.Editor)

            Me.m_mhSpace = New cMessageHandler(AddressOf Me.OnCoreMessage, eCoreComponentType.Ecospace, eMessageType.DataValidation, Me.UIContext.SyncObject)
            Me.UIContext.Core.Messages.AddMessageHandler(Me.m_mhSpace)
#If DEBUG Then
            Me.m_mhSpace.Name = "ucLayerEditorRegions"
#End If

        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso Me.components IsNot Nothing Then
                    Me.components.Dispose()
                    Me.UIContext.Core.Messages.RemoveMessageHandler(Me.m_mhSpace)
                    Me.m_mhSpace.Dispose()
                    Me.m_mhSpace = Nothing
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Public Overrides Sub UpdateContent(editor As cLayerEditorRaster)

            MyBase.UpdateContent(editor)
            If (Me.UIContext Is Nothing) Then Return

            ' Sanity checks
            If (editor Is Nothing) Then Return
            If (editor.Layer Is Nothing) Then Return
            If (Me.m_nudRegion Is Nothing) Then Return

            editor.CellValueMax = editor.Layer.Data.MaxValue
            Dim decMax As Decimal = CDec(editor.CellValueMax)
            Dim decVal As Decimal = CDec(editor.CellValue)

            ' Set control value
            Dim val As Decimal = Math.Min(decMax, decVal)
            If (val > Me.m_nudRegion.Maximum) Then
                Me.m_nudRegion.Maximum = decMax
                Me.m_nudRegion.Value = decVal
            Else
                Me.m_nudRegion.Value = decVal
                Me.m_nudRegion.Maximum = decMax
            End If

        End Sub

#End Region ' Overrides

#Region " Event handlers "

        Private Sub OnDrawRegionChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_nudRegion.ValueChanged

            If (Me.UIContext Is Nothing) Then Return

            Me.Editor.CellValue = CInt(Me.m_nudRegion.Value)

        End Sub

        Private Sub OnCoreMessage(ByRef msg As cMessage)
            Try
                If (msg.DataType = eDataTypes.EcospaceModelParameter) And (msg.Type = eMessageType.DataValidation) Then
                    Me.UpdateContent(Me.Editor)
                End If
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Event handlers

    End Class

End Namespace
