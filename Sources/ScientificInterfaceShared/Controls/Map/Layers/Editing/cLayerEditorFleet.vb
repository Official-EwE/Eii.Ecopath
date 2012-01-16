#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports selections of fleets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorFleet
        Inherits cLayerEditorTwoState

#Region " Construction "

        Public Sub New()
            Me.New(GetType(ucLayerEditorFleet))
        End Sub

        Public Sub New(ByVal t As Type)
            MyBase.New(t)
            Me.CellValue = 1
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the index of the Ecopath fleet to filter by.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Fleet() As Integer
            Get
                Dim layer As cLayerBundle = DirectCast(Me.Layer, cLayerBundle)
                Return layer.iLayer
            End Get
            Set(ByVal value As Integer)
                Dim layer As cLayerBundle = DirectCast(Me.Layer, cLayerBundle)
                ' Will fleet index change?
                If value <> layer.iLayer Then
                    ' #Yes: update index in the underlying layer collector
                    layer.iLayer = value
                    ' Force map update
                    Me.Layer.Update(cLayer.eChangeFlags.Map, False)
                End If
            End Set
        End Property

#End Region ' Public interfaces

    End Class

End Namespace