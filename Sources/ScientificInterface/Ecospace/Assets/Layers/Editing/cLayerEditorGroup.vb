#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing
Imports System.Windows.Forms
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports EwEUtils.Core

#End Region ' Imports 

Namespace Ecospace.Basemap.Layers

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
                Dim layerCore As ICoreGroupFilter = DirectCast(Me.Layer.Data, ICoreGroupFilter)
                Return layerCore.Group
            End Get
            Set(ByVal value As Integer)
                Dim layer As ICoreGroupFilter = DirectCast(Me.Layer.Data, ICoreGroupFilter)
                ' Will Group index change?
                If value <> layer.Group Then
                    ' #Yes: update Group index in the underlying Ecospace layer
                    layer.Group = value
                    ' Force map update
                    Me.Layer.Update(cLayer.eChangeFlags.Map, False)
                End If
            End Set
        End Property

#End Region ' Public interfaces

    End Class

End Namespace