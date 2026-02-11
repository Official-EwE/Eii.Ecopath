' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports selections of ports for fleets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorSailCost
        Inherits cLayerEditorRange
        Implements IFleetFilter

#Region " Construction "

        Public Sub New()
            Me.New(GetType(ucLayerEditorPort))
        End Sub

        Public Sub New(t As Type)
            MyBase.New(t)
            Me.CellValue = 1
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        Public Event FilterChanged(sender As IContentFilter) Implements IContentFilter.FilterChanged

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the index of the Ecopath fleet to filter by.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Fleet() As Integer _
            Implements IFleetFilter.Fleet
            Get
                Dim layer As cDisplayLayerRasterBundle = DirectCast(Me.Layer, cDisplayLayerRasterBundle)
                Return layer.iLayer
            End Get
            Set(value As Integer)
                Dim layer As cDisplayLayerRasterBundle = DirectCast(Me.Layer, cDisplayLayerRasterBundle)
                value = Math.Max(1, Math.Min(Me.UIContext.Core.nFleets, value))
                ' Will fleet index change?
                If (value <> layer.iLayer) Then
                    ' #Yes: update index in the underlying layer collector
                    layer.iLayer = value
                    ' Force map update
                    Me.Layer.Update(cDisplayLayer.eChangeFlags.Map, False)

                    Try
                        RaiseEvent FilterChanged(Me)
                    Catch ex As Exception

                    End Try
                End If
            End Set
        End Property

#End Region ' Public interfaces

    End Class

End Namespace