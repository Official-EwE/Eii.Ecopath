' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports selections of ports for fleets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorPorts
        Inherits cLayerEditorTwoState
        Implements IFleetFilter

#Region " Construction "

        Public Sub New()
            Me.New(GetType(ucLayerEditorPort))
        End Sub

        Public Sub New(t As Type)
            MyBase.New(t, True)
            Me.CellValue = 1
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        Public Event OnFilterChanged(sender As IContentFilter) Implements IFleetFilter.FilterChanged

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
                value = Math.Max(0, Math.Min(Me.UIContext.Core.nFleets, value))
                ' Will fleet index change?
                If (value <> layer.iLayer) Then
                    ' #Yes: update index in the underlying layer collector
                    layer.iLayer = value
                    ' Force map update
                    Me.Layer.Update(cDisplayLayer.eChangeFlags.Map, False)

                    Try
                        RaiseEvent OnFilterChanged(Me)
                    Catch ex As Exception
                        ' NOP
                    End Try
                End If
            End Set
        End Property

        ''' <summary>
        ''' Overridden to set coastal cells only
        ''' </summary>
        ''' <param name="ptSet"></param>
        ''' <param name="value"></param>
        ''' <param name="e"></param>
        ''' <param name="ptClick"></param>
        Protected Overrides Function SetCellValue(ptSet As System.Drawing.Point,
                                                  value As Object,
                                                  e As System.Windows.Forms.MouseEventArgs,
                                                  ptClick As System.Drawing.Point) As Boolean

            Dim core As cCore = Me.UIContext.Core
            Dim bm As cEcospaceBasemap = core.EcospaceBasemap
            Dim depth As cEcospaceLayerDepth = bm.LayerDepth

            If depth.IsCoastalCell(ptSet.Y, ptSet.X) Then
                Return MyBase.SetCellValue(ptSet, value, e, ptClick)
            End If
            Return False

        End Function

#End Region ' Public interfaces

    End Class

End Namespace