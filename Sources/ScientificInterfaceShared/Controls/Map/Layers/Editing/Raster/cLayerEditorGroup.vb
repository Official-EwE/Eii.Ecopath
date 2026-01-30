' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports selections of groups.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorGroup
        Inherits cLayerEditorRange
        Implements IGroupFilter

#Region " Construction "

        Public Sub New()
            Me.New(GetType(ucLayerEditorGroup))
        End Sub

        Public Sub New(t As Type)
            MyBase.New(t)
            Me.CellValue = 1
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        Public Event OnFilterChanged(sender As IContentFilter) Implements IGroupFilter.FilterChanged

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the index of the Ecopath group to filter by.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Group() As Integer _
            Implements IGroupFilter.Group
            Get
                Dim layerCore As cDisplayLayerRasterBundle = DirectCast(Me.Layer, cDisplayLayerRasterBundle)
                Return layerCore.iLayer
            End Get
            Set(value As Integer)
                Dim layerCore As cDisplayLayerRasterBundle = DirectCast(Me.Layer, cDisplayLayerRasterBundle)
                ' Will Group index change?
                If (value <> layerCore.iLayer) Then
                    ' #Yes: update Group index in the underlying Ecospace layer
                    layerCore.iLayer = value
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

#End Region ' Public interfaces

    End Class

End Namespace