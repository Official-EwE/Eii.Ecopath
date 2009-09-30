#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing
Imports System.Windows.Forms
Imports ScientificInterface.Ecospace.Basemap.Layers

#End Region ' Imports 

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports selections of fleets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorFleet
        Inherits cLayerEditorTwoState

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorFleet))
            Me.CellValue = 1
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the index of the Ecopath group whose migration data
        ''' is being edited.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Fleet() As Integer
            Get
                Dim layerCore As cEcospaceLayerPort = DirectCast(Me.Layer.Data, cEcospaceLayerPort)
                Return layerCore.Fleet
            End Get
            Set(ByVal value As Integer)
                Dim layer As cEcospaceLayerPort = DirectCast(Me.Layer.Data, cEcospaceLayerPort)
                ' Will fleet index change?
                If value <> layer.Fleet Then
                    ' #Yes: update fleet index in the underlying Ecospace layer
                    layer.Fleet = value
                    ' Force map update
                    Me.Layer.Update(cLayer.eChangeFlags.Map)
                End If
            End Set
        End Property

#End Region ' Public interfaces

    End Class

End Namespace