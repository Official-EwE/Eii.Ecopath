#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Controls.Map.Layers

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modification of Ecospace migration data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorMigration
        Inherits cLayerEditor

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorMigration))
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
        Public Property Group() As Integer
            Get
                Dim layerCore As cLayerBundle = DirectCast(Me.Layer, cLayerBundle)
                Return layerCore.iLayer
            End Get
            Set(ByVal value As Integer)
                Dim layerCore As cLayerBundle = DirectCast(Me.Layer, cLayerBundle)
                ' Will Group index change?
                If value <> layerCore.iLayer Then
                    ' #Yes: update Group index in the underlying Ecospace layer
                    layerCore.iLayer = value
                    ' Force map update
                    Me.Layer.Update(cLayer.eChangeFlags.Map, False)
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' This editor requires a 1 pt cursor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property CursorSize As Integer
            Get
                Return 1
            End Get
            Set(value As Integer)
                'NOP
            End Set
        End Property
#End Region ' Public interfaces

    End Class

End Namespace