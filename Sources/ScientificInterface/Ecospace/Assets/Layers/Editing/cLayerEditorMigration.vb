'==============================================================================
'
' $Log: cLayerEditorMigration.vb,v $
' Revision 1.1  2008/11/04 04:40:16  jeroens
' Split into separate files, moved
'
'==============================================================================

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
    ''' Layer editor that supports manual modification of Ecospace migration data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorMigration
        Inherits cLayerEditor

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorMigration))
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
                ' ToDo: perform sanity checks here?
                Dim layerCore As cEcospaceLayerMigration = DirectCast(Me.Layer.Data, cEcospaceLayerMigration)
                Return layerCore.Group
            End Get
            Set(ByVal value As Integer)
                Dim layerMigration As cEcospaceLayerMigration = DirectCast(Me.Layer.Data, cEcospaceLayerMigration)
                ' Will group index change?
                If value <> layerMigration.Group Then
                    ' #Yes: update group index in the underlying Ecospace layer
                    layerMigration.Group = value
                    ' Force map update
                    Me.Layer.Update(cLayer.eChangeFlags.Map)
                End If
            End Set
        End Property

#End Region ' Public interfaces

    End Class

End Namespace