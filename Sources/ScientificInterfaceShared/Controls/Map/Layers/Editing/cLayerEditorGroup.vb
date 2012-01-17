#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports selections of groups.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorGroup
        Inherits cLayerEditorRange

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
        ''' Get/set the index of the Ecopath group to filter by.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Group() As Integer
            Get
                Dim layerCore As cLayerBundle = DirectCast(Me.Layer, cLayerBundle)
                Return layerCore.iLayer + 1
            End Get
            Set(ByVal value As Integer)
                Dim layerCore As cLayerBundle = DirectCast(Me.Layer, cLayerBundle)
                value -= 1
                ' Will Group index change?
                If value <> layerCore.iLayer Then
                    ' #Yes: update Group index in the underlying Ecospace layer
                    layerCore.iLayer = value
                    ' Force map update
                    Me.Layer.Update(cLayer.eChangeFlags.Map, False)
                End If
            End Set
        End Property

#End Region ' Public interfaces

    End Class

End Namespace